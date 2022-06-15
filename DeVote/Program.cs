using DeVote.Cryptography;
using DeVote.Extensions;
using DeVote.Network;
using DeVote.Network.Communication;
using DeVote.Structures;
using DeVote.PyRecognition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
using DeVote.Misc;
using DeVote.Logging;

namespace DeVote
{
    public class Program
    {
        /// <summary>
        /// Log proxy to log events (infos, errors, warnings, etc..)
        /// </summary>
        static readonly LogProxy Log = new("DeVote");

        public static void Main()
        {
            #region Main
            // Start the server first, so anyone can connect after we get added to the seeder
            var server = new Server(4269);
            Log.Info("Starting the server...");
            server.RunServerAsync();

            //Initialize Packet Handler
            Log.Info("Initializing Packet Handler...");
            PacketsHandler.Init();

            //Initialize Recognition modules
            Log.Info("Initializing Recognition modules...");
            _ = Recognition.Current;

            // Starting the seeder client to be able to connect to the network
            DNSSeeder.AsynchronousClient seederClient = new();

            // Get the addresses of all nodes, and add out IP to the seeder
            Log.Info("Fetching DNS Seeder...");
            seederClient.StartClient(server.Port);

            Log.Info("Loading the Blockchain...");
            Blockchain.Current.LoadBlockchain();

            // Check if there is anyone on network, if there is none, we create our own AES Key
            if (seederClient.EndPoints.Count == 0)
            {
                Log.Info("Generating AES Key...");
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
                Log.Info("Starting Packets handler...");
                Task.Factory.StartNew(PacketsHandler.Handle);
            }
            else
            {
                Log.Info("Acquiring AES Key through ECDH (0)...");
                // If there is someone on the network, we connect to them, and we do a Key Exchange with the first one we connect to
                seederClient.EndPoints.ForEach(endPoint => Task.Factory.StartNew(() => { new Node(endPoint).Start(); }));

                // Wait till we connect
                while (NetworkManager.NodesCount == 0) Task.Delay(100);

                // Get the connection
                var node = NetworkManager.GetNodes().FirstOrDefault();

                // Generate request by adding special request bytes to the beginning of the packet (so the other side can Identify the packet), and adding our public key
                var request = Constants.ECDHOperations[0].Concat(ECDH.PublicKey).ToArray();

                // Send the request
                Log.Info("Acquiring AES Key through ECDH (1)...");
                node.Send(request);

                // Wait for an inital response containing other party public key
                while (!PacketsHandler.Packets.Any(y => y.Value.Take(Constants.ECDHOperations[1].Length).SequenceEqual(Constants.ECDHOperations[1]))) ;

                Log.Info("Acquiring AES Key through ECDH (2)...");

                // Get that response
                var responsePacket = PacketsHandler.Packets.First(y => y.Value.Take(Constants.ECDHOperations[1].Length).SequenceEqual(Constants.ECDHOperations[1])).Value;

                // Extract other party public key
                var otherPartyPublicKey = responsePacket.Skip(Constants.ECDHOperations[1].Length).ToArray();

                // Wait for the final response, containing the secret AES Key 
                while (!PacketsHandler.Packets.Any(y => y.Value.Take(Constants.ECDHOperations[2].Length).SequenceEqual(Constants.ECDHOperations[2]))) ;

                Log.Info("Acquiring AES Key through ECDH (3)...");

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

                Log.Info("Acquired the AES Key through ECDH!");

                // Start the packet Handler since we have now our AES Key, we can Decrypt incoming packets from the network
                Log.Info("Starting Packets handler...");
                Task.Factory.StartNew(PacketsHandler.Handle);

                Log.Info("Syncing with other nodes...");
                NetworkManager.Sync();
                Log.Info("Finished Syncing!");

                // Tell the network our MachineID and if we're wether a FullNode or Not
                NetworkManager.Broadcast(new Network.Messages.LRNConsensus() { ConsensusRN = long.MaxValue, FullNode = Settings.Current.FullNode, MachineID = Constants.MachineID }.Create());
            }

            // Console Title adj, helps with debugging, shows connected entpoints
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var lines = NetworkManager.GetNodes().Select(node => node.EndPoint + "=" + node.Connected + "|");
                    Console.Title = "|" + string.Join(Environment.NewLine, lines);
                    await Task.Delay(1000);
                }
            });

            Log.Info("Starting LRN Consensus...");
            // Starting the Lowest Random Number Consensus
            Task.Factory.StartNew(LRNConsensus.Current.Start);

            Log.Info("DeVote is functioning!");

            while (true) // Commands to help with the system
            {
                Console.WriteLine("Write a cmd");
                var input = Console.ReadLine().Split(' ');
                var cmd = input[0];
                var data = input.Length > 1 ? input[1] : string.Empty;
                switch (cmd)
                {
                    case "chat":
                        var x = Blockchain.Current.Block.Transactions;
                        var test = new Network.Messages.Test(null);
                        foreach (var node in NetworkManager.GetNodes())
                        {
                            node.Send(test.Create(data));
                        }
                        break;
                    case "shutdown":
                        server.Stop();
                        Blockchain.Current.SaveBlockchain();
                        Environment.Exit(0);
                        break;
                    case "largepacket":
                        var pkt = new byte[1038476];
                        Constants.FastRandom.NextBytes(pkt);
                        NetworkManager.Broadcast(pkt);
                        break;
                }
            }
            #endregion
        }
    }
}