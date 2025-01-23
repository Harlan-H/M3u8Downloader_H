using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IMediaDownloadParam : IDownloadParamBase
    {
        bool IsVideoStream { get; }
        IList<IStreamInfo> Medias { get; }
        Uri Subtitle { get; }
    }
}
