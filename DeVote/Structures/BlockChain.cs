using System;
using System.Collections.Generic;

namespace DeVote.Structures
{
    public class BlockChain
    {
        public LinkedList<Block> VChain { set; get; }

        public BlockChain()
        {
            VChain = new LinkedList<Block>();
            Block GenesisBlock = new Block(DateTime.Now,null, null);
            VChain.AddFirst(GenesisBlock);
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = VChain.Last.Value;
            block.Height = latestBlock.Height + 1;
            block.PrevHash = latestBlock.Hash;
            block.Hash = block.ComputeHash();
            VChain.AddLast(block);
        }
    }
}
