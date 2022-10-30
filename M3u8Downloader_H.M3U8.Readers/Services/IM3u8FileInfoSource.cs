using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.Readers.Services
{
    public interface IM3u8FileInfoSource
    {
        Task<M3UFileInfo> GetM3u8FileInfo(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, bool isRetry, CancellationToken cancellationToken = default);
        ValueTask<M3UFileInfo> GetM3u8FileInfo(Uri uri,string? content, IEnumerable<KeyValuePair<string, string>>? headers,  CancellationToken cancellationToken = default);
    }
}
