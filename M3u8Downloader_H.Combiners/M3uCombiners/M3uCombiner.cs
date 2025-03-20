using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Meger;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Abstractions.Extensions;

namespace M3u8Downloader_H.Combiners.M3uCombiners
{
    public class M3uCombiner(ILog Log, IDownloadParamBase DownloadParams) : IDisposable
    {
        private bool _isFile = false;
        private readonly FileStream videoFileStream = File.Create(DownloadParams.GetVideoFullPath());
        public IDialogProgress DialogProgress { get; set; } = default!;
        public IMergeSetting Settings { get; set; } = default!;

        public void Initialize(M3UFileInfo m3UFileInfo) => _isFile = m3UFileInfo.IsFile;

        public async ValueTask MegerVideoHeader(M3UMediaInfo? m3UMapInfo, CancellationToken cancellationToken = default)
        {
            if (m3UMapInfo is null)
                return;

            Log.Info("开始合并fmp4头");
            await MegerVideoInternalAsync(m3UMapInfo, cancellationToken);
        }

        public async ValueTask StartMerging(M3UFileInfo m3UFileInfo, CancellationToken cancellationToken = default)
        {
            Log.Info("开始合并fmp4,数量:{0}", m3UFileInfo.MediaFiles.Count);
            for (int i = 0; i < m3UFileInfo.MediaFiles.Count; i++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await MegerVideoInternalAsync(m3UFileInfo.MediaFiles[i], cancellationToken);
                    DialogProgress.Report(i / (double)m3UFileInfo.MediaFiles.Count);
                }
                catch (Exception) when (Settings.ForcedMerger)
                {
                    continue;
                }
            }
            Log.Info("fmp4合并完成");
        }

        protected virtual async ValueTask MegerVideoInternalAsync(M3UMediaInfo m3UMediaInfo,CancellationToken cancellationToken)
        {
            string tsFileName = _isFile ? m3UMediaInfo.Uri.OriginalString : Path.Combine(DownloadParams.GetCachePath(), m3UMediaInfo.Title);
            using Stream tsStream = File.OpenRead(tsFileName);
            await tsStream.CopyToAsync(videoFileStream, cancellationToken);
        }

        public void Dispose()
        {
            videoFileStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
