using M3u8Downloader_H.Common.M3u8Infos;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Combiners.Extensions
{
    internal static class M3uFileInfoExtension
    {
        public static async ValueTask WriteToAsync(this M3UFileInfo m3UFileInfo,string filePath,CancellationToken cancellationToken)
        {
            using var writer = File.CreateText(filePath);
            await m3UFileInfo.WriteToAsync(writer, cancellationToken);
        }

        public static async ValueTask WriteToAsync(this M3UFileInfo m3UFileInfo, TextWriter textWriter,CancellationToken cancellationToken)
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
    }
}
