using System;
using System.Collections.Generic;

namespace DeVote
{
    public class BlockChain
    {
        public List<Block> VChain { set; get; }

        public BlockChain()
        {
            VChain = new List<Block>();
            VChain.Add(new Block(DateTime.UtcNow.ToString("d"),null, "{}"));
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = VChain[VChain.Count - 1];
            block.Height = latestBlock.Height + 1;
            block.PrevHash = latestBlock.Hash;
            block.Hash = block.ComputeHash();
            VChain.Add(block);
        }
    }
}
