using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace M3u8Downloader_H.Converters;

public class EqualityConverter(bool isInverted) : IValueConverter
{
    public static EqualityConverter IsEqual { get; } = new(false);
    public static EqualityConverter IsNotEqual { get; } = new(true);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return EqualityComparer<object>.Default.Equals(value, parameter) != isInverted;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
