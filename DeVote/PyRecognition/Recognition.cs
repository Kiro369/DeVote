using System;
using System.IO;
using Python.Runtime;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using DeVotingApp;

/**
 IronPython works only with pure python *.py modules, it does not support C Extension *.pyd modules.
 Python.NET works well with any module because unlike IronPython, it does not compile Python code, it just embed the Python Interpreter in .Net.
**/

namespace DeVote.PyRecognition
{
    public class Recognition
    {
        public static Recognition Current = new Recognition(Settings.Current.PythonDLLPath, Settings.Current.SitePackagesPath);

        private dynamic ocrModule, faceVerificationModule;

        public Recognition(string PythonDLLPath, string SitePackagesPath)
        {
            InitPythonInterpreter(PythonDLLPath, SitePackagesPath);
        }

        public void InitPythonInterpreter(string PythonDLLPath, string SitePackagesPath)
        {
            try
            {
                // Set the path of the Python Interpreter's DLL file.
                // It will be loaded to DeVote.exe process and the python thread will call it to run python code.
                Runtime.PythonDLL = PythonDLLPath;

                var RecognitionModulesPath = Path.GetFullPath(@"../../../../Recognition");

                // Set a list of paths that are searched for imported modules.            
                Environment.SetEnvironmentVariable("PYTHONPATH", $"{SitePackagesPath};{RecognitionModulesPath};", EnvironmentVariableTarget.Process);

                // Append paths to python standard modules path
                PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process) + "C:\\Users\\Kiro\\AppData\\Local\\Programs\\Python\\Python37\\Lib;";

                // Initialize the Python interpreter.
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

                // Console.WriteLine(PythonEngine.PythonPath);
                // Console.WriteLine(PythonEngine.GetPythonThreadID());

                // Acquire the GIL to the current python thread.
                // GIL: Global interpreter lock allows multi-threaded program to interact safely with the Python interpreter.
                using (var _ = Py.GIL())
                {


                    //Console.WriteLine(threadState);

                    var stopwatch = Stopwatch.StartNew();
                    ocrModule = Py.Import("ID_OCR");
                    stopwatch.Stop();
                    Console.WriteLine($"Importing ocr module took {stopwatch.ElapsedMilliseconds} ms ");
                    stopwatch.Restart();

                    var stopwatch2 = Stopwatch.StartNew();
                    faceVerificationModule = Py.Import("identity_verification");
                    stopwatch.Stop();
                    Console.WriteLine($"Importing verification module took {stopwatch.ElapsedMilliseconds} ms ");
                    Console.WriteLine("Python runtime is initialized");
                }
                // We can release the GIL to allow other python threads to run
                //threadState = PythonEngine.BeginAllowThreads();
                // or re-aquire it for the current thread
                //PythonEngine.EndAllowThreads(threadState);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public string ExtractID(string idpath)
        {
            string ID = string.Empty;
            using (Py.GIL())
            {
                dynamic info = ocrModule.ocr_id(idpath, "Front");
                ID = info["Front"]["ID"];
            }
            return ID;
        }

        public IDInfo ScanCard(string path, int execution_time)
        {
            IDInfo Info = null;
            using (Py.GIL())
            {
                var z = ocrModule.scan_card(path, execution_time);
                var y = z[0].ToString().Equals("True");
                var x = z[1]["Front"];
                Info = new IDInfo()
                {
                    Name = x["first_name"],
                    LastName = x["full_name"],
                    Address = x["address"],
                    ID = x["ID"],
                    FrontIDPath = x["front_path"],
                };
            }

            return Info;

        }

        public IDInfo ScanCard(int path, int execution_time)
        {
            using (Py.GIL())
            {
                var z = ocrModule.scan_card(path, execution_time);
                var y = z[0].ToString().Equals("True");
                var x = z[1]["Front"];
                var Info = new IDInfo()
                {
                    Name = x["first_name"],
                    LastName = x["full_name"],
                    Address = x["address"],
                    ID = x["ID"],
                    FrontIDPath = x["front_path"],
                };

                return Info;
            }
        }

        public bool VerifyVoter(string idPath, string voterImage)
        {
            using (Py.GIL())
            {
                var res = faceVerificationModule.verify_id_frame(idPath, voterImage);
                var isVerified = res.IsTrue();
                //bool.TryParse(xyz, out bool isVerified);
                return isVerified;
            }
        }

        public dynamic VerifyPerson(string camPath, string frontIDPath, int numberOfFrames)
        {
            using (Py.GIL())
            {
                return faceVerificationModule.verify_personality(camPath, frontIDPath, numberOfFrames);
            }
        }

        public dynamic VerifyPerson(int camPath, string frontIDPath, int numberOfFrames)
        {
            using (Py.GIL())
            {
                return faceVerificationModule.verify_personality(camPath, frontIDPath, numberOfFrames);
            }
        }

        public bool IsIdSideABackAPI(string idPath)
        {
            bool isSideABack = false;
            var multiForm = new MultipartFormDataContent();
            multiForm.Add(new StringContent("Pdf417"), "types");
            multiForm.Add(new StringContent("type,length"), "fields");
            FileStream fs = File.OpenRead(idPath);
            multiForm.Add(new StreamContent(fs), "file", Path.GetFileName(idPath));
            var url = "https://wabr.inliteresearch.com/barcodes";
            using var client = new HttpClient();
            var response = client.PostAsync(url, multiForm).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request is sent successfully");
                string responseString = response.Content.ReadAsStringAsync().Result;
                JObject responseObj = JObject.Parse(responseString);
                var Barcodes = responseObj.SelectToken("Barcodes[0]");
                if (Barcodes != null)
                {
                    Console.WriteLine($"Barcodes {Barcodes}");
                    isSideABack = true;
                }
                else
                    Console.WriteLine($"No Barcode of type Pdf417 detected {Barcodes}");
            }
            else
                Console.WriteLine($"Could not sent the Request");
            return isSideABack;
        }

        public void EndPythonInterpreter()
        {
            PythonEngine.Shutdown();
        }
    }
}