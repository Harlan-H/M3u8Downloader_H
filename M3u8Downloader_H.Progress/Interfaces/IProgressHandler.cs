using M3u8Downloader_H.Progress.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Progress.Interfaces
{
    public interface IProgressHandler
    {
        IDisposable Acquire();
        void Clear();
    }
}
