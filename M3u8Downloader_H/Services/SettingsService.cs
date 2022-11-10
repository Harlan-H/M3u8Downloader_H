using Settings;
using Newtonsoft.Json;
using M3u8Downloader_H.Attributes;

namespace M3u8Downloader_H.Services
{
    public class SettingsService : SettingsManager
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        [Range(1,200)]
        public int MaxThreadCount { get; set; } = 1;

        /// <summary>
        /// 同时下载的任务数量
        /// </summary>
        [Range(1,10)]
        public int MaxConcurrentDownloadCount { get; set; } = 1;

        [Range(0,10)]
        public int RetryCount { get; set; } 
#if DEBUG
        public string SavePath { get; set; } = @"E:\desktop\download";
#else
        public string SavePath { get; set; } = Path.Combine(System.Environment.CurrentDirectory, "download");
#endif
        public string PluginKey { get; set; } = default!;
        public string SelectedFormat { get; set; } = "默认";
        public bool SkipDirectoryExist { get; set; } = true;
        public bool ForcedMerger { get; set; }
        public bool IsCleanUp { get; set; } = true;
        public bool SkipRequestError { get; set; }
        public bool IsResetAddress { get; set; } = true;
        public bool IsResetName { get; set; } = true;

        [JsonIgnore]
        public string Separator { get; set; } = "----";

        [Update]
        public string ProxyAddress { get; set; } = string.Empty;

        [JsonIgnore]
        public string Headers { get; set; } = string.Empty;

#if DEBUG
        public double RecordDuration { get; set; } = 60 * 10;
#else
        public double RecordDuration { get; set; } = 60 * 60 * 12;
#endif
        [Range(10,300)]
        public int Timeouts { get; set; } = 10;

        public SettingsService()
        {
#if DEBUG
            Configuration.DirectoryName = @"e:\desktop\";
#endif
        }

    }
}
