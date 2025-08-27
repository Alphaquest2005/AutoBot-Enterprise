using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class HandleErrorStateStep
    {
        private static List<Line> GetFailedLines(ILogger logger, Template template) // Add logger parameter
        {
            int? templateId = template?.OcrTemplates?.Id;
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetFailedLines), "Processing", "Getting initially failed lines (FailedFields or Missing Required Values).", $"TemplateId: {templateId}", "");
            // Null check
            if (template?.Lines == null)
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(GetFailedLines), "Validation", "Cannot get failed lines: Template.Lines is null.", $"TemplateId: {templateId}", "");
                return new List<Line>();
            }

            try
            {
                var failed = template.Lines
                    .Where(line => line?.OCR_Lines != null) // Ensure line and OCR_Lines not null
                    .DistinctBy(x => x.OCR_Lines.Id) // Requires MoreLinq or equivalent implementation
                    .Where(z =>
                        (z.FailedFields != null && z.FailedFields.Any()) || // Has any explicitly marked failed fields
                        (
                            (z.OCR_Lines.Fields != null && z.OCR_Lines.Fields.Any(f =>
                                f != null && f.IsRequired &&
                                f.FieldValue?.Value == null)) && // Has a required field with null value
                            (z.Values == null ||
                             !z.Values.Any()) // And has no successfully extracted values for the line
                        )
                    )
                    .ToList();
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(GetFailedLines), "Completion", "Found initially failed lines.", $"Count: {failed.Count}, TemplateId: {templateId}", new { FailedLines = failed });
                return failed;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetFailedLines), "Get initially failed lines", 0, $"Error getting failed lines for TemplateId: {templateId}. Error: {ex.Message}");
                return new List<Line>();
            }
        }
    }
}