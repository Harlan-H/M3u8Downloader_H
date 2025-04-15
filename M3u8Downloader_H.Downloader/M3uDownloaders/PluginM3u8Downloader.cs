using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.M3u8;


namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class PluginM3u8Downloader(IDownloadService downloadService, HttpClient httpClient, IM3uFileInfo m3UFileInfo) : M3u8Downloader(httpClient)
    {
        public override async ValueTask Initialization(CancellationToken cancellationToken)
        {
            await base.Initialization(cancellationToken);
            await downloadService.Initialize(_headers, m3UFileInfo, cancellationToken);
        }
        

        protected override Stream DownloadAfter(Stream stream, string contentType, CancellationToken cancellationToken)
            =>  downloadService.HandleData(stream, cancellationToken);
        

    }
}
