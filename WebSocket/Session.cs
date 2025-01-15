using Dignus.Collections;
using Dignus.WebSockets.Interfaces;
using System.Net.WebSockets;

namespace Dignus.WebSockets
{
    internal class Session : ISession
    {
        private readonly IPacketDeserializer _packetDeserializer;

        private WebSocket _webSocket;
        private int _disposed = 0;

        private int _bufferSize = 4096;
        private Action<Session> _disposedCallback;

        private readonly ArrayQueue<byte> _receivedBuffer = [];
        private readonly ArrayQueue<ISessionComponent> _persistentSessionComponents = [];
        public Session(IPacketDeserializer packetDeserializer,
            ICollection<ISessionComponent> persistentSessionComponents)
        {
            _packetDeserializer = packetDeserializer;
            foreach (var component in persistentSessionComponents)
            {
                _persistentSessionComponents.Add(component);
            }
        }
        public void SetSocket(WebSocket webSocket, SocketOption socketOption)
        {
            _webSocket = webSocket;
            _bufferSize = socketOption.BufferSize;

            foreach (var component in _persistentSessionComponents)
            {
                component.SetSession(this);
            }
            _disposed = 0;
        }
        public void SetDisposedCallback(Action<Session> disposedCallback)
        {
            _disposedCallback = disposedCallback;
        }
        private async Task BeginReceiveAsync()
        {
            byte[] buffer = new byte[_bufferSize];
            var receive = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

            _receivedBuffer.AddRange(new ArraySegment<byte>(buffer, 0, receive.Count));
            if (receive.EndOfMessage == false)
            {
                _ = BeginReceiveAsync();
                return;
            }
            var bodyBytes = _receivedBuffer.Read(_receivedBuffer.LongCount);
            _packetDeserializer.Deserialize(bodyBytes);
            _ = BeginReceiveAsync();
        }
        public Task SendAsync(byte[] bytes, WebSocketMessageType webSocketMessageType)
        {
            return _webSocket.SendAsync(bytes, webSocketMessageType, true, CancellationToken.None);
        }
        public Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType webSocketMessageType)
        {
            return _webSocket.SendAsync(bytes, webSocketMessageType, true, CancellationToken.None);
        }
        public Task SendAsync(byte[] bytes, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken)
        {
            return _webSocket.SendAsync(bytes, webSocketMessageType, true, cancellationToken);
        }
        public Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken)
        {
            return _webSocket.SendAsync(bytes, webSocketMessageType, true, cancellationToken);
        }

        internal void SetClientWebSocket(ClientWebSocket webSocket)
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

        public void AddSessionComponent(ISessionComponent component)
        {
            throw new NotImplementedException();
        }

        public void RemoveSessionComponent(ISessionComponent component)
        {
            throw new NotImplementedException();
        }

        public WebSocket GetWebSocket()
        {
            return _webSocket;
        }
    }
}
