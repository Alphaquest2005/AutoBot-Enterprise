// Assuming Invoice and related entities are here
// Assuming OCR_Lines and Fields are here

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement
using OCR.Business.Entities; // Assuming Fields is in this namespace

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class AddMissingRequiredFieldValuesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<AddMissingRequiredFieldValuesStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            // Use FilePath as the identifier
            _logger.Debug("Executing AddMissingRequiredFieldValuesStep for File: {FilePath}", context.FilePath);



            var res = true;
            foreach (var template in context.Templates)
            {

                // Added null checks and Any() for CsvLines
                if (template.CsvLines != null && template.CsvLines.Any())
                {
                    var requiredFieldsList = GetRequiredFieldsWithValues(context,template);
                    // Use FilePath as the identifier
                    _logger.Debug("Found {RequiredFieldCount} required fields with values for File: {FilePath}.",
                        requiredFieldsList.Count, context.FilePath);

                    AddRequiredFieldValuesToDocuments(context,template, requiredFieldsList);

                    // Replace Console.WriteLine with Serilog Information log
                    // Use FilePath as the identifier
                    _logger.Information("Added missing required field values for File: {FilePath}.", context.FilePath);

                    // Use FilePath as the identifier
                    _logger.Debug(
                        "Finished executing AddMissingRequiredFieldValuesStep successfully for File: {FilePath}",
                        context.FilePath);
                    return true; // Indicate success
                }

                // Log a warning if required data is missing
                // Use FilePath as the identifier
                _logger.Warning(
                    "Skipping AddMissingRequiredFieldValuesStep on Template:{template.OcrInvoices.Name} due to missing CsvLines for File: {FilePath}.",
                    context.FilePath);
            }
            return true; // Indicate success
        }



        private void AddRequiredFieldValuesToDocuments(InvoiceProcessingContext context, Invoice template,
            List<Fields> requiredFieldsList)
        {
             // Use FilePath as the identifier
             _logger.Debug("Starting to add required field values to documents for File: {FilePath}", context.FilePath);

            // Assuming CsvLines is List<object> where the first object is List<IDictionary<string, object>>
            // Need null checks for safety
            if (template.CsvLines == null || !template.CsvLines.Any() || !(template.CsvLines.First() is List<IDictionary<string, object>> firstDocList))
            {
                 // Use FilePath as the identifier
                _logger.Warning("CsvLines is null, empty, or not in the expected format for Template:{template} File: {FilePath}. Cannot add required fields.", context.FilePath);
                return;
            }

             // Use FilePath as the identifier
             _logger.Debug("Processing {DocumentCount} documents for File: {FilePath}.", firstDocList.Count, context.FilePath);

            foreach (var field in requiredFieldsList)
            {
                 // Use FilePath as the identifier
                 _logger.Verbose("Processing required field: {FieldName} for File: {FilePath}", field?.Field ?? "NULL_FIELD", context.FilePath); // Use Verbose for potentially high-volume logs
                 if (field == null) continue; // Skip null fields

                foreach (var doc in firstDocList)
                {
                    // Ensure doc is not null before processing
                    if (doc != null)
                    {
                        AddMissingFieldToDocument(doc, field);
                    }
                    else
                    {
                         // Use FilePath as the identifier
                        _logger.Verbose("Encountered a null document while processing field {FieldName} for File: {FilePath}, skipping.", field.Field, context.FilePath);
                    }
                }
            }
             // Use FilePath as the identifier
             _logger.Debug("Finished adding required field values to documents for File: {FilePath}", context.FilePath);
        }

        private List<Fields> GetRequiredFieldsWithValues(InvoiceProcessingContext context, Invoice template)
        {
             // Use FilePath as the identifier
            _logger.Verbose("Getting required fields with values for File: {FilePath}", context.FilePath);
            // Added null check for safety
            if (template?.Lines == null)
            {
                 // Use FilePath as the identifier
                _logger.Warning("Template or Template.Lines is null in GetRequiredFieldsWithValues for File: {FilePath}. Returning empty list.", context.FilePath);
                return new List<Fields>();
            }

            var fields = template.Lines
                .Where(line => line?.OCR_Lines?.Fields != null) // Ensure line and fields are not null
                .SelectMany(x => x.OCR_Lines.Fields)
                .Where(z => z != null && z.IsRequired && z.FieldValue != null && !string.IsNullOrEmpty(z.FieldValue.Value)) // Ensure field and FieldValue are not null
                .ToList();
             // Use FilePath as the identifier
            _logger.Verbose("Found {FieldCount} required fields with values for File: {FilePath}", fields.Count, context.FilePath);
            return fields;
        }

        private void AddMissingFieldToDocument(IDictionary<string, object> doc, Fields field)
        {
            // Added null checks for safety - doc null check happens in caller
            if (field == null || string.IsNullOrEmpty(field.Field) || field.FieldValue == null)
            {
                 _logger.Verbose("Skipping AddMissingFieldToDocument due to null field or missing FieldName/FieldValue.");
                 return;
            }

            if (!doc.ContainsKey(field.Field)) // Use ContainsKey for dictionaries
            {
                 _logger.Verbose("Adding missing field '{FieldName}' with value '{FieldValue}' to document.", field.Field, field.FieldValue.Value);
                doc.Add(field.Field, field.FieldValue.Value);
            }
            else
            {
                  _logger.Verbose("Field '{FieldName}' already exists in document, skipping.", field.Field);
            }
        }
    }
}
