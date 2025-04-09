using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Models
{
    internal class DialogProgress(Action<double> action) : IDialogProgress
    {
        private readonly Action<double> action = action;

        public IDisposable Acquire()
        {
            throw new NotImplementedException();
        }

        public void IncProgressNum(bool isInc)
        {
            throw new NotImplementedException();
        }

        public void Report(double value)
        {
            Debug.Print("Report double value :{0}", value);
            action.Invoke(value);
        }

        public void Report(long value)
        {
            throw new NotImplementedException();
        }

        public void SetDownloadStatus(bool IsLiveDownloading)
        {
            throw new NotImplementedException();
        }
    }
}
