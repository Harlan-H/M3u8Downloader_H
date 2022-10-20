using M3u8Downloader_H.M3U8.Adapters;
using M3u8Downloader_H.M3U8.Infos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3UFileReaderExtension
    {
        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader, Uri baseUri, Stream stream)
        {
            return reader.Read(baseUri, new StreamAdapter(stream));
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader, Uri baseUri, string text)
        {
            return reader.Read(baseUri, new TextAdapter(text));
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReader reader, Uri baseUri, FileInfo file)
        {
            return reader.Read(baseUri, new FileAdapter(file));
        }
    }
}
