using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.Plugin.PluginManagers;
using System.Net.Http;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using M3u8Downloader_H.M3U8.AttributeReader;
using System.Collections.Generic;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.M3U8
{
    public class M3u8FileInfoClient(HttpClient httpClient, IPluginManager? PluginManager,ILog log, IM3u8DownloadParam DownloadParam)
    {
        public IDownloaderSetting DownloaderSetting { get; set; } = default!;

        public M3UFileReaderManager M3UFileReadManager
        {
            get
            {
                M3UFileReaderManager m3UFileReaderManager;
                if (PluginManager?.M3U8FileInfoStreamService is not null)
                    m3UFileReaderManager = new PluginM3UFileReaderManager(PluginManager?.M3U8FileInfoStreamService!, httpClient);
                else
                    m3UFileReaderManager = new M3UFileReaderManager(httpClient);

                m3UFileReaderManager.M3u8FileReader = M3u8FileReader;
                m3UFileReaderManager.DownloadParam = DownloadParam;
                m3UFileReaderManager.DownloaderSetting  = DownloaderSetting;
                m3UFileReaderManager.Log = log;
                return m3UFileReaderManager;
            }
        }

        internal M3UFileReaderWithStream M3u8FileReader
        {
            get
            {
                M3UFileReaderWithStream m3UFileReaderWithStream ;
                if (PluginManager?.M3UFileReaderInterface is not null)
                    m3UFileReaderWithStream =  new M3UFileReaderWithPlugin(PluginManager?.M3UFileReaderInterface!);
                else
                    m3UFileReaderWithStream =  new M3UFileReaderWithStream();

                m3UFileReaderWithStream.AttributeReaders = AttributeReaders;
                return m3UFileReaderWithStream;
            }
        }


        internal IDictionary<string, IAttributeReader> AttributeReaders
        {
            get
            {
                if (PluginManager?.AttributeReaders is not null)
                    return PluginManager?.AttributeReaders!;
                else
                    return AttributeReaderRoot.Instance.AttributeReaders;
            }
        }


    }
}
