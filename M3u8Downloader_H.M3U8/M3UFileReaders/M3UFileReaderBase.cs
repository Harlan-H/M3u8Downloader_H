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

        protected M3UKeyInfo? GetM3UKeyInfo(string? method, string? uri, string? key, string? iv)
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


        protected M3UMediaInfo GetM3UMediaInfo(string uri, string? title)
        {
            M3UMediaInfo m3UMediaInfo = new()
            {
                Uri = RequestUri.Join(uri),
                Title = string.IsNullOrWhiteSpace(title) ? CurrentTitle : title
            };
            return m3UMediaInfo;
        }

        public abstract M3UFileInfo Read(Stream stream);
    }
}
