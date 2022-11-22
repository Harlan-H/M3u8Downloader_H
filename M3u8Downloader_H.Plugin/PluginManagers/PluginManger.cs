using M3u8Downloader_H.Plugin.AttributeReaderManagers;

namespace M3u8Downloader_H.Plugin.PluginManagers
{
    public partial class PluginManger : IPluginManager
    {
        private readonly IPluginBuilder pluginBuilder;

        public IDictionary<string, IAttributeReader> AttributeReaders { get; private set; } = default!;

        public IDownloadService? PluginService { get; private set; }

        public IM3u8UriProvider? M3U8UriProvider { get; private set; }

        public IM3uFileReader? M3UFileReaderInterface { get; private set; }

        public IM3u8FileInfoStreamService? M3U8FileInfoStreamService { get; private set; }

        private PluginManger(IPluginBuilder pluginBuilder)
        {
            this.pluginBuilder = pluginBuilder;
        }

        private void Build()
        {
            M3U8UriProvider = pluginBuilder.CreateM3u8UriProvider();
            M3UFileReaderInterface = pluginBuilder.CreateM3u8FileReader();
            M3U8FileInfoStreamService = pluginBuilder.CreateM3U8FileInfoStreamService();
            var attributeReaderManager = new AttributeReaderManager();
            pluginBuilder.SetAttributeReader(attributeReaderManager);
            AttributeReaders = attributeReaderManager.AttributeReaders;
            PluginService = pluginBuilder.CreatePluginService();
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
