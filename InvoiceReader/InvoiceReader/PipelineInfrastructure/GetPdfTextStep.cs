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

            var ripTask = Task.Run(() =>
            {
                var txt = "------------------------------------------Ripped Text-------------------------\r\n";
                txt += PdfPigText(context.FilePath); // Now PdfPigText is in this class
                return txt;
            });

            var singleColumnTask = Task.Run(() =>
            {
                var txt =
                    "------------------------------------------Single Column-------------------------\r\n";
                txt += new PdfOcr().Ocr(context.FilePath, PageSegMode.SingleColumn);
                return txt;
            });

            var sparseTextTask = Task.Run(() =>
            {
                var txt = "------------------------------------------SparseText-------------------------\r\n";
                txt += new PdfOcr().Ocr(context.FilePath, PageSegMode.SparseText);
                return txt;
            });

            await Task.WhenAll(ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false);

            pdftxt.AppendLine(singleColumnTask.Result);
            pdftxt.AppendLine(sparseTextTask.Result);
            pdftxt.AppendLine(ripTask.Result);

            context.PdfText = pdftxt;
            return true; // Indicate success
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