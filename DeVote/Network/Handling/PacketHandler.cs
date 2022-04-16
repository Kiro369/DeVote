using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DeVote.Network.Handling
{
    public sealed class PacketHandler<T>
       where T : IPacket
    {
        public PacketHandler(ConstructorInfo constructor, Enum typeId)
        {
            TypeId = typeId;
            Constructor = constructor;
        }

        public ConstructorInfo Constructor { get; private set; }

        public Enum TypeId { get; private set; }

        public void Invoke(Node client, params object[] parameters)
        {
            var processor = (T)Constructor.Invoke(parameters);

            if (processor.Read(client))
                processor.Handle(client);

        }
    }
}
