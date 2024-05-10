using M3u8Downloader_H.Common.M3u8Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Combiners.Interfaces
{
    public interface IDownloadParams
    {
        string VideoFullPath { get; set; }
        string VideoFullName { get; set; }
        IProgress<double> VodProgress { get; set; }
        IProgress<double> LiveProgress { get; set; }
        Action<string> ChangeVideoNameDelegate { get; set; }
    }
}
