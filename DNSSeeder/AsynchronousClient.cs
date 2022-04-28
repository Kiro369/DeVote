using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DNSSeeder
{
    public class AsynchronousClient
    {
        public List<string> EndPoints;
        // ManualResetEvent instances signal completion.  
        private static readonly ManualResetEvent connectDone = 
            new(false), sendDone = 
            new(false), receiveDone = 
            new(false);
        readonly string SeederHost;
        readonly int SeederPort;

        // The response from the remote device.  
        private static string response = string.Empty;

        /// <summary>
        /// The constructor for the Seeder Client, host and port for the DNS Seeder are required
        /// Note: dnsseeder.ddns.net is a Dynamio DNS I created with NO-IP.
        /// NO-IP is a Free Dynamic DNS and Managed DNS Provider
        /// </summary>
        /// <param name="host">DNS Seeder host that the client will connect to</param>
        /// <param name="port">DNS Seeder port</param>
        public AsynchronousClient(string host = "dnsseeder.ddns.net", int port = 6942)
        {
            EndPoints = new List<string>();
            SeederHost = host;
            SeederPort = port;
        }

        /// <summary>
        /// Start the Seeder Client
        /// </summary>
        /// <param name="port">Set this to anything, to add the address to the list in the Seeder</param>
        public void StartClient(int port = 0)
        {
            var externalIP = new HttpClient().GetStringAsync("https://api.ipify.org").Result;
            connectDone.Reset(); sendDone.Reset(); receiveDone.Reset();

            //Resolving the DNS Seeder host to get the acutal IP of our Seeder. 
            if (!IPAddress.TryParse(SeederHost, out IPAddress ipAddress))
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(SeederHost);
                ipAddress = ipHostInfo.AddressList[0];
            }


            // Establish the remote endpoint for the socket. IPAddress.Parse("127.0.0.1")
            IPEndPoint remoteEndPoint = new(ipAddress.ToString().Equals(externalIP) ? IPAddress.Parse("127.0.0.1") : ipAddress, SeederPort);

            // Create a TCP/IP socket.  
            Socket client = new(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            client.BeginConnect(remoteEndPoint,
                new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();

            // Send test data to the remote device.  
            Send(client, BitConverter.GetBytes(port));
            sendDone.WaitOne();

            // Receive the response from the remote device.  
            Receive(client);
            receiveDone.WaitOne();

            EndPoints = string.IsNullOrEmpty(response) ? new List<string>() : response.Split(Environment.NewLine).ToList();
            // Write the response to the console.  
            Console.WriteLine("Addresses received : {0}", EndPoints.Count);

            // Release the socket.  
            client.Shutdown(SocketShutdown.Both);
            client.Close();
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

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private static void Send(Socket client, byte[] byteData)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
