using System;
using System.Collections.Generic;
using System.Linq;

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public partial class HandleErrorStateStep
    {
        private static void AddExistingFailedLines(Invoice template, List<Line> failedlines)
        {
          


                int? templateId = template?.OcrInvoices?.Id;
                _logger.Verbose("Adding existing failed lines from template parts for TemplateId: {TemplateId}",
                    templateId);
                // Null checks
                if (template?.Parts == null || failedlines == null)
                {
                    _logger.Warning(
                        "Cannot add existing failed lines: Template.Parts or target failedlines list is null for TemplateId: {TemplateId}",
                        templateId);
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
                    _logger.Verbose(
                        "Found {Count} existing failed lines in Template Parts to add for TemplateId: {TemplateId}",
                        countAdded, templateId);
                    if (countAdded > 0)
                    {
                        failedlines.AddRange(existingFailed);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error adding existing failed lines for TemplateId: {TemplateId}", templateId);
                }
            
        }
    }
}