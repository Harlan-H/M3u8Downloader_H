using M3u8Downloader_H.bilibili.plugin.Models;
using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Plugin;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.bilibili.plugin.Lives
{
    internal class LiveClient : IM3u8UriProvider
    {
        private static readonly Regex regex = new(@"__NEPTUNE_IS_MY_WAIFU__=(\{.*?\})</script>", RegexOptions.Compiled);
        public async Task<Uri> GetM3u8UriAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            if (uri.LocalPath.Contains("m3u8"))
                return uri;

            var raw = await GetLiveStreamInfo(httpClient,uri, headers, cancellationToken);
            var data= ExtractStreamInfos(raw);
            var requestUrl = GetRequestUrl(data);
            return new Uri(requestUrl, UriKind.Absolute);
        }

        private static async Task<string> GetLiveStreamInfo(HttpClient httpClient,LiveId liveId, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, liveId);
            request.AddHeaders(headers);
            if(!request.Headers.Contains("accept"))
            {
                request.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            }
            return await httpClient.GetStringAsync(request, cancellationToken);
        }

        private static string ExtractStreamInfos(string raw)
        {
            var initState = regex.Match(raw).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(initState))
                throw new InvalidDataException("没有获取到直播流得数据");

            return initState;
        }

        private static string GetRequestUrl(string raw)
        {
            var result = JsonConvert.DeserializeObject<LiveRoomInfos>(raw);
            if (result is null)
                throw new InvalidDataException("解析直播数据失败");

            if (result.StreamInfos is null)
                throw new InvalidDataException("主播可能没有开播");

            var requesturl = (from streaminfo in result.StreamInfos
                              where streaminfo.ProtocolName == "http_hls"
                              from codecinfo in streaminfo.CodecInfos
                              where codecinfo.FormatName == "ts"
                              select codecinfo.HostUrl + codecinfo.BaseUrl + codecinfo.Extra).FirstOrDefault();
            if (requesturl is null)
                throw new InvalidDataException("获取直播m3u8地址失败");

            return requesturl;
        }
    }
}
