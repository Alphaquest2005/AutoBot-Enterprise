using System.Linq;
using System.Threading.Tasks;
using System.IO; // Needed for FileInfo
using System.Collections.Generic; // Needed for List<Line>
using OCR.Business.Entities; // Assuming Line is defined here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ConstructEmailBodyStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (string.IsNullOrEmpty(context.Error) || context.FileInfo == null || string.IsNullOrEmpty(context.TxtFile))
            {
                // Required data is missing
                return false;
            }

            // Logic from the original CreateEmail method for constructing the body
            // Assuming CommandsTxt is accessible, perhaps from InvoiceProcessingUtils
            // If not, it might need to be passed in the context or a constant here.
            // For now, assuming InvoiceProcessingUtils.CommandsTxt is accessible.
            var body = $"Hey,\r\n\r\n {context.Error}-'{context.FileInfo.Name}'.\r\n\r\n\r\n" +
                       $"{(context.FailedLines != null && context.FailedLines.Any() ? context.FailedLines.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name + "\r\n\r\n\r\n" : "")}" +
                       $"{context.FailedLines?.Select(x => $"Line:{x.OCR_Lines.Name} - RegId: {x.OCR_Lines.RegularExpressions.Id} - Regex: {x.OCR_Lines.RegularExpressions.RegEx} - Fields: {x.FailedFields.SelectMany(z => z.ToList()).SelectMany(z => z.Value.ToList()).Select(z => $"{z.Key.fields.Key} - '{z.Key.fields.Field}'").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n\r\n" + c)}").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}\r\n\r\n" +
                       "Thanks\r\n" +
                       "Thanks\r\n" +
                       $"AutoBot\r\n" +
                       $"\r\n" +
                       $"\r\n" +
                       InvoiceProcessingUtils.CommandsTxt // Assuming CommandsTxt is accessible here
                ;

            context.EmailBody = body;

            System.Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Constructed email body.");

            return true; // Indicate success
        }
    }
}