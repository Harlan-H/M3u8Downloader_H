using M3u8Downloader_H.Core.M3uCombiners;
using M3u8Downloader_H.M3U8.Infos;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderSources
{
    internal class OnlyMergeSource : DownloaderSource
    {
        public override async Task DownloadAsync(CancellationToken cancellationToken = default)
        {
            CreateDirectory(_savePath, true);
            if (M3UFileInfo.MediaFiles.Count < 2) return;

            await VideoMerge(true);
        }
    }
}
