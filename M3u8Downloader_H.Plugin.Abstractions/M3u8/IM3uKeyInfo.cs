using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.M3u8
{
    public partial interface IM3uKeyInfo 
    {
        public string Method { get; }

        public Uri Uri { get; }

        public byte[] BKey { get; }

        public byte[] IV { get; }
    }

}
