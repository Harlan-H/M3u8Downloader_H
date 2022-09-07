namespace M3u8Downloader_H.ViewModels.FrameWork
{
    public interface IVIewModelFactory
    {
        DownloadViewModel CreateDownloadViewModel();
        SettingsViewModel CreateSettingsViewModel();
        MessageBoxViewModel CreateMessageBoxViewModel();

    }
}
