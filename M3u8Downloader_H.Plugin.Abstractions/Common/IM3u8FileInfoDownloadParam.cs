using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.M3u8Infos;

namespace M3u8Downloader_H.Abstractions.Common
{
    public interface IM3u8FileInfoDownloadParam : IDownloadParamBase
    {
        M3UFileInfo M3UFileInfos { get; }
    }
}
