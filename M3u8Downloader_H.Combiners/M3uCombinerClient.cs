using CliWrap.Builders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Meger;
using M3u8Downloader_H.Combiners.Extensions;
using M3u8Downloader_H.Combiners.M3uCombiners;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Core.VideoConverter;

namespace M3u8Downloader_H.Combiners
{
    public class M3uCombinerClient(ILog Log, IDownloadParamBase DownloadParams)
    {
#if DEBUG
        private readonly FFmpeg _ffmpeg = new (@"F:\Sources\Library\ffmpeg-4.3.1-2020-11-19-full_build-shared\bin\ffmpeg.exe");
#else
        private readonly FFmpeg  _ffmpeg = new("./ffmpeg.exe");
#endif
        private readonly string _cachePath = Path.Combine(DownloadParams.SavePath, DownloadParams.VideoName);
        public M3UFileInfo M3UFileInfo { get; set; } = default!;
        public IDialogProgress DialogProgress { get; set; } = default!;
        public IMergeSetting Settings { get; set; } = default!;


        public async Task Converter(bool isFile, CancellationToken cancellationToken = default)
        {
            Log?.Info("开始合并视频流");
            if (Settings.SelectedFormat == "mp4")
            {
                if (M3UFileInfo.MediaFiles.Any(m => m.Duration > 0))
                    await ConvertWithM3u8File(cancellationToken);
                else
                    await ConvertWithFile(isFile, cancellationToken);
            }
            else
            {
                await VideoMerge(isFile, cancellationToken);
            }
            Log?.Info("合并完成");
            RemoveCacheDirectory(_cachePath);
        }

        //通过xml,目录,json等方式可能无法判断流的时长，所以采用原先的转码方案
        protected async ValueTask ConvertWithFile(bool isFile, CancellationToken cancellationToken)
        {
            await VideoMerge(isFile, cancellationToken);
            await ConvertToMp4(Path.Combine(DownloadParams.SavePath, DownloadParams.VideoFullName), false, cancellationToken);
            File.Delete(Path.Combine(DownloadParams.SavePath, DownloadParams.VideoFullName));
        }

        protected async ValueTask ConvertWithM3u8File(CancellationToken cancellationToken)
        {
            string m3u8FilePath = Path.Combine(_cachePath, "generated.m3u8");
            if (Settings.ForcedMerger)
                M3UFileInfo.MediaFiles = M3UFileInfo.MediaFiles.Where(m => File.Exists(Path.Combine(_cachePath, m.Title))).ToList();
            await M3UFileInfo.WriteToAsync(m3u8FilePath, cancellationToken);
            await ConvertToMp4(m3u8FilePath, true, cancellationToken);
            File.Delete(m3u8FilePath);
        }

        protected async ValueTask VideoMerge(bool isFile, CancellationToken cancellationToken = default)
        {
            using M3uCombiner m3UCombiner = isFile && M3UFileInfo.Key is not null
                ? new CryptM3uCombiner(M3UFileInfo, _cachePath)
                : new M3uCombiner(_cachePath);

            m3UCombiner.Progress = DialogProgress;
            m3UCombiner.Initialization(Path.Combine(DownloadParams.SavePath, DownloadParams.VideoFullName));
            await m3UCombiner.MegerVideoHeader(M3UFileInfo.Map, cancellationToken);
            await m3UCombiner.StartMerging(M3UFileInfo, Settings.ForcedMerger, cancellationToken);
        }

        protected async ValueTask ConvertToMp4(string m3u8FilePath, bool allowed_extensions, CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();

            if (allowed_extensions)
                arguments.Add("-allowed_extensions").Add("ALL");

            arguments.Add("-i").Add(m3u8FilePath)
                     .Add("-f").Add(Settings.SelectedFormat)
                     .Add("-c:a").Add("copy")
                     .Add("-c:v").Add("copy")
                     .Add("-bsf:a").Add("aac_adtstoasc");

           
            var tmpOutputFile = Path.ChangeExtension(DownloadParams.VideoName, Settings.SelectedFormat);
            DownloadParams.VideoFullName = tmpOutputFile;
            Log?.Info("开始转码:{0}", tmpOutputFile);
            arguments
                .Add("-nostdin")
                .Add("-y").Add(Path.Combine(DownloadParams.SavePath, tmpOutputFile));


           await _ffmpeg.ExecuteAsync(arguments.Build(), DialogProgress, cancellationToken);
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
