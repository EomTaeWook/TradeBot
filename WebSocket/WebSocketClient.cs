using Dignus.WebSockets.Interfaces;
using System.Net.WebSockets;

namespace Dignus.WebSockets
{
    public abstract class WebSocketClientBase
    {
        protected abstract void OnConnected(ISession session);
        protected abstract void OnDisconnected(ISession session);

        private int _connected = 0;
        private Session _session;
        private readonly SerializerFactoryDelegate _serializerFactoryDelegate;
        private readonly SocketOption _socketOption;
        public WebSocketClientBase(SessionConfiguration sessionConfiguration)
        {
            _serializerFactoryDelegate = sessionConfiguration.SerializerFactoryDelegate;
            _socketOption = sessionConfiguration.SocketOption;
        }
        public async Task ConnectAsync(string uriString, int timeout = 5000)
        {
            if (Interlocked.CompareExchange(ref _connected, 1, 0) != 0)
            {
                throw new Exception("already connect client");
            }

            var clientWebSocket = new ClientWebSocket();

            try
            {
                var timeoutTokenSource = new CancellationTokenSource(timeout);
                await clientWebSocket.ConnectAsync(new Uri(uriString), timeoutTokenSource.Token);
            }
            catch (Exception)
            {
                _connected = 0;
                throw;
            }

            _session = CreateSession(clientWebSocket);
        }
        private void Disconnected(ISession session)
        {
            OnDisconnected(session);
            Close();
        }
        private Session CreateSession(WebSocket webSocket)
        {
            var sessionInitializationParams = _serializerFactoryDelegate();

            if (sessionInitializationParams.Item1 == null)
            {
                throw new ArgumentNullException(nameof(IPacketDeserializer));
            }
            var sessionComponents = sessionInitializationParams.Item2;
            if (sessionComponents == null)
            {
                sessionComponents = Array.Empty<ISessionComponent>();
            }
            var session = new Session(sessionInitializationParams.Item1,
                sessionComponents);

            session.SetSocket(webSocket, _socketOption);
            _session.SetDisposedCallback(Disconnected);

            return session;
        }
        public Task SendAsync(byte[] bytes, WebSocketMessageType webSocketMessageType)
        {
            return SendAsync(bytes, webSocketMessageType, CancellationToken.None);
        }
        public Task SendAsync(byte[] bytes, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken)
        {
            return _session.SendAsync(bytes, webSocketMessageType, cancellationToken);
        }

        public void Close()
        {
            if (_session == null)
            {
                return;
            }
            if (_session.IsDispose() == false)
            {
                _session.Dispose();
            }
            _connected = 0;
        }
    }
}
