using System;
using System.Diagnostics;

namespace M3u8Downloader_H.Extensions
{
    internal static  class ProcessExtensions
    {
        extension(Process)
        {
            public static void ShowFile(string fileName)
            {

                if (OperatingSystem.IsWindows())
                {
                    Process.Start("explorer.exe", $"/select,\"{fileName}\"");
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", $"-R \"{fileName}\"");
                }
                else if (OperatingSystem.IsLinux())
                {
                    Process.Start("xdg-open", System.IO.Path.GetDirectoryName(fileName)!);
                }
            }
        }
    }
}
