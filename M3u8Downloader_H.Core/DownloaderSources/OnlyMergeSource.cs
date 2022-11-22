using M3u8Downloader_H.Common.M3u8Infos;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.DownloaderSources
{
    internal class OnlyMergeSource : DownloaderSource
    {
        public override async Task DownloadAsync(CancellationToken cancellationToken = default)
        {
            if (M3UFileInfo.MediaFiles.Count < 2)
                throw new InvalidDataException("视频流太少不能少于2个");

            CreateDirectory(_savePath, true);

            //合并只采用原始合并方案
            await VideoMerge(true, cancellationToken);
            if (_formats == "mp4")
            {
                await ConverterToMp4(VideoFullName,false, cancellationToken);
                File.Delete(VideoFullName);
            }             
        }

    }
}
