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
        public byte[] buffer = new byte[BufferSize];

        NetworkStream _n = null;

        TcpClient _client;

        public Client(string ip, int port) {
            _client = new TcpClient(ip, port);
        }
        public Client(IPEndPoint endPoint) {
            _client = new TcpClient(endPoint);
        }
        protected Client(TcpClient client) {
            _client = client;
        }

        public NetworkStream Stream
        {
            get
            {
                return _n == null ? _n = _client.GetStream() : _n;
            }
        }
        public string EndPoint
        {
            get
            {
                return _client.Client.RemoteEndPoint.ToString();
            }
        }
        public bool Connected
        {
            get
            {
                return _client.Client.Connected;
            }
        }
    }
}
