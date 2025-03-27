using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.RestServer.Attributes;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithM3u8FileInfo :RequestBase
    {
        [JsonPropertyName("content")]
        [Required(ExceptionMsg = "m3UFileInfo解析失败")]
        public IM3uFileInfo M3UFileInfos { get; set; } = default!;

        public IDownloadParamBase ToDownloadParam()
        {
            if (!M3UFileInfos.MediaFiles.Any())
                throw new ArgumentException("m3u8的数据不能为空");

            return new DownloadParamsBase(M3UFileInfos.MediaFiles[0].Uri, VideoName,null, SavePath, "mp4", Headers);
        }
   
    }
}
