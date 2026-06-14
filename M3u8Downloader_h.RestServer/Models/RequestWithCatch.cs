using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Common.DownloadPrams;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace M3u8Downloader_H.RestServer.Models
{
    internal class RequestWithCatch
    {
        public string Action { get; set; } = default!;

        [JsonConverter(typeof(SingleOrArrayConverter<DataInfo>))]
        public List<DataInfo> Data { get; set; } = default!;
    }

    internal class DataInfo
    {
        public Uri Url  { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Referer { get; set; } = default!;
        public string Origin { get; set; } = default!;
        public string Cookie { get; set; } = default!;
        public string Ext { get; set; } = default!;
    }

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(RequestWithCatch))]
    internal partial class RequestWithCatchContext : JsonSerializerContext;

    public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
    {
        public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<T>>( ref reader, options);
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var item = JsonSerializer.Deserialize<T>( ref reader,  options);
                return item == null? []: [item];
            }

            return [];
        }

        public override void Write( Utf8JsonWriter writer, List<T> value,JsonSerializerOptions options)
        {
            JsonSerializer.Serialize( writer, value, options);
        }
    }
} 
