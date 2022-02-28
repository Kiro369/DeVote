﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeVote.Structures;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using DeVote.Network;
using System.Threading.Tasks;
using DeVote.Cryptography;
using DeVote.Extensions;

namespace DeVote
{
    class Program
    {
        /// <summary>
        /// Connected Nodes
        /// </summary>
        public static Dictionary<string, Node> Nodes = new Dictionary<string, Node>();
        static void Main(string[] args)
        {
            #region Test
            //FindPort2();
            ////ECDH.test();
            ////return;
            ////var x = new byte[] { 1, 1 };
            ////var y = x.Take(5).ToArray();
            ////PacketsHandler.Init();
            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ////AsynchronousServer server = new AsynchronousServer(4269);
            ////Task.Factory.StartNew(() => { server.Start(); });
            ////Task.Delay(5000).Wait();
            ////AsynchronousClient client = new AsynchronousClient("127.0.0.1");
            ////Task.Factory.StartNew(() => { client.StartClient(); });
            ////Task.Delay(5000).Wait();
            ////var serverFromClient = Nodes.FirstOrDefault().Value;
            ////var request = ECDHRequestBytes.Concat(ECDH.PublicKey.ToByteArray()).ToArray();
            ////serverFromClient.Send(request);
            ////Task.Delay(5000).Wait();
            ////var pkt = PacketsHandler.Packets.Dequeue();
            ////Console.WriteLine(Encoding.UTF8.GetString(pkt));
            ////Console.ReadLine();
            //return;
            #endregion
            #region Main
            // Starting the seeder client to be able to connect to the network
            DNSSeeder.AsynchronousClient seederClient = new DNSSeeder.AsynchronousClient();

            // Start the server first, so anyone can connect after we get added to the seeder
            AsynchronousServer server = new AsynchronousServer(4269, seederClient);
            Task.Factory.StartNew(() => { server.Start(); });

            PacketsHandler.Init();

            // Get the addresses of all nodes, and add out IP to the seeder
            seederClient.StartClient(server.Port);

            // Check if there is anyone on network, if there is none, we create our own AES Key
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
                // Start the packet Handler since we have our AES Key, we can Decrypt incoming packets from the network
                Task.Factory.StartNew(() => PacketsHandler.Handle());
            }
            else
            {
                // If there is someone on the network, we connect to them, and we do a Key Exchange with the first one we connect to
                for (var i = 0; i < seederClient.Addresses.Length; i++)
                {
                    var spltr = seederClient.Addresses[i].Split(":");
                    var address = spltr[0];
                    var port = int.Parse(spltr[1]);
                    Task.Factory.StartNew(() => // Connect to another node, in another thread
                    {
                        AsynchronousClient client = new AsynchronousClient(address, port);
                        client.StartClient();
                    });
                    if (i == 0) // If it's the first node we connect to, we do Elliptic Curve Diffie Hellman Key Exchange with them
                    {
                        // Wait till we connect
                        while (Nodes.Count == 0) Task.Delay(100);

                        // Get the connection
                        var node = Nodes.FirstOrDefault().Value;

                        // Generate request by adding special request bytes to the beginning of the packet (so the other side can Identify the packet), and adding our public key
                        var request = Constants.ECDHOperations[0].Concat(ECDH.PublicKey.ToByteArray()).ToArray();

                        // Send the request
                        node.Send(request);

                        // Wait for an inital response containing other party public key
                        while (!PacketsHandler.Packets.Any(y => y.Value.Take(Constants.ECDHOperations[1].Length).SequenceEqual(Constants.ECDHOperations[1]))) ;

                        // Get that response
                        var responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(Constants.ECDHOperations[1].Length).SequenceEqual(Constants.ECDHOperations[1])).Value;

                        // Extract other party public key
                        var otherPartyPublicKey = responsePacket.Skip(Constants.ECDHOperations[1].Length).ToArray();

                        // Wait for the final response, containing the secret AES Key 
                        while (!PacketsHandler.Packets.Any(y => y.Value.Take(Constants.ECDHOperations[2].Length).SequenceEqual(Constants.ECDHOperations[2]))) ;

                        // Get that response
                        responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(Constants.ECDHOperations[2].Length).SequenceEqual(Constants.ECDHOperations[2])).Value.Skip(Constants.ECDHOperations[2].Length).ToArray();

                        // Split the encrypted and serialized Key from ECDH AES IV 
                        responsePacket.Split(responsePacket.Length - 16, out byte[] encryptedSecretKey, out byte[] IV);

                        // Decrypt the Key
                        var aesKeySerialized = ECDH.Decrypt(encryptedSecretKey, otherPartyPublicKey, IV);

                        // Finally Deserialize the AES Key
                        var key = ProtoBuf.Serializer.Deserialize<AESKey>(new System.Buffers.ReadOnlySequence<byte>(aesKeySerialized));

                        // Set the Key, so we can use it
                        AES.Key = key;

                        // Start the packet Handler since we have now our AES Key, we can Decrypt incoming packets from the network
                        Task.Factory.StartNew(() => PacketsHandler.Handle());
                    }
                }
            }
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var lines = Nodes.Values.Select(node => node.Address + "=" + node.Socket.Connected + "|");
                    Console.Title = "|" + string.Join(Environment.NewLine, lines);
                    await Task.Delay(1000);
                }
            });
            while (true)
            {
                Console.WriteLine("Write a msg");
                var msg = Console.ReadLine();
                var test = Network.Messages.Test.Create(msg);
                var etest = AES.Encrypt(test);
                foreach (var node in Nodes.Values)
                {
                    node.Send(etest);
                }
            }
            #endregion

            #region BlockChain Test
            return;
            BlockChain deVOTE = new BlockChain();
           
            // load form leveldb 
            deVOTE.LoadBlockChain();

            if(deVOTE.Blocks.Count==0){
            Block GenesisBlock = deVOTE.CreateGenesisBlock();
            deVOTE.Blocks.AddFirst(GenesisBlock);
            }
           

            Console.WriteLine("current blocks");
            Console.WriteLine(JsonConvert.SerializeObject(deVOTE.Blocks, Formatting.Indented));


            List<Transaction> myTransactions = new List<Transaction>();
            for (int i = 0; i < 2; i++)
            {
                Transaction newTX = new Transaction("elector" + i, "elected" + i);
                myTransactions.Add(newTX);
            }

            // Console.WriteLine(JsonConvert.SerializeObject(myTransactions, Formatting.Indented));

            Block myBlock = new Block(myTransactions);
            myBlock.Miner = "Test33";
            myBlock.Transactions = myTransactions;

            // add new block to current blockchain
            deVOTE.AddBlock(myBlock);

            // Save block into leveldb
            deVOTE.SaveBlockChain();

            Console.WriteLine("new blocks");
            Console.WriteLine(JsonConvert.SerializeObject(deVOTE.Blocks, Formatting.Indented));


            //load a block
            Block TargetBlock = Block.Load(2,deVOTE.LevelDB);
            Console.WriteLine("TargetBlock");
            Console.WriteLine(JsonConvert.SerializeObject(TargetBlock, Formatting.Indented));

            // load null block 
            Block NullBlock = Block.Load(600,deVOTE.LevelDB);
            Console.WriteLine("NullBlock");
            Console.WriteLine(JsonConvert.SerializeObject(NullBlock, Formatting.Indented));

            // var SerializedBlockchain = deVOTE.SerializeBlockchain();
            // Console.WriteLine("SerializedBlockChain", SerializedBlockchain);

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