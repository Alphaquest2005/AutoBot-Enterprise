using MoreLinq;

namespace WaterNut.DataSpace;

public partial class Invoice
{
    public List<dynamic> Read(string text)
    {
        return Read(text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
    }

    public List<dynamic> Read(List<string> text)
    {
        Console.WriteLine($"[OCR DEBUG] Invoice.Read: Starting read for Invoice ID {OcrInvoices.Id}, Name '{OcrInvoices.Name}'. Received {text.Count} lines.");
        try
        {


            var lineCount = 0;
            var section = "";
            foreach (var line in text)
            {
                    
                Sections.ForEach(s =>
                {
                    if (line.Contains(s.Value)) section = s.Key;
                });

                lineCount += 1;
                var iLine = new List<InvoiceLine>(){ new InvoiceLine(line, lineCount) };
                Parts.ForEach(x => x.Read(iLine, section)); // Part.Read will log its own entry
            }

            AddMissingRequiredFieldValues();

            if (!this.Lines.SelectMany(x => x.Values.Values).Any()) return new List<dynamic>();//!Success



            var ores = Parts.Select(x =>
                {
                    // Pass null for top-level calls, indicating no instance filtering yet
                    var lst = SetPartLineValues(x, null);
                    return lst;
                }
            ).ToList();

                
            // ores contains results from SetPartLineValues for each top-level part.
            // Each item in ores is a List<IDictionary<string, object>> representing instances found for that part.
            var finalResultList = ores.SelectMany(x => x.ToList()).ToList(); // This should now be a list of correctly formed invoice instances.

            if (!finalResultList.Any())
            {
                Console.WriteLine($"[OCR DEBUG] Invoice.Read: No instances found for Invoice ID {OcrInvoices.Id}. Returning empty list structure.");
                // Return the expected structure but with an empty inner list
                return new List<dynamic> { new List<IDictionary<string, object>>() };
            }

            Console.WriteLine($"[OCR DEBUG] Invoice.Read: Finished read for Invoice ID {OcrInvoices.Id}. Returning {finalResultList.Count} assembled instances.");

            // Return the list of instances, wrapped in List<dynamic> as expected by the caller.
            // The caller expects `csvLines.First()` to be the `List<IDictionary<string, object>>`
            return new List<dynamic> { finalResultList };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}