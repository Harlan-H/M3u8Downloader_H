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
        private M3UFileReaderManager? _m3UFileReaderManager;

        private readonly HttpClient httpClient = default!;
        private readonly IPluginManager? pluginManager;
        private readonly ILog log;
        private readonly IM3u8DownloadParam downloadParam = default!;
        public readonly IDownloaderSetting downloaderSetting = default!;

        public M3UFileReaderManager M3UFileReadManager
        {
            get
            {
                if(_m3UFileReaderManager == null)
                {
                    if (pluginManager?.M3U8FileInfoStreamService is not null)
                        _m3UFileReaderManager = new PluginM3UFileReaderManager(pluginManager?.M3U8FileInfoStreamService!, httpClient);
                    else
                        _m3UFileReaderManager = new M3UFileReaderManager(httpClient);

                    _m3UFileReaderManager.M3u8FileReader = M3u8FileReader;
                    _m3UFileReaderManager.DownloadParam = downloadParam;
                    _m3UFileReaderManager.DownloaderSetting  = downloaderSetting;
                    _m3UFileReaderManager.Log = log;
                }

                return _m3UFileReaderManager;
            }
        }


        public M3UFileReaderManager DefaultM3uFileReadManager
        {
            get
            {
                _m3UFileReaderManager ??= new()
                {
                    M3u8FileReader = new M3UFileReaderWithStream(),
                    Log = log
                };
                return _m3UFileReaderManager;
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
