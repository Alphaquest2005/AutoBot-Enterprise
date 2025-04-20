namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class GetPdfTextStep
{
    private static Task<string> GetRippedTextAsync(InvoiceProcessingContext context)
    {
        string filePath = context.FilePath;
        _logger.Debug("Starting Ripped Text (PdfPig) task for File: {FilePath}", filePath);
        var ripTask = Task.Run(() =>
        {
            // PdfPigText handles its own try/catch and logging, returns error string on failure
            var txt = "------------------------------------------Ripped Text-------------------------\r\n";
            _logger.Verbose("Calling PdfPigText for File: {FilePath}", filePath);
            string rippedResult = PdfPigText(filePath);
            txt += rippedResult;

            // Check if PdfPigText returned an error message and throw if it did
            if (rippedResult.StartsWith("Error reading Ripped Text (PdfPig):", StringComparison.Ordinal))
            {
                _logger.Warning("Ripped Text (PdfPig) task failed internally for File: {FilePath}. Throwing exception.",
                    filePath);
                // Throw an exception so Task.WhenAll catches it in AggregateException
                throw new Exception($"PdfPig text extraction failed for {filePath}. See previous logs.");
            }

            _logger.Information(
                "Ripped Text (PdfPig) task completed successfully for File: {FilePath}. Result Length: {Length}",
                filePath, rippedResult.Length);
            return txt;
        });
        return ripTask;
    }
}