using System;
using System.Threading.Tasks;
using pdf_ocr;
using Tesseract;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class GetPdfTextStep
    {
        private static Task<string> GetSingleColumnPdfText(InvoiceProcessingContext context)
        {
            string filePath = context.FilePath;
            _logger.Debug("Starting Single Column OCR task for File: {FilePath}", filePath);
            return Task.Run(() =>
            {
                try
                {
                    var txt = "------------------------------------------Single Column-------------------------\r\n";
                    _logger.Verbose("Executing PdfOcr().Ocr with SingleColumn for File: {FilePath}", filePath);
                    txt += new PdfOcr().Ocr(filePath, PageSegMode.SingleColumn);
                    _logger.Information(
                        "Single Column OCR task completed successfully for File: {FilePath}. Result Length: {Length}",
                        filePath, txt.Length);
                    return txt;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error during Single Column OCR task for File: {FilePath}", filePath);
                    throw; // Re-throw the exception
                }
            });
        }
    }
}