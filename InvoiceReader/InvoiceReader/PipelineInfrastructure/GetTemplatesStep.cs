using OCR.Business.Entities;
using System.Data.Entity; // Added using directive
using Core.Common; // Added for BaseDataModel
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class GetTemplatesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<GetTemplatesStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown"; // Use FilePath for context if available
            _logger.Debug("Executing GetTemplatesStep for File: {FilePath}", filePath);

            // Null check for context
            if (context == null)
            {
                 _logger.Error("GetTemplatesStep executed with null context.");
                 return false;
            }

             _logger.Information("Getting invoice templates for File: {FilePath}", filePath);

            try
            {
                _logger.Debug("Calling GetInvoiceTemplatesAsync for File: {FilePath}", filePath);
                // GetInvoiceTemplatesAsync handles its own logging
                var templates = await GetInvoiceTemplatesAsync().ConfigureAwait(false);
                _logger.Debug("GetInvoiceTemplatesAsync returned {TemplateCount} templates for File: {FilePath}", templates?.Count ?? 0, filePath);

                context.Templates = templates; // Assign the retrieved List<Invoice>
                _logger.Information("Successfully retrieved and assigned {TemplateCount} invoice templates to context for File: {FilePath}", templates?.Count ?? 0, filePath);

                 _logger.Debug("Finished executing GetTemplatesStep successfully for File: {FilePath}", filePath);
                return true; // Indicate success
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error executing GetTemplatesStep for File: {FilePath}", filePath);
                 context.Templates = new List<Invoice>(); // Ensure Templates is initialized even on error
                 return false; // Indicate failure
            }
        }

        private static async Task<List<Invoice>> GetInvoiceTemplatesAsync()
        {
             _logger.Debug("Starting GetInvoiceTemplatesAsync to retrieve templates from database.");
             int? appSettingsId = null;
             try
             {
                 // Safely get ApplicationSettingsId
                 appSettingsId = BaseDataModel.Instance?.CurrentApplicationSettings?.ApplicationSettingsId;
                 if (appSettingsId == null)
                 {
                     _logger.Error("Cannot retrieve templates: CurrentApplicationSettings or its Id is null.");
                     return new List<Invoice>(); // Return empty list if settings are missing
                 }
                  _logger.Debug("Target ApplicationSettingsId: {AppSettingsId}", appSettingsId);
             }
             catch (Exception settingsEx)
             {
                  _logger.Error(settingsEx, "Error accessing BaseDataModel ApplicationSettingsId.");
                  return new List<Invoice>(); // Return empty list on error
             }


            List<Invoice> templates = null;
            List<Invoices> ocrInvoices = null;
            try
            {
                using (var ctx = new OCRContext()) // Assuming OCRContext is accessible
                {
                     _logger.Information("Querying database (OCRContext) for active invoice templates for AppSettingsId: {AppSettingsId}", appSettingsId);
                     _logger.Verbose("Includes: Parts, InvoiceIdentificatonRegEx.*, RegEx.*, Parts.RecuringPart, Parts.Start.*, Parts.End.*, Parts.PartTypes, Parts.ChildParts.*, Parts.ParentParts.*, Parts.Lines.*, Parts.Lines.Fields.*, Parts.Lines.Fields.ChildFields.*");

                    // Build the query - Consider replacing string includes with lambda expressions for type safety if possible
                    var query = ctx.Invoices
                        .Include(x => x.Parts)
                        .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions") // String includes might be less performant/safe
                        .Include("RegEx.RegEx")
                        .Include("RegEx.ReplacementRegEx")
                        .Include("Parts.RecuringPart")
                        .Include("Parts.Start.RegularExpressions")
                        .Include("Parts.End.RegularExpressions")
                        .Include("Parts.PartTypes")
                        .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
                        .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
                        .Include("Parts.Lines.RegularExpressions")
                        .Include("Parts.Lines.Fields.FieldValue")
                        .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
                        .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
                        .Include("Parts.Lines.Fields.ChildFields.FieldValue")
                        .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
                        .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
                        .Where(x => x.IsActive)
                        .Where(x => x.ApplicationSettingsId == appSettingsId.Value); // Use the retrieved value

                     _logger.Debug("Executing database query asynchronously.");
                    ocrInvoices = await query.ToListAsync().ConfigureAwait(false); // Execute async
                     _logger.Information("Database query completed. Retrieved {OcrInvoiceCount} active OcrInvoices entities for AppSettingsId: {AppSettingsId}", ocrInvoices?.Count ?? 0, appSettingsId);
                } // Context disposed here

                // Conversion step
                if (ocrInvoices != null)
                {
                     _logger.Debug("Converting {OcrInvoiceCount} OcrInvoices entities to Invoice objects.", ocrInvoices.Count);
                     // Assuming Invoice constructor handles the mapping and potential errors
                     templates = ocrInvoices.Select(x => new Invoice(x)).ToList();
                     _logger.Information("Successfully converted {OcrInvoiceCount} OcrInvoices to {InvoiceCount} Invoice objects.", ocrInvoices.Count, templates?.Count ?? 0);
                }
                else
                {
                     _logger.Warning("OcrInvoices list was null after database query. Returning empty template list.");
                     templates = new List<Invoice>();
                }
            }
            catch (Exception dbEx)
            {
                 _logger.Error(dbEx, "Error during database query or conversion in GetInvoiceTemplatesAsync for AppSettingsId: {AppSettingsId}", appSettingsId);
                 return new List<Invoice>(); // Return empty list on error
            }

             _logger.Debug("Finished GetInvoiceTemplatesAsync. Returning {TemplateCount} templates.", templates?.Count ?? 0);
            return templates ?? new List<Invoice>(); // Ensure non-null return
        }
    }
}