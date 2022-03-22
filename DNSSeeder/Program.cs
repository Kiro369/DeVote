using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DNSSeeder
{
    class Program
    {
        // A Concurrent List (or Bag) that gonna hold the Addresses to be seeded
        public static ConcurrentBag<string> Addresses;
        // The port our DNS Seeder gonna be listening to
        private static readonly int Port = 6942;

        static void Main(string[] args)
        {
            // Initalizing our Concurrent Bag
            Addresses = new ConcurrentBag<string>();

            AsynchronousServer seeder = new AsynchronousServer(Port);
            seeder.Start();

        }
    }
}
