using System;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public partial class GetPdfTextStep
    {
        private static string PdfPigText(string file)
        {
            _logger.Debug("Extracting text using PdfPig for File: {FilePath}", file);
            try
            {
                var sb = new StringBuilder();
                using (var pdf = PdfDocument.Open(file))
                {
                    _logger.Verbose("Opened PDF document with {PageCount} pages for File: {FilePath}",
                        pdf.NumberOfPages,
                        file);
                    foreach (var page in pdf.GetPages())
                    {
                        _logger.Verbose(
                            "Extracting text from Page {PageNumber} using ContentOrderTextExtractor for File: {FilePath}",
                            page.Number, file);
                        var text = ContentOrderTextExtractor.GetText(page);
                        sb.AppendLine(text);
                    }
                }

                string result = sb.ToString();
                _logger.Verbose(
                    "PdfPig text extraction completed successfully for File: {FilePath}. Result Length: {Length}", file,
                    result.Length); // Changed level to Verbose
                return result;
            }
            catch (Exception ex) // Catch specific exceptions if possible (e.g., PdfDocumentInvalidPasswordException)
            {
                _logger.Error(ex, "Error during PdfPig text extraction for File: {FilePath}", file);
                // Return specific error message instead of throwing; caller will check and throw.
                return $"Error reading Ripped Text (PdfPig): {ex.Message}";
            }
        }
    }
    
}