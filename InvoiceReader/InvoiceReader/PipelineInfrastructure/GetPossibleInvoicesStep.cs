using OCR.Business.Entities;
// Added using directive
// Added using directive
using System.Text.RegularExpressions; // Added using directive

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class GetPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            Console.WriteLine($"[OCR DEBUG] Pipeline Step: Getting possible invoices.");

            // Assuming GetPossibleInvoices logic from original method
            // and IsInvoiceDocument is accessible or moved.
            
            await Task.Delay(1); // Simulate async work if the original method was fast

            context.PossibleInvoices = context.Templates
                .OrderBy(x => !x.OcrInvoices.Name.ToUpper().Contains("Tropical".ToUpper())) // Assuming OcrInvoices has a Name property
                .ThenBy(x => x.OcrInvoices.Id) // Assuming OcrInvoices has an Id property
                .Where(tmp => IsInvoiceDocument(tmp.OcrInvoices, context.PdfText.ToString())) // Now IsInvoiceDocument is in this class
                .ToList();

            return true; // Indicate success
        }

        private static bool IsInvoiceDocument(Invoices invoice, string fileText)
        {
            return invoice.InvoiceIdentificatonRegEx.Any() && invoice.InvoiceIdentificatonRegEx.Any(x =>
                Regex.IsMatch(fileText,
                    x.OCR_RegularExpressions.RegEx,
                    RegexOptions.IgnoreCase |RegexOptions.Multiline | RegexOptions.ExplicitCapture));
        }
    }
}