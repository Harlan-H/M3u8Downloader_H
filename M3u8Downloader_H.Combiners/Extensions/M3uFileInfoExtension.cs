using M3u8Downloader_H.Abstractions.M3u8;
using System.Diagnostics;
using System.Text;

namespace M3u8Downloader_H.Combiners.Extensions
{
    internal static class M3uFileInfoExtension
    {
        extension(IM3uFileInfo m3UFileInfo)
        {
            public async ValueTask WriteToAsync(string filePath, CancellationToken cancellationToken)
            {
                using var writer = File.CreateText(filePath);
                await m3UFileInfo.WriteToAsync(writer, cancellationToken);
            }

            public async ValueTask WriteToAsync(TextWriter textWriter, CancellationToken cancellationToken)
            {
                var buffer = new StringBuilder();
                buffer.AppendLine("#EXTM3U");
                buffer.AppendLine($"#EXT-X-MEDIA-SEQUENCE:0");
                if (m3UFileInfo.Map is not null)
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

            public Func<Stream, CancellationToken, Task> GenerateM3U8Stream(string cachePath)
            {
                return async (stream, token) =>
                {
                    using var writer = new StreamWriter(stream,new UTF8Encoding(false));
                    // M3U8 文件头
                    await writer.WriteLineAsync("#EXTM3U");
                    await writer.WriteLineAsync("#EXT-X-MEDIA-SEQUENCE:0");

                    // FMP4 初始化片段（Map）
                    if (m3UFileInfo.Map is not null)
                    {
                        if (OperatingSystem.IsWindows())
                        {
                            await writer.WriteLineAsync($"#EXT-X-MAP:URI=\"{Path.Combine(cachePath, m3UFileInfo.Map.Title)}\"");
                        }
                        else
                        {
                            var mapPath = Path.Combine(cachePath, m3UFileInfo.Map.Title));
                            mapPath = mapPath.Replace("\\", "/");
                            await writer.WriteLineAsync($"#EXT-X-MAP:URI=\"file://{mapPath}\"");
                        }
                    }

                    // 媒体片段
                    foreach (var mediaFile in m3UFileInfo.MediaFiles)
                    {
                         await writer.WriteLineAsync($"#EXTINF:{mediaFile.Duration:F3},");
                         if (OperatingSystem.IsWindows())
                         {
                            await writer.WriteLineAsync(Path.Combine(cachePath, mediaFile.Title));
                         }
                         else
                         {
                            var mediaPath = Path.Combine(cachePath, mediaFile.Title));
                            mediaPath = mediaPath.Replace("\\", "/");
                            await writer.WriteLineAsync($"file://{mediaPath}");
                         }
                    }

                    await writer.WriteLineAsync("#EXT-X-ENDLIST");
                };

            }

            public Func<Stream, CancellationToken, Task> GenerateConcatStream(string cachePath)
            {
                return async (stream, token) =>
                {
                    using var writer = new StreamWriter(stream);

                    if(m3UFileInfo.Map is not null )
                    {
                        await writer.WriteLineAsync($"file '{Path.Combine(cachePath, m3UFileInfo.Map.Title)}'");
                    }

                    foreach (var item in m3UFileInfo.MediaFiles)
                    {
                        await writer.WriteLineAsync($"file '{Path.Combine(cachePath, item.Title)}'");
                    }
                };

            }
        }


       




    }
}
