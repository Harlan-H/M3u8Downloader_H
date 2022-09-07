using System.Net.Http;
using M3u8Downloader_H.Core.M3u8Analyzers;
using M3u8Downloader_H.Core.M3uDownloaders;
using M3u8Downloader_H.Core.Utils;


namespace M3u8Downloader_H.Core
{
    public class VideoDownloadClient
    {
        public AnalyzerClient AnalyzerClient { get; }
        public DownloaderFactory DownloaderFactory { get; }
        public VideoDownloadClient(HttpClient httpclient)
        {
            AnalyzerClient = new AnalyzerClient(httpclient);
            DownloaderFactory = new(httpclient, AnalyzerClient);
        }

        public VideoDownloadClient() : this(Http.Client)
        {

        }
     }
}
