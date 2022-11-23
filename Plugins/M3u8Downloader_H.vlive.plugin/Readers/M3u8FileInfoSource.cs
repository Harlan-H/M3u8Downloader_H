using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H.vlive.plugin.Readers
{
    internal class M3u8FileInfoSource : IM3u8FileInfoStreamService
    {
        public async Task<(Uri?, Stream)> GetM3u8FileStreamAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            return await httpClient.GetStreamAndUriAsync(uri, headers, cancellationToken);
        }
    }
}
