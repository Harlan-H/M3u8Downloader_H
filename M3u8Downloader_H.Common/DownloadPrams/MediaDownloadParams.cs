using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Common.DownloadPrams
{
    public class MediaDownloadParams : DownloadParamsBase, IMediaDownloadParam
    {
        public IList<IStreamInfo> Medias { get; } = [];

        public bool IsVideoStream { get; set; }


        public MediaDownloadParams(string savePath, Uri videoUrl, Uri? audioUrl,  string? videoName, IDictionary<string, string>? headers)
            : base(videoUrl, videoName, savePath, "mp4", headers)
        {
            var videoTitle = videoUrl.IsFile ? videoUrl.Segments.Last() : VideoName + ".video";
            Medias.Add(new StreamInfo(videoUrl, videoTitle,"video"));
            if (audioUrl is not null)
            {
                var audioTitle = audioUrl.IsFile ? audioUrl.Segments.Last() : VideoName + ".audio";
                Medias.Add(new StreamInfo(audioUrl, audioTitle, "audio"));
            }
        }
    }
}
