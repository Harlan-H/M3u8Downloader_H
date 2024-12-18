using System.IO;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Combiners.M3uCombiners
{
    internal class CryptM3uCombiner(M3UFileInfo m3UFileInfo, string videoFullName) : M3uCombiner(videoFullName)
    {
        private readonly M3UFileInfo m3UFileInfo = m3UFileInfo;

        public override void Initialization(string videoName)
        {
            base.Initialization(videoName);

            if (m3UFileInfo.Key is null)
                throw new InvalidDataException("没有可用的key");

            if (m3UFileInfo.Key.Uri != null && !m3UFileInfo.Key.Uri.IsFile)
                throw new InvalidDataException("传入的key不能是网络文件");

            if (m3UFileInfo.Key.Uri != null && m3UFileInfo.Key.BKey == null)
            {
                byte[] data = File.ReadAllBytes(m3UFileInfo.Key.Uri.OriginalString);
                m3UFileInfo.Key.BKey = data.TryParseKey(m3UFileInfo.Key.Method);
            }else if (m3UFileInfo.Key.BKey != null)
            {
                m3UFileInfo.Key.BKey = m3UFileInfo.Key.BKey.TryParseKey(m3UFileInfo.Key.Method);     
            }else
            {
                throw new InvalidDataException("key是空的");
            }                 
        }

        protected override Stream HandleData(string path)
        {
            var tsStream = base.HandleData(path);
            return tsStream.AesDecrypt(m3UFileInfo.Key.BKey, m3UFileInfo.Key.IV);
        }

    }
}
