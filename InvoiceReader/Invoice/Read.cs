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
    public List<dynamic> Read(string text)
    {
        int? invoiceId = this.OcrInvoices?.Id;
        _logger.Debug("Entering Read(string) for InvoiceId: {InvoiceId}. Input text length: {Length}", invoiceId, text?.Length ?? 0);
        if (text == null)
        {
             _logger.Warning("Read(string) called with null text for InvoiceId: {InvoiceId}. Returning empty list structure.", invoiceId);
             // Return expected structure but empty
             return new List<dynamic> { new List<IDictionary<string, object>>() };
        }
        // Split lines and call the primary Read method
        return Read(text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
    }

    // Primary Read method accepting list of lines
    public List<dynamic> Read(List<string> text)
    {
        int? invoiceId = this.OcrInvoices?.Id;
        string invoiceName = this.OcrInvoices?.Name ?? "Unknown";
        int inputLineCount = text?.Count ?? 0;
        _logger.Information("Starting Read(List<string>) for InvoiceId: {InvoiceId}, Name: '{InvoiceName}'. Received {LineCount} lines.",
            invoiceId, invoiceName, inputLineCount);

        // Null check input list
        if (text == null)
        {
             _logger.Error("Read(List<string>) called with null text list for InvoiceId: {InvoiceId}. Cannot proceed.", invoiceId);
             // Success property is on WaterNut.DataSpace.Invoice, not OCR.Business.Entities.Invoices
             return new List<dynamic> { new List<IDictionary<string, object>>() };
        }
         // Check if Parts collection is initialized
         if (this.Parts == null)
         {
              _logger.Error("Invoice Parts collection is null for InvoiceId: {InvoiceId}. Cannot proceed with Read.", invoiceId);
              // Success property is on WaterNut.DataSpace.Invoice, not OCR.Business.Entities.Invoices
              return new List<dynamic> { new List<IDictionary<string, object>>() };
         }


        try
        {
            var lineCount = 0;
            var section = ""; // Current section name
            _logger.Debug("Iterating through {LineCount} input text lines...", inputLineCount);
            foreach (var lineText in text)
            {
                lineCount++;
                string previousSection = section; // Store previous section for logging change

                // Determine current section based on predefined markers
                // Use safe navigation and OrdinalIgnoreCase
                Sections.Where(s => lineText != null && s.Value != null && lineText.Contains(s.Value, StringComparison.OrdinalIgnoreCase))
                        .ForEach(s => section = s.Key); // Using Core.Common.Extensions.ForEach

                if (section != previousSection)
                {
                     _logger.Verbose("Line {LineNum}: Section changed from '{PreviousSection}' to '{CurrentSection}'. Line Text: '{LineText}'", lineCount, previousSection, section, lineText);
                } else {
                     _logger.Verbose("Line {LineNum}: Processing line in section '{CurrentSection}'. Line Text: '{LineText}'", lineCount, section, lineText);
                }


                // Create InvoiceLine object (assuming constructor is simple)
                var iLine = new List<InvoiceLine>(){ new InvoiceLine(lineText, lineCount) };

                // Call Read on each Part
                _logger.Verbose("Calling Part.Read for {PartCount} parts for Line {LineNum}.", this.Parts.Count, lineCount);
                Parts.ForEach(x => { // Using Core.Common.Extensions.ForEach
                    if (x != null) {
                        // Part.Read should handle its own detailed logging
                        x.Read(iLine, section);
                    } else {
                         _logger.Warning("Skipping null Part object during Read iteration for Line {LineNum}.", lineCount);
                    }
                });
            }
             _logger.Debug("Finished iterating through text lines.");

            _logger.Debug("Calling AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}", invoiceId);
            AddMissingRequiredFieldValues(); // Handles its own logging
            _logger.Debug("Finished AddMissingRequiredFieldValues for InvoiceId: {InvoiceId}", invoiceId);

            // Check if any values were extracted at all using the Lines property (which has logging)
            if (!Lines.Any()) // Accessing Lines property triggers its getter logic and logging
            {
                 _logger.Warning("No values found in any lines after processing for InvoiceId: {InvoiceId}. Returning empty list structure.", invoiceId);
                 // Success property is on WaterNut.DataSpace.Invoice, not OCR.Business.Entities.Invoices
                 return new List<dynamic> { new List<IDictionary<string, object>>() };
            }

            _logger.Debug("Calling SetPartLineValues for {PartCount} top-level parts to assemble results for InvoiceId: {InvoiceId}", this.Parts.Count, invoiceId);
            // Assemble results by calling SetPartLineValues for each top-level part
            var ores = Parts.Select(x =>
                {
                    if (x == null) {
                         _logger.Warning("Skipping null Part during final result assembly.");
                         return new List<IDictionary<string, object>>(); // Return empty list for null part
                    }
                    // Pass null for top-level calls, indicating no instance filtering yet
                    // SetPartLineValues should handle its own logging
                    var lst = SetPartLineValues(x, null);
                    return lst;
                }
            ).ToList();
             _logger.Debug("Finished calling SetPartLineValues for all parts.");

            // Flatten the results from all parts safely
            var finalResultList = ores.SelectMany(x => x ?? Enumerable.Empty<IDictionary<string, object>>()) // Safe SelectMany
                                      .Where(d => d != null) // Filter out potential null dictionaries
                                      .ToList();

            if (!finalResultList.Any())
            {
                 _logger.Warning("No instances assembled after SetPartLineValues for InvoiceId: {InvoiceId}. Returning empty list structure.", invoiceId);
                 // Success property is on WaterNut.DataSpace.Invoice, not OCR.Business.Entities.Invoices
                 // Return the expected structure but with an empty inner list
                 return new List<dynamic> { new List<IDictionary<string, object>>() };
            }
_logger.Information("Finished Read for InvoiceId: {InvoiceId}. Returning {InstanceCount} assembled instances.", invoiceId, finalResultList.Count);
// Success property is on WaterNut.DataSpace.Invoice, not OCR.Business.Entities.Invoices
// It will be evaluated based on Part success when accessed.



            // Return the list of instances, wrapped in List<dynamic> as expected by the caller.
            return new List<dynamic> { finalResultList };
        }
        catch (Exception e)
        {
             _logger.Error(e, "Error during Read(List<string>) for InvoiceId: {InvoiceId}, Name: '{InvoiceName}'", invoiceId, invoiceName);
             // Success property is on WaterNut.DataSpace.Invoice, not OCR.Business.Entities.Invoices
             throw; // Re-throw original exception
        }
    }

     // Assuming SetPartLineValues exists in another partial class part
     // private List<IDictionary<string, object>> SetPartLineValues(Part part, int? instance) { ... }
}