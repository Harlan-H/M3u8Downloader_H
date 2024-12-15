using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using System;
using System.IO;
using System.Text;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3UFileReaderExtension
    {
        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader,Uri baseUri)
        {
            return reader.GetM3u8FileInfo(baseUri,File.OpenRead(baseUri.OriginalString));
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri, Stream stream)
        {
            reader.WithUri(baseUri);
            return reader.Read(stream);
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri, string m3u8Content)
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(m3u8Content), false);
            reader.WithUri(baseUri);
            return reader.Read(stream);
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri, FileInfo file)
        {
            reader.WithUri(baseUri);
            return reader.Read(file.OpenRead());
        }
    }
}
