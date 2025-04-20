namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class InvoiceProcessingPipeline
{
    private bool IsInitialRunUnsuccessful(bool initialRunSuccess)
    {
        string filePath = _context?.FilePath ?? "Unknown";
        _logger.Debug("Checking IsInitialRunUnsuccessful for File: {FilePath}. InitialRunSuccess: {InitialRunSuccess}",
            filePath, initialRunSuccess);

        // Check each condition and log
        if (!initialRunSuccess)
        {
            _logger.Warning("Initial run was unsuccessful (initialRunSuccess is false).");
            return true;
        }

        // Safe check for CsvLines
        if (_context.CsvLines == null)
        {
            _logger.Warning("Initial run considered unsuccessful: CsvLines is null.");
            return true;
        }

        // Safe check for CsvLines count using Any()
        if (!_context.CsvLines.Any())
        {
            _logger.Warning("Initial run considered unsuccessful: CsvLines count is 0.");
            return true;
        }

        // Check Template and Success property safely
        bool templateSuccess = _context.Template?.Success ?? false; // Default to false if Template is null
        if (!templateSuccess)
        {
            _logger.Warning(
                "Initial run considered unsuccessful: Template.Success is false (or Template is null). TemplateId: {TemplateId}",
                _context.Template?.OcrInvoices?.Id);
            return true;
        }

        _logger.Debug("Initial run conditions met for success path for File: {FilePath}", filePath);
        return false; // All conditions passed, initial run was successful enough
    }
}