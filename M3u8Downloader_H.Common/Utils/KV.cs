using System;
using System.Collections.Generic;

namespace M3u8Downloader_H.Common.Utils
{
    public static class KV
    {
        public static KeyValuePair<string, string> Parse(string text, char separator = ':')
        {
            if (string.IsNullOrEmpty(text))
                return new KeyValuePair<string, string>();

            var strArray = text.Split(
                [separator], 2, StringSplitOptions.RemoveEmptyEntries
            );

            if (strArray.Length == 2)
            {
                return new KeyValuePair<string, string>(strArray[0].Trim(),
                    (strArray.Length > 1 ? strArray[1] : string.Empty).Trim(' ', '"'));
            }

            return new KeyValuePair<string, string>();
        }
    }
}