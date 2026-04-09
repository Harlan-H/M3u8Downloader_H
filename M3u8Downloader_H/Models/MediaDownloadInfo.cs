using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace M3u8Downloader_H.Models
{
    public partial class MediaDownloadInfo : ObservableObject
    {
        [ObservableProperty]
        public partial string VideoUrl { get; set; } 

        [ObservableProperty]
        public partial string? AudioUrl { get; set; } 

        [ObservableProperty]
        public partial string? VideoName { get; set; }

        [ObservableProperty]
        public partial int StreamIndex { get; set; } = 0;


        public void Reset(bool resetUrl, bool resetName)
        {
            if (resetUrl)
            {
                VideoUrl = string.Empty;
                AudioUrl = string.Empty;
            }
            if (resetName) 
                VideoName = string.Empty;
        }

        public Uri GetVideoRequestUri()
        {
            if (string.IsNullOrWhiteSpace(VideoUrl) && string.IsNullOrWhiteSpace(AudioUrl))
                throw new InvalidOperationException("视频地址和音频地址不能同时为空");

            Uri uri = new(VideoUrl);
            if (uri.IsFile)
                throw new InvalidOperationException("请确认是否输入正确的网络地址");

            return uri;
        }

        public Uri? GetAudioRequestUri()
        {
            return !string.IsNullOrWhiteSpace(AudioUrl) ? new Uri(AudioUrl) : null; 
        }
    }
}
