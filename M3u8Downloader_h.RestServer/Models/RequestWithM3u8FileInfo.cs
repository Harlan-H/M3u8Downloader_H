using M3u8Downloader_H.M3U8.Infos;
using M3u8Downloader_H.RestServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithM3u8FileInfo :RequestBase
    {
        [JsonPropertyName("content")]
        [Required(ExceptionMsg = "m3UFileInfo解析失败")]
        public M3UFileInfo M3u8FileInfo { get; set; } = default!;
    }
}
