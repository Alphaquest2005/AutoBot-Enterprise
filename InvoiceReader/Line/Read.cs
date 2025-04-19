using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace;

public partial class Line
{
    public bool Read(string line, int lineNumber, string section, int instance)
    {
        try
        {
            //Instance += 1;
            var matches = Regex.Matches(line, OCR_Lines.RegularExpressions.RegEx,
                (OCR_Lines.RegularExpressions.MultiLine == true
                    ? RegexOptions.Multiline
                    : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (matches.Count == 0) return false;
            var values = new Dictionary<(Fields Fields, int Instance), string>();
            //var instance = 0;


            FormatValues(instance, matches, values);

            SaveLineValues(lineNumber, section, instance, values);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}