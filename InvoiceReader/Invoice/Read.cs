using MoreLinq;
using Core.Common.Extensions; // Added for ForEach
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using Core.Common; // Added for BetterExpando if needed by SetPartLineValues
using OCR.Business.Entities; // Added for InvoiceLine

namespace WaterNut.DataSpace;

public partial class Invoice
{
    // Assuming _logger exists from another partial part
    // private static readonly ILogger _logger = Log.ForContext<Invoice>();

    // Overload for string input - simply splits and calls the main Read method

    // Primary Read method accepting list of lines
    public List<dynamic> Read(List<string> text)
    {
        int? invoiceId = this.OcrInvoices?.Id;
        string invoiceName = this.OcrInvoices?.Name ?? "Unknown";
        int inputLineCount = text?.Count ?? 0;
        string methodName = nameof(Read) + "(List<string>)";
        _logger.Information("Entering {MethodName} for InvoiceId: {InvoiceId}, Name: '{InvoiceName}'. Received {LineCount} lines.",
            methodName, invoiceId, invoiceName, inputLineCount);

        // --- Input Validation ---
        if (text == null)
        {
            _logger.Error("{MethodName}: Called with null text list for InvoiceId: {InvoiceId}. Cannot proceed. Returning empty structure.", methodName, invoiceId);
            var emptyResult = new List<dynamic> { new List<IDictionary<string, object>>() };
            _logger.Information("Exiting {MethodName} for InvoiceId: {InvoiceId} due to null text list.", methodName, invoiceId);
            return emptyResult;
        }
        if (this.Parts == null)
        {
            _logger.Error("{MethodName}: Invoice Parts collection is null for InvoiceId: {InvoiceId}. Cannot proceed. Returning empty structure.", methodName, invoiceId);
            var emptyResult = new List<dynamic> { new List<IDictionary<string, object>>() };
            _logger.Information("Exiting {MethodName} for InvoiceId: {InvoiceId} due to null Parts collection.", methodName, invoiceId);
            return emptyResult;
        }
        _logger.Verbose("{MethodName}: Input validation passed for InvoiceId: {InvoiceId}.", methodName, invoiceId);

        // --- Main Processing ---
        List<dynamic> finalResult = new List<dynamic> { new List<IDictionary<string, object>>() }; // Default empty structure
        try
        {
            var lineCount = 0;
            var section = ""; // Current section name
            _logger.Debug("{MethodName}: Starting iteration through {LineCount} input text lines for InvoiceId: {InvoiceId}...", methodName, inputLineCount, invoiceId);

            // --- Line Iteration ---
            foreach (var lineText in text)
            {
                lineCount++;
                string previousSection = section; // Store previous section for logging change

                // Determine current section based on predefined markers
                string detectedSectionKey = Sections.FirstOrDefault(s => lineText != null && s.Value != null && lineText.Contains(s.Value, StringComparison.OrdinalIgnoreCase)).Key;
                if (detectedSectionKey != null)
                {
                    section = detectedSectionKey;
                }
                // Log section for every line
                _logger.Verbose("{MethodName}: Line {LineNum}/{TotalLines}: Section='{CurrentSection}'. Text='{LineText}'", methodName, lineCount, inputLineCount, section, lineText);


                // Create InvoiceLine object
                var iLine = new List<InvoiceLine>() { new InvoiceLine(lineText, lineCount) };
                _logger.Verbose("{MethodName}: Line {LineNum}: Created InvoiceLine object.", methodName, lineCount);

                // Call Read on each Part
                _logger.Verbose("{MethodName}: Line {LineNum}: Calling Part.Read for {PartCount} parts.", methodName, lineCount, this.Parts.Count);
                Parts.ForEach(part => { // Using Core.Common.Extensions.ForEach
                    if (part != null)
                    {
                        // Explicitly qualify Part type due to potential namespace conflicts (CS0436 warning)
                        int? partId = ((WaterNut.DataSpace.Part)part).OCR_Part?.Id;
                        _logger.Verbose("{MethodName}: Line {LineNum}: Calling Read() on PartId: {PartId}...", methodName, lineCount, partId);
                        // Part.Read should handle its own detailed logging (entry/exit/logic)
                        part.Read(iLine, section);
                        _logger.Verbose("{MethodName}: Line {LineNum}: Finished Read() on PartId: {PartId}.", methodName, lineCount, partId);
                    }
                    else
                    {
                        _logger.Warning("{MethodName}: Line {LineNum}: Skipping null Part object during Read iteration.", methodName, lineCount);
                    }
                });
            }
            _logger.Debug("{MethodName}: Finished iterating through {LineCount} text lines for InvoiceId: {InvoiceId}.", methodName, inputLineCount, invoiceId);

            // --- Post-Iteration Processing ---
            _logger.Debug("{MethodName}: Calling AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}...", methodName, invoiceId);
            AddMissingRequiredFieldValues(); // Assumes this method handles its own logging
            _logger.Debug("{MethodName}: Finished AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}.", methodName, invoiceId);

            // Check if any values were extracted (using the Lines property which has logging)
            _logger.Verbose("{MethodName}: Checking if any lines were extracted via Lines property access...", methodName);
            if (!Lines.Any()) // Accessing Lines property triggers its getter logic and logging
            {
                _logger.Warning("{MethodName}: No values found in any lines after processing for InvoiceId: {InvoiceId}. Returning empty list structure.", methodName, invoiceId);
                // finalResult is already the empty structure
                _logger.Information("Exiting {MethodName} for InvoiceId: {InvoiceId} because no lines were extracted.", methodName, invoiceId);
                return finalResult;
            }
            _logger.Verbose("{MethodName}: Lines property indicates values were extracted.", methodName);


            // --- Result Assembly ---
            _logger.Debug("{MethodName}: Calling SetPartLineValues for {PartCount} top-level parts to assemble results for InvoiceId: {InvoiceId}...", methodName, this.Parts.Count, invoiceId);
            var ores = Parts.Select(part =>
            {
                if (part == null)
                {
                    _logger.Warning("{MethodName}: Skipping null Part during final result assembly.", methodName);
                    return new List<IDictionary<string, object>>(); // Return empty list for null part
                }
                // Explicitly qualify Part type due to potential namespace conflicts (CS0436 warning)
                int? partId = ((WaterNut.DataSpace.Part)part).OCR_Part?.Id;
                _logger.Verbose("{MethodName}: Calling SetPartLineValues for top-level PartId: {PartId} (Instance: null)...", methodName, partId);
                // Pass null for top-level calls, indicating no instance filtering yet
                // SetPartLineValues should handle its own logging
                var partResultList = SetPartLineValues(part, null);
                _logger.Verbose("{MethodName}: Finished SetPartLineValues for PartId: {PartId}. Returned {Count} items.", methodName, partId, partResultList?.Count ?? 0);
                return partResultList;
            }).ToList();
            _logger.Debug("{MethodName}: Finished calling SetPartLineValues for all {PartCount} top-level parts. Raw result list count: {OresCount}", methodName, this.Parts.Count, ores.Count);

            // Flatten the results from all parts safely
            var finalResultList = ores.SelectMany(x => x ?? Enumerable.Empty<IDictionary<string, object>>()) // Safe SelectMany
                                      .Where(d => d != null) // Filter out potential null dictionaries
                                      .ToList();
            _logger.Debug("{MethodName}: Flattened results. Final instance count: {FinalCount}", methodName, finalResultList.Count);


            if (!finalResultList.Any())
            {
                _logger.Warning("{MethodName}: No instances assembled after SetPartLineValues for InvoiceId: {InvoiceId}. Returning empty list structure.", methodName, invoiceId);
                // finalResult is already the empty structure
                _logger.Information("Exiting {MethodName} for InvoiceId: {InvoiceId} because no instances were assembled.", methodName, invoiceId);
                return finalResult;
            }

            // --- Success ---
            finalResult = new List<dynamic> { finalResultList }; // Wrap the final list
            _logger.Information("Exiting {MethodName} successfully for InvoiceId: {InvoiceId}. Returning {InstanceCount} assembled instances.", methodName, invoiceId, finalResultList.Count);
            return finalResult;
        }
        catch (Exception e)
        {
            _logger.Error(e, "{MethodName}: Unhandled exception during processing for InvoiceId: {InvoiceId}, Name: '{InvoiceName}'.", methodName, invoiceId, invoiceName);
            // Success property evaluation happens later based on Part success.
            _logger.Information("Exiting {MethodName} for InvoiceId: {InvoiceId} due to exception.", methodName, invoiceId);
            throw; // Re-throw original exception to signal failure
        }
    }

     // Assuming SetPartLineValues exists in another partial class part
     // private List<IDictionary<string, object>> SetPartLineValues(Part part, int? instance) { ... }
}