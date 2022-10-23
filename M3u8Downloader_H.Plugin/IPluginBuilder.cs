using M3u8Downloader_H.M3U8.AttributeReaderManager;
using M3u8Downloader_H.M3U8.AttributeReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Plugin
{
    public interface IPluginBuilder
    {
        void SetAttributeReader(IAttributeReaderManager attributeReader);
        IDownloadService? CreatePluginService();
    }
}
