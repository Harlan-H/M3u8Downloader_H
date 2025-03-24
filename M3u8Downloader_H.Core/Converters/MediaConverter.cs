using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Converter;

namespace M3u8Downloader_H.Core.Converters
{
    public class MediaConverter : IConverter
    {
        public ValueTask StartMerge(IDialogProgress progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
