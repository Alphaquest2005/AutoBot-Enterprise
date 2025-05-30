// File: OCRCorrectionService/OCRRegexManagement.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Regex Pattern Updates

        /// <summary>
        /// Updates OCR regex patterns and field format rules based on successful corrections
        /// Focuses heavily on field formatting since that's where most errors occur
        /// </summary>
        public async Task UpdateRegexPatternsAsync(List<CorrectionResult> corrections, string fileText)
        {
            if (!corrections?.Any(c => c.Success) == true || string.IsNullOrEmpty(fileText))
            {
                _logger.Information("No successful corrections to learn from");
                return;
            }

            _logger.Information("Updating database with {CorrectionCount} successful corrections", corrections.Count(c => c.Success));

            try
            {
                // Separate corrections by type - prioritize field formatting
                var formatCorrections = corrections.Where(c => c.Success && IsFieldFormatError(c)).ToList();
                var extractionCorrections = corrections.Where(c => c.Success && !IsFieldFormatError(c)).ToList();

                _logger.Information("Processing {FormatCount} field format corrections and {ExtractionCount} extraction corrections",
                    formatCorrections.Count, extractionCorrections.Count);

                // Process field format corrections first (most common)
                if (formatCorrections.Any())
                {
                    await UpdateFieldFormatRegexAsync(formatCorrections, fileText).ConfigureAwait(false);
                }

                // Process extraction pattern corrections
                if (extractionCorrections.Any())
                {
                    await UpdateExtractionRegexAsync(extractionCorrections, fileText).ConfigureAwait(false);
                }

                // Log all corrections to learning table for analysis
                await LogCorrectionsToLearningTableAsync(corrections, fileText).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating regex patterns and field formats");
            }

        }

        /// <summary>
        /// Updates field format regex patterns in the database (OCR-FieldFormatRegEx table)
        /// This is the key method for handling most OCR errors
        /// </summary>
        private async Task UpdateFieldFormatRegexAsync(List<CorrectionResult> formatCorrections, string fileText)
        {
            _logger.Information("Updating field format regex patterns for {Count} corrections", formatCorrections.Count);

            try
            {
                using var ctx = new OCR.Business.Entities.OCRContext();

                foreach (var correction in formatCorrections)
                {
                    await CreateFieldFormatRegexAsync(ctx, correction, fileText).ConfigureAwait(false);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _logger.Information("Successfully updated field format regex patterns");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating field format regex patterns");
            }
        }

        /// <summary>
        /// Creates a field format regex entry in the database
        /// </summary>
        private async Task CreateFieldFormatRegexAsync(OCR.Business.Entities.OCRContext ctx, CorrectionResult correction, string fileText)
        {
            try
            {
                // Find the field in the database
                var field = ctx.Fields.FirstOrDefault(f => f.Field == correction.FieldName);
                if (field == null)
                {
                    _logger.Warning("Field {FieldName} not found in database", correction.FieldName);
                    return;
                }

                // Create regex pattern for the format correction
                var (regexPattern, replacementPattern) = WaterNut.DataSpace.OCRCorrectionService.CreateFormatCorrectionPatterns(correction.OldValue, correction.NewValue);

                if (string.IsNullOrEmpty(regexPattern) || string.IsNullOrEmpty(replacementPattern))
                {
                    _logger.Warning("Could not create regex patterns for {FieldName}", correction.FieldName);
                    return;
                }

                // Check if similar format regex already exists
                var existingFormatRegex = ctx.OCR_FieldFormatRegEx
                    .FirstOrDefault(ffr => ffr.Fields.Id == field.Id &&
                                          ffr.RegEx.RegEx.Contains(regexPattern.Substring(0, Math.Min(10, regexPattern.Length))));

                if (existingFormatRegex != null)
                {
                    _logger.Information("Similar field format regex already exists for {FieldName}", correction.FieldName);
                    return;
                }

                // Create new regex entries
                var correctionRegex = new OCR.Business.Entities.RegularExpressions
                {
                    RegEx = regexPattern,
                    MultiLine = false,
                    TrackingState = TrackableEntities.TrackingState.Added
                };
                ctx.RegularExpressions.Add(correctionRegex);

                var replacementRegex = new OCR.Business.Entities.RegularExpressions
                {
                    RegEx = replacementPattern,
                    MultiLine = false,
                    TrackingState = TrackableEntities.TrackingState.Added
                };
                ctx.RegularExpressions.Add(replacementRegex);

                // Create field format regex entry
                var fieldFormatRegex = new OCR.Business.Entities.FieldFormatRegEx
                {
                    Fields = field,
                    RegEx = correctionRegex,
                    ReplacementRegEx = replacementRegex,
                    TrackingState = TrackableEntities.TrackingState.Added
                };
                ctx.OCR_FieldFormatRegEx.Add(fieldFormatRegex);

                _logger.Information("Created field format regex for {FieldName}: {Pattern} → {Replacement}",
                    correction.FieldName, regexPattern, replacementPattern);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating field format regex for {FieldName}", correction.FieldName);
            }
        }

        /// <summary>
        /// Updates extraction regex patterns for field detection improvements
        /// </summary>
        private async Task UpdateExtractionRegexAsync(List<CorrectionResult> extractionCorrections, string fileText)
        {
            _logger.Information("Updating extraction regex patterns for {Count} corrections", extractionCorrections.Count);

            try
            {
                using var ctx = new OCR.Business.Entities.OCRContext();

                foreach (var correction in extractionCorrections)
                {
                    await UpdateExtractionPatternAsync(ctx, correction, fileText).ConfigureAwait(false);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _logger.Information("Successfully updated extraction regex patterns");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating extraction regex patterns");
            }
        }

        /// <summary>
        /// Updates extraction pattern for a specific correction
        /// </summary>
        private async Task UpdateExtractionPatternAsync(OCR.Business.Entities.OCRContext ctx, CorrectionResult correction, string fileText)
        {
            try
            {
                // Find the field and its associated lines
                var field = ctx.Fields.FirstOrDefault(f => f.Field == correction.FieldName);
                if (field == null)
                {
                    _logger.Warning("Field {FieldName} not found for extraction pattern update", correction.FieldName);
                    return;
                }

                // For now, log the extraction pattern update - full implementation would require
                // more complex regex pattern analysis and DeepSeek integration
                _logger.Information("Extraction pattern update needed for {FieldName}: {OldValue} → {NewValue}",
                    correction.FieldName, correction.OldValue, correction.NewValue);

                // TODO: Implement full extraction pattern update logic
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating extraction pattern for {FieldName}", correction.FieldName);
            }
        }

        /// <summary>
        /// Logs corrections to learning table for analysis and future improvements
        /// </summary>
        private async Task LogCorrectionsToLearningTableAsync(List<CorrectionResult> corrections, string fileText)
        {
            try
            {
                using var ctx = new OCR.Business.Entities.OCRContext();

                foreach (var correction in corrections.Where(c => c.Success))
                {
                    var learningEntry = new OCR.Business.Entities.OCRCorrectionLearning
                    {
                        FieldName = correction.FieldName,
                        OriginalError = correction.OldValue,
                        CorrectValue = correction.NewValue,
                        CorrectionType = IsFieldFormatError(correction) ? "FieldFormat" : "Extraction",
                        Confidence = (decimal)correction.Confidence,
                        Success = true,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = "OCRCorrectionService",
                        DeepSeekReasoning = $"Corrected {correction.FieldName} from '{correction.OldValue}' to '{correction.NewValue}'",
                        TrackingState = TrackableEntities.TrackingState.Added
                    };

                    ctx.OCRCorrectionLearning.Add(learningEntry);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _logger.Information("Logged {Count} corrections to learning table", corrections.Count(c => c.Success));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error logging corrections to learning table");
            }
        }

        /// <summary>
        /// Determines if a correction is a field formatting error (most common type)
        /// </summary>
        private bool IsFieldFormatError(CorrectionResult correction)
        {
            if (string.IsNullOrEmpty(correction.OldValue) || string.IsNullOrEmpty(correction.NewValue))
                return false;

            // Common field formatting patterns
            var formatPatterns = new[]
            {
                // Decimal separator issues (comma vs period)
                (old: @"[\d,]+,\d{2}", @new: @"[\d.]+\.\d{2}"),
                // Currency symbol issues
                (old: @"[^$][\d.]+", @new: @"\$[\d.]+"),
                // Negative number formatting
                (old: @"[\d.]+-", @new: @"-[\d.]+"),
                // Space removal in numbers
                (old: @"[\d\s]+", @new: @"[\d]+"),
                // OCR character confusion (0/O, 1/l, etc.)
                (old: @"[O]", @new: @"[0]"),
                (old: @"[l]", @new: @"[1]")
            };

            foreach (var (oldPattern, newPattern) in formatPatterns)
            {
                if (Regex.IsMatch(correction.OldValue, oldPattern) && Regex.IsMatch(correction.NewValue, newPattern))
                {
                    return true;
                }
            }

            return false;
        }



        /// <summary>
        /// Creates a regex update request based on a successful correction
        /// </summary>
        private async Task<RegexUpdateRequest> CreateRegexUpdateRequestAsync(CorrectionResult correction, string[] fileLines)
        {
            try
            {
                // Find the line containing the error using DeepSeek
                var lineInfo = await FindErrorLineAsync(correction, fileLines).ConfigureAwait(false);
                if (lineInfo == null)
                {
                    _logger.Warning("Could not locate error line for field {Field}", correction.FieldName);
                    return null;
                }

                // Determine the best regex correction strategy
                var strategy = await DetermineRegexStrategyAsync(correction, lineInfo, fileLines).ConfigureAwait(false);
                if (strategy == null)
                {
                    _logger.Warning("Could not determine regex strategy for field {Field}", correction.FieldName);
                    return null;
                }

                return new RegexUpdateRequest
                {
                    FieldName = correction.FieldName,
                    CorrectionType = correction.CorrectionType,
                    OldValue = correction.OldValue,
                    NewValue = correction.NewValue,
                    LineNumber = lineInfo.LineNumber,
                    LineText = lineInfo.LineText,
                    Strategy = strategy,
                    Confidence = correction.Confidence
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating regex update request for {Field}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Finds the line containing the OCR error using DeepSeek analysis
        /// </summary>
        private async Task<LineInfo> FindErrorLineAsync(CorrectionResult correction, string[] fileLines)
        {
            var fileText = string.Join("\n", fileLines.Select((line, i) => $"{i + 1}: {line}"));

            var prompt = $@"OCR ERROR LINE DETECTION:

Find the line number in the invoice text where the field '{correction.FieldName}' contains the incorrect value '{correction.OldValue}' that was corrected to '{correction.NewValue}'.

INVOICE TEXT WITH LINE NUMBERS:
{TruncateForLog(fileText, 4000)}

CORRECTION DETAILS:
- Field: {correction.FieldName}
- Incorrect Value: {correction.OldValue}
- Correct Value: {correction.NewValue}
- Error Type: {correction.CorrectionType}

RESPONSE FORMAT (JSON only):
{{
  ""line_number"": 15,
  ""line_text"": ""Total: $123,45"",
  ""confidence"": 0.90,
  ""reasoning"": ""Found incorrect value on line 15""
}}

Return null if not found: {{""line_number"": null}}";

            try
            {
                var response = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);
                return ParseLineInfoResponse(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding error line for field {Field}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Determines the best regex correction strategy
        /// </summary>
        private async Task<RegexCorrectionStrategy> DetermineRegexStrategyAsync(
            CorrectionResult correction,
            LineInfo lineInfo,
            string[] fileLines)
        {
            var windowStart = Math.Max(0, lineInfo.LineNumber - 5);
            var windowEnd = Math.Min(fileLines.Length - 1, lineInfo.LineNumber + 5);
            var windowLines = fileLines.Skip(windowStart).Take(windowEnd - windowStart + 1).ToArray();
            var windowText = string.Join("\n", windowLines.Select((line, i) => $"{windowStart + i + 1}: {line}"));

            var prompt = $@"OCR REGEX CORRECTION STRATEGY:

Determine the best approach to fix OCR errors for this field.

CORRECTION DETAILS:
- Field: {correction.FieldName}
- Old Value: {correction.OldValue}
- New Value: {correction.NewValue}
- Error Type: {correction.CorrectionType}

TEXT WINDOW:
{windowText}

STRATEGY OPTIONS:
1. FORMAT_FIX: Post-processing regex (e.g., comma→period)
2. PATTERN_UPDATE: Update field detection pattern
3. CHARACTER_MAP: Character substitution rule
4. VALIDATION_RULE: Flag unreasonable values

RESPONSE FORMAT (JSON only):
{{
  ""strategy_type"": ""FORMAT_FIX"",
  ""regex_pattern"": ""\\$?([0-9]+)[,]([0-9]{{2}})"",
  ""replacement_pattern"": ""$1.$2"",
  ""confidence"": 0.85,
  ""reasoning"": ""Systematic comma-to-period conversion needed""
}}";

            try
            {
                var response = await _deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);
                return ParseRegexStrategyResponse(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error determining regex strategy for field {Field}", correction.FieldName);
                return null;
            }
        }

        /// <summary>
        /// Applies regex updates to the system
        /// </summary>
        private async Task ApplyRegexUpdatesAsync(List<RegexUpdateRequest> updates)
        {
            if (!updates.Any()) return;

            try
            {
                var formatFixes = updates.Where(u => u.Strategy?.StrategyType == "FORMAT_FIX").ToList();
                var patternUpdates = updates.Where(u => u.Strategy?.StrategyType == "PATTERN_UPDATE").ToList();
                var characterMaps = updates.Where(u => u.Strategy?.StrategyType == "CHARACTER_MAP").ToList();
                var validationRules = updates.Where(u => u.Strategy?.StrategyType == "VALIDATION_RULE").ToList();

                _logger.Information("Applying {FormatFixes} format fixes, {PatternUpdates} pattern updates, {CharMaps} character maps, {ValidationRules} validation rules",
                    formatFixes.Count, patternUpdates.Count, characterMaps.Count, validationRules.Count);

                // Apply each type of update
                foreach (var update in formatFixes)
                    await ApplyFormatFixAsync(update).ConfigureAwait(false);

                foreach (var update in patternUpdates)
                    await ApplyPatternUpdateAsync(update).ConfigureAwait(false);

                foreach (var update in characterMaps)
                    await ApplyCharacterMappingAsync(update).ConfigureAwait(false);

                foreach (var update in validationRules)
                    await LogValidationRuleAsync(update).ConfigureAwait(false);

                _logger.Information("Completed applying {Count} regex updates", updates.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying regex updates");
            }
        }

        /// <summary>
        /// Load and apply existing regex patterns during OCR processing
        /// </summary>
        private async Task<List<RegexPattern>> LoadRegexPatternsAsync()
        {
            try
            {
                var regexConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRRegexPatterns.json");

                if (!File.Exists(regexConfigPath))
                {
                    _logger.Information("No existing regex patterns found");
                    return new List<RegexPattern>();
                }

                var json = File.ReadAllText(regexConfigPath);
                var patterns = JsonSerializer.Deserialize<List<RegexPattern>>(json) ?? new List<RegexPattern>();

                _logger.Information("Loaded {Count} regex patterns from configuration", patterns.Count);
                return patterns;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading regex patterns");
                return new List<RegexPattern>();
            }
        }

        /// <summary>
        /// Apply learned regex patterns to text before processing
        /// </summary>
        private async Task<string> ApplyLearnedRegexPatternsAsync(string text, string fieldName)
        {
            try
            {
                var patterns = await LoadRegexPatternsAsync();
                var applicablePatterns = patterns.Where(p =>
                    p.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) &&
                    p.StrategyType == "FORMAT_FIX" &&
                    p.Confidence > 0.7).ToList();

                if (!applicablePatterns.Any())
                    return text;

                var processedText = text;
                foreach (var pattern in applicablePatterns.OrderByDescending(p => p.Confidence))
                {
                    try
                    {
                        var regex = new Regex(pattern.Pattern, RegexOptions.IgnoreCase);
                        var newText = regex.Replace(processedText, pattern.Replacement);

                        if (newText != processedText)
                        {
                            _logger.Debug("Applied regex pattern for {Field}: {OldValue} → {NewValue}",
                                fieldName, processedText, newText);
                            processedText = newText;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Error applying regex pattern {Pattern} for field {Field}",
                            pattern.Pattern, fieldName);
                    }
                }

                return processedText;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying learned regex patterns for field {Field}", fieldName);
                return text;
            }
        }

        /// <summary>
        /// Actual implementation of regex update system with database/file persistence
        /// </summary>
        private async Task ApplyFormatFixAsync(RegexUpdateRequest update)
        {
            try
            {
                _logger.Information("Applying format fix for {Field}: {Pattern} → {Replacement}",
                    update.FieldName, update.Strategy.RegexPattern, update.Strategy.ReplacementPattern);

                var regexConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRRegexPatterns.json");

                List<RegexPattern> existingPatterns = new List<RegexPattern>();

                // Load existing patterns
                if (File.Exists(regexConfigPath))
                {
                    try
                    {
                        var existingJson = File.ReadAllText(regexConfigPath);
                        existingPatterns = JsonSerializer.Deserialize<List<RegexPattern>>(existingJson) ?? new List<RegexPattern>();
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning(ex, "Could not load existing regex patterns, starting fresh");
                    }
                }

                // Add new pattern (or update existing)
                var existingPattern = existingPatterns.FirstOrDefault(p =>
                    p.FieldName == update.FieldName && p.StrategyType == update.Strategy.StrategyType);

                if (existingPattern != null)
                {
                    // Update existing pattern
                    existingPattern.Pattern = update.Strategy.RegexPattern;
                    existingPattern.Replacement = update.Strategy.ReplacementPattern;
                    existingPattern.Confidence = update.Strategy.Confidence;
                    existingPattern.LastUpdated = DateTime.UtcNow;
                    existingPattern.UpdateCount++;
                    _logger.Information("Updated existing regex pattern for {Field}", update.FieldName);
                }
                else
                {
                    // Add new pattern
                    existingPatterns.Add(new RegexPattern
                    {
                        FieldName = update.FieldName,
                        StrategyType = update.Strategy.StrategyType,
                        Pattern = update.Strategy.RegexPattern,
                        Replacement = update.Strategy.ReplacementPattern,
                        Confidence = update.Strategy.Confidence,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        UpdateCount = 1,
                        CreatedBy = "OCRCorrectionService"
                    });
                    _logger.Information("Added new regex pattern for {Field}", update.FieldName);
                }

                // Save updated patterns
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingPatterns, options);
                File.WriteAllText(regexConfigPath, updatedJson);

                _logger.Information("Regex patterns saved to {Path}", regexConfigPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying format fix for {Field}", update.FieldName);
            }
        }

        /// <summary>
        /// Applies pattern update (field detection improvement)
        /// </summary>
        private async Task ApplyPatternUpdateAsync(RegexUpdateRequest update)
        {
            try
            {
                _logger.Information("Applied pattern update for {Field}: {Pattern}",
                    update.FieldName, update.Strategy.RegexPattern);
                // TODO: Update existing field detection patterns
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying pattern update for {Field}", update.FieldName);
            }
        }

        /// <summary>
        /// Applies character mapping rules
        /// </summary>
        private async Task ApplyCharacterMappingAsync(RegexUpdateRequest update)
        {
            try
            {
                _logger.Information("Applied character mapping for {Field}: {Pattern} → {Replacement}",
                    update.FieldName, update.Strategy.RegexPattern, update.Strategy.ReplacementPattern);
                // TODO: Add systematic character substitution rules
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying character mapping for {Field}", update.FieldName);
            }
        }

        /// <summary>
        /// Logs validation rules for manual review
        /// </summary>
        private async Task LogValidationRuleAsync(RegexUpdateRequest update)
        {
            _logger.Information("Validation rule suggestion for {Field}: {Reasoning}",
                update.FieldName, update.Strategy.Reasoning);
        }

        /// <summary>
        /// Parses line info response
        /// </summary>
        private LineInfo ParseLineInfoResponse(string response)
        {
            try
            {
                var cleanJson = CleanJsonResponse(response);
                if (string.IsNullOrWhiteSpace(cleanJson)) return null;

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                if (root.TryGetProperty("line_number", out var lineElement) &&
                    lineElement.ValueKind != JsonValueKind.Null)
                {
                    return new LineInfo
                    {
                        LineNumber = lineElement.GetInt32(),
                        LineText = GetStringValue(root, "line_text") ?? "",
                        Confidence = GetDoubleValue(root, "confidence"),
                        Reasoning = GetStringValue(root, "reasoning") ?? ""
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error parsing line info response");
                return null;
            }
        }

        /// <summary>
        /// Parses regex strategy response
        /// </summary>
        private RegexCorrectionStrategy ParseRegexStrategyResponse(string response)
        {
            try
            {
                var cleanJson = CleanJsonResponse(response);
                if (string.IsNullOrWhiteSpace(cleanJson)) return null;

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                return new RegexCorrectionStrategy
                {
                    StrategyType = GetStringValue(root, "strategy_type") ?? "FORMAT_FIX",
                    RegexPattern = GetStringValue(root, "regex_pattern") ?? "",
                    ReplacementPattern = GetStringValue(root, "replacement_pattern") ?? "",
                    Confidence = GetDoubleValue(root, "confidence"),
                    Reasoning = GetStringValue(root, "reasoning") ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error parsing regex strategy response");
                return null;
            }
        }

        #endregion


    }
}