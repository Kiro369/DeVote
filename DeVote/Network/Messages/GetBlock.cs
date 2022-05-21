using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeVote.Extensions;
using DeVote.Network.Transmission;
using DeVote.Structures;
using ProtoBuf;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.GetBlock)]
    class GetBlock : Packet
    {
        public static Dictionary<int, Block> RecievedBlocks = new Dictionary<int, Block>();
        [ProtoMember(1)] public PacketType Type { get; set; }
        [ProtoMember(2)] public int BlockHeight { get; set; }
        // fulldata => true 
        [ProtoMember(3)] public Block Block { get; set; }

        public GetBlock(byte[] incomingPacket) : base(incomingPacket) {}
        public GetBlock() : base(null) { }

        public override void Handle(Node client)
        {
            switch (Type)
            {
                case PacketType.Request:
                    Block = Block.LoadProtobuffedBlock(BlockHeight, Blockchain.Current.LevelDB);
                    // response
                    client.Send(Create());
                    break;
                case PacketType.Response:
                    if (Block != null && Block.Height > 0)
                    {
                        if (RecievedBlocks.ContainsKey(BlockHeight))
                            RecievedBlocks[BlockHeight] = Block;
                        else RecievedBlocks.Add(BlockHeight, Block);
                    }
                    break;
            }
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<GetBlock>().CopyProperties(this);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] Create()
        {
            Serialize(this);
            Finalize<GetBlock>();
            return Buffer;
        }
    }
}
