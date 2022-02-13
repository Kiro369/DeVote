using System;
using System.Collections.Generic;
using System.Text;

namespace DeVote.Network
{
    public interface IPacket
    {
        bool Read(Node client);
        void Handle(Node client);
        //void Dispose();
    }
}
