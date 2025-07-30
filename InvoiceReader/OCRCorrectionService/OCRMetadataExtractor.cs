// File: OCRCorrectionService/OCRMetadataExtractor.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; // For OCRContext and Include methods like .Include()
using Serilog; // ILogger is available as this._logger
using OCR.Business.Entities; // For Invoice, Part, Line, Fields, OcrInvoices, PartTypes, RegularExpressions, FieldFormatRegEx, OCRContext

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced OCR Metadata Extraction

        /// <summary>
        /// Main instance method to extract comprehensive OCRFieldMetadata for all relevant fields
        /// found in runtimeInvoiceData, by cross-referencing with the ocrTemplate.
        /// </summary>
        /// <param name="runtimeInvoiceData">Data from the current OCR run (dynamic object or dictionary).</param>
        /// <param name="ocrTemplate">The OCR.Business.Entities.Invoice template used for the extraction.</param>
        /// <param name="precomputedMappings">Optional: A precomputed dictionary mapping canonical field names
        /// (like "InvoiceTotal") to their (LineId, FieldId, PartId) in the template. If null, mappings are generated.</param>
        /// <returns>A dictionary where the key is the runtime field name and the value is the corresponding OCRFieldMetadata.</returns>
        public Dictionary<string, OCRFieldMetadata> ExtractEnhancedOCRMetadata(
            IDictionary<string, object> runtimeInvoiceData,
            Template ocrTemplate, // This is OCR.Business.Entities.Invoice
            Dictionary<string, (int LineId, int FieldId, int? PartId)> precomputedMappings = null)
        {
            var metadataOutput = new Dictionary<string, OCRFieldMetadata>();
            if (runtimeInvoiceData == null || ocrTemplate == null)
            {
                _logger.Warning("ExtractEnhancedOCRMetadata: runtimeInvoiceData or ocrTemplate is null. Cannot extract metadata.");
                return metadataOutput;
            }

            // Use OcrInvoices.Id if available for logging, otherwise fallback to Invoice.Id
            var templateLogId = ocrTemplate.OcrTemplates?.Id.ToString() ?? ocrTemplate.OcrTemplates.Id.ToString();
            _logger.Debug("Starting metadata extraction against OCR Template (Log ID: {TemplateLogId}) for {FieldCount} runtime fields.", templateLogId, runtimeInvoiceData.Count);

            var fieldMappings = precomputedMappings ?? this.CreateEnhancedFieldMapping(ocrTemplate);
            var invoiceContextData = this.GetInvoiceContext(ocrTemplate);

            foreach (var kvp in runtimeInvoiceData)
            {
                var runtimeFieldName = kvp.Key;
                var runtimeFieldValue = kvp.Value?.ToString();

                if (IsMetadataField(runtimeFieldName)) // Static call from OCRUtilities part of this partial class
                {
                    _logger.Verbose("Skipping metadata field: {FieldName}", runtimeFieldName);
                    continue;
                }

                string baseFieldNameForMapping = runtimeFieldName;
                int? detailLineNumberHint = null;

                var underscoreParts = runtimeFieldName.Split('_');
                if (underscoreParts.Length > 1 && underscoreParts.Last().StartsWith("L", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(underscoreParts.Last().Substring(1), out int ln)) // CORRECTED: Use Substring
                {
                    baseFieldNameForMapping = string.Join("_", underscoreParts.Take(underscoreParts.Length - 1));
                    detailLineNumberHint = ln;
                }

                if (fieldMappings.TryGetValue(baseFieldNameForMapping, out var mappingInfo))
                {
                    var ocrFieldDefinition = FindOcrFieldDefinitionInTemplate(ocrTemplate, mappingInfo.LineId, mappingInfo.FieldId);

                    // Ensure Lines (OCR_Lines) navigation property is loaded or accessible.
                    // If ocrFieldDefinition.Lines is null, it means the field isn't properly linked to a line definition in the template object.
                    if (ocrFieldDefinition != null && ocrFieldDefinition.Lines != null)
                    {
                        var partContextData = GetPartContextFromTemplate(ocrTemplate, mappingInfo.PartId);
                        var lineDefinition = ocrFieldDefinition.Lines; // This is OCR_Lines navigation property from Fields

                        var metadata = new OCRFieldMetadata
                        {
                            FieldName = runtimeFieldName,
                            Value = runtimeFieldValue,
                            RawValue = runtimeFieldValue,

                            LineNumber = detailLineNumberHint ?? 0, // This is a hint from field name, actual file line # might differ or be unavailable here.
                            FieldId = ocrFieldDefinition.Id,
                            LineId = lineDefinition.Id,
                            RegexId = lineDefinition.RegExId,
                            Key = ocrFieldDefinition.Key,
                            Field = ocrFieldDefinition.Field,
                            EntityType = ocrFieldDefinition.EntityType,
                            DataType = ocrFieldDefinition.DataType,
                            LineName = lineDefinition.Name,
                            LineRegex = lineDefinition.RegularExpressions?.RegEx,
                            LineText = null, // Actual document line text is not typically part of the static template metadata.
                            IsRequired = ocrFieldDefinition.IsRequired,
                            PartId = partContextData?.PartId,
                            PartName = partContextData?.PartName,
                            PartTypeId = partContextData?.PartTypeId,
                            InvoiceId = invoiceContextData?.InvoiceId, // From OCR.Business.Entities.OcrInvoices
                            InvoiceName = invoiceContextData?.InvoiceName,
                        };
                        metadata.FormatRegexes = GetFieldFormatRegexesFromDb(ocrFieldDefinition.Id);
                        metadataOutput[runtimeFieldName] = metadata;
                    }
                    else
                    {
                        _logger.Warning("Could not find valid OCR Field Definition (or its associated Line) for mapping: RuntimeField '{RuntimeField}', MappedBase '{BaseField}', TemplateLineId {LineId}, TemplateFieldId {FieldId}",
                            runtimeFieldName, baseFieldNameForMapping, mappingInfo.LineId, mappingInfo.FieldId);
                    }
                }
                else
                {
                    _logger.Debug("No precomputed mapping found for runtime field '{RuntimeField}' (mapped base '{BaseField}'). This field will not have template-derived metadata.", runtimeFieldName, baseFieldNameForMapping);
                }
            }
            _logger.Information("Metadata extraction complete. Extracted {Count} OCRFieldMetadata entries for Template (Log ID: {TemplateLogId}).", metadataOutput.Count, templateLogId);
            return metadataOutput;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Template field definition locator with navigation property traversal
        /// 
        /// **LOG_THE_WHAT**: OCR field definition resolver navigating template entity relationships to find specific field
        /// **LOG_THE_HOW**: Traverses Lines→OCR_Lines→Fields navigation properties using LineId and FieldId selectors
        /// **LOG_THE_WHY**: Provides template field definition lookup for metadata extraction and field context resolution
        /// **LOG_THE_WHO**: Returns OCR.Business.Entities.Fields or null if navigation path fails or field not found
        /// **LOG_THE_WHAT_IF**: Expects loaded navigation properties; handles null references gracefully in chain
        /// </summary>
        private OCR.Business.Entities.Fields FindOcrFieldDefinitionInTemplate(Template ocrTemplate, int lineId, int fieldId)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete template field definition lookup narrative
            _logger.Verbose("🔍 **FIELD_DEFINITION_LOOKUP**: Locating OCR field definition in template structure");
            _logger.Verbose("   - **LOOKUP_PARAMETERS**: LineId={LineId}, FieldId={FieldId}, TemplateId={TemplateId}", 
                lineId, fieldId, ocrTemplate?.OcrTemplates?.Id ?? 0);
            _logger.Verbose("   - **NAVIGATION_PATH**: Template→Lines→OCR_Lines→Fields entity relationship traversal");
            
            // **LOG_THE_HOW**: Navigation property traversal with null-safe chaining
            var invoiceLineWrapper = ocrTemplate.Lines?.FirstOrDefault(lWrapper => lWrapper.OCR_Lines?.Id == lineId);
            
            if (invoiceLineWrapper?.OCR_Lines == null)
            {
                _logger.Verbose("⚠️ **LINE_NOT_FOUND**: LineId={LineId} not found in template Lines collection", lineId);
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: FindOcrFieldDefinitionInTemplate dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType1 = "Invoice"; // Field definition lookup is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType1} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec1 = TemplateSpecification.CreateForUtilityOperation(documentType1, "FindOcrFieldDefinitionInTemplate", 
                    new { lineId, fieldId, templateId = ocrTemplate?.OcrTemplates?.Id }, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec1 = templateSpec1
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field definition entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec1.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess1 = validatedSpec1.IsValid;
                
                return null;
            }
            
            var fieldDefinition = invoiceLineWrapper.OCR_Lines.Fields?.FirstOrDefault(f => f.Id == fieldId);
            
            if (fieldDefinition == null)
            {
                _logger.Verbose("⚠️ **FIELD_NOT_FOUND**: FieldId={FieldId} not found in LineId={LineId} Fields collection", fieldId, lineId);
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: FindOcrFieldDefinitionInTemplate dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType2 = "Invoice"; // Field definition lookup is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType2} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec2 = TemplateSpecification.CreateForUtilityOperation(documentType2, "FindOcrFieldDefinitionInTemplate", 
                    new { lineId, fieldId, templateId = ocrTemplate?.OcrTemplates?.Id }, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec2 = templateSpec2
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field definition entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec2.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess2 = validatedSpec2.IsValid;
            }
            else
            {
                _logger.Verbose("✅ **FIELD_DEFINITION_FOUND**: Located field '{FieldName}' with Key='{FieldKey}'", 
                    fieldDefinition.Field, fieldDefinition.Key);
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: FindOcrFieldDefinitionInTemplate dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType3 = "Invoice"; // Field definition lookup is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType3} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec3 = TemplateSpecification.CreateForUtilityOperation(documentType3, "FindOcrFieldDefinitionInTemplate", 
                    new { lineId, fieldId, templateId = ocrTemplate?.OcrTemplates?.Id }, fieldDefinition);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec3 = templateSpec3
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field definition entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec3.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess3 = validatedSpec3.IsValid;
            }
            
            return fieldDefinition;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Invoice context extractor with template identification
        /// 
        /// **LOG_THE_WHAT**: Invoice context data extraction providing template identification and naming information
        /// **LOG_THE_HOW**: Accesses OcrTemplates navigation property, handles null references with fallback logic
        /// **LOG_THE_WHY**: Provides invoice-level context for metadata objects linking fields to their parent template
        /// **LOG_THE_WHO**: Returns InvoiceContext with InvoiceId and InvoiceName or empty context for null input
        /// **LOG_THE_WHAT_IF**: Expects template with OcrTemplates navigation; creates fallback context when missing
        /// </summary>
        private InvoiceContext GetInvoiceContext(Template template)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete invoice context extraction narrative
            _logger.Verbose("🔍 **INVOICE_CONTEXT_EXTRACTION**: Extracting invoice-level context from template");
            
            if (template == null)
            {
                _logger.Verbose("⚠️ **NULL_TEMPLATE**: Template is null - returning empty InvoiceContext");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetInvoiceContext dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType1 = "Invoice"; // Invoice context extraction is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType1} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec1 = TemplateSpecification.CreateForUtilityOperation(documentType1, "GetInvoiceContext", 
                    template, new InvoiceContext());

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec1 = templateSpec1
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Invoice context entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec1.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess1 = validatedSpec1.IsValid;
                
                return new InvoiceContext();
            }
            
            // **LOG_THE_HOW**: Navigation property access with fallback handling
            if (template.OcrTemplates == null)
            {
                _logger.Debug("⚠️ **MISSING_OCR_TEMPLATES**: Template OcrTemplates navigation property is null");
                _logger.Debug("   - **FALLBACK_STRATEGY**: Using Template.OcrTemplates.Id as fallback for InvoiceContext");
                _logger.Debug("   - **CONTEXT_LIMITATION**: Limited context available due to missing navigation property");
                
                var fallbackContext = new InvoiceContext 
                { 
                    InvoiceId = template.OcrTemplates.Id, 
                    InvoiceName = template.OcrTemplates.Name ?? "Unknown OCR Template Name" 
                };
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetInvoiceContext dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType2 = "Invoice"; // Invoice context extraction is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType2} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec2 = TemplateSpecification.CreateForUtilityOperation(documentType2, "GetInvoiceContext", 
                    template, fallbackContext);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec2 = templateSpec2
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Invoice context entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec2.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess2 = validatedSpec2.IsValid;
                
                return fallbackContext;
            }
            
            // **LOG_THE_WHO**: Complete context extraction with full template details
            var context = new InvoiceContext
            {
                InvoiceId = template.OcrTemplates.Id,
                InvoiceName = template.OcrTemplates.Name
            };
            
            _logger.Verbose("✅ **INVOICE_CONTEXT_COMPLETE**: InvoiceId={InvoiceId}, Name='{InvoiceName}'", 
                context.InvoiceId, context.InvoiceName);
            _logger.Verbose("   - **SUCCESS_ASSERTION**: Complete invoice context extracted for metadata correlation");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetInvoiceContext dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType3 = "Invoice"; // Invoice context extraction is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType3} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec3 = TemplateSpecification.CreateForUtilityOperation(documentType3, "GetInvoiceContext", 
                template, context);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec3 = templateSpec3
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Invoice context entity operations
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec3.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess3 = validatedSpec3.IsValid;
            
            return context;
        }

        /// <summary>
        /// Gets PartContext from the OCR Template based on a PartId.
        /// </summary>
        private PartContext GetPartContextFromTemplate(Template template, int? partId)
        {
            if (!partId.HasValue || template?.Parts == null) return new PartContext { PartId = partId }; // Return with PartId if known, even if not found

            // template.Parts is a collection of InvoicePart (wrapper), each has an OCR_Part property.
            var invoicePartWrapper = template.Parts.FirstOrDefault(pWrapper => pWrapper.OCR_Part?.Id == partId.Value);
            var ocrPart = invoicePartWrapper?.OCR_Part; // ocrPart is OCR.Business.Entities.Parts

            if (ocrPart != null)
            {
                return new PartContext
                {
                    PartId = ocrPart.Id,
                    PartName = ocrPart.PartTypes?.Name, // PartTypes is navigation to OCR_PartTypes
                    PartTypeId = ocrPart.PartTypeId
                };
            }
            _logger.Debug("PartId {PartIdVal} not found within the provided OCR Template's (Invoice.Id: {TemplateId}) Parts collection.", partId.Value, template.OcrTemplates.Id);
            return new PartContext { PartId = partId }; // Return with PartId if known
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Database field format regex retriever with pattern extraction
        /// 
        /// **LOG_THE_WHAT**: Format correction pattern retrieval from FieldFormatRegEx table for automatic data standardization
        /// **LOG_THE_HOW**: Database query with Include for navigation properties, transforms to FieldFormatRegexInfo objects
        /// **LOG_THE_WHY**: Provides format correction patterns for metadata objects enabling automatic data format standardization
        /// **LOG_THE_WHO**: Returns List<FieldFormatRegexInfo> with pattern and replacement regexes or empty list
        /// **LOG_THE_WHAT_IF**: Expects valid FieldId; handles database errors gracefully; returns empty list on failure
        /// </summary>
        private List<FieldFormatRegexInfo> GetFieldFormatRegexesFromDb(int? fieldDefinitionId)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete format regex retrieval narrative
            _logger.Verbose("📊 **FORMAT_REGEX_RETRIEVAL**: Retrieving field format correction patterns from database");
            _logger.Verbose("   - **ARCHITECTURAL_INTENT**: Load automatic format correction patterns for field data standardization");
            _logger.Verbose("   - **QUERY_TARGET**: FieldDefinitionId={FieldDefinitionId}", fieldDefinitionId);
            
            var formatRegexesInfoList = new List<FieldFormatRegexInfo>();
            
            if (!fieldDefinitionId.HasValue)
            {
                _logger.Verbose("⚠️ **NULL_FIELD_ID**: FieldDefinitionId is null - returning empty format regex list");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetFieldFormatRegexesFromDb dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType4 = "Invoice"; // Format regex retrieval is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType4} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec4 = TemplateSpecification.CreateForUtilityOperation(documentType4, "GetFieldFormatRegexesFromDb", 
                    fieldDefinitionId, formatRegexesInfoList);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec4 = templateSpec4
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Format regex pattern operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec4.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess4 = validatedSpec4.IsValid;
                
                return formatRegexesInfoList;
            }

            try
            {
                // **LOG_THE_HOW**: Database context creation and query execution
                _logger.Verbose("💾 **DATABASE_QUERY_START**: Executing FieldFormatRegEx query with navigation includes");
                
                using (var ctx = new OCRContext())
                {
                    var dbFieldFormats = ctx.OCR_FieldFormatRegEx
                        .Where(ffr => ffr.FieldId == fieldDefinitionId.Value)
                        .Include(ffr => ffr.RegEx)
                        .Include(ffr => ffr.ReplacementRegEx)
                        .ToList();

                    _logger.Verbose("📊 **QUERY_RESULT**: Found {FormatCount} format correction rules for field", dbFieldFormats.Count);

                    // **LOG_THE_WHO**: Format regex transformation and object creation
                    foreach (var dbffr in dbFieldFormats)
                    {
                        var formatInfo = new FieldFormatRegexInfo
                        {
                            FormatRegexId = dbffr.Id,
                            RegexId = dbffr.RegExId,
                            ReplacementRegexId = dbffr.ReplacementRegExId,
                            Pattern = dbffr.RegEx?.RegEx,
                            Replacement = dbffr.ReplacementRegEx?.RegEx
                        };
                        
                        formatRegexesInfoList.Add(formatInfo);
                        
                        _logger.Verbose("🔧 **FORMAT_RULE_LOADED**: Pattern='{Pattern}' → Replacement='{Replacement}'", 
                            formatInfo.Pattern, formatInfo.Replacement);
                    }
                }
                
                // **LOG_THE_WHAT_IF**: Successful retrieval completion
                _logger.Verbose("✅ **FORMAT_REGEX_RETRIEVAL_COMPLETE**: {RuleCount} format correction rules loaded successfully", 
                    formatRegexesInfoList.Count);
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetFieldFormatRegexesFromDb dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType5 = "Invoice"; // Format regex retrieval is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType5} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec5 = TemplateSpecification.CreateForUtilityOperation(documentType5, "GetFieldFormatRegexesFromDb", 
                    fieldDefinitionId, formatRegexesInfoList);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec5 = templateSpec5
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Format regex pattern operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec5.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess5 = validatedSpec5.IsValid;
            }
            catch (Exception ex)
            {
                // **LOG_THE_WHAT_IF**: Database error handling with comprehensive diagnostics
                _logger.Error(ex, "❌ **FORMAT_REGEX_RETRIEVAL_ERROR**: Database error retrieving format correction patterns");
                _logger.Error("   - **ERROR_CONTEXT**: FieldDefinitionId={FieldDefId}, ExceptionType={ExceptionType}", 
                    fieldDefinitionId.Value, ex.GetType().Name);
                _logger.Error("   - **IMPACT_ASSESSMENT**: Field will not have format correction patterns available");
                _logger.Error("   - **FALLBACK_BEHAVIOR**: Returning empty format regex list for graceful degradation");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetFieldFormatRegexesFromDb dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType6 = "Invoice"; // Format regex retrieval is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType6} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec6 = TemplateSpecification.CreateForUtilityOperation(documentType6, "GetFieldFormatRegexesFromDb", 
                    fieldDefinitionId, formatRegexesInfoList);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec6 = templateSpec6
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Format regex pattern operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec6.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess6 = validatedSpec6.IsValid;
            }
            
            return formatRegexesInfoList;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Template field mapping generator with comprehensive entity correlation
        /// 
        /// **LOG_THE_WHAT**: Template field mapping dictionary creation correlating canonical field names to database entity IDs
        /// **LOG_THE_HOW**: Traverses Parts→Lines→Fields hierarchy, creates bidirectional mappings from Field and Key properties
        /// **LOG_THE_WHY**: Enables rapid runtime field lookup for metadata extraction and template correlation workflows
        /// **LOG_THE_WHO**: Returns Dictionary mapping field names to (LineId, FieldId, PartId) tuples for direct entity access
        /// **LOG_THE_WHAT_IF**: Expects template with loaded navigation properties; handles null references with empty mapping
        /// </summary>
        public Dictionary<string, (int LineId, int FieldId, int? PartId)> CreateEnhancedFieldMapping(Template ocrTemplate)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete field mapping generation narrative
            _logger.Information("🗺️ **FIELD_MAPPING_GENERATOR_START**: Creating comprehensive template field mapping dictionary");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Build rapid lookup table correlating field names to database entity coordinates");
            _logger.Information("   - **MAPPING_STRATEGY**: Traverse template hierarchy to extract all field definitions with entity relationships");
            _logger.Information("   - **TEMPLATE_CONTEXT**: TemplateId={TemplateId}, Name={TemplateName}", 
                ocrTemplate?.OcrTemplates?.Id ?? 0, ocrTemplate?.OcrTemplates?.Name ?? "Unknown");
            
            var mappings = new Dictionary<string, (int LineId, int FieldId, int? PartId)>(StringComparer.OrdinalIgnoreCase);
            
            if (ocrTemplate?.Parts == null)
            {
                // **LOG_THE_WHAT_IF**: Invalid template handling with comprehensive error context
                _logger.Warning("⚠️ **INVALID_TEMPLATE_STATE**: Template or Parts collection is null - cannot generate field mappings");
                _logger.Warning("   - **TEMPLATE_STATE**: Template={TemplateState}, Parts={PartsState}", 
                    ocrTemplate == null ? "NULL" : "PROVIDED", ocrTemplate?.Parts == null ? "NULL" : "PROVIDED");
                _logger.Warning("   - **IMPACT_ASSESSMENT**: Field mapping will be empty, metadata extraction will fail for all fields");
                _logger.Warning("   - **RECOMMENDED_ACTION**: Verify template loading includes Parts navigation properties");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CreateEnhancedFieldMapping dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType7 = "Invoice"; // Field mapping generation is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType7} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec7 = TemplateSpecification.CreateForUtilityOperation(documentType7, "CreateEnhancedFieldMapping", 
                    ocrTemplate, mappings);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec7 = templateSpec7
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field mapping entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec7.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess7 = validatedSpec7.IsValid;
                
                return mappings;
            }

            try
            {
                // **LOG_THE_HOW**: Template hierarchy traversal with comprehensive entity processing
                _logger.Information("🔄 **HIERARCHY_TRAVERSAL_START**: Processing template Parts→Lines→Fields structure");
                var processedParts = 0;
                var processedLines = 0;
                var processedFields = 0;
                var createdMappings = 0;
                
                foreach (var partWrapper in ocrTemplate.Parts.Where(pWrap => pWrap?.OCR_Part != null))
                {
                    processedParts++;
                    var ocrPart = partWrapper.OCR_Part;
                    
                    _logger.Verbose("🔧 **PART_PROCESSING**: Processing Part {PartNumber} - PartId={PartId}, PartTypeId={PartTypeId}", 
                        processedParts, ocrPart.Id, ocrPart.PartTypeId);
                    
                    if (ocrPart.Lines == null)
                    {
                        _logger.Verbose("⚠️ **PART_NO_LINES**: Part {PartId} has no Lines collection - skipping", ocrPart.Id);
                        continue;
                    }

                    foreach (var lineDef in ocrPart.Lines.Where(lDef => lDef?.Fields != null))
                    {
                        processedLines++;
                        _logger.Verbose("🔧 **LINE_PROCESSING**: Processing Line {LineNumber} - LineId={LineId}, Name='{LineName}'", 
                            processedLines, lineDef.Id, lineDef.Name);
                        
                        foreach (var fieldDef in lineDef.Fields)
                        {
                            processedFields++;
                            _logger.Verbose("🔍 **FIELD_PROCESSING**: Field {FieldNumber} - FieldId={FieldId}, Field='{Field}', Key='{Key}'", 
                                processedFields, fieldDef.Id, fieldDef.Field, fieldDef.Key);
                            
                            // **LOG_THE_WHO**: Canonical name mapping from Field property
                            string canonicalNameFromField = MapTemplateFieldToPropertyName(fieldDef.Field);
                            if (!string.IsNullOrEmpty(canonicalNameFromField) && !mappings.ContainsKey(canonicalNameFromField))
                            {
                                mappings[canonicalNameFromField] = (lineDef.Id, fieldDef.Id, ocrPart.Id);
                                createdMappings++;
                                _logger.Verbose("✅ **MAPPING_CREATED**: '{CanonicalName}' → LineId={LineId}, FieldId={FieldId}, PartId={PartId}", 
                                    canonicalNameFromField, lineDef.Id, fieldDef.Id, ocrPart.Id);
                            }

                            // **LOG_THE_WHAT_IF**: Additional mapping from Key property if different
                            string canonicalNameFromKey = MapTemplateFieldToPropertyName(fieldDef.Key);
                            if (!string.IsNullOrEmpty(canonicalNameFromKey) &&
                               !mappings.ContainsKey(canonicalNameFromKey) &&
                               (string.IsNullOrEmpty(canonicalNameFromField) || !canonicalNameFromKey.Equals(canonicalNameFromField, StringComparison.OrdinalIgnoreCase)))
                            {
                                mappings[canonicalNameFromKey] = (lineDef.Id, fieldDef.Id, ocrPart.Id);
                                createdMappings++;
                                _logger.Verbose("✅ **KEY_MAPPING_CREATED**: '{CanonicalKey}' → LineId={LineId}, FieldId={FieldId}, PartId={PartId}", 
                                    canonicalNameFromKey, lineDef.Id, fieldDef.Id, ocrPart.Id);
                            }
                        }
                    }
                }
                
                // **LOG_THE_WHO**: Mapping generation completion with comprehensive metrics
                var templateLogId = ocrTemplate.OcrTemplates?.Id.ToString() ?? ocrTemplate.OcrTemplates.Id.ToString();
                _logger.Information("✅ **FIELD_MAPPING_COMPLETE**: Enhanced field mapping dictionary generated successfully");
                _logger.Information("   - **PROCESSING_METRICS**: Parts={Parts}, Lines={Lines}, Fields={Fields}, Mappings={Mappings}", 
                    processedParts, processedLines, processedFields, createdMappings);
                _logger.Information("   - **TEMPLATE_CONTEXT**: TemplateId={TemplateLogId}, TotalMappings={MappingCount}", 
                    templateLogId, mappings.Count);
                _logger.Information("   - **SUCCESS_ASSERTION**: Field mapping dictionary ready for rapid runtime field correlation");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CreateEnhancedFieldMapping dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType8 = "Invoice"; // Field mapping generation is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType8} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec8 = TemplateSpecification.CreateForUtilityOperation(documentType8, "CreateEnhancedFieldMapping", 
                    ocrTemplate, mappings);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec8 = templateSpec8
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field mapping entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec8.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess8 = validatedSpec8.IsValid;
            }
            catch (Exception ex)
            {
                // **LOG_THE_WHAT_IF**: Exception handling with comprehensive error diagnostics
                var templateLogId = ocrTemplate.OcrTemplates?.Id.ToString() ?? ocrTemplate.OcrTemplates.Id.ToString();
                _logger.Error(ex, "❌ **FIELD_MAPPING_ERROR**: Exception occurred during field mapping generation");
                _logger.Error("   - **ERROR_CONTEXT**: TemplateId={TemplateLogId}, ExceptionType={ExceptionType}", 
                    templateLogId, ex.GetType().Name);
                _logger.Error("   - **PARTIAL_STATE**: {MappingCount} mappings created before failure", mappings.Count);
                _logger.Error("   - **IMPACT_ASSESSMENT**: Incomplete field mapping may cause metadata extraction failures");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: CreateEnhancedFieldMapping dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType9 = "Invoice"; // Field mapping generation is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType9} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec9 = TemplateSpecification.CreateForUtilityOperation(documentType9, "CreateEnhancedFieldMapping", 
                    ocrTemplate, mappings);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec9 = templateSpec9
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field mapping entity operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec9.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess9 = validatedSpec9.IsValid;
            }
            
            return mappings;
        }
        #endregion
    }
}