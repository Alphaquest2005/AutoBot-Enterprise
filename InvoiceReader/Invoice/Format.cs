using System.Text.RegularExpressions;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    public string Format(string pdftxt)
    {
        try
        {
            foreach (var reg in OcrInvoices.RegEx.OrderBy(x => x.Id))
            {
                pdftxt = Regex.Replace(pdftxt, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            }

            return pdftxt;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}