using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class M3uFileReaderExtensions
    {
        extension(IM3uFileReader m3UFileReader)
        {
            public async Task<IM3uFileInfo> GetM3u8FileInfo(Uri baseUri)
                => await m3UFileReader.GetM3u8FileInfo(baseUri, File.OpenRead(baseUri.OriginalString));


            public async Task<IM3uFileInfo> GetM3u8FileInfo(Uri baseUri,string m3u8Content)
            {
                Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(m3u8Content), false);
                return await m3UFileReader.GetM3u8FileInfo(baseUri,stream);
            }

//             public async Task<IM3uFileInfo> GetM3u8FileInfo(FileInfo file)
//                 => await m3UFileReader.GetM3u8FileInfo(file.OpenRead());
        } 
    }
}
