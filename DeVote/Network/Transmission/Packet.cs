using DeVote.Extensions;
using DeVote.Network.Handling;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network.Transmission
{
    abstract class Packet : PacketProcessor, IPacket
    {
        readonly PacketTypes ID;
        readonly int IncomingLength;
        public Packet(byte[] incomingPacket) : base(incomingPacket)
        {
            if (incomingPacket == null || incomingPacket.Length < 6) return;
            IncomingLength = BitConverter.ToInt32(Buffer, 0);
            ID = (PacketTypes)BitConverter.ToInt16(Buffer, 4);
        }

        public T Deserialize<T>()
        {
            using var stream = new MemoryStream(Buffer.Skip(6).ToArray());
            return Serializer.Deserialize<T>(stream);
        }

        public void Serialize(object obj)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, obj);
            var srlzd = stream.ToArray();
            Buffer = new byte[srlzd.Length + 6];
            srlzd.CopyTo(Buffer, 6);
        }

        public unsafe void Finalize<T>()
        {
            var myAttribute = (NodePacketHandlerAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(NodePacketHandlerAttribute));
            base.WriteHeader(Buffer.Length - 6);
            fixed (byte* ptr = Buffer)
                *(short*)(ptr + 4) = (short)(PacketTypes)myAttribute.PacketTypeId;

        }

        public abstract void Handle(Node client);
        public abstract bool Read(Node client);
    }
}
