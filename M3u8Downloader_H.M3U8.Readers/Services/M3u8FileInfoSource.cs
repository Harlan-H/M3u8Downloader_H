using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.Readers.Services
{
    internal class M3u8FileInfoSource : IM3u8FileInfoSource
    {
        private readonly HttpClient _httpClient;
        private readonly M3UFileReader _m3UFileReader;
        public M3u8FileInfoSource(HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = default!)
        {
            _httpClient = httpClient;
            _m3UFileReader = new M3UFileReader(attributeReaders);
        }

        public async Task<M3UFileInfo> GetM3u8FileInfo(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, bool isRetry,  CancellationToken cancellationToken = default)
        {
            _ = isRetry;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var m3u8FileInfo = await GetM3u8FileInfo(uri, headers, cancellationToken);
                    return m3u8FileInfo.MediaFiles != null && m3u8FileInfo.MediaFiles.Any()
                        ? m3u8FileInfo
                        : throw new InvalidDataException($"'{uri.OriginalString}' 没有包含任何数据");
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(2000, cancellationToken);
                    continue;
                }
            }

            throw new InvalidOperationException($"'{uri.OriginalString}' 请求失败，请检查网络是否可以访问");
        }

        public async ValueTask<M3UFileInfo> GetM3u8FileInfo(Uri uri, string? content, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            if (content is not null)
                return _m3UFileReader.GetM3u8FileInfo(uri, content);
            else if (uri.IsFile)
            {
                string ext = Path.GetExtension(uri.OriginalString).Trim('.');
                return GetM3u8FileInfo(ext, uri);
            }
            else
                return await GetM3u8FileInfo(uri,  headers, true, cancellationToken);
        }

        protected M3UFileInfo GetM3u8FileInfo(string ext, Uri uri)
        {
            M3UFileInfo m3UFileInfo = ext switch
            {
                "xml" => new XmlAnalyzer(uri),
                "json" => new JsonAnalyzer(uri),
                "" => new DirectoryAnalyzer(uri),
                "m3u8" => _m3UFileReader.GetM3u8FileInfo(uri, new FileInfo(uri.OriginalString)),
                _ => throw new InvalidOperationException("请确认是否为.m3u8或.json或.xml或文件夹"),
            };
            return m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any()
                ? m3UFileInfo
                : throw new InvalidDataException($"'{uri.OriginalString}' 没有包含任何数据");
        }


        protected virtual HttpRequestMessage BeforeRequest(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers)
        {
            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, uri);
            httpRequestMessage.AddHeaders(headers);
            return httpRequestMessage;
        }

        protected async Task<M3UFileInfo> GetM3u8FileInfo(Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage httpRequestMessage = BeforeRequest(uri, headers);
            Stream stream = await _httpClient.GetStreamAsync(httpRequestMessage,true, cancellationToken);
            M3UFileInfo m3uFileInfo = _m3UFileReader.GetM3u8FileInfo(uri, PostRequest(stream));
            if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
            {
                M3UStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                return await GetM3u8FileInfo(m3UStreamInfo.Uri, headers, cancellationToken);
            }
            return m3uFileInfo;
        }


        protected virtual Stream PostRequest(Stream stream)
        {
            return stream;
        }
    }
}
