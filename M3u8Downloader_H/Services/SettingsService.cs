using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Attributes;
using M3u8Downloader_H.Settings.Services;
using M3u8Downloader_H.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;


#if !DEBUG
using System.IO;
#endif

namespace M3u8Downloader_H.Services
{
    [ObservableObject]
    public partial class SettingsService :  SettingsBase,  IDownloaderSetting,IMergeSetting
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        [Range(1,200)]
        [ObservableProperty]
        public partial int MaxThreadCount { get; set; } = 5;

        /// <summary>
        /// 同时下载的任务数量
        /// </summary>
        [ObservableProperty]
        public partial int MaxConcurrentDownloadCount { get; set; } = 3;

        [ObservableProperty]
        public partial int RetryCount { get; set; } = 5;

        [ObservableProperty]
        public partial string SavePath { get; set; } 
#if DEBUG            
            = @"C:\Users\admin\Desktop\666\download";
#else
            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download");
#endif
        [ObservableProperty]
        public partial string PluginKey { get; set; } = default!;

        [ObservableProperty]
        public partial string SelectedFormat { get; set; } = "mp4";

        [JsonIgnore]
        public bool SkipDirectoryExist { get; set; } = true;

        [ObservableProperty]
        public partial bool ForcedMerger { get; set; }

        [ObservableProperty]
        public partial bool IsCleanUp { get; set; } = true;

        [ObservableProperty]
        public partial bool SkipRequestError { get; set; }

        [ObservableProperty]
        public partial bool IsResetAddress { get; set; } = true;

        [ObservableProperty]
        public partial bool IsResetName { get; set; } = true;

        [JsonIgnore]
        public string Separator { get; set; } = "----";

        [ObservableProperty]
        public partial string ProxyAddress { get; set; } = string.Empty;

        [ObservableProperty]
        public partial Dictionary<string,string> Headers { get; set; } = default!;

        [ObservableProperty]
        public partial double RecordDuration { get; set; } 
#if DEBUG            
            = 60 * 10;
#else
            = 60 * 60 * 12;
#endif

        [ObservableProperty]
        public partial int Timeouts { get; set; } = 30;

        public SettingsService() : base(SerializerContext.Default)
        {
#if DEBUG
            Configuration.DirectoryName = @"C:\Users\admin\Desktop\666";
#endif
        }

//         public void UpdateAll()
//         {
//             ThrottlingSemaphore.Instance.MaxCount = MaxConcurrentDownloadCount;
//            
//         }

    }

    public partial class SettingsService
    {
        [JsonSerializable(typeof(SettingsService))]
        private partial class SerializerContext : JsonSerializerContext;
    }

}
