using System;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using Newtonsoft.Json;

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
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Transactions = transactions;
            nTx = transactions.Count;
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            nTx++;
        }

        // Save Block into LevelDB as byte array representation of Protobuf Encoding.
        public void SaveBlockAsByteArray(LevelDB.DB levelDB)
        {
            byte[] SerializedBlock = SerializeBlock(this);
            byte[] height = BitConverter.GetBytes(Height);
            levelDB.Put(height, SerializedBlock);
        }

        // Save Block into LevelDB as string -json- representation.
        public void SaveBlockAsString(LevelDB.DB levelDB)
        {
            string StringifiedBlock = JsonConvert.SerializeObject(this);
            levelDB.Put(Height.ToString(), StringifiedBlock);
        }

        // Load Single Block saved as byte array representation from LevelDB.
        public static Block LoadProtobuffedBlock(int height,LevelDB.DB levelDB)
        {
            byte[] HeightByte = BitConverter.GetBytes(height);
            var SerializedBlock = levelDB.Get(HeightByte);
            if (SerializedBlock != null)
            {
                return DeserializeBlock(SerializedBlock) ;
            }
            else return new Block(new List<Transaction>());
        }
        // Load Single Block saved as string representation from LevelDB.
        public static Block LoadStringifiedBlock(int height, LevelDB.DB levelDB)
        {
            String StringifiedBlock = levelDB.Get(height.ToString());
            if (StringifiedBlock != null)
            {
                Block LoadedBlock = JsonConvert.DeserializeObject<Block>(StringifiedBlock);
                return LoadedBlock;
            }
            else return new Block(new List<Transaction>());
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

        public static string GetBlockProto()
        {
                return Serializer.GetProto<Block>();
        }
    }
}