using System;
using System.Globalization;

namespace M3u8Downloader_H.Converters
{
    public class DoubleToTimespanConverters : BaseConverters<double, string>
    {
        public static DoubleToTimespanConverters Instance { get; } = new DoubleToTimespanConverters();
        public override string Convert(double value, Type targetType, object? parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss", culture);
        }

        public override double ConvertBack(string value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
