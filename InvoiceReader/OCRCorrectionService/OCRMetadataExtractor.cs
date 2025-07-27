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
            var templateLogId = ocrTemplate.OcrInvoices?.Id.ToString() ?? ocrTemplate.OcrInvoices.Id.ToString();
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
        /// Helper to find an OCR.Business.Entities.Fields definition within a loaded OCR Template.
        /// Assumes the ocrTemplate object has its navigation properties (Lines, OCR_Lines.Fields) loaded if not using lazy loading.
        /// </summary>
        private OCR.Business.Entities.Fields FindOcrFieldDefinitionInTemplate(Template ocrTemplate, int lineId, int fieldId)
        {
            // ocrTemplate.Lines is a collection of InvoiceLine (wrapper), each has an OCR_Lines property.
            var invoiceLineWrapper = ocrTemplate.Lines?.FirstOrDefault(lWrapper => lWrapper.OCR_Lines?.Id == lineId);
            return invoiceLineWrapper?.OCR_Lines?.Fields?.FirstOrDefault(f => f.Id == fieldId);
        }

        /// <summary>
        /// Gets InvoiceContext (ID and Name of the OcrInvoice template definition).
        /// </summary>
        private InvoiceContext GetInvoiceContext(Template template)
        {
            if (template == null) return new InvoiceContext();
            // template.OcrInvoices is the navigation property to the OCR_Invoices table entry from the Invoice entity.
            if (template.OcrInvoices == null)
            {
                _logger.Debug("OCR Template (Invoice.Id: {TemplateId}) does not have OcrInvoices details linked directly. Using Invoice.Id as fallback for context.", template.OcrInvoices.Id);
                return new InvoiceContext { InvoiceId = template.OcrInvoices.Id, InvoiceName = template.OcrInvoices.Name ?? "Unknown OCR Template Name" };
            }
            return new InvoiceContext
            {
                InvoiceId = template.OcrInvoices.Id,
                InvoiceName = template.OcrInvoices.Name
            };
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
            _logger.Debug("PartId {PartIdVal} not found within the provided OCR Template's (Invoice.Id: {TemplateId}) Parts collection.", partId.Value, template.OcrInvoices.Id);
            return new PartContext { PartId = partId }; // Return with PartId if known
        }

        /// <summary>
        /// Gets FieldFormatRegexInfo for a given database FieldId (OCR.Business.Entities.Fields.Id) by querying the database.
        /// </summary>
        private List<FieldFormatRegexInfo> GetFieldFormatRegexesFromDb(int? fieldDefinitionId)
        {
            var formatRegexesInfoList = new List<FieldFormatRegexInfo>();
            if (!fieldDefinitionId.HasValue) return formatRegexesInfoList;

            try
            {
                using (var ctx = new OCRContext())
                {
                    // Ensure navigation property names match your EF model for FieldFormatRegEx -> RegularExpressions
                    var dbFieldFormats = ctx.OCR_FieldFormatRegEx
                        .Where(ffr => ffr.FieldId == fieldDefinitionId.Value)
                        .Include(ffr => ffr.RegEx)     // Assumes navigation property name for pattern regex is "RegEx"
                        .Include(ffr => ffr.ReplacementRegEx)    // Assumes navigation property name for replacement regex is "ReplacementRegEx"
                        .ToList();

                    foreach (var dbffr in dbFieldFormats)
                    {
                        formatRegexesInfoList.Add(new FieldFormatRegexInfo
                        {
                            FormatRegexId = dbffr.Id,
                            RegexId = dbffr.RegExId,
                            ReplacementRegexId = dbffr.ReplacementRegExId,
                            Pattern = dbffr.RegEx?.RegEx,
                            Replacement = dbffr.ReplacementRegEx?.RegEx
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error retrieving field format regexes from DB for FieldDefinitionId {FieldDefId}", fieldDefinitionId.Value);
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
                _logger.Warning("CreateEnhancedFieldMapping: OCR Template (ID: {TemplateId}) or its Parts collection is null.", ocrTemplate?.OcrInvoices.Id);
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
                var templateLogId = ocrTemplate.OcrInvoices?.Id.ToString() ?? ocrTemplate.OcrInvoices.Id.ToString();
                _logger.Debug("Created {MappingCount} enhanced field mappings from OCR Template (Log ID: {TemplateLogId}).", mappings.Count, templateLogId);
            }
            catch (Exception ex)
            {
                var templateLogId = ocrTemplate.OcrInvoices?.Id.ToString() ?? ocrTemplate.OcrInvoices.Id.ToString();
                _logger?.Error(ex, "Error creating enhanced field mappings from OCR Template (Log ID: {TemplateLogId}).", templateLogId);
            }
            return mappings;
        }
        #endregion
    }
}