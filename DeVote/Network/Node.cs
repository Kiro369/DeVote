using DeVote.Cryptography;
using DeVote.Network.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network
{
    public class Node : Client
    {
        public Node(string ip, int port) : base(ip, port) { }
        public Node(string endPoint) : base(IPEndPoint.Parse(endPoint)) { }
        private Node(TcpClient client) : base(client) { }

        public async void Start()
        {
            while (!Connected)
                Console.WriteLine("Waiting to get connected to " + EndPoint);

            Program.Nodes[EndPoint] = this;

            await Read();
        }
        async Task Read()
        {
            await Task.Yield();
            try
            {
                while (true)
                {
                    if (Stream.CanRead)
                    {
                        int bytesRead = Stream.Read(buffer, 0, BufferSize);
                        if (bytesRead > 0)
                        {
                            // Add the packet to the handler to be handled
                            PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(this, buffer.Take(bytesRead).ToArray()));

                            // Clear buffer 
                            Array.Clear(buffer, 0, buffer.Length);

                            // Keep recieving.  
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    Program.Nodes.Remove(EndPoint);
                    Console.WriteLine(EndPoint + " forcibly disconnected");
                }
                else throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            Send(byteData);
        }
        public void Send(byte[] byteData, bool encrypt = true)
        {
            try
            {
                // Encrypt data if possible
                if (AES.Key != null && encrypt)
                    byteData = AES.Encrypt(byteData);

                // Begin sending the data to the remote device.  
                Stream.WriteAsync(byteData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static implicit operator Node(TcpClient client) => new Node(client);
    }
}
