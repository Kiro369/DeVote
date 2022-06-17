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
                    if (client.MachineID.Equals(Block.Miner))
                    {
                        var merkle = Argon2.ComputeMerkleRoot(Block.Transactions);
                        if (merkle.Equals(Block.MerkleRoot))
                        {
                            string blockHeader = Blockchain.Current.Blocks.Last.Value.Hash + Block.Timestamp.ToString() + merkle;
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
                            else
                                Log.Error($"Couldn't generate the same Block hash. Recieved: {Block.Hash}, Generated: {hash}");
                        }
                        else
                            Log.Error($"Couldn't generate the same MerkleRoot. Recieved: {Block.MerkleRoot}, Generated: {merkle}");
                    }
                    else
                        Log.Error($"Couldn't add block: {Block.Height}, Miner: {client.MachineID}, while Choosen is: {Network.LRNConsensus.Current.Choosen}");
                }
                else
                    Log.Error($"Couldn't add block: {Block.Height}, due to wrong number of transactions!");
            }
            else
            {
                Log.Warn("Failed to Add Block, Syncing with other nodes instead...");
                Sync();
                if (Block.Height == Blockchain.Current.Blocks.Last.Value.Height + 1)
                    Handle(client);
                else if (Block.Height == Blockchain.Current.Blocks.Last.Value.Height)
                    Log.Info("Sync succeed, node is up to date!");
            }
        }

        public override bool Read(Node client)
        {
            try
            {
                Deserialize<AddBlock>().CopyProperties(this);
                if (Block != null && Block.Transactions == null)
                    Block.Transactions = new();
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
