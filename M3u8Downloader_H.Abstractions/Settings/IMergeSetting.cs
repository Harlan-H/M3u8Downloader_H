namespace M3u8Downloader_H.Abstractions.Settings
{
    public interface IMergeSetting
    {
        string SelectedFormat { get; }
        bool ForcedMerger { get; }
        bool IsCleanUp { get; }
    }
}
