using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network.Communication
{
    public class Server : TcpListener
    {
        bool _stop = false;
        public int Port
        {
            get
            {
                return ((IPEndPoint)LocalEndpoint).Port;
            }
        }
        public Server(int port = 4269, DNSSeeder.AsynchronousClient seederClient = null) : base(IPAddress.Any, FindPort(port, seederClient)) {}

        public async void RunServerAsync()
        {
            Start();
            try
            {
                while (!_stop)
                    await Accept(await AcceptTcpClientAsync());
            }
            finally { base.Stop(); }
        }
        async Task Accept(TcpClient client)
        {
            await Task.Yield();
            try
            {
                var node = (Node)client;
                Program.Nodes[node.EndPoint] = node;
                Read(node);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void Read(Node node)
        {
            try
            {
                node.Stream.BeginRead(node.buffer, 0, Client.BufferSize, new AsyncCallback(ReadCallback), node);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    Program.Nodes.Remove(node.EndPoint);
                    Console.WriteLine(node.EndPoint + " forcibly disconnected");
                }
                else throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public async void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            Node node = (Node)ar.AsyncState;

            try
            {
                int bytesRead = node.Stream.EndRead(ar);

                if (bytesRead > 0)
                {
                    // Add the packet to the handler to be handled
                    PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(node, node.buffer.Take(bytesRead).ToArray()));

                    // Clear buffer 
                    Array.Clear(node.buffer, 0, node.buffer.Length);

                    // Keep recieving.  
                    Read(node);
                }
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    Program.Nodes.Remove(node.EndPoint);
                    Console.WriteLine(node.EndPoint + " forcibly disconnected");
                }
                else throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static int FindPort(int basePort, DNSSeeder.AsynchronousClient seederClient)
        {
            if (seederClient == null) return basePort;
            var externalIP = new WebClient().DownloadString("https://api.ipify.org");
            seederClient.StartClient();
            while (true)
            {
                var endPoint = externalIP + ":" + basePort;
                if (!seederClient.EndPoints.Contains(endPoint))
                    return basePort;
                basePort++;
            }
        }
        public new void Stop()
        {
            _stop = true;
        }
    }
}
