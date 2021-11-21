using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace DeVote.Structures
{
    public class BlockChain
    {
        public LinkedList<Block> VChain { set; get; }
        public LevelDb levelDb;

        public BlockChain()
        {
            VChain = new LinkedList<Block>();
            Block GenesisBlock = new Block(DateTime.Now, null, null);
            VChain.AddFirst(GenesisBlock);

            string parent = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string dbPath = parent+"\\dbTest";

            // Create a new leveldb
            LevelDb myLevelDb = new LevelDb(dbPath);
            this.levelDb = myLevelDb;
            this.levelDb.db.Put(GenesisBlock.Height.ToString(), JsonConvert.SerializeObject(GenesisBlock));
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
        public void saveBlock(Block block)
        {
            // Put each block as key-value pair where key is the position (height) of the block
            this.levelDb.db.Put(block.Height.ToString(), JsonConvert.SerializeObject(block));
        }
    }
}
