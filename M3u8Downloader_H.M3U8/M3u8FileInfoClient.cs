using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.Plugin.PluginManagers;
using System.IO;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace M3u8Downloader_H.M3U8
{
    public class M3u8FileInfoClient(HttpClient httpClient, IPluginManager? PluginManager)
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly IPluginManager? pluginManager = PluginManager;
        private M3UFileReaderManager? _m3UFileReaderManager;

        public M3UFileReaderManager M3UFileReader { get => _m3UFileReaderManager ??= CreateM3u8FileInfoManager(); }

        public M3UFileReaderManager CreateM3u8FileInfoManager()
        {
            M3UFileReaderManager m3UFileReaderManager = pluginManager?.M3U8FileInfoStreamService is not null
                ? new PluginM3UFileReaderManager(pluginManager.M3U8FileInfoStreamService, pluginManager?.M3UFileReaderInterface, httpClient, pluginManager?.AttributeReaders)
                 : new M3UFileReaderManager(pluginManager?.M3UFileReaderInterface, httpClient, pluginManager?.AttributeReaders);
            return m3UFileReaderManager;
        }
    }
}
