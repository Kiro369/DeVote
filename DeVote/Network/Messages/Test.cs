using System;
using System.Collections.Generic;
using System.Text;

namespace DeVote.Network.Messages
{
    [Handling.NodePacketHandler(PacketTypes.Test)]
    class Test : IPacket
    {
        public Test(byte[] arr)
        {

        }

        public void Handle(Node client)
        {
            throw new NotImplementedException();
        }

        public bool Read(Node client)
        {
            throw new NotImplementedException();
        }
    }
}
