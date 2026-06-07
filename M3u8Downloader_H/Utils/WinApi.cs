using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace M3u8Downloader_H.Utils
{
    internal partial class WinApi
    {
        [LibraryImport("user32.dll", EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool MessageBox(nint hWnd, string text, string caption, uint type);
    }
}
