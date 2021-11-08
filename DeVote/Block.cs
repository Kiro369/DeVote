using System;
using System.Text;
using System.Security.Cryptography;

namespace DeVote
{
    public class Block
    {
        public int Height { get; set; }
        public string Date;
        public string Miner;
        public string Hash { get; set; }
        public string PrevHash;
        public string Data { get; set; }

        public Block(string date, string prevHash, string data)
        {
            Date = date;
            PrevHash = prevHash;
            Data = data;
            Hash = ComputeHash();
        }

        public string ComputeHash()
        {
            // Create a SHA256   
            using (SHA256 sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{Date}{PrevHash}{Data}"));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}