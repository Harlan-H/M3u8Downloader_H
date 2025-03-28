using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.Extensions;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    public  class OnlyDecryptDownloader() : DownloaderBase()
    {
        private IM3uKeyInfo _keyInfo = default!;
        public void Initialization(IM3uKeyInfo keyinfo)
        {
            _keyInfo = keyinfo;
        }

        public override async Task DownloadAsync(IM3uFileInfo m3UFileInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(m3UFileInfo, cancellationToken);

            for (var i = 0; i < m3UFileInfo.MediaFiles.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string mediaPath = Path.Combine(_cachePath, m3UFileInfo.MediaFiles[i].Title);
                if (File.Exists(mediaPath))
                    continue;

                FileInfo fileInfo = new(m3UFileInfo.MediaFiles[i].Uri.OriginalString);
                if (!fileInfo.Exists || fileInfo.Length == 0) 
                {
                    throw new Exception("文件不存在或者文件大小为0");
                }
 
                using var fileStream = fileInfo.OpenRead();
                using Stream stream = fileStream.AesDecrypt(_keyInfo.BKey, _keyInfo.IV);

                await WriteToFileAsync(mediaPath, stream, cancellationToken);

                DialogProgress.Report((double)i / m3UFileInfo.MediaFiles.Count);
            }
        }
    }
}
