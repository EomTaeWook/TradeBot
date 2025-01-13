using System.Net.WebSockets;

namespace Dignus.WebSockets
{
    public class Session
    {
        private ClientWebSocket _webSocket;
        private int _disposed = 0;
        public Session()
        {
        }
        public void Send(ArraySegment<byte> bytes, WebSocketMessageType webSocketMessageType)
        {
            _webSocket.SendAsync(bytes, webSocketMessageType, true, CancellationToken.None);
        }

        public void SetClientWebSocket(ClientWebSocket webSocket)
        {
            _webSocket = webSocket;
        }
        public bool IsDispose()
        {
            return _disposed == 1;
        }
        public void Dispose()
        {
            if (_disposed == 1)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
                _webSocket.Dispose();

                _webSocket = null;
            }
        }

    }
}
