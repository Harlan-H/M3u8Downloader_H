using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Common.Utils;

namespace M3u8Downloader_H.Plugin.Services
{
    public class PluginStorage(string key) : IPluginStorage
    {
        private readonly string _rootPath = Path.Combine(StorageSpaceManager.GetPluginDataPath(), key);

        public bool Exists(string path)
            => File.Exists(Path.Combine(_rootPath, EscapePath(path)));

        public string GetPath(string path)
            => Path.Combine(_rootPath, EscapePath(path));

        public Stream OpenRead(string path)
            => File.OpenRead(Path.Combine(_rootPath, EscapePath(path)));

        public Stream OpenWrite(string path)
            => File.OpenWrite(Path.Combine(_rootPath, EscapePath(path)));

        public DirectoryInfo CreateDirectory(string Dir)
            => Directory.CreateDirectory(Path.Combine(_rootPath, EscapePath(Dir)));

        private static string EscapePath(string path)
        {
            var relativePath =
                path.Replace('\\', '/')
                .TrimStart('/');
            
            return Path.GetInvalidFileNameChars().Append('.').Aggregate(relativePath, (current, invalidChar) => current.Replace(invalidChar, '_'));
        }

  
    }
}
