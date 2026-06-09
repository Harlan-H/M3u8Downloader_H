using CliWrap;
using System.Text;
using M3u8Downloader_H.Common.Extensions;
using CliWrap.Builders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Combiners.Utils;
using M3u8Downloader_H.Common.Utils;
using M3u8Downloader_H.Progress.Interfaces;
using M3u8Downloader_H.Progress.Extensions;

namespace M3u8Downloader_H.Combiners.VideoConverter
{
    public class FFmpeg(ILog Log, IDownloadParamBase DownloadParams, IMergeSetting Settings)
    {
        public string CachePath { get; set; } = DownloadParams.CachePath;

        private static readonly string _filePath = StorageSpaceManager.GetFFmpegPath();

        public async ValueTask ConvertToMp4(IList<IStreamInfo> medias, IProgress<double> progress,CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();
            foreach (var item in medias)
            {
                arguments.Add("-i").Add(Path.Combine(CachePath, item.Title));
            }
            arguments.Add("-allowed_extensions").Add("ALL");

            await ConvertToMp4(arguments, progress, null, cancellationToken);
        }

        public async ValueTask ConvertToMp4(IList<IM3uFileInfoSource> m3UFileInfoSources, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();
            foreach(var item in m3UFileInfoSources)
            {
                var index = Path.Combine(CachePath, item.CachePath, "index.m3u8");
                arguments.Add("-allowed_extensions").Add("ALL")
                    .Add("-i").Add(index);
            }

            arguments
                .Add("-f").Add("hls");
            await ConvertToMp4(arguments, progress, null, cancellationToken);
        }


        private async ValueTask ConvertToMp4(ArgumentsBuilder argumentsBuilder, IProgress<double> progress, PipeSource? pipe, CancellationToken cancellationToken = default)
        {
            argumentsBuilder
                     .Add("-f").Add(Settings.SelectedFormat)
                     .Add("-c:a").Add("copy")
                     .Add("-c:v").Add("copy");

            Log?.Info("开始转码:{0}", DownloadParams.VideoName + '.' + Settings.SelectedFormat);
            argumentsBuilder
                .Add("-nostdin")
                .Add("-y").Add(DownloadParams.VideoFullName);

            DirEx.CreateDirecotry(DownloadParams.SavePath);

            await ExecuteAsync(argumentsBuilder.Build(), progress, pipe, cancellationToken);
        }

        private async ValueTask ExecuteAsync(
             string arguments,
             IProgress<double> progress,
             PipeSource ?pipe,
             CancellationToken cancellationToken = default)
        {
            var stdErrBuffer = new StringBuilder();

            var stdErrPipe = PipeTarget.Merge(
                PipeTarget.ToStringBuilder(stdErrBuffer,Encoding.UTF8), // error data collector
                progress?.Pipe(p => new FFmpegProgressRouter(p)) ?? PipeTarget.Null // progress
            );
            
            var commad = Cli.Wrap(_filePath)
                .WithArguments(arguments)
                .WithStandardErrorPipe(stdErrPipe)
                .WithValidation(CommandResultValidation.None);

            if (pipe is not null)
                commad = commad.WithStandardInputPipe(pipe);

            var result = await commad.ExecuteAsync(cancellationToken);

            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"ffmpeg异常退出 退出码 ({result.ExitCode})." +
                    Environment.NewLine +

                    "参数:" +
                    Environment.NewLine +
                    arguments +
                    Environment.NewLine +

                    "错误是:" +
                    Environment.NewLine +
                    stdErrBuffer
                );
            }
        }
      
    }
}

