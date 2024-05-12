using M3u8Downloader_H.Common.Interfaces;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Downloader.DownloaderSources;
using M3u8Downloader_H.Downloader.Extensions;
using M3u8Downloader_H.Plugin.PluginManagers;


namespace M3u8Downloader_H.Downloader
{
    public class M3uDownloaderClient
    {
        private readonly HttpClient httpClient;
        private readonly Uri url;
        private readonly M3UFileInfo m3UFileInfo;
        private readonly IPluginManager? pluginManager;
        private IDownloaderSource? _downloaderSource;


        public IDownloaderSource Downloader {  get =>_downloaderSource ??= CreateDownloadSource();  }

        public M3uDownloaderClient(HttpClient httpClient, Uri url , M3UFileInfo M3UFileInfo,  IPluginManager? PluginManager)
        {
            this.httpClient = httpClient;
            this.url = url;
            m3UFileInfo = M3UFileInfo;
            pluginManager = PluginManager;
        }

        private  DownloaderSource CreateDownloadSource()
        {
            bool isFile = m3UFileInfo.MediaFiles.Any(m => m.Uri.IsFile);
            DownloaderSource downloaderSource;
            if (isFile)
                downloaderSource = new NullSource(httpClient, pluginManager?.PluginService);
            else if (m3UFileInfo.IsVod())
                downloaderSource = new DownloadVodSource(httpClient, pluginManager?.PluginService);
            else
                downloaderSource = new DownloadLiveSource(httpClient, url, pluginManager?.PluginService);

            return downloaderSource;
        }
    }
}
