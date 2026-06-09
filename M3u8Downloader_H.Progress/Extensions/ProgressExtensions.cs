using M3u8Downloader_H.Progress.Interfaces;

namespace M3u8Downloader_H.Progress.Extensions
{
    public static class ProgressExtensions
    {
        extension(IProgressHandler progressHandler)
        {
            public IProgressReporter ToReporter()
                => progressHandler as IProgressReporter ?? throw new ArgumentNullException(nameof(progressHandler));
        }

        extension(IProgress<long> progress)
        {
            public IProgressReporter ToReporter()
                => progress as IProgressReporter ?? throw new ArgumentNullException(nameof(progress));
        }
    }
}
