using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.Models
{
    public interface ISnackbarMaranger
    {
        void Notify(string message);
    }
}
