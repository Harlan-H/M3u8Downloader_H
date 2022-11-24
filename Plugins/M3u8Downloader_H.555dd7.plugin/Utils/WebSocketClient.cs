using System.Net.WebSockets;

namespace M3u8Downloader_H._555dd7.plugin.Utils
{
    internal class WebSocketClient : IDisposable
    {
        private readonly ClientWebSocket _clientWebSocket;
        public WebSocketClient()
        {
            _clientWebSocket = new ClientWebSocket();
        }

        public async Task<byte[]> SendAsync(Uri uri, ReadOnlyMemory<byte> buffer ,CancellationToken cancellationToken)
        {
            await _clientWebSocket.ConnectAsync(uri, cancellationToken);
            await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Binary,true, cancellationToken);
            var receiveBuffer = new byte[1024];
            Memory<byte> memory = new(receiveBuffer);
            int count = 0;
            while (true)
            {
                var resp = await _clientWebSocket.ReceiveAsync(memory.Slice(count, receiveBuffer.Length), cancellationToken);
                count += resp.Count;
                if (resp.EndOfMessage)
                    break;
            }
            await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            return memory[..count].ToArray();
        }

        public void Dispose()
        {
            _clientWebSocket.Dispose();
        }
    }
}
