using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace M3u8Downloader_H.Converters
{
    public abstract class BaseConverters<TFrom, TTo> : IValueConverter
    {
        public abstract TTo Convert(TFrom value, Type targetType, object? parameter, CultureInfo culture);
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {   if(value is TFrom val)
                {
                    return Convert((TFrom)value, targetType, parameter, culture);
                }
                return default;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public abstract TFrom ConvertBack(TTo value, Type targetType, object? parameter, CultureInfo culture);

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                if(value is TTo val)
                {
                    return ConvertBack(val, targetType, parameter, culture);
                }
                return default;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
