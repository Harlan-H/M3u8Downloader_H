using System;
using System.Collections.Generic;
using System.Linq;

namespace M3u8Downloader_H.M3U8.Infos
{
    public class M3UFileInfo 
    {
        public int? Version { get; set; }

        public int? MediaSequence { get; set; }

        public int? TargetDuration { get; set; }

        public bool? AllowCache { get; set; }

        public string PlaylistType { get; set; } = default!;

        public M3UMediaInfo? Map { get; set; }

        public DateTime? ProgramDateTime { get; set; }

        public M3UKeyInfo Key { get; set; } = default!;

        public IList<M3UStreamInfo> Streams { get; set; } = default!;

        public IList<M3UMediaInfo> MediaFiles { get; set; } = default!;

        public M3UFileInfo(M3UFileInfo m3UFileInfo)
        {
            Version = m3UFileInfo.Version;
            MediaSequence = m3UFileInfo.MediaSequence;
            TargetDuration = m3UFileInfo.TargetDuration;
            AllowCache = m3UFileInfo.AllowCache;
            PlaylistType = m3UFileInfo.PlaylistType;
            Key = m3UFileInfo.Key;
            Streams = m3UFileInfo.Streams.ToList();
            MediaFiles = m3UFileInfo.MediaFiles.ToList();
        }



        public M3UFileInfo() { }
    
    
    }
}