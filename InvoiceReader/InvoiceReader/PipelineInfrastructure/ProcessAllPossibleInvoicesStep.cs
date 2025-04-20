using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Invoice
using System.Text; // Added for StringBuilder copy if needed

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ProcessAllPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<ProcessAllPossibleInvoicesStep>();

                public async Task<bool> Execute(InvoiceProcessingContext context)
         {
             string filePath = context?.FilePath ?? "Unknown";
             _logger.Information("Executing ProcessAllPossibleInvoicesStep for File: {FilePath}", filePath); // Changed Debug to Information for visibility
             _logger.Verbose("Context details at start of ProcessAllPossibleInvoicesStep: {@Context}", context); // Log context details

             // Null check context
             if (context == null)
             {
                  _logger.Error("ProcessAllPossibleInvoicesStep executed with null context.");
                  return false;
             }

             // Use Any() for potentially better performance and null safety
             if (context.PossibleInvoices == null || !context.PossibleInvoices.Any())
             {
                  _logger.Information("No possible invoices found in context for File: {FilePath}. Skipping template processing loop.", filePath);
                  return true; // Not an error if no possible invoices
             }

             var possibleInvoicesList = context.PossibleInvoices.ToList(); // Materialize the list
              _logger.Information("Found {Count} possible invoice templates to process for File: {FilePath}", possibleInvoicesList.Count, filePath);
              _logger.Verbose("Possible Invoices List details: {@PossibleInvoicesList}", possibleInvoicesList.Select(inv => new { Name = inv.OcrInvoices?.Name, Id = inv.OcrInvoices?.Id }).ToList()); // Log details of possible invoices

             try
             {
                 _logger.Debug("Calling ProcessInvoiceTemplatesAsync for File: {FilePath}", filePath);
                 await ProcessInvoiceTemplatesAsync(context, possibleInvoicesList).ConfigureAwait(false);
                 _logger.Debug("ProcessInvoiceTemplatesAsync completed for File: {FilePath}", filePath);


                 _logger.Information("Finished executing ProcessAllPossibleInvoicesStep for File: {FilePath}.", filePath); // Changed Debug to Information
                 return true; // Indicate that this step completed its iteration (individual template results are in context.Imports)
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error during ProcessAllPossibleInvoicesStep loop for File: {FilePath}", filePath);
                  return false; // Indicate failure of this step
             }
         }

         // Added filePath for context
         private static bool LogInvoiceProcessingCompletion(string filePath)
         {
              _logger.Information("Finished processing loop for all possible invoices for File: {FilePath}.", filePath);
             return true; // Indicate that this step completed its iteration (individual template results are in context.Imports)
         }

         private static async Task ProcessInvoiceTemplatesAsync(InvoiceProcessingContext originalContext, List<Invoice> possibleInvoicesList)
         {
              string filePath = originalContext?.FilePath ?? "Unknown";
              _logger.Information("Starting ProcessInvoiceTemplatesAsync loop for File: {FilePath}", filePath); // Changed Debug to Information

             for (int i = 0; i < possibleInvoicesList.Count; i++)
             {
                 var template = possibleInvoicesList[i];
                 bool isLastTemplate = (i == possibleInvoicesList.Count - 1);
                 int currentStepNum = i + 1;
                 int totalSteps = possibleInvoicesList.Count;

                 // Safe access to template details for logging
                 string templateName = template?.OcrInvoices?.Name ?? "Unknown Template Name";
                 int? templateId = template?.OcrInvoices?.Id;

                  _logger.Information("Processing Template {CurrentStep}/{TotalSteps}: Name: '{TemplateName}', ID: {TemplateId}, IsLast: {IsLast} for File: {FilePath}",
                     currentStepNum, totalSteps, templateName, templateId, isLastTemplate, filePath);

                 // Null check template before proceeding
                 if (template == null || template.OcrInvoices == null)
                 {
                      _logger.Warning("Skipping processing for Template {CurrentStep}/{TotalSteps} because template or OcrInvoices is null.", currentStepNum, totalSteps);
                      continue; // Skip to the next template
                 }

                 // Create a new context for each template processing pipeline to avoid interference
                  _logger.Debug("Creating new template-specific context for Template ID: {TemplateId}", templateId);
                  _logger.Verbose("Original context details before creating template-specific context: {@OriginalContext}", originalContext); // Log original context
                 var templateContext = new InvoiceProcessingContext
                 {
                     // Copy properties carefully
                     FilePath = originalContext.FilePath,
                     FileTypeId = originalContext.FileTypeId,
                     EmailId = originalContext.EmailId,
                     OverWriteExisting = originalContext.OverWriteExisting,
                     DocSet = originalContext.DocSet, // Reference copy - assuming read-only or shared state is ok
                     FileType = originalContext.FileType, // Reference copy - assuming read-only
                     Client = originalContext.Client, // Reference copy - assuming read-only
                     // PdfText is StringBuilder - reference copy. Sub-pipeline should ideally only read it.
                     // If sub-pipeline modifies PdfText, need a deep copy: new StringBuilder(originalContext.PdfText.ToString())
                     PdfText = originalContext.PdfText,
                     Template = template, // The specific template for this run
                     Templates = originalContext.Templates, // Reference copy - assuming read-only
                     PossibleInvoices = originalContext.PossibleInvoices, // Reference copy - assuming read-only
                     Imports = originalContext.Imports, // SHARED dictionary - intended for mutation
                     FormattedPdfText = originalContext.FormattedPdfText, // Reference copy (string is immutable)
                     // ImportStatus will default to 'Failed' (0) or be set by the sub-pipeline
                     // Other properties (Error, FailedLines, etc.) start null/empty
                 };
                  _logger.Verbose("Template-specific context created for Template ID: {TemplateId}: {@TemplateContext}", templateId, templateContext); // Log template-specific context

                  _logger.Information("Running sub-pipeline (InvoiceProcessingPipeline) for Template ID: {TemplateId}, File: {FilePath}", templateId, filePath);
                  // Pass the specific pipeline name for better context in logs
                 var invoiceProcessingPipeline = new InvoiceProcessingPipeline(templateContext, isLastTemplate);
                 try
                 {
                     _logger.Debug("Calling RunPipeline for sub-pipeline for Template ID: {TemplateId}", templateId);
                     bool subPipelineResult = await invoiceProcessingPipeline.RunPipeline().ConfigureAwait(false);
                     _logger.Debug("RunPipeline for sub-pipeline completed for Template ID: {TemplateId} with result: {Result}", templateId, subPipelineResult);

                      _logger.Information("Sub-pipeline finished for Template ID: {TemplateId}, File: {FilePath}. Final Status in sub-context: {ImportStatus}",
                         templateId, filePath, templateContext.ImportStatus);
                      _logger.Verbose("Sub-context after sub-pipeline run for Template ID: {TemplateId}: {@TemplateContext}", templateId, templateContext); // Log sub-context after run
                     // The result (success/failure) for this template is now in templateContext.ImportStatus
                     // and potentially updated originalContext.Imports dictionary.
                 }
                 catch (Exception ex)
                 {
                      // Log error specific to this template's pipeline run
                      _logger.Error(ex, "Error running sub-pipeline for Template ID: {TemplateId}, File: {FilePath}", templateId, filePath);
                      // Optionally update the shared Imports dictionary to reflect this failure
                      if (originalContext.Imports != null && templateId.HasValue)
                      {
                          // Ensure thread-safety if Imports could be modified concurrently (though likely not in this loop structure)
                          originalContext.Imports[templateId.Value.ToString()] = (filePath, $"Sub-pipeline failed: {ex.Message}", ImportStatus.Failed);
                           _logger.Warning("Updated shared Imports dictionary to reflect sub-pipeline failure for Template ID: {TemplateId}", templateId);
                      }
                 }
             }
              _logger.Information("Finished ProcessInvoiceTemplatesAsync loop for File: {FilePath}", filePath); // Changed Debug to Information
         }
     }
 }