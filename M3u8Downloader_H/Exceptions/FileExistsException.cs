using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Exceptions
{
    public class FileExistsException : Exception
    {
        public FileExistsException(string? message) : base(message)
        {
        }

        public override string ToString() => $"【{Message}】\n文件已经存在，请修改名称后再次尝试";
    }
}
