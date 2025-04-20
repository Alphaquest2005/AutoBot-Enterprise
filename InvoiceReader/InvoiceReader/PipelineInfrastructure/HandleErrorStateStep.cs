using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEntities.Business.Entities; // Assuming Invoice and related entities are here
using OCR.Business.Entities; // Assuming OCR_Lines and Fields are here
using DocumentDS.Business.Entities; // Assuming AsycudaDocumentSet is here
using EmailDownloader; // Assuming Client is here
using WaterNut.Business.Services.Utils; // Assuming InvoiceProcessingUtils and FileTypeManager are here

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class HandleErrorStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        private readonly bool _isLastTemplate; // Need to get this from somewhere, maybe context or constructor

        // Assuming _isLastTemplate is passed in the constructor
        public HandleErrorStateStep(bool isLastTemplate)
        {
            _isLastTemplate = isLastTemplate;
        }

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            if (context.Template == null || context.CsvLines == null || context.DocSet == null || context.Client == null || string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.EmailId) || string.IsNullOrEmpty(context.FormattedPdfText))
            {
                // Required data is missing
                return false;
            }

            // Logic from the original ErrorState method
            var failedlines = context.Template.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                z.FailedFields.Any() || (z.OCR_Lines.Fields.Any(f => f.IsRequired && f.FieldValue?.Value == null) &&
                                         !z.Values.Any())).ToList();

            failedlines.AddRange(context.Template.Parts.SelectMany(z => z.FailedLines).ToList());

            var allRequried = context.Template.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                z.OCR_Lines.Fields.Any(f => f.IsRequired && (f.Field != "SupplierCode" && f.Field != "Name"))).ToList();


            if (
                //---------Auto Add name and supplier code make this check redundant
                //!tmp.Parts.Any(x => x.AllLines.Any(z =>
                //    z.Values.Values.Any(v =>
                //        v.Keys.Any(k => k.fields.Field == "Name") &&
                //        v.Values.Any(kv => kv == tmp.OcrInvoices.Name))))) ||
                failedlines.Count >= allRequried.Count && allRequried.Count > 0) return false;

            if ( //_isLastTemplate && // Original code had this commented out
                failedlines.Any() && failedlines.Count < context.Template.Lines.Count &&
                (context.Template.Parts.First().WasStarted || !context.Template.Parts.First().OCR_Part.Start.Any()) &&
                context.Template.Lines.SelectMany(x => x.Values.Values).Any())
            {
                // Populate FileInfo and TxtFile in context for email pipeline
                context.FileInfo = new FileInfo(context.FilePath);
                // Assuming TxtFile was set in a previous step (e.g., WriteFormattedTextFileStep)
                // If not, it might need to be determined here or earlier.
                // For now, assuming it's available in context.TxtFile

                // Create and run the CreateEmailPipeline
                var createEmailPipeline = new CreateEmailPipeline(context);
                await createEmailPipeline.RunPipeline().ConfigureAwait(false);

                return true; // Indicate that error handling (including email) was attempted
            }

            System.Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Handled error state.");

            return false; // Indicate that the error state did not lead to a successful import
        }
    }
}