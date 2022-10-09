using M3u8Downloader_H.ViewModels.FrameWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.ViewModels
{
    public class AboutViewModel : DialogScreen
    {
        public void OnExplore(string str)
        {
            ProcessStartInfo processStartInfo = new(str)
            {
                UseShellExecute = true
            };
            Process.Start(processStartInfo);    
        }
    }
}
