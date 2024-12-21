using CliWrap.Builders;
using M3u8Downloader_H.Combiners.Extensions;
using M3u8Downloader_H.Combiners.Interfaces;
using M3u8Downloader_H.Combiners.M3uCombiners;
using M3u8Downloader_H.Common.Interfaces;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.VideoConverter;
using M3u8Downloader_H.Settings.Models;

namespace M3u8Downloader_H.Combiners
{
    public class M3uCombinerClient
    {
        private readonly FFmpeg _ffmpeg;
        private readonly M3UFileInfo _m3UFileInfo;

        public ILog? Log { get; set; }  
        public IDownloadParams DownloadParams { get; set; } = default!;
        public ISettings Settings { get; set; } = default!;

        public M3uCombinerClient(M3UFileInfo M3UFileInfo)
        {
#if DEBUG
            _ffmpeg = new(@"F:\源代码\库\ffmpeg-4.3.1-2020-11-19-full_build-shared\bin\ffmpeg.exe");
#else
            _ffmpeg = new("./ffmpeg.exe");
#endif
            _m3UFileInfo = M3UFileInfo;
        }

        public async Task Converter(bool isFile, CancellationToken cancellationToken = default)
        {
            Log?.Info("开始合并视频流");
            if (Settings.SelectedFormat == "mp4")
            {
                if (_m3UFileInfo.MediaFiles.Any(m => m.Duration > 0))
                    await ConvertWithM3u8File(cancellationToken);
                else
                    await ConvertWithFile(isFile, cancellationToken);
            }
            else
            {
                await VideoMerge(isFile, cancellationToken);
            }
            Log?.Info("合并完成");
            RemoveCacheDirectory(DownloadParams.VideoFullPath);
        }

        //通过xml,目录,json等方式可能无法判断流的时长，所以采用原先的转码方案
        protected async ValueTask ConvertWithFile(bool isFile, CancellationToken cancellationToken)
        {
            await VideoMerge(isFile, cancellationToken);
            await ConverterToMp4(DownloadParams.VideoFullName, false, cancellationToken);
            File.Delete(DownloadParams.VideoFullName);
        }

        protected async ValueTask ConvertWithM3u8File(CancellationToken cancellationToken)
        {
            string m3u8FilePath = Path.Combine(DownloadParams.VideoFullPath, "generated.m3u8");
            if (Settings.ForcedMerger)
                _m3UFileInfo.MediaFiles = _m3UFileInfo.MediaFiles.Where(m => File.Exists(Path.Combine(DownloadParams.VideoFullPath, m.Title))).ToList();
            await _m3UFileInfo.WriteToAsync(m3u8FilePath, cancellationToken);
            await ConverterToMp4(m3u8FilePath, true, cancellationToken);
            File.Delete(m3u8FilePath);
        }

        protected async ValueTask VideoMerge(bool isFile, CancellationToken cancellationToken = default)
        {
            using M3uCombiner m3UCombiner = isFile && _m3UFileInfo.Key is not null
                ? new CryptM3uCombiner(_m3UFileInfo, DownloadParams.VideoFullPath)
                : new M3uCombiner(DownloadParams.VideoFullPath);

            m3UCombiner.Progress = DownloadParams.VodProgress;
            m3UCombiner.Initialization(DownloadParams.VideoFullName);
            await m3UCombiner.MegerVideoHeader(_m3UFileInfo.Map, cancellationToken);
            await m3UCombiner.Start(_m3UFileInfo, Settings.ForcedMerger, cancellationToken);
        }

        protected async ValueTask ConverterToMp4(string m3u8FilePath, bool allowed_extensions, CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();

            if (allowed_extensions)
                arguments.Add("-allowed_extensions").Add("ALL");

            arguments.Add("-i").Add(m3u8FilePath)
                     .Add("-f").Add(Settings.SelectedFormat)
                     .Add("-c:a").Add("copy")
                     .Add("-c:v").Add("copy")
                     .Add("-bsf:a").Add("aac_adtstoasc");

            var tmpOutputFile = Path.ChangeExtension(DownloadParams.VideoFullName, Settings.SelectedFormat);
            Log?.Info("开始转码:%s", tmpOutputFile);
            DownloadParams.ChangeVideoNameDelegate(tmpOutputFile);
            arguments
                .Add("-nostdin")
                .Add("-y").Add(tmpOutputFile);

            await _ffmpeg.ExecuteAsync(arguments.Build(), DownloadParams.VodProgress, cancellationToken);
        }


        protected void RemoveCacheDirectory(string filePath, bool recursive = true)
        {

            if (Settings.IsCleanUp)
            {
                if (!Directory.Exists(filePath)) return;

                Directory.Delete(filePath, recursive);
                Log?.Info("删除{0}目录成功", filePath);
            }

        }

    }
}
