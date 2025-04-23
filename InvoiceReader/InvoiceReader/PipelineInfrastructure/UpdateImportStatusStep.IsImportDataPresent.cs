using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class UpdateImportStatusStep
    {
        private static bool IsImportDataPresent(InvoiceProcessingContext context)
        {
            _logger.Verbose("Checking if required data is present for import status update.");
            // Check each property and log which one is missing if any
            // Context null check happens in Execute
            if (!context.Templates.Any())
            {
                _logger.Warning("Required data missing for status update: Template is null.");
                return false;
            }

            // Check Template.OcrInvoices as it's used later
            context.Templates.Where(x => x.OcrInvoices == null)
                .Select<Invoice, object>(x =>
                {
                    _logger.Warning("Required data missing for status update: {x.OcrInvoices.Name}.OcrInvoices is null.");
                    return null;

                });
            

            if (string.IsNullOrEmpty(context.FilePath))
            {
                _logger.Warning("Required data missing for status update: FilePath is null or empty.");
                return false;
            }

            if (context.Imports == null)
            {
                _logger.Warning("Required data missing for status update: Imports dictionary is null.");
                return false;
            }
            // ImportStatus itself is read from the context, but its absence isn't checked here,
            // ProcessImportFile handles the default case (likely 'Failed' if not set previously).

            _logger.Verbose("Required data is present for import status update.");
            return true; // All required data is present
        }
    }
}