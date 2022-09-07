using System;
using System.IO;

namespace M3u8Downloader_H.M3U8.Adapters
{
    internal class FileAdapter : Adapter
    {
        public FileInfo File { get; }

        public FileAdapter(string fileName)
            : this(new FileInfo(fileName ?? throw new ArgumentNullException(nameof(fileName))))
        {
        }

        public FileAdapter(FileInfo file)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            if (!File.Exists)
            {
                throw new FileNotFoundException("File not found.", File.FullName);
            }
        }

        protected override Stream CreateStream()
        {
            return File.OpenRead();
        }
    }
}