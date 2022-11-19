using System;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Utils
{
    public class DownloadRateSource : IProgress<long>, IDisposable
    {
        private readonly Action<long> _handler;
        private bool IsCompleted = false;
        private long _BitRateValue;
        private long _lastBitRateValue;

        public DownloadRateSource(Action<long> Handler)
        {
            _handler = Handler;
        }

        public async void Run(CancellationToken cancellationToken)
        {
            try
            {
                do
                {
                    var bytes = _BitRateValue - _lastBitRateValue;
                    _lastBitRateValue = _BitRateValue;
                    _handler(bytes);
                    await Task.Delay(1000, cancellationToken);
                } while (!IsCompleted);
            }catch(TaskCanceledException)
            {

            }
            _handler(-1);
        }

        public void Dispose()
        {
            IsCompleted = true;
            GC.SuppressFinalize(this);
        }

        public void Report(long value) => Interlocked.Add(ref _BitRateValue, value);
    }
}
