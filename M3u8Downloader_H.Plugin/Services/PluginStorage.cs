using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Common.Utils;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginStorage(string key) : IPluginStorage
    {
        private readonly string _rootPath = Path.Combine(StorageSpaceManager.GetPluginDataPath(), key);

        public bool Exists(string path)
            => File.Exists(EscapePath(path));

        public string GetPath(string path)
            => EscapePath(path);

        public Stream OpenRead(string path)
            => File.OpenRead(EscapePath(path));

        public Stream OpenWrite(string path)
            => File.OpenWrite(EscapePath(path));

        public DirectoryInfo CreateDirectory(string Dir)
            => Directory.CreateDirectory(EscapePath(Dir));

        private string EscapePath(string path)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_rootPath, path));
            var rootPath = Path.GetFullPath(_rootPath);
            if(!fullPath.StartsWith(rootPath))
                throw new InvalidOperationException("非法路径");
            return fullPath;
        }

  
    }
}
