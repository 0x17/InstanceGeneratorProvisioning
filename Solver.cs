using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAMS;
using InstanceGeneratorProvisioning.Helpers;

namespace InstanceGeneratorProvisioning
{
    public class Solver
    {
        /*public static void SolveWithFixedOrderRepairs(GAMSWorkspace ws, List<Order> orders, List<Repair> repairs, int instanceIx)
        {
            using (var options = ws.AddOptions())
            {
                GAMSCheckpoint cp = ws.AddCheckpoint();
                var job = ws.AddJobFromFile($"{Config.ModelDirectoryPath}/{Config.ModelFileName}", jobName: $"JobFuerInstanz{instanceIx + 1}");
                job.Run(cp);
                var mi = cp.AddModelInstance();
                //mi.Instantiate();
            }

        }*/

        public static void BatchSolveInstances(GAMSWorkspace ws, int numInstances)
        {
            using (var options = ws.AddOptions())
            {
                for (var i = 0; i < numInstances; i++)
                {
                    var job = ws.AddJobFromFile($"{Config.ModelDirectoryPath}/{Config.ModelFileName}", jobName: $"JobFuerInstanz{i + 1}");
                    options.Defines["num"] = (i + 1).ToString();

                    options.Defines["herkunft"] = "0";
                    job.Run(options);
                    PrintResults(job.OutDB, i+1, false);

                    options.Defines["herkunft"] = "1";
                    job.Run(options);
                    PrintResults(job.OutDB, i+1, true);
                }
            }
        }

        public static string[] GetAllResulFilesForPath(string path)
        {
            return Directory.GetFiles(path).Select(Path.GetFileName).Where(filename => filename.EndsWith(".gdx") && filename.StartsWith("output_instance_")).ToArray();
        }

        public static void PrintAllResultsInPath(GAMSWorkspace ws, string path)
        {
            string[] resultFilenames = GetAllResulFilesForPath(path);
            foreach (var resultFilename in resultFilenames)
            {
                bool enforcedOrigin = int.Parse(resultFilename.Substring(resultFilename.Length - 5, 1)) == 1;
                GAMSDatabase db = ws.AddDatabaseFromGDX(resultFilename);
                int instanceIx = int.Parse(resultFilename.Replace("output_instance_", "")
                    .Replace($"_herkunft_{(enforcedOrigin ? "1" : "0")}.gdx", "")) - 1;
                PrintResults(db, instanceIx, enforcedOrigin);
            }
        }

        private static void PrintResults(GAMSDatabase db, int instanceIx, bool enforceOrigin)
        {
            string ostr = "";
            ostr += $"Instance {instanceIx + 1} {(enforceOrigin ? "with" : "w/out")} enforced origin\n";
            ostr += $"Delays {GDXUtils.GetLevelsStr(db.GetVariable("v"))}\n";
            ostr += $"Costs:\t{db.GetVariable("f").FirstRecord().Level}\n";
            
            int numGoods = db.GetSet("i").NumberRecords;
            int numComponents = db.GetSet("k").NumberRecords;

            var z = db.GetVariable("z");
            var xint = db.GetVariable("xint");
            var x = db.GetVariable("x");

            var orders = GetOrders(db.GetVariable("w"));

            // first good is external source (not from disassembling but instead from order)
            for (int i = 1; i < numGoods; i++)
            {
                for (int k = 0; k < numComponents; k++)
                {
                    int whenProvided = WhenProvided(x, i, k);
                    ostr += $"i{i + 1},k{k + 1}: Provided at {whenProvided} ";

                    int repairStart = GetRepairStart(z, i, k);
                    if (repairStart >= 0)
                        ostr += $";; repair starts at {repairStart}";

                    bool internallyProvided = InternallyProvided(xint, i, k);
                    if (!internallyProvided)
                    {
                        Order o = orders.First(order => order.K == k);
                        ostr += $";; ordered at {o.T}";
                    }

                    ostr += "\n";
                }
            }

            Console.WriteLine(ostr);
        }

        private static int WhenProvided(GAMSVariable x, int i, int k)
        {
            foreach (var symRec in x)
            {
                var keys = symRec.Keys;
                var vrec = (GAMSVariableRecord)symRec;
                if (SymIx(keys[0]) == i && SymIx(keys[1]) == k && Math.Abs(vrec.Level - 1.0) < 0.001)
                {
                    return SymIx(keys[2]);
                }
            }

            return -1;
        }

        private static int GetRepairStart(GAMSVariable z, int i, int k)
        {
            foreach (var symRec in z)
            {
                var keys = symRec.Keys;
                if (keys[0] == $"i{i + 1}" && keys[1] == $"k{k + 1}") {
                    return int.Parse(keys[3].Replace("t", ""));
                }
            }

            return -1;
        }

        public struct Order
        {
            public int K, S, T;

            public Order(int k, int s, int t)
            {
                K = k;
                S = s;
                T = t;
            }
        }

        public struct Repair
        {
            public int I, K, S, T;

            public Repair(int i, int k, int s, int t)
            {
                I = i;
                K = k;
                S = s;
                T = t;
            }
        }

        private static List<Order> GetOrders(GAMSVariable w)
        {
            List<Order> orders = new List<Order>();

            foreach (var symRec in w)
            {
                var keys = symRec.Keys;
                var vrec = (GAMSVariableRecord) symRec;
                if (Math.Abs(vrec.Level - 1.0) < 0.001)
                {
                    orders.Add(new Order(SymIx(keys[0]), SymIx(keys[1]), SymIx(keys[2])));
                }
            }

            return orders;
        }

        private static int SymIx(string key)
        {
            return int.Parse(key.Substring(1)) - 1;
        }

        private static bool InternallyProvided(GAMSVariable xint, int i, int k)
        {
            foreach (var symRec in xint)
            {
                var keys = symRec.Keys;
                var vrec = (GAMSVariableRecord) symRec;
                if (SymIx(keys[0]) == i && SymIx(keys[1]) == k && Math.Abs(vrec.Level - 1.0) < 0.001)
                {
                    return true;
                }
            }

            return false;
        }
    }
}