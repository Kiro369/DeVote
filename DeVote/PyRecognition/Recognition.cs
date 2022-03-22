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
        private dynamic OCRModule;
        private dynamic FaceVerificationModule;
        private IntPtr threadState;
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
            OCRModule = Py.Import("ID_OCR");
            FaceVerificationModule = Py.Import("identity_verification");
            stopwatch.Stop();
            Console.WriteLine($"Importing two modules took { stopwatch.ElapsedMilliseconds / 1024.0 } s ");
            Console.WriteLine("Python runtime is initialized");
        }

        public dynamic ExtractIDInfo(string idpath,string face)
        {
            var stopwatch = Stopwatch.StartNew();
            dynamic info = OCRModule.ocr_id(idpath,face);
            stopwatch.Stop();
            Console.WriteLine($"Extracting IDInfo took { stopwatch.ElapsedMilliseconds / 1024.0 } s ");
            return info;
        }

        public dynamic VerifyVoter(string idpath, string face)
        {
            // TODO: Invoke the right function
            FaceVerificationModule.verify_id_frame(idpath,"");
            return "";
        }
        public void EndPythonInterpreter()
        {
            PythonEngine.Shutdown();
        }
    }
}