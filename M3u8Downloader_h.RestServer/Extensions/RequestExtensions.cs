using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using M3u8Downloader_H.RestServer.Models;


namespace M3u8Downloader_H.RestServer.Extensions
{
    internal static class RequestExtensions
    {
        extension(RequestWithMediaUri requestWithMediaUri)
        {
            public IMediaDownloadParam ToMediaDownloadParams()
            {
                (Uri, long?)? audio = requestWithMediaUri.Audio is not null ? (requestWithMediaUri.Audio.Url, requestWithMediaUri.Audio.FileSize) : null;
                return new MediaDownloadParams(requestWithMediaUri.SavePath, (requestWithMediaUri.Video.Url, requestWithMediaUri.Video.FileSize), audio, requestWithMediaUri.VideoName, requestWithMediaUri.Headers)
                {
                    IsVideoStream = requestWithMediaUri.IsVideoStream,
                };
            }
        }

        extension(RequestWithM3u8FileInfo requestWithM3U8FileInfo)
        {
            public IDownloadParamBase ToDownloadParam()
            {
                if (!requestWithM3U8FileInfo.M3UFileInfos.MediaFiles.Any())
                    throw new ArgumentException("m3u8的数据不能为空");

                return new DownloadParamsBase(requestWithM3U8FileInfo.M3UFileInfos.MediaFiles[0].Uri, requestWithM3U8FileInfo.VideoName, requestWithM3U8FileInfo.SavePath, "mp4", requestWithM3U8FileInfo.Headers);
            }
        }

        extension(RequestWithURI requestWithURI)
        {
            public IM3u8DownloadParam ToM3u8DownloadParams()
                => new M3u8DownloadParams(requestWithURI.RequestUrl, requestWithURI.VideoName, requestWithURI.SavePath, "mp4", requestWithURI.Headers, requestWithURI.Method, requestWithURI.Key, requestWithURI.Iv);
        }

        extension(RequestWithCatch)
        {
            public static IM3u8DownloadParam ToM3u8DownloadParams(DataInfo dataInfo)
            {
                Dictionary<string, string> headers = [];
                if (!string.IsNullOrEmpty(dataInfo.Referer))
                {
                    headers.Add("referer", dataInfo.Referer);
                }
                if (!string.IsNullOrEmpty(dataInfo.Origin))
                {
                    headers.Add("origin", dataInfo.Origin);
                }
                if (!string.IsNullOrEmpty(dataInfo.Cookie))
                {
                    headers.Add("cookie", dataInfo.Cookie);
                }

                return new M3u8DownloadParams(dataInfo.Url, dataInfo.Title, string.Empty, "mp4", headers);
            }

            public static IMediaDownloadParam ToMediaDownloadParams(List<DataInfo> DataInfos)
            {
                var firstData = DataInfos[0];
                string videName = firstData.Title;
                Dictionary<string, string> headers = [];
                if (!string.IsNullOrEmpty(firstData.Referer))
                {
                    headers.Add("referer", firstData.Referer);
                }
                if (!string.IsNullOrEmpty(firstData.Origin))
                {
                    headers.Add("origin", firstData.Origin);
                }
                if (!string.IsNullOrEmpty(firstData.Cookie))
                {
                    headers.Add("cookie", firstData.Cookie);
                }


                (Uri, long?)? audio = DataInfos.Count > 1
                    ? (DataInfos[1].Url, null)
                    : null;
                return new MediaDownloadParams(string.Empty, (firstData.Url, null), audio, videName, headers)
                {
                    IsVideoStream = true,
                };
            }
        }
    }
}
