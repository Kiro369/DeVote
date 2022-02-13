using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeVote.Structures;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using DeVote.Network;
using System.Threading.Tasks;
using DeVote.Network.Cryptography;
using DeVote.Extensions;

namespace DeVote
{
    class Program
    {
        public static byte[][] ECDHOperations = new byte[][]
        {
            Encoding.UTF8.GetBytes("DeVoteECDiffieHellmanRequest"),
            Encoding.UTF8.GetBytes("DeVoteECDiffieHellmanInitalResponse"),
            Encoding.UTF8.GetBytes("DeVoteECDiffieHellmanFinalResponse")
        };
        public static Dictionary<string, Node> Nodes = new Dictionary<string, Node>();
        static void Main(string[] args)
        {
            #region Test
            var x = new byte[] { 1, 1 };
            var y = x.Take(5).ToArray();
            //PacketsHandler.Init();

            //AsynchronousServer server = new AsynchronousServer(4269);
            //Task.Factory.StartNew(() => { server.Start(); });
            //Task.Delay(5000).Wait();
            //AsynchronousClient client = new AsynchronousClient("127.0.0.1");
            //Task.Factory.StartNew(() => { client.StartClient(); });
            //Task.Delay(5000).Wait();
            //var serverFromClient = Nodes.FirstOrDefault().Value;
            //var request = ECDHRequestBytes.Concat(ECDH.PublicKey.ToByteArray()).ToArray();
            //serverFromClient.Send(request);
            //Task.Delay(5000).Wait();
            //var pkt = PacketsHandler.Packets.Dequeue();
            //Console.WriteLine(Encoding.UTF8.GetString(pkt));
            //Console.ReadLine();
            return;
            #endregion
            #region Main
            AsynchronousServer server = new AsynchronousServer(4269);
            Task.Factory.StartNew(() => { server.Start(); });

            DNSSeeder.AsynchronousClient seederClient = new DNSSeeder.AsynchronousClient();
            seederClient.StartClient(true);

            if (seederClient.Addresses.Length == 0)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.GenerateKey();
                    aes.GenerateIV();
                    AES.Key = new AESKey()
                    {
                        Key = aes.Key,
                        IV = aes.IV
                    };
                }
            }
            else
            {
                for (var i = 0; i < seederClient.Addresses.Length; i++)
                {
                    var address = seederClient.Addresses[i];
                    Task.Factory.StartNew(() =>
                    {
                        AsynchronousClient client = new AsynchronousClient(address);
                        client.StartClient();
                    });
                    if (i == 0)
                    {
                        while (Nodes.Count == 0) Task.Delay(100);
                        var node = Nodes.FirstOrDefault().Value;
                        var request = ECDHOperations[0].Concat(ECDH.PublicKey.ToByteArray()).ToArray();
                        node.Send(request);
                        while (!PacketsHandler.Packets.Any(y => y.Value.Take(ECDHOperations[1].Length).SequenceEqual(ECDHOperations[1])));
                        var responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(ECDHOperations[1].Length).SequenceEqual(ECDHOperations[1])).Value;
                        var otherPartyPublicKey = responsePacket.Skip(ECDHOperations[1].Length).ToArray();
                        while (!PacketsHandler.Packets.Any(y => y.Value.Take(ECDHOperations[2].Length).SequenceEqual(ECDHOperations[2]))) ;
                        responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(ECDHOperations[2].Length).SequenceEqual(ECDHOperations[2])).Value.Skip(ECDHOperations[2].Length).ToArray();
                        responsePacket.Split(responsePacket.Length - 16, out byte[] encryptedSecretKey, out byte[] IV);
                        var aesKeySerialized = ECDH.Decrypt(encryptedSecretKey, otherPartyPublicKey, IV);
                        var key = ProtoBuf.Serializer.Deserialize<AESKey>(new System.Buffers.ReadOnlySequence<byte>(aesKeySerialized));
                        AES.Key = key;
                    }
                }
            }
            _ = PacketsHandler.Handle();
            #endregion

            #region BlockChain Test
            BlockChain deVOTE = new BlockChain();
            List<Transaction> myTransactions = new List<Transaction>();
            for (int i = 0; i < 2; i++)
            {
                Transaction newTX = new Transaction("elector" + i,"elected" + i);
                myTransactions.Add(newTX);
            }

            // Console.WriteLine(JsonConvert.SerializeObject(myTransactions, Formatting.Indented));

            Block myBlock = new Block(myTransactions);
            myBlock.Miner = "Test33";
            myBlock.Transactions = myTransactions;
            deVOTE.AddBlock(myBlock);

            // Save block into leveldb
            deVOTE.SaveBlock(myBlock);

            var SerializedBlock = Block.SerializeBlock(myBlock);

            Block DeserializedBlock = Block.DeserializeBlock(SerializedBlock);
            Console.WriteLine(JsonConvert.SerializeObject(DeserializedBlock, Formatting.Indented));

            List<Byte[]> SerializedBlockChain = deVOTE.LoadBlockChain();

            Console.WriteLine("SerializedBlockChain");
            Console.WriteLine(JsonConvert.SerializeObject(SerializedBlockChain, Formatting.Indented));


            Console.WriteLine("");
            deVOTE.SaveBlockChain(SerializedBlockChain);

            // Close the connection
            deVOTE.LevelDB.Close();
            #endregion

            #region Prev tests
            //var originalText = "hello bitches";
            //Console.WriteLine("Original text: " + originalText);
            //var originalBytes = Encoding.UTF8.GetBytes(originalText);
            //var encryptedBytes = Network.Cryptography.AES.Encrypt(originalBytes);
            //Console.WriteLine("Encrypted string: " + Encoding.UTF8.GetString(encryptedBytes));
            //var decryptedBytes = Network.Cryptography.AES.Decrypt(encryptedBytes);
            //Console.WriteLine("Decypted string: " + Encoding.UTF8.GetString(decryptedBytes));
            //Console.WriteLine("Original and Decrypted bytes match: " + originalBytes.SequenceEqual(decryptedBytes));
            //Console.ReadLine();
            //// return;
            #endregion
        }
    }
}