using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.Core.M3uDownloaders
{
    internal class PluginM3u8Downloader : M3u8Downloader
    {
        private readonly IDownloadService _pluginDownload = default!;
        private readonly M3UFileInfo m3UFileInfo = default!;

        public PluginM3u8Downloader(IDownloadService downloadService,  M3UFileInfo m3UFileInfo) : base()
        {
            this.m3UFileInfo = m3UFileInfo;
            _pluginDownload = downloadService;
        }

        public override async ValueTask Initialization(CancellationToken cancellationToken)
        {
            await _pluginDownload.Initialize(HttpClient,Headers, cancellationToken);

            if (m3UFileInfo.Key is null)
                return;

            if (m3UFileInfo.Key.BKey != null)
            {
                _pluginDownload.SetCryptData(m3UFileInfo.Key.Method, m3UFileInfo.Key.BKey, m3UFileInfo.Key.IV);
            }
            else if (m3UFileInfo.Key.Uri != null)
            {
                //不会在尝试解析密钥 因为可能不知道 他会遇到啥类型的密钥
                byte[] data = m3UFileInfo.Key.Uri.IsFile
                    ? await File.ReadAllBytesAsync(m3UFileInfo.Key.Uri.OriginalString, cancellationToken)
                    : await HttpClient.GetByteArrayAsync(m3UFileInfo.Key.Uri, Headers, cancellationToken);
                _pluginDownload.SetCryptData(m3UFileInfo.Key.Method, data, m3UFileInfo.Key.IV);
            }
        }

        protected override Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
        {
            return _pluginDownload.HandleData(stream, cancellationToken);
        }

    }
}
