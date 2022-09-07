using System;
using System.IO;

namespace M3u8Downloader_H.M3U8.Adapters
{
    internal class StreamAdapter : Adapter
    {
        public new Stream Stream => base.Stream;

        public StreamAdapter(Stream stream)
        {
            base.Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        protected override Stream CreateStream()
        {
            return Stream;
        }
    }
}