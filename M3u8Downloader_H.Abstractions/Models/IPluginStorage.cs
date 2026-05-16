using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface IPluginStorage
    {
        Stream OpenRead(string path);

        Stream OpenWrite(string path);

        string GetPath(string path);

        bool Exists(string path);

        DirectoryInfo CreateDirectory(string Dir);
    }
}
