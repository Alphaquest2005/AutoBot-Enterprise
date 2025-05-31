// File: OCRCorrectionService/OCRCorrectionService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;

namespace WaterNut.DataSpace
{
    using System.IO;

    /// <summary>
    /// Service for handling OCR error corrections using DeepSeek LLM analysis
    /// Enhanced with comprehensive product validation and regex learning
    /// </summary>
    public partial class OCRCorrectionService : IDisposable
    {
        #region Fields and Properties

        private readonly DeepSeekInvoiceApi _deepSeekApi;
        private readonly ILogger _logger;
        private bool _disposed = false;

        // Configuration properties
        public double DefaultTemperature { get; set; } = 0.1; // Lower temp for corrections
        public int DefaultMaxTokens { get; set; } = 4096;

        #endregion

        #region Constructor

        public OCRCorrectionService(ILogger logger = null)
        {
            _deepSeekApi = new DeepSeekInvoiceApi();
            _logger = logger ?? Log.Logger.ForContext<OCRCorrectionService>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Corrects a single invoice using comprehensive DeepSeek analysis
        /// </summary>
        public async Task<bool> CorrectInvoiceAsync(ShipmentInvoice invoice, string fileText)
        {
            try
            {
                if (invoice == null || string.IsNullOrEmpty(fileText))
                {
                    _logger.Warning("Cannot correct invoice: null invoice or empty file text");
                    return false;
                }

                _logger.Information("Starting OCR correction for invoice {InvoiceNo}", invoice.InvoiceNo);

                // Extract metadata for enhanced correction processing
                var metadata = new Dictionary<string, OCRFieldMetadata>();
                
                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                if (!errors.Any())
                {
                    _logger.Information("No errors detected for invoice {InvoiceNo}", invoice.InvoiceNo);
                    return false;
                }

                _logger.Information("Detected {ErrorCount} errors for invoice {InvoiceNo}", errors.Count, invoice.InvoiceNo);

                var corrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText).ConfigureAwait(false);
                var successfulCorrections = corrections.Count(c => c.Success);

                _logger.Information("Applied {SuccessCount}/{TotalCount} corrections for invoice {InvoiceNo}",
                    successfulCorrections, corrections.Count, invoice.InvoiceNo);

                return successfulCorrections > 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error correcting invoice {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        /// <summary>
        /// Corrects multiple invoices with comprehensive validation and regex updates
        /// </summary>
        public async Task<List<ShipmentInvoice>> CorrectInvoicesAsync(List<ShipmentInvoice> invoices, string droppedFilePath)
        {
            var invoicesWithIssues = invoices.Where(x => x.TotalsZero != 0).ToList();
            _logger.Information("Found {Count} invoices with TotalsZero != 0", invoicesWithIssues.Count);

            foreach (var invoice in invoicesWithIssues)
            {
                try
                {
                    _logger.Information("Processing invoice {InvoiceNo} with TotalsZero: {TotalsZero}",
                        invoice.InvoiceNo, invoice.TotalsZero);

                    var txtFile = droppedFilePath + ".txt";
                    if (!System.IO.File.Exists(txtFile))
                    {
                        _logger.Warning("Text file not found: {FilePath}", txtFile);
                        continue;
                    }

                    var fileTxt = System.IO.File.ReadAllText(txtFile);
                    await this.CorrectInvoiceWithRegexUpdatesAsync(invoice, fileTxt).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error processing invoice {InvoiceNo}", invoice.InvoiceNo);
                }
            }

            return invoices;
        }

        /// <summary>
        /// Corrects a single invoice with comprehensive validation and regex updates
        /// </summary>
        public async Task<bool> CorrectInvoiceWithRegexUpdatesAsync(ShipmentInvoice invoice, string fileText)
        {
            try
            {
                if (invoice == null || string.IsNullOrEmpty(fileText))
                {
                    _logger.Warning("Cannot correct invoice: null invoice or empty file text");
                    return false;
                }

                _logger.Information("Starting comprehensive OCR correction for invoice {InvoiceNo}", invoice.InvoiceNo);

                // 1. Extract enhanced metadata for omission detection
                var metadata = new Dictionary<string, OCRFieldMetadata>();
                
                // 2. Detect all types of errors including omissions
                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText, metadata).ConfigureAwait(false);
                if (!errors.Any())
                {
                    _logger.Information("No errors detected for invoice {InvoiceNo}", invoice.InvoiceNo);
                    return false;
                }

                _logger.Information("Detected {ErrorCount} errors for invoice {InvoiceNo}: {ErrorSummary}",
                    errors.Count, invoice.InvoiceNo, string.Join(", ", errors.Select(e => $"{e.Field}({e.ErrorType})")));

                // 3. Apply corrections including omission handling
                var corrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText).ConfigureAwait(false);
                var successfulCorrections = corrections.Where(c => c.Success).ToList();

                _logger.Information("Applied {SuccessCount}/{TotalCount} corrections for invoice {InvoiceNo}",
                    successfulCorrections.Count, corrections.Count, invoice.InvoiceNo);

                // 4. Validate post-correction totals
                var postCorrectionValid = TotalsZero(invoice, _logger);
                _logger.Information("Post-correction TotalsZero validation: {IsValid} for invoice {InvoiceNo}",
                    postCorrectionValid, invoice.InvoiceNo);

                // 5. Update regex patterns based on successful corrections
                if (successfulCorrections.Any())
                {
                    await this.UpdateRegexPatternsAsync(successfulCorrections, fileText).ConfigureAwait(false);
                }

                return successfulCorrections.Any();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in comprehensive invoice correction for {InvoiceNo}", invoice?.InvoiceNo);
                return false;
            }
        }

        /// <summary>
        /// Processes enhanced corrections with metadata for precise database updates
        /// </summary>
        public async Task<EnhancedCorrectionResult> ProcessCorrectionsWithEnhancedMetadataAsync(
            IEnumerable<CorrectionResult> corrections,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata,
            string fileText,
            string filePath)
        {
            var result = new EnhancedCorrectionResult
            {
                TotalCorrections = corrections.Count(),
                ProcessedCorrections = new List<EnhancedCorrectionDetail>(),
                DatabaseUpdates = new List<DatabaseUpdateResult>(),
                StartTime = DateTime.UtcNow
            };

            _logger?.Information("Starting enhanced correction processing for {CorrectionCount} corrections with metadata context",
                corrections.Count());

            try
            {
                using var context = new OCRContext();

                foreach (var correction in corrections.Where(c => c.Success))
                {
                    var detail = await ProcessSingleCorrectionWithMetadataAsync(
                        context, correction, invoiceMetadata, fileText, filePath);

                    result.ProcessedCorrections.Add(detail);

                    if (detail.DatabaseUpdate != null)
                    {
                        result.DatabaseUpdates.Add(detail.DatabaseUpdate);
                    }

                    if (detail.DatabaseUpdate?.IsSuccess == true)
                    {
                        result.SuccessfulUpdates++;
                    }
                    else
                    {
                        result.FailedUpdates++;
                    }
                }

                result.EndTime = DateTime.UtcNow;
                result.ProcessingDuration = result.EndTime - result.StartTime;

                _logger?.Information("Enhanced correction processing completed. Success: {Success}, Failed: {Failed}, Duration: {Duration}ms",
                    result.SuccessfulUpdates, result.FailedUpdates, result.ProcessingDuration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error in enhanced correction processing");
                result.EndTime = DateTime.UtcNow;
                result.ProcessingDuration = result.EndTime - result.StartTime;
                result.HasErrors = true;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Processes a single correction with enhanced metadata context
        /// </summary>
        private async Task<EnhancedCorrectionDetail> ProcessSingleCorrectionWithMetadataAsync(
            OCRContext context,
            CorrectionResult correction,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata,
            string fileText,
            string filePath)
        {
            var detail = new EnhancedCorrectionDetail
            {
                Correction = correction,
                ProcessingTime = DateTime.UtcNow
            };

            try
            {
                // Get OCR metadata for this field
                var fieldMetadata = GetFieldMetadata(correction.FieldName, invoiceMetadata);
                detail.HasMetadata = fieldMetadata != null;
                detail.OCRMetadata = fieldMetadata;

                // Get enhanced database update context
                var updateContext = GetDatabaseUpdateContext(correction.FieldName, fieldMetadata);
                detail.UpdateContext = updateContext;

                if (!updateContext.IsValid)
                {
                    detail.SkipReason = updateContext.ErrorMessage;
                    _logger?.Warning("Skipping correction for field {FieldName}: {Reason}",
                        correction.FieldName, updateContext.ErrorMessage);
                    return detail;
                }

                // Execute database update based on strategy
                detail.DatabaseUpdate = await ExecuteEnhancedDatabaseUpdateAsync(
                    context, correction, updateContext, fileText, filePath);

                _logger?.Debug("Processed correction for field {FieldName} with strategy {Strategy}: {Success}",
                    correction.FieldName, updateContext.UpdateStrategy, detail.DatabaseUpdate?.IsSuccess);

                return detail;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error processing correction for field {FieldName}", correction.FieldName);
                detail.DatabaseUpdate = DatabaseUpdateResult.Failed($"Processing error: {ex.Message}", ex);
                return detail;
            }
        }

        /// <summary>
        /// Executes database update using enhanced metadata context
        /// </summary>
        private async Task<DatabaseUpdateResult> ExecuteEnhancedDatabaseUpdateAsync(
            OCRContext context,
            CorrectionResult correction,
            DatabaseUpdateContext updateContext,
            string fileText,
            string filePath)
        {
            try
            {
                switch (updateContext.UpdateStrategy)
                {
                    case DatabaseUpdateStrategy.UpdateRegexPattern:
                        return await UpdateRegexPatternWithMetadataAsync(context, correction, updateContext, fileText);

                    case DatabaseUpdateStrategy.CreateNewPattern:
                        return await CreateNewPatternWithMetadataAsync(context, correction, updateContext, fileText);

                    case DatabaseUpdateStrategy.UpdateFieldFormat:
                        return await UpdateFieldFormatWithMetadataAsync(context, correction, updateContext);

                    case DatabaseUpdateStrategy.LogOnly:
                        await LogCorrectionWithMetadataAsync(context, correction, updateContext, filePath);
                        return DatabaseUpdateResult.Success(0, "Logged correction without database update");

                    case DatabaseUpdateStrategy.SkipUpdate:
                    default:
                        _logger?.Debug("Skipping database update for field {FieldName} - strategy: {Strategy}",
                            correction.FieldName, updateContext.UpdateStrategy);
                        return DatabaseUpdateResult.Failed($"Update skipped - strategy: {updateContext.UpdateStrategy}");
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error executing enhanced database update for field {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Database update error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets field metadata from the invoice metadata dictionary
        /// </summary>
        private OCRFieldMetadata GetFieldMetadata(string fieldName, Dictionary<string, OCRFieldMetadata> invoiceMetadata)
        {
            if (invoiceMetadata == null) return null;

            // Try exact match first
            if (invoiceMetadata.TryGetValue(fieldName, out var metadata))
            {
                return metadata;
            }

            // Try to find by mapped database field name
            var mappedFieldInfo = MapDeepSeekFieldToDatabase(fieldName);
            if (mappedFieldInfo != null && invoiceMetadata.TryGetValue(mappedFieldInfo.DatabaseFieldName, out metadata))
            {
                return metadata;
            }

            // Try case-insensitive search
            var kvp = invoiceMetadata.FirstOrDefault(m =>
                string.Equals(m.Key, fieldName, StringComparison.OrdinalIgnoreCase));

            return kvp.Key != null ? kvp.Value : null;
        }

        /// <summary>
        /// Updates regex pattern using enhanced metadata context
        /// </summary>
        private async Task<DatabaseUpdateResult> UpdateRegexPatternWithMetadataAsync(
            OCRContext context,
            CorrectionResult correction,
            DatabaseUpdateContext updateContext,
            string fileText)
        {
            try
            {
                var enhancedFieldInfo = (EnhancedDatabaseFieldInfo)updateContext.FieldInfo;
                var metadata = enhancedFieldInfo.OCRMetadata;
                var regexId = metadata.RegexId.Value;

                _logger?.Information("Updating regex pattern {RegexId} for field {FieldName} using enhanced metadata",
                    regexId, correction.FieldName);

                // Get current regex pattern
                var currentRegex = await context.RegularExpressions
                    .FirstOrDefaultAsync(r => r.Id == regexId);

                if (currentRegex == null)
                {
                    return DatabaseUpdateResult.Failed($"Regex pattern {regexId} not found");
                }

                // Create enhanced regex pattern that handles the correction
                var enhancedPattern = await CreateEnhancedRegexPatternAsync(
                    currentRegex.RegEx, correction, metadata, fileText);

                // Update the regex pattern
                currentRegex.RegEx = enhancedPattern;
                currentRegex.LastUpdated = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger?.Information("Successfully updated regex pattern {RegexId} for field {FieldName}",
                    regexId, correction.FieldName);

                return DatabaseUpdateResult.Success(regexId, "Updated regex pattern");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error updating regex pattern for field {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Regex update error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates new regex pattern using enhanced metadata context
        /// </summary>
        private async Task<DatabaseUpdateResult> CreateNewPatternWithMetadataAsync(
            OCRContext context,
            CorrectionResult correction,
            DatabaseUpdateContext updateContext,
            string fileText)
        {
            try
            {
                var enhancedFieldInfo = (EnhancedDatabaseFieldInfo)updateContext.FieldInfo;
                var metadata = enhancedFieldInfo.OCRMetadata;

                _logger?.Information("Creating new regex pattern for field {FieldName} using enhanced metadata",
                    correction.FieldName);

                // Create new regex pattern based on the correction and context
                var newPattern = await CreateRegexPatternFromCorrectionAsync(correction, metadata, fileText);

                var newRegex = new RegularExpressions
                {
                    RegEx = newPattern,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    Description = $"Auto-generated pattern for {correction.FieldName} correction"
                };

                context.RegularExpressions.Add(newRegex);
                await context.SaveChangesAsync();

                _logger?.Information("Successfully created new regex pattern {RegexId} for field {FieldName}",
                    newRegex.Id, correction.FieldName);

                return DatabaseUpdateResult.Success(newRegex.Id, "Created new regex pattern");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating new regex pattern for field {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Pattern creation error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates field format regex using enhanced metadata context
        /// </summary>
        private async Task<DatabaseUpdateResult> UpdateFieldFormatWithMetadataAsync(
            OCRContext context,
            CorrectionResult correction,
            DatabaseUpdateContext updateContext)
        {
            try
            {
                var enhancedFieldInfo = (EnhancedDatabaseFieldInfo)updateContext.FieldInfo;
                var metadata = enhancedFieldInfo.OCRMetadata;
                var fieldId = metadata.FieldId.Value;

                _logger?.Information("Updating field format for field {FieldName} (FieldId: {FieldId}) using enhanced metadata",
                    correction.FieldName, fieldId);

                // Create or update field format regex
                var formatRegex = await GetOrCreateFieldFormatRegexAsync(context, fieldId, correction);

                await context.SaveChangesAsync();

                _logger?.Information("Successfully updated field format for field {FieldName}",
                    correction.FieldName);

                return DatabaseUpdateResult.Success(formatRegex.Id, "Updated field format");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error updating field format for field {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Field format update error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Logs correction with enhanced metadata context
        /// </summary>
        private async Task LogCorrectionWithMetadataAsync(
            OCRContext context,
            CorrectionResult correction,
            DatabaseUpdateContext updateContext,
            string filePath)
        {
            try
            {
                // Log to learning table with enhanced context
                var learningEntry = new OCRCorrectionLearning
                {
                    FieldName = correction.FieldName,
                    OriginalError = correction.OldValue,
                    CorrectValue = correction.NewValue,
                    CorrectionType = correction.CorrectionType,
                    Confidence = (decimal?)correction.Confidence,
                    DeepSeekReasoning = correction.Reasoning,
                    FilePath = filePath,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "OCRCorrectionService",
                    Success = true,
                    LineNumber = correction.LineNumber
                };

                context.OCRCorrectionLearning.Add(learningEntry);
                await context.SaveChangesAsync();

                _logger?.Debug("Logged correction with enhanced metadata for field {FieldName}", correction.FieldName);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error logging correction with metadata for field {FieldName}", correction.FieldName);
            }
        }

        /// <summary>
        /// Creates enhanced regex pattern that handles the correction
        /// </summary>
        private async Task<string> CreateEnhancedRegexPatternAsync(
            string currentPattern,
            CorrectionResult correction,
            OCRFieldMetadata metadata,
            string fileText)
        {
            try
            {
                // For now, use a simple approach - combine old and new patterns
                // In a full implementation, this would use DeepSeek to create optimal patterns
                var oldValue = correction.OldValue?.Replace(".", @"\.")?.Replace("(", @"\(")?.Replace(")", @"\)");
                var newValue = correction.NewValue?.Replace(".", @"\.")?.Replace("(", @"\(")?.Replace(")", @"\)");

                if (string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(newValue))
                {
                    return currentPattern; // Return original pattern on error
            }
        }

        /// <summary>
        /// Creates regex pattern from correction and metadata context
        /// </summary>
        private async Task<string> CreateRegexPatternFromCorrectionAsync(
            CorrectionResult correction,
            OCRFieldMetadata metadata,
            string fileText)
        {
            try
            {
                // Extract the line text around the correction
                var lines = fileText.Split('\n');
                var lineText = "";

                if (correction.LineNumber > 0 && correction.LineNumber <= lines.Length)
                {
                    lineText = lines[correction.LineNumber - 1];
                }

                // Create a basic pattern that would match the corrected value
                var correctedValue = correction.NewValue?.Replace(".", @"\.")?.Replace("(", @"\(")?.Replace(")", @"\)");

                if (string.IsNullOrEmpty(correctedValue))
                {
                    return @".*"; // Default pattern
                }

                // Create pattern based on field type
                var fieldInfo = MapDeepSeekFieldToDatabase(correction.FieldName);
                if (fieldInfo?.DataType == "decimal")
                {
                    return @"-?\d+(\.\d{1,4})?"; // Decimal pattern
                }
                else if (fieldInfo?.DataType == "DateTime")
                {
                    return @"\d{1,2}[\/\-]\d{1,2}[\/\-]\d{2,4}"; // Date pattern
                }
                else
                {
                    return $@"({correctedValue})"; // Literal match pattern
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating regex pattern from correction for field {FieldName}", correction.FieldName);
                return @".*"; // Default pattern on error
            }
        }

        /// <summary>
        /// Gets or creates field format regex for a field
        /// </summary>
        private async Task<FieldFormatRegEx> GetOrCreateFieldFormatRegexAsync(
            OCRContext context,
            int fieldId,
            CorrectionResult correction)
        {
            try
            {
                // Check if field format regex already exists
                var existingFormat = await context.FieldFormatRegEx
                    .FirstOrDefaultAsync(ffr => ffr.FieldId == fieldId);

                if (existingFormat != null)
                {
                    _logger?.Debug("Found existing field format regex for FieldId {FieldId}", fieldId);
                    return existingFormat;
                }

                // Create new field format regex
                var formatRegex = new FieldFormatRegEx
                {
                    FieldId = fieldId,
                    // Create regex patterns based on the correction
                    // This is a simplified implementation - would need proper regex IDs
                    RegExId = 1, // Placeholder - would need to create or find appropriate regex
                    ReplacementRegExId = 1 // Placeholder - would need to create or find appropriate replacement regex
                };

                context.FieldFormatRegEx.Add(formatRegex);

                _logger?.Information("Created new field format regex for FieldId {FieldId}", fieldId);
                return formatRegex;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting or creating field format regex for FieldId {FieldId}", fieldId);
                throw;
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
                    _deepSeekApi?.Dispose();
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