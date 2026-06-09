using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Progress.Interfaces
{
    public interface IProgressManager
    {
        IProgressHandler CreateHlsHandler();
        IProgressHandler CreateLiveHandler();
        IProgressHandler CreateVodHandler();
        IProgressHandler CreateMergerHandler();
    }
}
