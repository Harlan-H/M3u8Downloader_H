using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.ViewModels.Utils;
using PropertyChanged;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class DirConverterViewModel : Screen
    {

        private readonly M3u8FileInfoLocal m3U8FileInfoLocal;
        private M3UFileInfo _fileInfo = default!;

        [OnChangedMethod(nameof(OnM3u8DirUrlChanged))]
        public string M3u8DirUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;
        public string Method { get; set; } = default!;
        public string Key { get; set; } = default!;
        public string Iv { get; set; } = default!;
        public string Formats { get; set; } = default!;
        public string SavePath { get; set; } = default!;
        public double Progress { get; set; } = default!;
        public MyLog Log { get; } = new();

        public DirConverterViewModel()
        {
            m3U8FileInfoLocal = new M3u8FileInfoLocal(Log);
        }

        private void OnM3u8DirUrlChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            if (oldValue == newValue)
            {
                Log.Warn("本次传入的文件和上次传入的一致");
                return;
            }

            DirectoryInfo directoryInfo = new(newValue);
            if (!directoryInfo.Exists)
            {
                Log.Warn("{0}文件不存在", directoryInfo.Name);
                return;
            }

            Uri m3u8Uri;
            try
            {
                m3u8Uri = new(newValue);
                var ext = Path.GetExtension(newValue).Trim('.');
                _fileInfo = m3U8FileInfoLocal.M3UFileReadManager.GetM3u8FileInfo(ext, m3u8Uri);
                Log.Info("读取到{0}个文件数据", _fileInfo.MediaFiles.Count);

                VideoName = PathEx.GenerateFileNameWithoutExtension(m3u8Uri, VideoName);
                Log.Info("生成视频名称:{0}", VideoName);
                if (_fileInfo.Key is not null)
                {
                    Method = _fileInfo.Key.Method;
                    Log.Info("文件加密方式:{0}", Method);
                    Iv = _fileInfo.Key.IV.Length == 0 ? "" : Convert.ToHexString(_fileInfo.Key.IV);
                    Log.Info("文件加密iv:{0}", string.IsNullOrWhiteSpace(Iv) ? "0" : Iv);
                    Log.Info("密钥无法读取,请自行填写,如果不清楚直接使用base64方式");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return;
            }
        }

        public void OnProcess()
        {
            if (!_fileInfo.MediaFiles.Any(m => m.Uri.IsFile))
            {
                Log.Warn("ts文件不是本地路径");
                return;
            }
        }

        public void OnReset()
        {
            if (string.IsNullOrWhiteSpace(M3u8DirUrl))
                return;

            M3u8DirUrl = default!;
            VideoName = default!;
            Method = "AES-128";
            Key = default!;
            Iv = default!;
            SavePath = default!;
            Progress = default!;
            Log.Info("数据已全部清空");
        }

    }
}
