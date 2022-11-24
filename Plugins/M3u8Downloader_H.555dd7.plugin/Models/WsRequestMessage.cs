using M3u8Downloader_H.Common.Extensions;
using Newtonsoft.Json;
using System.Text;

namespace M3u8Downloader_H._555dd7.plugin.Models
{
    internal class WsRequestMessage
    {
        private readonly byte[] _hmacKey = Encoding.UTF8.GetBytes("55ca5c4d11424dcecfe16c08a943afdc");

        [JsonProperty("type")]
        public string Type { get; } = "getUrl";

        [JsonProperty("url")]
        public string Url { get; } = default!;

        [JsonProperty("sign")]
        public string Sign
        {
            get
            {
                var urlBytes = Encoding.UTF8.GetBytes(Url);
                byte[] encryptData = urlBytes.HmacSha256(_hmacKey);
                return Convert.ToHexString(encryptData).ToLower();
            }
        }

        public WsRequestMessage(string url)
        {
            Url = url;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
