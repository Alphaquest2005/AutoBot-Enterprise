using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Part
{
    private static Match IsMatch(string val, Start z) // Kept static IsMatch for potential reuse?
    {
        try
        {
            var options = (z.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.Singleline)
                          | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
            return Regex.Match(val, z.RegularExpressions.RegEx, options, TimeSpan.FromSeconds(2));
        }
        catch (Exception e)
        {
            Console.WriteLine(
                $"[OCR ERROR] IsMatch (Static): Exception for PartID={z?.PartId}, Pattern='{z?.RegularExpressions?.RegEx}': {e}");
            return null;
        }
    }
}