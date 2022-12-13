using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.PluginManager.Utils
{
    internal class FileEx
    {
        public static int VersionCompareTo(string version1, string version2)
        {
            var version = new Version(version1);
            var otherVersion = new Version(version2);
            return version.CompareTo(otherVersion);
        }

        public static string GetFileVersion(string path)
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
            return fileVersionInfo.FileVersion ?? string.Empty;
        }
    }
}
