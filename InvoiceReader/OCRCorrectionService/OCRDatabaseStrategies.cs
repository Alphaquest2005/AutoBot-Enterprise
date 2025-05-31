using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OCR.Business.Entities;
using Serilog;
using System.Data.Entity;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Database update strategies for OCR correction patterns with omission support
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Strategy Interfaces and Base Classes

        /// <summary>
        /// Interface for database update strategies
        /// </summary>
        public interface IDatabaseUpdateStrategy
        {
            string StrategyType { get; }
            Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request);
            bool CanHandle(CorrectionResult correction);
        }

        /// <summary>
        /// Base class for database update strategies
        /// </summary>
        public abstract class DatabaseUpdateStrategyBase : IDatabaseUpdateStrategy
        {
            protected readonly ILogger _logger;

            protected DatabaseUpdateStrategyBase(ILogger logger)
            {
                _logger = logger;
            }

            public abstract string StrategyType { get; }
            public abstract Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request);
            public abstract bool CanHandle(CorrectionResult correction);

            protected async Task<RegularExpressions> GetOrCreateRegexAsync(OCRContext context, string pattern, bool multiLine = false, int maxLines = 1)
            {
                // Check if regex already exists
                var existingRegex = await context.RegularExpressions
                    .FirstOrDefaultAsync(r => r.RegEx == pattern);

                if (existingRegex != null)
                {
                    _logger.Debug("Found existing regex pattern: {Pattern}", pattern);
                    return existingRegex;
                }

                // Create new regex
                var newRegex = new RegularExpressions
                {
                    RegEx = pattern,
                    MultiLine = multiLine,
                    MaxLines = maxLines,
                    TrackingState = TrackableEntities.TrackingState.Added
                };

                context.RegularExpressions.Add(newRegex);
                await context.SaveChangesAsync();

                _logger.Information("Created new regex pattern: {Pattern} (ID: {Id})", pattern, newRegex.Id);
                return newRegex;
            }

            protected async Task<Fields> GetOrCreateFieldAsync(OCRContext context, string fieldName, string entityType, string dataType, int lineId, string key)
            {
                // Check if field already exists
                var existingField = await context.Fields
                    .FirstOrDefaultAsync(f => f.Field == fieldName && f.EntityType == entityType && f.LineId == lineId);

                if (existingField != null)
                {
                    _logger.Debug("Found existing field: {FieldName} in {EntityType}", fieldName, entityType);
                    return existingField;
                }

                // Create new field
                var newField = new Fields
                {
                    Key = key,
                    Field = fieldName,
                    EntityType = entityType,
                    DataType = dataType,
                    IsRequired = false,
                    LineId = lineId,
                    AppendValues = true, // Always true for new fields to preserve existing behavior
                    TrackingState = TrackableEntities.TrackingState.Added
                };

                context.Fields.Add(newField);
                await context.SaveChangesAsync();

                _logger.Information("Created new field: {FieldName} in {EntityType} (ID: {Id})",
                    fieldName, entityType, newField.Id);
                return newField;
            }
        }

        #endregion

        #region Field Format Strategy

        /// <summary>
        /// Strategy for updating field format regex patterns
        /// </summary>
        public class FieldFormatUpdateStrategy : DatabaseUpdateStrategyBase
        {
            private readonly OCRCorrectionService _correctionService;

            public FieldFormatUpdateStrategy(ILogger logger, OCRCorrectionService correctionService) : base(logger)
            {
                _correctionService = correctionService;
            }

            public override string StrategyType => "FieldFormat";

            public override bool CanHandle(CorrectionResult correction)
            {
                return correction.CorrectionType == "FieldFormat" ||
                       correction.CorrectionType == "FORMAT_FIX" ||
                       correction.CorrectionType == "format_correction" ||
                       IsFormatCorrection(correction.OldValue, correction.NewValue);
            }

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request)
            {
                try
                {
                    _logger.Information("Executing field format update for field: {FieldName}", request.FieldName);

                    // Get or create field
                    var fieldInfo = _correctionService.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldInfo == null)
                    {
                        return DatabaseUpdateResult.Failed($"Unknown field: {request.FieldName}");
                    }

                    // Find existing field in database
                    var field = await context.Fields
                        .FirstOrDefaultAsync(f => f.Field == fieldInfo.DatabaseFieldName);

                    if (field == null)
                    {
                        return DatabaseUpdateResult.Failed($"Field not found in database: {fieldInfo.DatabaseFieldName}");
                    }

                    // Create regex pattern for the correction
                    var regexPattern = CreateFormatCorrectionPattern(request.OldValue, request.NewValue);
                    if (regexPattern == null)
                    {
                        return DatabaseUpdateResult.Failed($"Could not create regex pattern for {request.OldValue} -> {request.NewValue}");
                    }

                    // Get or create regex entries
                    var regex = await GetOrCreateRegexAsync(context, regexPattern.Value.Pattern);
                    var replacementRegex = await GetOrCreateRegexAsync(context, regexPattern.Value.Replacement);

                    // Check if field format regex already exists
                    var existingFieldFormat = await context.FieldFormatRegEx
                        .FirstOrDefaultAsync(ffr => ffr.FieldId == field.Id &&
                                              ffr.RegExId == regex.Id &&
                                              ffr.ReplacementRegExId == replacementRegex.Id);

                    if (existingFieldFormat != null)
                    {
                        _logger.Information("Field format regex already exists for field {FieldName}", request.FieldName);
                        return DatabaseUpdateResult.Success(existingFieldFormat.Id, "Existing");
                    }

                    // Create new field format regex
                    var fieldFormatRegex = new FieldFormatRegEx
                    {
                        FieldId = field.Id,
                        RegExId = regex.Id,
                        ReplacementRegExId = replacementRegex.Id,
                        TrackingState = TrackableEntities.TrackingState.Added
                    };

                    context.FieldFormatRegEx.Add(fieldFormatRegex);
                    await context.SaveChangesAsync();

                    _logger.Information("Created field format regex for {FieldName}: {Pattern} -> {Replacement}",
                        request.FieldName, regexPattern.Value.Pattern, regexPattern.Value.Replacement);

                    return DatabaseUpdateResult.Success(fieldFormatRegex.Id, "Created");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute field format update for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Database error: {ex.Message}");
                }
            }

            private bool IsFormatCorrection(string oldValue, string newValue)
            {
                // Check if this looks like a format correction
                if (string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(newValue))
                    return false;

                // Remove common formatting differences
                var oldNormalized = Regex.Replace(oldValue, @"[\s\$,\-]", "");
                var newNormalized = Regex.Replace(newValue, @"[\s\$,\-]", "");

                // If the core content is the same, it's likely a format correction
                return string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase);
            }

            private (string Pattern, string Replacement)? CreateFormatCorrectionPattern(string oldValue, string newValue)
            {
                // Decimal comma to point
                if (oldValue.Contains(",") && newValue.Contains(".") &&
                    oldValue.Replace(",", ".") == newValue)
                {
                    return (@"(\d+),(\d{2})", "$1.$2");
                }

                // Add currency symbol
                if (!oldValue.StartsWith("$") && newValue.StartsWith("$") &&
                    newValue.Substring(1) == oldValue)
                {
                    return (@"^(\d+\.?\d*)$", "$$1");
                }

                // Trailing minus to leading minus
                if (oldValue.EndsWith("-") && newValue.StartsWith("-") &&
                    oldValue.Substring(0, oldValue.Length - 1) == newValue.Substring(1))
                {
                    return (@"(\d+\.?\d*)-$", "-$1");
                }

                // OCR character confusion (O to 0, l to 1, etc.)
                var pattern = Regex.Escape(oldValue);
                var replacement = newValue;

                // Replace common OCR confusions
                pattern = pattern.Replace("O", "[O0]").Replace("l", "[l1]").Replace("I", "[I1]");

                return (pattern, replacement);
            }
        }

        #endregion

        #region NEW: Omission Update Strategy

        /// <summary>
        /// Strategy for handling omission corrections by creating new fields and regex patterns
        /// </summary>
        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            private readonly OCRCorrectionService _correctionService;

            public OmissionUpdateStrategy(ILogger logger, OCRCorrectionService correctionService) : base(logger)
            {
                _correctionService = correctionService;
            }

            public override string StrategyType => "Omission";

            public override bool CanHandle(CorrectionResult correction)
            {
                return correction.CorrectionType == "omission";
            }

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request)
            {
                try
                {
                    _logger.Information("Executing omission update for field: {FieldName}", request.FieldName);

                    // Get field mapping information
                    var fieldInfo = _correctionService.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldInfo == null)
                    {
                        return DatabaseUpdateResult.Failed($"Unknown field: {request.FieldName}");
                    }

                    // Request regex pattern from DeepSeek for this omission
                    var regexResponse = await _correctionService.RequestNewRegexFromDeepSeek(
                        new CorrectionResult 
                        { 
                            FieldName = request.FieldName,
                            NewValue = request.NewValue,
                            OldValue = request.OldValue,
                            LineText = request.LineText,
                            ContextLinesBefore = ParseContextLines(request.ContextBefore),
                            ContextLinesAfter = ParseContextLines(request.ContextAfter)
                        }, 
                        new LineContext 
                        { 
                            LineId = request.LineId,
                            PartId = request.PartId,
                            RegexPattern = request.ExistingRegex
                        });

                    if (regexResponse == null)
                    {
                        return DatabaseUpdateResult.Failed("Failed to get regex pattern from DeepSeek");
                    }

                    // Execute the strategy returned by DeepSeek
                    if (regexResponse.Strategy == "modify_existing_line")
                    {
                        return await ModifyExistingLine(context, request, regexResponse, fieldInfo);
                    }
                    else if (regexResponse.Strategy == "create_new_line")
                    {
                        return await CreateNewLine(context, request, regexResponse, fieldInfo);
                    }
                    else
                    {
                        return DatabaseUpdateResult.Failed($"Unknown strategy: {regexResponse.Strategy}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute omission update for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Database error: {ex.Message}");
                }
            }

            private async Task<DatabaseUpdateResult> ModifyExistingLine(
                OCRContext context, 
                RegexUpdateRequest request, 
                RegexCreationResponse regexResponse, 
                DatabaseFieldInfo fieldInfo)
            {
                try
                {
                    // Update existing regex pattern
                    var existingRegex = await context.RegularExpressions
                        .FirstOrDefaultAsync(r => r.Id == request.RegexId);

                    if (existingRegex == null)
                    {
                        return DatabaseUpdateResult.Failed($"Existing regex not found: {request.RegexId}");
                    }

                    existingRegex.RegEx = regexResponse.CompleteLineRegex;
                    existingRegex.MultiLine = regexResponse.IsMultiline;
                    existingRegex.MaxLines = regexResponse.MaxLines;
                    existingRegex.TrackingState = TrackableEntities.TrackingState.Modified;

                    // Create new field for this line
                    var newField = await GetOrCreateFieldAsync(
                        context,
                        fieldInfo.DatabaseFieldName,
                        fieldInfo.EntityType,
                        fieldInfo.DataType,
                        request.LineId.Value,
                        request.FieldName);

                    await context.SaveChangesAsync();

                    _logger.Information("Modified existing line regex and created field for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Success(newField.Id, "Modified existing line");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error modifying existing line for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Modify existing line error: {ex.Message}");
                }
            }

            private async Task<DatabaseUpdateResult> CreateNewLine(
                OCRContext context, 
                RegexUpdateRequest request, 
                RegexCreationResponse regexResponse, 
                DatabaseFieldInfo fieldInfo)
            {
                try
                {
                    // Create new regex pattern
                    var newRegex = await GetOrCreateRegexAsync(
                        context,
                        regexResponse.RegexPattern,
                        regexResponse.IsMultiline,
                        regexResponse.MaxLines);

                    // Create new line
                    var newLine = new Lines
                    {
                        PartId = request.PartId.Value,
                        RegExId = newRegex.Id,
                        Name = $"Auto_{request.FieldName}_{DateTime.Now:yyyyMMdd_HHmm}",
                        IsActive = true,
                        TrackingState = TrackableEntities.TrackingState.Added
                    };

                    context.Lines.Add(newLine);
                    await context.SaveChangesAsync();

                    // Create new field for this line
                    var newField = await GetOrCreateFieldAsync(
                        context,
                        fieldInfo.DatabaseFieldName,
                        fieldInfo.EntityType,
                        fieldInfo.DataType,
                        newLine.Id,
                        request.FieldName);

                    _logger.Information("Created new line and field for {FieldName}: LineId={LineId}, FieldId={FieldId}",
                        request.FieldName, newLine.Id, newField.Id);

                    return DatabaseUpdateResult.Success(newField.Id, "Created new line");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error creating new line for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Create new line error: {ex.Message}");
                }
            }

            private List<string> ParseContextLines(string contextString)
            {
                if (string.IsNullOrEmpty(contextString))
                    return new List<string>();

                return contextString.Split('\n').ToList();
            }
        }

        #endregion

        #region Strategy Factory

        /// <summary>
        /// Factory for creating database update strategies - Enhanced with omission support
        /// </summary>
        public class DatabaseUpdateStrategyFactory
        {
            private readonly ILogger _logger;
            private readonly List<IDatabaseUpdateStrategy> _strategies;

            public DatabaseUpdateStrategyFactory(ILogger logger, OCRCorrectionService correctionService)
            {
                _logger = logger;
                _strategies = new List<IDatabaseUpdateStrategy>
                {
                    new FieldFormatUpdateStrategy(logger, correctionService),
                    new OmissionUpdateStrategy(logger, correctionService) // NEW: Omission strategy
                };
            }

            public IDatabaseUpdateStrategy GetStrategy(CorrectionResult correction)
            {
                var strategy = _strategies.FirstOrDefault(s => s.CanHandle(correction));

                if (strategy == null)
                {
                    _logger.Warning("No strategy found for correction type: {CorrectionType}", correction.CorrectionType);
                    return _strategies.First(); // Default to field format strategy
                }

                _logger.Debug("Selected strategy: {StrategyType} for correction: {CorrectionType}",
                    strategy.StrategyType, correction.CorrectionType);

                return strategy;
            }

            public IEnumerable<IDatabaseUpdateStrategy> GetAllStrategies()
            {
                return _strategies.AsReadOnly();
            }
        }

        #endregion

        #region Request Models - Enhanced

        /// <summary>
        /// Enhanced request for updating regex patterns in database
        /// </summary>
        public class RegexUpdateRequest
        {
            public string FieldName { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
            public int LineNumber { get; set; }
            public string LineText { get; set; }
            public string WindowText { get; set; }
            public string CorrectionType { get; set; }
            public double Confidence { get; set; }
            public string DeepSeekReasoning { get; set; }
            public string FilePath { get; set; }
            public string InvoiceType { get; set; }
            public DatabaseUpdateStrategy Strategy { get; set; }
            
            // NEW: Enhanced properties for omission handling
            public int? LineId { get; set; }
            public int? PartId { get; set; }
            public int? RegexId { get; set; }
            public string ExistingRegex { get; set; }
            public string ContextBefore { get; set; }
            public string ContextAfter { get; set; }
        }

        #endregion
    }
}