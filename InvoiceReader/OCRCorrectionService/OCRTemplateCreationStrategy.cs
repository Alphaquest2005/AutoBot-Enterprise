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
                // **STEP 1**: Create or get template (OCR-Invoices) and save it first to get ID
                var template = await this.GetOrCreateTemplateAsync(context, request.TemplateName).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false); // Save template first to get ID
                _logger.Information("✅ **TEMPLATE_ENTITY_CREATED**: Template ID={TemplateId}, Name='{TemplateName}'", template.Id, template.Name);

                // **STEP 2**: Group DeepSeek errors by entity type
                var groupedErrors = GroupErrorsByEntityType(request.AllDeepSeekErrors);
                _logger.Information("📊 **ERROR_GROUPING_COMPLETE**: Found {HeaderCount} header fields, {LineItemCount} line item patterns", 
                    groupedErrors.HeaderFields.Count, groupedErrors.LineItemPatterns.Count);

                // **STEP 3**: Create header part and fields
                var headerPart = await this.CreateHeaderPartAsync(context, template, groupedErrors.HeaderFields).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false); // Save part to get ID
                _logger.Information("✅ **HEADER_PART_CREATED**: Part ID={PartId} with {FieldCount} fields", headerPart.Id, groupedErrors.HeaderFields.Count);

                // **STEP 4**: Create line item parts for each multi-field pattern
                var lineItemParts = new List<Parts>();
                foreach (var linePattern in groupedErrors.LineItemPatterns)
                {
                    var lineItemPart = await this.CreateLineItemPartAsync(context, template, linePattern).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false); // Save each part to get ID
                    lineItemParts.Add(lineItemPart);
                    _logger.Information("✅ **LINE_ITEM_PART_CREATED**: Part ID={PartId} for pattern '{PatternName}' with {FieldCount} fields", 
                        lineItemPart.Id, linePattern.Field, linePattern.CapturedFields?.Count ?? 0);
                }

                // **STEP 5**: Create format corrections (FieldFormatRegEx entries)
                await this.CreateFormatCorrectionsAsync(context, groupedErrors.FormatCorrections).ConfigureAwait(false);
                _logger.Information("✅ **FORMAT_CORRECTIONS_CREATED**: Created {CorrectionCount} format correction rules", groupedErrors.FormatCorrections.Count);

                // **STEP 6**: Final save for any remaining changes
                _logger.Information("💾 **DATABASE_SAVE_START**: Attempting final save of remaining entities to database");
                try 
                {
                    await context.SaveChangesAsync().ConfigureAwait(false);
                    _logger.Information("💾 **DATABASE_COMMIT_SUCCESS**: All template entities saved to database");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException validationEx)
                {
                    _logger.Error("❌ **DATABASE_VALIDATION_ERROR**: Entity validation failed during template creation");
                    _logger.Error("   - **VALIDATION_EXCEPTION_TYPE**: {ExceptionType}", validationEx.GetType().FullName);
                    _logger.Error("   - **VALIDATION_EXCEPTION_MESSAGE**: {ExceptionMessage}", validationEx.Message);
                    
                    foreach (var validationErrors in validationEx.EntityValidationErrors)
                    {
                        var entityName = validationErrors.Entry.Entity.GetType().Name;
                        _logger.Error("🔍 **ENTITY_VALIDATION_ERRORS**: Entity '{EntityName}' has {ErrorCount} validation errors", 
                            entityName, validationErrors.ValidationErrors.Count());
                        
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            _logger.Error("   - **FIELD_ERROR**: Property '{PropertyName}' - {ErrorMessage}", 
                                validationError.PropertyName, validationError.ErrorMessage);
                        }
                        
                        // Log entity state and values for debugging
                        var entityEntry = validationErrors.Entry;
                        _logger.Error("🔍 **ENTITY_STATE_DEBUG**: Entity '{EntityName}' state = {EntityState}", 
                            entityName, entityEntry.State);
                        
                        var entityObject = entityEntry.Entity;
                        if (entityObject != null)
                        {
                            var properties = entityObject.GetType().GetProperties()
                                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                                .Take(10); // Limit to first 10 properties to avoid log spam
                            
                            foreach (var prop in properties)
                            {
                                try 
                                {
                                    var value = prop.GetValue(entityObject);
                                    _logger.Error("     • **{PropertyName}**: {PropertyValue}", 
                                        prop.Name, value?.ToString() ?? "NULL");
                                }
                                catch (Exception propEx)
                                {
                                    _logger.Error("     • **{PropertyName}**: ERROR_READING_PROPERTY - {Error}", 
                                        prop.Name, propEx.Message);
                                }
                            }
                        }
                    }
                    
                    throw; // Re-throw to maintain existing error handling flow
                }

                var result = new DatabaseUpdateResult
                {
                    IsSuccess = true,
                    Message = $"Successfully created template '{request.TemplateName}' with {groupedErrors.HeaderFields.Count} header fields, {lineItemParts.Count} line item patterns, and {groupedErrors.FormatCorrections.Count} format corrections",
                    RecordId = template.Id,
                    RegexId = template.Id, // CRITICAL FIX: Set RegexId to template ID for CreateInvoiceTemplateAsync compatibility
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
        /// Includes database schema validation to ensure only valid fields are included.
        /// </summary>
        private GroupedDeepSeekErrors GroupErrorsByEntityType(List<InvoiceError> allErrors)
        {
            _logger.Information("📋 **ERROR_ANALYSIS_START**: Analyzing {ErrorCount} DeepSeek errors with database schema validation", allErrors?.Count ?? 0);

            if (allErrors == null || !allErrors.Any())
            {
                _logger.Warning("⚠️ **NO_ERRORS_PROVIDED**: No DeepSeek errors to process");
                return new GroupedDeepSeekErrors();
            }

            // **CRITICAL**: Validate all errors against actual database schema before processing
            var validatedGrouped = ValidateAndFilterAgainstSchema(allErrors);

            _logger.Information("📊 **ERROR_GROUPING_SUMMARY**: {HeaderCount} header fields, {LineItemCount} line patterns, {FormatCount} format corrections (post-validation)",
                validatedGrouped.HeaderFields.Count, validatedGrouped.LineItemPatterns.Count, validatedGrouped.FormatCorrections.Count);

            return validatedGrouped;
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
                await this.CreateHeaderLineAndFieldAsync(context, headerPart, headerField).ConfigureAwait(false);
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
            var regex = await this.GetOrCreateRegexAsync(context, error.SuggestedRegex, 
                            error.RequiresMultilineRegex, this.CalculateMaxLinesFromContext(error), 
                            $"Header {error.Field} - DeepSeek suggested").ConfigureAwait(false);

            // Regex now guaranteed to have database ID from GetOrCreateRegexAsync

            // Create line with globally unique name (truncated to fit 50-char database limit)
            var truncatedField = error.Field.Length > 20 ? error.Field.Substring(0, 20) : error.Field;
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var line = new Lines
            {
                Name = $"H_{truncatedField}_{uniqueId}",
                PartId = headerPart.Id,
                RegExId = regex.Id,
                TrackingState = TrackingState.Added
            };
            context.Lines.Add(line);

            // CRITICAL FIX: Save line to database to get ID before creating Field
            if (line.Id == 0)
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Verbose("🔧 **LINE_SAVED**: Saved line to database, got ID={LineId}", line.Id);
            }

            // Create field with unique key to prevent conflicts
            var uniqueFieldId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var field = new Fields
            {
                Field = error.Field,
                Key = $"{error.Field}_{uniqueFieldId}",
                LineId = line.Id,
                EntityType = "ShipmentInvoice", // Header fields target ShipmentInvoice entity
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
            var regex = await this.GetOrCreateRegexAsync(context, multiFieldError.SuggestedRegex,
                            multiFieldError.RequiresMultilineRegex, this.CalculateMaxLinesFromContext(multiFieldError),
                            $"{template.Name} {multiFieldError.Field} - DeepSeek multi-field").ConfigureAwait(false);

            // Regex now guaranteed to have database ID from GetOrCreateRegexAsync

            // Create line with globally unique name (truncated to fit 50-char database limit)
            var truncatedField = multiFieldError.Field.Length > 15 ? multiFieldError.Field.Substring(0, 15) : multiFieldError.Field;
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var line = new Lines
            {
                Name = $"LI_{truncatedField}_{uniqueId}",
                PartId = lineItemPart.Id,
                RegExId = regex.Id,
                TrackingState = TrackingState.Added
            };
            context.Lines.Add(line);

            // CRITICAL FIX: Save line to database to get ID before creating Fields
            if (line.Id == 0)
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Verbose("🔧 **LINE_SAVED**: Saved line to database, got ID={LineId}", line.Id);
            }

            // Create fields for each captured field
            if (multiFieldError.CapturedFields?.Any() == true)
            {
                foreach (var capturedField in multiFieldError.CapturedFields)
                {
                    // Create unique field key with GUID to prevent conflicts like we do for Lines
                    var uniqueFieldId = Guid.NewGuid().ToString("N").Substring(0, 8);
                    var field = new Fields
                    {
                        Field = capturedField,
                        Key = $"InvoiceDetail_{capturedField}_{uniqueFieldId}",
                        LineId = line.Id,
                        EntityType = "InvoiceDetails", // Line item fields target InvoiceDetails entity
                        // DisplayName = ConvertToDisplayName(capturedField), // Not in schema
                        DataType = InferDataTypeFromField(capturedField, null),
                        IsRequired = IsRequiredLineItemField(capturedField),
                        TrackingState = TrackingState.Added
                    };
                    context.Fields.Add(field);
                    _logger.Verbose("✅ **LINE_ITEM_FIELD_CREATED**: {FieldName} -> {Key} for LineId={LineId}", capturedField, field.Key, line.Id);
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
                        await this.CreateSingleFormatCorrectionAsync(context, correction.Field, fieldCorrection).ConfigureAwait(false);
                    }
                }
                else if (correction.ErrorType == "format_correction")
                {
                    // Direct format correction from error
                    await this.CreateDirectFormatCorrectionAsync(context, correction).ConfigureAwait(false);
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
            var patternRegex = await this.GetOrCreateRegexAsync(context, fieldCorrection.Pattern, false, 1, 
                                   $"Format correction pattern for {fieldCorrection.FieldName}").ConfigureAwait(false);
            var replacementRegex = await this.GetOrCreateRegexAsync(context, fieldCorrection.Replacement, false, 1, 
                                       $"Format correction replacement for {fieldCorrection.FieldName}").ConfigureAwait(false);

            // Regexes now guaranteed to have database IDs from GetOrCreateRegexAsync

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

            var patternRegex = await this.GetOrCreateRegexAsync(context, pattern, false, 1, $"{description} - pattern").ConfigureAwait(false);
            var replacementRegex = await this.GetOrCreateRegexAsync(context, replacement, false, 1, $"{description} - replacement").ConfigureAwait(false);

            // Regexes now guaranteed to have database IDs from GetOrCreateRegexAsync

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
        /// Database schema validation for ShipmentInvoice and InvoiceDetails entities.
        /// Based on actual Entity Framework models from EntryDataDS.Business.Entities.
        /// </summary>
        private static class DatabaseSchema
        {
            /// <summary>
            /// Valid ShipmentInvoice entity fields with their OCR template constraints.
            /// Uses pseudo-datatypes that match the OCR template system ("String", "Number", "English Date").
            /// </summary>
            public static readonly Dictionary<string, SchemaField> ShipmentInvoiceFields = new Dictionary<string, SchemaField>
            {
                // Required fields for invoice processing
                { "InvoiceNo", new SchemaField { Type = "String", IsRequired = true } },
                
                // Important financial fields (not required but commonly expected)
                { "InvoiceDate", new SchemaField { Type = "English Date", IsRequired = false } },
                { "InvoiceTotal", new SchemaField { Type = "Number", IsRequired = false } },
                { "SubTotal", new SchemaField { Type = "Number", IsRequired = false } },
                { "TotalInternalFreight", new SchemaField { Type = "Number", IsRequired = false } },
                { "TotalOtherCost", new SchemaField { Type = "Number", IsRequired = false } },
                { "TotalInsurance", new SchemaField { Type = "Number", IsRequired = false } },
                { "TotalDeduction", new SchemaField { Type = "Number", IsRequired = false } },
                
                // Supplier information fields
                { "SupplierCode", new SchemaField { Type = "String", IsRequired = false } },
                { "SupplierName", new SchemaField { Type = "String", IsRequired = false } },
                { "SupplierAddress", new SchemaField { Type = "String", IsRequired = false } },
                { "SupplierCountry", new SchemaField { Type = "String", IsRequired = false } },
                { "ConsigneeName", new SchemaField { Type = "String", IsRequired = false } },
                { "ConsigneeAddress", new SchemaField { Type = "String", IsRequired = false } },
                { "ConsigneeCountry", new SchemaField { Type = "String", IsRequired = false } },
                { "Currency", new SchemaField { Type = "String", IsRequired = false } },
                { "EmailId", new SchemaField { Type = "String", IsRequired = false } },
                
                // System fields (usually set automatically)
                { "ImportedLines", new SchemaField { Type = "Number", IsRequired = false } },
                { "FileLineNumber", new SchemaField { Type = "Number", IsRequired = false } }
            };

            /// <summary>
            /// Valid InvoiceDetails entity fields with their OCR template constraints.
            /// Maps to ShipmentInvoiceDetails table with pseudo-datatypes for OCR templates.
            /// </summary>
            public static readonly Dictionary<string, SchemaField> InvoiceDetailsFields = new Dictionary<string, SchemaField>
            {
                // Required fields for line items (critical for invoice processing)
                { "ItemDescription", new SchemaField { Type = "String", IsRequired = true } },
                { "Quantity", new SchemaField { Type = "Number", IsRequired = true } },
                { "Cost", new SchemaField { Type = "Number", IsRequired = true } }, // Maps to UnitPrice from DeepSeek
                
                // Important optional line item fields  
                { "LineNumber", new SchemaField { Type = "Number", IsRequired = false } },
                { "ItemNumber", new SchemaField { Type = "String", IsRequired = false } }, // Maps to ItemCode from DeepSeek
                { "Units", new SchemaField { Type = "String", IsRequired = false } },
                { "TotalCost", new SchemaField { Type = "Number", IsRequired = false } }, // Maps to LineTotal from DeepSeek
                { "Discount", new SchemaField { Type = "Number", IsRequired = false } },
                { "TariffCode", new SchemaField { Type = "String", IsRequired = false } },
                { "Category", new SchemaField { Type = "String", IsRequired = false } },
                { "CategoryTariffCode", new SchemaField { Type = "String", IsRequired = false } },
                
                // System fields (usually set automatically)
                { "FileLineNumber", new SchemaField { Type = "Number", IsRequired = false } },
                { "InventoryItemId", new SchemaField { Type = "Number", IsRequired = false } },
                { "SalesFactor", new SchemaField { Type = "Number", IsRequired = false } } // Set automatically to 1.0
            };

            /// <summary>
            /// Field mapping from DeepSeek/OCR names to actual database field names.
            /// </summary>
            public static readonly Dictionary<string, string> FieldNameMapping = new Dictionary<string, string>
            {
                // Header field mappings (exact matches mostly)
                { "InvoiceNo", "InvoiceNo" },
                { "InvoiceDate", "InvoiceDate" },
                { "InvoiceTotal", "InvoiceTotal" },
                { "SubTotal", "SubTotal" },
                { "SupplierName", "SupplierName" },
                { "SupplierCode", "SupplierCode" },
                { "Currency", "Currency" },
                { "TotalInternalFreight", "TotalInternalFreight" },
                { "TotalOtherCost", "TotalOtherCost" },
                { "TotalInsurance", "TotalInsurance" },
                { "TotalDeduction", "TotalDeduction" },
                
                // Line item field mappings (DeepSeek → Database)
                { "ItemDescription", "ItemDescription" }, // Direct match
                { "UnitPrice", "Cost" }, // DeepSeek UnitPrice → Database Cost
                { "ItemCode", "ItemNumber" }, // DeepSeek ItemCode → Database ItemNumber
                { "Quantity", "Quantity" }, // Direct match
                { "LineTotal", "TotalCost" }, // DeepSeek LineTotal → Database TotalCost
                
                // Fields to ignore (not in database schema)
                { "Size", null }, // Size field doesn't exist in database
                { "Color", null }, // Color field doesn't exist in database
                { "SKU", null } // SKU field doesn't exist in database
            };
        }

        /// <summary>
        /// Schema field definition with OCR template constraints.
        /// </summary>
        public class SchemaField
        {
            public string Type { get; set; }
            public bool IsRequired { get; set; }
        }

        /// <summary>
        /// Validates and filters DeepSeek errors against actual database schema.
        /// Ensures only valid database fields are included in templates.
        /// </summary>
        private GroupedDeepSeekErrors ValidateAndFilterAgainstSchema(List<InvoiceError> allErrors)
        {
            _logger.Information("🔍 **SCHEMA_VALIDATION_START**: Validating {ErrorCount} DeepSeek errors against database schema", allErrors?.Count ?? 0);
            
            var grouped = new GroupedDeepSeekErrors();
            var invalidFields = new List<string>();
            var missingRequiredFields = new List<string>();
            
            if (allErrors == null || !allErrors.Any())
            {
                _logger.Warning("⚠️ **NO_ERRORS_FOR_VALIDATION**: No DeepSeek errors provided for schema validation");
                return grouped;
            }

            foreach (var error in allErrors)
            {
                _logger.Verbose("🔍 **VALIDATING_ERROR**: Field='{Field}', Type='{Type}'", error.Field, error.ErrorType);
                
                var isLineItemError = IsLineItemFieldBySchema(error.Field);
                var validatedError = ValidateErrorAgainstSchema(error, isLineItemError);
                
                if (validatedError != null)
                {
                    // Categorize the validated error
                    switch (error.ErrorType?.ToLower())
                    {
                        case "omission":
                        case "format_correction":
                            if (isLineItemError)
                                grouped.FormatCorrections.Add(validatedError);
                            else
                                grouped.HeaderFields.Add(validatedError);
                            break;
                        case "multi_field_omission":
                            grouped.LineItemPatterns.Add(validatedError);
                            break;
                    }
                    
                    _logger.Verbose("✅ **FIELD_VALIDATED**: {Field} → Accepted for template", validatedError.Field);
                }
                else
                {
                    invalidFields.Add(error.Field);
                    _logger.Warning("❌ **FIELD_REJECTED**: {Field} → Not valid database field", error.Field);
                }
            }

            // Check for missing required fields
            CheckForMissingRequiredFields(grouped, missingRequiredFields);

            // Log validation summary
            _logger.Information("📊 **SCHEMA_VALIDATION_SUMMARY**: {ValidCount} valid fields, {InvalidCount} invalid fields, {MissingCount} missing required fields",
                grouped.HeaderFields.Count + grouped.LineItemPatterns.Count + grouped.FormatCorrections.Count,
                invalidFields.Count, missingRequiredFields.Count);

            if (invalidFields.Any())
            {
                _logger.Warning("⚠️ **INVALID_FIELDS_FOUND**: {InvalidFields}", string.Join(", ", invalidFields));
            }

            if (missingRequiredFields.Any())
            {
                _logger.Error("❌ **MISSING_REQUIRED_FIELDS**: {MissingFields}", string.Join(", ", missingRequiredFields));
            }

            return grouped;
        }

        /// <summary>
        /// Validates a single DeepSeek error against database schema.
        /// </summary>
        private InvoiceError ValidateErrorAgainstSchema(InvoiceError error, bool isLineItem)
        {
            var schemaFields = isLineItem ? DatabaseSchema.InvoiceDetailsFields : DatabaseSchema.ShipmentInvoiceFields;
            var fieldName = GetMappedFieldName(error.Field);
            
            // Check if field should be ignored (mapped to null)
            if (fieldName == null)
            {
                _logger.Verbose("🚫 **FIELD_IGNORED**: {Field} → Mapped to null, skipping", error.Field);
                return null;
            }
            
            // Check if mapped field exists in schema
            if (!schemaFields.ContainsKey(fieldName))
            {
                _logger.Warning("❌ **FIELD_NOT_IN_SCHEMA**: {Field} → {MappedField} not found in {Entity} schema", 
                    error.Field, fieldName, isLineItem ? "InvoiceDetails" : "ShipmentInvoice");
                return null;
            }

            var schemaField = schemaFields[fieldName];
            
            // Validate captured fields for multi-field errors
            if (error.CapturedFields?.Any() == true)
            {
                var validCapturedFields = new List<string>();
                foreach (var capturedField in error.CapturedFields)
                {
                    var mappedCapturedField = GetMappedFieldName(capturedField);
                    if (mappedCapturedField != null && schemaFields.ContainsKey(mappedCapturedField))
                    {
                        validCapturedFields.Add(capturedField); // Keep original name for DeepSeek compatibility
                        _logger.Verbose("✅ **CAPTURED_FIELD_VALID**: {Field} → {MappedField}", capturedField, mappedCapturedField);
                    }
                    else
                    {
                        _logger.Warning("❌ **CAPTURED_FIELD_INVALID**: {Field} → {MappedField} not in schema", capturedField, mappedCapturedField ?? "null");
                    }
                }
                
                if (!validCapturedFields.Any())
                {
                    _logger.Warning("❌ **NO_VALID_CAPTURED_FIELDS**: Error {Field} has no valid captured fields", error.Field);
                    return null;
                }
                
                // Update error with only valid captured fields
                error.CapturedFields = validCapturedFields;
            }

            // Create a new validated error with database-compatible field name
            var validatedError = new InvoiceError
            {
                Field = fieldName, // Use database field name
                ErrorType = error.ErrorType,
                ExtractedValue = error.ExtractedValue,
                CorrectValue = error.CorrectValue,
                LineText = error.LineText,
                LineNumber = error.LineNumber,
                Confidence = error.Confidence,
                SuggestedRegex = error.SuggestedRegex,
                CapturedFields = error.CapturedFields, // Already validated above
                FieldCorrections = error.FieldCorrections,
                Reasoning = error.Reasoning,
                RequiresMultilineRegex = error.RequiresMultilineRegex,
                ContextLinesBefore = error.ContextLinesBefore,
                ContextLinesAfter = error.ContextLinesAfter
            };
            
            _logger.Verbose("✅ **ERROR_VALIDATED**: {OriginalField} → {DatabaseField} ({EntityType})", 
                error.Field, fieldName, isLineItem ? "InvoiceDetails" : "ShipmentInvoice");
                
            return validatedError;
        }

        /// <summary>
        /// Maps DeepSeek/OCR field names to actual database field names.
        /// </summary>
        private string GetMappedFieldName(string originalField)
        {
            if (DatabaseSchema.FieldNameMapping.TryGetValue(originalField, out var mappedField))
            {
                return mappedField; // May be null for ignored fields
            }
            
            // If no explicit mapping, check if field exists directly in either schema
            if (DatabaseSchema.ShipmentInvoiceFields.ContainsKey(originalField) || 
                DatabaseSchema.InvoiceDetailsFields.ContainsKey(originalField))
            {
                return originalField; // Direct match
            }
            
            return null; // Invalid field
        }

        /// <summary>
        /// Determines if a field belongs to line items vs header based on database schema.
        /// </summary>
        private bool IsLineItemFieldBySchema(string fieldName)
        {
            var mappedField = GetMappedFieldName(fieldName);
            if (mappedField == null) return false;
            
            return DatabaseSchema.InvoiceDetailsFields.ContainsKey(mappedField);
        }

        /// <summary>
        /// Checks for missing required fields and logs warnings.
        /// </summary>
        private void CheckForMissingRequiredFields(GroupedDeepSeekErrors grouped, List<string> missingRequiredFields)
        {
            // Check required header fields
            var requiredHeaderFields = DatabaseSchema.ShipmentInvoiceFields
                .Where(kvp => kvp.Value.IsRequired)
                .Select(kvp => kvp.Key)
                .ToList();
                
            var presentHeaderFields = grouped.HeaderFields.Select(e => e.Field).ToHashSet();
            
            foreach (var requiredField in requiredHeaderFields)
            {
                if (!presentHeaderFields.Contains(requiredField))
                {
                    missingRequiredFields.Add($"ShipmentInvoice.{requiredField}");
                    _logger.Warning("⚠️ **MISSING_REQUIRED_HEADER_FIELD**: {Field}", requiredField);
                }
            }

            // Check required line item fields
            var requiredLineFields = DatabaseSchema.InvoiceDetailsFields
                .Where(kvp => kvp.Value.IsRequired)
                .Select(kvp => kvp.Key)
                .ToList();
                
            var hasLineItemPattern = grouped.LineItemPatterns.Any();
            
            if (hasLineItemPattern)
            {
                var allCapturedFields = grouped.LineItemPatterns
                    .SelectMany(e => e.CapturedFields ?? new List<string>())
                    .Select(f => GetMappedFieldName(f))
                    .Where(f => f != null)
                    .ToHashSet();
                    
                foreach (var requiredField in requiredLineFields)
                {
                    if (!allCapturedFields.Contains(requiredField))
                    {
                        missingRequiredFields.Add($"InvoiceDetails.{requiredField}");
                        _logger.Warning("⚠️ **MISSING_REQUIRED_LINE_FIELD**: {Field}", requiredField);
                    }
                }
            }
        }

        /// <summary>
        /// Infers appropriate data type from field name and sample value.
        /// Uses production-compatible DataType values that match ImportByDataType.cs processing.
        /// </summary>
        private string InferDataTypeFromField(string fieldName, string sampleValue)
        {
            var lowerField = fieldName.ToLower();

            if (lowerField.Contains("date"))
                return "English Date"; // Matches production code in ImportByDataType.cs line 73
            if (lowerField.Contains("price") || lowerField.Contains("total") || lowerField.Contains("amount") || 
                lowerField.Contains("quantity") || lowerField.Contains("cost") || lowerField.Contains("freight") ||
                lowerField.Contains("insurance") || lowerField.Contains("deduction"))
                return "Number"; // Matches production code in ImportByDataType.cs line 68
            
            return "String"; // Matches production code in ImportByDataType.cs line 65 (capital S)
        }

        /// <summary>
        /// Determines if a header field is required based on database schema.
        /// </summary>
        private bool IsRequiredField(string fieldName)
        {
            return DatabaseSchema.ShipmentInvoiceFields.TryGetValue(fieldName, out var field) && field.IsRequired;
        }

        /// <summary>
        /// Determines if a line item field is required based on database schema.
        /// </summary>
        private bool IsRequiredLineItemField(string fieldName)
        {
            return DatabaseSchema.InvoiceDetailsFields.TryGetValue(fieldName, out var field) && field.IsRequired;
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