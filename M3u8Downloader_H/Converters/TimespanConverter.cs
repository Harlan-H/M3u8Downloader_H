using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace M3u8Downloader_H.Converters
{
    internal class TimespanConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Format: Minutes:Seconds
        /// </summary>
        public const string TimeSpanFormatString = @"mm\:ss";

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            var timespanFormatted = $"{value.ToString(TimeSpanFormatString)}";
            writer.WriteValue(timespanFormatted);
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            TimeSpan.TryParseExact((string)reader.Value!, TimeSpanFormatString, null, out TimeSpan parsedTimeSpan);
            return parsedTimeSpan;
        }
    }
}
