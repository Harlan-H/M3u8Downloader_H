using M3u8Downloader_H._555dd7.plugin.Models;
using M3u8Downloader_H._555dd7.plugin.Utils;
using M3u8Downloader_H.Common.Extensions;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace M3u8Downloader_H._555dd7.plugin.Streams
{
    internal class Extractor
    {
        private readonly HttpClient httpClient;
        private readonly IEnumerable<KeyValuePair<string, string>>? headers;
        private static readonly Regex regex = new("player_aaaa=(.*?)</script>", RegexOptions.Compiled);
        private static readonly Regex wssRegex = new("'(wss://.*?)'", RegexOptions.Compiled);
        private static readonly byte[] _key = Encoding.UTF8.GetBytes("55ca5c48a943afdc");
        private static readonly byte[] _iv = Encoding.UTF8.GetBytes("d11424dcecfe16c0");

        public Extractor(HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers)
        {
            this.httpClient = httpClient;
            this.headers = headers;
        }

        public async Task<Uri> GetM3u8IndexUrl(VideoId videoId,CancellationToken cancellationToken)
        {
            Uri uri = videoId;
            var url = await GetMainPageUrl(uri, cancellationToken);
            var encryptedData = GetEncryptData(new(url));
            var wssUrl = await GetWssAddress(uri,cancellationToken);
            var recvBytes = await SendData(new Uri(wssUrl), encryptedData.ToBytes(), cancellationToken);
            return GetPlayUrl(recvBytes);
        }

        private static Uri GetPlayUrl(byte[] bytes)
        {
            var decryptText = bytes.AesDecrypt(_key, _iv).GetString();
            var r = JsonConvert.DeserializeObject<Response>(decryptText);
            if (r is null)
                throw new InvalidDataException("返回内容序列化失败");

            if (r.Code != 200)
                throw new InvalidDataException($"数据返回异常,错误:{r.Msg}");

            return r.Url;
        }

        private static async Task<byte[]> SendData(Uri wssuri, byte[] buffer, CancellationToken cancellationToken)
        {
            using WebSocketClient webSocketClient = new();
            var wsResp = await webSocketClient.SendAsync(wssuri, buffer, cancellationToken);
            return wsResp.GetString().ToHexBytes();
        }

        private async Task<string> GetMainPageUrl(Uri uri, CancellationToken cancellationToken)
        {
            var raw = await httpClient.GetStringAsync(uri, headers, cancellationToken);
            var initState = ExtractInitState(raw);
            PlayerInfo? playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(initState);
            if (playerInfo is null)
                throw new InvalidDataException("主页关键数据反序列化失败");

            return playerInfo.Url;
        }

        private async Task<string> GetWssAddress(Uri baseUri, CancellationToken cancellationToken = default)
        {
            Uri uri = new(baseUri, "/player.html");
            var raw = await httpClient.GetStringAsync(uri, headers, cancellationToken);
            var wssGroups = wssRegex.Matches(raw);
            if (wssGroups.Count < 2)
                throw new InvalidDataException("wss地址获取失败");

            var randomNum = new Random().Next(wssGroups.Count);
            return wssGroups[randomNum].Groups[1].Value;
        }

        private static string ExtractInitState(string raw)
        {
            var initState = regex.Match(raw).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(initState))
                throw new InvalidDataException("没有获取到网页关键数据");

            return initState;
        }

        private string GetEncryptData(WsRequestMessage message)
        {
            var MessageContentBytes = message.ToString().ToBytes();
            var encryptedData = MessageContentBytes.AesEncrypt(_key, _iv);
            return encryptedData.GetHexString();
        }
    }
}
