using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using M3u8Downloader_H.Models;

namespace M3u8Downloader_H.Services
{
    public class PluginService
    {
#if DEBUG
        private readonly string ConfigPath = "e:/desktop/config.xml";
#else
        private readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugin", "config.xml");
#endif
        private PluginInfos PluginInfo = default!;
        public PluginService()
        {

        }

        public void Load()
        {
            try
            {
                XmlSerializer xs = new(typeof(PluginInfos));
                XmlReader xmlReader = XmlReader.Create(File.OpenRead(ConfigPath));
                PluginInfo = (PluginInfos)(xs.Deserialize(xmlReader) ?? new PluginInfos());
            }catch(FileNotFoundException)
            {
                PluginInfo = new PluginInfos();
            }
        }

        public List<PluginItem> GetPluginItem() => PluginInfo.PluginItems;

    }
}
