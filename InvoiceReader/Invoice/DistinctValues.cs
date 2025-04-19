using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    private Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>
        DistinctValues(
            Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>> lineValues)
    {
        var res = new Dictionary<(int lineNumber, string section), Dictionary<(Fields fields, int instance), string>>();
        foreach (var val in lineValues.Where(val =>
                     !res.Values.Any(z => z.Values.Any(q => val.Value.ContainsValue(q)))))
        {
            res.Add((val.Key.lineNumber, val.Key.section), val.Value);
        }

        return res;
    }
}