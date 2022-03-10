using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DeVote.Network.Communication
{
    public class Client
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] Buffer = new byte[BufferSize];

        NetworkStream _n = null;

        TcpClient _client;

        public Client(string ip, int port) {
            _client = new TcpClient(ip, port);
        }
        public Client(IPEndPoint endPoint) {
            _client = new TcpClient(endPoint.Address.ToString(), endPoint.Port);
        }
        protected Client(TcpClient client) {
            _client = client;
        }

        public NetworkStream Stream => _n ??= _client.GetStream();

        public string EndPoint => _client.Client.RemoteEndPoint!.ToString();

        public bool Connected => _client.Client.Connected;
    }
}
