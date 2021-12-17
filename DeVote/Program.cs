using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeVote.Structures;

namespace DeVote
{
    class Program
    {
        static void Main(string[] args)
        {

            // DNSSeeder.AsynchronousClient seederClient = new DNSSeeder.AsynchronousClient();
            // seederClient.StartClient(true);

            // Console.ReadLine();

            // return;

            Console.WriteLine("Hello World!");

            BlockChain deVOTE = new BlockChain();
            List<Transaction> myTransactions = new List<Transaction>();
            myTransactions.Add(new Transaction(1, DateTime.UtcNow.ToString("d"), "elector1", "elected1"));
            myTransactions.Add(new Transaction(2, DateTime.UtcNow.ToString("d"), "elector2", "elected2"));
            myTransactions.Add(new Transaction(3, DateTime.UtcNow.ToString("d"), "elector3", "elected3"));

            Block myBlock = new Block(DateTime.Now, null, myTransactions);
            deVOTE.AddBlock(myBlock);

            // Save block into leveldb
            deVOTE.SaveBlock(myBlock);

            // Create new iterator
            using (var iterator = deVOTE.LevelDB.CreateIterator())
            {
                // Iterate to print the key-value pairs as strings
                for (iterator.SeekToFirst(); iterator.IsValid(); iterator.Next())
                {
                    Console.WriteLine("Block : {0}", iterator.KeyAsString());
                    Console.WriteLine("Value : {0}", iterator.ValueAsString());
                }
            }

            // Close the connection
            deVOTE.LevelDB.Close();
        }
    }
}