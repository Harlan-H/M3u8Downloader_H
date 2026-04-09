using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Utils
{
    internal class Disposable(Action dispose) : IDisposable
    {
        public void Dispose() => dispose();
    }
}
