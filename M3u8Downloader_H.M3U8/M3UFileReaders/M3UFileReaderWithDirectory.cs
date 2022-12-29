using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8.Utilities;

namespace M3u8Downloader_H.M3U8.M3UFileReaders
{
    internal sealed class M3UFileReaderWithDirectory : M3UFileReaderBase
    {
        public override M3UFileInfo Read(Stream stream)
        {
            DirectoryInfo mydir = new(RequestUri.OriginalString);
            var fileinfos = mydir.EnumerateFiles();
            if (!fileinfos.Any())
                throw new InvalidOperationException("此文件夹中没有任何文件");


            M3UFileInfo m3UFileInfo = new()
            {
                Key = GetM3UKeyInfo(fileinfos)!,
                Map = GetMapMediaInfo(fileinfos),
                MediaFiles = GetM3UMediaInfos(fileinfos),
                PlaylistType = "VOD"
            };
            return m3UFileInfo;
        }

        private M3UKeyInfo? GetM3UKeyInfo(IEnumerable<FileInfo> fileInfos)
        {
            var keyInfos = fileInfos.FirstOrDefault(f => f.Name.EndsWith(".key", StringComparison.CurrentCultureIgnoreCase));
            if (keyInfos != null)
            {
                return GetM3UKeyInfo(null, keyInfos.FullName, null, null);
            }
            return null;
        }

        private M3UMediaInfo? GetMapMediaInfo(IEnumerable<FileInfo> fileInfos)
        {
            var mapMediaInfo = fileInfos.FirstOrDefault(f => f.Name.StartsWith("header.", StringComparison.CurrentCultureIgnoreCase));
            if (mapMediaInfo is not null)
                return GetM3UMediaInfo(mapMediaInfo.FullName, mapMediaInfo.Name);
            return null;
        }

        public List<M3UMediaInfo> GetM3UMediaInfos(IEnumerable<FileInfo> fileInfos)
        {
            var tmpFileInfos = fileInfos
                .Where(f => f.Name.EndsWith(".ts", StringComparison.CurrentCultureIgnoreCase) || f.Name.EndsWith(".tmp", StringComparison.CurrentCultureIgnoreCase))
                .OrderBy(f => f.Name, new SpecialComparer());

            List<M3UMediaInfo> m3UMediaInfos = new();
            foreach (var fileinfo in tmpFileInfos)
            {
                var m3umediainfo = GetM3UMediaInfo(fileinfo.FullName, fileinfo.Name);
                m3UMediaInfos.Add(m3umediainfo);
            }
            return m3UMediaInfos;
        }
    }
}
