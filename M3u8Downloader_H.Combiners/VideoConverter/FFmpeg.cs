using CliWrap;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.Extensions;

namespace M3u8Downloader_H.Core.VideoConverter
{
    internal class FFmpeg(string filePath)
    {
        private readonly string _filePath = filePath;

        public async ValueTask ExecuteAsync(
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

