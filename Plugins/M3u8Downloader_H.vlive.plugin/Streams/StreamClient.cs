using M3u8Downloader_H.Common.Extensions;
using M3u8Downloader_H.Plugin;
using M3u8Downloader_H.vlive.plugin.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H.vlive.plugin.Streams
{
    internal class StreamClient : IM3u8UriProvider
    {
        private static readonly Regex regex = new(@"__PRELOADED_STATE__=(\{.*?\}),function", RegexOptions.Compiled);
        public async Task<Uri> GetM3u8UriAsync(HttpClient httpClient, Uri uri, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            PostDetail postDetail = await GetPostDetail(uri, httpClient, headers, cancellationToken);
            string inKey = await GetInKey(postDetail, httpClient, headers, cancellationToken);
            return new VodPlayInfo(postDetail.VodId, inKey);
        }

        private static async Task<PostDetail> GetPostDetail(VideoId videoId, HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            var raw = await httpClient.GetStringAsync(videoId, headers, cancellationToken);
            var jsonRaw = ExtractInitState(raw);
            PostDetail? postDetail = JsonConvert.DeserializeObject<PostDetail>(jsonRaw!);
            if (postDetail is null)
                throw new InvalidDataException("主页关键数据反序列化失败");

            return postDetail;
        }

        private static async Task<string> GetInKey(PostDetail postDetail, HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers, CancellationToken cancellationToken = default)
        {
            Uri url =new($"https://www.vlive.tv/globalv-web/vam-web/video/v1.0/vod/{postDetail.LiveChatId}/inkey?appId=8c6cc7b45d2568fb668be6e05b6e5a3b&platformType=PC&gcc=CN&locale=zh_CN");
            using HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, url);
            httpRequestMessage.AddHeaders(headers);
            if (!httpRequestMessage.Headers.Contains("Referer"))
                httpRequestMessage.Headers.Add("Referer", "https://www.vlive.tv/");

            var raw = await httpClient.GetStringAsync(httpRequestMessage, cancellationToken);
            InkeyInfos? inkeyInfo = JsonConvert.DeserializeObject<InkeyInfos>(raw);
            if (inkeyInfo is null)
                throw new InvalidDataException("inkey信息反序列话失败");

            return inkeyInfo.Inkey;
        }

        private static string ExtractInitState(string raw)
        {
            var initState = regex.Match(raw).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(initState))
                throw new InvalidDataException("没有获取到网页关键数据");

            return initState;
        }
    }
}
