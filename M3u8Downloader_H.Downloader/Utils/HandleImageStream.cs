using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Downloader.Utils
{
    internal class HandleImageStream : Stream
    {
        private readonly Stream stream;
        private readonly IProgress<long> _downloadrate = default!;
        private IMemoryOwner<byte>? _memoryOwner;
        private Memory<byte> _tsMemory = Memory<byte>.Empty;
        private int _position;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => stream.Length;

        public override long Position
        {
            get => _position;
            set => _position = (int)value;
        }

        public HandleImageStream(Stream stream,IProgress<long> downloadrate)
        {
            this.stream = stream;
            _downloadrate = downloadrate;
        }

        protected override void Dispose(bool disposing)
        {
            stream?.Dispose();
            _memoryOwner?.Dispose();
            base.Dispose(disposing);
        }

        public async Task InitializePositionAsync(int capacity,CancellationToken cancellationToken = default)
        {
            //循环读取的目的是 他可能一次性没有办法读到我需要的数据    
            _memoryOwner = MemoryPool<byte>.Shared.Rent(capacity);
            int bytesRead, totalBytes = 0;
            do
            {
                bytesRead = await stream.ReadAsync(_memoryOwner.Memory[totalBytes..capacity], cancellationToken);
                totalBytes += bytesRead;
            } while (bytesRead > 0 && totalBytes < capacity);

            int pos = FindTsMagic(_memoryOwner.Memory.Span, totalBytes) ?? throw new InvalidDataException("数据流中没有找到ts结构已退出");
            _tsMemory = _memoryOwner.Memory[pos..totalBytes];
        }

        private static int? FindTsMagic(ReadOnlySpan<byte> data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (data[i] == 0x47 && data[i + 188] == 0x47 && data[i + 188 * 2] == 0x47)
                {
                    return i;
                }
            }
            return null;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int bytesRead;
            if (!_tsMemory.IsEmpty && _position < _tsMemory.Length)
            {
                _tsMemory.CopyTo(buffer);
                _position += bytesRead = _tsMemory.Length;
            }
            else
            {
                bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                _position += bytesRead;
            }
            _downloadrate?.Report(bytesRead);
            return bytesRead;
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            return Position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => throw new ArgumentOutOfRangeException(nameof(origin)),
                _ => throw new ArgumentOutOfRangeException(nameof(origin))
            };
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = stream.Read(buffer, offset, count);
            _position += bytesRead;
            _downloadrate?.Report(bytesRead);
            return bytesRead;
        }
    }
}
