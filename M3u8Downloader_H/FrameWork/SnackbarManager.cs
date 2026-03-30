using Avalonia.Threading;
using Material.Styles.Controls;
using Material.Styles.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.FrameWork;

public class SnackbarManager(string hostname,TimeSpan duration)
{
    public void Notify(string message)
    {
        SnackbarHost.Post(
            new SnackbarModel(message, duration),
            hostname,
            DispatcherPriority.Normal
            );
    }
}
