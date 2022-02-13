using System;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;

namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    public class Block
    {
        [ProtoMember(1)] public int Height { get; set; }
        [ProtoMember(2)] public string PrevHash;
        [ProtoMember(3)] public string Hash { get; set; }
        [ProtoMember(4)] public long Timestamp;
        [ProtoMember(5)] public string Miner;
        [ProtoMember(6)] public List<Transaction> Transactions { get; set; }
        [ProtoMember(7)] public int nTx;
        [ProtoMember(8)] public string MerkleRoot;

        public Block(List<Transaction> transactions)
        {
            this.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            this.Transactions = transactions;
            this.nTx = transactions.Count;
        }
        
        public static byte[] SerializeBlock(Block block)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, block);
                return stream.ToArray();
            }
        }

        public static Block DeserializeBlock(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<Block>(stream);
            }
        }
    }
}