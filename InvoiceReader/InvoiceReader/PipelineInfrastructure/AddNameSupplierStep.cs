using System.Threading.Tasks;
using System.Collections.Generic;
using CoreEntities.Business.Entities; // Assuming Invoice is defined here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class AddNameSupplierStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.Template == null || context.CsvLines == null)
            {
                // Required data is missing
                return false;
            }

            // The original AddNameSupplier method logic is implemented here
            if(context.CsvLines.Count == 1 && !context.Template.Lines.All(x => "Name, SupplierCode".Contains(x.OCR_Lines.Name)))
                foreach (var doc in ((List<IDictionary<string, object>>) context.CsvLines.First()))
                {
                    if (!doc.Keys.Contains("SupplierCode")) doc.Add("SupplierCode", context.Template.OcrInvoices.Name);
                    if (!doc.Keys.Contains("Name")) doc.Add("Name", context.Template.OcrInvoices.Name);
                }

             System.Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Added Name and Supplier information.");


            return true; // Indicate success
        }
    }
}