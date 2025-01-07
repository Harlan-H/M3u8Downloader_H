using Caliburn.Micro;
using M3u8Downloader_H.Attributes;


namespace M3u8Downloader_H.Models
{
    public class VideoDownloadInfo : PropertyChangedBase
    {
        [Extension(["", "m3u8", "json", "txt", "xml"], ExceptionMsg = "请确认是否为.m3u8或.txt或.json或.xml或文件夹")]
        public string RequestUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;

        public string? Method { get; set; } = default!;
        public string? Key { get; set; } = default!;
        public string? Iv { get; set; } = default!;

        public VideoDownloadInfo()
        {

        }

        public void Reset(bool resetUrl,bool resetName)
        {
            if(resetUrl) RequestUrl = string.Empty;
            if(resetName) VideoName = string.Empty;
            Key = null;
            Method = null;
            Iv = null;
        }
    }
}
