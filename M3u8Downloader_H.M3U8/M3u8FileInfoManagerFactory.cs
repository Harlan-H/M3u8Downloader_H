using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.Plugin;
using System.Collections.Generic;
using System.Net.Http;

namespace M3u8Downloader_H.M3U8
{
    public class M3u8FileInfoManagerFactory
    {
        public static IM3UFileInfoMananger CreateM3u8FileInfoManager(IM3u8FileInfoStreamService? m3u8FileInfoStreamServic, IM3uFileReader? m3UFileReader, HttpClient httpClient, IDictionary<string, IAttributeReader>? attributeReaders = null)
        {
            M3UFileReaderManager m3UFileReaderManager;
            if (m3u8FileInfoStreamServic is not null)
                m3UFileReaderManager = new PluginM3UFileReaderManager(m3u8FileInfoStreamServic,  httpClient, attributeReaders);
            else
                m3UFileReaderManager = new M3UFileReaderManager(httpClient, attributeReaders);
            m3UFileReaderManager.M3UFileReader = m3UFileReader;
            return m3UFileReaderManager;
        }
    }
}
