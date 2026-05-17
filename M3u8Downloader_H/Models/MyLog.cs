using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace M3u8Downloader_H.Models
{
    public class MyLog(ICollection<LogParams> Logs) :  Abstractions.Common.ILog
    {
        public virtual void Info(string format, params object[] args)
        {
            Logs.Add(new LogParams(LogType.Info, string.Format(format, args)));
        }

        public virtual void Warn(string format, params object[] args)
        {
            Logs.Add(new LogParams(LogType.Warning, string.Format(format, args)));
        }

        public virtual void Error(Exception exception)
        {
            Logs.Add(new LogParams(LogType.Error, exception.ToString()));
        }
    }
}
