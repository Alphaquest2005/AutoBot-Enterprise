using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public partial class HandleErrorStateStep
    {
        private static List<Line> GetFailedLines(InvoiceProcessingContext context)
        {
            int? templateId = context?.Template?.OcrInvoices?.Id;
            _logger.Verbose(
                "Getting initially failed lines (FailedFields or Missing Required Values) for TemplateId: {TemplateId}",
                templateId);
            // Null check
            if (context?.Template?.Lines == null)
            {
                _logger.Warning("Cannot get failed lines: Template.Lines is null for TemplateId: {TemplateId}",
                    templateId);
                return new List<Line>();
            }

            try
            {
                var failed = context.Template.Lines
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
                _logger.Verbose("Found {Count} initially failed lines for TemplateId: {TemplateId}", failed.Count,
                    templateId);
                return failed;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting failed lines for TemplateId: {TemplateId}", templateId);
                return new List<Line>();
            }
        }
    }
}