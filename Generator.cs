using System;
using System.IO;
using GAMS;
using InstanceGeneratorProvisioning.Helpers;

namespace InstanceGeneratorProvisioning
{
    public class Generator
    {
        public static GAMSDatabase GenerateInstance(GAMSWorkspace ws, SetSizes sizes, int seed)
        {
            Utils.SetSeed(seed);
            var db = ws.AddDatabase();
            var setNames = new[] {"i", "k", "s", "t"};
            var setDescriptions = new[] {"Gueter", "Komponenten", "Schadensbilder", "Perioden"};
            GDXUtils.Sets1D(db, setNames, sizes.ToArray(), setDescriptions);

            var ekt = Utils.RandomValuesMatrixRowAscendingDiscrete(sizes.ngoods, sizes.ncomponents, 1, 3);
            var eks = Utils.RandomValuesMatrixDiscrete(sizes.ngoods, sizes.ncomponents, 2, 3);
            var ekreal = Utils.RandomValuesMatrixDiscrete(sizes.ngoods, sizes.ncomponents, 2, 3);

            GDXUtils.Parameter2D(db, "ekt", "i", "k", ekt);
            GDXUtils.Parameter2D(db, "eks", "i", "k", eks);
            GDXUtils.Parameter2D(db, "ekreal", "i", "k", ekreal);

            GDXUtils.Parameter1D(db, "due", "i", Utils.RandomValuesDiscrete(sizes.ngoods, 0, 20), "Liefertermin/Frist");
            GDXUtils.Parameter1D(db, "c", "i", Utils.RandomValues(sizes.ngoods, 0, 150), "Verspaetungskostensatz pro ZE");
            GDXUtils.Parameter1D(db, "rd", "k", Utils.RandomValuesDiscrete(sizes.ncomponents, 1, 4), "Remontagedauer in ZE");
            GDXUtils.Parameter1D(db, "rc", "k", Utils.RandomValuesDiscrete(sizes.ncomponents, 1, 2), "Reparaturkapazitaet in ME");
            GDXUtils.Parameter1D(db, "rmc", "k", Utils.RandomValuesDiscrete(sizes.ncomponents, 4, 6), "Remontagekapazitaet in ME");
            var housingCosts = new double[sizes.ncomponents, sizes.ndamagepatterns];
            for (var k = 0; k < housingCosts.GetUpperBound(0); k++)
                Utils.AssignRow(housingCosts, k, Utils.RandomValuesDescending(sizes.ndamagepatterns, 0.0001, 2.0));
            GDXUtils.Parameter2D(db, "hc", "k", "s", housingCosts, "Lagerkostensatz pro ZE und ME in Zustand");
            GDXUtils.Parameter2D(db, "d", "k", "s", Utils.RandomValuesMatrixRowAscendingDiscrete(sizes.ncomponents, sizes.ndamagepatterns, 0, 62), "Reparaturdauern in ZE");
            GDXUtils.Parameter2D(db, "bd", "k", "s", Utils.RandomValuesMatrixRowAscendingDiscrete(sizes.ncomponents, sizes.ndamagepatterns, 1, 4), "Bestelldauer in ZE");
            GDXUtils.Parameter2D(db, "bc", "k", "s", Utils.RandomValuesMatrixRowAscending(sizes.ncomponents, sizes.ndamagepatterns, 0.5, 16.0), "Bestellkostensatz pro ME in Zustand");

            Console.Write($"Generated instance with seed {seed} and {db.NrSymbols} symbols: ");

            return db;
        }

        public static void BatchGenerateInstances(GAMSWorkspace ws, int numInstances)
        {
            for (var i = 0; i < numInstances; i++)
            {
                using (var db = GenerateInstance(ws, new SetSizes
                {
                    ngoods = 4,
                    ncomponents = 3,
                    ndamagepatterns = 3,
                    nperiods = 1000
                }, i + 1))
                {
                    Console.WriteLine($"Writing instance {i + 1}...");
                    db.Export($"instance{i + 1}.gdx");
                }

                Console.WriteLine($"Copying instance {i + 1} to GAMS model directory..");
                File.Copy($"instance{i + 1}.gdx", $"{Config.ModelDirectoryPath}/instance{i + 1}.gdx", true);
            }
        }
    }
}