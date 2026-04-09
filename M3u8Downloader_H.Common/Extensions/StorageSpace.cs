using System;

namespace M3u8Downloader_H.Common.Extensions
{
    public enum StorageSpace
    {
        SyncedUserDomain,
        UserDomain,
        MachineDomain,
        UserProfile,
        Instance
    }

    public static class StorageSpaceExtensions
    {

        extension(StorageSpace storageSpace) {

            public string GetDirectoryPath()
            {
                return storageSpace switch
                {
                    StorageSpace.SyncedUserDomain => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    StorageSpace.UserDomain => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    StorageSpace.MachineDomain => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    StorageSpace.UserProfile => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    StorageSpace.Instance => AppDomain.CurrentDomain.BaseDirectory,
                    _ => throw new ArgumentOutOfRangeException(nameof(storageSpace), storageSpace, null)
                };
            }
         
        }
       
    }
}
