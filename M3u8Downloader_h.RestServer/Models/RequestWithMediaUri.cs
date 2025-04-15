using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.RestServer.Attributes;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithMediaUri : RequestBase
    {
        public bool IsVideoStream { get; set; } = true;

        [Required(ExceptionMsg = "video必须填写不能为空")]
        public MediaInfo Video { get; set; } = default!;
        public MediaInfo? Audio { get; set; }

        public IMediaDownloadParam ToMediaDownloadParams()
        {
            (Uri,long?)? audio = Audio is not null ? (Audio.Url, Audio.FileSize) : null;
            return new MediaDownloadParams(SavePath, (Video.Url, Video.FileSize), audio, VideoName, Headers)
            {
                IsVideoStream = IsVideoStream,
            };
        }
    }

    internal class MediaInfo
    {
        [Required(ExceptionMsg = "url不能为空")]
        public Uri Url { get; set; } = default!;

        public long? FileSize { get; set; }
    }
}
