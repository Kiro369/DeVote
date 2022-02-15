using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.Test)]
    class Test : IPacket
    {
        byte[] incomingPacket;
        [ProtoMember(1)] public string Message { get; set; }
        public Test(byte[] arr)
        {
            incomingPacket = arr.Skip(4).ToArray();
        }

        public void Handle(Node client)
        {
            Console.WriteLine($"[{client.Address}] says: {Message}");
        }

        public bool Read(Node client)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(incomingPacket))
                {
                    Message =  Serializer.Deserialize<Test>(stream).Message;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte[] Create(string msg)
        {
            var tst = new Test(new byte[] { });
            tst.Message = msg;
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, tst);
                var srlzd = stream.ToArray();
                return (BitConverter.GetBytes((short)PacketTypes.Test).Concat(BitConverter.GetBytes((short)srlzd.Length))).Concat(srlzd).ToArray();
            }
        }
    }
}
