namespace M3u8Downloader_H.Abstractions.M3u8
{
    public interface IM3uFileInfo
    {
        public int? Version { get;  }

        public int? MediaSequence { get;  }

        public int? TargetDuration { get;  }

        public bool? AllowCache { get;  }

        public string PlaylistType { get; set; }

        public IM3uMediaInfo? Map { get;  }

        public DateTime? ProgramDateTime { get;  }

        public IM3uKeyInfo Key { get;  }

        public IList<IM3uStreamInfo> Streams { get;  }

        public IList<IM3uMediaInfo> MediaFiles { get;  }

        //当原始的m3u8中的数据 不满足需求的时候 可以通过自定义的数据 进行操作
        public object? UserData { get;  }

        public bool IsFile { get; }

        public bool IsCrypted { get; }
    }
}
