using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Serilog;
using WaterNut.DataSpace;
using OCR.Business.Entities;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Enhanced OCR metadata extractor with PartId support for omission handling
    /// </summary>
    public partial class OCRCorrectionService
    {
        /// <summary>
        /// Extracts comprehensive OCR metadata from template processing results with PartId support
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

                    // Part Context - ENHANCED FOR OMISSION SUPPORT
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
        /// Searches for field in line values with enhanced PartId support
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

                                    // Part context - ENHANCED FOR OMISSION SUPPORT
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
        /// Gets part context information with database lookup for PartId
        /// </summary>
        private PartContext GetPartContext(Part part)
        {
            try
            {
                var ocrPart = ((WaterNut.DataSpace.Part)part)?.OCR_Part;
                return new PartContext
                {
                    PartId = ocrPart?.Id,
                    PartTypeId = ocrPart?.PartTypeId,
                    PartName = ocrPart?.PartTypes?.Name
                };
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting part context");
                return new PartContext();
            }
        }

        /// <summary>
        /// Gets part context information from a line (finds the parent part) - ENHANCED FOR OMISSION SUPPORT
        /// </summary>
        private PartContext GetPartContextFromLine(Line line)
        {
            try
            {
                // Find the part that contains this line
                var partId = line.OCR_Lines?.PartId;
                if (!partId.HasValue)
                {
                    _logger?.Warning("Line {LineId} has no PartId", line.OCR_Lines?.Id);
                    return new PartContext();
                }

                using (var ctx = new OCRContext())
                {
                    var ocrPart = ctx.Parts
                        .Include(p => p.PartTypes)
                        .FirstOrDefault(p => p.Id == partId.Value);

                    if (ocrPart != null)
                    {
                        _logger?.Debug("Found part context: PartId={PartId}, PartName={PartName}",
                            ocrPart.Id, ocrPart.PartTypes?.Name);

                        return new PartContext
                        {
                            PartId = ocrPart.Id,
                            PartTypeId = ocrPart.PartTypeId,
                            PartName = ocrPart.PartTypes?.Name
                        };
                    }
                    else
                    {
                        _logger?.Warning("No part found with PartId {PartId}", partId.Value);
                        return new PartContext();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting part context from line");
                return new PartContext();
            }
        }

        /// <summary>
        /// Gets line context information with enhanced PartId support
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

        /// <summary>
        /// Creates enhanced field mapping from template with PartId support
        /// </summary>
        public Dictionary<string, (int LineId, int FieldId, int? PartId)> CreateEnhancedFieldMapping(Invoice template)
        {
            var mappings = new Dictionary<string, (int LineId, int FieldId, int? PartId)>();

            try
            {
                foreach (var part in template.Parts ?? Enumerable.Empty<Part>())
                {
                    var partContext = GetPartContext(part);

                    foreach (var line in part.Lines ?? Enumerable.Empty<Line>())
                    {
                        if (line?.OCR_Lines?.Fields != null)
                        {
                            foreach (var field in line.OCR_Lines.Fields)
                            {
                                var fieldName = field.Field?.Trim();
                                if (!string.IsNullOrEmpty(fieldName))
                                {
                                    // Map common field names to ShipmentInvoice properties
                                    var mappedFieldName = MapFieldNameToProperty(fieldName);
                                    if (!string.IsNullOrEmpty(mappedFieldName) && !mappings.ContainsKey(mappedFieldName))
                                    {
                                        mappings[mappedFieldName] = (line.OCR_Lines.Id, field.Id, partContext.PartId);

                                        _logger?.Debug("Enhanced mapping: {FieldName} → LineId={LineId}, FieldId={FieldId}, PartId={PartId}",
                                            mappedFieldName, line.OCR_Lines.Id, field.Id, partContext.PartId);
                                    }
                                }
                            }
                        }
                    }
                }

                _logger?.Debug("Created {MappingCount} enhanced field mappings from template", mappings.Count);
                return mappings;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating enhanced field mappings from template");
                return new Dictionary<string, (int LineId, int FieldId, int? PartId)>();
            }
        }

        /// <summary>
        /// Maps template field names to ShipmentInvoice property names
        /// </summary>
        private string MapFieldNameToProperty(string fieldName)
        {
            return fieldName?.ToLowerInvariant() switch
            {
                "invoicetotal" or "total" or "invoice_total" => "InvoiceTotal",
                "subtotal" or "sub_total" => "SubTotal",
                "totalinternalfreight" or "freight" or "internal_freight" => "TotalInternalFreight",
                "totalothercost" or "other_cost" or "othercost" => "TotalOtherCost",
                "totalinsurance" or "insurance" => "TotalInsurance",
                "totaldeduction" or "deduction" or "discount" => "TotalDeduction",
                "invoiceno" or "invoice_no" or "invoice_number" => "InvoiceNo",
                "invoicedate" or "invoice_date" or "date" => "InvoiceDate",
                "currency" => "Currency",
                "suppliername" or "supplier_name" or "supplier" => "SupplierName",
                "supplieraddress" or "supplier_address" or "address" => "SupplierAddress",
                _ => null
            };
        }

        /// <summary>
        /// Finds line context for correction processing with enhanced PartId support
        /// </summary>
        public LineContext FindLineContext(CorrectionResult correction,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata,
            string originalText)
        {
            try
            {
                // Primary strategy: Match by exact line text
                var exactTextMatch = FindLineByExactText(correction.LineText, invoiceMetadata, originalText);
                if (exactTextMatch != null)
                {
                    _logger?.Information("Found exact line text match for correction on field {FieldName}",
                        correction.FieldName);
                    return exactTextMatch;
                }

                // Fallback 1: Fuzzy text matching
                var fuzzyMatch = FindLineBySimilarText(correction.LineText, invoiceMetadata, originalText, 0.8);
                if (fuzzyMatch != null)
                {
                    _logger?.Warning("Using fuzzy text match for field {FieldName}. Expected: '{Expected}', Found: '{Found}'",
                        correction.FieldName, correction.LineText, fuzzyMatch.LineText);
                    return fuzzyMatch;
                }

                // Fallback 2: Line number based (if text matching fails)
                var lineNumberMatch = FindLineByNumber(correction.LineNumber, invoiceMetadata, originalText);
                if (lineNumberMatch != null)
                {
                    _logger?.Warning("Using line number fallback for field {FieldName} at line {LineNumber}",
                        correction.FieldName, correction.LineNumber);
                    return lineNumberMatch;
                }

                // Helper method for line number fallback
                LineContext FindLineByNumber(int lineNumber, Dictionary<string, OCRFieldMetadata> metadata, string originalText)
                {
                    var lineMetadata = metadata.Values.FirstOrDefault(m => m.LineNumber == lineNumber);
                    return lineMetadata != null ? CreateLineContextFromMetadata(lineMetadata, originalText) : null;
                }

                // Helper method to create line context from metadata
                LineContext CreateLineContextFromMetadata(OCRFieldMetadata metadata, string originalText)
                {
                    return new LineContext
                    {
                        LineId = metadata.LineId,
                        LineNumber = metadata.LineNumber,
                        LineText = GetOriginalLineText(metadata.LineNumber, originalText),
                        RegexPattern = metadata.LineRegex,
                        LineName = metadata.LineName,
                        LineRegex = metadata.LineRegex,
                        RegexId = metadata.RegexId,
                        PartId = metadata.PartId,
                        PartName = metadata.PartName,
                        PartTypeId = metadata.PartTypeId,
                        IsOrphaned = false,
                        RequiresNewLineCreation = false
                    };
                }

                // Helper method to get original line text
                string GetOriginalLineText(int lineNumber, string originalText)
                {
                    if (string.IsNullOrEmpty(originalText) || lineNumber <= 0)
                        return "";

                    var lines = originalText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    return lineNumber <= lines.Length ? lines[lineNumber - 1] : "";
                }

                // Helper method to create orphaned line context
                LineContext CreateOrphanedLineContext(CorrectionResult correction, string originalText)
                {
                    return new LineContext
                    {
                        LineId = null,
                        LineNumber = correction.LineNumber,
                        LineText = correction.LineText ?? GetOriginalLineText(correction.LineNumber, originalText),
                        ContextLinesBefore = correction.ContextLinesBefore,
                        ContextLinesAfter = correction.ContextLinesAfter,
                        RequiresMultilineRegex = correction.RequiresMultilineRegex,
                        IsOrphaned = true,
                        RequiresNewLineCreation = true,
                        PartId = null
                    };
                }

                // Last resort: Create orphaned context
                _logger?.Warning("No matching line found for field {FieldName}, creating orphaned context",
                    correction.FieldName);
                return CreateOrphanedLineContext(correction, originalText);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error finding line context for correction on field {FieldName}", correction.FieldName);
                return CreateOrphanedLineContext(correction, originalText);
            }
        }



        #endregion

        #region Enhanced Line Context Creation

        /// <summary>
        /// Enhanced line context class with PartId support for omission handling
        /// </summary>
        public class LineContext
        {
            public int? LineId { get; set; }
            public int LineNumber { get; set; }
            public string LineText { get; set; }
            public string RegexPattern { get; set; }
            public string WindowText { get; set; }
            public string LineName { get; set; }
            public string LineRegex { get; set; }
            public int? RegexId { get; set; }
            public List<FieldInfo> FieldsInLine { get; set; } = new List<FieldInfo>();
            public List<OCRFieldMetadata> ExistingFields { get; set; } = new List<OCRFieldMetadata>();
            public List<string> ContextLinesBefore { get; set; } = new List<string>();
            public List<string> ContextLinesAfter { get; set; } = new List<string>();
            public bool RequiresMultilineRegex { get; set; }
            public bool IsOrphaned { get; set; }
            public bool RequiresNewLineCreation { get; set; }

            // Enhanced PartId support for omission handling
            public int? PartId { get; set; }
            public string PartName { get; set; }
            public int? PartTypeId { get; set; }

            public string FullContextWithLineNumbers => string.Join("\n",
                ContextLinesBefore.Concat(new[] { $">>> Line {LineNumber}: {LineText} <<<" }).Concat(ContextLinesAfter));
        }

        /// <summary>
        /// Field information for line context
        /// </summary>
        public class FieldInfo
        {
            public int FieldId { get; set; }
            public string Key { get; set; }        // Maps to regex named group
            public string Field { get; set; }      // Database field name
            public string EntityType { get; set; }
            public string DataType { get; set; }
            public bool? IsRequired { get; set; }
        }

        #endregion

        #region Helper Methods for Enhanced Processing

        /// <summary>
        /// Gets line regex pattern from database
        /// </summary>
        private string GetLineRegexPattern(int? lineId)
        {
            if (!lineId.HasValue) return null;

            try
            {
                using var context = new OCRContext();
                var line = context.Lines
                    .Include(l => l.RegularExpressions)
                    .FirstOrDefault(l => l.Id == lineId);

                return line?.RegularExpressions?.RegEx;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting line regex pattern for LineId: {LineId}", lineId);
                return null;
            }
        }

        /// <summary>
        /// Enhanced field existence checking using regex named groups (Enhanced version)
        /// </summary>
        public bool IsFieldExistingInLineEnhanced(string deepSeekFieldName, LineContext lineContext)
        {
            var namedGroups = new List<string>();

            if (string.IsNullOrEmpty(regexPattern)) return namedGroups;

            try
            {
                // Pattern to match named groups: (?<groupName>...) or (?'groupName'...)
                var namedGroupPattern = @"\(\?<([^>]+)>|\(\?'([^']+)'";
                var matches = System.Text.RegularExpressions.Regex.Matches(regexPattern, namedGroupPattern);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    // Group 1 is for (?<name>) format, Group 2 is for (?'name') format
                    var groupName = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        namedGroups.Add(groupName);
                    }
                }

                _logger?.Debug("Extracted named groups from regex: {Groups}", string.Join(", ", namedGroups));
                return namedGroups;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error extracting named groups from regex pattern: {Pattern}", regexPattern);
                return namedGroups;
            }
        }

        /// <summary>
        /// Enhanced field existence checking using regex named groups (Enhanced version)
        /// </summary>
        public bool IsFieldExistingInLineEnhanced(string deepSeekFieldName, LineContext lineContext)
        {
            if (lineContext?.FieldsInLine == null) return false;

            try
            {
                // Map DeepSeek field name to expected Key or Field name
                var fieldMapping = MapDeepSeekFieldToDatabase(deepSeekFieldName);
                if (fieldMapping == null) return false;

                // Check if field exists by Key (regex named group) or Field name
                var exists = lineContext.FieldsInLine.Any(f =>
                    f.Key.Equals(deepSeekFieldName, StringComparison.OrdinalIgnoreCase) ||
                    f.Key.Equals(fieldMapping.DatabaseFieldName, StringComparison.OrdinalIgnoreCase) ||
                    f.Field.Equals(fieldMapping.DatabaseFieldName, StringComparison.OrdinalIgnoreCase));

                _logger?.Debug("Field existence check for {FieldName}: {Exists} in line with {FieldCount} fields",
                    deepSeekFieldName, exists, lineContext.FieldsInLine.Count);

                return exists;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error checking field existence for {FieldName}", deepSeekFieldName);
                return false;
            }
        }

        /// <summary>
        /// Gets window of text around a specific line (Enhanced version)
        /// </summary>
        private string ExtractWindowTextEnhanced(string text, int lineNumber, int windowSize)
        {
            if (string.IsNullOrEmpty(text) || lineNumber <= 0)
                return "";

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var startLine = Math.Max(0, lineNumber - windowSize - 1);
            var endLine = Math.Min(lines.Length - 1, lineNumber + windowSize - 1);

            var windowLines = new List<string>();
            for (int i = startLine; i <= endLine; i++)
            {
                windowLines.Add($"{i + 1}: {lines[i]}");
            }

            return string.Join("\n", windowLines);
        }

        /// <summary>
        /// Determines invoice type from file path or content (Enhanced version)
        /// </summary>
        private string DetermineInvoiceTypeEnhanced(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "Unknown";

            var fileName = System.IO.Path.GetFileName(filePath).ToLowerInvariant();

            if (fileName.Contains("amazon"))
                return "Amazon";
            if (fileName.Contains("temu"))
                return "Temu";
            if (fileName.Contains("shein"))
                return "Shein";
            if (fileName.Contains("alibaba"))
                return "Alibaba";

            return "Generic";
        }

        /// <summary>
        /// Validates OCR field metadata completeness
        /// </summary>
        public bool ValidateOCRFieldMetadata(OCRFieldMetadata metadata)
        {
            if (metadata == null) return false;

            var isValid = !string.IsNullOrEmpty(metadata.FieldName) &&
                         metadata.LineNumber > 0 &&
                         metadata.FieldId.HasValue;

            if (!isValid)
            {
                _logger?.Warning("Invalid OCR field metadata: FieldName={FieldName}, LineNumber={LineNumber}, FieldId={FieldId}",
                    metadata.FieldName, metadata.LineNumber, metadata.FieldId);
            }

            return isValid;
        }

        /// <summary>
        /// Creates comprehensive metadata summary for debugging
        /// </summary>
        public string CreateMetadataSummary(Dictionary<string, OCRFieldMetadata> metadata)
        {
            if (metadata == null || !metadata.Any())
                return "No metadata available";

            var summary = new System.Text.StringBuilder();
            summary.AppendLine($"OCR Metadata Summary ({metadata.Count} fields):");

            foreach (var kvp in metadata.OrderBy(m => m.Value.LineNumber))
            {
                var meta = kvp.Value;
                summary.AppendLine($"  {kvp.Key}: Line {meta.LineNumber}, FieldId {meta.FieldId}, " +
                                 $"LineId {meta.LineId}, PartId {meta.PartId} ({meta.EntityType})");
            }

            return summary.ToString();
        }

        #endregion
    }
}
