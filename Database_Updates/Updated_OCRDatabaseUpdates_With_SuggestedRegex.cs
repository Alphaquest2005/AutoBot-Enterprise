// Updated OCRDatabaseUpdates.cs - Uses proper SuggestedRegex field
// This version assumes the database has been updated with the SuggestedRegex field
// and the domain models have been regenerated

// Replace the LogCorrectionLearningAsync method in OCRDatabaseUpdates.cs with this version:

private async Task LogCorrectionLearningAsync(
    OCRContext context,
    RegexUpdateRequest request,
    DatabaseUpdateResult dbUpdateResult)
{
    if (request == null)
    {
        _logger.Error("🚨 **LEARNING_LOG_FAILED**: Attempted to log a null RegexUpdateRequest.");
        return;
    }

    dbUpdateResult ??= DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was null.");

    try
    {
        var safeConfidence = (request.Confidence >= 0 && request.Confidence <= 1.0) ? Math.Round(request.Confidence, 4) : (double?)null;
        
        // Step 2: Log the prepared, safe-to-log data.
        _logger.Information(
            "📝 **LEARNING_LOG_PREP**: Field='{FieldName}', Type='{CorrectionType}', NewValue='{NewValue}', Confidence={Confidence}, Success={IsSuccess}, Message='{Message}'",
            request.FieldName,
            request.CorrectionType,
            request.NewValue,
            safeConfidence?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "null",
            dbUpdateResult.IsSuccess.ToString(),
            dbUpdateResult.Message
        );

        // 🔍 **ENHANCED_LOGGING**: Log the SuggestedRegex field preparation
        _logger.Error("🔍 **LEARNING_RECORD_PREP**: Preparing OCRCorrectionLearning record for Field '{FieldName}'", request.FieldName);
        _logger.Error("   - **SuggestedRegex**: '{SuggestedRegex}'", request.SuggestedRegex ?? "NULL");
        _logger.Error("   - **Pattern**: '{Pattern}'", request.Pattern ?? "NULL");
        _logger.Error("   - **Replacement**: '{Replacement}'", request.Replacement ?? "NULL");
        _logger.Error("   - **CorrectionType**: '{CorrectionType}'", request.CorrectionType);
        _logger.Error("   - **WindowText**: '{WindowText}'", request.WindowText ?? "NULL");

        var learning = new OCRCorrectionLearning
        {
            FieldName = request.FieldName,
            OriginalError = request.OldValue ?? string.Empty,
            CorrectValue = request.NewValue ?? string.Empty,
            LineNumber = request.LineNumber,
            LineText = request.LineText ?? string.Empty,
            WindowText = request.WindowText ?? string.Empty, // Clean WindowText, no SuggestedRegex mixed in
            SuggestedRegex = request.SuggestedRegex, // ✅ **PROPER_FIELD**: Direct assignment to dedicated field
            CorrectionType = request.CorrectionType,
            DeepSeekReasoning = TruncateForLog(request.DeepSeekReasoning, 1000),
            Confidence = safeConfidence,
            InvoiceType = request.InvoiceType,
            FilePath = request.FilePath,
            Success = dbUpdateResult.IsSuccess,
            ErrorMessage = dbUpdateResult.IsSuccess ? null : TruncateForLog(dbUpdateResult.Message, 2000),
            CreatedBy = "OCRCorrectionService",
            CreatedDate = DateTime.Now,
            RequiresMultilineRegex = request.RequiresMultilineRegex,
            ContextLinesBefore = request.ContextLinesBefore != null ? string.Join("\n", request.ContextLinesBefore) : null,
            ContextLinesAfter = request.ContextLinesAfter != null ? string.Join("\n", request.ContextLinesAfter) : null,
            LineId = request.LineId,
            PartId = request.PartId,
            RegexId = dbUpdateResult.IsSuccess ? dbUpdateResult.RecordId : request.RegexId,
        };
        
        // ✅ **CLEAN_IMPLEMENTATION**: SuggestedRegex now stored in proper dedicated field
        _logger.Information("✅ **PROPER_FIELD_USAGE**: SuggestedRegex '{SuggestedRegex}' stored in dedicated database field", 
            request.SuggestedRegex ?? "NULL");

        context.OCRCorrectionLearning.Add(learning);
        await context.SaveChangesAsync().ConfigureAwait(false);
        _logger.Information("✅ **LEARNING_LOG_SUCCESS**: Successfully saved learning record ID {LearningId} for Field '{FieldName}'.", learning.Id, learning.FieldName);
    }
    catch (DbEntityValidationException vex)
    {
        var errorMessages = vex.EntityValidationErrors
            .SelectMany(x => x.ValidationErrors)
            .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
        var fullErrorMessage = string.Join("; ", errorMessages);
        _logger.Error(vex, "🚨 **LEARNING_LOG_DB_VALIDATION_FAILED**: CRITICAL - DbEntityValidationException while saving record for Field '{FieldName}'. Errors: {ValidationErrors}", request.FieldName, fullErrorMessage);
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "🚨 **LEARNING_LOG_FAILED**: CRITICAL - Unhandled exception while saving OCRCorrectionLearning record for Field '{FieldName}'.", request.FieldName);
    }
}

// Updated extraction method - now much simpler since we have proper field
/// <summary>
/// Extract SuggestedRegex from OCRCorrectionLearning record
/// Now uses dedicated SuggestedRegex field instead of parsing WindowText
/// </summary>
private static string GetSuggestedRegexFromLearningRecord(OCRCorrectionLearning learningRecord)
{
    // ✅ **SIMPLE_ACCESS**: Direct field access, no parsing needed
    return learningRecord?.SuggestedRegex;
}

// Updated CreateTemplateLearningRecordsAsync method in OCRCorrectionService.cs:
private async Task CreateTemplateLearningRecordsAsync(
    OCRContext dbContext,
    List<InvoiceError> detectedErrors,
    string templateName,
    string filePath,
    int templateId)
{
    if (detectedErrors == null || !detectedErrors.Any())
    {
        _logger.Information("📝 **NO_TEMPLATE_LEARNING**: No detected errors to create learning records from");
        return;
    }

    _logger.Information("📝 **TEMPLATE_LEARNING_PROCESSING**: Creating {Count} learning records for template '{TemplateName}'", 
        detectedErrors.Count, templateName);

    var learningRecords = new List<OCRCorrectionLearning>();

    foreach (var error in detectedErrors)
    {
        try
        {
            // ✅ **CLEAN_WINDOWTEXT**: WindowText contains only the actual window text
            var windowText = !string.IsNullOrWhiteSpace(error.CapturedFields?.FirstOrDefault())
                ? string.Join(",", error.CapturedFields)
                : error.LineText ?? "";

            var learning = new OCRCorrectionLearning
            {
                FieldName = error.Field ?? "Unknown",
                OriginalError = error.ExtractedValue ?? "Missing",
                CorrectValue = error.CorrectValue ?? "Template Pattern",
                LineNumber = error.LineNumber,
                LineText = error.LineText ?? "",
                WindowText = windowText, // ✅ **CLEAN**: Pure window text, no mixed data
                SuggestedRegex = error.SuggestedRegex, // ✅ **PROPER_FIELD**: Direct assignment
                CorrectionType = "template_creation", // Special type for template creation
                DeepSeekReasoning = error.Reasoning ?? $"Template creation pattern identification for {templateName}",
                Confidence = error.Confidence,
                InvoiceType = templateName,
                FilePath = filePath,
                Success = true, // Template creation was successful
                ErrorMessage = null,
                CreatedBy = "OCRCorrectionService_TemplateCreation",
                CreatedDate = DateTime.Now,
                RequiresMultilineRegex = error.RequiresMultilineRegex,
                ContextLinesBefore = error.ContextLinesBefore != null ? string.Join("\n", error.ContextLinesBefore) : null,
                ContextLinesAfter = error.ContextLinesAfter != null ? string.Join("\n", error.ContextLinesAfter) : null,
                RegexId = templateId // Link to the created template
            };

            learningRecords.Add(learning);
            
            _logger.Information("📝 **TEMPLATE_LEARNING_RECORD**: Field='{Field}', Type='{Type}', SuggestedRegex='{SuggestedRegex}', Confidence={Confidence}", 
                learning.FieldName, learning.CorrectionType, learning.SuggestedRegex ?? "NULL", learning.Confidence);
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "⚠️ **TEMPLATE_LEARNING_ERROR**: Failed to create learning record for field '{Field}'", error.Field);
        }
    }

    if (learningRecords.Any())
    {
        try
        {
            _logger.Information("💾 **TEMPLATE_LEARNING_SAVE**: Saving {Count} template learning records to database", learningRecords.Count);
            
            dbContext.OCRCorrectionLearning.AddRange(learningRecords);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            
            _logger.Information("✅ **TEMPLATE_LEARNING_SUCCESS**: Successfully saved {Count} template learning records", learningRecords.Count);
            
            // Log summary of what was learned
            var fieldSummary = learningRecords.GroupBy(l => l.FieldName).ToDictionary(g => g.Key, g => g.Count());
            var regexSummary = learningRecords.Where(l => !string.IsNullOrWhiteSpace(l.SuggestedRegex)).Count();
            
            _logger.Information("📊 **TEMPLATE_LEARNING_SUMMARY**: Fields learned: {FieldSummary}, SuggestedRegex patterns: {RegexCount}", 
                string.Join(", ", fieldSummary.Select(kvp => $"{kvp.Key}({kvp.Value})")), regexSummary);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "🚨 **TEMPLATE_LEARNING_SAVE_FAILED**: Failed to save template learning records");
            // Don't throw - template creation was successful, learning is supplementary
        }
    }
}

// Updated LoadLearnedRegexPatternsAsync method:
public async Task<List<RegexPattern>> LoadLearnedRegexPatternsAsync(string invoiceType = null, double minimumConfidence = 0.8)
{
    _logger.Information("📚 **LOADING_LEARNED_PATTERNS**: Loading regex patterns from OCRCorrectionLearning with confidence >= {MinConfidence}", minimumConfidence);
    
    var patterns = new List<RegexPattern>();
    
    try
    {
        using (var context = new OCRContext())
        {
            var query = context.OCRCorrectionLearning
                .Where(l => l.Success == true && l.Confidence >= minimumConfidence);
            
            if (!string.IsNullOrWhiteSpace(invoiceType))
            {
                query = query.Where(l => l.InvoiceType == invoiceType);
            }
            
            var learningRecords = await query
                .OrderByDescending(l => l.CreatedDate)
                .Take(1000) // Limit to recent records
                .ToListAsync()
                .ConfigureAwait(false);
            
            _logger.Information("📊 **LEARNING_RECORDS_FOUND**: Found {Count} successful learning records", learningRecords.Count);
            
            foreach (var record in learningRecords)
            {
                try
                {
                    // ✅ **DIRECT_FIELD_ACCESS**: Simple, clean access to SuggestedRegex field
                    if (!string.IsNullOrWhiteSpace(record.SuggestedRegex))
                    {
                        var pattern = new RegexPattern
                        {
                            FieldName = record.FieldName,
                            Pattern = record.SuggestedRegex, // ✅ **CLEAN_ACCESS**: Direct field access
                            Replacement = record.CorrectValue,
                            StrategyType = DetermineStrategyType(record.CorrectionType),
                            Confidence = record.Confidence ?? 0.0,
                            CreatedDate = record.CreatedDate,
                            LastUpdated = record.CreatedDate,
                            UpdateCount = 1,
                            CreatedBy = record.CreatedBy,
                            InvoiceType = record.InvoiceType
                        };
                        
                        patterns.Add(pattern);
                        
                        _logger.Information("📝 **PATTERN_LOADED**: Field='{Field}', Pattern='{Pattern}', Confidence={Confidence}", 
                            pattern.FieldName, pattern.Pattern, pattern.Confidence);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "⚠️ **PATTERN_LOAD_ERROR**: Failed to process learning record ID {RecordId}", record.Id);
                }
            }
            
            _logger.Information("✅ **PATTERNS_LOADED**: Successfully loaded {PatternCount} regex patterns from learning records", patterns.Count);
            
            return patterns;
        }
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "🚨 **LOAD_PATTERNS_FAILED**: Failed to load learned regex patterns");
        return new List<RegexPattern>();
    }
}