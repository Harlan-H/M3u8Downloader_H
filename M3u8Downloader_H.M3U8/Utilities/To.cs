using System;

namespace M3u8Downloader_H.M3U8.Utilities
{
    public static class To
    {
        private static bool? Bool(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new bool?();
            if (string.Equals(text, "YES"))
                return true;

            return string.Equals(text, "NO") ? false : new bool?(Convert.ToBoolean(text));
        }

        public static T? Value<T>(string text) where T : struct
        {
            if (string.IsNullOrWhiteSpace(text))
                return new T?();

            
            var conversionType = typeof(T);
            if (conversionType == typeof(bool))
            {
                ValueType? valueType = Bool(text);
                return (T?)valueType;
            }

            try
            {
                return (T)Convert.ChangeType(text, conversionType);
            }
            catch
            {
                return new T?();
            }
        }



    }
}