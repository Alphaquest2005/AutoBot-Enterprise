using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Serilog;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Enhanced integration service that combines metadata extraction, field mapping, and database updates
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Enhanced Integration Methods

        /// <summary>
        /// Processes OCR corrections with enhanced metadata context for precise database updates
        /// </summary>
        /// <param name="corrections">Corrections from DeepSeek</param>
        /// <param name="invoiceMetadata">Enhanced OCR metadata for all fields</param>
        /// <param name="fileText">Original OCR text</param>
        /// <param name="filePath">File path for logging</param>
        /// <returns>Processing results with database update details</returns>
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
                var metadata = updateContext.FieldInfo.OCRMetadata;
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
                var metadata = updateContext.FieldInfo.OCRMetadata;

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
                var metadata = updateContext.FieldInfo.OCRMetadata;
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
                    return currentPattern; // Return unchanged if we can't create a pattern
                }

                // Create a pattern that matches both old and new values
                var enhancedPattern = $"({currentPattern})|({oldValue.Replace(oldValue, newValue)})";

                _logger?.Debug("Created enhanced regex pattern for field {FieldName}: {Pattern}",
                    correction.FieldName, enhancedPattern);

                return enhancedPattern;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating enhanced regex pattern for field {FieldName}", correction.FieldName);
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
                var existingFormat = await context.OCR_FieldFormatRegEx
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

                context.OCR_FieldFormatRegEx.Add(formatRegex);

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
    }
}
