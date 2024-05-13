using M3u8Downloader_H.Combiners.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Models
{
    public class DownloadParam : IDownloadParams
    {
        public string VideoFullPath { get ; set ; } = default!;
        public string VideoFullName { get; set; } = default!;
        public IProgress<double> VodProgress { get ; set ; } = default!;
        public IProgress<double> LiveProgress { get ; set ; } = default!;
        public Action<string> ChangeVideoNameDelegate { get ; set ; } = default!;
    }
}
