using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeVote.Structures;
using System.Text;
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
            for (int i = 0; i < 10; i++)
            {
                myTransactions.Add(new Transaction(i, DateTime.UtcNow, "elector" + i, "elected" + i));
            }

            Block myBlock = new Block(DateTime.UtcNow, null, myTransactions);
            deVOTE.AddBlock(myBlock);

            // Save block into leveldb
            deVOTE.SaveBlock(myBlock);

            // byte[] TargetBlock1 = deVOTE.LoadBlock("1");
            // Console.WriteLine("TargetBlock {0}",Encoding.UTF8.GetString(TargetBlock1));
            
            List<Byte[]> SerializedBlockChain = deVOTE.LoadBlockChain();
            Console.WriteLine("SerializedBlockChain");
            Console.WriteLine(JsonConvert.SerializeObject(SerializedBlockChain, Formatting.Indented));


            Console.WriteLine("");
            deVOTE.SaveBlockChain(SerializedBlockChain);

            // Close the connection
            deVOTE.LevelDB.Close();
        }
    }
}