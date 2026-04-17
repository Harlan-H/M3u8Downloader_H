using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Demo.Plugins.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Demo.Plugins
{
    public class Download : IDownloadPlugin
    {
        public IDownloadService? CreateDownloadService(IDownloadService downloadService, IDownloadContext downloadContext)
            => new PluginDownload(downloadService, downloadContext);

        public IM3uFileReader? CreateM3uFileReader(IM3uFileReader m3UFileReader, IDownloadContext downloadContext) 
            => new PluginM3u8FileReader(m3UFileReader, downloadContext);
    }
}
