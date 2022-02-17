using System;
using System.Collections.Generic;
using System.IO;
using DeVote.Cryptography;
using LevelDB;
using ProtoBuf;


namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    public class BlockChain
    {

        [ProtoMember(1)] public LinkedList<Block> Blocks { set; get; }
        public DB LevelDB;

        public BlockChain()
        {
            Blocks = new LinkedList<Block>();
            // Create a new leveldb
            string dbPath = Directory.GetCurrentDirectory() + "\\dbTest";
            LevelDB = new DB(new Options { CreateIfMissing = true }, dbPath);
            //SaveBlock(GenesisBlock);
        }

        public Block CreateGenesisBlock()
        {
            Block GenesisBlock = new Block(new List<Transaction>{
                new Transaction("","")
            });
            // Block GenesisBlock = new Block();
            GenesisBlock.Height = 1;
            GenesisBlock.Miner = "";
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

        // Load BlockChain from levelDB into memory
        public void LoadBlockChain()
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
        // Save Serialized BlockChain
        public void SaveBlockChain()
        {
            foreach (Block block in this.Blocks)
            {
                byte[] SerializedBlock = Block.SerializeBlock(block);
                byte[] Height = BitConverter.GetBytes(block.Height);
                this.LevelDB.Put(Height, SerializedBlock);
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