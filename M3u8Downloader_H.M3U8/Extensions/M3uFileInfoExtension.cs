using M3u8Downloader_H.Abstractions.M3u8;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3uFileInfoExtension
    {
        extension(IM3uFileInfo m3UFileInfo)
        {
            public async Task GenerateM3U8StreamToFile(string filePath,string cachePath,CancellationToken cancellationToken)
            {
                using var writer = File.CreateText(filePath);
                // M3U8 文件头
                await writer.WriteLineAsync("#EXTM3U");
                await writer.WriteLineAsync("#EXT-X-MEDIA-SEQUENCE:0");

                // FMP4 初始化片段（Map）
                if (m3UFileInfo.Map is not null)
                {
                    var mapPath = Path.Combine(cachePath, m3UFileInfo.Map.Title);
                    mapPath = mapPath.Replace('\\', '/');
                    if (OperatingSystem.IsWindows())
                    {
                        await writer.WriteLineAsync($"#EXT-X-MAP:URI=\"{mapPath}\"");
                    }
                    else
                    {
                        await writer.WriteLineAsync($"#EXT-X-MAP:URI=\"file://{mapPath}\"");
                    }
                }

                // 媒体片段
                foreach (var mediaFile in m3UFileInfo.MediaFiles)
                {
                    await writer.WriteLineAsync($"#EXTINF:{mediaFile.Duration:F3},");
                    var mediaPath = Path.Combine(cachePath, mediaFile.Title);
                    mediaPath = mediaPath.Replace('\\', '/');
                    if (OperatingSystem.IsWindows())
                    {
                        await writer.WriteLineAsync(mediaPath);
                    }
                    else
                    {
                        await writer.WriteLineAsync($"file://{mediaPath}");
                    }
                }

                await writer.WriteLineAsync("#EXT-X-ENDLIST");
            }



            public async Task GenerateConcatStreamToFile(string filePath, string cachePath, CancellationToken cancellationToken)
            {
                using var writer = File.CreateText(filePath);

                if (m3UFileInfo.Map is not null)
                {
                    await writer.WriteLineAsync($"file '{Path.Combine(cachePath, m3UFileInfo.Map.Title)}'");
                }

                foreach (var item in m3UFileInfo.MediaFiles)
                {
                    await writer.WriteLineAsync($"file '{Path.Combine(cachePath, item.Title)}'");
                }
            }
        }
    }
}
