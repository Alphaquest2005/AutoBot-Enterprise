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

            foreach (var template in context.Templates)
            {


                _logger.Debug("Executing AddNameSupplierStep for File: {FilePath}", context.FilePath);

                // Added null check and Any() for CsvLines

                if (template == null || template.CsvLines == null || !template.CsvLines.Any())
                {
                    // Log a warning if required data is missing
                    _logger.Warning(
                        "Skipping AddNameSupplierStep due to missing Template or CsvLines for File: {FilePath}.",
                        context.FilePath);
                    return false;
                }

                // The original AddNameSupplier method logic is implemented here
                // Added null checks for Template.Lines and Template.OcrInvoices
                _logger.Debug("Checking condition to add Name/SupplierCode for File: {FilePath}", context.FilePath);
                if (template.CsvLines.Count == 1 && template.Lines != null &&
                    template.OcrInvoices != null &&
                    !template.Lines.All(x =>
                        x?.OCR_Lines != null &&
                        "Name, SupplierCode".Contains(x.OCR_Lines.Name))) // Added null checks in lambda
                {
                    _logger.Debug("Condition met to add Name/SupplierCode for single CSV line set for File: {FilePath}",
                        context.FilePath);
                    // Need null checks for safety
                    if (!(template.CsvLines.First() is List<IDictionary<string, object>> firstDocList))
                    {
                        _logger.Warning(
                            "CsvLines is not in the expected format (List<IDictionary<string, object>>) for File: {FilePath}. Cannot add Name/SupplierCode.",
                            context.FilePath);
                        // Decide if this is a failure or just a skip
                        _logger.Debug(
                            "Finished executing AddNameSupplierStep (skipped Name/Supplier addition due to format) for File: {FilePath}",
                            context.FilePath);
                        return true; // Assuming not a fatal error for the whole pipeline step
                    }

                    foreach (var doc in firstDocList)
                    {
                        if (doc == null)
                        {
                            _logger.Verbose(
                                "Encountered a null document, skipping Name/SupplierCode addition for File: {FilePath}",
                                context.FilePath);
                            continue;
                        }

                        // Check and add SupplierCode
                        if (!doc.ContainsKey("SupplierCode"))
                        {
                            _logger.Verbose("Adding SupplierCode '{SupplierCode}' to document for File: {FilePath}",
                                template.OcrInvoices.Name, context.FilePath);
                            doc.Add("SupplierCode", template.OcrInvoices.Name);
                        }
                        else
                        {
                            _logger.Verbose("SupplierCode already exists in document, skipping for File: {FilePath}",
                                context.FilePath);
                        }

                        // Check and add Name
                        if (!doc.ContainsKey("Name"))
                        {
                            _logger.Verbose("Adding Name '{Name}' to document for File: {FilePath}",
                                template.OcrInvoices.Name, context.FilePath);
                            doc.Add("Name", template.OcrInvoices.Name);
                        }
                        else
                        {
                            _logger.Verbose("Name already exists in document, skipping for File: {FilePath}",
                                context.FilePath);
                        }
                    }

                    _logger.Information("Added Name and Supplier information for File: {FilePath}.", context.FilePath);
                }
                else
                {
                    _logger.Debug(
                        "Condition not met or skipped adding Name/SupplierCode for File: {FilePath}. CsvLines Count: {CsvLineCount}, Template Lines Null: {IsTemplateLinesNull}, OcrInvoices Null: {IsOcrInvoicesNull}",
                        context.FilePath, template.CsvLines.Count, template.Lines == null,
                        template.OcrInvoices == null);
                    // Optionally log the result of the All() check if needed for debugging
                    _logger.Information("Skipped adding Name and Supplier information for File: {FilePath}.",
                        context.FilePath);
                }

                // Removed redundant Console.WriteLine

                _logger.Debug("Finished executing AddNameSupplierStep successfully for File: {FilePath}",
                    context.FilePath);

            }

            return true; // Indicate success
        }
    }
}