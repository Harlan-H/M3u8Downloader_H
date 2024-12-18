using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Combiners.M3uCombiners
{
    internal class M3uCombiner(string dirPath) : IM3uCombiner
    {
        protected readonly string cacheFullPath = dirPath;
        protected FileStream videoFileStream = default!;

        public IProgress<double> Progress { get; set; } = default!;

        protected virtual Stream HandleData(string path) => File.OpenRead(path);

        protected string GetTsPath(M3UMediaInfo m3UMediaInfo)
        {
            return m3UMediaInfo.Uri.IsFile ? m3UMediaInfo.Uri.OriginalString : Path.Combine(cacheFullPath, m3UMediaInfo.Title);
        }

        public virtual void Initialization(string videoName) 
        {
            if(videoName is not null)
                videoFileStream = File.Create(videoName);
        }

        public async ValueTask MegerVideoHeader(M3UMediaInfo? m3UMapInfo, CancellationToken cancellationToken)
        {
            if (m3UMapInfo is null)
                return;

            await MegerVideoInternalAsync(m3UMapInfo, cancellationToken);
        }

        public async ValueTask Start(M3UFileInfo m3UFileInfo, bool forceMerge, CancellationToken cancellationToken)
        {

            for (int i = 0; i < m3UFileInfo.MediaFiles.Count; i++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await MegerVideoInternalAsync(m3UFileInfo.MediaFiles[i], cancellationToken);
                    Progress.Report(i / (double)m3UFileInfo.MediaFiles.Count);
                }
                catch(CryptographicException)
                {
                    throw new CryptographicException("解密失败,请确认key,iv是否正确");
                }
                catch (Exception) when (forceMerge)
                {
                    continue;
                }
            }
       }

        protected virtual async ValueTask MegerVideoInternalAsync(M3UMediaInfo m3UMediaInfo,CancellationToken cancellationToken)
        {
            string tsFileName = GetTsPath(m3UMediaInfo);
            using Stream tsStream = HandleData(tsFileName);
            await tsStream.CopyToAsync(videoFileStream, cancellationToken);
        }

        public void Dispose()
        {
            videoFileStream?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
