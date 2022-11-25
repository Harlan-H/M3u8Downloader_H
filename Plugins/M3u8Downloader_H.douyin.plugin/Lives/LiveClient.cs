using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.douyin.plugin.Models;
using M3u8Downloader_H.Plugin;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.douyin.plugin.Lives
{
    internal class LiveClient : IM3u8UriProvider
    {
        private static readonly Regex regex = new(@"id=""RENDER_DATA""[\s]type=""application/json"">(.*?)</script>", RegexOptions.Compiled);
        public async Task<Uri> GetM3u8UriAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            var url = await GetLiveUrl(uri, httpClient, headers, cancellationToken);
            return new Uri(url, UriKind.Absolute);
        }

        private static async Task<string> GetLiveUrl(LiveId liveId,HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, liveId);
            httpRequestMessage.AddHeaders(headers);
            if(!httpRequestMessage.Headers.Contains("Cookie"))
            {
                httpRequestMessage.Headers.Add("Cookie", "__ac_nonce=063804b7b008ae876eeb");
            }
            var raw = await httpClient.GetStringAsync(httpRequestMessage, cancellationToken);
            var initState = ExtractStreamInfos(raw);
            string decodeData = System.Web.HttpUtility.UrlDecode(initState);
            StreamInfos? streamInfos = JsonConvert.DeserializeObject<StreamInfos>(decodeData);
            if (streamInfos is null)
                throw new InvalidDataException("数据解析失败");

            if (streamInfos.Url is null)
                throw new InvalidDataException("没有获取到任何m3u8地址，请确认主播是否开播");

            return streamInfos.Url;
        }


        private static string ExtractStreamInfos(string raw)
        {
            var initState = regex.Match(raw).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(initState))
                throw new InvalidDataException("没有获取到直播流得数据");

            return initState;
        }
    }
}
