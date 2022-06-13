using DeVote.Extensions;
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
                NetworkManager.AddNode(node.EndPoint, node);
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
                node.Stream.BeginRead(node.Buffer, node.Index, Client.BufferSize - node.Index, ReadCallback, node);
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    NetworkManager.RemoveNode(node.EndPoint);
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

            // Temp buffer
            byte[] buffer;

            try
            {
                int bytesRead = node.Stream.EndRead(ar);

                if (bytesRead > 0)
                {
                    node.Index += bytesRead;

                    // Check for footer, to know wether we recieved the full package or not
                    if ((buffer = node.Buffer.Take(node.Index).ToArray()).EndsWith(Constants.EOTFlag))
                    {
                        // Add the packet to the handler to be handled
                        PacketsHandler.Packets.Enqueue(new KeyValuePair<Node, byte[]>(node, buffer.SkipLast(Constants.EOTFlag.Length).ToArray()));

                        // Clear buffer 
                        Array.Clear(node.Buffer, 0, node.Buffer.Length);

                        // Clear index
                        node.Index = 0;
                    }

                    // Keep recieving.  
                    Read(node);
                }
            }
            catch (System.IO.IOException e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException is SocketException)
                    {
                        if ((e.InnerException as SocketException).ErrorCode == 10054)
                        {
                            NetworkManager.RemoveNode(node.EndPoint);
                            Console.WriteLine(node.EndPoint + " forcibly disconnected");
                            return;
                        }
                    }
                    else throw e.InnerException;
                }
                Console.WriteLine(e.ToString());
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 10054)
                {
                    NetworkManager.RemoveNode(node.EndPoint);
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
