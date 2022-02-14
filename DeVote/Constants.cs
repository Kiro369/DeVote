using DeVote.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeVote
{
    class Constants
    {
        public static byte[][] ECDHOperations = new byte[][]
        {
            Argon2.Hash("DeVoteECDiffieHellmanRequest"),
            Argon2.Hash("DeVoteECDiffieHellmanInitalResponse"),
            Argon2.Hash("DeVoteECDiffieHellmanFinalResponse")
        };
        public static byte[] Argon2Salt = Encoding.UTF8.GetBytes("DeVoteArgon2Salt");
    }
}
