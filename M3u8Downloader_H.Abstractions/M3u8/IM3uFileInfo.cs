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
    }
}
