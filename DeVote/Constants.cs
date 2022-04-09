using DeviceId;
using DeVote.Cryptography;
using System.IO;
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
        public static readonly string BlockchainPath = Directory.GetCurrentDirectory() + "\\Blockchain",
            VotedDLTPath = Directory.GetCurrentDirectory() + "\\VotedDLT",
            MachineID = Argon2.ComputeHash(new DeviceIdBuilder().AddMacAddress().AddMachineName().AddOsVersion().ToString());
    }
}
