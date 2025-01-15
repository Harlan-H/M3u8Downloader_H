using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Common
{
    internal interface IDownloadParamBase
    {
        public string VideoName { get; set; }
        public string CachePath { get; }
        public string SavePath { get; }
        public IDictionary<string, string>? Headers { get; }
    }
}
