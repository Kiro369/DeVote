using System;
using System.IO;
using Newtonsoft.Json;

namespace DeVote
{
    public class Settings
    {
        public string Argon2Salt { get; set; }
        public string BlockchainPath { get; set; }
        public string VotedDLTPath { get; set; }
        public string TransactionsDLTPath { get; set; }
        public string BlocksPath { get; set; }

        public string PythonDLLPath { get; set; }
        public string SitePackagesPath { get; set; }
        public string RecognitionModulesPath { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Governorate { get; set; }
        public string BlockchainExplorerEndPoint { get; set; }
        public bool FullNode { get; set; }

        public static readonly Settings Current;

        // A Static constructor is called only once, before any static member is accessed.
        static Settings()
        {
            string appsettingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            if (!File.Exists(appsettingsFilePath)) throw new FileNotFoundException("appsettings.json does not exist.");

            string jsonString = File.ReadAllText(appsettingsFilePath);
            Current = JsonConvert.DeserializeObject<Settings>(jsonString);
        }
    }
}
