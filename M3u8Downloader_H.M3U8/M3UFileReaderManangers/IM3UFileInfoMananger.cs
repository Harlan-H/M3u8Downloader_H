using M3u8Downloader_H.Common.M3u8Infos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.M3UFileReaderManangers
{
    public interface IM3UFileInfoMananger
    {
        Task<M3UFileInfo> GetM3u8FileInfo(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, bool isRetry, CancellationToken cancellationToken = default);
        ValueTask<M3UFileInfo> GetM3u8FileInfo(Uri uri,string? content, IEnumerable<KeyValuePair<string, string>>? headers,  CancellationToken cancellationToken = default);
    }
}
