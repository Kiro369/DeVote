using System;
using System.Collections.Generic;
using System.IO;
using DeVote.Cryptography;
using LevelDB;
using Newtonsoft.Json;
using ProtoBuf;


namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    public class Blockchain
    {
        public static readonly Blockchain Current = new Blockchain();
        [ProtoMember(1)] public LinkedList<Block> Blocks { set; get; }
        public DB LevelDB;
        public Block Block;
        public Blockchain()
        {
            Blocks = new LinkedList<Block>();
            // Create a new leveldb
            LevelDB = new DB(new Options { CreateIfMissing = true }, Constants.BlockchainPath);
            //SaveBlock(GenesisBlock);
        }

        public Block CreateGenesisBlock()
        {
            Block GenesisBlock = new Block(new List<Transaction>{
                new Transaction("test","test")
            });
            // Block GenesisBlock = new Block();
            GenesisBlock.Height = 1;
            GenesisBlock.Miner = "deVote";
            GenesisBlock.Hash = Argon2.ComputeHash(GenesisBlock.Timestamp.ToString());
            return GenesisBlock;
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = Blocks.Last.Value;
            block.Height = latestBlock.Height + 1;
            block.PrevHash = latestBlock.Hash;
            block.nTx = block.Transactions.Count;
            block.MerkleRoot = Argon2.ComputeMerkleRoot(block.Transactions);
            string blockHeader = block.PrevHash + block.Timestamp.ToString() + block.MerkleRoot;
            Console.WriteLine($"blockHeader {blockHeader}");
            block.Hash = Argon2.ComputeHash(blockHeader);
            this.Blocks.AddLast(block);
        }

        // Load Blockchain saved in byte array representation from levelDB into memory
        public void LoadProtobuffedBlockchain()
        {
            var snapShot = this.LevelDB.CreateSnapshot();
            var readOptions = new ReadOptions { Snapshot = snapShot };

            using (var iterator = this.LevelDB.CreateIterator(readOptions))
            {
                for (iterator.SeekToFirst(); iterator.IsValid(); iterator.Next())
                {
                    byte[] SerializedBlock = iterator.Value();
                    var block = Block.DeserializeBlock(SerializedBlock);
                    this.Blocks.AddLast(block);
                }
            }
        }

        // Load Blockchain saved in string representation from levelDB into memory
        public void LoadStringifiedBlockchain()
        {
            var snapShot = this.LevelDB.CreateSnapshot();
            var readOptions = new ReadOptions { Snapshot = snapShot };

            using (var iterator = this.LevelDB.CreateIterator(readOptions))
            {
                for (iterator.SeekToFirst(); iterator.IsValid(); iterator.Next())
                {
                    string StringifiedBlock = iterator.ValueAsString();
                    Block block = JsonConvert.DeserializeObject<Block>(StringifiedBlock);
                    this.Blocks.AddLast(block);
                }
            }
        }

        // Save Blockchain as byte array representation of Protobuf Encoding.
        public void SaveBlockchainAsByteArray()
        {
            foreach (Block block in this.Blocks)
            {
                byte[] SerializedBlock = Block.SerializeBlock(block);
                byte[] Height = BitConverter.GetBytes(block.Height);
                this.LevelDB.Put(Height, SerializedBlock);
            }
        }

        // Save Blockchain as string representation.
        public void SaveBlockchainAsString()
        {
            foreach (Block block in this.Blocks)
            {
                string StringifiedBlock = JsonConvert.SerializeObject(block);
                this.LevelDB.Put(block.Height.ToString(), StringifiedBlock);
            }
        }

        public byte[] SerializeBlockchain()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                return stream.ToArray();
            }
        }
    }
}
