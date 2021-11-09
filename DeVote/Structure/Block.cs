using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Structure
{
    public class Block
    {
        public int Height { get; set; }
        // public string Date;
        public string Miner;
        public DateTime Timestamp;
        public string Hash { get; set; }
        public string PrevHash;
        public List<Transaction> Transactions { get; set; }


        public Block(DateTime timestamp, string prevHash, List<Transaction> transactions)
        {
            Timestamp = timestamp;
            PrevHash = prevHash;
            Transactions = transactions;
            Hash = ComputeHash();
        }

        public string ComputeHash()
        {
            // Create a SHA256   
            using (SHA256 sha256 = SHA256.Create())
            {
                string data = Timestamp + PrevHash + JsonConvert.SerializeObject(Transactions);
                // Console.WriteLine(data);
                // ComputeHash - returns byte array  
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

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