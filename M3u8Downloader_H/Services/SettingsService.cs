using Settings;
using System.IO;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace M3u8Downloader_H.Services
{
    public class SettingsService : SettingsManager
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        public int MaxThreadCount { get; set; } = 1;

        /// <summary>
        /// 同时下载的任务数量
        /// </summary>
        public int MaxConcurrentDownloadCount { get; set; } = 1;

        public int RetryCount { get; set; }
#if DEBUG
        public string SavePath { get; set; } = @"E:\desktop\download";
#else
        public string SavePath { get; set; } = Path.Combine(System.Environment.CurrentDirectory, "download");
#endif
        public PluginItem Pluginitem { get; set; } = default!;
        public string SelectedFormat { get; set; } = "默认";
        public bool SkipDirectoryExist { get; set; } = true;
        public bool ForcedMerger { get; set; }
        public bool IsCleanUp { get; set; } = true;
        public bool SkipRequestError { get; set; }
        public bool IsResetAddress { get; set; } = true;
        public bool IsResetName { get; set; } = true;

        [JsonIgnore]
        public string Separator { get; set; } = "----";

        public string ProxyAddress { get; set; } = string.Empty;
        public string Headers { get; set; } = string.Empty;

#if DEBUG
        public double RecordDuration { get; set; } = 60 * 10;
#else
        public double RecordDuration { get; set; } = 60 * 60 * 12;
#endif
        public int Timeouts { get; set; } = 10;

        public SettingsService()
        {
#if DEBUG
            Configuration.DirectoryName = @"e:\desktop\";
#endif
        }


        public void ServiceUpdate()
        {
            MaxThreadCount = MaxThreadCount.Range(1, 200);
            MaxConcurrentDownloadCount = MaxConcurrentDownloadCount.Range(1, 10);
            RetryCount = RetryCount.Range(0, 10);
            Timeouts = Timeouts.Range(10, 300);

            HttpClient.DefaultProxy = string.IsNullOrWhiteSpace(ProxyAddress)? new WebProxy() : new WebProxy(ProxyAddress);
        }

    }
}
