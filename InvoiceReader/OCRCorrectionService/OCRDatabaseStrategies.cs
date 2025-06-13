// File: OCRCorrectionService/OCRDatabaseStrategies.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OCR.Business.Entities; // For DB entities like RegularExpressions, Fields, Lines, FieldFormatRegEx
using Serilog;
using System.Data.Entity; // For EF operations like FirstOrDefaultAsync
using TrackableEntities; // For TrackingState

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Strategy Interfaces and Base Classes

        public interface IDatabaseUpdateStrategy
        {
            string StrategyType { get; }

            Task<DatabaseUpdateResult> ExecuteAsync(
                OCRContext context,
                RegexUpdateRequest request,
                OCRCorrectionService serviceInstance);

            bool CanHandle(RegexUpdateRequest request);
        }

        public abstract class DatabaseUpdateStrategyBase : IDatabaseUpdateStrategy
        {
            protected readonly ILogger _logger;

            protected DatabaseUpdateStrategyBase(ILogger logger)
            {
                _logger = logger ?? Log.Logger;
            }

            public abstract string StrategyType { get; }

            public abstract Task<DatabaseUpdateResult> ExecuteAsync(
                OCRContext context,
                RegexUpdateRequest request,
                OCRCorrectionService serviceInstance);

            public abstract bool CanHandle(RegexUpdateRequest request);

            protected async Task<RegularExpressions> GetOrCreateRegexAsync(
                OCRContext context,
                string pattern,
                bool multiLine = false,
                int maxLines = 1,
                string description = null)
            {
                var existingRegex = await context.RegularExpressions
                                        .FirstOrDefaultAsync(
                                            r => r.RegEx == pattern && r.MultiLine == multiLine
                                                                    && r.MaxLines == maxLines).ConfigureAwait(false);
                if (existingRegex != null)
                {
                    _logger.Debug("Found existing regex pattern (ID: {RegexId}): {Pattern}", existingRegex.Id, pattern);
                    return existingRegex;
                }

                var newRegex = new RegularExpressions
                                   {
                                       RegEx = pattern,
                                       MultiLine = multiLine,
                                       MaxLines = maxLines,
                                       Description = description ?? $"Auto-generated: {DateTime.UtcNow}",
                                       CreatedDate = DateTime.UtcNow,
                                       LastUpdated = DateTime.UtcNow,
                                       TrackingState = TrackingState.Added
                                   };
                context.RegularExpressions.Add(newRegex);
                _logger.Information(
                    "Prepared new regex pattern for creation: {Pattern} with MultiLine={IsMultiLine}",
                    pattern,
                    multiLine);
                return newRegex;
            }

            protected async Task<Fields> GetOrCreateFieldAsync(
                OCRContext context,
                string fieldKey,
                string dbFieldName,
                string entityType,
                string dataType,
                int lineId,
                bool isRequired = false,
                bool appendValues = true)
            {
                var existingField = await context.Fields.FirstOrDefaultAsync(
                                            f => f.LineId == lineId
                                                 && ((!string.IsNullOrEmpty(fieldKey) && f.Key == fieldKey)
                                                     || (string.IsNullOrEmpty(fieldKey) && f.Field == dbFieldName)))
                                        .ConfigureAwait(false);

                if (existingField != null)
                {
                    _logger.Debug(
                        "Found existing field definition (ID: {FieldId}) for LineId {LineId}, Key '{Key}', DBField '{DbField}'",
                        existingField.Id,
                        lineId,
                        fieldKey,
                        dbFieldName);
                    return existingField;
                }

                var newField = new Fields
                                   {
                                       LineId = lineId,
                                       Key = fieldKey,
                                       Field = dbFieldName,
                                       EntityType = entityType,
                                       DataType = dataType,
                                       IsRequired = isRequired,
                                       AppendValues = appendValues,
                                       TrackingState = TrackingState.Added
                                   };
                context.Fields.Add(newField);
                _logger.Information(
                    "Prepared new field definition for LineId {LineId}, Key '{Key}', DBField '{DbField}'",
                    lineId,
                    fieldKey,
                    dbFieldName);
                return newField;
            }
        }

        #endregion

        #region Field Format Strategy

        public class FieldFormatUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public FieldFormatUpdateStrategy(ILogger logger)
                : base(logger)
            {
            }

            public override string StrategyType => "FieldFormat";

            public override bool CanHandle(RegexUpdateRequest request)
            {
                if (string.IsNullOrEmpty(request.OldValue) && !string.IsNullOrEmpty(request.NewValue))
                    return false;

                return request.CorrectionType == "FieldFormat" || request.CorrectionType == "FORMAT_FIX"
                                                               || request.CorrectionType == "format_correction"
                                                               || request.CorrectionType == "decimal_separator"
                                                               || request.CorrectionType == "DecimalSeparator"
                                                               || request.CorrectionType == "character_confusion"
                                                               || IsPotentialFormatCorrection(
                                                                   request.OldValue,
                                                                   request.NewValue);
            }

            private bool IsPotentialFormatCorrection(string oldValue, string newValue)
            {
                if (string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(newValue) || oldValue == newValue)
                    return false;
                var oldNormalized = Regex.Replace(oldValue, @"[\s\$,€£\-()]", "");
                var newNormalized = Regex.Replace(newValue, @"[\s\$,€£\-()]", "");
                return string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase);
            }

            public override async Task<DatabaseUpdateResult> ExecuteAsync(
                OCRContext context,
                RegexUpdateRequest request,
                OCRCorrectionService serviceInstance)
            {
                _logger.Information(
                    "Executing FieldFormatUpdateStrategy for field: {FieldName}, Value: '{OldValue}' -> '{NewValue}'",
                    request.FieldName,
                    request.OldValue,
                    request.NewValue);
                try
                {
                    if (!request.LineId.HasValue)
                    {
                        return DatabaseUpdateResult.Failed(
                            $"Field Definition ID (Fields.Id) is required for FieldFormatUpdateStrategy for field '{request.FieldName}'. It should be passed via RegexUpdateRequest.LineId.");
                    }

                    int fieldDefinitionId = request.LineId.Value;

                    var fieldDef = await context.Fields.FindAsync(fieldDefinitionId).ConfigureAwait(false);
                    if (fieldDef == null)
                        return DatabaseUpdateResult.Failed(
                            $"Field definition with ID {fieldDefinitionId} not found for field '{request.FieldName}'.");

                    var formatPatterns =
                        serviceInstance.CreateAdvancedFormatCorrectionPatterns(request.OldValue, request.NewValue);

                    if (!formatPatterns.HasValue || string.IsNullOrEmpty(formatPatterns.Value.Pattern))
                    {
                        return DatabaseUpdateResult.Failed(
                            $"Could not generate format correction regex for '{request.FieldName}': '{request.OldValue}' -> '{request.NewValue}'.");
                    }

                    var patternRegexEntity = await this.GetOrCreateRegexAsync(
                                                     context,
                                                     formatPatterns.Value.Pattern,
                                                     description: $"Pattern for format fix: {request.FieldName}")
                                                 .ConfigureAwait(false);
                    var replacementRegexEntity = await this.GetOrCreateRegexAsync(
                                                         context,
                                                         formatPatterns.Value.Replacement,
                                                         description:
                                                         $"Replacement for format fix: {request.FieldName}")
                                                     .ConfigureAwait(false);

                    if (patternRegexEntity.TrackingState == TrackingState.Added
                        || replacementRegexEntity.TrackingState == TrackingState.Added)
                    {
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    var existingFieldFormat = await context.OCR_FieldFormatRegEx.FirstOrDefaultAsync(
                                                      ffr => ffr.FieldId == fieldDefinitionId
                                                             && ffr.RegExId == patternRegexEntity.Id
                                                             && ffr.ReplacementRegExId == replacementRegexEntity.Id)
                                                  .ConfigureAwait(false);

                    if (existingFieldFormat != null)
                    {
                        _logger.Information(
                            "FieldFormatRegEx (ID: {Id}) already exists for FieldId {FieldId}.",
                            existingFieldFormat.Id,
                            fieldDefinitionId);
                        return DatabaseUpdateResult.Success(existingFieldFormat.Id, "Existing FieldFormatRegEx");
                    }

                    var newFieldFormatRegex = new FieldFormatRegEx
                                                  {
                                                      FieldId = fieldDefinitionId,
                                                      RegExId = patternRegexEntity.Id,
                                                      ReplacementRegExId = replacementRegexEntity.Id,
                                                      TrackingState = TrackingState.Added
                                                  };
                    context.OCR_FieldFormatRegEx.Add(newFieldFormatRegex);
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    _logger.Information(
                        "Created new FieldFormatRegEx (ID: {NewId}) for FieldId {FieldId}: Pattern='{Pattern}' -> Replacement='{Replacement}'",
                        newFieldFormatRegex.Id,
                        fieldDefinitionId,
                        formatPatterns.Value.Pattern,
                        formatPatterns.Value.Replacement);
                    return DatabaseUpdateResult.Success(newFieldFormatRegex.Id, "Created FieldFormatRegEx");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute FieldFormatUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed(
                        $"Database error in FieldFormatUpdateStrategy: {ex.Message}",
                        ex);
                }
            }
        }

        #endregion

        #region Omission Update Strategy

        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public OmissionUpdateStrategy(ILogger logger)
                : base(logger)
            {
            }

            public override string StrategyType => "Omission";

            public override bool CanHandle(RegexUpdateRequest request) => request.CorrectionType.StartsWith("omission");

            public override async Task<DatabaseUpdateResult> ExecuteAsync(
                OCRContext context,
                RegexUpdateRequest request,
                OCRCorrectionService serviceInstance)
            {
                _logger.Error(
                    "🔍 **OMISSION_STRATEGY_START**: Executing OmissionUpdateStrategy for field: {FieldName}",
                    request.FieldName);

                try
                {
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldMappingInfo == null)
                    {
                        return DatabaseUpdateResult.Failed(
                            $"Unknown field mapping for omitted field '{request.FieldName}'.");
                    }

                    var correctionForPrompt = new CorrectionResult
                                                  {
                                                      FieldName = request.FieldName,
                                                      NewValue = request.NewValue,
                                                      LineText = request.LineText,
                                                      LineNumber = request.LineNumber,
                                                      CorrectionType = request.CorrectionType
                                                  };
                    var lineContextForPrompt = new LineContext
                                                   {
                                                       LineNumber = request.LineNumber,
                                                       LineText = request.LineText,
                                                       WindowText = request.WindowText
                                                   };

                    var regexResponse = await serviceInstance
                                            .RequestNewRegexFromDeepSeek(correctionForPrompt, lineContextForPrompt)
                                            .ConfigureAwait(false);
                    if (regexResponse == null
                        || !serviceInstance.ValidateRegexPattern(regexResponse, correctionForPrompt))
                    {
                        return DatabaseUpdateResult.Failed(
                            $"DeepSeek failed to generate or validate a regex for omission: '{request.FieldName}'.");
                    }

                    if (!request.PartId.HasValue)
                        request.PartId = await DeterminePartIdForNewOmissionLineAsync(
                                             context,
                                             fieldMappingInfo,
                                             request.FieldName,
                                             serviceInstance).ConfigureAwait(false);
                    if (!request.PartId.HasValue)
                        return DatabaseUpdateResult.Failed($"Cannot determine PartId for new line.");

                    return await CreateNewLineForOmissionAsync(
                               context,
                               request,
                               regexResponse,
                               fieldMappingInfo,
                               serviceInstance).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute OmissionUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Omission strategy database error: {ex.Message}", ex);
                }
            }

            // File: OCRCorrectionService/OCRDatabaseStrategies.cs

            private async Task<DatabaseUpdateResult> CreateNewLineForOmissionAsync(
                OCRContext context,
                RegexUpdateRequest request,
                RegexCreationResponse regexResp,
                DatabaseFieldInfo fieldInfo,
                OCRCorrectionService serviceInstance)
            {
                string normalizedPattern = regexResp.RegexPattern.Replace("\\\\", "\\");

                // ENHANCED LOGIC: Use a more specific, robust pattern for known, critical omissions.
                if (fieldInfo.DatabaseFieldName == "TotalDeduction" &&
                    (request.LineText?.IndexOf("Free Shipping", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    // This specific pattern correctly captures the absolute numeric value for 'Free Shipping' deductions.  
                    normalizedPattern = @"Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)";
                    _logger.Information("Overriding with specific robust pattern for 'Free Shipping' omission: {Pattern}", normalizedPattern);
                }
                else if (fieldInfo.DatabaseFieldName == "TotalInsurance" &&
                         (request.LineText?.IndexOf("Gift Card", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    // This specific pattern correctly handles the negative value for 'Gift Card' amounts.
                    normalizedPattern = @"Gift Card Amount:\s*(?<TotalInsurance>-?\$?[\d,]+\.?\d*)";
                    _logger.Information("Overriding with specific robust pattern for 'Gift Card' omission: {Pattern}", normalizedPattern);
                }

                var newRegexEntity = await this.GetOrCreateRegexAsync(
                                         context,
                                         normalizedPattern,
                                         regexResp.IsMultiline,
                                         regexResp.MaxLines,
                                         $"For omitted field: {request.FieldName}").ConfigureAwait(false);

                if (newRegexEntity.TrackingState == TrackingState.Added)
                    await context.SaveChangesAsync().ConfigureAwait(false);


                var newLineEntity = new Lines
                {
                    PartId = request.PartId.Value,
                    RegExId = newRegexEntity.Id,
                    Name =
                                                $"AutoOmission_{request.FieldName.Replace(" ", "_").Substring(0, Math.Min(request.FieldName.Length, 40))}_{DateTime.Now:HHmmssfff}",
                    IsActive = true,
                    TrackingState = TrackingState.Added
                };
                context.Lines.Add(newLineEntity);
                await context.SaveChangesAsync().ConfigureAwait(false);


                // EXPLICITLY set appendValues to true for known aggregate fields.
                bool shouldAppend = fieldInfo.DatabaseFieldName == "TotalDeduction"
                                    || fieldInfo.DatabaseFieldName == "TotalOtherCost"
                                    || fieldInfo.DatabaseFieldName == "TotalInsurance";

                var newFieldEntity = await this.GetOrCreateFieldAsync(
                                         context,
                                         request.FieldName,
                                         fieldInfo.DatabaseFieldName,
                                         fieldInfo.EntityType,
                                         fieldInfo.DataType,
                                         newLineEntity.Id,
                                         false,
                                         shouldAppend).ConfigureAwait(false);

                if (newFieldEntity.TrackingState == TrackingState.Added)
                {
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                _logger.Information(
                    "Successfully created new Line (ID: {LineId}) and Field (ID: {FieldId}) with AppendValues={Append}",
                    newLineEntity.Id,
                    newFieldEntity.Id,
                    shouldAppend);

                return DatabaseUpdateResult.Success(newFieldEntity.Id, "Created new line, field, and regex for omission");
            }

            private async Task<int?> DeterminePartIdForNewOmissionLineAsync(
                OCRContext context,
                DatabaseFieldInfo fieldInfo,
                string originalFieldNameFromRequest,
                OCRCorrectionService serviceInstance)
            {
                string targetPartTypeName =
                    (fieldInfo?.EntityType == "InvoiceDetails"
                     || originalFieldNameFromRequest.ToLower().Contains("invoicedetail"))
                        ? "LineItem"
                        : "Header";
                var part = await context.Parts.Include(p => p.PartTypes).FirstOrDefaultAsync(
                                   p => p.PartTypes.Name.Equals(targetPartTypeName, StringComparison.OrdinalIgnoreCase))
                               .ConfigureAwait(false);
                return part?.Id ?? (await context.Parts.OrderBy(p => p.Id).FirstOrDefaultAsync().ConfigureAwait(false))
                       ?.Id;
            }
        }

        #endregion

        #region Strategy Factory

        public class DatabaseUpdateStrategyFactory
        {
            private readonly ILogger _logger;

            public DatabaseUpdateStrategyFactory(ILogger logger)
            {
                _logger = logger;
            }

            public IDatabaseUpdateStrategy GetStrategy(RegexUpdateRequest request)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (new OmissionUpdateStrategy(_logger).CanHandle(request)) return new OmissionUpdateStrategy(_logger);
                if (new FieldFormatUpdateStrategy(_logger).CanHandle(request))
                    return new FieldFormatUpdateStrategy(_logger);
                throw new InvalidOperationException(
                    $"No suitable update strategy found for correction type: {request.CorrectionType}");
            }
        }

        #endregion
    }

}