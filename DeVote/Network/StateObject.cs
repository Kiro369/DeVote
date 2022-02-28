using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DeVote.Network
{
    public class Node
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Client socket.
        public Socket Socket = null;

        public string Address
        {
            get
            {
                return Socket == null ? "" : ((IPEndPoint)Socket.RemoteEndPoint).ToString();
            }
        }

        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            Send(byteData);
        }
        public void Send(byte[] byteData)
        {
            try
            {
                // Begin sending the data to the remote device.  
                Socket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), Socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
