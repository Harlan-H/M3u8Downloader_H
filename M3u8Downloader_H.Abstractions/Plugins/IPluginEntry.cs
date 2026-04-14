using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Abstractions.Plugins.Window;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Plugins
{
    public interface IPluginEntry
    {
        bool HasWindow { get; }
        IWindowPlugin? CreateWindoPlugin();

        bool HasDownload { get; }
        IDownloadPlugin? CreateDownloadPlugin();
    }
}
