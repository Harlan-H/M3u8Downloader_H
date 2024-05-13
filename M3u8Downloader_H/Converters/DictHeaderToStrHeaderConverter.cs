using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace M3u8Downloader_H.Converters
{
    public class DictHeaderToStrHeaderConverter : BaseConverters<IEnumerable<KeyValuePair<string, string>>, string>
    {
        public static DictHeaderToStrHeaderConverter Instance { get; } = new();
        public override string Convert(IEnumerable<KeyValuePair<string, string>> value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;

            StringBuilder stringBuilder = new();
            foreach (var item in value)
            {
                stringBuilder.Append(item.Key);
                stringBuilder.Append(':');
                stringBuilder.Append(item.Value);
                stringBuilder.Append(Environment.NewLine);
            }
            return stringBuilder.ToString();
        }

        public override IEnumerable<KeyValuePair<string, string>> ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value)) return null!;

            Dictionary<string,string> tmpHeader = value.Split(Environment.NewLine)
                    .Select(h => h.Split(":", 2, StringSplitOptions.TrimEntries))
                    .ToDictionary(r => r[0], r => r[1]);

            return tmpHeader;
        }
    }
}
