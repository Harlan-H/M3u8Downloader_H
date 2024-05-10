namespace M3u8Downloader_H.Settings.Services
{
    public class Configuration
    {
        public StorageSpace StorageSpace { get; set; } = StorageSpace.Instance;
        public string DirectoryName { get; set; } = string.Empty;
        public string FileName { get; set; } = "Settings.dat";

        public bool ThrowIfCannotSave { get; set; } = true;

        public bool ThrowIfCannotLoad { get; set; } = true;
    }
}
