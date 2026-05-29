using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace M3u8Downloader_H.M3U8.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<TEnum>(this TEnum value)
                where TEnum : Enum
        {
            var field = typeof(TEnum).GetField(value.ToString());

            return field?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description
                ?? value.ToString();
        }
    }
}
