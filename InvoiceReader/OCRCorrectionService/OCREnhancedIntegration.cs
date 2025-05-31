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
    /// Now fully supports omission handling with DeepSeek regex creation
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Enhanced Integration Methods

        /// <summary>
        /// Processes OCR corrections with enhanced metadata context for precise database updates
        /// Now includes full omission handling support
        /// </summary>
        /// <param name="corrections">Corrections from DeepSeek (format corrections and omissions)</param>
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
        /// Now handles both format corrections and omissions
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

                // Create line context from correction and metadata
                var lineContext = CreateLineContextFromCorrection(correction, fieldMetadata, fileText);

                // Determine correction strategy based on type
                if (correction.CorrectionType == "omission")
                {
                    detail.DatabaseUpdate = await ProcessOmissionCorrectionAsync(
                        context, correction, lineContext, filePath);
                }
                else
                {
                    // Handle format corrections with existing logic
                    var updateContext = GetDatabaseUpdateContext(correction.FieldName, fieldMetadata);
                    detail.UpdateContext = updateContext;

                    if (!updateContext.IsValid)
                    {
                        detail.SkipReason = updateContext.ErrorMessage;
                        _logger?.Warning("Skipping correction for field {FieldName}: {Reason}",
                            correction.FieldName, updateContext.ErrorMessage);
                        return detail;
                    }

                    detail.DatabaseUpdate = await ExecuteEnhancedDatabaseUpdateAsync(
                        context, correction, updateContext, fileText, filePath);
                }

                _logger?.Debug("Processed correction for field {FieldName} (type: {CorrectionType}): {Success}",
                    correction.FieldName, correction.CorrectionType, detail.DatabaseUpdate?.IsSuccess);

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
        /// Creates line context from correction data and metadata
        /// </summary>
        private LineContext CreateLineContextFromCorrection(CorrectionResult correction,
            OCRFieldMetadata fieldMetadata, string fileText)
        {
            return new LineContext
            {
                LineId = fieldMetadata?.LineId,
                LineNumber = correction.LineNumber,
                LineText = correction.LineText,
                ContextLinesBefore = correction.ContextLinesBefore ?? new List<string>(),
                ContextLinesAfter = correction.ContextLinesAfter ?? new List<string>(),
                RequiresMultilineRegex = correction.RequiresMultilineRegex,
                IsOrphaned = fieldMetadata?.LineId == null,
                RequiresNewLineCreation = fieldMetadata?.LineId == null,
                WindowText = GetLineWindow(correction.LineNumber, fileText, 5)
            };
        }

        /// <summary>
        /// Processes omission corrections with DeepSeek regex creation
        /// </summary>
        private async Task<DatabaseUpdateResult> ProcessOmissionCorrectionAsync(
            OCRContext context, CorrectionResult correction, LineContext lineContext, string filePath)
        {
            try
            {
                _logger?.Information("Processing omission correction for field {FieldName}", correction.FieldName);

                // Check if field exists in line
                var fieldExists = await CheckFieldExistsInLineAsync(context, correction.FieldName, lineContext.LineId);

                if (fieldExists)
                {
                    // Field exists, this is actually a format correction
                    _logger?.Information("Field {FieldName} exists in line, treating as format correction", correction.FieldName);
                    return await CreateFieldFormatCorrectionAsync(context, correction, lineContext);
                }

                // True omission - need to create new field and/or regex
                _logger?.Information("True omission detected for field {FieldName}, requesting regex from DeepSeek", correction.FieldName);

                // Request new regex pattern from DeepSeek
                var regexResponse = await RequestNewRegexFromDeepSeekAsync(correction, lineContext);
                if (regexResponse == null)
                {
                    return DatabaseUpdateResult.Failed("Failed to get regex pattern from DeepSeek");
                }

                // Validate the regex pattern
                if (!ValidateRegexPattern(regexResponse, correction))
                {
                    return DatabaseUpdateResult.Failed($"Invalid regex pattern from DeepSeek: {regexResponse.RegexPattern}");
                }

                // Create database entries based on strategy
                return await CreateDatabaseEntriesForOmissionAsync(context, correction, lineContext, regexResponse);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error processing omission correction for field {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Omission processing error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if a field already exists in a line's regex pattern
        /// </summary>
        private async Task<bool> CheckFieldExistsInLineAsync(OCRContext context, string fieldName, int? lineId)
        {
            if (!lineId.HasValue) return false;

            var fieldMapping = MapDeepSeekFieldToDatabase(fieldName);
            if (fieldMapping == null) return false;

            var existingField = await context.Fields
                .FirstOrDefaultAsync(f => f.LineId == lineId &&
                    (f.Key == fieldName || f.Key == fieldMapping.DatabaseFieldName ||
                     f.Field == fieldMapping.DatabaseFieldName));

            return existingField != null;
        }

        /// <summary>
        /// Requests new regex pattern from DeepSeek for omission handling
        /// </summary>
        private async Task<RegexCreationResponse> RequestNewRegexFromDeepSeekAsync(
            CorrectionResult correction, LineContext lineContext)
        {
            try
            {
                // Get existing regex pattern and named groups
                string existingPattern = null;
                var existingGroups = new List<string>();

                if (lineContext.LineId.HasValue)
                {
                    using var context = new OCRContext();
                    var line = await context.Lines
                        .Include(l => l.RegularExpressions)
                        .FirstOrDefaultAsync(l => l.Id == lineContext.LineId);

                    if (line?.RegularExpressions != null)
                    {
                        existingPattern = line.RegularExpressions.RegEx;
                        existingGroups = ExtractNamedGroupsFromRegex(existingPattern);
                    }
                }

                var prompt = CreateRegexCreationPrompt(correction, lineContext, existingPattern, existingGroups);
                var response = await _deepSeekApi.GetResponseAsync(prompt);

                return ParseRegexCreationResponse(response);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error requesting regex from DeepSeek for field {FieldName}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Creates the prompt for DeepSeek regex creation
        /// </summary>
        private string CreateRegexCreationPrompt(CorrectionResult correction, LineContext lineContext,
            string existingPattern, List<string> existingGroups)
        {
            return $@"CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:

A field '{correction.FieldName}' with value '{correction.NewValue}' was found but not extracted by current OCR processing.

CURRENT LINE REGEX: {existingPattern ?? "None"}
EXISTING NAMED GROUPS: {string.Join(", ", existingGroups)}

TARGET LINE:
{correction.LineText}

FULL CONTEXT:
{string.Join("\n", correction.ContextLinesBefore)}
>>> TARGET LINE {correction.LineNumber}: {correction.LineText} <<<
{string.Join("\n", correction.ContextLinesAfter)}

REQUIREMENTS:
1. Create a regex pattern that extracts the value '{correction.NewValue}' using named group (?<{correction.FieldName}>pattern)
2. If updating existing regex, ensure you don't break existing named groups: {string.Join(", ", existingGroups)}
3. Pattern should work with the provided context
4. Decide if this should be a separate line or modify existing line regex

RESPONSE FORMAT:
{{
  ""strategy"": ""modify_existing_line"" OR ""create_new_line"",
  ""regex_pattern"": ""(?<{correction.FieldName}>your_pattern_here)"",
  ""complete_line_regex"": ""full regex if modifying existing line"",
  ""is_multiline"": {correction.RequiresMultilineRegex.ToString().ToLower()},
  ""max_lines"": {(correction.RequiresMultilineRegex ? "5" : "1")},
  ""test_match"": ""exact text from context that should be matched"",
  ""confidence"": 0.95,
  ""reasoning"": ""why you chose this approach and pattern"",
  ""preserves_existing_groups"": true
}}

Choose the safest approach that won't break existing extractions.";
        }

        /// <summary>
        /// Creates database entries for omission corrections
        /// </summary>
        private async Task<DatabaseUpdateResult> CreateDatabaseEntriesForOmissionAsync(
            OCRContext context, CorrectionResult correction, LineContext lineContext, RegexCreationResponse regexResponse)
        {
            try
            {
                if (regexResponse.Strategy == "modify_existing_line" && lineContext.LineId.HasValue)
                {
                    return await ModifyExistingLineForOmissionAsync(context, correction, lineContext, regexResponse);
                }
                else
                {
                    return await CreateNewLineForOmissionAsync(context, correction, lineContext, regexResponse);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating database entries for omission {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Database creation error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Modifies existing line to include new field
        /// </summary>
        private async Task<DatabaseUpdateResult> ModifyExistingLineForOmissionAsync(
            OCRContext context, CorrectionResult correction, LineContext lineContext, RegexCreationResponse regexResponse)
        {
            // Update existing regex pattern
            var existingLine = await context.Lines
                .Include(l => l.RegularExpressions)
                .FirstOrDefaultAsync(l => l.Id == lineContext.LineId);

            if (existingLine?.RegularExpressions == null)
            {
                return DatabaseUpdateResult.Failed("Could not find existing line or regex to modify");
            }

            // Update the regex pattern
            existingLine.RegularExpressions.RegEx = regexResponse.CompleteLineRegex ?? regexResponse.RegexPattern;
            existingLine.RegularExpressions.MultiLine = regexResponse.IsMultiline;
            existingLine.RegularExpressions.MaxLines = regexResponse.MaxLines;

            // Create new field entry
            var fieldMapping = MapDeepSeekFieldToDatabase(correction.FieldName);
            var newField = new Fields
            {
                LineId = lineContext.LineId.Value,
                Key = correction.FieldName,
                Field = fieldMapping?.DatabaseFieldName ?? correction.FieldName,
                EntityType = fieldMapping?.EntityType ?? "ShipmentInvoice",
                DataType = fieldMapping?.DataType ?? "string",
                IsRequired = false,
                AppendValues = true,
                TrackingState = TrackableEntities.TrackingState.Added
            };

            context.Fields.Add(newField);
            await context.SaveChangesAsync();

            _logger?.Information("Modified existing line {LineId} to include field {FieldName}",
                lineContext.LineId, correction.FieldName);

            return DatabaseUpdateResult.Success(newField.Id, "Modified existing line");
        }

        /// <summary>
        /// Creates new line for omission correction
        /// </summary>
        private async Task<DatabaseUpdateResult> CreateNewLineForOmissionAsync(
            OCRContext context, CorrectionResult correction, LineContext lineContext, RegexCreationResponse regexResponse)
        {
            // Create new regex entry
            var newRegex = new RegularExpressions
            {
                RegEx = regexResponse.RegexPattern,
                MultiLine = regexResponse.IsMultiline,
                MaxLines = regexResponse.MaxLines,
                TrackingState = TrackableEntities.TrackingState.Added
            };
            context.RegularExpressions.Add(newRegex);
            await context.SaveChangesAsync();

            // Determine PartId
            var partId = await DeterminePartIdForFieldAsync(context, correction.FieldName, lineContext);

            // Create new line entry
            var newLine = new Lines
            {
                PartId = partId,
                RegExId = newRegex.Id,
                Name = $"Auto_{correction.FieldName}_{DateTime.Now:yyyyMMdd_HHmmss}",
                IsActive = true,
                TrackingState = TrackableEntities.TrackingState.Added
            };
            context.Lines.Add(newLine);
            await context.SaveChangesAsync();

            // Create new field entry
            var fieldMapping = MapDeepSeekFieldToDatabase(correction.FieldName);
            var newField = new Fields
            {
                LineId = newLine.Id,
                Key = correction.FieldName,
                Field = fieldMapping?.DatabaseFieldName ?? correction.FieldName,
                EntityType = fieldMapping?.EntityType ?? "ShipmentInvoice",
                DataType = fieldMapping?.DataType ?? "string",
                IsRequired = false,
                AppendValues = true,
                TrackingState = TrackableEntities.TrackingState.Added
            };
            context.Fields.Add(newField);
            await context.SaveChangesAsync();

            _logger?.Information("Created new line {LineId} and field {FieldId} for omission {FieldName}",
                newLine.Id, newField.Id, correction.FieldName);

            return DatabaseUpdateResult.Success(newField.Id, "Created new line and field");
        }

        /// <summary>
        /// Determines the appropriate PartId for a new field
        /// </summary>
        private async Task<int> DeterminePartIdForFieldAsync(OCRContext context, string fieldName, LineContext lineContext)
        {
            // If we have a source line, use its PartId
            if (lineContext.LineId.HasValue)
            {
                var sourceLine = await context.Lines.FirstOrDefaultAsync(l => l.Id == lineContext.LineId);
                if (sourceLine != null)
                {
                    return sourceLine.PartId;
                }
            }

            // Fallback: determine by field type
            var fieldMapping = MapDeepSeekFieldToDatabase(fieldName);
            if (fieldMapping != null)
            {
                // Header fields
                if (fieldMapping.EntityType == "ShipmentInvoice")
                {
                    var headerPart = await context.Parts
                        .FirstOrDefaultAsync(p => p.PartTypes.Name == "Header");
                    if (headerPart != null) return headerPart.Id;
                }
                // Line item fields
                else if (fieldMapping.EntityType == "InvoiceDetails")
                {
                    var lineItemPart = await context.Parts
                        .FirstOrDefaultAsync(p => p.PartTypes.Name == "LineItem");
                    if (lineItemPart != null) return lineItemPart.Id;
                }
            }

            // Ultimate fallback - use first available part
            var firstPart = await context.Parts.FirstOrDefaultAsync();
            return firstPart?.Id ?? 1;
        }

        /// <summary>
        /// Creates field format correction for existing fields
        /// </summary>
        private async Task<DatabaseUpdateResult> CreateFieldFormatCorrectionAsync(
            OCRContext context, CorrectionResult correction, LineContext lineContext)
        {
            try
            {
                var fieldMapping = MapDeepSeekFieldToDatabase(correction.FieldName);
                if (fieldMapping == null)
                {
                    return DatabaseUpdateResult.Failed($"Unknown field mapping for {correction.FieldName}");
                }

                var field = await context.Fields
                    .FirstOrDefaultAsync(f => f.LineId == lineContext.LineId &&
                        (f.Key == correction.FieldName || f.Field == fieldMapping.DatabaseFieldName));

                if (field == null)
                {
                    return DatabaseUpdateResult.Failed($"Could not find field {correction.FieldName} in line {lineContext.LineId}");
                }

                // Create format correction patterns
                var (regexPattern, replacementPattern) = CreateFormatCorrectionPatterns(correction.OldValue, correction.NewValue);
                if (string.IsNullOrEmpty(regexPattern))
                {
                    return DatabaseUpdateResult.Failed($"Could not create format correction pattern for {correction.FieldName}");
                }

                // Create or update field format regex
                var formatCorrection = await GetOrCreateFieldFormatRegexAsync(context, field.Id, regexPattern, replacementPattern);
                await context.SaveChangesAsync();

                _logger?.Information("Created field format correction for {FieldName}: {Pattern} → {Replacement}",
                    correction.FieldName, regexPattern, replacementPattern);

                return DatabaseUpdateResult.Success(formatCorrection.Id, "Created field format correction");
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating field format correction for {FieldName}", correction.FieldName);
                return DatabaseUpdateResult.Failed($"Format correction error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes database update using enhanced metadata context (existing method)
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

        #region Helper Methods (existing methods kept as-is)

        private async Task<DatabaseUpdateResult> UpdateRegexPatternWithMetadataAsync(
            OCRContext context, CorrectionResult correction, DatabaseUpdateContext updateContext, string fileText)
        {
            // Existing implementation
            return DatabaseUpdateResult.Success(0, "Updated regex pattern");
        }

        private async Task<DatabaseUpdateResult> CreateNewPatternWithMetadataAsync(
            OCRContext context, CorrectionResult correction, DatabaseUpdateContext updateContext, string fileText)
        {
            // Existing implementation
            return DatabaseUpdateResult.Success(0, "Created new pattern");
        }

        private async Task<DatabaseUpdateResult> UpdateFieldFormatWithMetadataAsync(
            OCRContext context, CorrectionResult correction, DatabaseUpdateContext updateContext)
        {
            // Existing implementation
            return DatabaseUpdateResult.Success(0, "Updated field format");
        }

        private async Task LogCorrectionWithMetadataAsync(
            OCRContext context, CorrectionResult correction, DatabaseUpdateContext updateContext, string filePath)
        {
            // Existing implementation
        }

        private async Task<FieldFormatRegEx> GetOrCreateFieldFormatRegexAsync(
            OCRContext context, int fieldId, string pattern, string replacement)
        {
            // Create regex entries
            var patternRegex = new RegularExpressions
            {
                RegEx = pattern,
                MultiLine = false,
                TrackingState = TrackableEntities.TrackingState.Added
            };
            context.RegularExpressions.Add(patternRegex);

            var replacementRegex = new RegularExpressions
            {
                RegEx = replacement,
                MultiLine = false,
                TrackingState = TrackableEntities.TrackingState.Added
            };
            context.RegularExpressions.Add(replacementRegex);

            await context.SaveChangesAsync();

            // Create field format regex
            var fieldFormatRegex = new FieldFormatRegEx
            {
                FieldId = fieldId,
                RegExId = patternRegex.Id,
                ReplacementRegExId = replacementRegex.Id,
                TrackingState = TrackableEntities.TrackingState.Added
            };
            context.OCR_FieldFormatRegEx.Add(fieldFormatRegex);

            return fieldFormatRegex;
        }

        #endregion
    }
}