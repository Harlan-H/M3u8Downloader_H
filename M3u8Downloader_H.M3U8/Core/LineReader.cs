using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace M3u8Downloader_H.M3U8.Core
{
    internal sealed class LineReader(Stream stream) : IEnumerator<string>
    {
        private readonly StreamReader _reader = new(stream);

        public string Current { get; private set; } = default!;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool MoveNext()
        {
            bool endOfStream = _reader.EndOfStream;
            Current = endOfStream ? string.Empty : _reader.ReadLine() ?? string.Empty;
            return !endOfStream;
        }

        public void Reset()
        {
            var baseStream = _reader.BaseStream;
            if (baseStream.CanSeek)
                baseStream.Seek(0L, SeekOrigin.Begin);
            _reader.DiscardBufferedData();
            Current = string.Empty;
        }
    }
}