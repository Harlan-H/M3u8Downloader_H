using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithM3u8FileInfo :RequestBase
    {
        [JsonPropertyName("content")]
        [Required(ExceptionMsg = "m3UFileInfo解析失败")]
        public M3UFileInfo M3u8FileInfo { get; set; } = default!;
    }
}
