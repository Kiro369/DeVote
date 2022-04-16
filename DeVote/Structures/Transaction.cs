using System;
using ProtoBuf;
using System.Text;
using System.Diagnostics;
using DeVote.Cryptography;

namespace DeVote.Structures
{
    [ProtoContract(SkipConstructor = true)]
    public class Transaction
    {
        [ProtoMember(1)] public string Hash { get; set; }
        [ProtoMember(2)] public long Date;
        [ProtoMember(3)] public string Elector { get; set; }
        [ProtoMember(4)] public string Elected { get; set; }

        public Transaction(string elector, string elected)
        {
            Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Elector = elector;
            Elected = elected;

            var stopwatch = Stopwatch.StartNew();
            Hash = Argon2.ComputeHash(this.Date + this.Elector + this.Elected);
            stopwatch.Stop();
            Console.WriteLine($"Hashing Tx took { stopwatch.ElapsedMilliseconds / 1024.0 } s ");
        }
    }
}