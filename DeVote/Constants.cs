using DeviceId;
using DeVote.Cryptography;
using System.IO;
using System.Text;

namespace DeVote
{
    class Constants
    {
        /// <summary>
        /// Packets headers for Elliptic curve Diffie-Hellman Key Exchange process
        /// </summary>
        public static byte[][] ECDHOperations = new byte[][]
        {
            Argon2.Hash("DeVoteECDiffieHellmanRequest"),
            Argon2.Hash("DeVoteECDiffieHellmanInitalResponse"),
            Argon2.Hash("DeVoteECDiffieHellmanFinalResponse")
        };

        /// <summary>
        /// Argon2 hash salt, each application should have its own Salt
        /// </summary>
        public static byte[] Argon2Salt = Encoding.UTF8.GetBytes("DeVoteArgon2Salt");
        
        public static readonly string BlockchainPath = Directory.GetCurrentDirectory() + "\\Blockchain", // Blockchain LevelDB database path
            VotedDLTPath = Directory.GetCurrentDirectory() + "\\VotedDLT", // VotedDLT (the side DLT containing people voted) LevelDB database path
            MachineID = Argon2.ComputeHash(new DeviceIdBuilder().AddMacAddress().AddMachineName().AddOsVersion().ToString()); // Unique ID for each machine

        /// <summary>
        /// BlockTime is the time between every block, time is represented in minutes. Note: A block is added whenever time_now.minute % BlockTime == 0
        /// </summary>
        public const short BlockTime = 5; 
    }
}
