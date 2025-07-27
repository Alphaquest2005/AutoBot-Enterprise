using System.Text.RegularExpressions;
using System.Linq; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for OcrInvoices type if needed
using System.Collections.Generic; // Added for IEnumerable

namespace WaterNut.DataSpace
{
    public partial class Template
    {
        // Logger instance is defined in the main Template.cs partial class file.
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5); // Define a timeout

        public string Format(string pdftxt)
        {
            int? invoiceId = this.OcrTemplates?.Id;
            _logger.Debug("Entering Format method for InvoiceId: {InvoiceId}. Input text length: {Length}", invoiceId,
                pdftxt?.Length ?? 0);

            // Null check for critical properties
            // Use ?. operator for safe navigation
            // Corrected type guess based on namespace and common patterns
            // Corrected type guess again
            IEnumerable<TemplateRegEx> regexCollection = this.OcrTemplates?.RegEx;
            if (regexCollection == null)
            {
                _logger.Warning(
                    "Cannot format text: OcrInvoices or OcrInvoices.RegEx is null for InvoiceId: {InvoiceId}. Returning original text.",
                    invoiceId);
                return pdftxt ?? string.Empty; // Return original or empty string if null
            }

            if (pdftxt == null)
            {
                _logger.Warning(
                    "Cannot format text: Input pdftxt is null for InvoiceId: {InvoiceId}. Returning empty string.",
                    invoiceId);
                return string.Empty;
            }

            try
            {
                // Order and materialize the list for processing
                var regexList =
                    regexCollection.OrderBy(x => x?.Id ?? int.MaxValue)
                        .ToList(); // Handle potential nulls in list during sort
                _logger.Debug("Applying {Count} formatting regex patterns sequentially for InvoiceId: {InvoiceId}.",
                    regexList.Count, invoiceId);
                int appliedCount = 0;

                foreach (var reg in regexList)
                {
                    // Safe checks for regex patterns within the loop
                    string pattern = reg?.RegEx?.RegEx;
                    // Default to empty string replacement if ReplacementRegEx or its RegEx property is null
                    string replacement = reg?.ReplacementRegEx?.RegEx ?? string.Empty;
                    int regId = reg?.Id ?? -1; // Use -1 or another indicator for null reg

                    if (reg == null || reg.RegEx == null || string.IsNullOrEmpty(pattern))
                    {
                        _logger.Warning(
                            "Skipping formatting step: RegEx info or pattern is null/empty for RegId: {RegId}, InvoiceId: {InvoiceId}",
                            regId, invoiceId);
                        continue; // Skip this iteration
                    }

                    if (reg.ReplacementRegEx == null)
                    {
                        // Log if the replacement object itself is null, leading to empty string replacement
                        _logger.Warning(
                            "ReplacementRegEx object is null for RegId: {RegId}, InvoiceId: {InvoiceId}. Replacement will be empty string.",
                            regId, invoiceId);
                    }

                    _logger.Verbose(
                        "Applying RegId: {RegId} - Pattern: '{Pattern}', Replacement: '{Replacement}' for InvoiceId: {InvoiceId}",
                        regId, pattern, replacement, invoiceId);

                    try
                    {
                        // Apply Regex.Replace with timeout
                        pdftxt = Regex.Replace(pdftxt, pattern, replacement,
                            RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture,
                            RegexTimeout);
                        appliedCount++;
                        _logger.Verbose("Text after applying RegId: {RegId}. New Length: {Length}", regId,
                            pdftxt.Length);
                    }
                    catch (RegexMatchTimeoutException timeoutEx)
                    {
                        _logger.Error(timeoutEx,
                            "Regex replace timed out (>{TimeoutSeconds}s) for RegId: {RegId}, Pattern: '{Pattern}', InvoiceId: {InvoiceId}. Skipping this replacement.",
                            RegexTimeout.TotalSeconds, regId, pattern, invoiceId);
                        // Continue with the next regex
                    }
                    catch (ArgumentException argEx) // Catch invalid regex patterns
                    {
                        _logger.Error(argEx,
                            "Invalid regex pattern encountered for RegId: {RegId}, Pattern: '{Pattern}', InvoiceId: {InvoiceId}. Skipping this replacement.",
                            regId, pattern, invoiceId);
                    }
                    catch (Exception regexEx) // Catch other regex errors
                    {
                        _logger.Error(regexEx,
                            "Error applying regex replace for RegId: {RegId}, Pattern: '{Pattern}', InvoiceId: {InvoiceId}. Skipping this replacement.",
                            regId, pattern, invoiceId);
                        // Continue with the next regex
                    }
                }

                _logger.Debug(
                    "Finished formatting text for InvoiceId: {InvoiceId}. Applied {AppliedCount}/{TotalCount} patterns. Final text length: {Length}",
                    invoiceId, appliedCount, regexList.Count, pdftxt.Length);
                return pdftxt;
            }
            catch (Exception e) // Catch any unexpected errors during setup or loop
            {
                _logger.Error(e, "Unexpected error during Format method for InvoiceId: {InvoiceId}", invoiceId);
                throw; // Re-throw the original exception as per original code
            }
        }
    }
}