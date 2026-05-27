using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.M3U8.Models;
using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.M3U8.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-MEDIA", typeof(MediaManifestAttributeReader))]
    internal class MediaManifestAttributeReader : AttributeReader
    {
        public override Task WriteAsync(M3UFileInfo m3UFileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            m3UFileInfo.Medias ??= [];
            var m3UmediaManifest = new M3UMediaManifest();
            var source = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(e => KV.Parse(e, '='))
                                .ToList();

            foreach (var keyValuePair in source)
            {
                switch (keyValuePair.Key)
                {
                    case "URI":
                        {
                            var relativeUri = new Uri( keyValuePair.Value);
                            m3UmediaManifest.Uri = relativeUri.IsAbsoluteUri ? relativeUri: new Uri(baseUri, relativeUri);
                        }
                        break;
                    case "TYPE":
                        m3UmediaManifest.Type = keyValuePair.Value;
                        break;
                    case "GROUP-ID":
                        m3UmediaManifest.GroupId = keyValuePair.Value;
                        break;
                    case "LANGUAGE":
                        m3UmediaManifest.Language = keyValuePair.Value;
                        break;
                    case "NAME":
                        m3UmediaManifest.Name = keyValuePair.Value;
                        break;
                    case "DEFAULT":
                        m3UmediaManifest.Default = keyValuePair.Value;
                        break;
                    case "AUTOSELECT":
                        m3UmediaManifest.AutoSelect = keyValuePair.Value;
                        break;
                    case "CHANNELS":
                        m3UmediaManifest.Channels = keyValuePair.Value;
                        break;
                }
            }
            if(m3UmediaManifest.Type == "CLOSED-CAPTIONS")
                return Task.CompletedTask;

            m3UFileInfo.Medias.Add(m3UmediaManifest);
            return Task.CompletedTask;
        }
    }
}
