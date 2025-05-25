using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class HandleErrorStateStep
    {
        private static bool HasMissingRequiredData(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HasMissingRequiredData), "Validation", "Checking for missing required data in context.", "", "");
            // Check each property and log which one is missing if any
            // Context null check happens in Execute
            if (!context.MatchedTemplates.Any())
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: Templates collection is null or empty.", "", "");
                return true;
            }

            if (context.MatchedTemplates.Select(x => x.CsvLines).All(x => x == null))
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: CsvLines for all templates is Empty.", "", "");
                return true;
            }

            if (!context.MatchedTemplates.Select(x => x.DocSet).Any())
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: DocSet is null.", "", "");
                return true;
            }

            if (context.Client == null)
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: Client is null.", "", "");
                return true;
            }

            if (string.IsNullOrEmpty(context.FilePath))
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: FilePath is null or empty.", "", "");
                return true;
            }

            if (string.IsNullOrEmpty(context.EmailId))
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: EmailId is null or empty.", "", "");
                return true;
            }
            //Todo: Create test to test this condition
            if (context.MatchedTemplates.Select(x => x.FormattedPdfText).All(string.IsNullOrEmpty)) // Simplified check
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(HasMissingRequiredData), "Validation", "Missing required data: FormattedPdfText is null or empty for all templates.", "", "");
                return true;
            }

            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(HasMissingRequiredData), "Validation", "No missing required data found in context.", "", "");
            return false; // All required data is present
        }
    }
}