using Newtonsoft.Json;

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

        public override string ToString() => JsonConvert.SerializeObject(this);

        public static implicit operator string(Response<T> response) => response.ToString();
    }

    internal class Response : Response<string>
    {
        public Response(int code, string message, string? data = null) : base(code, message, data)
        {

        }

        public static string Success(string msg = "请求成功") => new Response(0, msg);

        public static string Error(string msg) => new Response(1, msg);
    }

}
