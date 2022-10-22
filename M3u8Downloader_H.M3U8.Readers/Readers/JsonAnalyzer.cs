using System;
using System.IO;
using System.Text.Json;
using M3u8Downloader_H.M3U8.Extensions;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.M3U8.Readers
{
    internal sealed class JsonAnalyzer : AnalyzerBase
    {
        private readonly Uri host;

        public JsonAnalyzer(Uri host) : base(host)
        {
            this.host = host;
        }

        public override M3UFileInfo Read()
        {
            M3UFileInfo? m3ufileInfo = JsonSerializer.Deserialize<M3UFileInfo>(File.ReadAllText(host.OriginalString));
            if(m3ufileInfo is null)
                throw new InvalidDataException("不能是空的m3u8数据");

            if (m3ufileInfo.Key is not null)
                m3ufileInfo.Key.Uri = m3ufileInfo.Key?.Uri != null ? host.Join(m3ufileInfo.Key?.Uri?.OriginalString!) : default!;

            for (int i = 0; i < m3ufileInfo.MediaFiles.Count; i++)
            {
                var mediaInfo = m3ufileInfo.MediaFiles[i];
                m3ufileInfo.MediaFiles[i] = GetM3UMediaInfo(mediaInfo.Uri.OriginalString, mediaInfo.Title);
            }

            m3ufileInfo.PlaylistType = "VOD";
            return m3ufileInfo;
        }

    }
}
