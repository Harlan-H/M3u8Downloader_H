using System;
using System.Collections.Generic;
using System.Linq;

namespace M3u8Downloader_H.Common.M3u8Infos
{
    public partial class M3UFileInfo 
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

        //当原始的m3u8中的数据 不满足需求的时候 可以通过自定义的数据 进行操作
        public object? UserData { get; set; }

        public bool IsFile => MediaFiles.Any(m => m.Uri.IsFile);

        public M3UFileInfo(M3UFileInfo m3UFileInfo)
        {
            Version = m3UFileInfo.Version;
            MediaSequence = m3UFileInfo.MediaSequence;
            TargetDuration = m3UFileInfo.TargetDuration;
            AllowCache = m3UFileInfo.AllowCache;
            PlaylistType = m3UFileInfo.PlaylistType;
            Key = m3UFileInfo.Key;
            Streams = [.. m3UFileInfo.Streams];
            MediaFiles = [.. m3UFileInfo.MediaFiles];
        }



        public M3UFileInfo()
        {

        }
    }

    public partial class M3UFileInfo
    {
        public static M3UFileInfo CreateVodM3UFileInfo()
        {
            M3UFileInfo m3UFileInfo = new()
            {
                PlaylistType = "VOD"
            };
            return m3UFileInfo;
        }
    }
}