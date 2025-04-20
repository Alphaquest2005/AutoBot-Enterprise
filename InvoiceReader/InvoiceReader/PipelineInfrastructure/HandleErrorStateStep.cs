// Assuming Invoice and related entities are here
// Assuming OCR_Lines and Fields are here
// Assuming AsycudaDocumentSet is here
// Assuming Client is here

// Assuming InvoiceProcessingUtils and FileTypeManager are here

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
            if (!HasMissingRequiredData(context))
            {
                List<Line> failedlines = GetFailedLines(context);

                AddExistingFailedLines(context, failedlines);

                List<Line> allRequried = GetDistinctRequiredLines(context);

                if (failedlines.Count >= allRequried.Count && allRequried.Count > 0) return false;

                if (IsValidErrorState(context, failedlines))
                {
                    return await HandleErrorEmailPipeline(context).ConfigureAwait(false);
                }
                else
                {
                    return HandleUnsuccessfulImport();
                }
            }
            else
            {
                // Required data is missing
                return false;     
            }           
        }

        private static bool HasMissingRequiredData(InvoiceProcessingContext context)
        {
            return context.Template == null || context.CsvLines == null || context.DocSet == null || context.Client == null || string.IsNullOrEmpty(context.FilePath) || string.IsNullOrEmpty(context.EmailId) || string.IsNullOrEmpty(context.FormattedPdfText);
        }

        private static bool HandleUnsuccessfulImport()
        {
            Console.WriteLine(
                            $"[OCR DEBUG] Pipeline Step: Handled error state.");

            return false; // Indicate that the error state did not lead to a successful import
        }

        private static async Task<bool> HandleErrorEmailPipeline(InvoiceProcessingContext context)
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

        private static bool IsValidErrorState(InvoiceProcessingContext context, List<Line> failedlines)
        {
            return //_isLastTemplate && // Original code had this commented out
                            failedlines.Any() && failedlines.Count < context.Template.Lines.Count &&
                            (context.Template.Parts.First().WasStarted || !context.Template.Parts.First().OCR_Part.Start.Any()) &&
                            context.Template.Lines.SelectMany(x => x.Values.Values).Any();
        }

        private static List<Line> GetDistinctRequiredLines(InvoiceProcessingContext context)
        {
            return context.Template.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                            z.OCR_Lines.Fields.Any(f => f.IsRequired && (f.Field != "SupplierCode" && f.Field != "Name"))).ToList();
        }

        private static void AddExistingFailedLines(InvoiceProcessingContext context, List<Line> failedlines)
        {
            failedlines.AddRange(context.Template.Parts.SelectMany(z => z.FailedLines).ToList());
        }

        private static List<Line> GetFailedLines(InvoiceProcessingContext context)
        {
            // Logic from the original ErrorState method
            return context.Template.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                z.FailedFields.Any() || (z.OCR_Lines.Fields.Any(f => f.IsRequired && f.FieldValue?.Value == null) &&
                                         !z.Values.Any())).ToList();
        }
    }
}