using System;

namespace M3u8Downloader_H.Settings.Services
{
    public enum StorageSpace
    {
        SyncedUserDomain,
        UserDomain,
        MachineDomain,
        Instance
    }

    internal static class StorageSpaceExtensions
    {
        public static string GetDirectoryPath(this StorageSpace storageSpace)
        {
            return storageSpace switch
            {
                StorageSpace.SyncedUserDomain => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                StorageSpace.UserDomain => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                StorageSpace.MachineDomain => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                StorageSpace.Instance => AppDomain.CurrentDomain.BaseDirectory,
                _ => throw new ArgumentOutOfRangeException(nameof(storageSpace), storageSpace, null)
            };
        }
    }
}
