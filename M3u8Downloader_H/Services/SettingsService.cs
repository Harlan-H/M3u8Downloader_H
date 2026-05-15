using CommunityToolkit.Mvvm.ComponentModel;
using M3u8Downloader_H.Abstractions.M3uDownloaders;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Common.Services;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Plugin.Services;
using M3u8Downloader_H.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
        public partial string SavePath { get; set; } = StorageSpaceManager.GetSavePath();

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
        [JsonPropertyName("Proxy")]
        public partial ProxyService ProxyInfo { get; set; } = new();

        [ObservableProperty]
        public partial Dictionary<string,string> Headers { get; set; } = default!;

        [ObservableProperty]
        [JsonConverter(typeof(TimeSpanJsonConverter))]
        public partial TimeSpan RecordDuration { get; set; } 
#if DEBUG            
            =  new TimeSpan(0,10, 0);
#else
            =  new TimeSpan(12,0, 0);
#endif

        [ObservableProperty]
        public partial int Timeouts { get; set; } = 30;

        public SettingsService() : base(SerializerContext.Default,Path.Combine(StorageSpaceManager.GetConfigPath(), "Settings.dat"))
        {

        }

    }

    public partial class SettingsService
    {
        [JsonSerializable(typeof(SettingsService))]
        private partial class SerializerContext : JsonSerializerContext;

        private class TimeSpanJsonConverter : JsonConverter<TimeSpan>
        {
            public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Number)
                {
                    if(reader.TryGetDouble(out double sec))
                        return TimeSpan.FromSeconds(sec);
                }
                return TimeSpan.Zero;
            }

            public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value.TotalSeconds);
            }
        }
    }


    public partial class ProxyService : ObservableObject
    {

        [ObservableProperty]
        public partial string Address { get; set; } = default!;

        [ObservableProperty]
        public partial string UserName { get; set; } = default!;

        [ObservableProperty]
        public partial string PassWord { get; set; } = default!;


        public ProxyService Clone()
        {
            return new ProxyService()
            {
                Address = Address,
                UserName = UserName,
                PassWord = PassWord,
            };
        }

    }

    public partial class ProxyService : IEquatable<ProxyService>
    {
        public bool Equals(ProxyService? other)
            => StringComparer.Ordinal.Equals(Address, other?.Address)
            && StringComparer.Ordinal.Equals(UserName, other?.UserName)
            && StringComparer.Ordinal.Equals(PassWord, other?.PassWord);

        public override bool Equals(object? obj) => obj is ProxyService proxyService && Equals(proxyService);
        public override int GetHashCode()  => base.GetHashCode();

        public static bool operator ==(ProxyService proxyInfo, ProxyService proxyInfo1) => proxyInfo.Equals(proxyInfo1);
        public static bool operator !=(ProxyService proxyInfo, ProxyService proxyInfo1) => !(proxyInfo == proxyInfo1);
    }
}
