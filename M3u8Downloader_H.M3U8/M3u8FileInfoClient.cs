using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using System.Net.Http;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Plugins;

namespace M3u8Downloader_H.M3U8
{
    public class M3u8FileInfoClient
    {
        private readonly HttpClient httpClient = default!;
        private readonly IPluginManager? pluginManager;
        private readonly ILog log;
        private readonly IM3u8DownloadParam downloadParam = default!;
        public readonly IDownloaderSetting downloaderSetting = default!;
        public M3UFileReaderManager M3UFileReadManager
        {
            get
            {
                M3UFileReaderManager m3UFileReaderManager;
                if (pluginManager?.M3U8FileInfoStreamService is not null)
                    m3UFileReaderManager = new PluginM3UFileReaderManager(pluginManager?.M3U8FileInfoStreamService!, httpClient);
                else
                    m3UFileReaderManager = new M3UFileReaderManager(httpClient);

                m3UFileReaderManager.M3u8FileReader = M3u8FileReader;
                m3UFileReaderManager.DownloadParam = downloadParam;
                m3UFileReaderManager.DownloaderSetting  = downloaderSetting;
                m3UFileReaderManager.Log = log;
                return m3UFileReaderManager;
            }
        }


        public M3UFileReaderManager DefaultM3uFileReadManager
        {
            get
            {
                M3UFileReaderManager m3UFileReaderManager = new()
                {
                    M3u8FileReader = new M3UFileReaderWithStream(),
                    Log = log
                };
                return m3UFileReaderManager;
            }
        }

        internal M3UFileReaderWithStream M3u8FileReader
        {
            get
            {
                M3UFileReaderWithStream m3UFileReaderWithStream;
                if (pluginManager?.M3UFileReaderInterface is not null)
                    m3UFileReaderWithStream =  new M3UFileReaderWithPlugin(pluginManager?.M3UFileReaderInterface!);
                else
                    m3UFileReaderWithStream =  new M3UFileReaderWithStream(pluginManager?.AttributeReaders);

                return m3UFileReaderWithStream;
            }
        }

        public M3u8FileInfoClient(HttpClient httpClient, IPluginManager? PluginManager, ILog log, IM3u8DownloadParam DownloadParam, IDownloaderSetting DownloaderSetting)
        {
            this.httpClient = httpClient;
            pluginManager = PluginManager;
            this.log = log;
            downloadParam = DownloadParam;
            downloaderSetting = DownloaderSetting;
        }

        public M3u8FileInfoClient(ILog log)
        {
            this.log = log;
        }
    }
}
