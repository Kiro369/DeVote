using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DeVote.Cryptography;
using DeVote.Logging;
using DeVote.Structures;
using Newtonsoft.Json;

namespace DeVote.Network
{
    public class NetworkManager
    {
        /// <summary>
        /// An instance of our consensus algorithm
        /// </summary>
        public static readonly LRNConsensus Consensus = new();

        /// <summary>
        /// Connected Nodes
        /// </summary>
        static readonly Dictionary<string, Node> Nodes = new();

        /// <summary>
        /// Connected nodes count
        /// </summary>
        public static int NodesCount { get { return Nodes.Count; } }

        /// <summary>
        /// Object to lock while adjusting the Nodes collection to avoid multi threading issues
        /// </summary>
        static readonly object lockable = new();

        /// <summary>
        /// Latest block height recieved from LatestHeight packet
        /// </summary>
        public static int LatestBlockHeight = 0;

        /// <summary>
        /// Logger to log info/errors
        /// </summary>
        protected static readonly LogProxy Log = new("PacketProcessor");

        /// <summary>
        /// Broadcast a packet to the whole network
        /// </summary>
        /// <param name="packet">the packet to be broadcasted</param>
        /// <param name="fullNodesOnly">Send to full nodes only</param>
        /// <param name="encrypt">encrypt the packet or not</param>
        public static void Broadcast(byte[] packet, bool fullNodesOnly = false, bool encrypt = true)
        {
            foreach (var node in Nodes.Values)
            {
                if (!fullNodesOnly || node.FullNode)
                    node.Send(packet, encrypt);
            }
        }

        /// <summary>
        /// Send a packet to a full node in the network
        /// </summary>
        /// <param name="packet">the packet to be sent</param>
        /// <param name="encrypt">encrypt the packet or not</param>
        public static void SendToFullNode(byte[] packet, bool encrypt = true)
        {
            var fullNodes = Nodes.Values.Where(node => node.FullNode);
            if (fullNodes.Count() == 0)
            {
                WaitFor(() => (fullNodes = Nodes.Values.Where(node => node.FullNode)).Any());
            }
            fullNodes.First().Send(packet, encrypt);
        }

        /// <summary>
        /// Resets the generated random number for the LRNConsensus
        /// </summary>
        public static void ResetRNConsensus()
        {
            lock (lockable)
            {
                foreach (var node in Nodes.Values)
                {
                    node.ConsensusRN = long.MaxValue;
                }
            }
        }

        /// <summary>
        /// Adds a node to the collection of connected nodes
        /// </summary>
        /// <param name="endPoint">the remote endpoint of the node</param>
        /// <param name="node">the node to be added</param>
        public static void AddNode(string endPoint, Node node)
        {
            lock (lockable)
            {
                Nodes[endPoint] = node;
                if (AES.Key != null)
                    node.Send(new Messages.LRNConsensus() { FullNode = Settings.Current.FullNode, ConsensusRN = long.MaxValue, MachineID = Constants.MachineID}.Create());
            }
        }
        /// <summary>
        /// Removed a node from the collection by its end point
        /// </summary>
        /// <param name="endPoint">the end point of the node to be removed</param>
        public static void RemoveNode(string endPoint)
        {
            lock (lockable)
            {
                if (Nodes.ContainsKey(endPoint))
                    Nodes.Remove(endPoint);
            }
        }

        /// <summary>
        /// Gets all the connected Nodes
        /// </summary>
        /// <returns>A collection of connected Nodes</returns>
        public static IEnumerable<Node> GetNodes()
        {
            lock (lockable)
            {
                return Nodes.Values;
            }
        }

        public static void SendLocation(bool isTest = false)
        {
            string endpoint = $"{Settings.Current.BlockchainExplorerEndPoint}/vms";

            var values = new Dictionary<string, string> { };
            values["id"] = Constants.MachineID;
            values["lat"] = Settings.Current.Latitude.ToString();
            values["lng"] = Settings.Current.Longitude.ToString();
            values["address"] = Settings.Current.Address;
            values["governorate"] = Settings.Current.Governorate;

            // for the sake of testing and avoiding endpoint error.
            if (isTest)
            {
                Random rnd = new();
                values["id"] += rnd.Next(1, 100);
                endpoint = "https://devote-explorer-backend.herokuapp.com/vms";
            }

            using var client = new HttpClient();
            var requestBody = new StringContent(JsonConvert.SerializeObject(values).ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(endpoint, requestBody).Result;
            if (response.IsSuccessStatusCode) Console.WriteLine("Machine's location sent and added successfully");

            else
            {
                Console.WriteLine("Sending machine's location failed or it's already added");
                //Console.WriteLine(response.StatusCode.ToString());
                //string responseString = response.Content.ReadAsStringAsync().Result;
                //JObject responseObj = JObject.Parse(responseString);
                //Console.WriteLine(responseObj.SelectToken("errors[0].detail"));
            }
        }

        public static void SendCandidates(bool isTest = false)
        {
            var list = new List<Dictionary<string, string>>();
            var Candidate1 = new Dictionary<string, string> { };
            Candidate1["id"] = Constants.Candidate1ID;
            Candidate1["name"] = "عبد الفتاح السيسي";
            Candidate1["color"] = "0xff26375f";

            var Candidate2 = new Dictionary<string, string> { };
            Candidate2["id"] = Constants.Candidate2ID;
            Candidate2["name"] = "موسى مصطفى موسى";
            Candidate2["color"] = "0xffd82148";

            list.Add(Candidate1);
            list.Add(Candidate2);

            string endpoint = $"{Settings.Current.BlockchainExplorerEndPoint}/candidates";
            foreach (var pair in list)
            {
                using var client = new HttpClient();
                var requestBody = new StringContent(JsonConvert.SerializeObject(pair).ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(endpoint, requestBody).Result;
                if (response.IsSuccessStatusCode) Console.WriteLine("Candidate is sent and added successfully");
                else Console.WriteLine("Sending Candidate failed or it's already added");
            }
        }

        public static string BroadcastTransaction(string candidateHash, DeVotingApp.IDInfo info, List<string> imagesPaths)
        {
            var txData = new Transaction(Argon2.ComputeHash(info.ID), candidateHash);
            var hash = txData.Hash;
            var imgsData = imagesPaths.Select(imgPath => File.ReadAllBytes(imgPath)).ToArray();
            var compressedData = Misc.ImageProcessor.CompressImages(imgsData);
            var front = Misc.ImageProcessor.Compress(File.ReadAllBytes(info.FrontIDPath));
            var transaction = new Messages.Transaction(null)
            {
                TransactionData = txData,
                TxRecord = new TransactionRecord(Encoding.UTF8.GetBytes(hash), front, new byte[] { }, compressedData)
            };

            var packet = transaction.Create();

            if (!VotedDLT.Current.Contains(txData.Elector))
            {
                VotedDLT.Current.Add(txData.Elector, Constants.MachineID);
                txData.Elector = Constants.MachineID;
                Blockchain.Current.Block.AddTransaction(txData);
                if (Settings.Current.FullNode)
                {
                    TransactionsDLT.Current.AddRecord(transaction.TxRecord);
                }
                Broadcast(packet);
                return hash;
            }

            return string.Empty;
        }

        public static void WaitFor(Func<bool> condition)
        {
            while (!condition())
                Task.Delay(100).Wait();
        }

        public static void Sync()
        {
            var heightRequest = new Messages.LatestHeight() { Type = PacketType.Request }.Create();
            Broadcast(heightRequest);

            WaitFor(() => LatestBlockHeight > 0);

            while (Blockchain.Current.Blocks.Last.Value.Height < LatestBlockHeight)
            {
                var neededHeight = Blockchain.Current.Blocks.Last.Value.Height + 1;
                // get block request (neededHeight); 
                var getBlockRequest = new Messages.GetBlock
                {
                    Type = PacketType.Request,
                    BlockHeight = neededHeight
                };
                SendToFullNode(getBlockRequest.Create());

                WaitFor(() => Messages.GetBlock.RecievedBlocks.ContainsKey(neededHeight));

                var neededBlock = Messages.GetBlock.RecievedBlocks[neededHeight];

                // foreach transaction in that block 
                foreach (Transaction tx in neededBlock.Transactions)
                {
                    SendToFullNode(new Messages.TransactionData() { Hash = tx.Hash }.Create());

                    // get full transaction data TransactionData => verify then add it 
                    WaitFor(() => Messages.TransactionData.RecievedRecords.ContainsKey(tx.Hash));
                    TransactionRecord txRecord = Messages.TransactionData.RecievedRecords[tx.Hash];

                    // TxRecord already have compressed images
                    (string frontIDPath, string backIDPath) = txRecord.DecompressID();

                    if (!txRecord.IsVoterVerified(frontIDPath))
                    {
                        Log.Error($"Couldn't verify a transaction: {tx.Hash}, In Block: {neededHeight}");
                        Log.Error("Unable to SYNC properly");
                        Environment.Exit(1);
                        break;
                    }

                    if (Settings.Current.FullNode)
                    {
                        TransactionsDLT.Current.AddRecord(txRecord);
                    }

                }

                Blockchain.Current.AddBlock(neededBlock);

            }
        }
    }
}
