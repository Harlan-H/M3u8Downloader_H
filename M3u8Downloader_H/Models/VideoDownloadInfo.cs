using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Caliburn.Micro;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Attributes;
using M3u8Downloader_H.Common.Utils;


namespace M3u8Downloader_H.Models
{
    public class VideoDownloadInfo : PropertyChangedBase
    {
        private static readonly string[] extensionArr = ["", "m3u8", "json", "txt", "xml"];
       // [Extension(["", "m3u8", "json", "txt", "xml"], ExceptionMsg = "请确认是否为.m3u8或.txt或.json或.xml或文件夹")]
        public string RequestUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;

        public string Method { get; set; } = default!;
        public string? Key { get; set; } = default!;
        public string? Iv { get; set; } = default!;

        public Action<Uri> HandleTextAction { get; set; } = default!;
        public Action<IM3u8DownloadParam, string?> NormalProcessDownloadAction { get; set; } = default!;

        public VideoDownloadInfo()
        {

        }

        public void Reset(bool resetUrl,bool resetName)
        {
            if(resetUrl) RequestUrl = string.Empty;
            if(resetName) VideoName = string.Empty;
            Key = null;
            Method = "AES-128";
            Iv = null;
        }

        public M3u8DownloadParams Clone()
        {
            return new M3u8DownloadParams(new Uri(RequestUrl), VideoName, Method, Key, Iv);
        }

        public void DoProcess()
        {
            if(string.IsNullOrWhiteSpace(RequestUrl))
                throw new InvalidOperationException("下载地址不能为空");

            Uri uri = new(RequestUrl!, UriKind.Absolute);
            if (!uri.IsFile)
            {
                NormalProcessDownloadAction(Clone(), null);
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
                NormalProcessDownloadAction(Clone(), null);
                return;
            }
        }

    }
}
