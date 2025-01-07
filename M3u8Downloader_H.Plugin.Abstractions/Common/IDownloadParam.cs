using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IDownloadParam
    {
        public Uri RequestUrl { get; }
        public string VideoName { get; set; }
        public string SavePath { get; }
        public IDictionary<string, string>? Headers { get; }
    }
}
