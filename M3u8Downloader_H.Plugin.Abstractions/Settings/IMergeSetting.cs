namespace M3u8Downloader_H.Abstractions.Settings
{
    public interface IMergeSetting
    {
        string SelectedFormat { get; }
        bool ForcedMerger { get; }
        bool IsCleanUp { get; }

        /// <summary>
        /// 提供选项以确定是否使用Concat列表文件进行合并，以避免一次性打开文件过多报错。
        /// (对于长影片使用M3U文件时容易出现这种情况。)
        /// </summary>
        bool ConcatMerger { get; }
    }
}
