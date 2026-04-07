using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using System.Linq;


namespace M3u8Downloader_H.Models
{
    public partial class M3u8DownloadInfo : ObservableObject
    {
        private static readonly string[] extensionArr = ["m3u8", "json"];

        [ObservableProperty]
        public partial string[] MethodArr { get; private set; } = ["AES-128", "AES-196", "AES-256"];

        [ObservableProperty]
        public partial string RequestUrl { get; set; }
        [ObservableProperty]
        public partial string VideoName { get; set; }
        [ObservableProperty]
        public partial string Method { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string? Key { get; set; } 
        [ObservableProperty]
        public partial string? Iv { get; set; }

        public void Reset(bool resetUrl,bool resetName)
        {
            if(resetUrl) RequestUrl = string.Empty;
            if(resetName) VideoName = string.Empty;
            Key = null;
            Method = string.Empty;
            Iv = null;
        }

        public Uri GetRequestUri()
        {
            Uri uri = new(RequestUrl, UriKind.Absolute);
            if (!uri.IsFile)
                return uri;

            string ext = Path.GetExtension(RequestUrl).Trim('.');
            if(extensionArr.FirstOrDefault(e => e == ext) is null)
                throw new InvalidOperationException("请确认是否为.m3u8或.json文件");
            
            return uri;
        }
    }
}
