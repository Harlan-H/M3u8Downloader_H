using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.M3U8.Models;
using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithM3u8FileInfo :RequestBase
    {
        [JsonPropertyName("content")]
        [Required(ExceptionMsg = "m3UFileInfo解析失败")]
        public IM3uFileInfo M3UFileInfos { get; set; } = default!;   
    }

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(RequestWithM3u8FileInfo))]
    internal partial class RequestWithM3u8FileInfoContext : JsonSerializerContext;
}
