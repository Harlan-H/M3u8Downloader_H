using System;
using System.Globalization;
using System.Windows;

namespace M3u8Downloader_H.PluginManager.Converters
{
    internal class IntToBoolVisibilityConverter : BaseConverters<int, Visibility>
    {
        public static IntToBoolVisibilityConverter Instance { get; } = new IntToBoolVisibilityConverter();
        public override Visibility Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public override int ConvertBack(Visibility value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
