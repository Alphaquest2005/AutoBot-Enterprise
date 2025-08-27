using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class HandleErrorStateStep
    {
        private static void AddExistingFailedLines(ILogger logger, Template template, List<Line> failedlines) // Add logger parameter
        {
          


                int? templateId = template?.OcrTemplates?.Id;
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(AddExistingFailedLines), "Processing", "Adding existing failed lines from template parts.", $"TemplateId: {templateId}", "");
                // Null checks
                if (template?.Parts == null || failedlines == null)
                {
                    logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(AddExistingFailedLines), "Validation", "Cannot add existing failed lines: Template.Parts or target failedlines list is null.", $"TemplateId: {templateId}", "");
                    return;
                }

                try
                {
                    var existingFailed = template.Parts
                        .Where(part => part?.FailedLines != null) // Check part and FailedLines are not null
                        .SelectMany(z => z.FailedLines)
                        .Where(line => line != null) // Ensure individual lines are not null
                        .ToList();

                    int countAdded = existingFailed.Count;
                    logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(AddExistingFailedLines), "Processing", "Found existing failed lines in Template Parts to add.", $"Count: {countAdded}, TemplateId: {templateId}", new { ExistingFailedLines = existingFailed });
                    if (countAdded > 0)
                    {
                        failedlines.AddRange(existingFailed);
                        logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(AddExistingFailedLines), "Completion", "Added existing failed lines to the list.", $"TotalFailedLinesCount: {failedlines.Count}", "");
                    }
                    else
                    {
                        logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(AddExistingFailedLines), "Completion", "No existing failed lines found in Template Parts to add.", $"TemplateId: {templateId}", "");
                    }
                }
                catch (Exception ex)
                {
                    logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(AddExistingFailedLines), "Add existing failed lines from template parts", 0, $"Error adding existing failed lines for TemplateId: {templateId}. Error: {ex.Message}");
                }
            
        }
    }
}