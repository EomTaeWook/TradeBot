using System.Net.WebSockets;

namespace Dignus.WebSockets
{
    public class WebSocketClientBase
    {
        private int _connected = 0;
        private Session _session;
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

            _session.SetClientWebSocket(clientWebSocket);
        }
        public void Send()
        {

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
