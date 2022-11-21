using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace M3u8Downloader_H.M3U8.Utilities
{
    internal class SpecialComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null || y == null)
                throw new InvalidDataException("无效得文件名称必须以数字命名");

            string rawX = Path.GetFileNameWithoutExtension(x);
            string rawY = Path.GetFileNameWithoutExtension(y);

            int nX = Convert.ToInt32(rawX, CultureInfo.CurrentCulture);
            int nY = Convert.ToInt32(rawY, CultureInfo.CurrentCulture);

            return nX - nY;
        }
    }
}
