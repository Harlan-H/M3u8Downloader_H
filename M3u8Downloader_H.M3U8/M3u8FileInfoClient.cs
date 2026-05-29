using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Models;
using M3u8Downloader_H.Abstractions.Plugins.Download;
using M3u8Downloader_H.M3U8.AttributeReaders;
using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.M3U8.M3UFileReaders;
using System;

namespace M3u8Downloader_H.M3U8
{
    public partial class M3u8FileInfoClient(IDownloadContext context, IDownloadPlugin? downloadPlugin)
    {
        public M3UFileReaderManager M3UFileReadManager
        {
            get => new(context, M3u8FileReader);
        }

        public IM3uFileReader M3u8FileReader
        {
            get
            {
                IM3u8DownloadParam m3U8DownloadParam = (IM3u8DownloadParam)context.DownloadParam;
                IM3uFileReader  m3UFileReader = new M3UFileReaderWithStream();
                if (downloadPlugin is not null)
                {
                    var reader = downloadPlugin.CreateM3uFileReader(m3UFileReader, context);
                    if (reader is not null)
                    {
                        m3UFileReader = reader;
                    }
                }
                else if (m3U8DownloadParam.M3UFileInfoSources is not null 
                    && m3U8DownloadParam.M3UFileInfoSources.Count == 1 
                    && m3U8DownloadParam.M3UFileInfoSources[0].RequestUrl.IsFile 
                    && m3U8DownloadParam.M3UFileInfoSources[0].RequestUrl.OriginalString.EndsWith(".json"))
                {
                    m3UFileReader = new M3UFileReaderWithJson();
                }
                m3UFileReader.InitAttributeReade(AttributeReaderRoot.Instance.AttributeReaders);
                return m3UFileReader;
            }
        }

    }

    public partial class M3u8FileInfoClient
    {
        public static IM3uFileReader CreateM3uFileReader()
        {
            M3UFileReaderWithStream m3UFileReader = new();
            m3UFileReader.InitAttributeReade(AttributeReaderRoot.Instance.AttributeReaders);
            return m3UFileReader;
        }
    }
}
