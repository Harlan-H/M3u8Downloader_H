using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Attributes
{
    internal class UpdateAttribute : BaseAttribute
    {
        public override void Validate(object obj, object property, object? value)
        {
            var proxyAddress = value as string;
            HttpClient.DefaultProxy = string.IsNullOrWhiteSpace(proxyAddress) ? new WebProxy() : new WebProxy(proxyAddress);
        }
    }
}
