using CommunityToolkit.Mvvm.Messaging.Messages;
using M3u8Downloader_H.Plugin.Models.Context;

namespace M3u8Downloader_H.Messages
{
    public class GetAppComandServiceMessage(WindowContext windowContext) : ValueChangedMessage<WindowContext>(windowContext)
    {
        
    }
}
