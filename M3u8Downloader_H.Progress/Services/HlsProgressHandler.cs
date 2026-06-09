using M3u8Downloader_H.Progress.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Progress.Services
{
    internal class HlsProgressHandler : ProgressHandleBase, IProgressHandler, IProgressReporter
    {
        private double _Totalduration;
        private readonly ProgressManager progressManager;

        public HlsProgressHandler(ProgressManager progressManager) :base(progressManager) 
        {
            progressManager.Status = DownloadStatus.StartedLive;
            this.progressManager = progressManager;
        }
        protected override void UpdateProgress()
        {
            base.UpdateProgress();
            progressManager.Progress = _Totalduration;
        }

        public void Report(double value)
        {
            _Totalduration = value;
        }
    }
}
