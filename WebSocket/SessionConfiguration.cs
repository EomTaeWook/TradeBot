using Dignus.WebSockets.Interfaces;

namespace Dignus.WebSockets
{
    public delegate Tuple<IPacketDeserializer, ICollection<ISessionComponent>> SerializerFactoryDelegate();
    public class SessionConfiguration
    {
        public SocketOption SocketOption { get; private set; }
        public SerializerFactoryDelegate SerializerFactoryDelegate { get; private set; }

        public SessionConfiguration(SerializerFactoryDelegate serializerFactoryDelegate, SocketOption socketOption = null)
        {
            if (serializerFactoryDelegate == null)
            {
                throw new ArgumentNullException(nameof(serializerFactoryDelegate));
            }
            SerializerFactoryDelegate = serializerFactoryDelegate;
            if (socketOption == null)
            {
                socketOption = new SocketOption();
            }
            SocketOption = socketOption;
        }
    }
}
