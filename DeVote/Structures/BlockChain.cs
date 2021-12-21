using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using LevelDB;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

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
            Block GenesisBlock = new Block(DateTime.UtcNow, "", new List<Transaction>{
                new Transaction(0,DateTime.UtcNow,"","")
            });
            GenesisBlock.Height = 1;
            GenesisBlock.nTx = 0;
            GenesisBlock.Miner = "";
            return GenesisBlock;
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = VChain.Last.Value;
            block.Height = latestBlock.Height + 1;
            block.PrevHash = latestBlock.Hash;
            block.Hash = block.ComputeHash();
            block.nTx = block.Transactions.Count;
            VChain.AddLast(block);
        }

        // Save Block into LevelDB in Base64 Representation of Protobuf Encoding.
        public void SaveBlock(Block block)
        {
            // Convert Block Object to a BlockBuf Message.
            BlockBuf blockBuf = new BlockBuf
            {
                Hash = block.Hash,
                Height = block.Height,
                PrevHash = block.PrevHash,
                Timestamp = Timestamp.FromDateTime(block.Timestamp),
                NTx = block.nTx
            };

            foreach (Transaction transaction in block.Transactions)
            {
                var TxJSON = JsonConvert.SerializeObject(transaction);
                // Parses a Transaction Message from the given JSON.
                BlockBuf.Types.Transaction TxBuf = BlockBuf.Types.Transaction.Parser.ParseJson(TxJSON);
                blockBuf.Transactions.Add(TxBuf);
            }

            // Converts BlockBuf into a byte string in protobuf encoding.
            ByteString BlockBufByteString = blockBuf.ToByteString();

            // Save Each BlockBuf into LevelDB in base64 representation where key is Height.
            this.LevelDB.Put(block.Height.ToString(), BlockBufByteString.ToBase64());
        }

        // Load Single Block from LevelDB
        public byte[] LoadBlock(string height)
        {
            var BlockBase64String = LevelDB.Get(height);
            if (BlockBase64String != null)
            {
                // Write the entire byte array to the provided stream
                // ByteString.FromBase64(BlockBase64String).WriteTo(stream);

                byte[] BlockByteArray = ByteString.FromBase64(BlockBase64String).ToByteArray();
                return BlockByteArray;
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
                    // Constructs a ByteString from Base64 Encoded String.
                    ByteString BlockByteString = ByteString.FromBase64(iterator.ValueAsString());

                    // Parses a BlockBuf from the given byte string.
                    BlockBuf blockBuf = BlockBuf.Parser.ParseFrom(BlockByteString);

                    // Converts Each block into a byte array in protobuf encoding.
                    byte[] SerializedBlock = MessageExtensions.ToByteArray(blockBuf);

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
                BlockBuf DeserializedBlock = BlockBuf.Parser.ParseFrom(block);

                // Converts BlockBuf into a byte string in protobuf encoding.
                ByteString BlockBufByteString = DeserializedBlock.ToByteString();

                // Save Each BlockBuf into LevelDB in base64 representation where key is Height.
                this.LevelDB.Put(DeserializedBlock.Height.ToString(), BlockBufByteString.ToBase64());
                Console.WriteLine("DeserializedBlock {0}", DeserializedBlock.ToString());
            }
        }
    }
}
