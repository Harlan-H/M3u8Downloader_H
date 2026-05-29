using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.M3U8.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.Interfaces
{
    public interface IM3uDownloader : IDownloader
    {
        Func<IList<IM3uStreamInfo>, IEnumerable<IM3uMediaManifest>?, IEnumerable<IM3uMediaManifest>?, Task<IList<M3uFileInfoSource>>> FileInfoSourcesFactory { get; set; }
    }
}
