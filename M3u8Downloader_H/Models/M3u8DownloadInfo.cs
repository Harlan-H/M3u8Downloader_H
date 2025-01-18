using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Caliburn.Micro;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Attributes;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Services;


namespace M3u8Downloader_H.Models
{
    public class M3u8DownloadInfo : PropertyChangedBase
    {
        private static readonly string[] extensionArr = ["", "m3u8", "json", "txt", "xml"];

        // [Extension(["", "m3u8", "json", "txt", "xml"], ExceptionMsg = "请确认是否为.m3u8或.txt或.json或.xml或文件夹")]
        public string RequestUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;

        public string Method { get; set; } = default!;
        public string? Key { get; set; } = default!;
        public string? Iv { get; set; } = default!;

        public Action<Uri> HandleTextAction { get; set; } = default!;
        public Action<M3u8DownloadParams, string?> NormalProcessDownloadAction { get; set; } = default!;


        public void Reset(bool resetUrl,bool resetName)
        {
            if(resetUrl) RequestUrl = string.Empty;
            if(resetName) VideoName = string.Empty;
            Key = null;
            Method = "AES-128";
            Iv = null;
        }


        public void DoProcess(SettingsService settingsService)
        {
            if(string.IsNullOrWhiteSpace(RequestUrl))
                throw new InvalidOperationException("下载地址不能为空");

            Uri uri = new(RequestUrl!, UriKind.Absolute);
            if (!uri.IsFile)
            {
                M3u8DownloadParams m3U8DownloadParams = new(settingsService, new Uri(RequestUrl), VideoName, Method, Key, Iv);
                NormalProcessDownloadAction(m3U8DownloadParams, null);
                return;
            }


            string ext = Path.GetExtension(RequestUrl).Trim('.');
            string? extension = extensionArr.Where(e => e == ext).FirstOrDefault() ?? throw new InvalidOperationException("请确认是否为.m3u8或.txt或.json或.xml或文件夹");
            if (extension == "")
            {
                FileAttributes fileAttribute = File.GetAttributes(RequestUrl);
                if (fileAttribute != FileAttributes.Directory)
                    throw new InvalidOperationException("请确认是否为文件夹");

                HandleTextAction(uri);
                return;
            }
            else
            {
                M3u8DownloadParams m3U8DownloadParams = new(settingsService, new Uri(RequestUrl), VideoName, Method, Key, Iv);
                NormalProcessDownloadAction(m3U8DownloadParams, null);
                return;
            }
        }

    }
}
