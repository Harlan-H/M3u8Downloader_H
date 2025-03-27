using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Common.Extensions;
using System.Text;
using M3u8Downloader_H.Common.M3u8;
using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class CryptM3uDownloader(HttpClient httpClient, IM3uFileInfo m3UFileInfo) : M3u8Downloader(httpClient)
    {
        private bool initialized = false;
        private readonly HttpClient httpClient = httpClient;


        public override async ValueTask Initialization(CancellationToken cancellationToken)
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
                        : await httpClient.GetByteArrayAsync(m3UFileInfo.Key.Uri, _headers, cancellationToken);

                    Log?.Info("获取到密钥:{0}", Encoding.UTF8.GetString(data));
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
            initialized = true;
        }

        protected override Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
        {   
            return stream.AesDecrypt(m3UFileInfo.Key.BKey, m3UFileInfo.Key.IV);
        }
    }
}
