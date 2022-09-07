using System.IO;
using M3u8Downloader_H.Exceptions;

namespace M3u8Downloader_H.Utils
{
    public static class FileEx
    {
        private static readonly string[] formats = { "ts", "m4s", "mp4" };
        public static void EnsureFileNotExist(string filename)
        {
            foreach (string item in formats)
            {
                string filePath = filename + '.' + item;
                FileInfo fileInfo = new(filePath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    throw new FileExistsException(filename);
                }
            }
        }

        public static void EnsureFileNotExist(string savepath, string filename)
        {
            try
            {
                DirectoryInfo dir = new(savepath);
                FileInfo[] files = dir.GetFiles();
                foreach (var file in files)
                {
                    string noExtension = Path.GetFileNameWithoutExtension(file.Name);
                    if (noExtension == filename)
                    {
                        if (file.Length > 0)
                        {
                            throw new FileExistsException(filename);
                        }
                        break;
                    }
                }
            }catch(DirectoryNotFoundException)
            {

            }
        }

    }
}
