using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Database update strategies for OCR correction patterns
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

            protected async Task<RegularExpressions> GetOrCreateRegexAsync(OCRContext context, string pattern, bool multiLine = false)
            {
                // Check if regex already exists
                var existingRegex = context.RegularExpressions
                    .FirstOrDefault(r => r.RegEx == pattern);

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
                    MaxLines = multiLine ? 10 : 1
                };

                context.RegularExpressions.Add(newRegex);
                await context.SaveChangesAsync();

                _logger.Information("Created new regex pattern: {Pattern} (ID: {Id})", pattern, newRegex.Id);
                return newRegex;
            }

            protected async Task<Fields> GetOrCreateFieldAsync(OCRContext context, string fieldName, string entityType, string dataType)
            {
                // Check if field already exists
                var existingField = context.Fields
                    .FirstOrDefault(f => f.Field == fieldName && f.EntityType == entityType);

                if (existingField != null)
                {
                    _logger.Debug("Found existing field: {FieldName} in {EntityType}", fieldName, entityType);
                    return existingField;
                }

                // Create new field
                var newField = new Fields
                {
                    Key = fieldName,
                    Field = fieldName,
                    EntityType = entityType,
                    DataType = dataType,
                    IsRequired = false, // Will be determined by field mapping
                    LineId = 0 // Will be set when associated with a line
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
            public FieldFormatUpdateStrategy(ILogger logger) : base(logger) { }

            public override string StrategyType => "FieldFormat";

            public override bool CanHandle(CorrectionResult correction)
            {
                return correction.CorrectionType == "FieldFormat" ||
                       correction.CorrectionType == "FORMAT_FIX" ||
                       IsFormatCorrection(correction.OldValue, correction.NewValue);
            }

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request)
            {
                try
                {
                    _logger.Information("Executing field format update for field: {FieldName}", request.FieldName);

                    // Get or create field
                    var fieldInfo = MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldInfo == null)
                    {
                        return DatabaseUpdateResult.Failed($"Unknown field: {request.FieldName}");
                    }

                    var field = await GetOrCreateFieldAsync(context, fieldInfo.DatabaseFieldName,
                        fieldInfo.EntityType, fieldInfo.DataType);

                    // Create regex pattern for the correction
                    var regexPattern = CreateFormatCorrectionPattern(request.OldValue, request.NewValue);
                    if (string.IsNullOrEmpty(regexPattern.Pattern))
                    {
                        return DatabaseUpdateResult.Failed($"Could not create regex pattern for {request.OldValue} -> {request.NewValue}");
                    }

                    // Get or create regex entries
                    var regex = await GetOrCreateRegexAsync(context, regexPattern.Pattern);
                    var replacementRegex = await GetOrCreateRegexAsync(context, regexPattern.Replacement);

                    // Check if field format regex already exists
                    var existingFieldFormat = context.OCR_FieldFormatRegEx
                        .FirstOrDefault(ffr => ffr.FieldId == field.Id &&
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
                        ReplacementRegExId = replacementRegex.Id
                    };

                    context.OCR_FieldFormatRegEx.Add(fieldFormatRegex);
                    await context.SaveChangesAsync();

                    _logger.Information("Created field format regex for {FieldName}: {Pattern} -> {Replacement}",
                        request.FieldName, regexPattern.Pattern, regexPattern.Replacement);

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

            private (string Pattern, string Replacement) CreateFormatCorrectionPattern(string oldValue, string newValue)
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



        #region Strategy Factory

        /// <summary>
        /// Factory for creating database update strategies
        /// </summary>
        public class DatabaseUpdateStrategyFactory
        {
            private readonly ILogger _logger;
            private readonly List<IDatabaseUpdateStrategy> _strategies;

            public DatabaseUpdateStrategyFactory(ILogger logger)
            {
                _logger = logger;
                _strategies = new List<IDatabaseUpdateStrategy>
                {
                    new FieldFormatUpdateStrategy(logger)
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

        #region Request Models

        /// <summary>
        /// Request for updating regex patterns in database
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
        }

        #endregion
    }
}
