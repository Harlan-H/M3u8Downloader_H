using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace M3u8Downloader_H.Models
{
    public class MyLog : ObservableObject, Abstractions.Common.ILog
    {
        public ObservableCollection<LogParams> Logs { get; } = [];

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

        public virtual string CopyLog()
        {
            StringBuilder sb = new();
            foreach (var log in Logs)
            {
                sb.Append(log.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append(' ');
                sb.Append(log.Message);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
