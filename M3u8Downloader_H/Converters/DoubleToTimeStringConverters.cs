using System;
using System.Globalization;

namespace M3u8Downloader_H.Converters
{
    public class DoubleToTimeStringConverters : BaseConverters<double, string>
    {
        public static DoubleToTimeStringConverters Instance { get; } = new DoubleToTimeStringConverters();
        public override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds(value).ToString();
        }

        public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.ParseExact(value, "g", CultureInfo.InvariantCulture).TotalSeconds;
        }
    }
}
