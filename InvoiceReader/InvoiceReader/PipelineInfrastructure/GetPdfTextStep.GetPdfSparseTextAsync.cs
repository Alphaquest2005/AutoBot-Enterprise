using pdf_ocr;
using Tesseract;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class GetPdfTextStep
{
    private static Task<string> GetPdfSparseTextAsync(InvoiceProcessingContext context)
    {
        string filePath = context.FilePath; // Null/empty check done in Execute
        _logger.Debug("Starting Sparse Text OCR task for File: {FilePath}", filePath);
        return Task.Run(() =>
        {
            try
            {
                var txt = "------------------------------------------SparseText-------------------------\r\n";
                _logger.Verbose("Executing PdfOcr().Ocr with SparseText for File: {FilePath}", filePath);
                // Assuming PdfOcr().Ocr might throw exceptions
                txt += new PdfOcr().Ocr(filePath, PageSegMode.SparseText);
                _logger.Information(
                    "Sparse Text OCR task completed successfully for File: {FilePath}. Result Length: {Length}",
                    filePath, txt.Length);
                return txt;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during Sparse Text OCR task for File: {FilePath}", filePath);
                // Throw exception to be caught by AggregateException handler in Execute
                throw; // Re-throw the exception
            }
        });
    }
}