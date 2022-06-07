using DeVote.Extensions;
using DeVote.Network.Transmission;
using DeVote.Structures;
using ProtoBuf;
using System;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.LatestHeight)]
    class LatestHeight : Packet
    {
        [ProtoMember(1)] public PacketType Type { get; set; }
        [ProtoMember(2)] public int BlockHeight { get; set; }

        public LatestHeight(byte[] incomingPacket) : base(incomingPacket) {}
        public LatestHeight() : base(null) { }

        public override void Handle(Node client)
        {
            switch (Type)
            {
                case PacketType.Request:
                    BlockHeight = Blockchain.Current.Blocks.Last.Value.Height;
                    Type = PacketType.Response;
                    client.Send(Create());
                    break;
                case PacketType.Response:
                    if (BlockHeight > NetworkManager.LatestBlockHeight)
                    {
                        NetworkManager.LatestBlockHeight = BlockHeight;
                    }
                    break;
            }
        }

        public override bool Read(Node client)
        {
            {
                try
                {
                    Deserialize<LatestHeight>().CopyProperties(this);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public byte[] Create()
        {
            Serialize(this);
            Finalize<LatestHeight>();
            return Buffer;
        }
    }
}
