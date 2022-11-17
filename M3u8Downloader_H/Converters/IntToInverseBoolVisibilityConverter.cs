using System;
using System.Globalization;
using System.Windows;

namespace M3u8Downloader_H.Converters
{
    class IntToInverseBoolVisibilityConverter : BaseConverters<int, Visibility>
    {
        public static IntToInverseBoolVisibilityConverter Instance { get; } = new();

        public override Visibility Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public override int ConvertBack(Visibility value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
