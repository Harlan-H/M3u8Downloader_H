﻿using Newtonsoft.Json;
using M3u8Downloader_H.Attributes;
using M3u8Downloader_H.Settings.Services;
using M3u8Downloader_H.Settings.Models;
using System.Collections.Generic;
using System;



#if !DEBUG
using System.IO;
#endif

namespace M3u8Downloader_H.Services
{
    public class SettingsService : SettingsManager, ICloneable, ISettings
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        [Range(1,200)]
        public int MaxThreadCount { get; set; } = 5;

        /// <summary>
        /// 同时下载的任务数量
        /// </summary>
        [Range(1,30)]
        public int MaxConcurrentDownloadCount { get; set; } = 3;

        [Range(1, 20)]
        public int RetryCount { get; set; } = 5;
#if DEBUG
        public string SavePath { get; set; } = @"E:\desktop\download";
#else
        public string SavePath { get; set; } = Path.Combine(System.Environment.CurrentDirectory, "download");
#endif
        public string PluginKey { get; set; } = default!;
        public string SelectedFormat { get; set; } = "默认";

        [JsonIgnore]
        public bool SkipDirectoryExist { get; set; } = true;
        public bool ForcedMerger { get; set; }
        public bool IsCleanUp { get; set; } = true;
        public bool IsPlaySound { get; set; } = true;

        public bool SkipRequestError { get; set; }
        public bool IsResetAddress { get; set; } = true;
        public bool IsResetName { get; set; } = true;

        [JsonIgnore]
        public string Separator { get; set; } = "----";

        [Update]
        public string ProxyAddress { get; set; } = string.Empty;

        public Dictionary<string,string> Headers { get; set; } = default!;

#if DEBUG
        public double RecordDuration { get; set; } = 60 * 10;
#else
        public double RecordDuration { get; set; } = 60 * 60 * 12;
#endif
        [Range(1,300)]
        public int Timeouts { get; set; } = 10;

        public SettingsService()
        {
#if DEBUG
            Configuration.DirectoryName = @"e:\desktop\";
#endif
        }


        public object Clone()
        {
            return new SettingsService()
            {
                MaxThreadCount = MaxThreadCount,
                MaxConcurrentDownloadCount = MaxConcurrentDownloadCount,
                RetryCount = RetryCount,
                SavePath = SavePath,
                PluginKey = PluginKey,
                SelectedFormat = SelectedFormat,
                ForcedMerger = ForcedMerger,
                IsCleanUp = IsCleanUp,
                IsPlaySound = IsPlaySound,
                SkipRequestError = SkipRequestError,
                IsResetAddress = IsResetAddress,
                ProxyAddress = ProxyAddress,
                Headers = Headers,
                RecordDuration = RecordDuration,
                Timeouts = Timeouts,
            };
        }

        public void CopyFrom(SettingsService other)
        {
            MaxThreadCount = other.MaxThreadCount;
            MaxConcurrentDownloadCount = other.MaxConcurrentDownloadCount;
            RetryCount = other.RetryCount;
            SavePath = other.SavePath;
            PluginKey = other.PluginKey;
            SelectedFormat = other.SelectedFormat;
            ForcedMerger = other.ForcedMerger;
            IsCleanUp = other.IsCleanUp;
            IsPlaySound = other.IsPlaySound;
            SkipRequestError = other.SkipRequestError;
            IsResetAddress = other.IsResetAddress;
            ProxyAddress = other.ProxyAddress;
            Headers = other.Headers;
            RecordDuration = other.RecordDuration;
            Timeouts = other.Timeouts;
        }

    }
}
