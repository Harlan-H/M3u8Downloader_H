using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Plugin;
using System.IO;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    internal class M3UFileReaderWithPlugin : M3UFileReaderWithStream 
    {
        private readonly IM3uFileReader m3UFileReader;

        public M3UFileReaderWithPlugin(IM3uFileReader m3UFileReader)
        {
            this.m3UFileReader = m3UFileReader;
        }

        public override M3UFileInfo Read(Stream stream)
        {
            return m3UFileReader.Read(RequestUri,stream);
        }

    }
}
