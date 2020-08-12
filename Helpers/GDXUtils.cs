using System;
using System.Linq;
using GAMS;

namespace InstanceGeneratorProvisioning.Helpers
{
    public class GDXUtils
    {
        public static GAMSSet[] Sets1D(GAMSDatabase db, string[] ids, int[] sizes, string[] descriptions = null)
        {
            var sets = new GAMSSet[ids.Length];
            for (var i = 0; i < ids.Length; i++) sets[i] = Set1D(db, ids[i], sizes[i], descriptions == null ? "" : descriptions[i]);
            return sets;
        }

        public static GAMSSet Set1D(GAMSDatabase db, string id, int size, string description = "")
        {
            var set = db.AddSet(id, 1, description);
            for (var i = 0; i < size; i++) set.AddRecord($"{id}{i + 1}");
            return set;
        }

        public static GAMSParameter Parameter1D(GAMSDatabase db, string id, string setId, double[] values, string description = "")
        {
            var param = db.AddParameter(id, 1, description);
            for (var i = 0; i < values.Length; i++)
            {
                var rec = param.AddRecord($"{setId}{i + 1}");
                rec.Value = values[i];
            }

            return param;
        }

        public static GAMSParameter Parameter1D(GAMSDatabase db, string id, string setId, double value, int repeatCount, string description = "")
        {
            var values = Enumerable.Repeat(value, repeatCount).ToArray();
            return Parameter1D(db, id, setId, values, description);
        }

        public static GAMSParameter Parameter2D(GAMSDatabase db, string id, string set1Id, string set2Id, double[,] values, string description = "")
        {
            var param = db.AddParameter(id, 2, description);
            for (var i = 0; i < values.GetUpperBound(0); i++)
            {
                for (var j = 0; j < values.GetUpperBound(1); j++)
                {
                    var rec = param.AddRecord($"{set1Id}{i + 1}", $"{set2Id}{j + 1}");
                    rec.Value = values[i, j];
                }
            }

            return param;
        }

        public static string GetLevelsStr(GAMSVariable v)
        {
            string[] levels = new string[v.NumberRecords];
            int ctr = 0;
            foreach (var symRec in v)
            {
                var varRec = (GAMSVariableRecord) symRec;
                levels[ctr++] = varRec.Level.ToString();
            }
            return String.Join(";", levels);
        }
    }
}