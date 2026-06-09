using Avalonia.Threading;
using M3u8Downloader_H.Progress.Utils;


namespace M3u8Downloader_H.Progress.Services
{
    internal class GlobalProgressService
    {
        public static GlobalProgressService Instance = new();

        private readonly DispatcherTimer _timer;
        private readonly Lock _lock = new();
        private int _counter = 0;

        private GlobalProgressService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
        }

        private void Refresh()
        {
            lock (_lock)
            {
                if (_counter > 0 && !_timer.IsEnabled)
                    _timer.Start();
                else if (_counter < 1 && _timer.IsEnabled)
                {
                    _timer.Stop();
                }
            }
        }

        public IDisposable AddTickCallback(EventHandler eventHandler)
        {
            lock (_lock)
            {
                _timer.Tick += eventHandler;
                _counter++;
            }
            Refresh();

            return new Disposable(() =>
            {
                lock (_lock)
                {
                    _timer.Tick -= eventHandler;
                    _counter--;
                }
                Refresh();
            });

        }
    }
}
