namespace InstanceGeneratorProvisioning
{
    public struct SetSizes
    {
        public int ngoods, ncomponents, ndamagepatterns, nperiods;

        public int[] ToArray()
        {
            return new[] {ngoods, ncomponents, ndamagepatterns, nperiods};
        }
    }
}