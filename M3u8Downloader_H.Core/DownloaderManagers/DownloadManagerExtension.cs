using M3u8Downloader_H.Core.Utils.Extensions;
using M3u8Downloader_H.M3U8.Infos;
using System.Text;

namespace M3u8Downloader_H.Core.DownloaderManagers
{
    public static class DownloadManagerExtension
    {
        public static IDownloadManager WithKeyInfo(this IDownloadManager downloadManager, string method, string key)
        {
            return downloadManager.WithKeyInfo(method, Encoding.UTF8.GetBytes(key), null!);
        }

        public static IDownloadManager WithKeyInfo(this IDownloadManager downloadManager, string method, string key,string iv)
        {
            return downloadManager.WithKeyInfo(method, Encoding.UTF8.GetBytes(key),iv?.ToHex()!);
        }

        public static IDownloadManager WithKeyInfo(this IDownloadManager downloadManager, string method, byte[] key ,byte[] iv)
        {
            M3UKeyInfo m3UKeyInfo = new()
            {
                Method = method,
                BKey = key,
                IV = iv,
            };
            return downloadManager.WithKeyInfo(m3UKeyInfo);
        }
    }
}
