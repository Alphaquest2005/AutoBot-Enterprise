using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using WaterNut.DataSpace;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Enhanced OCR metadata extractor that captures complete template context for precise database updates
    /// </summary>
    public partial class OCRCorrectionService
    {
        /// <summary>
        /// Extracts comprehensive OCR metadata from template processing results
        /// </summary>
        public Dictionary<string, OCRFieldMetadata> ExtractEnhancedOCRMetadata(
            IDictionary<string, object> invoiceDict,
            Invoice template,
            Dictionary<string, (int LineId, int FieldId)> fieldMappings)
        {
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            try
            {
                _logger?.Debug("Starting enhanced OCR metadata extraction for {FieldCount} fields", invoiceDict.Count);

                // Get invoice context
                var invoiceContext = GetInvoiceContext(template);

                // Extract metadata for each field in the invoice
                foreach (var kvp in invoiceDict)
                {
                    var fieldName = kvp.Key;
                    var fieldValue = kvp.Value?.ToString();

                    // Skip metadata fields
                    if (IsMetadataFieldInternal(fieldName)) continue;

                    // Try to find field mapping
                    if (fieldMappings.TryGetValue(fieldName, out var mapping))
                    {
                        var fieldMetadata = ExtractFieldMetadata(fieldName, fieldValue, template, invoiceContext, mapping.LineId, mapping.FieldId);
                        if (fieldMetadata != null)
                        {
                            metadata[fieldName] = fieldMetadata;
                        }
                    }
                    else
                    {
                        // Try to find field directly in template if no mapping exists
                        var fieldMetadata = ExtractFieldMetadataFromTemplate(fieldName, fieldValue, template, invoiceContext);
                        if (fieldMetadata != null)
                        {
                            metadata[fieldName] = fieldMetadata;
                        }
                    }
                }

                _logger?.Debug("Extracted enhanced OCR metadata for {FieldCount} fields", metadata.Count);
                return metadata;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting enhanced OCR metadata");
                return new Dictionary<string, OCRFieldMetadata>();
            }
        }

        /// <summary>
        /// Extracts metadata for a specific field from template processing using LineId and FieldId
        /// </summary>
        private OCRFieldMetadata ExtractFieldMetadata(
            string fieldName,
            string fieldValue,
            Invoice template,
            InvoiceContext invoiceContext,
            int lineId,
            int fieldId)
        {
            try
            {
                // Find field context using specific LineId and FieldId
                var fieldContext = FindFieldInTemplateByIds(lineId, fieldId, template);
                if (fieldContext == null)
                {
                    _logger?.Warning("Could not find field {FieldName} with LineId {LineId} and FieldId {FieldId} in template", fieldName, lineId, fieldId);
                    return null;
                }

                // Create comprehensive metadata
                var metadata = new OCRFieldMetadata
                {
                    // Basic field information
                    FieldName = fieldName,
                    Value = fieldValue,
                    RawValue = fieldContext.RawValue,

                    // OCR Template Context
                    LineNumber = fieldContext.LineNumber,
                    FieldId = fieldContext.FieldId,
                    LineId = fieldContext.LineId,
                    RegexId = fieldContext.RegexId,
                    Key = fieldContext.Key,
                    Field = fieldContext.Field,
                    EntityType = fieldContext.EntityType,
                    DataType = fieldContext.DataType,

                    // Line Context
                    LineName = fieldContext.LineName,
                    LineRegex = fieldContext.LineRegex,
                    IsRequired = fieldContext.IsRequired,

                    // Part Context
                    PartId = fieldContext.PartId,
                    PartName = fieldContext.PartName,
                    PartTypeId = fieldContext.PartTypeId,

                    // Invoice Context
                    InvoiceId = invoiceContext.InvoiceId,
                    InvoiceName = invoiceContext.InvoiceName,

                    // Processing Context
                    Section = fieldContext.Section,
                    Instance = fieldContext.Instance,
                    Confidence = fieldContext.Confidence
                };

                // Add field format regex information
                metadata.FormatRegexes = GetFieldFormatRegexes(fieldContext.FieldId);

                return metadata;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting metadata for field {FieldName}", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Extracts metadata for a field by searching through template (fallback method)
        /// </summary>
        private OCRFieldMetadata ExtractFieldMetadataFromTemplate(
            string fieldName,
            string fieldValue,
            Invoice template,
            InvoiceContext invoiceContext)
        {
            try
            {
                // Try to find field in template processing results
                var fieldContext = FindFieldInTemplate(fieldName, template);
                if (fieldContext == null)
                {
                    _logger?.Warning("Could not find field {FieldName} in template processing results", fieldName);
                    return null;
                }

                // Create comprehensive metadata (same as above)
                var metadata = new OCRFieldMetadata
                {
                    // Basic field information
                    FieldName = fieldName,
                    Value = fieldValue,
                    RawValue = fieldContext.RawValue,

                    // OCR Template Context
                    LineNumber = fieldContext.LineNumber,
                    FieldId = fieldContext.FieldId,
                    LineId = fieldContext.LineId,
                    RegexId = fieldContext.RegexId,
                    Key = fieldContext.Key,
                    Field = fieldContext.Field,
                    EntityType = fieldContext.EntityType,
                    DataType = fieldContext.DataType,

                    // Line Context
                    LineName = fieldContext.LineName,
                    LineRegex = fieldContext.LineRegex,
                    IsRequired = fieldContext.IsRequired,

                    // Part Context
                    PartId = fieldContext.PartId,
                    PartName = fieldContext.PartName,
                    PartTypeId = fieldContext.PartTypeId,

                    // Invoice Context
                    InvoiceId = invoiceContext.InvoiceId,
                    InvoiceName = invoiceContext.InvoiceName,

                    // Processing Context
                    Section = fieldContext.Section,
                    Instance = fieldContext.Instance,
                    Confidence = fieldContext.Confidence
                };

                // Add field format regex information
                metadata.FormatRegexes = GetFieldFormatRegexes(fieldContext.FieldId);

                return metadata;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting metadata for field {FieldName}", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Finds field context using specific LineId and FieldId
        /// </summary>
        private FieldContext FindFieldInTemplateByIds(int lineId, int fieldId, Invoice template)
        {
            try
            {
                // Find the specific line
                var line = template.Lines?.FirstOrDefault(l => l.OCR_Lines?.Id == lineId);
                if (line?.Values == null) return null;

                // Search through line values for the specific field
                foreach (var outerKvp in line.Values)
                {
                    var (lineNumber, section) = outerKvp.Key;
                    var innerDict = outerKvp.Value;

                    if (innerDict != null)
                    {
                        foreach (var innerKvp in innerDict)
                        {
                            var (fields, instance) = innerKvp.Key;
                            var rawValue = innerKvp.Value;

                            // Check if this is the specific field we're looking for
                            if (fields?.Id == fieldId)
                            {
                                var partContext = GetPartContextFromLine(line);
                                var lineContext = GetLineContext(line, partContext);

                                return new FieldContext
                                {
                                    // Field information
                                    FieldId = fields.Id,
                                    Field = fields.Field,
                                    Key = fields.Key,
                                    EntityType = fields.EntityType,
                                    DataType = fields.DataType,
                                    IsRequired = fields.IsRequired,
                                    RawValue = rawValue,

                                    // Line context
                                    LineNumber = lineNumber,
                                    LineId = lineContext.LineId,
                                    LineName = lineContext.LineName,
                                    LineRegex = lineContext.LineRegex,
                                    RegexId = lineContext.RegexId,

                                    // Part context
                                    PartId = lineContext.PartId,
                                    PartName = lineContext.PartName,
                                    PartTypeId = lineContext.PartTypeId,

                                    // Processing context
                                    Section = section,
                                    Instance = instance,
                                    Confidence = 1.0 // Default confidence for successful extraction
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error finding field with LineId {LineId} and FieldId {FieldId} in template", lineId, fieldId);
                return null;
            }
        }

        /// <summary>
        /// Finds field context in template processing results by field name
        /// </summary>
        private FieldContext FindFieldInTemplate(string fieldName, Invoice template)
        {
            try
            {
                // Search through all parts, lines, and field values to find the field
                foreach (var part in template.Parts ?? Enumerable.Empty<Part>())
                {
                    var partContext = GetPartContext(part);

                    foreach (var line in part.Lines ?? Enumerable.Empty<Line>())
                    {
                        var lineContext = GetLineContext(line, partContext);

                        // Search through line values for the field
                        var fieldContext = FindFieldInLineValues(fieldName, line, lineContext);
                        if (fieldContext != null)
                        {
                            return fieldContext;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error finding field {FieldName} in template", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Searches for field in line values
        /// </summary>
        private FieldContext FindFieldInLineValues(string fieldName, Line line, LineContext lineContext)
        {
            try
            {
                if (line.Values == null) return null;

                foreach (var outerKvp in line.Values)
                {
                    var (lineNumber, section) = outerKvp.Key;
                    var innerDict = outerKvp.Value;

                    if (innerDict != null)
                    {
                        foreach (var innerKvp in innerDict)
                        {
                            var (fields, instance) = innerKvp.Key;
                            var rawValue = innerKvp.Value;

                            // Check if this field matches what we're looking for
                            if (fields?.Field == fieldName || fields?.Key == fieldName)
                            {
                                return new FieldContext
                                {
                                    // Field information
                                    FieldId = fields.Id,
                                    Field = fields.Field,
                                    Key = fields.Key,
                                    EntityType = fields.EntityType,
                                    DataType = fields.DataType,
                                    IsRequired = fields.IsRequired,
                                    RawValue = rawValue,

                                    // Line context
                                    LineNumber = lineNumber,
                                    LineId = lineContext.LineId,
                                    LineName = lineContext.LineName,
                                    LineRegex = lineContext.LineRegex,
                                    RegexId = lineContext.RegexId,

                                    // Part context
                                    PartId = lineContext.PartId,
                                    PartName = lineContext.PartName,
                                    PartTypeId = lineContext.PartTypeId,

                                    // Processing context
                                    Section = section,
                                    Instance = instance,
                                    Confidence = 1.0 // Default confidence for successful extraction
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error searching field {FieldName} in line values", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Gets invoice context information
        /// </summary>
        private InvoiceContext GetInvoiceContext(Invoice template)
        {
            return new InvoiceContext
            {
                InvoiceId = template.OcrInvoices?.Id,
                InvoiceName = template.OcrInvoices?.Name
            };
        }

        /// <summary>
        /// Gets part context information
        /// </summary>
        private PartContext GetPartContext(Part part)
        {
            var ocrPart = ((WaterNut.DataSpace.Part)part)?.OCR_Part;
            return new PartContext
            {
                PartId = ocrPart?.Id,
                PartTypeId = ocrPart?.PartTypeId,
                PartName = ocrPart?.PartTypes?.Name
            };
        }

        /// <summary>
        /// Gets part context information from a line (finds the parent part)
        /// </summary>
        private PartContext GetPartContextFromLine(Line line)
        {
            try
            {
                // Find the part that contains this line
                var partId = line.OCR_Lines?.PartId;
                if (!partId.HasValue) return new PartContext();

                using (var ctx = new OCRContext())
                {
                    var ocrPart = ctx.OCR_Part
                        .Include("PartTypes")
                        .FirstOrDefault(p => p.Id == partId.Value);

                    return new PartContext
                    {
                        PartId = ocrPart?.Id,
                        PartTypeId = ocrPart?.PartTypeId,
                        PartName = ocrPart?.PartTypes?.Name
                    };
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting part context from line");
                return new PartContext();
            }
        }

        /// <summary>
        /// Gets line context information
        /// </summary>
        private LineContext GetLineContext(Line line, PartContext partContext)
        {
            return new LineContext
            {
                LineId = line.OCR_Lines?.Id,
                LineName = line.OCR_Lines?.Name,
                LineRegex = line.OCR_Lines?.RegularExpressions?.RegEx,
                RegexId = line.OCR_Lines?.RegularExpressions?.Id,
                PartId = partContext.PartId,
                PartName = partContext.PartName,
                PartTypeId = partContext.PartTypeId
            };
        }

        /// <summary>
        /// Gets field format regex information for a field
        /// </summary>
        private List<FieldFormatRegexInfo> GetFieldFormatRegexes(int? fieldId)
        {
            var formatRegexes = new List<FieldFormatRegexInfo>();

            if (!fieldId.HasValue) return formatRegexes;

            try
            {
                using (var ctx = new OCRContext())
                {
                    var fieldFormatRegexes = ctx.OCR_FieldFormatRegEx
                        .Where(ffr => ffr.FieldId == fieldId.Value)
                        .Select(ffr => new FieldFormatRegexInfo
                        {
                            FormatRegexId = ffr.Id,
                            RegexId = ffr.RegExId,
                            ReplacementRegexId = ffr.ReplacementRegExId,
                            Pattern = ffr.OCR_RegularExpressions?.RegEx,
                            Replacement = ffr.OCR_RegularExpressions1?.RegEx
                        })
                        .ToList();

                    formatRegexes.AddRange(fieldFormatRegexes);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting field format regexes for FieldId: {FieldId}", fieldId);
            }

            return formatRegexes;
        }

        /// <summary>
        /// Checks if a field name is a metadata field that should be skipped
        /// </summary>
        private bool IsMetadataFieldInternal(string fieldName)
        {
            var metadataFields = new[] { "LineNumber", "FileLineNumber", "Section", "Instance" };
            return metadataFields.Contains(fieldName);
        }
    }
}
