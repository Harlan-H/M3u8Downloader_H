using M3u8Downloader_H.Abstractions.Plugins;
using System.IO;
using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    internal class M3UFileReaderWithPlugin(IM3uFileReader m3UFileReader) : M3UFileReaderWithStream 
    {
        private readonly IM3uFileReader m3UFileReader = m3UFileReader;

        public override IM3uFileInfo Read(Stream stream) => m3UFileReader.Read(RequestUri, stream);
    }
}
