using M3u8Downloader_H.Abstractions.M3u8;
using System.Diagnostics;
using System.Text;

namespace M3u8Downloader_H.Combiners.Extensions
{
    internal static class M3uFileInfoExtension
    {
        public static async ValueTask WriteToAsync(this IM3uFileInfo m3UFileInfo,string filePath,CancellationToken cancellationToken)
        {
            using var writer = File.CreateText(filePath);
            await m3UFileInfo.WriteToAsync(writer, cancellationToken);
        }

        public static async ValueTask WriteToAsync(this IM3uFileInfo m3UFileInfo, TextWriter textWriter,CancellationToken cancellationToken)
        {
            var buffer = new StringBuilder();
            buffer.AppendLine("#EXTM3U");
            buffer.AppendLine($"#EXT-X-MEDIA-SEQUENCE:0");
            if(m3UFileInfo.Map is not null)
                buffer.AppendLine($"#EXT-X-MAP:URI=\"{m3UFileInfo.Map.Title}\"");

            foreach (var item in m3UFileInfo.MediaFiles)
            {
                buffer.AppendLine($"#EXTINF:{item.Duration}");
                buffer.AppendLine(item.Title);
                await textWriter.WriteAsync(buffer, cancellationToken);
                buffer.Clear();
            }

            buffer.AppendLine("#EXT-X-ENDLIST");
            await textWriter.WriteAsync(buffer, cancellationToken);
        }

        public static Func<Stream, CancellationToken, Task> GenerateM3U8Stream(this IM3uFileInfo m3uFileInfo, string cachePath)
        {
            return async (stream, token) =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    // M3U8 文件头
                    await writer.WriteLineAsync("#EXTM3U");
                    await writer.WriteLineAsync("#EXT-X-MEDIA-SEQUENCE:0");

                    // FMP4 初始化片段（Map）
                    if (m3uFileInfo.Map is not null)
                    {
                        await writer.WriteLineAsync($"#EXT-X-MAP:URI=\"{Path.Combine(cachePath, m3uFileInfo.Map.Title) }\"");
                    }

                    // 媒体片段
                    foreach (var mediaFile in m3uFileInfo.MediaFiles)
                    {
                        await writer.WriteLineAsync($"#EXTINF:{mediaFile.Duration:F3},");
                        await writer.WriteLineAsync(Path.Combine(cachePath, mediaFile.Title));
                    }

                    await writer.WriteLineAsync("#EXT-X-ENDLIST");
                }
            };
            
        }

        public static Func<Stream, CancellationToken, Task> GenerateConcatStream(this IM3uFileInfo m3uFileInfo,string cachePath)
        {
            return async (stream, token) =>
            {
                using var writer = new StreamWriter(stream);

                foreach (var item in m3uFileInfo.MediaFiles)
                {
                    await writer.WriteLineAsync($"file '{Path.Combine(cachePath, item.Title)}'");
                }
            };

        }
    }
}
