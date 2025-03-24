using M3u8Downloader_H.Abstractions.Common;

namespace M3u8Downloader_H.Abstractions.Converter
{
    public interface IConverter
    {
        ValueTask StartMerge(IDialogProgress progress, CancellationToken cancellationToken);
    }
}
