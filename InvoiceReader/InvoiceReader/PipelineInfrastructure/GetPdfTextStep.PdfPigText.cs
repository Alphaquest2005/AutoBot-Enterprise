using System;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class GetPdfTextStep
    {
        private static string PdfPigText(InvoiceProcessingContext context, string file) // Add context parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(PdfPigText), "Extract text from PDF using PdfPig", $"FilePath: {file}");

            try
            {
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(PdfPigText), "StartExtraction", "Extracting text using PdfPig", $"FilePath: {file}");

                var sb = new StringBuilder();
                using (var pdf = PdfDocument.Open(file))
                {
                    context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(PdfPigText), "OpenDocument", "Opened PDF document", $"PageCount: {pdf.NumberOfPages}, FilePath: {file}");
                    
                    foreach (var page in pdf.GetPages())
                    {
                        context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(PdfPigText), "ExtractPageText", "Extracting text from Page", $"PageNumber: {page.Number}, FilePath: {file}");
                        var text = ContentOrderTextExtractor.GetText(page);
                        sb.AppendLine(text);
                    }
                }
 
                string result = sb.ToString();
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(PdfPigText), "ExtractionComplete", "PdfPig text extraction completed successfully", $"FilePath: {file}, ResultLength: {result.Length}");

                methodStopwatch.Stop();
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(PdfPigText), "Successfully extracted text", $"ExtractedTextLength: {result.Length}", methodStopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(PdfPigText), "Extract text from PDF using PdfPig", methodStopwatch.ElapsedMilliseconds, ex.Message);

                // Return specific error message instead of throwing; caller will check and throw.
                return $"Error reading Ripped Text (PdfPig): {ex.Message}";
            }
        }
    }
    
}