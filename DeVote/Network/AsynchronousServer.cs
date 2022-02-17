using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DeVote.Network
{
    class AsynchronousServer
    {
        // Thread signal.  
        public ManualResetEvent allDone = new ManualResetEvent(false);
        // The port our DNS Seeder gonna be listening to
        public readonly int Port;
        /// <summary>
        /// Construct an Asynchronous Server
        /// </summary>
        /// <param name="port">Desired port</param>
        /// <param name="f">Find another port if the desired is unavailable</param>
        public AsynchronousServer(int port, bool f)
        {
            Port = f ? FindPort(port) : port;
        }
        public void Start()
        {
            // Establish the local endpoint for the socket.  
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(int.MaxValue);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }
        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            Node state = new Node();
            state.Socket = handler;
            var endPoint = ((IPEndPoint)handler.RemoteEndPoint).ToString();
            Program.Nodes[endPoint] = state;

            try
            {
                handler.BeginReceive(state.buffer, 0, Node.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    Console.WriteLine(endPoint + " forcibly disconnected");
                }
                else throw e;
            }
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            Node state = (Node)ar.AsyncState;
            Socket handler = state.Socket;

            // Getting the Address of the Remote end point
            var Address = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // Add the packet to the handler to be handled
                PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(state, state.buffer.Take(bytesRead).ToArray()));

                Array.Clear(state.buffer, 0, state.buffer.Length);

                // Keep recieving.  
                try
                {
                    handler.BeginReceive(state.buffer, 0, Node.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == 10054)
                    {
                        Console.WriteLine(Address + " forcibly disconnected");
                    }
                    else throw e;
                }
            }
        }
        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        int FindPort(int basePort)
        {
            var ip = GetPublicIP();
            while (true)
            {
                using (var tcpClient = new TcpClient())
                {
                    try
                    {
                        tcpClient.Connect(ip, basePort);
                        basePort++;
                    }
                    catch (Exception)
                    {
                        return basePort;
                    }
                }
            }
        }
        string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org/";
            var req = WebRequest.Create(url);
            var resp = req.GetResponse();
            var sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
    }
}
