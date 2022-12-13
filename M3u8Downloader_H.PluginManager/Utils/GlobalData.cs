using System;

#if !DEBUG
using System.IO;
#endif

namespace M3u8Downloader_H.PluginManager.Utils
{
    public class GlobalData
    {
        public static Uri DataSetUrl { get; } = new("https://ghproxy.com/raw.githubusercontent.com/Harlan-H/M3u8Downloader_H.Plugins/develop/datasets.json");
        public static Uri DownloadBaseUrl { get; } = new("https://ghproxy.com/https://github.com/Harlan-H/M3u8Downloader_H.Plugins/releases/latest/download/");

        public static string PluginDirectory { get; } =
#if DEBUG
             "e:/desktop/Plugins/";
#else
             Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
#endif

        public static string AppPath { get; } =
#if DEBUG
            "E:\\MyDocument\\Visual Studio 2019\\Projects\\c#\\M3u8Downloader_H\\M3u8Downloader_H\\bin\\Debug\\net6.0-windows\\M3u8Downloader_H.exe";
#else
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "M3u8Downloader_H.exe");
#endif
    }
}
