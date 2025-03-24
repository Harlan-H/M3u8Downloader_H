using System;
using System.Collections.Generic;
using System.Linq;
using M3u8Downloader_H.Abstractions.M3u8;

namespace M3u8Downloader_H.Common.M3u8Infos
{
    public partial class M3UFileInfo : IM3uFileInfo
    {
        public int? Version { get; set; }

        public int? MediaSequence { get; set; }

        public int? TargetDuration { get; set; }

        public bool? AllowCache { get; set; }

        public string PlaylistType { get; set; } = default!;

        public IM3uMediaInfo? Map { get; set; }

        public DateTime? ProgramDateTime { get; set; }

        public IM3uKeyInfo Key { get; set; } = default!;

        public IList<IM3uStreamInfo> Streams { get; set; } = default!;

        public IList<IM3uMediaInfo> MediaFiles { get; set; } = default!;

        //当原始的m3u8中的数据 不满足需求的时候 可以通过自定义的数据 进行操作
        public object? UserData { get; set; }

        public bool IsFile => MediaFiles.Any(m => m.Uri.IsFile);

        public bool IsCrypted => Key is not null;

        public M3UFileInfo(M3UFileInfo m3UFileInfo)
        {
            Version = m3UFileInfo.Version;
            MediaSequence = m3UFileInfo.MediaSequence;
            TargetDuration = m3UFileInfo.TargetDuration;
            AllowCache = m3UFileInfo.AllowCache;
            PlaylistType = m3UFileInfo.PlaylistType;
            Map = m3UFileInfo.Map;
            ProgramDateTime = m3UFileInfo.ProgramDateTime;
            Key = m3UFileInfo.Key;
            Streams = [.. m3UFileInfo.Streams];
            MediaFiles = [.. m3UFileInfo.MediaFiles];
            UserData = m3UFileInfo.UserData;
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