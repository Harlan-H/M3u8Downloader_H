using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners.M3uCombiners;
using M3u8Downloader_H.Combiners.VideoConverter;

namespace M3u8Downloader_H.Combiners
{
    public class M3uCombinerClient(ILog Log, IDownloadParamBase DownloadParams, IMergeSetting Settings)
    {
        private M3uCombiner? m3UCombiner;
        private FFmpeg? ffmpeg;
        public IDialogProgress DialogProgress { get; set; } = default!;

        public M3uCombiner M3u8FileMerger
        {
            get
            {
                m3UCombiner ??= new(Log, DownloadParams, Settings);
                return m3UCombiner;
            }
        }


        public FFmpeg FFmpeg
        {
            get
            {
                ffmpeg ??= new(Log, DownloadParams, Settings);
                return ffmpeg;
            }
        }
    }
}
