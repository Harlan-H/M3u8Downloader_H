using System;
using System.IO;

namespace M3u8Downloader_H.M3U8.Adapters
{
    internal abstract class Adapter : IAdapter
    {
        protected Stream Stream { get; set; } = default!;

        public bool IsConnected { get; protected set; }

        protected abstract Stream CreateStream();

        public Stream Connect()
        {
            if (IsConnected)
                return Stream;

            Stream = CreateStream();
            if (Stream == null)
                throw new NullReferenceException("Stream is null.");

            IsConnected = true;
            return Stream;
        }

        public void Dispose()
        {
            IsConnected = false;
            Stream?.Dispose();
        }
    }
}