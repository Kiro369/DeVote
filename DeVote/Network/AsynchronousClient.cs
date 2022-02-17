using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeVote.Network
{
    class AsynchronousClient
    {
        // Packets Queue

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false), receiveDone =
            new ManualResetEvent(false);

        bool Stop = false;
        string NodeHost;
        int NodePort;
        Node Node;
        // The response from the remote device.  
        private static String response = String.Empty;

        /// <summary>
        /// The constructor for the Seeder Client, host and port for the DNS Seeder are required
        /// Note: dnsseeder.ddns.net is a Dynamio DNS I created with NO-IP.
        /// NO-IP is a Free Dynamic DNS and Managed DNS Provider
        /// </summary>
        /// <param name="host">DNS Seeder host that the client will connect to</param>
        /// <param name="port">DNS Seeder port</param>
        public AsynchronousClient(string host, int port = 4269)
        {
            NodeHost = host;
            NodePort = port;
        }

        /// <summary>
        /// Start the Seeder Client
        /// </summary>
        /// <param name="joinSeeder">Set this to true, to add the address to the list in the Seeder</param>
        public void StartClient()
        {
            // Parse string IP to an IPAddress object
            
            IPAddress ipAddress = IPAddress.Parse(NodeHost);

            // Establish the remote endpoint for the socket. 
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, NodePort);

            // Create a TCP/IP socket.  
            Socket client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // Connect to the remote endpoint.  
            client.BeginConnect(remoteEndPoint,
                new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();

            // Create the state object.  
            Node = new Node();
            Node.Socket = client;
            Program.Nodes[remoteEndPoint.ToString()] = Node;

            // Receive the response from the remote device.  
            while (!Stop)
            {
                receiveDone.Reset();
                Receive(client);
                receiveDone.WaitOne();
            }

            // Release the socket.  
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        public void StopClient()
        {
            Stop = true;
        }
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Begin receiving the data from the remote device.  
                client.BeginReceive(Node.buffer, 0, Node.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), Node);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                Node state = (Node)ar.AsyncState;
                Socket client = state.Socket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // Add the packet to the handler to be handled
                    PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(state, state.buffer.Take(bytesRead).ToArray()));

                    Array.Clear(state.buffer, 0, state.buffer.Length);

                    //// Signal that all bytes have been received.  
                    //client.BeginReceive(state.buffer, 0, Node.BufferSize, 0,
                    //    new AsyncCallback(ReceiveCallback), state);

                    receiveDone.Set();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            Send(client, byteData);
        }
        private void Send(Socket client, byte[] byteData)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
