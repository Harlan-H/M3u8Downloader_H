using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class Response<T>(int code, string message, T? data = default)
    {
        public static JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
        };

        public int Code { get; } = code;
        public string Message { get; } = message;
        public T? Data { get; } = data;

        public static implicit operator byte[](Response<T> response) => JsonSerializer.SerializeToUtf8Bytes(response, Response.jsonSerializerOptions);
    }

    internal class Response(int code, string message, string? data = null) : Response<string>(code, message, data)
    {
        public static byte[] Success(string msg = "请求成功") => new Response(0, msg);

        public static byte[] Error(string msg) => new Response(1, msg);
    }

}
