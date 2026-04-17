using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;

namespace M3u8Downloader_H.Demo.Plugins.Services
{
    internal class PluginM3u8FileReader(IM3uFileReader m3UFileReader, IDownloadContext downloadContext) : IM3uFileReader
    {
        public void InitAttributeReade(IAttributeReaderCollection readers)
        {
            downloadContext.Log.Info("plugin enter InitAttributeReade");
            downloadContext.Log.Info($" current {readers.Count} size");
            m3UFileReader.InitAttributeReade(readers);
        }

        public IM3uFileInfo GetM3u8FileInfo(Stream stream)
        {
            downloadContext.Log.Info("plugin enter GetM3u8FileInfo");
            var ret = m3UFileReader.GetM3u8FileInfo(stream);
            downloadContext.Log.Info($"plugin GetM3u8FileInfo result:{ret.MediaFiles.Count} size");
            return ret;
        }

    }
}
