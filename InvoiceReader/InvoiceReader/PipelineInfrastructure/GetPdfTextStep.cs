using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
// Added using directive
using pdf_ocr; // Added using directive
using Tesseract; // Added using directive

// Added using directive

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class GetPdfTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            Console.WriteLine($"[OCR DEBUG] Pipeline Step: Getting PDF text for file '{context.FilePath}'");

            StringBuilder pdftxt = new StringBuilder();

            Task<string> ripTask, singleColumnTask, sparseTextTask;
            SetupPdfTextExtraction(context, out ripTask, out singleColumnTask, out sparseTextTask);

            await Task.WhenAll(ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false);

            AppendPdfTextResults(context, pdftxt, ripTask, singleColumnTask, sparseTextTask);

            return true; // Indicate success
        }

        private static void SetupPdfTextExtraction(InvoiceProcessingContext context, out Task<string> ripTask, out Task<string> singleColumnTask, out Task<string> sparseTextTask)
        {
            ripTask = GetRippedTextAsync(context);
            singleColumnTask = GetSingleColumnPdfText(context);
            sparseTextTask = GetPdfSparseTextAsync(context);
        }

        private static void AppendPdfTextResults(InvoiceProcessingContext context, StringBuilder pdftxt, Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask)
        {
            pdftxt.AppendLine(singleColumnTask.Result);
            pdftxt.AppendLine(sparseTextTask.Result);
            pdftxt.AppendLine(ripTask.Result);

            context.PdfText = pdftxt;
        }

        private static Task<string> GetPdfSparseTextAsync(InvoiceProcessingContext context)
        {
            return Task.Run(() =>
            {
                var txt = "------------------------------------------SparseText-------------------------\r\n";
                txt += new PdfOcr().Ocr(context.FilePath, PageSegMode.SparseText);
                return txt;
            });
        }

        private static Task<string> GetSingleColumnPdfText(InvoiceProcessingContext context)
        {
            return Task.Run(() =>
            {
                var txt =
                    "------------------------------------------Single Column-------------------------\r\n";
                txt += new PdfOcr().Ocr(context.FilePath, PageSegMode.SingleColumn);
                return txt;
            });
        }

        private static Task<string> GetRippedTextAsync(InvoiceProcessingContext context)
        {
            var ripTask = Task.Run(() =>
            {
                var txt = "------------------------------------------Ripped Text-------------------------\r\n";
                txt += PdfPigText(context.FilePath); // Now PdfPigText is in this class
                return txt;
            });
            return ripTask;
        }

        private static string PdfPigText(string file)
        {
            try
            {
                var sb = new StringBuilder();
                using (var pdf = PdfDocument.Open(file))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        // Either extract based on order in the underlying document with newlines and spaces.
                        var text = ContentOrderTextExtractor.GetText(page);
                        sb.AppendLine(text);
                        // // Or based on grouping letters into words.
                        // var otherText = string.Join(" ", page.GetWords());
                        // // Or the raw text of the page's content stream.
                        // var rawText = page.Text;
                        //// Console.WriteLine(text);
                    }
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                //Console.WriteLine(e);
                //throw;
            }

            return "Error readying Ripped Text";
        }
    }
}