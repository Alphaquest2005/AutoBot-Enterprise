using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public partial class InvoiceProcessingPipeline
    {
        private bool IsInitialRunUnsuccessful(bool initialRunSuccess)
        {
            string filePath = _context?.FilePath ?? "Unknown";
            _logger.Debug(
                "Checking IsInitialRunUnsuccessful for File: {FilePath}. InitialRunSuccess: {InitialRunSuccess}",
                filePath, initialRunSuccess);

            // Check each condition and log
            if (!initialRunSuccess)
            {
                _logger.Warning("Initial run was unsuccessful (initialRunSuccess is false).");
                
                return true;
            }

            // Safe check for CsvLines
            if (_context.Templates == null || !_context.Templates.Any())
            {
                _logger.Warning("Initial run considered unsuccessful: CsvLines is null.");
                _context.AddError("Initial run considered unsuccessful: No Templates!");
                return true;
            }

            // Safe check for CsvLines count using Any()
            if (_context.Templates.All(x => x.CsvLines == null || !x.CsvLines.Any()))
            {
                _logger.Warning($"Initial run considered unsuccessful: CsvLines count is 0.");
                _context.AddError("Initial run considered unsuccessful: No Imports!");
                return true;
            }


            // The success of the initial steps (including text formatting and reading)
            // is already captured by initialRunSuccess and the check for CsvLines.
            // The check for Template.Success is based on the old data model and is removed.

            _logger.Debug("Initial run conditions met for success path for File: {FilePath}", filePath);
            return false; // All conditions passed, initial run was successful enough
        }
    }
}