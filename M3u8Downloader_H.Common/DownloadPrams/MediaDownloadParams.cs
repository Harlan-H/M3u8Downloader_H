using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class MediaDownloadParams : DownloadParamsBase, IMediaDownloadParam
    {
        public IList<IStreamInfo> Medias { get; } = [];

        public Uri Subtitle { get; } = default!;

        public bool IsVideoStream { get; set; }


        public MediaDownloadParams(string? cachePath, string savePath, Uri videoUrl, Uri? audioUrl, Uri? subtitle, string? videoName, IDictionary<string, string>? headers)
            : base(videoUrl, videoName, savePath, "mp4", headers)
        {
            Medias.Add(new StreamInfo(videoUrl));
            if (audioUrl is not null)
                Medias.Add(new StreamInfo(videoUrl, "audio"));
            if (subtitle is not null)
                Subtitle = subtitle;
        }
    }
}
