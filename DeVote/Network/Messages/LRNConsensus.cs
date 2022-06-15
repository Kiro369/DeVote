using DeVote.Network.Transmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network.Messages
{
    [Handling.NodePacketHandler(PacketTypes.LRNConsensus)]
    class LRNConsensus : Packet
    {
        public long ConsensusRN = long.MaxValue;
        public string MachineID = string.Empty;
        public bool FullNode = false;

        public LRNConsensus(byte[] incomingPacket) : base(incomingPacket) { }
        public LRNConsensus() : base(null) { }

        public override void Handle(Node client)
        {
            if (ConsensusRN < long.MaxValue)
                client.ConsensusRN = ConsensusRN;

            client.MachineID = MachineID;
            client.FullNode = FullNode;
        }

        public override bool Read(Node client)
        {
            try
            {
                Seek(6);
                FullNode = ReadBoolean();
                ConsensusRN = ReadLong();
                MachineID = ReadString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] Create()
        {
            Resize(16 + MachineID.Length);
            Seek(6);
            WriteBoolean(FullNode);
            WriteLong(ConsensusRN);
            WriteStringWithLength(MachineID);
            Finalize<LRNConsensus>();
            return Buffer;
        }
    }
}
