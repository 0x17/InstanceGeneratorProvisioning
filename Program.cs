using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using GAMS;

namespace InstanceGeneratorProvisioning
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var sysDir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Config.WinGamsPath : Config.OsxGamsPath;
            var ws = new GAMSWorkspace(Directory.GetCurrentDirectory(), sysDir);

            Generator.BatchGenerateInstances(ws, 2);

            //Solver.BatchSolveInstances(ws, 2);
            //Solver.PrintAllResultsInPath(ws, Directory.GetCurrentDirectory());

            Console.WriteLine("Press a key to quit...");
            Console.ReadKey();
        }
    }
}