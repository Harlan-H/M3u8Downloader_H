using M3u8Downloader_H.Common.Extensions;
using System.Text;

namespace M3u8Downloader_H.Core.Extensions
{
    public static class DownloadClientExtensions
    {
        public static void SetKeyInfo(this DownloadClient downloadManager, string method, string key)
        {
            downloadManager.M3UKeyInfo = new()
            {
                Method = method,
                BKey = Encoding.UTF8.GetBytes(key),
                IV = null!,
            };
        }

        public static void SetKeyInfo(this DownloadClient downloadManager, string method, string key, string iv)
        {
            downloadManager.M3UKeyInfo = new()
            {
                Method = method,
                BKey = Encoding.UTF8.GetBytes(key),
                IV = iv?.ToHex()!,
            };

        }

        public static void SetKeyInfo(this DownloadClient downloadManager, string method, byte[] key, byte[] iv)
        {
            downloadManager.M3UKeyInfo = new()
            {
                Method = method,
                BKey = key,
                IV = iv,
            };
        }
    }
}
