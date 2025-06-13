// File: OCRCorrectionService/OCRCorrectionService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // Keep for any direct JSON ops if needed, though prompts are now separate
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity; // For OCRContext if used directly for simple lookups
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For OCRContext, Invoice, OcrInvoices etc.
using TrackableEntities; // For TrackingState
using Serilog;
using InvoiceReader.OCRCorrectionService; // For DatabaseValidator
// WaterNut.Business.Services.Utils; // If used, ensure it's specific
// Core.Common.Extensions; // If used, ensure it's specific
// System.IO is used in a using directive block below

namespace WaterNut.DataSpace
{
    using System.IO;

    using WaterNut.Business.Services.Utils; // For File operations, typically for reading text files

    /// <summary>
    /// Service for handling OCR error corrections using DeepSeek LLM analysis.
    /// Enhanced with comprehensive product validation, regex learning, and omission handling.
    /// This is the main part of the partial class, orchestrating calls to other specialized parts.
    /// </summary>
    public partial class OCRCorrectionService : IDisposable
    {
        #region Fields and Properties

        private readonly DeepSeekInvoiceApi _deepSeekApi; // For making calls to DeepSeek
        private readonly ILogger _logger;
        private bool _disposed = false;
        private DatabaseUpdateStrategyFactory _strategyFactory; // Factory for DB update strategies

        // Configuration properties for DeepSeek calls (if not managed by DeepSeekInvoiceApi itself)
        public double DefaultTemperature { get; set; } = 0.1; // Lower temp for more deterministic corrections
        public int DefaultMaxTokens { get; set; } = 4096; // Max tokens for LLM responses

        #endregion

        #region Constructor

        public OCRCorrectionService(ILogger logger = null)
        {
            _logger = logger ?? Log.Logger.ForContext<OCRCorrectionService>();
            _deepSeekApi = new DeepSeekInvoiceApi(_logger); // Pass the logger to DeepSeekInvoiceApi
            _strategyFactory = new DatabaseUpdateStrategyFactory(_logger); // Initialize the strategy factory
        }

        #endregion

        #region Public Orchestration Methods

        /// <summary>
        /// Corrects a single invoice using comprehensive DeepSeek analysis, applies changes, and updates DB patterns.
        /// </summary>
        public async Task<bool> CorrectInvoiceAsync(ShipmentInvoice invoice, string fileText)
        {
            // **DATAFLOW_ENTRY**: Log the exact parameters passed to this method
            _logger.Information("🔍 **DATAFLOW_CORRECT_INVOICE_ENTRY**: InvoiceNo={InvoiceNo} | FileText_Length={FileTextLength} | FileText_IsNull={FileTextIsNull}", 
                invoice?.InvoiceNo, fileText?.Length ?? 0, string.IsNullOrEmpty(fileText));
            _logger.Information("🔍 **DATAFLOW_CORRECT_INVOICE_GIFTCARD**: FileText contains 'Gift Card'? {ContainsGiftCard}", 
                fileText?.Contains("Gift Card") == true);
                
            if (invoice == null || string.IsNullOrEmpty(fileText))
            {
                _logger.Warning("CorrectInvoiceAsync: Null invoice or empty file text for invoice ID (if available): {InvoiceNo}.", invoice?.InvoiceNo ?? "N/A");
                return false;
            }
            _logger.Information("Starting OCR correction process for invoice {InvoiceNo}.", invoice.InvoiceNo);

            try
            {
                // 1. Attempt to extract/build OCRFieldMetadata for context.
                // This is a simplified metadata extraction for a single invoice without full template context.
                // In a full system, `invoice` might already be associated with rich metadata from its initial parsing.
                var metadata = this.ExtractFullOCRMetadata(invoice, fileText); // Calls helper in this file (which could use OCRMetadataExtractor logic)

                // 2. Detect errors using the comprehensive error detection logic.
                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false); // From OCRErrorDetection.cs
                if (!errors.Any())
                {
                    _logger.Information("No significant OCR errors detected for invoice {InvoiceNo}. Final check with TotalsZero.", invoice.InvoiceNo);
                    return OCRCorrectionService.TotalsZero(invoice, _logger); // Use static TotalsZero from LegacySupport
                }
                _logger.Information("Detected {ErrorCount} potential errors/omissions for invoice {InvoiceNo}.", errors.Count, invoice.InvoiceNo);

                // 3. Apply the detected corrections/omissions to the in-memory ShipmentInvoice object.
                var appliedCorrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText, metadata).ConfigureAwait(false); // From OCRCorrectionApplication.cs
                var successfulValueApplications = appliedCorrections.Count(c => c.Success);
                _logger.Information("Applied {SuccessCount}/{TotalCount} value/format corrections to in-memory invoice {InvoiceNo}.",
                    successfulValueApplications, appliedCorrections.Count, invoice.InvoiceNo);

                // 3.5. Apply Caribbean customs business rules after standard OCR corrections
                var customsCorrections = this.ApplyCaribbeanCustomsRules(invoice, appliedCorrections.Where(c => c.Success).ToList());
                if (customsCorrections.Any())
                {
                    this.ApplyCaribbeanCustomsCorrectionsToInvoice(invoice, customsCorrections);
                    appliedCorrections.AddRange(customsCorrections); // Add customs corrections to the results
                    _logger.Information("Applied {CustomsCount} Caribbean customs rule corrections to invoice {InvoiceNo}.", 
                        customsCorrections.Count, invoice.InvoiceNo);
                }

                // 4. If any corrections were successfully applied to the object, update related database patterns.
                var successfulCorrectionResultsForDB = appliedCorrections.Where(c => c.Success).ToList();
                if (successfulCorrectionResultsForDB.Any())
                {
                    var regexUpdateRequests = successfulCorrectionResultsForDB.Select(c => CreateRegexUpdateRequest(c, fileText, metadata, invoice.Id)).ToList();
                    await this.UpdateRegexPatternsAsync(regexUpdateRequests).ConfigureAwait(false); // From OCRDatabaseUpdates.cs
                }

                // Return true if any corrections were successfully made OR if the invoice is now balanced.
                return successfulValueApplications > 0 || OCRCorrectionService.TotalsZero(invoice, _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during CorrectInvoiceAsync for {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        /// <summary>
        /// Corrects a list of ShipmentInvoice objects, assuming they all relate to the same source text file.
        /// </summary>
        public async Task<List<ShipmentInvoice>> CorrectInvoicesAsync(List<ShipmentInvoice> invoices, string singleDroppedFilePath)
        {
            if (invoices == null || !invoices.Any())
            {
                _logger.Information("CorrectInvoicesAsync: No invoices provided for batch correction.");
                return invoices ?? new List<ShipmentInvoice>();
            }
            _logger.Information("Starting batch correction for {InvoiceCount} invoices using text from {FilePath}.", invoices.Count, singleDroppedFilePath);

            string fileText = null;
            var txtFilePath = singleDroppedFilePath + ".txt"; // Common convention
            if (File.Exists(txtFilePath))
            {
                fileText = File.ReadAllText(txtFilePath);
            }
            else
            {
                _logger.Warning("Source text file not found: {TxtFilePath}. Cannot perform detailed corrections for batch.", txtFilePath);
                return invoices; // Return original list if no text context
            }

            var invoicesToProcess = invoices.Where(inv => inv != null).ToList();
            _logger.Information("Processing {ProcessCount} non-null invoices from batch.", invoicesToProcess.Count);

            foreach (var invoice in invoicesToProcess)
            {
                try
                {
                    _logger.Information("Processing invoice {InvoiceNo} from batch.", invoice.InvoiceNo);
                    // CorrectInvoiceWithRegexUpdatesAsync handles error detection, value application, and DB pattern updates.
                    await CorrectInvoiceWithRegexUpdatesAsync(invoice, fileText).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error processing invoice {InvoiceNo} in batch correction.", invoice.InvoiceNo);
                    // Continue with the next invoice
                }
            }
            return invoices; // Return the list; some invoices may have been modified.
        }

        /// <summary>
        /// Corrects a single invoice with comprehensive validation and subsequent regex/database updates.
        /// </summary>
        public async Task<bool> CorrectInvoiceWithRegexUpdatesAsync(ShipmentInvoice invoice, string fileText)
        {
            if (invoice == null || string.IsNullOrEmpty(fileText))
            {
                _logger.Warning("CorrectInvoiceWithRegexUpdatesAsync: Null invoice or empty file text for {InvoiceNo}.", invoice?.InvoiceNo ?? "N/A");
                return false;
            }
            _logger.Information("Starting comprehensive OCR correction with DB updates for invoice {InvoiceNo}.", invoice.InvoiceNo);

            try
            {
                // 1. Extract/Build Metadata
                var metadata = this.ExtractFullOCRMetadata(invoice, fileText); // Uses helper in this file

                // 2. Detect Errors (including omissions)
                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false); // From OCRErrorDetection.cs
                if (!errors.Any())
                {
                    _logger.Information("No significant errors detected for comprehensive check of invoice {InvoiceNo}. Validating totals.", invoice.InvoiceNo);
                    return OCRCorrectionService.TotalsZero(invoice, _logger); // From LegacySupport
                }
                _logger.Information("Detected {ErrorCount} errors/omissions for comprehensive check of invoice {InvoiceNo}: {ErrorSummary}",
                    errors.Count, invoice.InvoiceNo, string.Join(", ", errors.Select(e => $"{e.Field}({e.ErrorType})")));

                // 3. Apply Corrections to In-Memory Invoice Object
                var appliedCorrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText, metadata).ConfigureAwait(false); // From OCRCorrectionApplication.cs
                var successfulCorrectionResults = appliedCorrections.Where(c => c.Success).ToList();
                _logger.Information("Applied {SuccessCount}/{TotalCount} value/format corrections (in-memory) for invoice {InvoiceNo}.",
                   successfulCorrectionResults.Count, appliedCorrections.Count, invoice.InvoiceNo);

                // 3.5. Apply Caribbean customs business rules after standard OCR corrections
                var customsCorrections = this.ApplyCaribbeanCustomsRules(invoice, successfulCorrectionResults);
                if (customsCorrections.Any())
                {
                    this.ApplyCaribbeanCustomsCorrectionsToInvoice(invoice, customsCorrections);
                    appliedCorrections.AddRange(customsCorrections); // Add customs corrections to the results
                    _logger.Information("Applied {CustomsCount} Caribbean customs rule corrections to invoice {InvoiceNo}.", 
                        customsCorrections.Count, invoice.InvoiceNo);
                }

                // 4. Validate Post-Correction Totals
                bool postCorrectionValid = OCRCorrectionService.TotalsZero(invoice, _logger);
                _logger.Information("Post-correction TotalsZero validation for {InvoiceNo}: {IsValid}", invoice.InvoiceNo, postCorrectionValid);

                // 5. Update Database Regex Patterns based on successful corrections
                if (successfulCorrectionResults.Any())
                {
                    var regexUpdateRequests = successfulCorrectionResults.Select(c => CreateRegexUpdateRequest(c, fileText, metadata, invoice.Id)).ToList();
                    await this.UpdateRegexPatternsAsync(regexUpdateRequests).ConfigureAwait(false); // From OCRDatabaseUpdates.cs
                }

                // Return true if any corrections were made that resulted in a successful DB update (implies learning happened)
                // OR if the invoice is now balanced, even if no DB patterns were learnable.
                // The success of UpdateRegexPatternsAsync is logged internally by it.
                return successfulCorrectionResults.Any() || postCorrectionValid;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in comprehensive invoice correction with regex updates for {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        /// <summary>
        /// Processes a list of already derived CorrectionResult objects to update the database.
        /// This method is useful if corrections are generated externally or in a separate step.
        /// </summary>
        public async Task<EnhancedCorrectionResult> ProcessExternalCorrectionsForDBUpdateAsync(
            IEnumerable<CorrectionResult> corrections,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata, // Metadata corresponding to the state BEFORE these corrections
            string fileText, // Full original text
            string filePathForLearning) // Path for logging
        {
            var result = new EnhancedCorrectionResult { StartTime = DateTime.UtcNow };
            if (corrections == null || !corrections.Any())
            {
                _logger.Information("ProcessExternalCorrectionsForDBUpdateAsync: No corrections provided.");
                result.EndTime = DateTime.UtcNow;
                return result;
            }

            _logger.Information("Starting DB update process for {CorrectionCount} externally provided corrections.", corrections.Count());
            result.TotalCorrections = corrections.Count();

            var successfulCorrectionsForDB = corrections.Where(c => c.Success).ToList();

            // Process each correction to populate ProcessedCorrections collection
            foreach (var correction in corrections)
            {
                var detail = new EnhancedCorrectionDetail
                {
                    Correction = correction,
                    ProcessingTime = DateTime.UtcNow
                };

                // Try to find metadata for this correction
                if (invoiceMetadata != null && invoiceMetadata.TryGetValue(correction.FieldName, out var metadata))
                {
                    detail.HasMetadata = true;
                    detail.OCRMetadata = metadata;

                    // Get database update context if correction was successful
                    // The UpdateContext property is no longer needed as UpdateRegexPatternsAsync takes RegexUpdateRequest directly.
                    // If it's still used elsewhere, it needs to be re-evaluated. For now, removing the assignment.
                    // detail.UpdateContext = this.GetDatabaseUpdateContext(correction.FieldName, metadata);
                }
                else
                {
                    detail.HasMetadata = false;
                    detail.SkipReason = $"No metadata found for field '{correction.FieldName}'";
                }

                result.ProcessedCorrections.Add(detail);
            }

            if (successfulCorrectionsForDB.Any())
            {
                // Convert CorrectionResult to RegexUpdateRequest for the updated UpdateRegexPatternsAsync method
                var regexUpdateRequests = successfulCorrectionsForDB.Select(c => CreateRegexUpdateRequest(c, fileText, invoiceMetadata, null)).ToList(); // InvoiceId is null here as it's external corrections
                await this.UpdateRegexPatternsAsync(regexUpdateRequests).ConfigureAwait(false);
                result.SuccessfulUpdates = successfulCorrectionsForDB.Count; // Simplified: assumes all successful inputs lead to attempt.
            }
            else
            {
                _logger.Information("No successful corrections in the provided list to process for DB updates.");
            }

            result.EndTime = DateTime.UtcNow;
            result.ProcessingDuration = result.EndTime - result.StartTime;
            _logger.Information("External corrections DB update processing finished in {Duration}ms.", result.ProcessingDuration.TotalMilliseconds);
            return result;
        }


        #endregion

        #region Internal Helpers (Moved from other files or new for orchestration)

        // This helper might be called from ExtractFullOCRMetadata or other places.
        // It was originally in OCRMetadataExtractor.cs.
        private LineContext CreateLineContextFromMetadata(OCRFieldMetadata metadata, string fileText)
        {
            // This creates a LineContext based on existing metadata for a field that *was* extracted.
            // It's useful for providing context to DeepSeek if we're trying to correct an *existing* field's extraction.
            if (metadata == null) return null;
            return new LineContext
            {
                LineId = metadata.LineId,
                LineNumber = metadata.LineNumber,
                LineText = string.IsNullOrEmpty(metadata.LineText) ? this.GetOriginalLineText(fileText, metadata.LineNumber) : metadata.LineText,
                RegexPattern = metadata.LineRegex, // Regex of the line that extracted this field
                PartId = metadata.PartId,
                PartName = metadata.PartName,
                PartTypeId = metadata.PartTypeId,
                LineName = metadata.LineName,
                RegexId = metadata.RegexId,
                // FieldsInLine would need to be populated by querying Fields for this LineId or parsing LineRegex.
                // For simplicity, it can be done on-demand if needed by a consumer of LineContext.
            };
        }

        private RegexUpdateRequest CreateRegexUpdateRequest(CorrectionResult correction, string fileText, Dictionary<string, OCRFieldMetadata> metadata, int? invoiceId)
        {
            var request = new RegexUpdateRequest
            {
                FieldName = correction.FieldName,
                OldValue = correction.OldValue,
                NewValue = correction.NewValue,
                CorrectionType = correction.CorrectionType,
                Confidence = correction.Confidence,
                DeepSeekReasoning = correction.Reasoning,
                LineNumber = correction.LineNumber,
                RequiresMultilineRegex = correction.RequiresMultilineRegex,
                ExistingRegex = correction.ExistingRegex,
                SuggestedRegex = correction.SuggestedRegex,
                LineId = correction.LineId,
                PartId = correction.PartId,
                RegexId = correction.RegexId,
                InvoiceId = invoiceId
            };

            if (!string.IsNullOrEmpty(fileText))
            {
                var lines = fileText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                if (correction.LineNumber > 0 && correction.LineNumber <= lines.Length)
                {
                    request.LineText = lines[correction.LineNumber - 1];
                }
                request.WindowText = correction.WindowText;
                if (string.IsNullOrEmpty(request.WindowText) && correction.LineNumber > 0)
                {
                    int windowStart = Math.Max(0, correction.LineNumber - 1 - 2);
                    int windowEnd = Math.Min(lines.Length, correction.LineNumber - 1 + 3);
                    request.WindowText = string.Join(Environment.NewLine, lines.Skip(windowStart).Take(windowEnd - windowStart));
                }
                request.ContextLinesBefore = GetContextLines(lines, correction.LineNumber, -2);
                request.ContextLinesAfter = GetContextLines(lines, correction.LineNumber, 2);
            }

            if (metadata != null && metadata.TryGetValue(correction.FieldName, out var fieldMetadata))
            {
                request.PartName = fieldMetadata.PartName;
                request.InvoiceType = fieldMetadata.InvoiceType;
            }

            return request;
        }

        private List<string> GetContextLines(string[] allLines, int targetLineNumber, int offset)
        {
            var contextLines = new List<string>();
            if (allLines == null || allLines.Length == 0 || targetLineNumber <= 0 || targetLineNumber > allLines.Length)
            {
                return contextLines;
            }

            if (offset > 0) // Get lines after
            {
                for (int i = 0; i < offset; i++)
                {
                    int lineIndex = targetLineNumber + i;
                    if (lineIndex < allLines.Length)
                    {
                        contextLines.Add(allLines[lineIndex]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (offset < 0) // Get lines before
            {
                for (int i = Math.Abs(offset); i > 0; i--)
                {
                    int lineIndex = targetLineNumber - 1 - i; // Convert to 0-based index
                    if (lineIndex >= 0)
                    {
                        contextLines.Add(allLines[lineIndex]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return contextLines;
        }
        #endregion

        // Helper to get full metadata for an invoice, using only ShipmentInvoice and fileText.
        // This creates basic metadata. For richer metadata using OCR templates, 
        // use ExtractEnhancedOCRMetadata (from OCRMetadataExtractor.cs part) which requires an OCR.Business.Entities.Invoice template.
        private Dictionary<string, OCRFieldMetadata> ExtractFullOCRMetadata(ShipmentInvoice shipmentInvoice, string fileText)
        {
            _logger.Debug("Extracting basic OCRFieldMetadata for invoice {InvoiceNo} using ShipmentInvoice data and file text.", shipmentInvoice?.InvoiceNo ?? "Unknown");
            var metadataDict = new Dictionary<string, OCRFieldMetadata>();
            if (shipmentInvoice == null) return metadataDict;

            Action<string, object> addMetaIfValuePresent = (propName, value) =>
            {
                if (value == null || (value is string s && string.IsNullOrEmpty(s))) return;

                var mappedInfo = this.MapDeepSeekFieldToDatabase(propName); // From OCRFieldMapping.cs
                if (mappedInfo == null)
                {
                    _logger.Verbose("ExtractFullOCRMetadata: No mapping for property {Property}, cannot create metadata.", propName);
                    return;
                }

                // Attempt to find line number for this field in the text
                // This is a best-effort for basic metadata.
                int lineNumberInText = this.FindLineNumberInTextByFieldName(mappedInfo.DisplayName, fileText); // From OCRDeepSeekIntegration.cs (or utilities)
                string lineTextFromDoc = lineNumberInText > 0 ? this.GetOriginalLineText(fileText, lineNumberInText) : null; // From OCRUtilities.cs

                metadataDict[mappedInfo.DatabaseFieldName] = new OCRFieldMetadata
                {
                    FieldName = mappedInfo.DatabaseFieldName, // Use canonical name as key
                    Value = value.ToString(),
                    RawValue = value.ToString(), // Simplification: raw value is same as processed for this basic extraction
                    LineNumber = lineNumberInText,
                    LineText = lineTextFromDoc,
                    // The following template-specific IDs would typically be null/unknown in this basic extraction
                    FieldId = null,
                    LineId = null,
                    RegexId = null,
                    PartId = null,
                    InvoiceId = null, // OCR Template ID
                    // Populate what we can from DatabaseFieldInfo
                    Key = mappedInfo.DisplayName, // Best guess for Key
                    Field = mappedInfo.DatabaseFieldName,
                    EntityType = mappedInfo.EntityType,
                    DataType = mappedInfo.DataType,
                    IsRequired = mappedInfo.IsRequired,
                    // LineName, LineRegex, PartName, PartTypeId, InvoiceName also unknown without template
                };
            };

            // Iterate over ShipmentInvoice properties to create basic metadata
            if (shipmentInvoice != null)
            {
                addMetaIfValuePresent("InvoiceNo", shipmentInvoice.InvoiceNo);
                addMetaIfValuePresent("InvoiceDate", shipmentInvoice.InvoiceDate);
                addMetaIfValuePresent("InvoiceTotal", shipmentInvoice.InvoiceTotal);
                addMetaIfValuePresent("SubTotal", shipmentInvoice.SubTotal);
                addMetaIfValuePresent("TotalInternalFreight", shipmentInvoice.TotalInternalFreight);
                addMetaIfValuePresent("TotalOtherCost", shipmentInvoice.TotalOtherCost);
                addMetaIfValuePresent("TotalInsurance", shipmentInvoice.TotalInsurance);
                addMetaIfValuePresent("TotalDeduction", shipmentInvoice.TotalDeduction);
                addMetaIfValuePresent("Currency", shipmentInvoice.Currency);
                addMetaIfValuePresent("SupplierName", shipmentInvoice.SupplierName);
                addMetaIfValuePresent("SupplierAddress", shipmentInvoice.SupplierAddress);
                // Basic metadata for line items is harder without deeper parsing of fileText here.
                // This simple version primarily focuses on header fields.
            }

            _logger.Information("Basic metadata extraction: created {Count} entries for invoice {InvoiceNo}.", metadataDict.Count, shipmentInvoice.InvoiceNo);
            return metadataDict;
        }

        #region Regex Pattern Learning and Application

        /// <summary>
        /// Load regex patterns from configuration file for learned pattern application
        /// </summary>
        private Task<List<RegexPattern>> LoadRegexPatternsAsync()
        {
            try
            {
                var regexConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "OCRRegexPatterns.json");

                if (!File.Exists(regexConfigPath))
                {
                    _logger.Information("No existing regex patterns found at {Path}", regexConfigPath);
                    return Task.FromResult(new List<RegexPattern>());
                }

                var json = File.ReadAllText(regexConfigPath);
                var patterns = JsonSerializer.Deserialize<List<RegexPattern>>(json) ?? new List<RegexPattern>();

                _logger.Information("Loaded {Count} regex patterns from configuration", patterns.Count);
                return Task.FromResult(patterns);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading regex patterns");
                return Task.FromResult(new List<RegexPattern>());
            }
        }

        /// <summary>
        /// Apply learned regex patterns to text before processing
        /// </summary>
        private async Task<string> ApplyLearnedRegexPatternsAsync(string text, string fieldName)
        {
            try
            {
                var patterns = await LoadRegexPatternsAsync().ConfigureAwait(false);
                var applicablePatterns = patterns.Where(p =>
                    p.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) &&
                    p.StrategyType == "FORMAT_FIX" &&
                    p.Confidence > 0.7).ToList();

                if (!applicablePatterns.Any())
                {
                    _logger.Debug("No applicable patterns found for field {FieldName}", fieldName);
                    return text;
                }

                var transformedText = text;
                foreach (var pattern in applicablePatterns.OrderByDescending(p => p.Confidence))
                {
                    try
                    {
                        var regex = new Regex(pattern.Pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        var newText = regex.Replace(transformedText, pattern.Replacement);

                        if (newText != transformedText)
                        {
                            _logger.Debug("Applied pattern {Pattern} -> {Replacement} for field {FieldName}",
                                pattern.Pattern, pattern.Replacement, fieldName);
                            transformedText = newText;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Failed to apply regex pattern {Pattern} for field {FieldName}",
                            pattern.Pattern, fieldName);
                    }
                }

                return transformedText;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying learned regex patterns for field {FieldName}", fieldName);
                return text;
            }
        }

        #endregion

        #region IDisposable Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    // (_deepSeekApi as IDisposable)?.Dispose(); // If DeepSeekInvoiceApi implements IDisposable
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}