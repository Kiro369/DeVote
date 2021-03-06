using System.Collections.Generic;
using Konscious.Security.Cryptography;
using System.Text;
using System.Linq;
using System;
using DeVote.Structures;

namespace DeVote.Cryptography
{
    public class Argon2
    {
        /// <summary>
        /// Hashes a message using Argon2
        /// </summary>
        /// <param name="message"></param>
        /// <returns>an Argon2 hashed byte array</returns>
        public static byte[] Hash(string message)
        {
            // Console.WriteLine(message);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(message))
            {

                // Fixed Parameters.
                Salt = Constants.Argon2Salt,

                // Number of concurrent threads, that the algorithm will utilize to compute the hash.
                // It's recommended to be twice the amount of available CPU cores.
                DegreeOfParallelism = 8,

                // Tuning these 2 Parameters will affect the execution time and the resulting hash value.
                // Amount of memory (in kilobytes) to use
                MemorySize = 1024,

                // Number of iterations over the memory.
                Iterations = 4
            };

            // Desired number of returned bytes
            return argon2.GetBytes(32);
        }

        /// <summary>
        /// Computes Argon2 Hash from a string
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Hexadecimal string representation of the hash</returns>
        public static string ComputeHash(string message)
        {
            return BitConverter.ToString(Hash(message)).Replace("-", "");
        }

        /// <summary>
        /// Computes Merkle Root from a list of transactions
        /// </summary>
        /// <param name="transactions">List of transactions</param>
        /// <returns>Merkle root</returns>
        public static string ComputeMerkleRoot(List<Transaction> transactions)
        {
            if (transactions.Count == 0)
                return ComputeHash("DeVoteEmptyMerkleRoot");

            string[] hashList = transactions.Select(transaction => transaction.Hash).ToArray();
            while (true)
            {
                if (hashList.Length == 1) return hashList[0];

                var newHashList = new List<string>();

                int len = (hashList.Length % 2 != 0) ? hashList.Length - 1 : hashList.Length;

                for (int i = 0; i < len; i += 2)
                    newHashList.Add(ComputeHash(hashList[i] + hashList[i + 1]));

                if (len < hashList.Length) 
                    newHashList.Add(ComputeHash(hashList[^1] + hashList[^1]));

                hashList = newHashList.ToArray();
            }
        }
    }
}

