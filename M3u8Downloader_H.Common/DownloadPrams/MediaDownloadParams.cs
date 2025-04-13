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
            var videoTitle = videoUrl.IsFile ? videoUrl.Segments.Last() : _cacheName + ".video";
            Medias.Add(new StreamInfo(videoUrl, videoTitle,"video"));
            if (audioUrl is not null)
            {
                var audioTitle = audioUrl.IsFile ? audioUrl.Segments.Last() : _cacheName + ".audio";
                Medias.Add(new StreamInfo(audioUrl, audioTitle, "audio"));
            }
        }

        public MediaDownloadParams(string savePath, ValueTuple<Uri,long?> video, ValueTuple<Uri, long?>? audio,string? videoName, IDictionary<string, string>? headers)
            :base(video.Item1, videoName,savePath,"mp4",headers)
        {
            var videoTitle = _cacheName + ".video";
            Medias.Add(new StreamInfo(video.Item1, videoTitle, "video",video.Item2 ));
            if(audio is not null)
            {
                var audioTitle = _cacheName + ".audio";
                Medias.Add(new StreamInfo(audio.Value.Item1, audioTitle, "audio", audio.Value.Item2));
            }
        }

    }
}
