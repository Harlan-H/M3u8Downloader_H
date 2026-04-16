using CommunityToolkit.Mvvm.Messaging.Messages;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Messages
{
    public class GetAppComandServiceMessage(WindowContext windowContext) : ValueChangedMessage<WindowContext>(windowContext)
    {
        
    }
}
