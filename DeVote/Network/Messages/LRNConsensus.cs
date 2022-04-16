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
        long ConsensusRN = long.MaxValue;
        string MachineID = string.Empty;

        public LRNConsensus(byte[] incomingPacket) : base(incomingPacket) { }
        public LRNConsensus() : base(null) { }

        public override void Handle(Node client)
        {
            client.ConsensusRN = ConsensusRN;
            client.MachineID = MachineID;
        }

        public override bool Read(Node client)
        {
            try
            {
                Seek(4);
                ConsensusRN = ReadLong();
                MachineID = ReadString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public byte[] Create(long consensusRN)
        {
            Resize(12 + Constants.MachineID.Length);
            Seek(4);
            WriteLong(ConsensusRN);
            WriteStringWithLength(Constants.MachineID);
            Finalize<LRNConsensus>();
            return Buffer;
        }
    }
}
