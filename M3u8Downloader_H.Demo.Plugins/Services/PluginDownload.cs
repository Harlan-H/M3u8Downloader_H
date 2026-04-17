using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Demo.Plugins.Services
{
    internal class PluginDownload : IDownloadService
    {
        private IM3uKeyInfo m3UKeyInfo = default!;
        private readonly IDownloadService downloadService;
        private readonly IDownloadContext downloadContext;

        public Func<Stream, CancellationToken, Stream> HandleDataFunc { get; set; } = default!;
        public Func<string, Stream, CancellationToken, Task> WriteToFileFunc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PluginDownload(IDownloadService downloadService, IDownloadContext downloadContext)
        {
            this.downloadService = downloadService;
            this.downloadContext = downloadContext;
            downloadService.HandleDataFunc = HandleData;
        }

        public Task<bool> DownloadM3uMediaInfo(IM3uMediaInfo m3UMediaInfo, IEnumerable<KeyValuePair<string, string>>? headers, string mediaPath, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public async ValueTask BeforeDownload(IM3uFileInfo m3UFileInfo, CancellationToken cancellationToken)
        {
            if (m3UKeyInfo is not null)
                return;

            if (m3UFileInfo.Key.Uri != null && m3UFileInfo.Key.BKey == null)
            {
                try
                {
                    byte[] data = m3UFileInfo.Key.Uri.IsFile
                        ? await File.ReadAllBytesAsync(m3UFileInfo.Key.Uri.OriginalString, cancellationToken)
                        : await downloadContext.HttpClient.GetByteArrayAsync(m3UFileInfo.Key.Uri, downloadContext.DownloadParam.Headers, cancellationToken);

                    downloadContext.Log?.Info("获取转为base64的密钥 : {0}", Convert.ToBase64String(data));
                    m3UKeyInfo = M3uKeyInfoHelper.GetKeyInfoInstance(m3UFileInfo.Key.Method, data, m3UFileInfo.Key.IV);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new HttpRequestException("密钥获取失败");
                }
                catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new HttpRequestException("获取密钥失败，没有找到任何数据", e.InnerException, e.StatusCode);
                }
            }
        }


        public async ValueTask Initialization(CancellationToken cancellationToken = default)
        {
            downloadContext.Log.Info("plugin Initialization !!!");

            await downloadService.Initialization(cancellationToken);
            downloadContext.Log.Info("plugin Initialization end !!!");
        }

        public async Task StartDownload(IM3uFileInfo m3UFileInfo, CancellationToken cancellationToken = default)
        {
            downloadContext.Log.Info("plugin StartDownload !!!");
            await BeforeDownload(m3UFileInfo, cancellationToken);
            await downloadService.StartDownload(m3UFileInfo, cancellationToken);
            downloadContext.Log.Info("plugin StartDownload  end!!!");
        }

        public Stream HandleData(Stream stream, CancellationToken cancellationToken)
        {
            downloadContext.Log.Info("plugin HandleData !!!");
            return stream.AesDecrypt(m3UKeyInfo.BKey, m3UKeyInfo.IV);
        }
    }
}
