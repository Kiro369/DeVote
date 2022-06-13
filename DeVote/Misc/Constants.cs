using DeviceId;
using DeVote.Cryptography;
using DeVote.Extensions;
using System.IO;
using System.Text;

namespace DeVote
{
    public class Constants
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

        public static byte[] EOTFlag = Argon2.Hash("DeVoteEndOfTransmission");

        /// <summary>
        /// Argon2 hash salt, each application should have its own Salt
        /// </summary>
        public static byte[] Argon2Salt = Encoding.UTF8.GetBytes(Settings.Current.Argon2Salt);

        public static readonly string MachineID = Argon2.ComputeHash(new DeviceIdBuilder().AddMacAddress().AddMachineName().AddOsVersion().ToString()), // Unique ID for each machine
            Candidate1ID = Argon2.ComputeHash("CC"),
            Candidate2ID = Argon2.ComputeHash("Mousa");

        public static readonly FastRandom FastRandom = new();

        /// <summary>
        /// BlockTime is the time between every block, time is represented in minutes. Note: A block is added whenever time_now.minute % BlockTime == 0
        /// </summary>
        public const short BlockTime = 5;
    }
}
