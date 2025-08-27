using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using Serilog;

    public partial class HandleErrorStateStep
    {
        private static List<Line> GetDistinctRequiredLines(ILogger logger, Template template) // Add logger parameter, rename context to template
        {

            int? templateId = template?.OcrTemplates?.Id;
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetDistinctRequiredLines), "Processing", "Getting distinct required lines.", $"TemplateId: {templateId}", "");
            // Null check
            if (template?.Lines == null)
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(GetDistinctRequiredLines), "Validation", "Cannot get distinct required lines: Template.Lines is null.", $"TemplateId: {templateId}", "");
                return new List<Line>();
            }

            try
            {
                var requiredLines = template.Lines
                    .Where(line => line?.OCR_Lines?.Fields != null) // Ensure line, OCR_Lines, and Fields are not null
                    .DistinctBy(x => x.OCR_Lines.Id) // Requires MoreLinq or equivalent implementation
                    .Where(z => z.OCR_Lines.Fields.Any(f =>
                        f != null && f.IsRequired &&
                        (f.Field != "SupplierCode" && f.Field != "Name"))) // Ensure field is not null
                    .ToList();
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(GetDistinctRequiredLines), "Completion", "Found distinct required lines (excluding Name/SupplierCode).", $"Count: {requiredLines.Count}, TemplateId: {templateId}", new { RequiredLines = requiredLines });
                return requiredLines;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetDistinctRequiredLines), "Get distinct required lines", 0, $"Error getting distinct required lines for TemplateId: {templateId}. Error: {ex.Message}");
                return new List<Line>();
            }
        }
    }
}