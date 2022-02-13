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
        Dictionary<string, Node> Nodes = new Dictionary<string, Node>();
        // Thread signal.  
        public ManualResetEvent allDone = new ManualResetEvent(false);
        // The port our DNS Seeder gonna be listening to
        int Port;

        public AsynchronousServer(int port)
        {
            Port = port;
        }
        public void Start()
        {
            // Establish the local endpoint for the socket.  
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

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
            var Address = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
            Program.Nodes[Address] = state;

            handler.BeginReceive(state.buffer, 0, Node.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
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
                // All the data has arrived; put it in response.  
                PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(state, state.buffer.Take(bytesRead).ToArray()));

                Array.Clear(state.buffer, 0, state.buffer.Length);

                // Signal that all bytes have been received.  
                handler.BeginReceive(state.buffer, 0, Node.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
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
    }
}
