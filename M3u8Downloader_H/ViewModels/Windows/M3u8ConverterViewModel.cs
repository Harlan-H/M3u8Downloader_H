using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.M3U8;
using M3u8Downloader_H.Utils;
using M3u8Downloader_H.Extensions;
using M3u8Downloader_H.ViewModels.Utils;
using PropertyChanged;
using System.Windows.Input;

namespace M3u8Downloader_H.ViewModels.Windows
{
    public class M3u8ConverterViewModel : Screen
    {
        private readonly M3u8FileInfoLocal m3U8FileInfoLocal;
        private M3UFileInfo _fileInfo = default!;

        [OnChangedMethod(nameof(OnM3u8FileUrlChanged))]
        public string M3u8FileUrl { get; set; } = default!;
        public string VideoName { get; set; } = default!;
        public string Method { get; set; } = default!;
        public string Key {  get; set; } = default!;
        public string Iv { get; set; } = default!;
        public MyLog Log { get; } = new();

        public M3u8ConverterViewModel()
        {
            m3U8FileInfoLocal = new M3u8FileInfoLocal(Log);
        }

        private void OnM3u8FileUrlChanged(string oldValue, string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                return;

            if (oldValue == newValue )
            {
                Log.Warn("本次传入的文件和上次传入的一致");
                return;
            }

            FileInfo fileInfo = new(newValue);
            if (!fileInfo.Exists)
            {
                Log.Warn("{0}文件不存在", Path.GetFileName(newValue));
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
            if (string.IsNullOrWhiteSpace(M3u8FileUrl))
                return;

            M3u8FileUrl = default!;
            VideoName = default!;
            Method = "AES-128";
            Key = default!;
            Iv = default!;
            Log.Info("数据以全部清空");
        }
    }
}
