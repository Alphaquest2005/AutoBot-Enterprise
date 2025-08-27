using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        private static bool ValueForAllInstances(Line line, List<(string Instance, int LineNumber)> instances)
        {
            var lst = instances.Select(x => x.Instance).ToList();
            return line.Values.SelectMany(z => z.Value.Keys.Select(k => k.Instance)).Distinct().ToList().Union(lst)
                       .Count() ==
                   lst.Count();
        }
    }
}