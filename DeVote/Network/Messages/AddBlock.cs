using DeVote.Cryptography;
using DeVote.Extensions;
using DeVote.Network.Transmission;
using DeVote.Structures;
using ProtoBuf;
using static DeVote.Network.NetworkManager;

namespace DeVote.Network.Messages
{
    [ProtoContract(SkipConstructor = true)]
    [Handling.NodePacketHandler(PacketTypes.AddBlock)]
    class AddBlock : Packet
    {
        [ProtoMember(1)] public Block Block { get; set; }

        public AddBlock(byte[] incomingPacket) : base(incomingPacket) { }
        public AddBlock() : base(null) { }

        public override void Handle(Node client)
        {
            if (Block.Height == Blockchain.Current.Blocks.Last.Value.Height + 1)
            {
                if (Block.Transactions.Count == Block.nTx)
                {
                    var merkle = Argon2.ComputeMerkleRoot(Block.Transactions);
                    if (merkle.Equals(Block.MerkleRoot))
                    {
                        string blockHeader = Blockchain.Current.Blocks.Last.Value.PrevHash + Block.Timestamp.ToString() + merkle;
                        var hash = Argon2.ComputeHash(blockHeader);
                        if (hash.Equals(Block.Hash))
                        {
                            foreach (var tx in Block.Transactions)
                            {
                                if (Blockchain.Current.Block.Contains(tx.Hash))
                                {
                                    Blockchain.Current.Block.Remove(tx.Hash);
                                    continue;
                                }

                                SendToFullNode(new TransactionData() { Hash = tx.Hash }.Create());

                                WaitFor(() => TransactionData.RecievedRecords.ContainsKey(tx.Hash));

                                TransactionRecord txRecord = TransactionData.RecievedRecords[tx.Hash];

                                // TxRecord already have compressed images
                                (string frontIDPath, string backIDPath) = txRecord.DecompressID();

                                if (!txRecord.IsVoterVerified(frontIDPath))
                                {
                                    Log.Error($"Couldn't verify a transaction: {tx.Hash}, In Block: {Block.Height}");
                                    Log.Error("Unable to add mined block");
                                    return;
                                }
                            }
                            Blockchain.Current.AddBlock(Block);
                        }
                    }
                }
            }
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<AddBlock>().CopyProperties(this);
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
            Finalize<AddBlock>();
            return Buffer;
        }
    }
}
