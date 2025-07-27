using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class HandleErrorStateStep
    {
        // Added string filePath parameter
        // Added string filePath parameter
        private static bool IsValidErrorState(ILogger logger, Template template, List<Line> failedlines, string filePath) // Add logger parameter
        {
            int? templateId = template?.OcrTemplates?.Id;
            // Removed line using static _context: string filePath = _context?.FilePath ?? "Unknown";
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsValidErrorState), "Validation", "Checking IsValidErrorState.", $"File: {filePath}, TemplateId: {templateId}", "");

            // Perform null checks early
            if (template?.Lines == null || template?.Parts == null || failedlines == null)
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "Validation", "IsValidErrorState check cannot proceed due to null Template.Lines, Template.Parts, or failedlines list.", $"File: {filePath}, TemplateId: {templateId}", "");
                return false;
            }

            // Evaluate conditions step-by-step and log
            bool hasFailedLines = failedlines.Any();
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsValidErrorState), "ConditionCheck", "Condition: Has Failed Lines?", $"Result: {hasFailedLines}, Count: {failedlines.Count}", "");
            if (!hasFailedLines)
            {
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ShortCircuit", "Short-circuiting IsValidErrorState: No failed lines.", $"File: {filePath}, TemplateId: {templateId}", "");
                return false; // Short-circuit if no failed lines
            }

            bool lessThanTotalLines = failedlines.Count < template.Lines.Count;
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsValidErrorState), "ConditionCheck", "Condition: Failed Lines Count < Template Lines Count?", $"FailedCount: {failedlines.Count}, TemplateCount: {template.Lines.Count}, Result: {lessThanTotalLines}", "");
            if (!lessThanTotalLines)
            {
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ShortCircuit", "Short-circuiting IsValidErrorState: Failed lines count is not less than total lines count.", $"File: {filePath}, TemplateId: {templateId}", "");
                return false; // Short-circuit
            }

            // Safely check Parts conditions
            var firstPart = template.Parts.FirstOrDefault();
            bool firstPartStartedOrNoStart = false;
            if (firstPart != null)
            {
                bool wasStarted = firstPart.WasStarted;
                // Ensure OCR_Part and Start are not null before checking Any()
                bool hasNoStartConditions = !(firstPart.OCR_Part?.Start?.Any() ?? false);
                firstPartStartedOrNoStart = wasStarted || hasNoStartConditions;
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ConditionCheck", "Condition: First Part Started OR First Part Has No Start Conditions?", $"WasStarted: {wasStarted}, HasNoStart: {hasNoStartConditions}, Result: {firstPartStartedOrNoStart}", "");
            }
            else
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ConditionCheck", "Template has no Parts. Condition 'firstPartStartedOrNoStart' evaluated as false.", $"File: {filePath}, TemplateId: {templateId}", "");
                // If no parts, this condition fails unless logic requires otherwise
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ShortCircuit", "Short-circuiting IsValidErrorState: Template has no parts.", $"File: {filePath}, TemplateId: {templateId}", "");
                return false;
            }

            if (!firstPartStartedOrNoStart)
            {
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ShortCircuit", "Short-circuiting IsValidErrorState: First part not started and has start conditions.", $"File: {filePath}, TemplateId: {templateId}", "");
                return false; // Short-circuit
            }


            // Ensure Lines and Values are not null before checking Any()
            bool hasAnyValues = template.Lines
                .Where(l => l?.Values != null) // Check line and Values not null
                .SelectMany(x => x.Values.Values) // Select the values from the dictionary
                .Any(); // Check if any values exist across all lines
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsValidErrorState), "ConditionCheck", "Condition: Any Template Line has any Values?", $"Result: {hasAnyValues}", "");
            if (!hasAnyValues)
            {
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(IsValidErrorState), "ShortCircuit", "Short-circuiting IsValidErrorState: No template line has any values.", $"File: {filePath}, TemplateId: {templateId}", "");
                return false; // Short-circuit
            }

            // Original code commented out _isLastTemplate check
            // bool isLastTemplateCheck = _isLastTemplate;
            // logger?.Verbose("Condition [IsValidErrorState]: Is Last Template? {Result} (Currently not used in logic)", isLastTemplateCheck);

            // If all checks passed
            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(IsValidErrorState), "Completion", "IsValidErrorState evaluation result: TRUE.", $"File: {filePath}, TemplateId: {templateId}", "");
            return true;
        }
    }
}