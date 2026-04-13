using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Downloader.Extensions;
using System.Buffers;
using System.Net.Http.Headers;


namespace M3u8Downloader_H.Downloader.MediaDownloads
{
    public abstract class DownloaderBase(IDownloadContext downloadContext)
    {
        private bool _firstTimeToRun = true;
        private readonly IMediaDownloadParam DownloadParam = (IMediaDownloadParam)downloadContext.DownloadParam;
        internal IDialogProgress DialogProgress { get; set; } = default!;

        protected IEnumerable<KeyValuePair<string, string>>? _headers => DownloadParam.Headers ?? downloadContext.DownloaderSetting.Headers;

        protected string _cachePath => DownloadParam.CachePath;

        public virtual Task DownloadAsync(IStreamInfo streamInfo,CancellationToken cancellationToken = default)
        {
            if (_firstTimeToRun)
            {
                CreateDirectory(_cachePath);
                _firstTimeToRun = false;
            }

            return Task.CompletedTask;
        }


        protected async Task DownloadAsynInternal(IStreamInfo streamInfo, IEnumerable<KeyValuePair<string, string>>? headers, RangeHeaderValue? rangeHeaderValue,Func<FileStream> mediaFile, CancellationToken token)
        {  
            using Stream stream = await downloadContext.HttpClient.GetResponseContentAsync(streamInfo.Url, headers, rangeHeaderValue, token);
            using FileStream fileobject = mediaFile();
            await WriteToFileAsync(streamInfo, fileobject, stream, token); 
        }


        protected abstract void UpdateProgress(long total, long? filesize);

        protected async Task WriteToFileAsync(IStreamInfo streamInfo, FileStream fileStream, Stream stream, CancellationToken token = default)
        {
            using var buffer = MemoryPool<byte>.Shared.Rent(0x10000);
            Memory<byte> memory = buffer.Memory;
            long totalBytes = fileStream.Length;
            int bytesCopied = 0;
            do
            {
                bytesCopied = await stream.ReadAsync(memory, token);
                if (bytesCopied > 0) await fileStream.WriteAsync(memory[..bytesCopied], token);

                totalBytes += bytesCopied;
                DialogProgress?.Report(bytesCopied);
                UpdateProgress(totalBytes, streamInfo.FileSize!);
            } while (bytesCopied > 0);
        }


        protected void CreateDirectory(string dirPath)
        {
            DirectoryInfo directoryInfo = new(dirPath);
            if (directoryInfo.Exists)
            {
                downloadContext.Log?.Info("找到缓存目录:{0},开始继续下载", dirPath);
                return;
            }
            directoryInfo.Create();
            downloadContext.Log?.Info("创建缓存目录:{0}", dirPath);
        }

    }
}
