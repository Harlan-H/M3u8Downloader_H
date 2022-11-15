using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class Response<T>
    {
        public int Code { get; }
        public string Message { get; }
        public T? Data { get; }

        public Response(int code, string message, T? data = default)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public static implicit operator byte[](Response<T> response) => JsonSerializer.SerializeToUtf8Bytes(response,new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
        });
    }

    internal class Response : Response<string>
    {
        public Response(int code, string message, string? data = null) : base(code, message, data)
        {

        }

        public static byte[] Success(string msg = "请求成功") => new Response(0, msg);

        public static byte[] Error(string msg) => new Response(1, msg);
    }

}
