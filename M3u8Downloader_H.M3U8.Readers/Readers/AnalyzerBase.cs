using System;
using System.Linq;
using System.Text;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.Readers
{
    public abstract class AnalyzerBase
    {
        private readonly Uri host;
        private int currentIndex;
        protected string CurrentTitle => $"{++currentIndex}.tmp";
        public AnalyzerBase(Uri host)
        {
            this.host = host;
        }

        protected M3UKeyInfo? GetM3UKeyInfo(string? method, string? uri, string? key, string? iv)
        {
            if (uri == null && key == null && iv == null)
                return null;

            M3UKeyInfo m3UKeyInfo = new()
            {
                Method = method!,
                Uri = uri is not null ? host.Join(uri!) : default!,
                BKey = key is not null ? Encoding.UTF8.GetBytes(key!) : default!,
                IV =  iv?.ToHex()!
            };
            return m3UKeyInfo;
        }


        protected M3UMediaInfo GetM3UMediaInfo(string uri, string? title)
        {
            M3UMediaInfo m3UMediaInfo = new()
            {
                Uri = host.Join(uri),
                Title = string.IsNullOrWhiteSpace(title) ? CurrentTitle : title
            };
            return m3UMediaInfo;
        }

        public abstract M3UFileInfo Read();

        public static implicit operator M3UFileInfo(AnalyzerBase jsonAnalyer) => jsonAnalyer.Read();
    }
}
