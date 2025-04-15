using System.Globalization;
using System.Text;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.Extensions;

namespace M3u8Downloader_H.Common.M3u8
{
    public class M3uKeyInfoHelper(string method, byte[] bytes, byte[] iv) : IM3uKeyInfo
    {
        private static readonly Dictionary<string, (int, int)> KeyGroup = new() { { "AES-128", (16, 24) }, { "AES-192", (24, 32) }, { "AES-256", (32, 44) } };

        public string Method { get; } = method;

        public Uri Uri { get; } = default!;

        public byte[] BKey { get; } = bytes;

        public byte[] IV { get; } = iv;

        public static IM3uKeyInfo GetKeyInfoInstance(string method, byte[] bKey , byte[]? iv)
        {
            byte[] data = TryParseKey(method, bKey);
            return new M3uKeyInfoHelper(method, data, iv!);
        }

        public static IM3uKeyInfo GetKeyInfoInstance(IM3uKeyInfo m3UKeyInfo)
        {
            byte[] data = TryParseKey(m3UKeyInfo.Method, m3UKeyInfo.BKey);
            return new M3uKeyInfoHelper(m3UKeyInfo.Method, data, m3UKeyInfo.IV);
        }

        public static IM3uKeyInfo GetKeyInfoInstance(string method, string key)
        {
            byte[] data = TryParseKey(method, key);
            return new M3uKeyInfoHelper(method, data, null!);
        }

        public static IM3uKeyInfo GetKeyInfoInstance(string method, string key, string iv)
        {
            byte[] data = TryParseKey(method, key);
            return new M3uKeyInfoHelper(method, data, iv?.ToHex()!);
        }

        private static byte[] TryParseKey(string method,byte[] data)
        {
            string tmpMethod = string.IsNullOrWhiteSpace(method) ? "AES-128" : method.ToUpper(CultureInfo.CurrentCulture).Trim();
            if (KeyGroup.TryGetValue(tmpMethod, out (int, int) tmpKey))
            {
                if (data.Length == tmpKey.Item1)
                    return data;
                else if (data.Length == tmpKey.Item2)
                {
                    var stringdata = Encoding.UTF8.GetString(data);
                    return Convert.FromBase64String(stringdata);
                }
            }
            throw new InvalidCastException("无法解析的密钥,请确定是否为AES-128,AES-192,AES-256");
        }

        private static byte[] TryParseKey(string method, string data)
        {
            string tmpMethod = string.IsNullOrWhiteSpace(method) ? "AES-128" : method.ToUpper(CultureInfo.CurrentCulture).Trim();
            if (KeyGroup.TryGetValue(tmpMethod, out (int, int) tmpKey))
            {
                if (data.Length == tmpKey.Item1)
                    return Encoding.UTF8.GetBytes(data);
                else if (data.Length == tmpKey.Item2)
                {
                    return Convert.FromBase64String(data);
                }
            }
            throw new InvalidCastException("无法解析的密钥,请确定是否为AES-128,AES-192,AES-256");
        }
    }
}
