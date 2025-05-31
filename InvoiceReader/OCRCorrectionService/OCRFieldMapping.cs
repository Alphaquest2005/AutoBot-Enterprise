using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Handles mapping between DeepSeek field names and database field structures
    /// </summary>
    public partial class OCRCorrectionService
    {
        #region Field Mapping Configuration

        /// <summary>
        /// Maps DeepSeek field names to database field information
        /// </summary>
        private static readonly Dictionary<string, DatabaseFieldInfo> DeepSeekFieldMapping = new Dictionary<string, DatabaseFieldInfo>(StringComparer.OrdinalIgnoreCase)
        {
            // Invoice Header Fields
            ["InvoiceTotal"] = new DatabaseFieldInfo("InvoiceTotal", "ShipmentInvoice", "decimal", true, "Total"),
            ["SubTotal"] = new DatabaseFieldInfo("SubTotal", "ShipmentInvoice", "decimal", true, "Subtotal"),
            ["TotalInternalFreight"] = new DatabaseFieldInfo("TotalInternalFreight", "ShipmentInvoice", "decimal", false, "Freight"),
            ["TotalOtherCost"] = new DatabaseFieldInfo("TotalOtherCost", "ShipmentInvoice", "decimal", false, "Other"),
            ["TotalInsurance"] = new DatabaseFieldInfo("TotalInsurance", "ShipmentInvoice", "decimal", false, "Insurance"),
            ["TotalDeduction"] = new DatabaseFieldInfo("TotalDeduction", "ShipmentInvoice", "decimal", false, "Deduction"),

            // Invoice Identification Fields
            ["InvoiceNo"] = new DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "string", true, "Invoice"),
            ["InvoiceDate"] = new DatabaseFieldInfo("InvoiceDate", "ShipmentInvoice", "DateTime", true, "Date"),
            ["Currency"] = new DatabaseFieldInfo("Currency", "ShipmentInvoice", "string", false, "Currency"),

            // Supplier Fields
            ["SupplierName"] = new DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "string", true, "Supplier"),
            ["SupplierAddress"] = new DatabaseFieldInfo("SupplierAddress", "ShipmentInvoice", "string", false, "Address"),

            // Line Item Fields
            ["ItemDescription"] = new DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "string", true, "Description"),
            ["Quantity"] = new DatabaseFieldInfo("Quantity", "InvoiceDetails", "decimal", true, "Qty"),
            ["Cost"] = new DatabaseFieldInfo("Cost", "InvoiceDetails", "decimal", true, "Price"),
            ["TotalCost"] = new DatabaseFieldInfo("TotalCost", "InvoiceDetails", "decimal", true, "LineTotal"),
            ["Discount"] = new DatabaseFieldInfo("Discount", "InvoiceDetails", "decimal", false, "Discount"),

            // Alternative field names that DeepSeek might use
            ["Total"] = new DatabaseFieldInfo("InvoiceTotal", "ShipmentInvoice", "decimal", true, "Total"),
            ["Subtotal"] = new DatabaseFieldInfo("SubTotal", "ShipmentInvoice", "decimal", true, "Subtotal"),
            ["Freight"] = new DatabaseFieldInfo("TotalInternalFreight", "ShipmentInvoice", "decimal", false, "Freight"),
            ["Shipping"] = new DatabaseFieldInfo("TotalInternalFreight", "ShipmentInvoice", "decimal", false, "Freight"),
            ["Tax"] = new DatabaseFieldInfo("TotalOtherCost", "ShipmentInvoice", "decimal", false, "Tax"),
            ["Other"] = new DatabaseFieldInfo("TotalOtherCost", "ShipmentInvoice", "decimal", false, "Other"),
            ["Insurance"] = new DatabaseFieldInfo("TotalInsurance", "ShipmentInvoice", "decimal", false, "Insurance"),
            ["Deduction"] = new DatabaseFieldInfo("TotalDeduction", "ShipmentInvoice", "decimal", false, "Deduction"),
            ["GiftCard"] = new DatabaseFieldInfo("TotalDeduction", "ShipmentInvoice", "decimal", false, "GiftCard"),

            // Invoice identification alternatives
            ["Invoice"] = new DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "string", true, "Invoice"),
            ["InvoiceNumber"] = new DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "string", true, "Invoice"),
            ["OrderNumber"] = new DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "string", true, "Order"),
            ["Date"] = new DatabaseFieldInfo("InvoiceDate", "ShipmentInvoice", "DateTime", true, "Date"),

            // Supplier alternatives
            ["Supplier"] = new DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "string", true, "Supplier"),
            ["Vendor"] = new DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "string", true, "Vendor"),
            ["From"] = new DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "string", true, "From"),
            ["Address"] = new DatabaseFieldInfo("SupplierAddress", "ShipmentInvoice", "string", false, "Address"),

            // Line item alternatives
            ["Description"] = new DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "string", true, "Description"),
            ["Item"] = new DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "string", true, "Item"),
            ["Product"] = new DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "string", true, "Product"),
            ["Qty"] = new DatabaseFieldInfo("Quantity", "InvoiceDetails", "decimal", true, "Qty"),
            ["Price"] = new DatabaseFieldInfo("Cost", "InvoiceDetails", "decimal", true, "Price"),
            ["UnitPrice"] = new DatabaseFieldInfo("Cost", "InvoiceDetails", "decimal", true, "Price"),
            ["Amount"] = new DatabaseFieldInfo("TotalCost", "InvoiceDetails", "decimal", true, "Amount"),
            ["LineTotal"] = new DatabaseFieldInfo("TotalCost", "InvoiceDetails", "decimal", true, "LineTotal")
        };

        /// <summary>
        /// Information about a database field for mapping purposes
        /// </summary>
        public class DatabaseFieldInfo
        {
            public string DatabaseFieldName { get; }
            public string EntityType { get; }
            public string DataType { get; }
            public bool IsRequired { get; }
            public string DisplayName { get; }

            public DatabaseFieldInfo(string databaseFieldName, string entityType, string dataType, bool isRequired, string displayName)
            {
                DatabaseFieldName = databaseFieldName;
                EntityType = entityType;
                DataType = dataType;
                IsRequired = isRequired;
                DisplayName = displayName;
            }
        }

        #endregion

        #region Field Mapping Methods

        /// <summary>
        /// Maps a DeepSeek field name to database field information
        /// </summary>
        /// <param name="deepSeekFieldName">Field name from DeepSeek response</param>
        /// <returns>Database field information or null if not found</returns>
        public DatabaseFieldInfo MapDeepSeekFieldToDatabase(string deepSeekFieldName)
        {
            if (string.IsNullOrWhiteSpace(deepSeekFieldName))
            {
                _logger?.Warning("Empty or null DeepSeek field name provided for mapping");
                return null;
            }

            if (DeepSeekFieldMapping.TryGetValue(deepSeekFieldName.Trim(), out var fieldInfo))
            {
                _logger?.Debug("Mapped DeepSeek field '{DeepSeekField}' to database field '{DatabaseField}' in entity '{Entity}'",
                    deepSeekFieldName, fieldInfo.DatabaseFieldName, fieldInfo.EntityType);
                return fieldInfo;
            }

            _logger?.Warning("No mapping found for DeepSeek field '{DeepSeekField}'. Available mappings: {AvailableMappings}",
                deepSeekFieldName, string.Join(", ", DeepSeekFieldMapping.Keys.Take(10)));

            return null;
        }

        /// <summary>
        /// Gets all supported DeepSeek field names
        /// </summary>
        /// <returns>List of supported field names</returns>
        public IEnumerable<string> GetSupportedDeepSeekFields()
        {
            return DeepSeekFieldMapping.Keys.OrderBy(k => k);
        }

        /// <summary>
        /// Gets field mappings for a specific entity type
        /// </summary>
        /// <param name="entityType">Entity type (e.g., "ShipmentInvoice", "InvoiceDetails")</param>
        /// <returns>Field mappings for the entity</returns>
        public IEnumerable<KeyValuePair<string, DatabaseFieldInfo>> GetFieldMappingsForEntity(string entityType)
        {
            return DeepSeekFieldMapping.Where(kvp =>
                string.Equals(kvp.Value.EntityType, entityType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Validates if a field name is supported for correction
        /// </summary>
        /// <param name="fieldName">Field name to validate</param>
        /// <returns>True if field is supported</returns>
        public bool IsFieldSupported(string fieldName)
        {
            return !string.IsNullOrWhiteSpace(fieldName) &&
                   DeepSeekFieldMapping.ContainsKey(fieldName.Trim());
        }

        /// <summary>
        /// Gets the canonical database field name for a DeepSeek field
        /// </summary>
        /// <param name="deepSeekFieldName">DeepSeek field name</param>
        /// <returns>Canonical database field name or original if not mapped</returns>
        public string GetCanonicalFieldName(string deepSeekFieldName)
        {
            var fieldInfo = MapDeepSeekFieldToDatabase(deepSeekFieldName);
            return fieldInfo?.DatabaseFieldName ?? deepSeekFieldName;
        }

        /// <summary>
        /// Determines if a field is a monetary/currency field
        /// </summary>
        /// <param name="fieldName">Field name to check</param>
        /// <returns>True if field represents currency</returns>
        public bool IsMonetaryField(string fieldName)
        {
            var fieldInfo = MapDeepSeekFieldToDatabase(fieldName);
            return fieldInfo?.DataType == "decimal" &&
                   (fieldInfo.EntityType == "ShipmentInvoice" ||
                    fieldInfo.DatabaseFieldName.Contains("Cost") ||
                    fieldInfo.DatabaseFieldName.Contains("Total") ||
                    fieldInfo.DatabaseFieldName.Contains("Price"));
        }

        /// <summary>
        /// Gets validation rules for a specific field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Validation information</returns>
        public FieldValidationInfo GetFieldValidationInfo(string fieldName)
        {
            var fieldInfo = MapDeepSeekFieldToDatabase(fieldName);
            if (fieldInfo == null)
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Unknown field: {fieldName}" };

            return new FieldValidationInfo
            {
                IsValid = true,
                IsRequired = fieldInfo.IsRequired,
                DataType = fieldInfo.DataType,
                IsMonetary = IsMonetaryField(fieldName),
                MaxLength = GetMaxLengthForField(fieldInfo),
                ValidationPattern = GetValidationPatternForField(fieldInfo)
            };
        }

        private int? GetMaxLengthForField(DatabaseFieldInfo fieldInfo)
        {
            // Define max lengths based on database schema
            switch (fieldInfo.DatabaseFieldName)
            {
                case "InvoiceNo":
                case "Currency":
                    return 50;
                case "SupplierName":
                    return 255;
                case "SupplierAddress":
                    return 500;
                case "ItemDescription":
                    return 1000;
                default:
                    return fieldInfo.DataType == "string" ? 255 : null;
            }
        }

        private string GetValidationPatternForField(DatabaseFieldInfo fieldInfo)
        {
            switch (fieldInfo.DataType)
            {
                case "decimal":
                    return @"^-?\d+(\.\d{1,4})?$"; // Allow up to 4 decimal places
                case "DateTime":
                    return @"^\d{1,2}[\/\-]\d{1,2}[\/\-]\d{2,4}$"; // Basic date pattern
                case "string":
                    return fieldInfo.IsRequired ? @"^.+$" : @"^.*$"; // Non-empty if required
                default:
                    return @"^.*$";
            }
        }

        /// <summary>
        /// Field validation information
        /// </summary>
        public class FieldValidationInfo
        {
            public bool IsValid { get; set; }
            public bool IsRequired { get; set; }
            public string DataType { get; set; }
            public bool IsMonetary { get; set; }
            public int? MaxLength { get; set; }
            public string ValidationPattern { get; set; }
            public string ErrorMessage { get; set; }
        }

        #endregion

        #region Regex Named Group Extraction

        /// <summary>
        /// Extracts named groups from a regex pattern
        /// </summary>
        /// <param name="regexPattern">Regex pattern to analyze</param>
        /// <returns>List of named group names</returns>
        public List<string> ExtractNamedGroupsFromRegex(string regexPattern)
        {
            var namedGroups = new List<string>();

            if (string.IsNullOrEmpty(regexPattern)) return namedGroups;

            try
            {
                // Pattern to match named groups: (?<groupName>...) or (?'groupName'...)
                var namedGroupPattern = @"\(\?<([^>]+)>|\(\?'([^']+)'";
                var matches = Regex.Matches(regexPattern, namedGroupPattern);

                foreach (Match match in matches)
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
        /// Gets fields by regex named groups from database (CONSOLIDATED ENHANCED VERSION)
        /// Combines functionality from OCRMetadataExtractor.cs and adds comprehensive error handling
        /// </summary>
        /// <param name="regexPattern">Regex pattern to analyze</param>
        /// <param name="lineId">Line ID to filter fields</param>
        /// <returns>List of field information</returns>
        public async Task<List<FieldInfo>> GetFieldsByRegexNamedGroups(string regexPattern, int lineId)
        {
            try
            {
                using var context = new OCRContext();

                // Extract named groups from regex pattern
                var namedGroups = ExtractNamedGroupsFromRegex(regexPattern);

                // Find fields where Key matches the named groups
                var fields = await context.Fields
                    .Where(f => f.LineId == lineId && namedGroups.Contains(f.Key))
                    .Select(f => new FieldInfo
                    {
                        FieldId = f.Id,
                        Key = f.Key,
                        Field = f.Field,
                        EntityType = f.EntityType,
                        DataType = f.DataType,
                        IsRequired = f.IsRequired  // ENHANCED: Added IsRequired property
                    })
                    .ToListAsync();

                _logger?.Debug("Found {FieldCount} fields matching named groups for LineId {LineId}",
                    fields.Count, lineId);

                return fields;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting fields by regex named groups for LineId: {LineId}", lineId);
                return new List<FieldInfo>();
            }
        }

        /// <summary>
        /// Checks if a field exists in a line's regex named groups
        /// </summary>
        /// <param name="deepSeekFieldName">DeepSeek field name</param>
        /// <param name="lineContext">Line context with field information</param>
        /// <returns>True if field exists in line</returns>
        public bool IsFieldExistingInLine(string deepSeekFieldName, LineContext lineContext)
        {
            if (lineContext?.FieldsInLine == null) return false;

            // Map DeepSeek field name to expected Key or Field name
            var fieldMapping = MapDeepSeekFieldToDatabase(deepSeekFieldName);
            if (fieldMapping == null) return false;

            // Check if field exists by Key (regex named group) or Field name
            return lineContext.FieldsInLine.Any(f =>
                f.Key.Equals(deepSeekFieldName, StringComparison.OrdinalIgnoreCase) ||
                f.Key.Equals(fieldMapping.DatabaseFieldName, StringComparison.OrdinalIgnoreCase) ||
                f.Field.Equals(fieldMapping.DatabaseFieldName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Enhanced Metadata Integration

        /// <summary>
        /// Maps DeepSeek field name to database field and enriches with OCR metadata context
        /// </summary>
        /// <param name="deepSeekFieldName">Field name from DeepSeek response</param>
        /// <param name="metadata">OCR metadata for the field (optional)</param>
        /// <returns>Enhanced database field information with OCR context</returns>
        public EnhancedDatabaseFieldInfo MapDeepSeekFieldWithMetadata(string deepSeekFieldName, OCRFieldMetadata metadata = null)
        {
            var baseFieldInfo = MapDeepSeekFieldToDatabase(deepSeekFieldName);
            if (baseFieldInfo == null)
            {
                _logger?.Warning("No mapping found for DeepSeek field '{DeepSeekField}' in enhanced mapping", deepSeekFieldName);
                return null;
            }

            return new EnhancedDatabaseFieldInfo(
                baseFieldInfo.DatabaseFieldName,
                baseFieldInfo.EntityType,
                baseFieldInfo.DataType,
                baseFieldInfo.IsRequired,
                baseFieldInfo.DisplayName,
                metadata);
        }

        /// <summary>
        /// Gets database update context for a field correction using enhanced metadata
        /// </summary>
        /// <param name="fieldName">Field name being corrected</param>
        /// <param name="metadata">OCR metadata for the field</param>
        /// <returns>Database update context information</returns>
        public DatabaseUpdateContext GetDatabaseUpdateContext(string fieldName, OCRFieldMetadata metadata)
        {
            var enhancedFieldInfo = MapDeepSeekFieldWithMetadata(fieldName, metadata);
            if (enhancedFieldInfo == null)
            {
                return new DatabaseUpdateContext { IsValid = false, ErrorMessage = $"Unknown field: {fieldName}" };
            }

            return new DatabaseUpdateContext
            {
                IsValid = true,
                FieldInfo = enhancedFieldInfo, // Store as object, will be cast to EnhancedDatabaseFieldInfo when used
                UpdateStrategy = DetermineUpdateStrategy(enhancedFieldInfo),
                RequiredIds = GetRequiredDatabaseIds(metadata),
                ValidationRules = GetFieldValidationInfo(fieldName) // Store as object, will be cast to FieldValidationInfo when used
            };
        }

        /// <summary>
        /// Determines the appropriate database update strategy based on field metadata
        /// </summary>
        private DatabaseUpdateStrategy DetermineUpdateStrategy(EnhancedDatabaseFieldInfo fieldInfo)
        {
            if (!fieldInfo.HasOCRContext)
            {
                return DatabaseUpdateStrategy.SkipUpdate; // No OCR context, can't update database
            }

            var metadata = fieldInfo.OCRMetadata;

            // If we have complete OCR context, we can update regex patterns
            if (metadata.FieldId.HasValue && metadata.LineId.HasValue && metadata.RegexId.HasValue)
            {
                return DatabaseUpdateStrategy.UpdateRegexPattern;
            }

            // If we have field context but no regex, we might need to create new patterns
            if (metadata.FieldId.HasValue && metadata.LineId.HasValue)
            {
                return DatabaseUpdateStrategy.CreateNewPattern;
            }

            // If we only have basic context, update field format rules
            if (metadata.FieldId.HasValue)
            {
                return DatabaseUpdateStrategy.UpdateFieldFormat;
            }

            return DatabaseUpdateStrategy.LogOnly; // Log the correction but don't update database
        }

        /// <summary>
        /// Gets required database IDs for update operations
        /// </summary>
        private RequiredDatabaseIds GetRequiredDatabaseIds(OCRFieldMetadata metadata)
        {
            return new RequiredDatabaseIds
            {
                FieldId = metadata?.FieldId,
                LineId = metadata?.LineId,
                RegexId = metadata?.RegexId,
                InvoiceId = metadata?.InvoiceId,
                PartId = metadata?.PartId
            };
        }

        /// <summary>
        /// Enhanced database field information with OCR metadata context
        /// </summary>
        public class EnhancedDatabaseFieldInfo : DatabaseFieldInfo
        {
            public OCRFieldMetadata OCRMetadata { get; set; }
            public bool HasOCRContext { get; set; }
            public bool CanUpdateDatabase { get; set; }

            public EnhancedDatabaseFieldInfo() : base("", "", "", false, "") { }

            public EnhancedDatabaseFieldInfo(string databaseFieldName, string entityType, string dataType, bool isRequired, string displayName, OCRFieldMetadata metadata)
                : base(databaseFieldName, entityType, dataType, isRequired, displayName)
            {
                OCRMetadata = metadata;
                HasOCRContext = metadata != null;
                CanUpdateDatabase = metadata?.FieldId.HasValue == true && metadata?.LineId.HasValue == true;
            }
        }

        #endregion
    }
}