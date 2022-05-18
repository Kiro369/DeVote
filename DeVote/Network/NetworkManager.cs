using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DeVote.Structures;
using Newtonsoft.Json;

namespace DeVote.Network
{
    internal class NetworkManager
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
        /// Broadcast a packet to the whole network
        /// </summary>
        /// <param name="packet">the packet to be broadcasted</param>
        /// <param name="encrypt">encrypt the packet or not</param>
        public static void Broadcast(byte[] packet, bool encrypt = true)
        {
            foreach (var node in Nodes.Values)
            {
                node.Send(packet, encrypt);
            }
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
            string endpoint = $"http://localhost:{Settings.Current.BlockchainExplorerPort}/vms";

            var values = new Dictionary<string, string> { };
            values["id"] = Constants.MachineID;
            values["lat"] = Settings.Current.Latitude.ToString();
            values["lng"] = Settings.Current.Longitude.ToString();

            // for the sake of testing and avoiding endpoint error.
            if (isTest)
            {
                Random rnd = new();
                values["id"] += rnd.Next(1, 100);
                endpoint = "https://devote-explorer-backend.herokuapp.com/vms";
            }

            using (var client = new HttpClient())
            {
                var requestBody = new StringContent(JsonConvert.SerializeObject(values).ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(endpoint, requestBody).Result;
                if (response.IsSuccessStatusCode) Console.WriteLine("Machine's location sent and added successfully");

                else
                {
                    Console.WriteLine("Sending machine's location failed");
                    Console.WriteLine(response.StatusCode.ToString());
                    //string responseString = response.Content.ReadAsStringAsync().Result;
                    //JObject responseObj = JObject.Parse(responseString);
                    //Console.WriteLine(responseObj.SelectToken("errors[0].detail"));
                }
            }
        }

        public static void Sync()
        {
            var heightRequest = new Messages.LatestHeight() { Type = PacketType.Request }.Create();
            Broadcast(heightRequest);

            while (LatestBlockHeight == 0)
                Task.Delay(100).Wait();

            while (Blockchain.Current.Blocks.Last.Value.Height < LatestBlockHeight)
            {
                var block = new Block(new List<Transaction>());
                var neededHeight = Blockchain.Current.Blocks.Last.Value.Height + 1;
                // get block request (neededHeight); 
                var getBlockRequest = new Messages.GetBlock() { Type = PacketType.Request };
                getBlockRequest.BlockHeight = neededHeight;
                getBlockRequest.Create();

                // get full node 
                // send

                // no clue ?? maybe check for getBlockRequest.Block??
                while (getBlockRequest.Block == null) { }

                // foreach transaction in that block 
                foreach (Transaction tx in getBlockRequest.Block.Transactions)
                {
                    // get full transaction data TransactionData => verify then add it 
                    byte[] TxRecordBytes = TransactionsDLT.getTxRecord(Encoding.UTF8.GetBytes(tx.Hash));
                    TransactionRecord TxRecord = TransactionRecord.Deserialize(TxRecordBytes); ;
                    
                    // TxRecord already have compressed images
                    (string frontIDPath,string backIDPath) = TxRecord.DecompressID("jpg");

                    // it will be good if we don't decompress and just pass byte[] to Recognition module.
                    bool isTxRecordVerified = TxRecord.IsVoterVerified(frontIDPath);

                    //block.AddTransaction(transaction)
                    if (isTxRecordVerified) block.AddTransaction(tx);
                }


            }
        }
    }
}
