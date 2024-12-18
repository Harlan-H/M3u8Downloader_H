using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace M3u8Downloader_H.Common.Utils
{
    public class JsonPathConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var targetObj = Activator.CreateInstance(objectType);

            foreach (var prop in objectType.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var jsonPropertyAttr = prop.GetCustomAttributes(true).OfType<JsonPropertyAttribute>().FirstOrDefault() 
                        ?? throw new JsonReaderException($"{nameof(JsonPropertyAttribute)} is mandatory when using {nameof(JsonPathConverter)}");

                var jsonPath = jsonPropertyAttr.PropertyName;
                var token = jObject.SelectToken(jsonPath!);

                if (token != null && token.Type != JTokenType.Null)
                {
                    var jsonConverterAttr = prop.GetCustomAttributes(true).OfType<JsonConverterAttribute>().FirstOrDefault();
                    object? value;
                    if (jsonConverterAttr is null)
                    {
                        serializer.Converters.Clear();
                        value = token.ToObject(prop.PropertyType, serializer);
                    }
                    else
                    {
                        value = JsonConvert.DeserializeObject(token.ToString(), prop.PropertyType,
                            (JsonConverter?)Activator.CreateInstance(jsonConverterAttr.ConverterType)!);
                    }
                    prop.SetValue(targetObj, value, null);
                }
            }

            return targetObj;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
