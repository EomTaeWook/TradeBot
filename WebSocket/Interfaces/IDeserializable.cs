using Dignus.Collections;

namespace Dignus.WebSockets.Interfaces
{
    public interface IDeserializable
    {
        T DeserializeBody<T>(ArrayQueue<byte> body);
    }
}
