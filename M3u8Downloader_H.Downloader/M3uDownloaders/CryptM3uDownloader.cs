using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8;
using M3u8Downloader_H.M3U8.Models;


namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class CryptM3uDownloader : IDownloadService
    {
        private readonly IHttpClientWrapper HttpClientWrap;
        private bool initialized = false;
        private M3UFileInfo _m3uFileinfo = null!;
        private readonly IDownloadService downloadService;
        private readonly IDownloadContext context;

        public Func<Stream, CancellationToken,Task< Stream>> HandleDataFunc { get; set; } = default!;
        public Func<string, Stream, CancellationToken, Task> WriteToFileFunc { get; set; } = default!;
        public Func<IM3uMediaInfo, IEnumerable<KeyValuePair<string, string>>?, string, CancellationToken, Task<bool>> DownloadM3uMediaInfoFunc { get; set; } = default!;

        public CryptM3uDownloader(IDownloadService downloadService, IDownloadContext context)
        {
            this.downloadService = downloadService;
            this.context = context;
            downloadService.HandleDataFunc = HandleData;
            HttpClientWrap = context.HttpClient;
        }

        public ValueTask Initialization(IM3uFileInfoSource m3UFileInfoSource, CancellationToken cancellationToken = default)
            => downloadService.Initialization(m3UFileInfoSource,cancellationToken);
   

        public async ValueTask BeforeDownload(IM3uFileInfo m3UFileInfo,CancellationToken cancellationToken)
        {
            if (initialized)
                return;

            if (m3UFileInfo.Key is null)
                throw new InvalidDataException("没有可用的密钥信息");

            M3UFileInfo m3uFileinfoTmp = (M3UFileInfo)m3UFileInfo;
            if (m3UFileInfo.Key.Uri != null && m3UFileInfo.Key.BKey == null)
            {
                try
                {              
                    byte[] data = m3UFileInfo.Key.Uri.IsFile
                        ? await File.ReadAllBytesAsync(m3UFileInfo.Key.Uri.OriginalString, cancellationToken)
                        : await HttpClientWrap.GetByteArrayAsync(m3UFileInfo.Key.Uri, context.DownloadParam.Headers, cancellationToken);

                    context.Log?.Info("获取转为base64的密钥 : {0}", Convert.ToBase64String(data));
                    m3uFileinfoTmp.Key = M3uKeyInfoHelper.GetKeyInfoInstance(m3UFileInfo.Key.Method, data, m3UFileInfo.Key.IV);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new HttpRequestException("密钥获取失败");
                }
                catch (HttpRequestException e) when(e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new HttpRequestException("获取密钥失败，没有找到任何数据",e.InnerException,e.StatusCode);
                }
            }else
            {
                m3uFileinfoTmp.Key = m3UFileInfo.Key.BKey != null
                    ? m3uFileinfoTmp.Key = M3uKeyInfoHelper.GetKeyInfoInstance(m3UFileInfo.Key)
                    : throw new InvalidDataException("密钥为空");
            }
            _m3uFileinfo = m3uFileinfoTmp;
            initialized = true;
        }


        public async Task StartDownload(IM3uFileInfoSource m3UFileInfoSource, CancellationToken cancellationToken = default)
        {
            await BeforeDownload(m3UFileInfoSource.M3uFile!, cancellationToken);
            await downloadService.StartDownload(m3UFileInfoSource, cancellationToken);
        }


        public Task<Stream> HandleData(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult(stream.AesDecrypt(_m3uFileinfo.Key.BKey, _m3uFileinfo.Key.IV));
        }


    }
}
