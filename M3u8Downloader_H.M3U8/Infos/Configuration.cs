using System;
using System.Text;

namespace M3u8Downloader_H.M3U8.Infos
{
    internal sealed class Configuration
    {

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public static Configuration Default { get; } = new Configuration();

        private Configuration()
        {
        }
    }
}