namespace Dignus.WebSockets.Interfaces
{
    public interface IPacketDeserializer
    {
        void Deserialize(byte[] buffer);
    }
}
