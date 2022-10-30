using M3u8Downloader_H.M3U8.AttributeReaderManager;
using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.Plugin;
using System.Collections.Generic;

namespace M3u8Downloader_H.Core.DownloaderPluginManagers
{
    public class PluginManger : IPluginManager
    {
        private readonly IPluginBuilder pluginBuilder;

        public IDictionary<string, IAttributeReader> AttributeReaders { get; set; } = default!;

        public IDownloadService? PluginService { get;private set; }

        public IM3u8FileInfoService? M3U8FileInfoService { get; private set; }

        public PluginManger(IPluginBuilder pluginBuilder)
        {
            this.pluginBuilder = pluginBuilder;
        }

        public void Build()
        {
            var attributeReaderManager = new AttributeReaderManager();
            pluginBuilder.SetAttributeReader(attributeReaderManager);
            AttributeReaders = attributeReaderManager.AttributeReaders;
            PluginService = pluginBuilder.CreatePluginService();
            M3U8FileInfoService = pluginBuilder.CreateM3u8FileInfoService();
        }
    }
}
