using System;
using System.Timers;

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
            _timer =  new Timer(1000);
            _timer.Elapsed += TimerCallback;
        }

        public void Run()
        {
            _timer.Enabled = true;
        }

        private void TimerCallback(object? sender, ElapsedEventArgs e)
        {
            var bit = _BitRateValue;
            var bytes = bit - _lastBitRateValue;
            _lastBitRateValue = bit;
            _handler.Invoke(bytes);
        }

        public void Dispose()
        {
            _timer.Enabled = false;
            _timer.Dispose();
            _handler(-1);
            GC.SuppressFinalize(this);
        }

        public void Report(long value) => System.Threading.Interlocked.Add(ref _BitRateValue, value);
    }
}
