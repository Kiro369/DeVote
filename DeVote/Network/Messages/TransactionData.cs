using DeVote.Extensions;
using DeVote.Network.Transmission;
using DeVote.Structures;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.TransactionData)]
    class TransactionData : Packet
    {
        public static Dictionary<string, TransactionRecord> RecievedRecords = new Dictionary<string, TransactionRecord>();

        [ProtoMember(1)] public string Hash { get; set; }
        [ProtoMember(2)] public TransactionRecord Record { get; set; }

        public TransactionData(byte[] incomingPacket) : base(incomingPacket) {}
        public TransactionData() : base(null) { }

        public override void Handle(Node client)
        {
            if (Record == null || Record.Hash.Length == 0)
            {
                if (Settings.Current.FullNode)
                {
                    if (!string.IsNullOrEmpty(Hash))
                    {
                        Record = TransactionsDLT.Current.GetRecord(Hash);
                        client.Send(Create());
                    }
                }
            }
            else
            {
                RecievedRecords.Add(Encoding.UTF8.GetString(Record.Hash), Record);
            }
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<TransactionData>().CopyProperties(this);
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
            Finalize<TransactionData>();
            return Buffer;
        }
    }
}
