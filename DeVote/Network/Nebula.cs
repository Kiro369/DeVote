using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DeVote.Network
{
    public class Nebula
    {
        public static IPAddress GetNebulaIP()
        {
            return NetworkInterface.GetAllNetworkInterfaces().First(ni => ni.Name.Equals("nebula1")).GetIPProperties().UnicastAddresses.FirstOrDefault()!.Address;
        }
    }
}
