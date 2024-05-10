using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Downloader.M3uDownloaders
{
    internal interface IM3uDownloader
    {
        Task Start(M3UFileInfo m3UFileInfo, int TaskNumber, string filePath, int reserve0, bool skipRequestError = false, CancellationToken cancellationToken = default);
        Task<double> Start(M3UFileInfo m3UFileInfo, string savePath, int reserve0, bool skipRequestError = false, CancellationToken cancellationToken = default);
    }
}
