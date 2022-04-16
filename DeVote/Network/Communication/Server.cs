using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network.Communication
{
    public class Server : TcpListener
    {
        bool _stop = false;
        public int Port => ((IPEndPoint)LocalEndpoint).Port;
        public Server(int port = 4269) : base(IPAddress.Any, port) {}

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
                NetworkManager.Nodes[node.EndPoint] = node;
                Read(node);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void Read(Node node)
        {
            try
            {
                node.Stream.BeginRead(node.Buffer, 0, Client.BufferSize, ReadCallback, node);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    NetworkManager.Nodes.Remove(node.EndPoint);
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
                    PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(node, node.Buffer.Take(bytesRead).ToArray()));

                    // Clear buffer 
                    Array.Clear(node.Buffer, 0, node.Buffer.Length);

                    // Keep recieving.  
                    Read(node);
                }
            }
            catch (System.IO.IOException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                Console.WriteLine(e.ToString());
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    NetworkManager.Nodes.Remove(node.EndPoint);
                    Console.WriteLine(node.EndPoint + " forcibly disconnected");
                }
                else throw e;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static int FindPort(int basePort)
        {

            var externalIP = new HttpClient().GetStringAsync("https://api.ipify.org").Result;
            while (true)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        var connected = client.ConnectAsync(externalIP, basePort).Wait(1000);
                        if (connected)
                            basePort++;
                        else return basePort;
                    }
                }
                catch
                {
                    return basePort;
                }
            }
        }
        public new void Stop()
        {
            _stop = true;
        }
    }
}
