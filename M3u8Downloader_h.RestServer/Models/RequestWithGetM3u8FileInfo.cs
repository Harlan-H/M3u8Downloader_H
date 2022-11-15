using M3u8Downloader_H.RestServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithGetM3u8FileInfo : IValidate
    {
        [Required(ExceptionMsg = "content不能为空")]
        public string Content { get; set; } = default!;

        [JsonPropertyName("baseurl")]
        public Uri? Url { get; set; }
    }
}
