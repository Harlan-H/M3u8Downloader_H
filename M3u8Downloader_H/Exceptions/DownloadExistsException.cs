using System;
using System.Collections.Generic;
using System.Text;
using Tmds.DBus.Protocol;

namespace M3u8Downloader_H.Exceptions
{
    internal class DownloadExistsException(string? message) : Exception(message)
    {
    }
}
