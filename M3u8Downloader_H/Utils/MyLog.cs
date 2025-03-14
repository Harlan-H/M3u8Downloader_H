using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using M3u8Downloader_H.Models;

namespace M3u8Downloader_H.ViewModels.Utils
{
    public class MyLog : PropertyChangedBase, Abstractions.Common.ILog
    {
        public BindableCollection<LogParams> Logs { get; } = [];

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
            Logs.Add(new LogParams(LogType.Error, exception.Message));
        }

        public virtual string CopyLog()
        {
            StringBuilder sb = new();
            foreach (var log in Logs.ToArray())
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
