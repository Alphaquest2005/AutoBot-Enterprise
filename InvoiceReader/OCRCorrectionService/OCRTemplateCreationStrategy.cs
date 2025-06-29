// File: OCRCorrectionService/OCRTemplateCreationStrategy.cs
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OCR.Business.Entities;
using Serilog;
using TrackableEntities;
using Newtonsoft.Json;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        /// <summary>
        /// Creates complete OCR templates from DeepSeek error detection results.
        /// Handles unknown suppliers by dynamically creating all required database entities.
        /// </summary>
        public class TemplateCreationStrategy : DatabaseUpdateStrategyBase
    {
        public override string StrategyType => "template_creation";

        public TemplateCreationStrategy(ILogger logger) : base(logger) { }

        public override bool CanHandle(RegexUpdateRequest request)
        {
            // Handle requests that create new templates from DeepSeek corrections
            return request.ErrorType == "template_creation" || 
                   (request.TemplateName != null && request.CreateNewTemplate);
        }

        public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
        {
            _logger.Information("🏗️ **TEMPLATE_CREATION_START**: Creating new template '{TemplateName}' from DeepSeek corrections", request.TemplateName);
            
            try
            {
                // **STEP 1**: Create or get template (OCR-Invoices)
                var template = await GetOrCreateTemplateAsync(context, request.TemplateName);
                _logger.Information("✅ **TEMPLATE_ENTITY_CREATED**: Template ID={TemplateId}, Name='{TemplateName}'", template.Id, template.Name);

                // **STEP 2**: Group DeepSeek errors by entity type
                var groupedErrors = GroupErrorsByEntityType(request.AllDeepSeekErrors);
                _logger.Information("📊 **ERROR_GROUPING_COMPLETE**: Found {HeaderCount} header fields, {LineItemCount} line item patterns", 
                    groupedErrors.HeaderFields.Count, groupedErrors.LineItemPatterns.Count);

                // **STEP 3**: Create header part and fields
                var headerPart = await CreateHeaderPartAsync(context, template, groupedErrors.HeaderFields);
                _logger.Information("✅ **HEADER_PART_CREATED**: Part ID={PartId} with {FieldCount} fields", headerPart.Id, groupedErrors.HeaderFields.Count);

                // **STEP 4**: Create line item parts for each multi-field pattern
                var lineItemParts = new List<Parts>();
                foreach (var linePattern in groupedErrors.LineItemPatterns)
                {
                    var lineItemPart = await CreateLineItemPartAsync(context, template, linePattern);
                    lineItemParts.Add(lineItemPart);
                    _logger.Information("✅ **LINE_ITEM_PART_CREATED**: Part ID={PartId} for pattern '{PatternName}' with {FieldCount} fields", 
                        lineItemPart.Id, linePattern.Field, linePattern.CapturedFields?.Count ?? 0);
                }

                // **STEP 5**: Create format corrections (FieldFormatRegEx entries)
                await CreateFormatCorrectionsAsync(context, groupedErrors.FormatCorrections);
                _logger.Information("✅ **FORMAT_CORRECTIONS_CREATED**: Created {CorrectionCount} format correction rules", groupedErrors.FormatCorrections.Count);

                // **STEP 6**: Save all changes
                await context.SaveChangesAsync();
                _logger.Information("💾 **DATABASE_COMMIT_SUCCESS**: All template entities saved to database");

                var result = new DatabaseUpdateResult
                {
                    IsSuccess = true,
                    Message = $"Successfully created template '{request.TemplateName}' with {groupedErrors.HeaderFields.Count} header fields, {lineItemParts.Count} line item patterns, and {groupedErrors.FormatCorrections.Count} format corrections",
                    RecordId = template.Id,
                    FieldsCreated = groupedErrors.HeaderFields.Count + groupedErrors.LineItemPatterns.Sum(p => p.CapturedFields?.Count ?? 0),
                    LinesCreated = 1 + lineItemParts.Count, // Header + line items
                    PartsCreated = 1 + lineItemParts.Count,
                    TemplateCreated = true
                };

                _logger.Information("🎯 **TEMPLATE_CREATION_SUCCESS**: {Message}", result.Message);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_CREATION_ERROR**: Failed to create template '{TemplateName}'", request.TemplateName);
                return new DatabaseUpdateResult
                {
                    IsSuccess = false,
                    Message = $"Template creation failed: {ex.Message}",
                    Exception = ex
                };
            }
        }

        #region Template Creation Core Methods

        /// <summary>
        /// Creates or retrieves the main template entity (OCR-Invoices table).
        /// </summary>
        private async Task<Invoices> GetOrCreateTemplateAsync(OCRContext context, string templateName)
        {
            _logger.Information("🔍 **TEMPLATE_LOOKUP**: Searching for existing template '{TemplateName}'", templateName);

            var existingTemplate = await context.Invoices
                .FirstOrDefaultAsync(t => t.Name == templateName)
                .ConfigureAwait(false);

            if (existingTemplate != null)
            {
                _logger.Information("♻️ **TEMPLATE_EXISTS**: Found existing template ID={TemplateId}", existingTemplate.Id);
                return existingTemplate;
            }

            var newTemplate = new Invoices
            {
                Name = templateName,
                FileTypeId = 1147, // Standard ShipmentInvoice FileType
                ApplicationSettingsId = 3, // Standard application settings
                IsActive = true,
                TrackingState = TrackingState.Added
            };

            context.Invoices.Add(newTemplate);
            _logger.Information("🆕 **TEMPLATE_CREATED**: New template '{TemplateName}' prepared for database", templateName);
            return newTemplate;
        }

        /// <summary>
        /// Groups DeepSeek errors by their target entity type for structured processing.
        /// </summary>
        private GroupedDeepSeekErrors GroupErrorsByEntityType(List<InvoiceError> allErrors)
        {
            _logger.Information("📋 **ERROR_ANALYSIS_START**: Analyzing {ErrorCount} DeepSeek errors", allErrors?.Count ?? 0);

            var grouped = new GroupedDeepSeekErrors();

            if (allErrors == null || !allErrors.Any())
            {
                _logger.Warning("⚠️ **NO_ERRORS_PROVIDED**: No DeepSeek errors to process");
                return grouped;
            }

            foreach (var error in allErrors)
            {
                _logger.Verbose("🔍 **ERROR_ANALYSIS**: Processing error - Field='{Field}', Type='{Type}', HasCapturedFields={HasFields}", 
                    error.Field, error.ErrorType, error.CapturedFields?.Any() == true);

                switch (error.ErrorType?.ToLower())
                {
                    case "omission":
                    case "format_correction":
                        // Single field errors go to header (unless they're line item specific)
                        if (!IsLineItemField(error.Field))
                        {
                            grouped.HeaderFields.Add(error);
                            _logger.Verbose("📝 **HEADER_FIELD_ADDED**: {Field}", error.Field);
                        }
                        else
                        {
                            grouped.FormatCorrections.Add(error);
                            _logger.Verbose("🔧 **FORMAT_CORRECTION_ADDED**: {Field}", error.Field);
                        }
                        break;

                    case "multi_field_omission":
                        // Multi-field patterns become line item parts
                        grouped.LineItemPatterns.Add(error);
                        _logger.Verbose("📦 **LINE_ITEM_PATTERN_ADDED**: {Field} with {FieldCount} captured fields", 
                            error.Field, error.CapturedFields?.Count ?? 0);
                        break;

                    default:
                        _logger.Warning("⚠️ **UNKNOWN_ERROR_TYPE**: Unhandled error type '{Type}' for field '{Field}'", error.ErrorType, error.Field);
                        break;
                }
            }

            _logger.Information("📊 **ERROR_GROUPING_SUMMARY**: {HeaderCount} header fields, {LineItemCount} line patterns, {FormatCount} format corrections",
                grouped.HeaderFields.Count, grouped.LineItemPatterns.Count, grouped.FormatCorrections.Count);

            return grouped;
        }

        /// <summary>
        /// Creates the header part (PartTypeId=1) with all invoice-level fields.
        /// </summary>
        private async Task<Parts> CreateHeaderPartAsync(OCRContext context, Invoices template, List<InvoiceError> headerFields)
        {
            _logger.Information("🏗️ **HEADER_PART_CREATION**: Creating header part for template '{TemplateName}'", template.Name);

            var headerPart = new Parts
            {
                TemplateId = template.Id,
                PartTypeId = 1, // Header part type
                TrackingState = TrackingState.Added
            };
            context.Parts.Add(headerPart);

            // Create lines and fields for each header field
            foreach (var headerField in headerFields)
            {
                await CreateHeaderLineAndFieldAsync(context, headerPart, headerField);
                _logger.Verbose("✅ **HEADER_FIELD_PROCESSED**: {Field}", headerField.Field);
            }

            _logger.Information("✅ **HEADER_PART_COMPLETE**: Created header part with {FieldCount} fields", headerFields.Count);
            return headerPart;
        }

        /// <summary>
        /// Creates a single header line and its associated field from a DeepSeek error.
        /// </summary>
        private async Task CreateHeaderLineAndFieldAsync(OCRContext context, Parts headerPart, InvoiceError error)
        {
            _logger.Verbose("🔧 **HEADER_LINE_CREATION**: Creating line for field '{Field}'", error.Field);

            // Create regex pattern
            var regex = await GetOrCreateRegexAsync(context, error.SuggestedRegex, 
                error.RequiresMultilineRegex, CalculateMaxLinesFromContext(error), 
                $"Header {error.Field} - DeepSeek suggested");

            // Create line
            var line = new Lines
            {
                Name = error.Field,
                PartId = headerPart.Id,
                RegExId = regex.Id,
                TrackingState = TrackingState.Added
            };
            context.Lines.Add(line);

            // Create field
            var field = new Fields
            {
                Field = error.Field,
                Key = error.Field,
                LineId = line.Id,
                // DisplayName = ConvertToDisplayName(error.Field), // Not in schema
                DataType = InferDataTypeFromField(error.Field, error.CorrectValue),
                IsRequired = IsRequiredField(error.Field),
                TrackingState = TrackingState.Added
            };
            context.Fields.Add(field);

            _logger.Verbose("✅ **HEADER_LINE_COMPLETE**: Line='{LineName}', Field='{FieldName}', DataType='{DataType}'", 
                line.Name, field.Field, field.DataType);
        }

        /// <summary>
        /// Creates a line item part (PartTypeId=2) for multi-field patterns.
        /// </summary>
        private async Task<Parts> CreateLineItemPartAsync(OCRContext context, Invoices template, InvoiceError multiFieldError)
        {
            _logger.Information("🏗️ **LINE_ITEM_PART_CREATION**: Creating line item part for '{FieldName}'", multiFieldError.Field);

            var lineItemPart = new Parts
            {
                TemplateId = template.Id,
                PartTypeId = 2, // Line item part type
                TrackingState = TrackingState.Added
            };
            context.Parts.Add(lineItemPart);

            // Create the multi-field line
            var regex = await GetOrCreateRegexAsync(context, multiFieldError.SuggestedRegex,
                multiFieldError.RequiresMultilineRegex, CalculateMaxLinesFromContext(multiFieldError),
                $"{template.Name} {multiFieldError.Field} - DeepSeek multi-field");

            var line = new Lines
            {
                Name = multiFieldError.Field,
                PartId = lineItemPart.Id,
                RegExId = regex.Id,
                TrackingState = TrackingState.Added
            };
            context.Lines.Add(line);

            // Create fields for each captured field
            if (multiFieldError.CapturedFields?.Any() == true)
            {
                foreach (var capturedField in multiFieldError.CapturedFields)
                {
                    var field = new Fields
                    {
                        Field = capturedField,
                        Key = $"InvoiceDetail_{capturedField}",
                        LineId = line.Id,
                        // DisplayName = ConvertToDisplayName(capturedField), // Not in schema
                        DataType = InferDataTypeFromField(capturedField, null),
                        IsRequired = IsRequiredLineItemField(capturedField),
                        TrackingState = TrackingState.Added
                    };
                    context.Fields.Add(field);
                    _logger.Verbose("✅ **LINE_ITEM_FIELD_CREATED**: {FieldName} -> {Key}", capturedField, field.Key);
                }
            }

            _logger.Information("✅ **LINE_ITEM_PART_COMPLETE**: Created part with {FieldCount} fields", 
                multiFieldError.CapturedFields?.Count ?? 0);
            return lineItemPart;
        }

        /// <summary>
        /// Creates FieldFormatRegEx entries for automatic data format corrections.
        /// </summary>
        private async Task CreateFormatCorrectionsAsync(OCRContext context, List<InvoiceError> formatCorrections)
        {
            _logger.Information("🔧 **FORMAT_CORRECTIONS_START**: Creating {CorrectionCount} format correction rules", formatCorrections.Count);

            foreach (var correction in formatCorrections)
            {
                if (correction.FieldCorrections?.Any() == true)
                {
                    foreach (var fieldCorrection in correction.FieldCorrections)
                    {
                        await CreateSingleFormatCorrectionAsync(context, correction.Field, fieldCorrection);
                    }
                }
                else if (correction.ErrorType == "format_correction")
                {
                    // Direct format correction from error
                    await CreateDirectFormatCorrectionAsync(context, correction);
                }
            }

            _logger.Information("✅ **FORMAT_CORRECTIONS_COMPLETE**: All format correction rules created");
        }

        /// <summary>
        /// Creates a single FieldFormatRegEx entry from a field correction specification.
        /// </summary>
        private async Task CreateSingleFormatCorrectionAsync(OCRContext context, string parentField, FieldCorrection fieldCorrection)
        {
            _logger.Verbose("🔧 **FORMAT_CORRECTION_CREATION**: Field='{Field}', Pattern='{Pattern}' -> '{Replacement}'", 
                fieldCorrection.FieldName, fieldCorrection.Pattern, fieldCorrection.Replacement);

            // Find the field entity
            var field = await context.Fields
                .FirstOrDefaultAsync(f => f.Field == fieldCorrection.FieldName || f.Key.EndsWith($"_{fieldCorrection.FieldName}"))
                .ConfigureAwait(false);

            if (field == null)
            {
                _logger.Warning("⚠️ **FIELD_NOT_FOUND**: Cannot create format correction for unknown field '{FieldName}'", fieldCorrection.FieldName);
                return;
            }

            // Create pattern and replacement regexes
            var patternRegex = await GetOrCreateRegexAsync(context, fieldCorrection.Pattern, false, 1, 
                $"Format correction pattern for {fieldCorrection.FieldName}");
            var replacementRegex = await GetOrCreateRegexAsync(context, fieldCorrection.Replacement, false, 1, 
                $"Format correction replacement for {fieldCorrection.FieldName}");

            // Create the format correction
            var formatCorrection = new FieldFormatRegEx
            {
                FieldId = field.Id,
                RegExId = patternRegex.Id,
                ReplacementRegExId = replacementRegex.Id,
                TrackingState = TrackingState.Added
            };
            context.OCR_FieldFormatRegEx.Add(formatCorrection);

            _logger.Verbose("✅ **FORMAT_CORRECTION_CREATED**: FieldId={FieldId}, Pattern='{Pattern}' -> '{Replacement}'", 
                field.Id, fieldCorrection.Pattern, fieldCorrection.Replacement);
        }

        /// <summary>
        /// Creates format correction directly from error (e.g., Date, Currency standards).
        /// </summary>
        private async Task CreateDirectFormatCorrectionAsync(OCRContext context, InvoiceError error)
        {
            _logger.Verbose("🔧 **DIRECT_FORMAT_CORRECTION**: Field='{Field}', Type='{Type}'", error.Field, error.ErrorType);

            // Find the field
            var field = await context.Fields
                .FirstOrDefaultAsync(f => f.Field == error.Field)
                .ConfigureAwait(false);

            if (field == null)
            {
                _logger.Warning("⚠️ **FIELD_NOT_FOUND**: Cannot create direct format correction for unknown field '{Field}'", error.Field);
                return;
            }

            string pattern, replacement, description;

            // Generate pattern and replacement based on field type
            switch (error.Field.ToLower())
            {
                case "currency":
                    pattern = @"US[S$]";
                    replacement = "USD";
                    description = "Currency standardization to ISO 3-letter codes";
                    break;

                case "invoicedate":
                    pattern = @"\w+,\s*(\w+)\s*(\d+),\s*(\d+)\s*at\s*\d+:\d+\s*[AP]M\s*\w+";
                    replacement = "$3/$2/$1"; // Convert to MM/dd/yyyy
                    description = "Date format conversion to MM/dd/yyyy";
                    break;

                default:
                    _logger.Warning("⚠️ **UNKNOWN_FORMAT_CORRECTION**: No format correction defined for field '{Field}'", error.Field);
                    return;
            }

            var patternRegex = await GetOrCreateRegexAsync(context, pattern, false, 1, $"{description} - pattern");
            var replacementRegex = await GetOrCreateRegexAsync(context, replacement, false, 1, $"{description} - replacement");

            var formatCorrection = new FieldFormatRegEx
            {
                FieldId = field.Id,
                RegExId = patternRegex.Id,
                ReplacementRegExId = replacementRegex.Id,
                TrackingState = TrackingState.Added
            };
            context.OCR_FieldFormatRegEx.Add(formatCorrection);

            _logger.Verbose("✅ **DIRECT_FORMAT_CORRECTION_CREATED**: Field='{Field}', Pattern='{Pattern}' -> '{Replacement}'", 
                error.Field, pattern, replacement);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines if a field belongs to line items vs header.
        /// </summary>
        private bool IsLineItemField(string fieldName)
        {
            var lineItemFields = new[] { "ItemDescription", "ItemCode", "UnitPrice", "Quantity", "LineTotal", "Size", "Color", "SKU" };
            return lineItemFields.Any(f => fieldName.Contains(f));
        }

        /// <summary>
        /// Infers appropriate data type from field name and sample value.
        /// </summary>
        private string InferDataTypeFromField(string fieldName, string sampleValue)
        {
            var lowerField = fieldName.ToLower();

            if (lowerField.Contains("date"))
                return "shortdateformat";
            if (lowerField.Contains("price") || lowerField.Contains("total") || lowerField.Contains("amount"))
                return "decimal";
            if (lowerField.Contains("quantity"))
                return "int";
            
            return "string";
        }

        /// <summary>
        /// Determines if a header field is required.
        /// </summary>
        private bool IsRequiredField(string fieldName)
        {
            var requiredFields = new[] { "InvoiceNo", "InvoiceTotal", "SubTotal", "SupplierName" };
            return requiredFields.Contains(fieldName);
        }

        /// <summary>
        /// Determines if a line item field is required.
        /// </summary>
        private bool IsRequiredLineItemField(string fieldName)
        {
            var requiredFields = new[] { "ItemDescription", "UnitPrice" };
            return requiredFields.Contains(fieldName);
        }

        /// <summary>
        /// Converts field names to user-friendly display names.
        /// </summary>
        private string ConvertToDisplayName(string fieldName)
        {
            return System.Text.RegularExpressions.Regex.Replace(fieldName, "([a-z])([A-Z])", "$1 $2");
        }

        /// <summary>
        /// Calculates max lines needed from error context.
        /// </summary>
        private int CalculateMaxLinesFromContext(InvoiceError error)
        {
            int contextLines = 0;
            if (error.ContextLinesBefore?.Count > 0) contextLines += error.ContextLinesBefore.Count;
            if (error.ContextLinesAfter?.Count > 0) contextLines += error.ContextLinesAfter.Count;
            
            return error.RequiresMultilineRegex ? Math.Max(contextLines + 2, 3) : 1;
        }

        #endregion
    }

    #region Supporting Data Structures

    /// <summary>
    /// Groups DeepSeek errors by their processing requirements.
    /// </summary>
    public class GroupedDeepSeekErrors
    {
        public List<InvoiceError> HeaderFields { get; set; } = new List<InvoiceError>();
        public List<InvoiceError> LineItemPatterns { get; set; } = new List<InvoiceError>();
        public List<InvoiceError> FormatCorrections { get; set; } = new List<InvoiceError>();
    }

    // Note: Using DatabaseUpdateResult and RegexUpdateRequest from OCRDataModels.cs
    // These extensions are added via partial classes in that file

    /// <summary>
    /// Result of template creation operation with comprehensive details for LLM diagnosis.
    /// </summary>
    public class TemplateCreationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int PartsCreated { get; set; }
        public int LinesCreated { get; set; }
        public int FieldsCreated { get; set; }
        public int FormatCorrectionsCreated { get; set; }
        public int ErrorsProcessed { get; set; }
        public Exception Exception { get; set; }

        /// <summary>
        /// Comprehensive summary for LLM analysis and troubleshooting.
        /// </summary>
        public string GetDetailedSummary()
        {
            if (Success)
            {
                return $"✅ **TEMPLATE_CREATION_SUCCESS**: Template '{TemplateName}' (ID: {TemplateId}) created with {PartsCreated} parts, {LinesCreated} lines, {FieldsCreated} fields, and {FormatCorrectionsCreated} format corrections from {ErrorsProcessed} DeepSeek errors.";
            }
            else
            {
                return $"❌ **TEMPLATE_CREATION_FAILURE**: Template '{TemplateName}' creation failed - {Message}" + 
                       (Exception != null ? $" | Exception: {Exception.Message}" : "");
            }
        }
    }

    #endregion

    } // End OCRCorrectionService partial class
}