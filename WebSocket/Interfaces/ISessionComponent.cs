namespace Dignus.WebSockets.Interfaces
{
    public interface ISessionComponent
    {
        void SetSession(ISession session);
        void Dispose();

    }
}
