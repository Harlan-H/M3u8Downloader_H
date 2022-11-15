using M3u8Downloader_H.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Extensions
{
    internal static class ServiceExtension
    {
        public static void Init(this SettingsService settingsService)
        {
            settingsService.Load();
            settingsService.Validate();
            //settingsService.ServiceUpdate();
        }
    }
}
