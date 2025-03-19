using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal class OnlyDecryptDownloader(M3UFileInfo m3UFileInfo,HttpClient httpClient) : M3u8Downloader(httpClient)
    {
        public override ValueTask Initialization(CancellationToken cancellationToken = default)
        {
            if (m3UFileInfo.Key is null)
                throw new InvalidDataException("没有可用的密钥信息");


            if (m3UFileInfo.Key.Uri != null && m3UFileInfo.Key.BKey == null)
            {
                throw new HttpRequestException("密钥不能是网络地址");
            }
            else
            {
                m3UFileInfo.Key.BKey = m3UFileInfo.Key.BKey != null
                   ? m3UFileInfo.Key.BKey.TryParseKey(m3UFileInfo.Key.Method)
                   : throw new InvalidDataException("密钥为空");
            }
            return base.Initialization(cancellationToken);
        }

        public override async Task DownloadAsync(M3UFileInfo m3UFileInfo, CancellationToken cancellationToken = default)
        {
            await base.DownloadAsync(m3UFileInfo, cancellationToken);
            DialogProgress.SetDownloadStatus(false);

            for (var i = 0; i < m3UFileInfo.MediaFiles.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                FileInfo fileInfo = new(m3UFileInfo.MediaFiles[i].Uri.OriginalString);
                if (!fileInfo.Exists || fileInfo.Length == 0) 
                {
                    throw new Exception("文件不存在或者文件大小为0");
                }
 
                using var fileStream = fileInfo.OpenRead();
                using Stream stream = fileStream.AesDecrypt(m3UFileInfo.Key.BKey, m3UFileInfo.Key.IV);

                string mediaPath = Path.Combine(_cachePath, m3UFileInfo.MediaFiles[i].Title);
                await WriteToFileAsync(mediaPath, stream, cancellationToken);

                DialogProgress.Report((double)i / m3UFileInfo.MediaFiles.Count);
            }
        }
    }
}
