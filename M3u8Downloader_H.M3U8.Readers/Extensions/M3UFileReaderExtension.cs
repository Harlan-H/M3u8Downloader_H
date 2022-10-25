using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.M3U8.Readers;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3UFileReaderExtension
    {
        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader,string ext,Uri uri)
        {
            M3UFileInfo m3UFileInfo = ext switch
            {
                "xml" => new XmlAnalyzer(uri),
                "json" => new JsonAnalyzer(uri),
                "" => new DirectoryAnalyzer(uri),
                "m3u8" => reader.GetM3u8FileInfo(uri,new FileInfo(uri.OriginalString)),
                _ => throw new InvalidOperationException("请确认是否为.m3u8或.json或.xml或文件夹"),
            };
            return m3UFileInfo.MediaFiles != null && m3UFileInfo.MediaFiles.Any() 
                ? m3UFileInfo
                : throw new InvalidDataException($"'{uri.OriginalString}' 没有包含任何数据");
        }

        public static async Task<M3UFileInfo> GetM3u8FileInfo(this M3UFileReader reader, HttpClient http, Uri uri, bool isRetry, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            _ = isRetry;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var m3u8FileInfo = await reader.GetM3u8FileInfo(http, uri, headers, cancellationToken);
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

        public static async Task<M3UFileInfo> GetM3u8FileInfo(this M3UFileReader reader, HttpClient http, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            Stream stream = await http.GetStreamAsync(uri, headers, cancellationToken);
            M3UFileInfo m3uFileInfo = reader.GetM3u8FileInfo(uri, stream);
            if (m3uFileInfo.Streams != null && m3uFileInfo.Streams.Any())
            {
                M3UStreamInfo m3UStreamInfo = m3uFileInfo.Streams.Count > 1 ? m3uFileInfo.Streams.OrderByDescending(s => s.Bandwidth).First() : m3uFileInfo.Streams.First();
                stream = await http.GetStreamAsync(m3UStreamInfo.Uri, headers, cancellationToken);
                return reader.GetM3u8FileInfo(uri, stream);
            }
            return m3uFileInfo;
        }
    }
}
