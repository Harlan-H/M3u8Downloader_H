using CliWrap;
using System.Text;
using M3u8Downloader_H.Common.Extensions;
using CliWrap.Builders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Combiners.Utils;
using System.Diagnostics;
using M3u8Downloader_H.Combiners.Extensions;

using CliWrap.EventStream;
using M3u8Downloader_H.Common.Utils;

namespace M3u8Downloader_H.Combiners.VideoConverter
{
    public class FFmpeg(ILog Log, IDownloadParamBase DownloadParams, IMergeSetting Settings)
    {
        public string CachePath { get; set; } = DownloadParams.CachePath;

        private static readonly string _filePath = StorageSpaceManager.GetFFmpegPath();

        public async ValueTask ConvertToMp4(IList<IStreamInfo> medias, IDialogProgress dialogProgress,CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();
            foreach (var item in medias)
            {
                arguments.Add("-i").Add(Path.Combine(CachePath, item.Title));
            }
            arguments.Add("-allowed_extensions").Add("ALL");

            await ConvertToMp4(arguments, dialogProgress,null, cancellationToken);
        }

        public async ValueTask ConvertToMp4(IM3uFileInfo m3UFileInfo, IDialogProgress dialogProgress, CancellationToken cancellationToken = default)
        {
            if (!m3UFileInfo.MediaFiles.Any())
                throw new ArgumentException("m3u8文件内的文件不能为空");

            PipeSource pipeSource = default!;
            var arguments = new ArgumentsBuilder();
            if (m3UFileInfo.IsFile && m3UFileInfo.MediaFiles.First().Duration == 0)
            {
                arguments.Add("-f").Add("concat")
                        .Add("-safe").Add(0);
                pipeSource = PipeSource.Create(m3UFileInfo.GenerateConcatStream(CachePath));
            }
            else
            {
                arguments.Add("-f").Add("hls")
                    //allowed_extensions必须在-i pipe:0之前执行否则会报错"If you wish to override this adjust allowed_extensions, you can set it to 'ALL' to allow all"
                    .Add("-allowed_extensions").Add("ALL");
                pipeSource = PipeSource.Create(m3UFileInfo.GenerateM3U8Stream(CachePath));
            }

            arguments
                .Add("-protocol_whitelist").Add("file,pipe")
                .Add("-i").Add("pipe:0")
                .Add("-bsf:a").Add("aac_adtstoasc");
            await ConvertToMp4(arguments, dialogProgress, pipeSource, cancellationToken);
        }


        private async ValueTask ConvertToMp4(ArgumentsBuilder argumentsBuilder, IDialogProgress dialogProgress,PipeSource? pipe, CancellationToken cancellationToken = default)
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

            await ExecuteAsync(argumentsBuilder.Build(), dialogProgress, pipe, cancellationToken);
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

