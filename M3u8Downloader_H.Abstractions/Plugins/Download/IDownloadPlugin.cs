using M3u8Downloader_H.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Plugins.Download
{
    public interface IDownloadPlugin
    {
        IM3uFileReader? CreateM3uFileReader(IM3uFileReader m3UFileReader,IDownloadContext downloadContext);

        IDownloadService? CreateDownloadService(IDownloadService downloadService, IDownloadContext downloadContext);
    }
}
