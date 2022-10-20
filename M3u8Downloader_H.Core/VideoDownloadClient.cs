using System.Net.Http;
using M3u8Downloader_H.Core.M3uDownloaders;
using M3u8Downloader_H.Core.Utils;


namespace M3u8Downloader_H.Core
{
    public class VideoDownloadClient
    {
        public DownloaderFactory DownloaderFactory { get; }
        public VideoDownloadClient(HttpClient httpclient)
        {
            DownloaderFactory = new(httpclient);
        }

        public VideoDownloadClient() : this(Http.Client)
        {

        }
     }
}
