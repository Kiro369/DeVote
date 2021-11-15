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

            DNSSeeder.AsynchronousClient seederClient = new DNSSeeder.AsynchronousClient();
            seederClient.StartClient(true);

            Console.ReadLine();

            return;

            Console.WriteLine("Hello World!");

            BlockChain deVOTE = new BlockChain();
            List<Transaction> myTransactions = new List<Transaction>();
            myTransactions.Add(new Transaction(1, DateTime.UtcNow.ToString("d"), "elector1", "elected1"));
            myTransactions.Add(new Transaction(2, DateTime.UtcNow.ToString("d"), "elector2", "elected2"));
            myTransactions.Add(new Transaction(3, DateTime.UtcNow.ToString("d"), "elector3", "elected3"));

            // Console.WriteLine(JsonConvert.SerializeObject(myTransactions));

            Block myBlock = new Block(DateTime.Now, null, myTransactions);
            deVOTE.AddBlock(myBlock);

            Console.WriteLine(JsonConvert.SerializeObject(deVOTE, Formatting.Indented));
        }
    }
}