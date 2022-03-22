using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DNSSeeder
{
    class AsynchronousServer
    {
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
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(RecieveCallback), state);
        }
        public void RecieveCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Getting the Address of the Remote end point
            var Address = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                if (Address == "127.0.0.1")
                    Address = GetPublicIP();

                var port = BitConverter.ToInt32(state.buffer, 0);
                Address = Address + ":" + port;

                Console.WriteLine("Seeding the Addresses to " + Address);
                var addresses = Program.Addresses.Where(addr => !addr.Equals(Address)).ToArray();
                Send(handler, string.Join(Environment.NewLine, addresses));

                if (port != 0)
                {
                    if (Program.Addresses.Contains(Address))
                        Console.WriteLine("Address [" + Address + "] is already added to the list");
                    else
                    {
                        Console.WriteLine("Adding " + Address + " to our list");
                        Program.Addresses.Add(Address);
                    }
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

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        string GetPublicIP()
        {
            return new WebClient().DownloadString("https://api.ipify.org");
        }
        IPAddress GetNebulaIP()
        {
            return NetworkInterface.GetAllNetworkInterfaces().First(ni => ni.Name.Equals("nebula1")).GetIPProperties().UnicastAddresses.FirstOrDefault()!.Address;
        }
    }
}
