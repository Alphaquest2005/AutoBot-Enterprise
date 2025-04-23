using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class HandleErrorStateStep
    {
        private static bool HasMissingRequiredData(InvoiceProcessingContext context)
        {
            _logger.Verbose("Checking for missing required data in context.");
            // Check each property and log which one is missing if any
            // Context null check happens in Execute
            if (context.Templates == null || !context.Templates.Any())
            {
                _logger.Warning("Missing required data: Template is null.");
                return true;
            }

            if (!context.Templates.SelectMany(x => x.CsvLines).Any() )
            {
                _logger.Warning("Missing required data: CsvLines for all templates is Empty.");
                return true;
            }

            if (!context.Templates.Select(x => x.DocSet).Any())
            {
                _logger.Warning("Missing required data: DocSet is null.");
                return true;
            }

            if (context.Client == null)
            {
                _logger.Warning("Missing required data: Client is null.");
                return true;
            }

            if (string.IsNullOrEmpty(context.FilePath))
            {
                _logger.Warning("Missing required data: FilePath is null or empty.");
                return true;
            }

            if (string.IsNullOrEmpty(context.EmailId))
            {
                _logger.Warning("Missing required data: EmailId is null or empty.");
                return true;
            }
            //Todo: Create test to test this condition
            if (string.IsNullOrEmpty(context.Templates.Select(x => x.FormattedPdfText).Aggregate((o,n) => $"{o}\r\n{n}".Trim())))
            {
                _logger.Warning("Missing required data: FormattedPdfText is null or empty.");
                return true;
            }

            _logger.Verbose("No missing required data found in context.");
            return false; // All required data is present
        }
    }
}