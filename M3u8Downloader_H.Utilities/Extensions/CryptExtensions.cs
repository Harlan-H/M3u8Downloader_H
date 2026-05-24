using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace M3u8Downloader_H.Common.Extensions
{
    public static class CryptExtensions
    {

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

            var _crypto = _aes.CreateEncryptor(_aes.Key, _aes.IV);
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

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            return new CryptoStream(memory, decryptor, CryptoStreamMode.Read);
        }

    }
}
