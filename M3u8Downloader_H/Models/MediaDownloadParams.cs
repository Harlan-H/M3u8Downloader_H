using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Services;
using M3u8Downloader_H.Utils;

namespace M3u8Downloader_H.Models
{
    public class MediaDownloadParams : IMediaDownloadParam
    {
        public IList<IStreamInfo> Medias { get; } = [];

        public Uri Subtitle { get; } = default!;

        public string VideoName { get; } = default!;

        public string VideoFullName { get; private set; } = default!;

        public string SavePath { get; } = default!;

        public IDictionary<string, string>? Headers { get; }

        public bool IsVideoStream { get; set; }

        public MediaDownloadParams(SettingsService settingsService,string videoUrl,string? audioUrl,string? subtitle,string? videoName)
        {
            SavePath = settingsService.SavePath;
            Uri videoUri = new(videoUrl);
            Medias.Add(new StreamInfo(videoUri));
            if(!string.IsNullOrWhiteSpace(audioUrl))
                Medias.Add(new StreamInfo(new Uri(audioUrl), "audio"));
            if(!string.IsNullOrWhiteSpace(subtitle))
                Subtitle = new Uri(subtitle);
            VideoName = PathEx.GenerateFileNameWithoutExtension(videoUri, videoName);
        }

        public void SetVideoFullName(string videoName)
        {
            if (string.IsNullOrWhiteSpace(videoName))
                return;

            VideoFullName = videoName;
        }
    }
}
