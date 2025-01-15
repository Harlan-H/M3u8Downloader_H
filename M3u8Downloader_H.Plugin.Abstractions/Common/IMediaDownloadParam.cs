using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IMediaDownloadParam : IDownloadParamBase
    {
        IList<IStreamInfo> StreamInfos { get; }
    }
}
