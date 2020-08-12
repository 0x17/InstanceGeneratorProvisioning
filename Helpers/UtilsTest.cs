using NUnit.Framework;

namespace InstanceGeneratorProvisioning.Helpers
{
    [TestFixture]
    public class UtilsTest
    {
        [Test]
        public void TestRandomValues()
        {
            foreach (var val in Utils.RandomValues(3, 5, 10))
            {
                Assert.GreaterOrEqual(val, 5);
                Assert.LessOrEqual(val, 10);
            }
        }

        [Test]
        public void TestRandomValuesDescending()
        {
            var vals = Utils.RandomValuesDescending(3, 5, 10);
            for (var i = 0; i < vals.Length; i++)
            {
                var v = vals[i];
                Assert.GreaterOrEqual(v, 5);
                Assert.LessOrEqual(v, 10);
                if (i > 0) Assert.GreaterOrEqual(vals[i - 1], v);
            }
        }

        [Test]
        public void TestRandomValuesDiscrete()
        {
            foreach (var val in Utils.RandomValuesDiscrete(3, 5, 10))
            {
                Assert.GreaterOrEqual(val, 5);
                Assert.LessOrEqual(val, 10);
            }
        }

        [Test]
        public void TestRandomValuesMatrixRowAscending()
        {
            var mx = Utils.RandomValuesMatrixRowAscendingDiscrete(2, 3, 0, 62);
        }
    }
}