using M3u8Downloader_H.Common.Models;


namespace M3u8Downloader_H.Common.Services
{
    public sealed class TimeoutStream(
        Stream inner,
        TimeOutOptions options) : Stream
    {
        //private long _windowBytes;
        //private readonly Stopwatch _speedWatch = Stopwatch.StartNew();

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            var readTask = inner.ReadAsync(buffer, cancellationToken).AsTask();
            var timeoutTask = Task.Delay(options.IdleTimeout,cancellationToken);

            var completed = await Task.WhenAny(readTask,timeoutTask);
            if (completed.IsCompletedSuccessfully && completed != readTask)
            {
                throw new TimeoutException($"数据读取超时 {options.IdleTimeout.TotalSeconds} 秒");
            }

            int read = await readTask;
//             if (read > 0)
//             {
//                 CheckMinSpeed(read);
//             }

            return read;
        }

//         private void CheckMinSpeed(int read)
//         {
//             if (options.MinBytesPerSecond is null)
//                 return;
//             
//             _windowBytes += read;
// 
//             if (_speedWatch.Elapsed < options.SpeedCheckWindow)
//                 return;
// 
//             double speed = _windowBytes / _speedWatch.Elapsed.TotalSeconds;
// 
//             if (speed < options.MinBytesPerSecond.Value)
//             {
//                 throw new IOException(
//                     $"下载速率太低 " +
//                     $"当前: {speed:F2} B/s");
//             }
// 
//             _windowBytes = 0;
//             _speedWatch.Restart();
//         }



        public override bool CanRead => inner.CanRead;

        public override bool CanSeek => inner.CanSeek;

        public override bool CanWrite => inner.CanWrite;

        public override long Length => inner.Length;

        public override long Position
        {
            get => inner.Position;
            set => inner.Position = value;
        }

        public override void Flush()
                => inner.Flush();

        public override int Read(byte[] buffer,int offset,int count)
                => inner.Read(buffer,offset,count);

        public override long Seek(long offset,SeekOrigin origin)
                => inner.Seek(offset, origin);
        

        public override void SetLength(long value) 
                => inner.SetLength(value);

        public override int ReadByte()
                => inner.ReadByte();

        public override void Write(byte[] buffer, int offset, int count)
                => inner.Write(buffer, offset, count);

        public override async Task<int> ReadAsync(byte[] buffer,int offset,int count,CancellationToken cancellationToken)
            => await ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
            }

            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await inner.DisposeAsync();

            await base.DisposeAsync();
        }
    }
}
