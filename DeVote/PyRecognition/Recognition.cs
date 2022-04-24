using System;
using System.IO;
using Python.Runtime;
using System.Diagnostics;

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
            // Set the path of the Python Interpreter's DLL file.
            // It will be loaded to DeVote.exe process and the python thread will call it to run python code.
            Runtime.PythonDLL = PythonDLLPath;

            var RecognitionModulesPath = Path.GetFullPath(@"../../../../Recognition");

            // Set a list of paths that are searched for imported modules.            
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{SitePackagesPath};{RecognitionModulesPath};", EnvironmentVariableTarget.Process);
           
            // Append paths to python standard modules path
            PythonEngine.PythonPath = PythonEngine.PythonPath + ";" + Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

            // Initialize the Python interpreter.
            PythonEngine.Initialize();

            // Console.WriteLine(PythonEngine.PythonPath);
            // Console.WriteLine(PythonEngine.GetPythonThreadID());

            // Acquire the GIL to the current python thread.
            // GIL: Global interpreter lock allows multi-threaded program to interact safely with the Python interpreter.
            using var _ = Py.GIL();

            // We can release the GIL to allow other python threads to run
            // threadState = PythonEngine.BeginAllowThreads();
            // or re-aquire it for the current thread
            // PythonEngine.EndAllowThreads(threadState);

            // Console.WriteLine(threadState);

            var stopwatch = Stopwatch.StartNew();
            ocrModule = Py.Import("ID_OCR");
            faceVerificationModule = Py.Import("identity_verification");
            stopwatch.Stop();
            Console.WriteLine($"Importing two modules took { stopwatch.ElapsedMilliseconds / 1024.0 } s ");
            Console.WriteLine("Python runtime is initialized");
        }

        public dynamic ExtractIDInfo(string idpath,string face)
        {
            var stopwatch = Stopwatch.StartNew();
            dynamic info = ocrModule.ocr_id(idpath,face);
            stopwatch.Stop();
            Console.WriteLine($"Extracting IDInfo took { stopwatch.ElapsedMilliseconds / 1024.0 } s ");
            return info;
        }

        public bool VerifyVoter(string idPath, string voterImage)
        {
            return faceVerificationModule.verify_id_frame(idPath, voterImage);
        }
        public void EndPythonInterpreter()
        {
            PythonEngine.Shutdown();
        }
    }
}