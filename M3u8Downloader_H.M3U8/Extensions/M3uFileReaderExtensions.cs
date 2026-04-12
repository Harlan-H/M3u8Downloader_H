using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Plugins;
using M3u8Downloader_H.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3uFileReaderExtensions
    {
        extension(IM3uFileReader m3UFileReader)
        {
            public IM3uFileInfo GetM3u8FileInfo(Uri baseUri)
                => m3UFileReader.GetM3u8FileInfo(File.OpenRead(baseUri.OriginalString));


            public IM3uFileInfo GetM3u8FileInfo(string m3u8Content)
            {
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(m3u8Content), false);
                return m3UFileReader.GetM3u8FileInfo(stream);
            }

            public IM3uFileInfo GetM3u8FileInfo( FileInfo file)
                => m3UFileReader.GetM3u8FileInfo(file.OpenRead());

        } 
    }
}
