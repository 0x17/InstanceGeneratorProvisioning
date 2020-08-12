using System;
using System.Linq;

namespace InstanceGeneratorProvisioning.Helpers
{
    public static class Utils
    {
        private static Random Rand = new Random(23);

        public static void SetSeed(int seed)
        {
            Rand = new Random(seed);
        }

        public static double[] RandomValues(int count, double min, double max)
        {
            return Enumerable.Range(0, count).Select(i => min + Rand.NextDouble() * (max - min)).ToArray();
        }

        public static double[] RandomValuesDiscrete(int count, int min, int max)
        {
            return Enumerable.Range(0, count).Select(i => (double) Rand.Next(min, max)).ToArray();
        }

        public static double[] RandomValuesDescending(int count, double min, double max)
        {
            var vals = RandomValues(count, min, max);
            return vals.OrderByDescending(v => v).ToArray();
        }

        public static double[] RandomValuesAscending(int count, double min, double max)
        {
            var vals = RandomValues(count, min, max);
            return vals.OrderBy(v => v).ToArray();
        }

        public static double[] RandomValuesAscendingDiscrete(int count, int min, int max)
        {
            var vals = RandomValuesDiscrete(count, min, max);
            return vals.OrderBy(v => v).ToArray();
        }

        public static void AssignRow<T>(T[,] matrix, int rowIndex, T[] values)
        {
            for (var i = 0; i < values.Length; i++) matrix[rowIndex, i] = values[i];
        }

        public static double[,] RandomValuesMatrixRowAscendingDiscrete(int nrows, int ncols, int min, int max)
        {
            var res = new double[nrows, ncols];
            for (var i = 0; i < nrows; i++)
                AssignRow(res, i, RandomValuesAscendingDiscrete(ncols, min, max));
            return res;
        }

        public static double[,] RandomValuesMatrixRowAscending(int nrows, int ncols, double min, double max)
        {
            var res = new double[nrows, ncols];
            for (var i = 0; i < nrows; i++)
                AssignRow(res, i, RandomValuesAscending(ncols, min, max));
            return res;
        }


        public static double[,] RandomValuesMatrixDiscrete(int nrows, int ncols, int min, int max)
        {
            var res = new double[nrows, ncols];
            for (var i = 0; i < nrows; i++)
            for (var j = 0; j < ncols; j++)
                res[i, j] = Rand.Next(min, max);
            return res;
        }
    }
}