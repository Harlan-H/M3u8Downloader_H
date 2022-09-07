using System;
using System.IO;

namespace M3u8Downloader_H.M3U8.Adapters
{
    internal interface IAdapter : IDisposable
    {
        bool IsConnected { get; }

        Stream Connect();
    }
}