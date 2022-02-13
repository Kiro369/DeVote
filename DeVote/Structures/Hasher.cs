using System.Collections.Generic;
using Konscious.Security.Cryptography;
using System.Text;

namespace DeVote.Structures
{
    public class Hasher
    {
        public static string ComputeHash(string message)
        {
            // Console.WriteLine(message);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(message));
            
            // TODO : adding Constants.cs
            // Fixed Parameters.
            argon2.Salt = Encoding.UTF8.GetBytes("okdedjdn");

            // Number of concurrent threads, that the algorithm will utilize to compute the hash.
            // It's recommended to be twice the amount of available CPU cores.
            argon2.DegreeOfParallelism = 8;

            // Tuning these 2 Parameters will affect the execution time and the resulting hash value.
            // Amount of memory (in kilobytes) to use
            argon2.MemorySize = 1024;

            // Number of iterations over the memory.
            argon2.Iterations = 4;

            // Desired number of returned bytes
            return HexHash(argon2.GetBytes(32));
        }

        public static string HexHash(byte[] HashBytes)
        {
            StringBuilder HashHex = new StringBuilder();
            for (int i = 0; i < HashBytes.Length; i++)
            {
                HashHex.Append(HashBytes[i].ToString("x2"));
            }
            return HashHex.ToString();
        }

        public static string[] listHashes(List<Transaction> transactionC)
        {
            List<string> HashList = new List<string>();
            for (int i = 0; i < transactionC.Count; i++)
            {
                HashList.Add(transactionC[i].Hash);
            }
            return HashList.ToArray();
        }


        public static string ComputeMerkleRoot(List<Transaction> transactions)
        {
            string[] HashList = listHashes(transactions);
            while (true)
            {
                if (HashList.Length == 1) return HashList[0];

                List<string> newHashList = new List<string>();

                int len = (HashList.Length % 2 != 0) ? HashList.Length - 1 : HashList.Length;

                for (int i = 0; i < len; i += 2)
                    newHashList.Add(ComputeHash(HashList[i] + HashList[i + 1]));

                if (len < HashList.Length) newHashList.Add(
                        ComputeHash(HashList[HashList.Length - 1] + HashList[HashList.Length - 1])
                    );

                HashList = newHashList.ToArray();
            }
        }
    }
}

