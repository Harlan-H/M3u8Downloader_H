using CliWrap;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using M3u8Downloader_H.Common.Extensions;

namespace M3u8Downloader_H.Core.VideoConverter
{
    internal partial class FFmpegProgressRouter(IProgress<double> output) : PipeTarget
    {
        private readonly StringBuilder _buffer = new();
        private readonly IProgress<double> _output = output;

        private TimeSpan? _totalDuration;
        private TimeSpan? _lastOffset;

        private TimeSpan? TryParseTotalDuration(string data)
        {
            return data
            .Pipe(s => Duration().Match(s).Groups[1].Value)
            .NullIfWhiteSpace()?
            .Pipe(s => TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));
        }

        private TimeSpan? TryParseCurrentOffset(string data) => data
            .Pipe(s => Time().Matches(s)
                .Cast<Match>()
                .LastOrDefault()?
                .Groups[1]
                .Value)?
            .NullIfWhiteSpace()?
            .Pipe(s => TimeSpan.ParseExact(s, "c", CultureInfo.InvariantCulture));

        private void HandleBuffer()
        {
            var data = _buffer.ToString();

            _totalDuration ??= TryParseTotalDuration(data);
            if (_totalDuration is null)
                return;

            var currentOffset = TryParseCurrentOffset(data);
            if (currentOffset is null || currentOffset == _lastOffset)
                return;

            _lastOffset = currentOffset;

            var progress = (
                currentOffset.Value.TotalMilliseconds / _totalDuration.Value.TotalMilliseconds
            ).Clamp(0, 1);

            _output.Report(progress);
        }

        public override async Task CopyFromAsync(Stream source, CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(source, Console.OutputEncoding, false, 1024, true);

            var buffer = new char[1024];
            int charsRead;

            while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _buffer.Append(buffer, 0, charsRead);
                HandleBuffer();
            }
        }

        [GeneratedRegex(@"time=(\d\d:\d\d:\d\d.\d\d)")]
        private static partial Regex Time();

        [GeneratedRegex(@"Duration:\s(\d\d:\d\d:\d\d.\d\d)")]
        private static partial Regex Duration();
    }
}