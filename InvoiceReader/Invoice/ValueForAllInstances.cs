namespace WaterNut.DataSpace;

public partial class Invoice
{
    private static bool ValueForAllInstances(Line x, List<(int Instance, int LineNumber)> instances)
    {
        var lst = instances.Select(x => x.Instance).ToList();
        return x.Values.SelectMany(z => z.Value.Keys.Select(k => k.instance)).Distinct().ToList().Union(lst).Count() ==
               lst.Count();
    }
}