using System;
using System.IO;
using Newtonsoft.Json;

namespace DeVote
{
    public class Settings
    {
        public string _Argon2Salt { get; set; }
        public string _BlockchainPath { get; set; }
        public string _VotedDLTPath { get; set; }
        public string _PythonDLLPath { get; set; }
        public string _SitePackagesPath { get; set; }
        public string _RecognitionModulesPath { get; set; }
        public double _Latitude { get; set; }
        public double _Longitude { get; set; }
        public int _BlockchainExplorerPort { get; set; }

        public static string Argon2Salt { get; private set; }
        public static string BlockchainPath { get; private set; }
        public static string VotedDLTPath { get; private set; }
        public static string PythonDLLPath { get; private set; }
        public static string SitePackagesPath { get; private set; }
        public static string RecognitionModulesPath { get; private set; }
        public static double Latitude { get; private set; }
        public static double Longitude { get; private set; }
        public static int BlockchainExplorerPort { get; private set; }

        // deserialize appsettings.json to a settings instance then assign its values to the static variables of Settings class.
        public static void SetSettings()
        {
            var currentDir = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().LastIndexOf("bin"));
            string appsettingsFilePath = Path.Combine(currentDir, "appsettings.json");

            if (!File.Exists(appsettingsFilePath)) throw new FileNotFoundException("appsettings.json does not exist.");

            // Console.WriteLine($"currenDir : {currentDir}");
            // Console.WriteLine($"appsettingsFilePath : {appsettingsFilePath}");

            string jsonString = File.ReadAllText(appsettingsFilePath);
            Settings settings = JsonConvert.DeserializeObject<Settings>(jsonString);

            Argon2Salt = settings._Argon2Salt;
            BlockchainPath = settings._BlockchainPath;
            VotedDLTPath = settings._VotedDLTPath;
            PythonDLLPath = settings._PythonDLLPath;
            SitePackagesPath = settings._SitePackagesPath;
            RecognitionModulesPath = settings._RecognitionModulesPath;
            Latitude = settings._Latitude;
            Longitude = settings._Longitude;
            BlockchainExplorerPort = settings._BlockchainExplorerPort;
        }
    }
}
