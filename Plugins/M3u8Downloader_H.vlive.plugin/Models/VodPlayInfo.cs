namespace M3u8Downloader_H.vlive.plugin.Models
{
    internal readonly struct VodPlayInfo
    {
        private readonly string Vodid;
        private readonly string Inkey;
        private static string Pid {
            get
            {
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return $"rmcPlayer_{ts.Ticks}";
            }
        }

        public VodPlayInfo(string vodid,string inkey)
        {
            Vodid = vodid;
            Inkey = inkey;
        }

        public override string ToString()
        {
            return $"https://apis.naver.com/rmcnmv/rmcnmv/vod/play/v2.0/{Vodid}?key={Inkey}&pid={Pid}&sid=2024&ver=2.0&devt=html5_pc&doct=json&ptc=https&sptc=https&cpt=vtt&ctls=%7B%22visible%22%3A%7B%22fullscreen%22%3Atrue%2C%22logo%22%3Afalse%2C%22playbackRate%22%3Afalse%2C%22scrap%22%3Afalse%2C%22playCount%22%3Atrue%2C%22commentCount%22%3Atrue%2C%22title%22%3Atrue%2C%22writer%22%3Atrue%2C%22expand%22%3Atrue%2C%22subtitles%22%3Atrue%2C%22thumbnails%22%3Atrue%2C%22quality%22%3Atrue%2C%22setting%22%3Atrue%2C%22script%22%3Afalse%2C%22logoDimmed%22%3Atrue%2C%22badge%22%3Atrue%2C%22seekingTime%22%3Atrue%2C%22muted%22%3Atrue%2C%22muteButton%22%3Afalse%2C%22viewerNotice%22%3Afalse%2C%22linkCount%22%3Afalse%2C%22createTime%22%3Afalse%2C%22thumbnail%22%3Atrue%7D%2C%22clicked%22%3A%7B%22expand%22%3Afalse%2C%22subtitles%22%3Afalse%7D%7D&pv=4.26.9&dr=1920x1080&cpl=zh_CN&lc=zh_CN&adi=%5B%7B%22type%22%3A%22pre%22%2C%22exposure%22%3Afalse%2C%22replayExposure%22%3Afalse%7D%5D&adu=%2F&videoId={Vodid}&cc=CN";
        }

        public static implicit operator Uri(VodPlayInfo vodplayinfo) => new(vodplayinfo.ToString());
    }
}
