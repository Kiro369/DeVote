using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeVote.Cryptography;
using LevelDB;
using ProtoBuf;


namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    public class Blockchain
    {
        public static readonly Blockchain Current = new();
        [ProtoMember(1)] public LinkedList<Block> Blocks { set; get; }
        public DB LevelDB;
        public Block Block;
        public Blockchain()
        {
            Blocks = new LinkedList<Block>();
            Block = new Block();

            // Create a new leveldb
            LevelDB = new DB(new Options { CreateIfMissing = true, CompressionLevel = CompressionLevel.NoCompression }, Settings.Current.BlockchainPath);
        }

        public static Block GenesisBlock
        {
            get
            {
                var genesisBlock = new Block(new List<Transaction>{
                new Transaction("Ahmed","Mahmoud")
            })
                {
                    Height = 1,
                    Miner = "DeVote",
                    PrevHash = "0000000000000000000000000000000000000000000000000000000000000000"
                };
                genesisBlock.MerkleRoot = Argon2.ComputeMerkleRoot(genesisBlock.Transactions);
                string blockHeader = genesisBlock.PrevHash + "Kiro" + genesisBlock.MerkleRoot;
                genesisBlock.Hash = Argon2.ComputeHash(blockHeader);
                return genesisBlock;
            }
        }

        public void AddBlock(Block block)
        {
            block.nTx = block.Transactions.Count;
            block.MerkleRoot = Argon2.ComputeMerkleRoot(block.Transactions);
            string blockHeader = block.PrevHash + block.Timestamp.ToString() + block.MerkleRoot;
            Console.WriteLine($"blockHeader {blockHeader}");
            block.Hash = Argon2.ComputeHash(blockHeader);
            Blocks.AddLast(block);
            SaveBlock(block);
            SaveBlk(block);
        }

       public void SaveBlk(Block blk)
        {
            if (!Directory.Exists(Settings.Current.BlocksPath))
                Directory.CreateDirectory(Settings.Current.BlocksPath);

            var blkFile = $"{Settings.Current.BlocksPath}\\{blk.Height}.blk";
            if (File.Exists(blkFile))
                return;
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, blk);
            var srlzed = stream.ToArray();
            File.WriteAllBytes(blkFile, srlzed);
        }

        public Block GetBlock(int height)
        {
            return Blocks.First(blk => blk.Height == height);
        }

        // Load Blockchain saved in byte array representation from levelDB into memory
        public void LoadBlockchain()
        {
            var snapShot = this.LevelDB.CreateSnapshot();
            var readOptions = new ReadOptions { Snapshot = snapShot };

            using var iterator = this.LevelDB.CreateIterator(readOptions);
            for (iterator.SeekToFirst(); iterator.IsValid(); iterator.Next())
            {
                byte[] SerializedBlock = iterator.Value();
                var block = Block.DeserializeBlock(SerializedBlock);
                this.Blocks.AddLast(block);
            }

            if (Blocks.Count == 0)
            {
                var genesis = GenesisBlock;
                Blocks.AddFirst(genesis);
                SaveBlk(genesis);
            }

        }

        // Save Blockchain as byte array representation of Protobuf Encoding.
        public void SaveBlockchain()
        {
            foreach (Block block in this.Blocks)
            {
                SaveBlock(block);
            }
        }

        public void SaveBlock(Block block)
        {
            byte[] SerializedBlock = Block.SerializeBlock(block);
            byte[] Height = BitConverter.GetBytes(block.Height);
            LevelDB.Put(Height, SerializedBlock);
            SaveBlk(block);
        }

        public byte[] SerializeBlockchain()
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, this);
            return stream.ToArray();
        }
    }
}
