using System;
// Assuming Invoice is defined here

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class AddNameSupplierStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<AddNameSupplierStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
             // Basic context validation
            if (context == null)
            {
                _logger.Error("AddNameSupplierStep executed with null context.");
                return false;
            }
             if (!context.Templates.Any())
            {
                 _logger.Warning("Skipping AddNameSupplierStep: No Templates found in context for File: {FilePath}", context.FilePath ?? "Unknown");
                 return true; // No templates to process, not a failure.
            }

            string filePath = context.FilePath ?? "Unknown";

            foreach (var template in context.Templates)
            {
                 int? templateId = template?.OcrInvoices?.Id; // Safe access
                 _logger.Debug("Executing AddNameSupplierStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

                 try // Wrap processing for each template
                 {
                     // --- Validation for this template ---
                     if (template == null || template.CsvLines == null || !template.CsvLines.Any())
                     {
                         string errorMsg = $"Skipping AddNameSupplierStep for TemplateId: {templateId} due to missing Template or CsvLines for File: {filePath}.";
                         _logger.Warning(errorMsg);
                         // This might not be a critical failure for the whole step if other templates exist.
                         // However, to align with stop-on-failure, we'll treat it as such for now.
                         // If partial success is desired, logic needs adjustment.
                         context.AddError(errorMsg);
                         continue;
                     }
                     // --- End Validation ---

                     // --- Core Logic ---
                     _logger.Debug("Checking condition to add Name/SupplierCode for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                     bool conditionMet = template.CsvLines.Count == 1 &&
                                         template.Lines != null &&
                                         template.OcrInvoices != null &&
                                         !template.Lines.All(x =>
                                             x?.OCR_Lines != null &&
                                             "Name, SupplierCode".Contains(x.OCR_Lines.Name));

                     if (conditionMet)
                     {
                         _logger.Debug("Condition met to add Name/SupplierCode for single CSV line set for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                         
                         // Check format and process
                         if (template.CsvLines.First() is List<IDictionary<string, object>> firstDocList)
                         {
                             foreach (var doc in firstDocList)
                             {
                                 if (doc == null)
                                 {
                                      _logger.Verbose("Encountered a null document within CsvLines list, skipping Name/SupplierCode addition for this doc. File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                                      continue; // Skip this specific null document
                                 }

                                 // Add SupplierCode if missing
                                 if (!doc.ContainsKey("SupplierCode"))
                                 {
                                     _logger.Verbose("Adding SupplierCode '{SupplierCode}' to document. File: {FilePath}, TemplateId: {TemplateId}", template.OcrInvoices.Name, filePath, templateId);
                                     doc.Add("SupplierCode", template.OcrInvoices.Name);
                                 }
                                 
                                 // Add Name if missing
                                 if (!doc.ContainsKey("Name"))
                                 {
                                     _logger.Verbose("Adding Name '{Name}' to document. File: {FilePath}, TemplateId: {TemplateId}", template.OcrInvoices.Name, filePath, templateId);
                                     doc.Add("Name", template.OcrInvoices.Name);
                                 }
                             }
                              _logger.Information("Added Name and/or Supplier information where missing for File: {FilePath}, TemplateId: {TemplateId}.", filePath, templateId);
                         }
                         else
                         {
                              // Log warning if format is wrong, but don't fail the whole step unless required.
                              _logger.Warning("CsvLines is not in the expected format (List<IDictionary<string, object>>) for File: {FilePath}, TemplateId: {TemplateId}. Cannot add Name/SupplierCode.", filePath, templateId);
                              // Decide if this is critical. For now, let the step succeed but log the issue.
                         }
                     }
                     else
                     {
                         _logger.Information("Condition not met or skipped adding Name/SupplierCode for File: {FilePath}, TemplateId: {TemplateId}.", filePath, templateId);
                     }
                     // --- End Core Logic ---

                     _logger.Debug("Finished processing AddNameSupplierStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Error during AddNameSupplierStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     _logger.Error(ex, errorMsg); // Log the error with exception details
                     context.AddError(errorMsg); // Add error to context
                     continue; // Stop processing immediately
                 }
            }

            // If the loop completes without any template causing a 'return false', the step is successful.
            _logger.Information("AddNameSupplierStep completed successfully for all applicable templates in File: {FilePath}.", filePath);
            return true;
        }
    }
}