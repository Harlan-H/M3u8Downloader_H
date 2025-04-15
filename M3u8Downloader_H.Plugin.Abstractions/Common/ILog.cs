using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface ILog
    {
        void Info(string format, params object[] args);

        void Warn(string format, params object[] args);

        void Error(Exception exception);
    }
}
