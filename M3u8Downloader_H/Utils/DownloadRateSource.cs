using System;
using System.Threading;

namespace M3u8Downloader_H.Utils
{
    public class DownloadRateSource : IProgress<long>, IDisposable
    {
        private readonly Action<long> _handler;
        private long _BitRateValue;
        private long _lastBitRateValue;
        private readonly Timer _timer = default!;

        public DownloadRateSource(Action<long> Handler)
        {
            _handler = Handler;
            _timer = new Timer(s => TimerCallback());
        }

        public void Run()
        {
            _timer.Change(0, 1000);
        }

        private void TimerCallback()
        {
            var bit = _BitRateValue;
            var bytes = bit - _lastBitRateValue;
            _lastBitRateValue = bit;
            _handler.Invoke(bytes);
        }

        public void Dispose()
        {
            _timer.Dispose();
            _handler(-1);
            GC.SuppressFinalize(this);
        }

        public void Report(long value) => Interlocked.Add(ref _BitRateValue, value);
    }
}
