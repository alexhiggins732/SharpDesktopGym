using Python.Runtime;
using System;

namespace GymSharp
{
    public class PythonInitiliazer
    {
        static PythonInitiliazer()
        {
            InitializePython();
        }
        static bool initialized;
        public static void InitializePython()
        {
            if (!initialized)
            {
                initPython();
                initialized = true;
            }
        }
        private static void initPython()
        {

            string pathToVirtualEnv = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python37_64\";

            var path = Environment.GetEnvironmentVariable("PATH").TrimEnd(';');
            path = string.IsNullOrEmpty(path) ? pathToVirtualEnv : path + ";" + pathToVirtualEnv;
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONPATH", $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib;{pathToVirtualEnv}\\scripts", EnvironmentVariableTarget.Process);
            Runtime.PythonDLL = "C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\Python37_64\\python37.dll";

            PythonEngine.PythonHome = pathToVirtualEnv;
            PythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);
            PythonEngine.Initialize();
        }
    }
}
