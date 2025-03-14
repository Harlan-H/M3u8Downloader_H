using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.M3U8.M3UFileReaderManangers;
using M3u8Downloader_H.M3U8.M3UFileReaders;

namespace M3u8Downloader_H.M3U8
{
    public class M3u8FileInfoLocal(ILog log)
    {
        public M3UFileReaderManager M3UFileReadManager
        {
            get
            {
                M3UFileReaderManager m3UFileReaderManager;
                m3UFileReaderManager = new M3UFileReaderManager(null!)
                {
                    M3u8FileReader = new M3UFileReaderWithStream(),
                    Log = log
                };
                return m3UFileReaderManager;
            }
        }
    }
}
