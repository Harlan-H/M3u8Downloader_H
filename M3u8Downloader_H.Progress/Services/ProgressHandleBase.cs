using M3u8Downloader_H.Progress.Interfaces;
using M3u8Downloader_H.Progress.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Progress.Services
{
    internal class ProgressHandleBase(ProgressManager progressManager) : IProgressHandler
    {
        private readonly GlobalProgressService GlobalProgressService = GlobalProgressService.Instance;
        private long _lastBytes;
        private long _countBytes;

        protected void HandleDownloadBytes()
        {
            long tmpBytes = _countBytes;
            progressManager.DownloadedBytes = tmpBytes - _lastBytes;
            _lastBytes = tmpBytes;
        }

        protected virtual void UpdateProgress()
        {
            HandleDownloadBytes();
        }

        public void Report(long value)
        {
            Interlocked.Add(ref _countBytes, value);
        }

        public IDisposable Acquire() => GlobalProgressService.AddTickCallback((_, _) => UpdateProgress());

        public void Clear() => progressManager.Clear();
    }
}
