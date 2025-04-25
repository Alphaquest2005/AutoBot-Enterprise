using System;
using System.Linq;
using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace.PipelineInfrastructure{

public partial class GetPossibleInvoicesStep
{
    public static bool IsInvoiceDocument(Invoices invoice, string fileText, string filePath)
    {
        // Invoice null check happens in caller's Where clause
        int invoiceId = invoice.Id;
        _logger.Verbose(
            "Checking if document matches Invoice Template using InvoiceId: {InvoiceId} for File: {FilePath}",
            invoiceId, filePath);

        // Check if InvoiceIdentificatonRegEx collection exists and has items
        if (invoice.InvoiceIdentificatonRegEx == null || !invoice.InvoiceIdentificatonRegEx.Any())
        {
            _logger.Warning(
                "No Invoice Identification Regex patterns found for InvoiceId: {InvoiceId}. Cannot determine if it's an invoice document based on regex.",
                invoiceId);
            return false; // Cannot match without patterns
        }

        bool isMatch = false;
        try
        {
            // Iterate through regex patterns, ensuring inner objects aren't null
            foreach (var regexInfo in invoice.InvoiceIdentificatonRegEx.Where(r => r?.OCR_RegularExpressions != null))
            {
                string pattern = regexInfo.OCR_RegularExpressions.RegEx;
                int regexId = regexInfo.OCR_RegularExpressions.Id;

                if (string.IsNullOrEmpty(pattern))
                {
                    _logger.Verbose("Skipping empty regex pattern for RegexId: {RegexId}, InvoiceId: {InvoiceId}",
                        regexId, invoiceId);
                    continue; // Skip empty patterns
                }

                _logger.Verbose(
                    "Testing RegexId: {RegexId}, Pattern: '{Pattern}' against file text for InvoiceId: {InvoiceId}",
                    regexId, pattern, invoiceId);
                // Use compiled regex options and add a timeout
                isMatch = Regex.IsMatch(fileText,
                    pattern,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture,
                    RegexTimeout); // Use defined timeout

                if (isMatch)
                {
                    _logger.Information(
                        "Regex match FOUND! RegexId: {RegexId} matched for InvoiceId: {InvoiceId}, File: {FilePath}. Document identified as this invoice type.",
                        regexId, invoiceId, filePath);
                    return true; // Exit method on first match
                }
                else
                {
                    _logger.Verbose("RegexId: {RegexId} did not match for InvoiceId: {InvoiceId}", regexId, invoiceId);
                }
            }
        }
        catch (RegexMatchTimeoutException timeoutEx)
        {
            // Log timeout specifically
            _logger.Error(timeoutEx,
                "Regex match timed out (>{TimeoutSeconds}s) while checking InvoiceId: {InvoiceId}, RegexId: {RegexId} for File: {FilePath}",
                RegexTimeout.TotalSeconds, invoiceId, "N/A",
                filePath); // Cannot easily get RegexId here if it timed out
            return false; // Treat timeout as non-match
        }
        catch (Exception ex)
        {
            // Log any other exceptions during regex processing
            _logger.Error(ex, "Error during regex matching process for InvoiceId: {InvoiceId} for File: {FilePath}",
                invoiceId, filePath);
            return false; // Treat error as non-match
        }

        // Log only if no patterns matched after checking all of them
        _logger.Debug(
            "No identifying regex patterns matched for InvoiceId: {InvoiceId}, File: {FilePath}. Document NOT identified as this invoice type.",
            invoiceId, filePath);
        return false; // No match found
    }
}
}