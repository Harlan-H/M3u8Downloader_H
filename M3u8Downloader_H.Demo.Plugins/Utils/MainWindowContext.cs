using M3u8Downloader_H.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Demo.Plugins.Utils
{
    public class MainWindowContext(IWindowContext windowContext)
    {
        public IWindowContext WindowContext { get; } = windowContext;
    }
}
