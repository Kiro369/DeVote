using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeVote.Network.Cryptography
{
    [ProtoContract]
    class AESKey
    {
        [ProtoMember(1)]
        public byte[] Key {get;set;}
        [ProtoMember(2)]
        public byte[] IV { get; set; }
    }
}
