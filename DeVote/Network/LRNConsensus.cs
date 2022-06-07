using DeVote.Extensions;
using DeVote.Network.Messages;
using DeVote.Structures;
using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DeVote.Network
{
    public class LRNConsensus
    {
        public static LRNConsensus Current = new();

        /// <summary>
        /// The choosen Node, that will add the next Block!
        /// </summary>
        public string Choosen { get; private set; }

        /// <summary>
        /// Starts the consensus algorithm
        /// </summary>
        public async Task Start()
        {
            for ( ; ; )
            {
                var Time = GetInternetTime();

                var currentMinute = Time.Minute;
                var mineMinute = Round(currentMinute);

                if (currentMinute == mineMinute)
                {
                    Choosen = Perform();

                    // Reset the RN of all Nodes
                    NetworkManager.ResetRNConsensus();

                    if (Choosen.Equals(Constants.MachineID))
                    {
                        Blockchain.Current.AddBlock(Blockchain.Current.Block);
                        var addBlock = new AddBlock() { Block = Blockchain.Current.Blocks.Last.Value };
                        NetworkManager.Broadcast(addBlock.Create());
                    }

                }
                else if (currentMinute == mineMinute - 1)
                {
                    // Generate Radom number and broadcast it 
                    var rn = FastRandom.Current.NextInt64();
                    var consensusPacket = new Messages.LRNConsensus();
                    var generatedPacket = consensusPacket.Create(rn);
                    NetworkManager.Broadcast(generatedPacket);
                }

                var seconds = 60 - Time.Second; // Seconds left to next minute
                await Task.Delay(seconds * 1000); // Optimally in this loop, we want to do 1 iteration per minute 
            }
        }

        /// <summary>
        /// Performs the consensus to pick who will mine the next block!
        /// </summary>
        /// <returns>The choosen one</returns>
        public static string Perform()
        {
            var s = string.Empty;
            var min = long.MaxValue;
            foreach (var item in NetworkManager.GetNodes())
            {
                if (item.ConsensusRN < min)
                {
                    s = item.EndPoint;
                    min = item.ConsensusRN;
                }
            }
            return s;
        }

        /// <summary>
        /// Clears the choosen Node, should be called after adding a block
        /// </summary>
        public void Clear()
        {
            Choosen = string.Empty;
        }

        /// <summary>
        /// Rounds current minute to nearest mine minute (the minute a block will be mined)
        /// </summary>
        /// <param name="minute">Current minute -to round</param>
        /// <returns></returns>
        static int Round(int minute)
        {
            while (minute % Constants.BlockTime != 0)
                minute++;
            return minute;
        }

        /// <summary>
        /// Gets current UTC time from a reliable source on the internet, since current device time can be manipulated.
        /// </summary>
        /// <returns></returns>
        private static DateTime GetInternetTime()
        {
            var client = new TcpClient("time.nist.gov", 13);
            using var streamReader = new StreamReader(client.GetStream());
            var response = streamReader.ReadToEnd();
            var utcDateTimeString = response.Substring(7, 17);
            return DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }
    }
}
