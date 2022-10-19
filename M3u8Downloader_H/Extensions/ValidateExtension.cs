using M3u8Downloader_H.Attributes;
using M3u8Downloader_H.Models;
using M3u8Downloader_H.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Extensions
{
    public static class ValidateExtension
    {
        public static (Uri, string) Validate(this VideoDownloadInfo obj)
        {
            Type type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.IsDefined(typeof(ExtensionAttribute), false)) continue;

                ExtensionAttribute attribute = (ExtensionAttribute)prop.GetCustomAttributes(typeof(ExtensionAttribute), false)[0];
                (Uri uri, string? ext) = attribute.Validate(prop.GetValue(obj));
                if (ext is null)
                   throw new InvalidOperationException(attribute.ExceptionMsg);

                if (ext == "")
                {
                    FileAttributes fileAttribute = File.GetAttributes(uri.OriginalString);
                    if (fileAttribute != FileAttributes.Directory)
                        throw new InvalidOperationException("请确认是否为文件夹");
                }
                return (uri, ext);
            }
            return (default!, default!);
        }

        public static void Validate(this SettingsService obj)
        {
            Type type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.IsDefined(typeof(RangeAttribute), false)) continue;

                RangeAttribute attribute = (RangeAttribute)prop.GetCustomAttributes(typeof(RangeAttribute), false)[0];
                attribute.Validate(obj, prop, prop.GetValue(obj) as int?);
            }
        }

    }
}
