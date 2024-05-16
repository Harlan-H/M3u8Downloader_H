using System.IO;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.Common.M3u8Infos;


namespace M3u8Downloader_H.Downloader.M3uDownloaders
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
            await _pluginDownload.Initialize(Headers, m3UFileInfo, cancellationToken);
        }

        protected override Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
        {
            return _pluginDownload.HandleData(stream, cancellationToken);
        }

    }
}
