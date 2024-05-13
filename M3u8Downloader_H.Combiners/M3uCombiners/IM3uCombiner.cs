using System;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Combiners.M3uCombiners
{
    internal interface IM3uCombiner : IDisposable
    {
        void Initialization(string videoName);
        ValueTask MegerVideoHeader(M3UMediaInfo? m3UMapInfo, CancellationToken cancellationToken);
        ValueTask Start(M3UFileInfo m3UFileInfo, bool forceMerge, CancellationToken cancellationToken);

    }
}
