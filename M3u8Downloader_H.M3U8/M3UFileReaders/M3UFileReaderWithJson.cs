using System.IO;
using System.Text.Json;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.Extensions;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    internal sealed class M3UFileReaderWithJson : M3UFileReaderBase
    {

        public override M3UFileInfo Read(Stream stream)
        {
            M3UFileInfo? m3ufileInfo = JsonSerializer.Deserialize<M3UFileInfo>(stream) ?? throw new InvalidDataException("不能是空的m3u8数据");               

            if (m3ufileInfo.Key is not null)
                m3ufileInfo.Key.Uri = m3ufileInfo.Key?.Uri != null ? RequestUri.Join(m3ufileInfo.Key?.Uri?.OriginalString!) : default!;

            for (int i = 0; i < m3ufileInfo.MediaFiles.Count; i++)
            {
                var mediaInfo = m3ufileInfo.MediaFiles[i];
                m3ufileInfo.MediaFiles[i] = GetM3UMediaInfo(mediaInfo.Uri.OriginalString, mediaInfo.Title);
            }

            m3ufileInfo.PlaylistType = "VOD";
            stream?.Dispose();
            return m3ufileInfo;
        }

    }
}
