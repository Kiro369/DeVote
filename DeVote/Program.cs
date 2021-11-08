using System;
using Newtonsoft.Json;

namespace DeVote
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            BlockChain deVOTE = new BlockChain();
            deVOTE.AddBlock(new Block(DateTime.UtcNow.ToString("d"), null, "{Elector:Voter1,Elected:Elected1}"));
            deVOTE.AddBlock(new Block(DateTime.UtcNow.ToString("d"), null, "{Elector:Voter2,Elected:Elected2}"));
            // Console.WriteLine(string.Join<Block>("\n", deVOTE.VChain));
            Console.WriteLine(JsonConvert.SerializeObject(deVOTE, Formatting.Indented));
        }
    }
}