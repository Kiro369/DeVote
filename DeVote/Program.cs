using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeVote.Structures;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using DeVote.Network;
using System.Threading.Tasks;

namespace DeVote
{
    class Program
    {
        public static Dictionary<string, Node> Nodes = new Dictionary<string, Node>();
        static void Main(string[] args)
        {

            #region Test
            AsynchronousServer server = new AsynchronousServer(4269);
            Task.Factory.StartNew(() => { server.Start(); });
            Task.Delay(5000).Wait();
            AsynchronousClient client = new AsynchronousClient("127.0.0.1");
            Task.Factory.StartNew(() => { client.StartClient(); });
            Task.Delay(5000).Wait();
            var serverFromClient = Nodes.FirstOrDefault().Value;
            serverFromClient.Send("Hellooooooo");
            Task.Delay(5000).Wait();
            var pkt = PacketsHandler.Packets.Dequeue();
            Console.WriteLine(Encoding.UTF8.GetString(pkt));
            Console.ReadLine();
            return;
            #endregion
            #region Main
            DNSSeeder.AsynchronousClient seederClient = new DNSSeeder.AsynchronousClient();
            //seederClient.StartClient(true);

            if (seederClient.Addresses.Length == 0)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.GenerateKey();
                    aes.GenerateIV();
                    Network.Cryptography.AES.Key = new Network.Cryptography.AESKey()
                    {
                        Key = aes.Key,
                        IV = aes.IV
                    };
                }
            }
            else
            {
                foreach (var address in seederClient.Addresses)
                {
                    Task.Factory.StartNew(() =>
                    {
                        AsynchronousClient client = new AsynchronousClient(address);
                        client.StartClient();
                    });
                }
                while (Nodes.Count == 0) Task.Delay(100);
                var node = Nodes.FirstOrDefault().Value;
                //node.
            }
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

            //Console.WriteLine("Hello World!");

            //BlockChain deVOTE = new BlockChain();
            //List<Transaction> myTransactions = new List<Transaction>();
            //for (int i = 0; i < 10; i++)
            //{
            //    myTransactions.Add(new Transaction(i, DateTime.UtcNow, "elector" + i, "elected" + i));
            //}

            //Block myBlock = new Block(DateTime.UtcNow, null, myTransactions);
            //deVOTE.AddBlock(myBlock);

            //// Save block into leveldb
            //deVOTE.SaveBlock(myBlock);

            //// byte[] TargetBlock1 = deVOTE.LoadBlock("1");
            //// Console.WriteLine("TargetBlock {0}",Encoding.UTF8.GetString(TargetBlock1));

            //List<Byte[]> SerializedBlockChain = deVOTE.LoadBlockChain();
            //Console.WriteLine("SerializedBlockChain");
            //Console.WriteLine(JsonConvert.SerializeObject(SerializedBlockChain, Formatting.Indented));


            //Console.WriteLine("");
            //deVOTE.SaveBlockChain(SerializedBlockChain);

            //// Close the connection
            //deVOTE.LevelDB.Close();
            #endregion
        }
    }
}