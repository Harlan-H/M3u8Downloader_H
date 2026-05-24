using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Core
{
    internal sealed class LineReader(Stream stream) : IAsyncEnumerator<string>
    {
        private readonly StreamReader _reader = new(stream);

        public string Current  { get; private set; } = default!;

        public async ValueTask DisposeAsync()
        {
            _reader.Dispose();
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            var line = await _reader.ReadLineAsync();
            if (line == null)
                return false;

            Current = line;
            return true;
        }
    }
}