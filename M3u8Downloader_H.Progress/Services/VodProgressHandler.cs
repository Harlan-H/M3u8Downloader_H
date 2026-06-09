using M3u8Downloader_H.Progress.Interfaces;


namespace M3u8Downloader_H.Progress.Services
{
    internal class VodProgressHandler :ProgressHandleBase, IProgressHandler, IProgressReporter
    {
        private readonly ProgressManager progressManager;
        private double _currentProgress;

        public VodProgressHandler(ProgressManager progressManager) :base(progressManager)
        {
            progressManager.Status = DownloadStatus.StartedVod;
            this.progressManager = progressManager;
        }

        
        protected override void UpdateProgress()
        {
            base.UpdateProgress();
            progressManager.Progress = _currentProgress;
        }

        public void Report(double value)
        {
            _currentProgress = value;
        }
    }
}
