using OCR.Business.Entities;
// Added using directive
using System.Data.Entity; // Added using directive
// Added using directive

// Added using directive

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class GetTemplatesStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            Console.WriteLine($"[OCR DEBUG] Pipeline Step: Getting templates.");

            // Need to convert List<Invoice> to IEnumerable<OcrInvoices> if necessary
            // Assuming Invoice and OcrInvoices are compatible or a mapping is needed.
            // For now, assuming direct assignment is possible or will be handled later.
            context.Templates = await GetInvoiceTemplatesAsync().ConfigureAwait(false);; // Assuming casting is possible

            return true; // Indicate success
        }

        private static async Task<List<Invoice>> GetInvoiceTemplatesAsync()
        {
            List<Invoice> templates;
            using (var ctx = new OCRContext()) // Assuming OCRContext is accessible
            {
                var ocrInvoices = await ctx.Invoices
                    .Include(x => x.Parts)
                    .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions")
                    .Include("RegEx.RegEx")
                    .Include("RegEx.ReplacementRegEx")
                    .Include("Parts.RecuringPart")
                    .Include("Parts.Start.RegularExpressions")
                    .Include("Parts.End.RegularExpressions")
                    .Include("Parts.PartTypes")
                    .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
                    .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
                    .Include("Parts.Lines.RegularExpressions")
                    .Include("Parts.Lines.Fields.FieldValue")
                    .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
                    .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
                    .Include("Parts.Lines.Fields.ChildFields.FieldValue")
                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
                    .Where(x => x.IsActive)
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId) // Assuming BaseDataModel is accessible
                                                                                                         // .Where(filter) // Need to determine how to handle the filter in the pipeline context
                    .ToListAsync() // Changed to ToListAsync for async execution
                    .ConfigureAwait(false);

                templates = ocrInvoices.Select(x => new Invoice(x)).ToList(); // Added conversion to Invoice
            }

            return templates;
        }
    }
}