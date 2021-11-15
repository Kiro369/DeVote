using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DNSSeeder
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Size of receive buffer. We only need 1 byte, 1 for seeding and adding the IP of the client to the list and others for seeding only.
        public const int BufferSize = 1;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Client socket.
        public Socket workSocket = null;

        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

}
