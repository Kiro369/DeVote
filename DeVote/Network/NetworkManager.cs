using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network
{
    internal class NetworkManager
    {
        /// <summary>
        /// An instance of our consensus algorithm
        /// </summary>
        public static readonly LRNConsensus Consensus = new LRNConsensus();

        /// <summary>
        /// Connected Nodes
        /// </summary>
        static Dictionary<string, Node> Nodes = new();

        /// <summary>
        /// Connected nodes count
        /// </summary>
        public static int NodesCount { get { return Nodes.Count; } }

        /// <summary>
        /// Object to lock while adjusting the Nodes collection to avoid multi threading issues
        /// </summary>
        static object lockable = new();

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
    }
}
