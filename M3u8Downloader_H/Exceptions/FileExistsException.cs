using System;

namespace M3u8Downloader_H.Exceptions
{
    public class FileExistsException(string? message) : Exception(message)
    {
    }
}
