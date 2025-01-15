using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Models
{
    public class M3u8DownloadParams : IM3u8DownloadParam
    {
        public Uri RequestUrl { get; set; } = default!;

        public string Method { get;  } = default!;

        public string? Key { get;  } = default!;

        public string? Iv { get;  } = default!;

        public string VideoName { get; set; } = default!;

        public string VideoFullName { get; set; } = default!;

        public string SavePath { get; set; } = default!;

        public IDictionary<string, string> Headers { get; set; } = default!;

        public M3u8DownloadParams(Uri url,string? videoname)
        {
            RequestUrl = url;
            if(videoname is not  null) 
                VideoName = videoname;
        }

        public M3u8DownloadParams(Uri url, string? videoname,string method,string? key,string? iv)
        {
            RequestUrl = url;
            if (videoname is not null)
                VideoName = videoname;
            Method = method;
            Key = key;
            Iv = iv;
        }
    }
}
