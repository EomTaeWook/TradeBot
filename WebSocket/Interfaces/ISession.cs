using System.Net.WebSockets;

namespace Dignus.WebSockets.Interfaces
{
    public interface ISession
    {
        Task SendAsync(byte[] bytes, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken);
        Task SendAsync(ArraySegment<byte> bytes, WebSocketMessageType webSocketMessageType, CancellationToken cancellationToken);
        void Dispose();
        void AddSessionComponent(ISessionComponent component);
        void RemoveSessionComponent(ISessionComponent component);
        WebSocket GetWebSocket();
    }
}
