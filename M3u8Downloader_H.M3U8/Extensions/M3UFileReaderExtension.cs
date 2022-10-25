using M3u8Downloader_H.M3U8.Infos;
using System;
using System.IO;
using System.Text;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3UFileReaderExtension
    {
        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader, Uri baseUri, Stream stream)
        {
            return reader.Read(baseUri, stream);
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader, Uri baseUri, string text)
        {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text), false);
            return reader.Read(baseUri, stream);
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader, Uri baseUri, FileInfo file)
        {
            return reader.Read(baseUri, file.OpenRead());
        }
    }
}
