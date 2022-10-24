using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderPluginManagers
{
    public interface IPluginManager
    {
        IDictionary<string, IAttributeReader> AttributeReaders { get; }
        IDownloadService? PluginService { get; }
    }
}
