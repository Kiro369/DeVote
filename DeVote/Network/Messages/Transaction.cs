using DeVote.Cryptography;
using DeVote.Extensions;
using DeVote.Network.Transmission;
using DeVote.PyRecognition;
using DeVote.Structures;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.Transaction)]
    class Transaction : Packet
    {
        [ProtoMember(1)] public Structures.Transaction TransactionData { get; set; }
        [ProtoMember(2)] public TransactionRecord TxRecord { get; set; }

        public Transaction(byte[] incomingPacket) : base(incomingPacket) {}

        public override void Handle(Node client)
        {
            if (VotedDLT.Current.Contains(TransactionData.Elector))
            {
                // TODO: Boradcast a warning that this voter did vote before
            }
            else
            {
                (string frontIDPath, _) = TxRecord.DecompressID("webp");

                bool voterVerified = TxRecord.IsVoterVerified(frontIDPath);
                if (voterVerified)
                {
                    var extractedID = Recognition.Current.ExtractID(frontIDPath);
                    if (!string.IsNullOrEmpty(extractedID))
                    {
                        var hash = Argon2.ComputeHash(extractedID);
                        if (hash.Equals(TransactionData.Elector))
                        {
                            VotedDLT.Current.Add(TransactionData.Elector, client.MachineID);
                            TransactionData.Elector = client.MachineID;
                            Blockchain.Current.Block.AddTransaction(TransactionData);

                            if (Settings.Current.FullNode)
                            {
                                TransactionsDLT.Current.AddRecord(TxRecord);
                            }
                        }
                        else
                        {
                            // TODO: Hash didn't match the supplied hash, this transaction is compromised, or maybe its the machine????! 
                        }
                    }
                    else
                    {
                        // TODO: Acknowledge that we couldn't extract the ID number from the supplied ID image
                    }
                }
                else
                {
                    // TODO: Acknowledge that we couldn't verify this voter 
                }
            }
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<Transaction>().CopyProperties(this);
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
            Finalize<Transaction>();
            return Buffer;
        }
    }
}
