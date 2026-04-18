using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M3u8Downloader_H.Utils
{
    internal class DebounceDispatcher
    {
        private CancellationTokenSource? _cts;

        public async Task DebounceAsync(Func<Task> action, int delay)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                await Task.Delay(delay, _cts.Token);

                if (!_cts.Token.IsCancellationRequested)
                {
                    await action();
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        public async Task DebounceAsync(Action action, int delay)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                await Task.Delay(delay, _cts.Token);

                if (!_cts.Token.IsCancellationRequested)
                {
                    action();
                }
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
