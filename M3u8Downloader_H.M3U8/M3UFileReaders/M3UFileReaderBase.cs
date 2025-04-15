using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    public abstract class M3UFileReaderBase
    {
        private int currentIndex;
        protected string CurrentTitle => $"{++currentIndex}.tmp";
        protected Uri RequestUri = default!;

        public M3UFileReaderBase()
        {
        }

        public M3UFileReaderBase WithUri(Uri uri)
        {
            RequestUri = uri;
            return this;
        }

        /// <summary>
        /// 获取m3u8的key结构体,此方法一般只有在文件夹合并方案中才会使用到
        /// </summary>
        /// <param name="method">方法例如aes-128</param>
        /// <param name="uri">aes的请求地址</param>
        /// <param name="key">aes实际内容</param>
        /// <param name="iv">iv</param>
        /// <returns>IM3uKeyInfo的结构体</returns>
        protected IM3uKeyInfo? GetM3UKeyInfo(string? method, string? uri, string? key, string? iv)
        {
            if (uri == null && key == null && iv == null)
                return null;

            M3UKeyInfo m3UKeyInfo = new()
            {
                Method = method!,
                Uri = uri is not null ? RequestUri.Join(uri!) : default!,
                BKey = key is not null ? Encoding.UTF8.GetBytes(key!) : default!,
                IV = iv?.ToHex()!
            };
            return m3UKeyInfo;
        }

        /// <summary>
        /// 获取ts流行信息,需要下载的ts无需传入title参数,需要合并的必须传入
        /// </summary>
        /// <param name="uri">ts的地址</param>
        /// <param name="title">ts的名字</param>
        /// <returns>IM3uMediaInfo结构体</returns>
        protected IM3uMediaInfo GetM3UMediaInfo(string uri, string? title)
        {
            M3UMediaInfo m3UMediaInfo = new()
            {
                Uri = RequestUri.Join(uri),
                Title = string.IsNullOrWhiteSpace(title) ? CurrentTitle : title
            };
            return m3UMediaInfo;
        }

        public abstract IM3uFileInfo Read(Stream stream);
    }
}
