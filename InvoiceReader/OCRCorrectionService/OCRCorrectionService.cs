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
                    // The filePath for a single invoice correction might be the original PDF path or derived.
                    string filePathForLearning = $"single_correction_{invoice.InvoiceNo}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt"; // Placeholder
                    await this.UpdateRegexPatternsAsync(successfulCorrectionResultsForDB, fileText, filePathForLearning, metadata).ConfigureAwait(false); // From OCRDatabaseUpdates.cs
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
                    string filePathForLearning = $"comprehensive_corr_{invoice.InvoiceNo}_{DateTime.UtcNow:yyyyMMddHHmmss}.txt"; // Placeholder
                    await this.UpdateRegexPatternsAsync(successfulCorrectionResults, fileText, filePathForLearning, metadata).ConfigureAwait(false); // From OCRDatabaseUpdates.cs
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
                    if (correction.Success)
                    {
                        detail.UpdateContext = this.GetDatabaseUpdateContext(correction.FieldName, metadata);
                    }
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
                // This calls the main DB update logic.
                await this.UpdateRegexPatternsAsync(successfulCorrectionsForDB, fileText, filePathForLearning, invoiceMetadata).ConfigureAwait(false);
                // Note: UpdateRegexPatternsAsync logs its own successes/failures.
                // EnhancedCorrectionResult might need more granular feedback from UpdateRegexPatternsAsync if it were to return detailed counts.
                // For now, we assume UpdateRegexPatternsAsync handles the DB updates and logging.
                // We can estimate success based on the input.
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
        #endregion

        #region Pipeline Orchestration Methods (Testable)

        /// <summary>
        /// Executes complete correction pipeline for a single correction with retry logic.
        /// Testable instance method called by extension method.
        /// </summary>
        internal async Task<PipelineExecutionResult> ExecuteFullPipelineInternal(
            CorrectionResult correction,
            TemplateContext templateContext,
            ShipmentInvoice invoice,
            string fileText,
            int maxRetries = 3)
        {
            var result = new PipelineExecutionResult
            {
                OriginalCorrection = correction,
                StartTime = DateTime.UtcNow,
                RetryAttempts = 0
            };

            _logger.Information("🔍 **PIPELINE_FULL_START**: Starting full pipeline for field {FieldName} with max {MaxRetries} retries", 
                correction.FieldName, maxRetries);

            try
            {
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    result.RetryAttempts = attempt - 1;
                    
                    _logger.Information("🔍 **PIPELINE_ATTEMPT_{Attempt}**: Pipeline attempt {Attempt} of {MaxRetries} for field {FieldName}", 
                        attempt, attempt, maxRetries, correction.FieldName);

                    try
                    {
                        // Step 1: Generate regex pattern if needed
                        var lineContext = correction.CreateLineContext(this, templateContext.Metadata, fileText);
                        result.PatternGeneratedCorrection = await correction.GenerateRegexPattern(this, lineContext).ConfigureAwait(false);
                        
                        if (!result.PatternGeneratedCorrection.Success)
                        {
                            _logger.Warning("❌ **PIPELINE_PATTERN_GENERATION_FAILED_ATTEMPT_{Attempt}**: Pattern generation failed on attempt {Attempt}", attempt, attempt);
                            continue; // Try next attempt
                        }

                        // Step 2: Validate pattern
                        result.ValidatedCorrection = result.PatternGeneratedCorrection.ValidatePattern(this);
                        
                        if (!result.ValidatedCorrection.Success)
                        {
                            _logger.Warning("❌ **PIPELINE_VALIDATION_FAILED_ATTEMPT_{Attempt}**: Pattern validation failed on attempt {Attempt}", attempt, attempt);
                            continue; // Try next attempt
                        }

                        // Step 3: Apply to database
                        result.DatabaseResult = await result.ValidatedCorrection.ApplyToDatabase(templateContext, this).ConfigureAwait(false);
                        
                        if (!result.DatabaseResult.IsSuccess)
                        {
                            _logger.Warning("❌ **PIPELINE_DATABASE_FAILED_ATTEMPT_{Attempt}**: Database update failed on attempt {Attempt}: {Error}", 
                                attempt, attempt, result.DatabaseResult.Message);
                            continue; // Try next attempt
                        }

                        // Step 4: Re-import and validate
                        result.ReimportResult = await result.DatabaseResult.ReimportAndValidate(templateContext, this, fileText).ConfigureAwait(false);
                        
                        if (!result.ReimportResult.Success)
                        {
                            _logger.Warning("❌ **PIPELINE_REIMPORT_FAILED_ATTEMPT_{Attempt}**: Re-import failed on attempt {Attempt}: {Error}", 
                                attempt, attempt, result.ReimportResult.ErrorMessage);
                            continue; // Try next attempt
                        }

                        // Step 5: Update invoice data
                        result.InvoiceResult = await result.ReimportResult.UpdateInvoiceData(invoice, this).ConfigureAwait(false);
                        
                        if (result.InvoiceResult.Success)
                        {
                            _logger.Information("✅ **PIPELINE_SUCCESS_ATTEMPT_{Attempt}**: Full pipeline completed successfully on attempt {Attempt}", attempt, attempt);
                            result.Success = true;
                            break; // Success, exit retry loop
                        }
                        else
                        {
                            _logger.Warning("❌ **PIPELINE_INVOICE_UPDATE_FAILED_ATTEMPT_{Attempt}**: Invoice update failed on attempt {Attempt}: {Error}", 
                                attempt, attempt, result.InvoiceResult.ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "🚨 **PIPELINE_EXCEPTION_ATTEMPT_{Attempt}**: Exception during pipeline attempt {Attempt} for field {FieldName}", 
                            attempt, attempt, correction.FieldName);
                        
                        if (attempt == maxRetries)
                        {
                            result.ErrorMessage = $"Pipeline failed after {maxRetries} attempts. Last error: {ex.Message}";
                        }
                    }
                }

                if (!result.Success)
                {
                    _logger.Error("❌ **PIPELINE_ALL_ATTEMPTS_FAILED**: All {MaxRetries} pipeline attempts failed for field {FieldName}", 
                        maxRetries, correction.FieldName);
                    
                    // TODO: Implement developer email notification for persistent failures
                    await this.NotifyDeveloperOfPersistentFailure(correction, result, templateContext.FilePath).ConfigureAwait(false);
                }

                result.EndTime = DateTime.UtcNow;
                result.TotalDuration = result.EndTime - result.StartTime;

                _logger.Information("🔍 **PIPELINE_FULL_COMPLETE**: Full pipeline completed for field {FieldName} - Success: {Success}, Duration: {Duration}ms, Attempts: {Attempts}", 
                    correction.FieldName, result.Success, result.TotalDuration.TotalMilliseconds, result.RetryAttempts + 1);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_CRITICAL_EXCEPTION**: Critical exception in full pipeline for field {FieldName}", correction.FieldName);
                result.Success = false;
                result.ErrorMessage = $"Critical pipeline exception: {ex.Message}";
                result.EndTime = DateTime.UtcNow;
                result.TotalDuration = result.EndTime - result.StartTime;
                return result;
            }
        }

        /// <summary>
        /// Executes batch correction pipeline for multiple corrections.
        /// Testable instance method called by extension method.
        /// </summary>
        internal async Task<BatchPipelineResult> ExecuteBatchPipelineInternal(
            IEnumerable<CorrectionResult> corrections,
            TemplateContext templateContext,
            ShipmentInvoice invoice,
            string fileText,
            int maxRetries = 3)
        {
            var result = new BatchPipelineResult
            {
                StartTime = DateTime.UtcNow,
                TotalCorrections = corrections.Count()
            };

            _logger.Information("🔍 **PIPELINE_BATCH_START**: Starting batch pipeline for {Count} corrections", result.TotalCorrections);

            try
            {
                foreach (var correction in corrections)
                {
                    try
                    {
                        var individualResult = await this.ExecuteFullPipelineInternal(
                            correction, templateContext, invoice, fileText, maxRetries).ConfigureAwait(false);
                        
                        result.IndividualResults.Add(individualResult);
                        result.RetryAttempts += individualResult.RetryAttempts;

                        if (individualResult.Success)
                        {
                            result.SuccessfulCorrections++;
                            if (individualResult.DatabaseUpdated)
                                result.DatabaseUpdates++;
                            if (individualResult.CorrectionApplied)
                                result.InvoiceUpdates++;
                        }
                        else
                        {
                            result.FailedCorrections++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "🚨 **PIPELINE_BATCH_INDIVIDUAL_EXCEPTION**: Exception processing correction for field {FieldName}", correction.FieldName);
                        result.FailedCorrections++;
                    }
                }

                result.Success = result.SuccessfulCorrections > 0;
                result.EndTime = DateTime.UtcNow;
                result.TotalDuration = result.EndTime - result.StartTime;

                _logger.Information("✅ **PIPELINE_BATCH_COMPLETE**: Batch pipeline completed - Success: {SuccessCount}/{TotalCount}, Duration: {Duration}ms, Success Rate: {SuccessRate:F1}%", 
                    result.SuccessfulCorrections, result.TotalCorrections, result.TotalDuration.TotalMilliseconds, result.SuccessRate);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_BATCH_CRITICAL_EXCEPTION**: Critical exception in batch pipeline");
                result.Success = false;
                result.ErrorMessage = $"Critical batch pipeline exception: {ex.Message}";
                result.EndTime = DateTime.UtcNow;
                result.TotalDuration = result.EndTime - result.StartTime;
                return result;
            }
        }

        /// <summary>
        /// Creates template context from OCR field metadata.
        /// Testable instance method called by extension method.
        /// </summary>
        internal TemplateContext CreateTemplateContextInternal(Dictionary<string, OCRFieldMetadata> metadata, string fileText)
        {
            _logger.Information("🔍 **PIPELINE_TEMPLATE_CONTEXT_CREATE**: Creating template context from {MetadataCount} metadata entries", 
                metadata?.Count ?? 0);

            var context = new TemplateContext
            {
                Metadata = metadata ?? new Dictionary<string, OCRFieldMetadata>(),
                FileText = fileText
            };

            try
            {
                // Extract template information from metadata
                if (metadata != null && metadata.Any())
                {
                    var firstMetadata = metadata.Values.First();
                    context.InvoiceId = firstMetadata.InvoiceId;
                    context.InvoiceName = firstMetadata.InvoiceName;
                    // FileTypeId would be accessed via navigation property when available

                    // Collect unique IDs
                    context.PartIds = metadata.Values
                        .Where(m => m.PartId.HasValue)
                        .Select(m => m.PartId.Value)
                        .Distinct()
                        .ToList();

                    context.LineIds = metadata.Values
                        .Where(m => m.LineId.HasValue)
                        .Select(m => m.LineId.Value)
                        .Distinct()
                        .ToList();

                    _logger.Information("✅ **PIPELINE_TEMPLATE_CONTEXT_SUCCESS**: Template context created - InvoiceId: {InvoiceId}, Parts: {PartCount}, Lines: {LineCount}", 
                        context.InvoiceId, context.PartIds.Count, context.LineIds.Count);
                }
                else
                {
                    _logger.Information("⚠️ **PIPELINE_TEMPLATE_CONTEXT_MINIMAL**: Template context created with minimal information (no metadata)");
                }

                return context;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_TEMPLATE_CONTEXT_EXCEPTION**: Exception creating template context");
                return context; // Return partial context
            }
        }

        /// <summary>
        /// Creates line context from correction result.
        /// Testable instance method called by extension method.
        /// </summary>
        internal LineContext CreateLineContextInternal(CorrectionResult correction, Dictionary<string, OCRFieldMetadata> metadata, string fileText)
        {
            if (correction == null)
            {
                _logger.Error("CreateLineContextInternal: Correction is null");
                return null;
            }

            _logger.Information("🔍 **PIPELINE_LINE_CONTEXT_CREATE**: Creating line context for field {FieldName}", correction.FieldName);

            try
            {
                // Try to get metadata for this field
                OCRFieldMetadata fieldMetadata = null;
                if (metadata != null)
                {
                    if (metadata.TryGetValue(correction.FieldName, out var directMetadata))
                    {
                        fieldMetadata = directMetadata;
                    }
                    else
                    {
                        // Try mapped field name
                        var mappedInfo = this.MapDeepSeekFieldToDatabase(correction.FieldName);
                        if (mappedInfo != null && metadata.TryGetValue(mappedInfo.DatabaseFieldName, out var mappedMetadata))
                        {
                            fieldMetadata = mappedMetadata;
                        }
                    }
                }

                var lineContext = new LineContext
                {
                    LineNumber = correction.LineNumber,
                    LineText = correction.LineText ?? (correction.LineNumber > 0 ? this.GetOriginalLineText(fileText, correction.LineNumber) : null),
                    ContextLinesBefore = correction.ContextLinesBefore,
                    ContextLinesAfter = correction.ContextLinesAfter,
                    RequiresMultilineRegex = correction.RequiresMultilineRegex,
                    WindowText = correction.LineNumber > 0 ? this.ExtractWindowText(fileText, correction.LineNumber, 5) : null
                };

                if (fieldMetadata != null)
                {
                    lineContext.LineId = fieldMetadata.LineId;
                    lineContext.RegexId = fieldMetadata.RegexId;
                    lineContext.RegexPattern = fieldMetadata.LineRegex;
                    lineContext.PartId = fieldMetadata.PartId;
                    lineContext.LineName = fieldMetadata.LineName;
                    lineContext.PartName = fieldMetadata.PartName;
                    lineContext.PartTypeId = fieldMetadata.PartTypeId;
                    
                    _logger.Information("✅ **PIPELINE_LINE_CONTEXT_SUCCESS**: Line context created with metadata - LineId: {LineId}, PartId: {PartId}", 
                        lineContext.LineId, lineContext.PartId);
                }
                else
                {
                    lineContext.IsOrphaned = true;
                    lineContext.RequiresNewLineCreation = correction.CorrectionType == "omission";
                    
                    _logger.Information("⚠️ **PIPELINE_LINE_CONTEXT_ORPHANED**: Line context created without metadata - Orphaned: {IsOrphaned}", 
                        lineContext.IsOrphaned);
                }

                return lineContext;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_LINE_CONTEXT_EXCEPTION**: Exception creating line context for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Main entry point for functional pipeline integration with ReadFormattedTextStep.
        /// Detects invoice errors and executes complete correction pipeline.
        /// Called from ReadFormattedTextStep.cs OCR correction section.
        /// </summary>
        public static async Task<bool> ExecuteFullPipelineForInvoiceAsync(List<dynamic> csvLines, Invoice template, ILogger logger)
        {
            logger?.Information("🔍 **PIPELINE_MAIN_ENTRY**: ExecuteFullPipelineForInvoiceAsync called with {CsvLineCount} CSV lines", csvLines?.Count ?? 0);

            try
            {
                // Create service instance for this correction session
                using var service = new OCRCorrectionService(logger);

                // Extract ShipmentInvoice from csvLines (convert dynamic to structured entity)
                var invoice = service.ConvertCsvLinesToShipmentInvoice(csvLines);
                if (invoice == null)
                {
                    logger?.Warning("❌ **PIPELINE_MAIN_NO_INVOICE**: Could not extract ShipmentInvoice from CSV lines");
                    return false;
                }

                // Extract file text from template FormattedPdfText
                var fileText = template?.FormattedPdfText;
                if (string.IsNullOrEmpty(fileText))
                {
                    logger?.Warning("❌ **PIPELINE_MAIN_NO_TEXT**: No FormattedPdfText available from template");
                    return false;
                }

                // Extract metadata from template for pipeline context
                // Create a basic dictionary from the invoice for metadata extraction
                var invoiceDict = service.ConvertShipmentInvoiceToDict(invoice);
                var metadata = service.ExtractEnhancedOCRMetadata(invoiceDict, template);
                var templateContext = metadata.CreateTemplateContext(service, fileText);
                templateContext.FilePath = template?.FilePath ?? "unknown";

                logger?.Information("🔍 **PIPELINE_MAIN_CONTEXT**: Created template context - InvoiceId: {InvoiceId}, Metadata: {MetadataCount}", 
                    templateContext.InvoiceId, templateContext.Metadata?.Count ?? 0);

                // Step 1: Detect invoice errors using comprehensive analysis
                var corrections = await service.DetectInvoiceErrorsAsync(invoice, fileText, templateContext.Metadata).ConfigureAwait(false);
                if (!corrections.Any())
                {
                    logger?.Information("✅ **PIPELINE_MAIN_NO_ERRORS**: No errors detected - invoice appears balanced");
                    return true;
                }

                logger?.Information("🔍 **PIPELINE_MAIN_ERRORS_DETECTED**: Found {ErrorCount} errors to correct: {ErrorTypes}", 
                    corrections.Count, string.Join(", ", corrections.Select(c => $"{c.Field}({c.ErrorType})")));

                // Convert InvoiceError to CorrectionResult for pipeline processing
                var correctionResults = corrections.Select(error => new CorrectionResult
                {
                    FieldName = error.Field,
                    OldValue = error.ExtractedValue,
                    NewValue = error.CorrectValue,
                    CorrectionType = error.ErrorType,
                    Success = true,
                    Confidence = error.Confidence,
                    Reasoning = error.Reasoning,
                    LineNumber = error.LineNumber,
                    LineText = error.LineText,
                    ContextLinesBefore = error.ContextLinesBefore,
                    ContextLinesAfter = error.ContextLinesAfter,
                    RequiresMultilineRegex = error.RequiresMultilineRegex
                }).ToList();

                // Step 2: Execute batch pipeline for all corrections
                var batchResult = await correctionResults.ExecuteBatchPipeline(
                    service, templateContext, invoice, fileText, maxRetries: 3).ConfigureAwait(false);

                if (batchResult.Success)
                {
                    logger?.Information("✅ **PIPELINE_MAIN_SUCCESS**: Batch pipeline completed successfully - {SuccessCount}/{TotalCount} corrections applied", 
                        batchResult.SuccessfulCorrections, batchResult.TotalCorrections);

                    // Step 3: Update the original csvLines with corrected invoice data
                    service.UpdateCsvLinesFromInvoice(csvLines, invoice);

                    // Calculate final TotalsZero to verify correction
                    OCRCorrectionService.TotalsZero(csvLines, out var finalTotalsZero, logger);
                    logger?.Information("🔍 **PIPELINE_MAIN_FINAL_TOTALS**: Final TotalsZero after pipeline = {TotalsZero:F2}", finalTotalsZero);

                    return Math.Abs(finalTotalsZero) <= 0.01; // Return true if balanced
                }
                else
                {
                    logger?.Warning("❌ **PIPELINE_MAIN_FAILED**: Batch pipeline failed - {FailureCount}/{TotalCount} corrections failed: {ErrorMessage}", 
                        batchResult.FailedCorrections, batchResult.TotalCorrections, batchResult.ErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "🚨 **PIPELINE_MAIN_EXCEPTION**: Critical exception in ExecuteFullPipelineForInvoiceAsync");
                return false;
            }
        }

        /// <summary>
        /// Notifies developer of persistent correction failures.
        /// TODO: Implement actual email notification system.
        /// </summary>
        private async Task NotifyDeveloperOfPersistentFailure(CorrectionResult correction, PipelineExecutionResult result, string filePath)
        {
            _logger.Warning("📧 **DEVELOPER_NOTIFICATION**: Persistent failure for field {FieldName} after {Attempts} attempts - Developer notification would be sent", 
                correction.FieldName, result.RetryAttempts + 1);

            // TODO: Implement developer email notification
            // This would integrate with the existing email system to send detailed failure reports
            // including correction details, error messages, and suggested manual actions

            await Task.Delay(1).ConfigureAwait(false); // Placeholder for async operation
        }

        /// <summary>
        /// Converts dynamic CSV lines to a ShipmentInvoice entity for pipeline processing.
        /// </summary>
        private ShipmentInvoice ConvertCsvLinesToShipmentInvoice(List<dynamic> csvLines)
        {
            if (csvLines == null || !csvLines.Any())
            {
                _logger.Warning("ConvertCsvLinesToShipmentInvoice: No CSV lines provided");
                return null;
            }

            try
            {
                // Extract the first invoice data from the CSV lines
                // This uses the same logic as in OCRLegacySupport.CreateTempShipmentInvoice
                var firstItem = csvLines.FirstOrDefault();
                if (firstItem == null)
                {
                    _logger.Warning("ConvertCsvLinesToShipmentInvoice: First CSV item is null");
                    return null;
                }

                IDictionary<string, object> invoiceDict = null;
                if (firstItem is List<IDictionary<string, object>> invoiceList && invoiceList.Any())
                {
                    invoiceDict = invoiceList.First();
                }
                else if (firstItem is IDictionary<string, object> singleDict)
                {
                    invoiceDict = singleDict;
                }

                if (invoiceDict == null)
                {
                    _logger.Warning("ConvertCsvLinesToShipmentInvoice: Could not extract invoice dictionary from CSV lines");
                    return null;
                }

                // Use the existing CreateTempShipmentInvoice method from OCRLegacySupport
                return OCRCorrectionService.CreateTempShipmentInvoice(invoiceDict, _logger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error converting CSV lines to ShipmentInvoice");
                return null;
            }
        }

        /// <summary>
        /// Converts a ShipmentInvoice entity back to a dictionary for metadata extraction.
        /// </summary>
        private IDictionary<string, object> ConvertShipmentInvoiceToDict(ShipmentInvoice invoice)
        {
            if (invoice == null)
            {
                _logger.Warning("ConvertShipmentInvoiceToDict: Invoice is null");
                return new Dictionary<string, object>();
            }

            var dict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = invoice.InvoiceNo,
                ["InvoiceDate"] = invoice.InvoiceDate,
                ["InvoiceTotal"] = invoice.InvoiceTotal,
                ["SubTotal"] = invoice.SubTotal,
                ["TotalInternalFreight"] = invoice.TotalInternalFreight,
                ["TotalOtherCost"] = invoice.TotalOtherCost,
                ["TotalInsurance"] = invoice.TotalInsurance,
                ["TotalDeduction"] = invoice.TotalDeduction,
                ["Currency"] = invoice.Currency,
                ["SupplierName"] = invoice.SupplierName,
                ["SupplierAddress"] = invoice.SupplierAddress
            };

            // Add invoice details if available
            if (invoice.InvoiceDetails?.Any() == true)
            {
                var detailsList = invoice.InvoiceDetails.Select(detail => new Dictionary<string, object>
                {
                    ["LineNumber"] = detail.LineNumber,
                    ["ItemDescription"] = detail.ItemDescription,
                    ["Quantity"] = detail.Quantity,
                    ["Cost"] = detail.Cost,
                    ["TotalCost"] = detail.TotalCost,
                    ["Discount"] = detail.Discount,
                    ["Units"] = detail.Units
                }).ToList();

                dict["InvoiceDetails"] = detailsList;
            }

            return dict;
        }

        /// <summary>
        /// Updates the original CSV lines with corrected invoice data.
        /// </summary>
        private void UpdateCsvLinesFromInvoice(List<dynamic> csvLines, ShipmentInvoice correctedInvoice)
        {
            if (csvLines == null || !csvLines.Any() || correctedInvoice == null)
            {
                _logger.Warning("UpdateCsvLinesFromInvoice: Invalid input parameters");
                return;
            }

            try
            {
                // Use the existing UpdateDynamicResultsWithCorrections method from OCRLegacySupport
                OCRCorrectionService.UpdateDynamicResultsWithCorrections(csvLines, new List<ShipmentInvoice> { correctedInvoice }, _logger);
                _logger.Information("Successfully updated CSV lines with corrected invoice data for {InvoiceNo}", correctedInvoice.InvoiceNo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating CSV lines from corrected invoice {InvoiceNo}", correctedInvoice?.InvoiceNo);
            }
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