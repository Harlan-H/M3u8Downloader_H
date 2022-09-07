using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Core.Utils
{
    internal class HandleImageStream : Stream
    {
        private readonly Stream stream;
        private readonly byte[] _buffer;
        private int _position;
        private int _length;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => stream.Length;

        public override long Position
        {
            get => _position;
            set => _position = (int)value;
        }

        public HandleImageStream(Stream stream, int capacity)
        {
            this.stream = stream;
            _buffer = new byte[capacity];
            _length = capacity;
        }

        protected override void Dispose(bool disposing)
        {
            stream?.Dispose();
            base.Dispose(disposing);
        }

        public async Task InitializePositionAsync(CancellationToken cancellationToken = default)
        {
            //循环读取的目的是 他可能一次性没有办法读到我需要的数据
            int bytesRead;
            int curPos = 0;
            do
            {
                bytesRead = await stream.ReadAsync(_buffer,curPos, _length - curPos, cancellationToken);
                curPos += bytesRead;
            } while (curPos < _length);

            _position = FindTsMagic(_buffer, _length) ?? throw new InvalidDataException("数据流中没有找到ts结构已退出");
        }

        private static int? FindTsMagic(byte[] data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (data[i] == 0x47 && data[i + 1] == 0x40 && data[i + 188] == 0x47)
                {
                    return i;
                }
            }
            return null;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesRead;
            if (_position < _length)
            {
                int len = _length - _position;
                int tmpCount = len > count ? len - count : len;
                Buffer.BlockCopy(_buffer, _position, buffer, 0, tmpCount);
                _position += bytesRead = tmpCount;
            }
            else
            {
                bytesRead = await stream.ReadAsync(buffer,offset, count, cancellationToken);
                _position += bytesRead;
            }
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
            throw new NotImplementedException();
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
            return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
        }
    }
}
