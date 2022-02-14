using System;
using System.Collections.Generic;
using System.IO;
using DeVote.Cryptography;
using LevelDB;

namespace DeVote.Structures
{
    public class BlockChain
    {
        public LinkedList<Block> VChain { set; get; }
        public DB LevelDB;

        public BlockChain()
        {
            VChain = new LinkedList<Block>();
            Block GenesisBlock = CreateGenesisBlock();
            VChain.AddFirst(GenesisBlock);

            // Create a new leveldb
            string dbPath = Directory.GetCurrentDirectory() + "\\dbTest";
            LevelDB = new DB(new Options { CreateIfMissing = true }, dbPath);
            SaveBlock(GenesisBlock);
        }

        private Block CreateGenesisBlock()
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
            Block latestBlock = VChain.Last.Value;
            block.Height = latestBlock.Height + 1;
            block.PrevHash = latestBlock.Hash;
            block.nTx = block.Transactions.Count;
            block.MerkleRoot = Argon2.ComputeMerkleRoot(block.Transactions);
            string blockHeader = block.PrevHash+block.Timestamp.ToString()+block.MerkleRoot;
            Console.WriteLine($"blockHeader {blockHeader}");
            block.Hash = Argon2.ComputeHash(blockHeader);
            VChain.AddLast(block);
        }

        // Save Block into LevelDB as byte array representation of Protobuf Encoding.
        public void SaveBlock(Block block)
        {
            byte[] SerializedBlock = Block.SerializeBlock(block);
            byte[] Height = BitConverter.GetBytes(block.Height);
            this.LevelDB.Put(Height, SerializedBlock);
        }

        // Load Single Block from LevelDB
        public byte[] LoadBlock(int height)
        {
            byte[] HeightByte = BitConverter.GetBytes(height);
            var SerializedBlock = LevelDB.Get(HeightByte);
            if (SerializedBlock != null)
            {
                return SerializedBlock;
            }
            else return new byte[] { };
        }

        // Load Serialized BlockChain
        public List<Byte[]> LoadBlockChain()
        {
            List<Byte[]> SerializedBlockChain = new List<Byte[]>();

            var snapShot = this.LevelDB.CreateSnapshot();
            var readOptions = new ReadOptions { Snapshot = snapShot };

            using (var iterator = this.LevelDB.CreateIterator(readOptions))
            {
                for (iterator.SeekToFirst(); iterator.IsValid(); iterator.Next())
                {
                    byte[] SerializedBlock = iterator.Value();
                    // TODO: create index map
                    SerializedBlockChain.Add(SerializedBlock);
                }
                return SerializedBlockChain;
            }
        }

        // Save Serialized BlockChain
        public void SaveBlockChain(List<Byte[]> BlockchainBytes)
        {
            foreach (byte[] block in BlockchainBytes)
            {
                Block SerializedBlock = Block.DeserializeBlock(block);
                byte[] Height = BitConverter.GetBytes(SerializedBlock.Height);

                // TODO: get height without deserialization
                // var firstBytes = new byte[4];
                // // copy the required number of bytes.
                // Array.Copy(block, 0, firstBytes, 0, firstBytes.Length);
                this.LevelDB.Put(Height,block);
            }
        }
    }
}