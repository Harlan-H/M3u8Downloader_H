using M3u8Downloader_H.Progress.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Progress.Services
{
    internal class MergerProgressHandler : ProgressHandleBase, IProgressHandler, IProgressReporter
    {
        private readonly ProgressManager progressManager;
        private double progressPercentage;
        public MergerProgressHandler(ProgressManager progressManager) : base(progressManager)
        {
            progressManager.Status = DownloadStatus.StartedVod;
            this.progressManager = progressManager;
        }

        protected override void UpdateProgress()
        {
            progressManager.Progress = progressPercentage;
        }


        public void Report(double value)
        {
            progressPercentage = value;
        }
    }
}
