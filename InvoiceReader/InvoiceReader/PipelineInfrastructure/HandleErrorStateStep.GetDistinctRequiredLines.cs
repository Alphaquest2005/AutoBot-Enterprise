using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public partial class HandleErrorStateStep
    {
        private static List<Line> GetDistinctRequiredLines(InvoiceProcessingContext context)
        {
            int? templateId = context?.Template?.OcrInvoices?.Id;
            _logger.Verbose("Getting distinct required lines for TemplateId: {TemplateId}", templateId);
            // Null check
            if (context?.Template?.Lines == null)
            {
                _logger.Warning(
                    "Cannot get distinct required lines: Template.Lines is null for TemplateId: {TemplateId}",
                    templateId);
                return new List<Line>();
            }

            try
            {
                var requiredLines = context.Template.Lines
                    .Where(line => line?.OCR_Lines?.Fields != null) // Ensure line, OCR_Lines, and Fields are not null
                    .DistinctBy(x => x.OCR_Lines.Id) // Requires MoreLinq or equivalent implementation
                    .Where(z => z.OCR_Lines.Fields.Any(f =>
                        f != null && f.IsRequired &&
                        (f.Field != "SupplierCode" && f.Field != "Name"))) // Ensure field is not null
                    .ToList();
                _logger.Verbose(
                    "Found {Count} distinct required lines (excluding Name/SupplierCode) for TemplateId: {TemplateId}",
                    requiredLines.Count, templateId);
                return requiredLines;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting distinct required lines for TemplateId: {TemplateId}", templateId);
                return new List<Line>();
            }
        }
    }
}