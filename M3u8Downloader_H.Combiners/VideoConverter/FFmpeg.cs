using CliWrap;
using System.Text;
using M3u8Downloader_H.Common.Extensions;
using CliWrap.Builders;
using M3u8Downloader_H.Abstractions.Common;
using M3u8Downloader_H.Abstractions.Settings;
using M3u8Downloader_H.Abstractions.M3u8;
using M3u8Downloader_H.Combiners.Utils;

namespace M3u8Downloader_H.Combiners.VideoConverter
{
    public class FFmpeg(ILog Log, IDownloadParamBase DownloadParams, IMergeSetting Settings)
    {
        public string CachePath { get; set; } = DownloadParams.CachePath;

        private readonly string _filePath
#if DEBUG
    = new(@"F:\源代码\库\ffmpeg\bin\ffmpeg.exe");
#else
            = new("./ffmpeg.exe");
#endif

        public async ValueTask ConvertToMp4(IList<IStreamInfo> medias, IDialogProgress dialogProgress,CancellationToken cancellationToken = default)
        {
            var arguments = new ArgumentsBuilder();
            foreach (var item in medias)
            {
                arguments.Add("-i").Add(item.Title);
            }

            await ConvertToMp4(arguments, dialogProgress, cancellationToken);
        }

        public async ValueTask ConvertToMp4(string m3u8ConcatTxt, IDialogProgress dialogProgress, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(m3u8ConcatTxt))
                throw new ArgumentException($"{m3u8ConcatTxt} 文件不存在");
            
            var arguments = new ArgumentsBuilder();
            arguments.Add("-f").Add("concat")
                    .Add("-safe").Add(0)
                    .Add("-i").Add(m3u8ConcatTxt)
                    .Add("-bsf:a").Add("aac_adtstoasc");
            await ConvertToMp4(arguments, dialogProgress, cancellationToken);
        }

        public async ValueTask ConvertToMp4(IM3uFileInfo m3UFileInfo, IDialogProgress dialogProgress, CancellationToken cancellationToken = default)
        {
            if (!m3UFileInfo.MediaFiles.Any())
                throw new ArgumentException("m3u8文件内的文件不能为空");

            StringBuilder stringBuilder = new();

            if (Settings.ConcatMerger)
            {
                Log?.Info("列表合并方式已开启");

                foreach (var info in m3UFileInfo.MediaFiles)
                {
                    stringBuilder.AppendLine($"file '{info.Title}'");
                }

                var m3u8ConcatTxt = Path.Combine(DownloadParams.CachePath, "file_list.txt");
                File.WriteAllText(m3u8ConcatTxt, stringBuilder.ToString());
                await ConvertToMp4(m3u8ConcatTxt, dialogProgress, cancellationToken);
                File.Delete(m3u8ConcatTxt);
            }
            else
            {

                stringBuilder.Append("concat:");
                stringBuilder.Append(m3UFileInfo.MediaFiles[0].Title);
                for (int i = 1; i < m3UFileInfo.MediaFiles.Count; i++)
                {
                    stringBuilder.Append('|');
                    stringBuilder.Append(m3UFileInfo.MediaFiles[i].Title);
                }

                var arguments = new ArgumentsBuilder();
                arguments.Add("-i").Add(stringBuilder.ToString())
                         .Add("-bsf:a").Add("aac_adtstoasc");
                await ConvertToMp4(arguments, dialogProgress, cancellationToken);
            }

        }

        private async ValueTask ConvertToMp4(ArgumentsBuilder argumentsBuilder, IDialogProgress dialogProgress, CancellationToken cancellationToken = default)
        {
            argumentsBuilder
                     .Add("-allowed_extensions").Add("ALL")
                     .Add("-f").Add(Settings.SelectedFormat)
                     .Add("-c:a").Add("copy")
                     .Add("-c:v").Add("copy");

            Log?.Info("开始转码:{0}", DownloadParams.VideoName + '.' + Settings.SelectedFormat);
            argumentsBuilder
                .Add("-nostdin")
                .Add("-y").Add(DownloadParams.VideoFullName);

            DirEx.CreateDirecotry(DownloadParams.SavePath);

            await ExecuteAsync(argumentsBuilder.Build(), dialogProgress, cancellationToken);
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
                .WithWorkingDirectory(CachePath)
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

