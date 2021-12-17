using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
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
            Block GenesisBlock = new Block(DateTime.Now, null, null);
            VChain.AddFirst(GenesisBlock);

            string dbPath = Directory.GetCurrentDirectory()+"\\dbTest";

            // Create a new leveldb
            LevelDB = new DB(new Options { CreateIfMissing = true }, dbPath);
            LevelDB.Put(GenesisBlock.Height.ToString(), JsonConvert.SerializeObject(GenesisBlock));
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = VChain.Last.Value;
            block.Height = latestBlock.Height + 1;
            block.PrevHash = latestBlock.Hash;
            block.Hash = block.ComputeHash();
            VChain.AddLast(block);
        }

        // Save block into leveldb
        public void SaveBlock(Block block)
        {
            // Put each block as key-value pair where key is the position (height) of the block
            LevelDB.Put(block.Height.ToString(), JsonConvert.SerializeObject(block));
        }
    }
}
