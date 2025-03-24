using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Abstractions.Converter
{
    public interface IConverter
    {
        ValueTask StartMerge(IDialogProgress progress, CancellationToken cancellationToken);
    }
}
