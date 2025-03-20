using CliWrap;
using System.Text;
using M3u8Downloader_H.Common.Extensions;
using CliWrap.Builders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Meger;
using M3u8Downloader_H.Common.M3u8Infos;
using M3u8Downloader_H.Abstractions.Extensions;

namespace M3u8Downloader_H.Combiners.VideoConverter
{
    public class FFmpeg(ILog Log, IDownloadParamBase DownloadParams)
    {
        public IDialogProgress DialogProgress { get; set; } = default!;
        public IMergeSetting Settings { get; set; } = default!;

        private readonly string _filePath
#if DEBUG
    = new(@"F:\源代码\库\ffmpeg\bin\ffmpeg.exe");
#else
            = new("./ffmpeg.exe");
#endif

        public async ValueTask ConvertToMp4(IMediaDownloadParam mediaDownloadParam,CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();
            foreach (var item in mediaDownloadParam.Medias)
            {
                arguments.Add("-i").Add($"{DownloadParams.VideoName}.{item.MediaType}.tmp");
            }
            if (mediaDownloadParam.Subtitle is not null)
                arguments.Add("-i").Add("");

            await ExecuteAsync(arguments.Build(), DialogProgress, cancellationToken);
        }

        public async ValueTask ConvertToMp4(string m3u8ConcatTxt, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(m3u8ConcatTxt))
                throw new ArgumentException($"{m3u8ConcatTxt} 文件不存在");
            
            var arguments = new ArgumentsBuilder();
            arguments.Add("-f").Add("concat")
                    .Add("-safe").Add(0)
                    .Add("-i").Add(m3u8ConcatTxt);
            await ConvertToMp4(arguments, cancellationToken);
        }

        public async ValueTask ConvertToMp4(M3UFileInfo m3UFileInfo,CancellationToken cancellationToken = default)
        {
            if (!m3UFileInfo.MediaFiles.Any())
                throw new ArgumentException("m3u8文件内的文件不能为空");

            StringBuilder stringBuilder = new();
            stringBuilder.Append("concat:");
            stringBuilder.Append(m3UFileInfo.MediaFiles[0].Title);
            for (int i = 1;i < m3UFileInfo.MediaFiles.Count;i++) 
            {
                stringBuilder.Append('|');
                stringBuilder.Append(m3UFileInfo.MediaFiles[i].Title);
            }

            var arguments = new ArgumentsBuilder();
            arguments.Add("-i").Add(stringBuilder.ToString());
            await ConvertToMp4(arguments, cancellationToken);
        }

        private async ValueTask ConvertToMp4(ArgumentsBuilder argumentsBuilder, CancellationToken cancellationToken = default)
        {
            argumentsBuilder
                     .Add("-allowed_extensions").Add("ALL")
                     .Add("-f").Add(Settings.SelectedFormat)
                     .Add("-c:a").Add("copy")
                     .Add("-c:v").Add("copy")
                     .Add("-bsf:a").Add("aac_adtstoasc");


            var tmpOutputFile = Path.ChangeExtension(DownloadParams.VideoName, Settings.SelectedFormat);
            Log?.Info("开始转码:{0}", tmpOutputFile);
            argumentsBuilder
                .Add("-nostdin")
                .Add("-y").Add(Path.Combine(DownloadParams.SavePath, tmpOutputFile));

            await ExecuteAsync(argumentsBuilder.Build(), DialogProgress, cancellationToken);
        }

        private async ValueTask ExecuteAsync(
             string arguments,
             IProgress<double> progress,
             CancellationToken cancellationToken = default)
        {
            var stdErrBuffer = new StringBuilder();

            var stdErrPipe = PipeTarget.Merge(
                PipeTarget.ToStringBuilder(stdErrBuffer), // error data collector
                progress?.Pipe(p => new FFmpegProgressRouter(p)) ?? PipeTarget.Null // progress
            );

            var result = await Cli.Wrap(_filePath)
                .WithArguments(arguments)
                .WithWorkingDirectory(DownloadParams.GetCachePath())
                .WithStandardErrorPipe(stdErrPipe)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(cancellationToken);

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

