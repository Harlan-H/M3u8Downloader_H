using M3u8Downloader_H.Plugin;

namespace M3u8Downloader_H._555dd7.plugin.Streams
{
    internal class StreamClient : IM3u8UriProvider
    {
        public async Task<Uri> GetM3u8UriAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            Extractor extractor = new(httpClient, headers);
            return await extractor.GetM3u8IndexUrl(uri, cancellationToken);
        }
    }
}
