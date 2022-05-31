using System;
using System.IO;
using Python.Runtime;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Http;

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

                // Console.WriteLine(PythonEngine.PythonPath);
                // Console.WriteLine(PythonEngine.GetPythonThreadID());

                // Acquire the GIL to the current python thread.
                // GIL: Global interpreter lock allows multi-threaded program to interact safely with the Python interpreter.
                using var _ = Py.GIL();

                // We can release the GIL to allow other python threads to run
                //var threadState = PythonEngine.BeginAllowThreads();
                // or re-aquire it for the current thread
                //PythonEngine.EndAllowThreads(threadState);

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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Py.GILState GIL()
        {
            return Py.GIL();
        }

        public dynamic ExtractIDInfo(string idpath,string face)
        {
            var stopwatch = Stopwatch.StartNew();
            dynamic info = ocrModule.ocr_id(idpath,face);
            stopwatch.Stop();
            Console.WriteLine($"Extracting IDInfo took { stopwatch.ElapsedMilliseconds } ms ");
            return info;
        }

        public string ScanCard(string path, int execution_time)
        {
            return ocrModule.scan_card(path, execution_time);
        }
        public bool test()
        {
            return ocrModule != null;
        }
        public dynamic ScanCard(int path, int execution_time)
        {
            return ocrModule.scan_card(path, execution_time);
        }

        public bool VerifyVoter(string idPath, string voterImage)
        {
            var stopwatch = Stopwatch.StartNew();
            bool isVerified = faceVerificationModule.verify_id_frame(idPath, voterImage);
            stopwatch.Stop();
            Console.WriteLine($"Verifying a voter's frame against ID took { stopwatch.ElapsedMilliseconds } ms ");
            return isVerified;
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

        public bool IsFrontOrBack(string imgPath)
        {
            var res = ocrModule.is_front_back(imgPath);
            return false;
        }

        public void EndPythonInterpreter()
        {
            PythonEngine.Shutdown();
        }
        public dynamic ContainsCard(string imagePath) /*object img*/
        {
            dynamic isCardPresent = ocrModule.is_there_card(imagePath);
            return isCardPresent;
        }
        public void PlayTTS(string text)
        {

        }
    }
}