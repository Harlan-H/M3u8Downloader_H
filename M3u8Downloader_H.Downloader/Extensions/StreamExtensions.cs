using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Downloader.Extensions
{
    internal static class StreamExtensions
    {
        public static async ValueTask CopyToAsync(this Stream stream, Stream destination,CancellationToken cancellationToken = default)
        {
            using var buffer = MemoryPool<byte>.Shared.Rent(81920);
            long totalBytes = 0L;
            int bytesCopied = 0;
            do
            {
                bytesCopied = await stream.ReadAsync(buffer.Memory, cancellationToken);
                if (bytesCopied > 0) await destination.WriteAsync(buffer.Memory[..bytesCopied], cancellationToken);

                totalBytes += bytesCopied;
            } while (bytesCopied > 0);
        }

        public static async ValueTask<int> CopyToAsync(this Stream stream,Stream destination, Memory<byte> buffer,CancellationToken cancellationToken = default)
        {
            int bytesCopied = await stream.ReadAsync(buffer, cancellationToken);
            if (bytesCopied > 0) await destination.WriteAsync(buffer[..bytesCopied], cancellationToken);

            return bytesCopied;
        }
    }
}
