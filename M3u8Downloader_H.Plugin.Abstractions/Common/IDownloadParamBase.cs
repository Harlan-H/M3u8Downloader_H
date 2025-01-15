using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IDownloadParamBase
    {
        //没有扩展名的名称
        string VideoName { get; set; }

        //包含扩展名的名称
        string VideoFullName { get; set; }
        string SavePath { get; set; }
        IDictionary<string, string>? Headers { get; }
    }
}
