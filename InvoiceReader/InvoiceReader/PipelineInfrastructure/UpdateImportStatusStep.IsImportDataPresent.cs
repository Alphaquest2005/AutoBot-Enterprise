using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class UpdateImportStatusStep
    {
        private static bool IsImportDataPresent(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsImportDataPresent), "Validation", "Checking if required data is present for import status update.", "", "");
            // Check each property and log which one is missing if any
            // Context null check happens in Execute
            if (!context.MatchedTemplates.Any())
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsImportDataPresent), "Validation", "Required data missing: Templates collection is null or empty.", "", "");
                return false;
            }

            // Check Template.OcrInvoices as it's used later
            // Corrected logging for null OcrInvoices
            foreach (var template in context.MatchedTemplates)
            {
                if (template?.OcrTemplates == null)
                {
                    logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(IsImportDataPresent), "Validation", "Required data missing: A template's OcrInvoices is null.", $"TemplateId: {template?.OcrTemplates?.Id ?? -1}", "");
                    // Decide if this should cause the whole step to fail or just skip this template.
                    // Based on Execute logic, missing data for a template causes 'continue', so return false here.
                    return false;
                }
            }
            

            if (string.IsNullOrEmpty(context.FilePath))
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsImportDataPresent), "Validation", "Required data missing: FilePath is null or empty.", "", "");
                return false;
            }

            if (context.Imports == null)
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsImportDataPresent), "Validation", "Required data missing: Imports dictionary is null.", "", "");
                return false;
            }
            // ImportStatus itself is read from the context, but its absence isn't checked here,
            // ProcessImportFile handles the default case (likely 'Failed' if not set previously).

            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsImportDataPresent), "Validation", "Required data is present for import status update.", "", "");
            return true; // All required data is present
        }
    }
}