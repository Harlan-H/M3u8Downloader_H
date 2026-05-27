using M3u8Downloader_H.M3U8.AttributeReader.Attributes;
using M3u8Downloader_H.M3U8.Models;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.M3U8.Utilities;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace M3u8Downloader_H.M3U8.AttributeReaders
{
    [M3U8Reader("#EXT-X-KEY", typeof(KeyAttributeReader))]
    internal class KeyAttributeReader : AttributeReader
    {
        public override Task WriteAsync(M3UFileInfo fileInfo, string value, IAsyncEnumerator<string> reader, Uri baseUri)
        {
            if (fileInfo.Key is not null) return Task.CompletedTask;

            var source =
                value.Split([','], StringSplitOptions.RemoveEmptyEntries)
                     .Select(e => KV.Parse(e, '='))
                     .ToList();

            if (source.Count == 0) return Task.CompletedTask;

            var m3ukeyinfo = new M3UKeyInfo();
            foreach (var keyValuePair in source)
            {
                switch (keyValuePair.Key)
                {
                    case "METHOD":
                        if (keyValuePair!.Value!.Equals("NONE", StringComparison.Ordinal)) return Task.CompletedTask;

                        m3ukeyinfo.Method = keyValuePair.Value;
                        break;
                    case "URI":
                        {
                            //如果是地址
                            if (Uri.TryCreate(keyValuePair.Value, UriKind.RelativeOrAbsolute, out Uri? relativeUri))
                            {
                                if (!relativeUri.IsAbsoluteUri)
                                {
                                    if (baseUri != null)
                                        m3ukeyinfo.Uri = new Uri(baseUri, relativeUri);
                                }
                                else
                                {
                                    m3ukeyinfo.Uri = relativeUri;
                                }
                            }
                            else
                            {
                                //否则密钥是显示再文件中的 不需要请求地址
                                m3ukeyinfo.BKey = Convert.FromBase64String(keyValuePair.Value);
                            }
                        }
                        break;
                    case "IV":
                        m3ukeyinfo.IV = keyValuePair.Value.ToHex();
                        break;
                }
            }
            fileInfo.Key = m3ukeyinfo;
            return Task.CompletedTask;
        }


    }
}