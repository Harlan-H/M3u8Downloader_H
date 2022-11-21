namespace M3u8Downloader_H.bilivideo.plugin.Extensions
{
    internal static class StringExtension
    {
        public static string GetPath(this string s)
        {
            int pos = s.IndexOf('?');
            return pos == -1 ? s : s.Remove(pos);
        }

        public static string? GetQuery(this string s)
        {
            int pos = s.IndexOf('?');
            return pos == -1 ? null : s[(pos + 1)..];
        }

        public static Dictionary<string, string>? GetQueryKeyPairs(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            var splitArr = s.Split("=");
            Dictionary<string, string> queryDict = new();
            for (int i = 0; i < splitArr.Length; i+=2)
            {
                queryDict.Add(splitArr[i], splitArr[i + 1]);
            }
            return queryDict;
        }
    }
}
