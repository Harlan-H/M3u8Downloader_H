using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace M3u8Downloader_H.Utils
{
    internal partial class ThrottlingSemaphore
    {
        public static ThrottlingSemaphore Instance { get; } = new();

        private readonly Lock _lock = new();
        private readonly Queue<TaskCompletionSource> _waiters = new();

        private int _maxCount = 0;
        private int _count;

        public int MaxCount 
        { 
            get
            {
                lock(_lock)
                {
                    return _maxCount;
                }
            }
            set
            {
                lock (_lock)
                {
                    if(_maxCount == value) 
                        return;

                    _maxCount = value;
                    Refresh();
                }
            }
        }

        private ThrottlingSemaphore()
        {
            
        }

        private void Refresh()
        {
            lock (_lock)
            {
                while (_count < MaxCount && _waiters.TryDequeue(out var waiter))
                {
                    if (waiter.TrySetResult())
                        _count++;
                }
            }
        }

        public async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
        {
            var waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            await using (cancellationToken.Register(() => waiter.TrySetCanceled(cancellationToken)))
            {
                lock (_lock)
                {
                    _waiters.Enqueue(waiter);
                    Refresh();
                }

                await waiter.Task;

                return new AcquiredAccess(this);
            }
        }

        private void Release()
        {
            lock (_lock)
            {
                _count--;
                Refresh();
            }
        }
    }

    internal partial class ThrottlingSemaphore
    {
        private class AcquiredAccess(ThrottlingSemaphore semaphore) : IDisposable
        {
            private bool _isDisposed;

            public void Dispose()
            {
                if (!_isDisposed)
                {
                    semaphore.Release();
                }

                _isDisposed = true;
            }
        }
    }
}
