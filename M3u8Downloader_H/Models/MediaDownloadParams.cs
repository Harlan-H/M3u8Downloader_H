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
    public class MediaDownloadParams : DownloadParamsBase,IMediaDownloadParam
    {
        public IList<IStreamInfo> Medias { get; } = [];

        public Uri Subtitle { get; } = default!;

        public bool IsVideoStream { get; set; }


        public MediaDownloadParams(SettingsService settingsService,string videoUrl,string? audioUrl,string? subtitle,string? videoName)
            :base(settingsService.SavePath,null)
        {
            Uri videoUri = new(videoUrl);
            Medias.Add(new StreamInfo(videoUri));
            if(!string.IsNullOrWhiteSpace(audioUrl))
                Medias.Add(new StreamInfo(new Uri(audioUrl), "audio"));
            if(!string.IsNullOrWhiteSpace(subtitle))
                Subtitle = new Uri(subtitle);
            VideoName = PathEx.GenerateFileNameWithoutExtension(videoUri, videoName);
        }
    }
}
