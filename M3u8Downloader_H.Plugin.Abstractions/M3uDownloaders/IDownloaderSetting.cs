using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.M3uDownloaders
{
    public interface IDownloaderSetting
    {
        public TimeSpan Timeouts { get; }
        public int RetryCount { get; }
        public bool SkipRequestError { get; }
        public int MaxThreadCount { get; }
        double RecordDuration { get; }
        Dictionary<string, string> Headers { get; }
    }

}
