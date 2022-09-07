using System;
using System.Threading.Tasks;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.Core.M3uCombiners
{
    internal interface IM3uCombiner : IDisposable
    {
        ValueTask Start(M3UFileInfo m3UFileInfo, bool forceMerge);

    }
}
