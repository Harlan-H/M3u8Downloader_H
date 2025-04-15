using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Models
{
    public enum LogType
    {
        Info,
        Warning, 
        Error
    }

    public class LogParams(LogType logType, string message)
    {
        public LogType Type { get; } = logType;
        public DateTime Time { get; } = DateTime.Now;
        public string Message { get; } = message;
    }
}
