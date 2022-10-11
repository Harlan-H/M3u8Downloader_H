using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExtensionAttribute : Attribute
    {
        public string ExceptionMsg { get; set; } = default!;

        private readonly string[] extArray = new[] { "", "m3u8", "json", "txt", "xml" };
        public ExtensionAttribute()
        {
        }

        public (Uri,string?) Validate(object? value)
        {
            var requestUrl = value as string;
            Uri uri = new(requestUrl!, UriKind.Absolute);
            if (!uri.IsFile)
                return (uri,"m3u8");

            string ext = Path.GetExtension(uri.OriginalString).Trim('.');
            string? extension = extArray.Where(e => e == ext).FirstOrDefault();
            return (uri, extension);
        }
    }
}
