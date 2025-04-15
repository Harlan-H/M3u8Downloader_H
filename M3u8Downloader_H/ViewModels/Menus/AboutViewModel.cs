using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;

namespace M3u8Downloader_H.ViewModels.Menus
{
    public class AboutViewModel : Screen
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
