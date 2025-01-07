using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Abstractions.Meger
{
    public interface IMergeSetting
    {
        string SelectedFormat { get; }
        bool ForcedMerger { get; }
        bool IsCleanUp { get; }
    }
}
