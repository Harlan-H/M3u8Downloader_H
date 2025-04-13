using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Downloader.Utils
{
    internal readonly struct FileSize(long bytes)
    {
        public long Bytes { get; } = bytes;

        public double KiloBytes => (double)Bytes / (1 << 10);

        public double MegaBytes => (double)Bytes / (1 << 20);

        public double GigaBytes => (double)Bytes / (1 << 30);

        private string GetLargestWholeNumberSymbol()
        {
            if (Math.Abs(GigaBytes) >= 1)
                return "GB";
            else if (Math.Abs(MegaBytes) >= 1)
                return "MB";
            else if (Math.Abs(KiloBytes) >= 1)
                return "KB";
            else
                return "B";
        }

        private double GetLargestWholeNumberValue()
        {
            if (Math.Abs(GigaBytes) >= 1)
                return GigaBytes;
            else if (Math.Abs(MegaBytes) >= 1)
                return MegaBytes;
            else if(Math.Abs(KiloBytes) >= 1)
                return KiloBytes;
            else
                return Bytes;
        }

        public override string ToString() => $"{GetLargestWholeNumberValue():0.##} {GetLargestWholeNumberSymbol()}";
    }
}
