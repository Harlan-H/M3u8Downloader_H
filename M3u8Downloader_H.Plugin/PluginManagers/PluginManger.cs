using M3u8Downloader_H.Plugin.AttributeReaderManagers;
using M3u8Downloader_H.M3U8.AttributeReaders;

namespace M3u8Downloader_H.Plugin.PluginManagers
{
    public partial class PluginManger : IPluginManager
    {
        private readonly IPluginBuilder pluginBuilder;

        public IDictionary<string, IAttributeReader> AttributeReaders { get; private set; } = default!;

        public IDownloadService? PluginService { get; private set; }

        public IM3u8FileInfoService? M3U8FileInfoService { get; private set; }

        private PluginManger(IPluginBuilder pluginBuilder)
        {
            this.pluginBuilder = pluginBuilder;
        }

        private void Build()
        {
            var attributeReaderManager = new AttributeReaderManager();
            pluginBuilder.SetAttributeReader(attributeReaderManager);
            AttributeReaders = attributeReaderManager.AttributeReaders;
            PluginService = pluginBuilder.CreatePluginService();
            M3U8FileInfoService = pluginBuilder.CreateM3u8FileInfoService();
        }
    }

    public partial class PluginManger
    {
        public static PluginManger? CreatePluginMangaer(IPluginBuilder? pluginBuilder)
        {
            if (pluginBuilder is null)
                return null;

            PluginManger pluginManger = new(pluginBuilder);
            pluginManger.Build();
            return pluginManger;
        }
    }
}
