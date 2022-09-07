using System;
using M3u8Downloader_H.M3U8.Infos;

namespace M3u8Downloader_H.Core.M3u8Analyzers
{
    public class AnalyzerFactory
    {
        public static AnalyzerBase CreateJsonAnalyzer(Uri uri) => new JsonAnalyzer(uri);

        public static AnalyzerBase CreateXmlAnalyzer(Uri uri) => new XmlAnalyzer(uri);

        public static AnalyzerBase CreateDirectoryAnalyzer(Uri uri) => new DirectoryAnalyzer(uri);

    }
}
