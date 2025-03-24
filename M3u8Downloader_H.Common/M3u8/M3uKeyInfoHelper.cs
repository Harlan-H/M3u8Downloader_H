using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.Extensions;

namespace M3u8Downloader_H.Common.M3u8
{
    public class M3uKeyInfoHelper(string method, byte[] bytes, byte[] iv) : IM3uKeyInfo
    {
        public string Method { get; } = method;

        public Uri Uri { get; } = default!;

        public byte[] BKey { get; } = bytes;

        public byte[] IV { get; } = iv;

        public static IM3uKeyInfo GetKeyInfoInstance(string method, byte[] bKey , byte[]? iv)
        {
            byte[] data = bKey.TryParseKey(method);
            return new M3uKeyInfoHelper(method, data, iv!);
        }

        public static IM3uKeyInfo GetKeyInfoInstance(IM3uKeyInfo m3UKeyInfo)
        {
            byte[] data = m3UKeyInfo.BKey.TryParseKey(m3UKeyInfo.Method);
            return new M3uKeyInfoHelper(m3UKeyInfo.Method, data, m3UKeyInfo.IV);
        }

        public static IM3uKeyInfo GetKeyInfoInstance(string method, string key)
        {
            return new M3uKeyInfoHelper(method, Encoding.UTF8.GetBytes(key), null!);
        }

        public static IM3uKeyInfo GetKeyInfoInstance(string method, string key, string iv)
        {
            return new M3uKeyInfoHelper(method, Encoding.UTF8.GetBytes(key), iv?.ToHex()!);
        }
    }
}
