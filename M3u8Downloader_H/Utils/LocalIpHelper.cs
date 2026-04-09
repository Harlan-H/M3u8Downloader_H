using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace M3u8Downloader_H.Utils
{
    public static class LocalIpHelper
    {
        /// <summary>
        /// 获取所有本机IPv4地址（已过滤无效网卡）
        /// </summary>
        public static List<IPAddress> GetAllLocalIPv4(bool includeVirtual = false)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(n =>
                    n.OperationalStatus == OperationalStatus.Up &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    (includeVirtual || !IsVirtual(n)))
                .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                .Where(ip =>
                    ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ip.Address))
                .Select(ip => ip.Address)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// 获取“最优”的局域网IP（优先 192.168.x.x / 10.x.x.x）
        /// </summary>
        public static IPAddress? GetPreferredLocalIPv4()
        {
            var ips = GetAllLocalIPv4();

            // 优先级排序
            return ips
                .OrderBy(ip => GetPriority(ip))
                .FirstOrDefault();
        }

        /// <summary>
        /// 判断是否虚拟网卡（Docker / VM / VPN）
        /// </summary>
        private static bool IsVirtual(NetworkInterface ni)
        {
            var name = ni.Name.ToLower();
            var desc = ni.Description.ToLower();

            return name.Contains("docker") ||
                   name.Contains("vmware") ||
                   name.Contains("virtual") ||
                   name.Contains("veth") ||
                   name.Contains("br-") ||
                   desc.Contains("virtual") ||
                   desc.Contains("vpn");
        }

        /// <summary>
        /// IP优先级（数值越小优先级越高）
        /// </summary>
        private static int GetPriority(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();

            // 192.168.x.x
            if (bytes[0] == 192 && bytes[1] == 168)
                return 1;

            // 10.x.x.x
            if (bytes[0] == 10)
                return 2;

            // 172.16 - 172.31
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return 3;

            return 100;
        }
    }
}