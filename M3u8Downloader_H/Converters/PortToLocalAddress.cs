using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Converters
{
    public class PortToLocalAddress : BaseConverters<int, string>
    {
        public static PortToLocalAddress Instance { get; } = new PortToLocalAddress();
        public override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"http://{GetLocalIp()}:{value}";
        }

        public override int ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "0.0.0.0";
        }
    }
}
