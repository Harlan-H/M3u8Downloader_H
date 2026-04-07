using M3u8Downloader_H.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace M3u8Downloader_H.Common.Utils;

public class StorageSpaceManager
{
    public static string GetCachesPath()
    {
#if DEBUG
        return @"C:\Users\admin\Desktop\666\Caches";
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(StorageSpace.UserProfile.GetDirectoryPath(), "Library", "Caches", "M3u8Downloader_H");
        else
            return Path.Combine(StorageSpace.Instance.GetDirectoryPath(), "Caches");
#endif
    }

    public static string GetSavePath()
    {
#if DEBUG
            return @"C:\Users\admin\Desktop\666\download";
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(StorageSpace.UserProfile.GetDirectoryPath(), "Downloads", "M3u8Downloader_H");
        else
            return Path.Combine(StorageSpace.Instance.GetDirectoryPath(), "download");
#endif
    }


    public static string GetPluginPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(StorageSpace.UserDomain.GetDirectoryPath(), "M3u8Downloader_H", "Plugins");
        else
            return Path.Combine(StorageSpace.Instance.GetDirectoryPath(), "Plugins");

    }


    public static string GetConfigPath()
    {
#if DEBUG
        return @"C:\Users\admin\Desktop\666";
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(StorageSpace.UserDomain.GetDirectoryPath(), "M3u8Downloader_H");
        else
            return Path.Combine(StorageSpace.Instance.GetDirectoryPath());
#endif
    }

    public static string GetFFmpegPath()
    {
#if DEBUG
        return @"C:\Users\admin\Desktop\666\ffmpeg.exe";
#else
        if(OperatingSystem.IsWindows())
            return Path.Combine(StorageSpace.Instance.GetDirectoryPath(), "ffmpeg.exe");
        else 
            return Path.Combine(StorageSpace.Instance.GetDirectoryPath(), "ffmpeg");
#endif
    }
}
