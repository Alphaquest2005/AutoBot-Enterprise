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
                return null;
            }
            
            var fieldDefinition = invoiceLineWrapper.OCR_Lines.Fields?.FirstOrDefault(f => f.Id == fieldId);
            
            if (fieldDefinition == null)
            {
                _logger.Verbose("⚠️ **FIELD_NOT_FOUND**: FieldId={FieldId} not found in LineId={LineId} Fields collection", fieldId, lineId);
            }
            else
            {
                _logger.Verbose("✅ **FIELD_DEFINITION_FOUND**: Located field '{FieldName}' with Key='{FieldKey}'", 
                    fieldDefinition.Field, fieldDefinition.Key);
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
                return new InvoiceContext();
            }
            
            // **LOG_THE_HOW**: Navigation property access with fallback handling
            if (template.OcrTemplates == null)
            {
                _logger.Debug("⚠️ **MISSING_OCR_TEMPLATES**: Template OcrTemplates navigation property is null");
                _logger.Debug("   - **FALLBACK_STRATEGY**: Using Template.OcrTemplates.Id as fallback for InvoiceContext");
                _logger.Debug("   - **CONTEXT_LIMITATION**: Limited context available due to missing navigation property");
                
                return new InvoiceContext 
                { 
                    InvoiceId = template.OcrTemplates.Id, 
                    InvoiceName = template.OcrTemplates.Name ?? "Unknown OCR Template Name" 
                };
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
            }
            catch (Exception ex)
            {
                // **LOG_THE_WHAT_IF**: Database error handling with comprehensive diagnostics
                _logger.Error(ex, "❌ **FORMAT_REGEX_RETRIEVAL_ERROR**: Database error retrieving format correction patterns");
                _logger.Error("   - **ERROR_CONTEXT**: FieldDefinitionId={FieldDefId}, ExceptionType={ExceptionType}", 
                    fieldDefinitionId.Value, ex.GetType().Name);
                _logger.Error("   - **IMPACT_ASSESSMENT**: Field will not have format correction patterns available");
                _logger.Error("   - **FALLBACK_BEHAVIOR**: Returning empty format regex list for graceful degradation");
            }
            
            return formatRegexesInfoList;
        }

        /// <summary>
        /// Creates a mapping dictionary from canonical property names (like "InvoiceTotal")
        /// to their (LineId, FieldId, PartId) within the given OCR Template.
        /// LineId here refers to OCR.Business.Entities.Lines.Id.
        /// FieldId here refers to OCR.Business.Entities.Fields.Id.
        /// PartId here refers to OCR.Business.Entities.Parts.Id.
        /// </summary>
        public Dictionary<string, (int LineId, int FieldId, int? PartId)> CreateEnhancedFieldMapping(Template ocrTemplate) // ocrTemplate is OCR.Business.Entities.Invoice
        {
            var mappings = new Dictionary<string, (int LineId, int FieldId, int? PartId)>(StringComparer.OrdinalIgnoreCase);
            if (ocrTemplate?.Parts == null) // ocrTemplate.Parts is a collection of InvoicePart wrappers
            {
                _logger.Warning("CreateEnhancedFieldMapping: OCR Template (ID: {TemplateId}) or its Parts collection is null.", ocrTemplate?.OcrTemplates.Id);
                return mappings;
            }

            try
            {
                foreach (var partWrapper in ocrTemplate.Parts.Where(pWrap => pWrap?.OCR_Part != null))
                {
                    var ocrPart = partWrapper.OCR_Part; // This is OCR.Business.Entities.Parts
                    if (ocrPart.Lines == null) continue; // ocrPart.Lines is Collection<OCR.Business.Entities.Lines>

                    foreach (var lineDef in ocrPart.Lines.Where(lDef => lDef?.Fields != null)) // lineDef is OCR.Business.Entities.Lines
                    {
                        foreach (var fieldDef in lineDef.Fields) // fieldDef is OCR.Business.Entities.Fields
                        {
                            // Map based on OCR.Business.Entities.Fields.Field property
                            string canonicalNameFromField = MapTemplateFieldToPropertyName(fieldDef.Field); // Static util
                            if (!string.IsNullOrEmpty(canonicalNameFromField) && !mappings.ContainsKey(canonicalNameFromField))
                            {
                                mappings[canonicalNameFromField] = (lineDef.Id, fieldDef.Id, ocrPart.Id);
                            }

                            // Also map based on OCR.Business.Entities.Fields.Key property if different and not yet mapped
                            string canonicalNameFromKey = MapTemplateFieldToPropertyName(fieldDef.Key);
                            if (!string.IsNullOrEmpty(canonicalNameFromKey) &&
                               !mappings.ContainsKey(canonicalNameFromKey) &&
                               (string.IsNullOrEmpty(canonicalNameFromField) || !canonicalNameFromKey.Equals(canonicalNameFromField, StringComparison.OrdinalIgnoreCase)))
                            {
                                mappings[canonicalNameFromKey] = (lineDef.Id, fieldDef.Id, ocrPart.Id);
                            }
                        }
                    }
                }
                var templateLogId = ocrTemplate.OcrTemplates?.Id.ToString() ?? ocrTemplate.OcrTemplates.Id.ToString();
                _logger.Debug("Created {MappingCount} enhanced field mappings from OCR Template (Log ID: {TemplateLogId}).", mappings.Count, templateLogId);
            }
            catch (Exception ex)
            {
                var templateLogId = ocrTemplate.OcrTemplates?.Id.ToString() ?? ocrTemplate.OcrTemplates.Id.ToString();
                _logger?.Error(ex, "Error creating enhanced field mappings from OCR Template (Log ID: {TemplateLogId}).", templateLogId);
            }
            return mappings;
        }
        #endregion
    }
}