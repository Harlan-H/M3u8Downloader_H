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

    public class LogParams
    {
        public LogType Type {get;}
        public DateTime Time { get;  }
        public string Message { get; } = default!;
        public LogParams(LogType logType,string message)
        {
            Type = logType;
            Time = DateTime.Now;
            Message = message;
        }
    }
}
