using System;
using System.Collections.Generic;
using System.Text;

namespace M3u8Downloader_H.Abstractions.M3u8
{
    public interface IM3uMediaManifest
    {
        public string Name { get; }
        public string Type { get;  }
        public string GroupId { get;  }
        public string AutoSelect { get;  }
        public string Language { get;  }
        public string Default { get;  }
        public string Channels { get;  }
        public Uri Uri { get;  }
    }
}
