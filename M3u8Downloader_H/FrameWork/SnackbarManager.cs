using Avalonia.Threading;
using M3u8Downloader_H.Abstractions.Models;
using Material.Styles.Controls;
using Material.Styles.Models;
using System;

namespace M3u8Downloader_H.FrameWork;

public class SnackbarManager(string hostname,TimeSpan duration) : ISnackbarMaranger
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
