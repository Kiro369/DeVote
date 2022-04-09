using DeVote.Extensions;
using DeVote.Network.Transmission;
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
    class Test : Packet
    {
        [ProtoMember(1)] public string Message { get; set; }
        public Test(byte[] arr) :base(arr) { }

        public override void Handle(Node client)
        {
            Console.WriteLine($"[{client.EndPoint}] says: {Message}");
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<Test>().CopyProperties(this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] Create(string msg)
        {
            Message = msg;
            Serialize(this);
            Finalize<Test>();
            return Buffer;
        }
    }
}
