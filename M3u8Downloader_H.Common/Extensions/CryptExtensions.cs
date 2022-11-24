using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.Common.Extensions
{
    public static class CryptExtensions
    {
        private static readonly Dictionary<string, (int, int)> KeyGroup = new() { { "AES-128", (16, 24) }, { "AES-192", (24, 32) }, { "AES-256", (32, 44) } };

        public static byte[] HmacSha256(this Stream memory, byte[] key)
        {
            using HMACSHA256 hmac = new(key);
            return hmac.ComputeHash(memory);
        }

        public static byte[] HmacSha256(this byte[] memory, byte[] key)
        {
            using HMACSHA256 hmac = new(key);
            return hmac.ComputeHash(memory);
        }

        public static byte[] AesEncrypt(this byte[] content, byte[] aesKey, byte[] aesIv)
        {
            using Aes _aes = Aes.Create();
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;
            _aes.Key = aesKey;
            _aes.IV = aesIv ?? new byte[16];

            using var _crypto = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            return _crypto.TransformFinalBlock(content, 0, content.Length);
        }

        public static Stream AesEncrypt(this Stream memory, byte[] aesKey, byte[] aesIv)
        {
            using Aes _aes = Aes.Create();
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;
            _aes.Key = aesKey;
            _aes.IV = aesIv ?? new byte[16];

            using var _crypto = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            return new CryptoStream(memory, _crypto, CryptoStreamMode.Write);
        }

        public static byte[] AesDecrypt(this byte[] content, byte[] aesKey, byte[] aesIV)
        {

            using Aes _aes = Aes.Create();
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;
            _aes.Key = aesKey;
            _aes.IV = aesIV ?? new byte[16];

            using var _crypto = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            return _crypto.TransformFinalBlock(content, 0, content.Length);
        }

        public static Stream AesDecrypt(this Stream memory, byte[] aesKey, byte[] aesIV)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Key = aesKey;
            aesAlg.IV = aesIV ?? new byte[16];

            using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            return new CryptoStream(memory, decryptor, CryptoStreamMode.Read);
        }

        public static byte[] TryParseKey(this byte[] data, string method)
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
            throw new InvalidCastException($"无法解析的密钥,请确定是否为AES-128,AES-192,AES-256");
        }
    }
}
