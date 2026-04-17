using System;
using System.IO;
using System.Text;
using System.Text.Json;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.Extensions;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    internal sealed class M3UFileReaderWithJson(Uri baseUri) : IM3uFileReader
    {
        private int currentIndex;
        private string CurrentTitle => $"{++currentIndex}.tmp";

        public void InitAttributeReade(IAttributeReaderCollection readers)
        {
           
        }

        public IM3uFileInfo GetM3u8FileInfo(Stream stream)
        {
            M3UFileInfo? m3ufileInfo = JsonSerializer.Deserialize<M3UFileInfo>(stream) ?? throw new InvalidDataException("不能是空的m3u8数据");

            M3UKeyInfo? m3UKeyInfo = m3ufileInfo.Key as M3UKeyInfo;
            if (m3UKeyInfo is not null)
            {
                m3UKeyInfo.Uri = m3ufileInfo.Key?.Uri != null ? baseUri.Join(m3ufileInfo.Key?.Uri?.OriginalString!) : default!;
            }

            for (int i = 0; i < m3ufileInfo.MediaFiles.Count; i++)
            {
                var mediaInfo = m3ufileInfo.MediaFiles[i];
                m3ufileInfo.MediaFiles[i] = GetM3UMediaInfo(mediaInfo.Uri.OriginalString, mediaInfo.Title);
            }

            m3ufileInfo.PlaylistType = "VOD";
            stream?.Dispose();
            return m3ufileInfo;
        }


        public IM3uKeyInfo? GetM3UKeyInfo(string? method, string? uri, string? key, string? iv)
        {
            if (uri == null && key == null && iv == null)
                return null;

            M3UKeyInfo m3UKeyInfo = new()
            {
                Method = method!,
                Uri = uri is not null ? baseUri.Join(uri!) : default!,
                BKey = key is not null ? Encoding.UTF8.GetBytes(key!) : default!,
                IV = iv?.ToHex()!
            };
            return m3UKeyInfo;
        }

        public IM3uMediaInfo GetM3UMediaInfo(string uri, string? title)
        {
            M3UMediaInfo m3UMediaInfo = new()
            {
                Uri = baseUri.Join(uri),
                Title = string.IsNullOrWhiteSpace(title) ? CurrentTitle : title
            };
            return m3UMediaInfo;
        }

    }
}
