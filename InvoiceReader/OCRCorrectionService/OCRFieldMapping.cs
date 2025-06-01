// File: OCRCorrectionService/OCRFieldMapping.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // For IsFieldExistingInLine's regex group extraction
using System.Threading.Tasks; // For async GetFieldsByRegexNamedGroups
using System.Data.Entity; // For async DB operations in GetFieldsByRegexNamedGroups
using OCR.Business.Entities; // For Fields DB entity in GetFieldsByRegexNamedGroups
using Serilog; // ILogger is available as this._logger

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Field Mapping Configuration

        /// <summary>
        /// Defines the primary mapping from DeepSeek/common field names to canonical database field information.
        /// Key: Common/DeepSeek field name (case-insensitive).
        /// Value: Information about the target database field.
        /// </summary>
        private static readonly Dictionary<string, DatabaseFieldInfo> DeepSeekToDBFieldMapping = CreateFieldMapping();

        /// <summary>
        /// Creates the field mapping dictionary to avoid circular reference issues during static initialization.
        /// </summary>
        private static Dictionary<string, DatabaseFieldInfo> CreateFieldMapping()
        {
            var mapping = new Dictionary<string, DatabaseFieldInfo>(StringComparer.OrdinalIgnoreCase);

            // Invoice Header Canonical Names (Primary keys for mapping)
            var invoiceTotal = new DatabaseFieldInfo("InvoiceTotal", "ShipmentInvoice", "decimal", true, "Invoice Total Amount");
            var subTotal = new DatabaseFieldInfo("SubTotal", "ShipmentInvoice", "decimal", true, "Subtotal");
            var totalInternalFreight = new DatabaseFieldInfo("TotalInternalFreight", "ShipmentInvoice", "decimal", false, "Freight/Shipping Charges");
            var totalOtherCost = new DatabaseFieldInfo("TotalOtherCost", "ShipmentInvoice", "decimal", false, "Taxes and Other Costs");
            var totalInsurance = new DatabaseFieldInfo("TotalInsurance", "ShipmentInvoice", "decimal", false, "Insurance Charges");
            var totalDeduction = new DatabaseFieldInfo("TotalDeduction", "ShipmentInvoice", "decimal", false, "Deductions/Discounts/Gift Cards");
            var invoiceNo = new DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "string", true, "Invoice Number");
            var invoiceDate = new DatabaseFieldInfo("InvoiceDate", "ShipmentInvoice", "DateTime", true, "Invoice Date");
            var currency = new DatabaseFieldInfo("Currency", "ShipmentInvoice", "string", false, "Currency Code (e.g., USD)");
            var supplierName = new DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "string", true, "Supplier/Vendor Name");
            var supplierAddress = new DatabaseFieldInfo("SupplierAddress", "ShipmentInvoice", "string", false, "Supplier Address");

            // Invoice Detail Canonical Names (Primary keys for mapping - used when fieldName is simple like "Quantity")
            var itemDescription = new DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "string", true, "Product/Service Description");
            var quantity = new DatabaseFieldInfo("Quantity", "InvoiceDetails", "decimal", true, "Quantity");
            var cost = new DatabaseFieldInfo("Cost", "InvoiceDetails", "decimal", true, "Unit Price/Cost");
            var totalCost = new DatabaseFieldInfo("TotalCost", "InvoiceDetails", "decimal", true, "Line Item Total Amount");
            var discount = new DatabaseFieldInfo("Discount", "InvoiceDetails", "decimal", false, "Line Item Discount");
            var units = new DatabaseFieldInfo("Units", "InvoiceDetails", "string", false, "Unit of Measure (e.g., EA, KG)");

            // Add canonical names first
            mapping["InvoiceTotal"] = invoiceTotal;
            mapping["SubTotal"] = subTotal;
            mapping["TotalInternalFreight"] = totalInternalFreight;
            mapping["TotalOtherCost"] = totalOtherCost;
            mapping["TotalInsurance"] = totalInsurance;
            mapping["TotalDeduction"] = totalDeduction;
            mapping["InvoiceNo"] = invoiceNo;
            mapping["InvoiceDate"] = invoiceDate;
            mapping["Currency"] = currency;
            mapping["SupplierName"] = supplierName;
            mapping["SupplierAddress"] = supplierAddress;
            mapping["ItemDescription"] = itemDescription;
            mapping["Quantity"] = quantity;
            mapping["Cost"] = cost;
            mapping["TotalCost"] = totalCost;
            mapping["Discount"] = discount;
            mapping["Units"] = units;

            // --- ALIASES: Common variations DeepSeek might return, mapping to the canonical names above ---
            mapping["Total"] = invoiceTotal;
            mapping["GrandTotal"] = invoiceTotal;
            mapping["AmountDue"] = invoiceTotal;
            mapping["Subtotal"] = subTotal; // Note: casing difference from canonical
            mapping["Freight"] = totalInternalFreight;
            mapping["Shipping"] = totalInternalFreight;
            mapping["ShippingAndHandling"] = totalInternalFreight;
            mapping["Tax"] = totalOtherCost; // Tax is often part of Other Costs
            mapping["VAT"] = totalOtherCost;
            mapping["OtherCharges"] = totalOtherCost;
            mapping["Insurance"] = totalInsurance; // Casing
            mapping["Deduction"] = totalDeduction; // Casing
            mapping["GiftCard"] = totalDeduction;
            mapping["Promotion"] = totalDeduction;
            mapping["InvoiceNumber"] = invoiceNo;
            mapping["InvoiceID"] = invoiceNo;
            mapping["OrderNumber"] = invoiceNo; // Or map to a separate OrderNo if distinct
            mapping["OrderNo"] = invoiceNo;
            mapping["Date"] = invoiceDate; // Generic "Date" often means InvoiceDate
            mapping["IssueDate"] = invoiceDate;
            mapping["Supplier"] = supplierName;
            mapping["Vendor"] = supplierName;
            mapping["SoldBy"] = supplierName;
            mapping["From"] = supplierName; // Can be ambiguous
            mapping["Address"] = supplierAddress; // Can be ambiguous (Supplier vs Billing vs Shipping)
            mapping["Description"] = itemDescription; // For line items
            mapping["ProductDescription"] = itemDescription;
            mapping["Item"] = itemDescription; // Ambiguous, but often description
            mapping["ProductName"] = itemDescription;
            mapping["Qty"] = quantity;
            mapping["Price"] = cost; // Unit Price
            mapping["UnitPrice"] = cost;
            mapping["Amount"] = totalCost; // For line items, "Amount" usually means line total
            mapping["LineTotal"] = totalCost;
            mapping["LineItemDiscount"] = discount;

            return mapping;
        }

        /// <summary>
        /// Represents structured information about a target database field.
        /// </summary>
        public class DatabaseFieldInfo
        {
            public string DatabaseFieldName { get; } // The canonical C# property name / DB column name.
            public string EntityType { get; }      // Name of the entity class, e.g., "ShipmentInvoice", "InvoiceDetails".
            public string DataType { get; }        // Expected data type, e.g., "string", "decimal", "DateTime", "int".
            public bool IsRequired { get; }        // Whether the field is considered mandatory.
            public string DisplayName { get; }     // A user-friendly or common name for the field, useful for prompts/logs.

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

        #region Field Mapping Public Methods

        /// <summary>
        /// Maps a field name (potentially from DeepSeek or other OCR output) to its canonical DatabaseFieldInfo.
        /// Handles common prefixed field names for invoice details (e.g., "InvoiceDetail_Line1_Quantity").
        /// </summary>
        public DatabaseFieldInfo MapDeepSeekFieldToDatabase(string rawFieldName)
        {
            if (string.IsNullOrWhiteSpace(rawFieldName))
            {
                _logger?.Verbose("MapDeepSeekFieldToDatabase: Received null or empty field name.");
                return null;
            }

            string fieldNameToMap = rawFieldName.Trim();
            // Check for common prefixes used for line item fields by some systems/prompts
            if (fieldNameToMap.StartsWith("InvoiceDetail_Line", StringComparison.OrdinalIgnoreCase))
            {
                var parts = fieldNameToMap.Split('_');
                if (parts.Length >= 3) // e.g., InvoiceDetail_Line1_Quantity -> Quantity
                {
                    fieldNameToMap = parts.Last(); 
                }
            }
            
            if (DeepSeekToDBFieldMapping.TryGetValue(fieldNameToMap, out var fieldInfo))
            {
                _logger?.Verbose("Mapped raw field '{RawField}' (processed as '{MappedKey}') to DB field '{DbField}', Entity '{Entity}'.", rawFieldName, fieldNameToMap, fieldInfo.DatabaseFieldName, fieldInfo.EntityType);
                return fieldInfo;
            }

            _logger?.Debug("No mapping found for raw field '{RawField}' (processed as '{MappedKey}').", rawFieldName, fieldNameToMap);
            return null;
        }

        /// <summary>
        /// Gets all field names that have defined mappings (primary keys of the mapping dictionary).
        /// </summary>
        public IEnumerable<string> GetSupportedMappedFields() => DeepSeekToDBFieldMapping.Keys.Where(k => DeepSeekToDBFieldMapping[k] != null && DeepSeekToDBFieldMapping[k].DatabaseFieldName == k).OrderBy(k => k);


        /// <summary>
        /// Validates if a given field name (after potential prefix stripping) is supported by the mapping.
        /// </summary>
        public bool IsFieldSupported(string rawFieldName)
        {
            if (string.IsNullOrWhiteSpace(rawFieldName)) return false;
            string fieldNameToMap = rawFieldName.Trim();
            if (fieldNameToMap.StartsWith("InvoiceDetail_Line", StringComparison.OrdinalIgnoreCase))
            {
                var parts = fieldNameToMap.Split('_');
                if (parts.Length >= 3) fieldNameToMap = parts.Last();
            }
            return DeepSeekToDBFieldMapping.ContainsKey(fieldNameToMap);
        }
        
        /// <summary>
        /// Retrieves validation rules (data type, required, pattern, etc.) for a given field name.
        /// </summary>
        public FieldValidationInfo GetFieldValidationInfo(string rawFieldName)
        {
            var fieldInfo = MapDeepSeekFieldToDatabase(rawFieldName);
            if (fieldInfo == null)
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{rawFieldName}' is unknown or not mapped." };

            return new FieldValidationInfo
            {
                IsValid = true, // Indicates that we have validation rules, not that the value IS valid
                DatabaseFieldName = fieldInfo.DatabaseFieldName,
                EntityType = fieldInfo.EntityType,
                IsRequired = fieldInfo.IsRequired,
                DataType = fieldInfo.DataType,
                IsMonetary = (fieldInfo.DataType == "decimal" || fieldInfo.DataType == "currency"), // Example
                MaxLength = GetMaxLengthForField(fieldInfo), // Helper
                ValidationPattern = GetValidationPatternForField(fieldInfo) // Helper
            };
        }

        // Helper method for GetFieldValidationInfo
        private int? GetMaxLengthForField(DatabaseFieldInfo fieldInfo)
        {
            // These lengths should ideally come from database schema metadata or attributes on entity properties.
            switch (fieldInfo.DatabaseFieldName)
            {
                case "InvoiceNo": return 50;
                case "Currency": return 10;
                case "SupplierName": return 255;
                case "SupplierAddress": return 500;
                case "ItemDescription": return 1000;
                case "Units": return 50;
                default:
                    return fieldInfo.DataType?.ToLowerInvariant() == "string" ? 255 : (int?)null; // Default for other strings
            }
        }

        // Helper method for GetFieldValidationInfo
        private string GetValidationPatternForField(DatabaseFieldInfo fieldInfo)
        {
            // These are example patterns. Real-world patterns can be more complex.
            switch (fieldInfo.DataType?.ToLowerInvariant())
            {
                case "decimal": case "currency":
                    return @"^-?\$?€?£?\s*(?:\d{1,3}(?:[,.]\d{3})*|\d+)(?:[.,]\d{1,4})?$"; // Handles 1,234.56 or 1.234,56 patterns roughly
                case "datetime":
                    return @"^\d{4}-\d{2}-\d{2}(?:T\d{2}:\d{2}(?::\d{2}(?:\.\d+)?)?Z?)?$|^\d{1,2}[/\-.]\d{1,2}[/\-.]\d{2,4}$|^(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\.?\s\d{1,2}(?:st|nd|rd|th)?(?:,)?\s\d{2,4}$";
                case "int": case "integer":
                     return @"^-?\d+$";
                case "string":
                    return fieldInfo.IsRequired ? @"^\s*\S+[\s\S]*$" : @"^[\s\S]*$"; // Non-whitespace if required
                default:
                    return @"^[\s\S]*$"; // Allow anything for unknown types
            }
        }

        /// <summary>
        /// Holds validation rules for a specific field.
        /// </summary>
        public class FieldValidationInfo
        {
            public bool IsValid { get; set; } // True if validation rules could be determined for this field.
            public string DatabaseFieldName { get; set; }
            public string EntityType {get; set;}
            public bool IsRequired { get; set; }
            public string DataType { get; set; }
            public bool IsMonetary { get; set; }
            public int? MaxLength { get; set; }
            public string ValidationPattern { get; set; }
            public string ErrorMessage { get; set; } // If IsValid is false, reason why.
        }

        #endregion

        #region Regex Group and Line Context Logic Aiding Mapping

        /// <summary>
        /// Gets field definitions from the database (OCR.Business.Entities.Fields) that correspond
        /// to named groups found in a given regex pattern string, for a specific OCR.Business.Entities.Lines.Id.
        /// </summary>
        public async Task<List<FieldInfo>> GetFieldsByRegexNamedGroupsAsync(string regexPatternText, int ocrLineDefinitionId)
        {
            // Uses utility: this.ExtractNamedGroupsFromRegex
            var namedGroupsInPattern = this.ExtractNamedGroupsFromRegex(regexPatternText); 
            if (!namedGroupsInPattern.Any()) return new List<FieldInfo>();

            try
            {
                using var context = new OCRContext(); 
                var fieldsFromDb = await context.Fields // From OCR.Business.Entities
                                       .Where(f => f.LineId == ocrLineDefinitionId && namedGroupsInPattern.Contains(f.Key))
                                       .Select(f => new FieldInfo // WaterNut.DataSpace.FieldInfo
                                                        {
                                                            FieldId = f.Id, Key = f.Key, Field = f.Field,
                                                            EntityType = f.EntityType, DataType = f.DataType, IsRequired = f.IsRequired
                                                        })
                                       .ToListAsync().ConfigureAwait(false);
                
                return fieldsFromDb;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error getting fields by regex named groups for OCR Line Definition ID: {OcrLineDefId}", ocrLineDefinitionId);
                return new List<FieldInfo>();
            }
        }
        
        /// <summary>
        /// Checks if a field (identified by its DeepSeek name) is expected to be extracted by the regex
        /// associated with the provided LineContext, by checking against its defined FieldsInLine.
        /// </summary>
        public bool IsFieldExistingInLineContext(string deepSeekFieldName, LineContext lineContext)
        {
            if (lineContext == null) return false;
            
            var fieldMapping = MapDeepSeekFieldToDatabase(deepSeekFieldName);
            // If no mapping, we can't reliably check against DB field names, so only check Key against raw deepSeekFieldName.
            string keyToMatch1 = deepSeekFieldName;
            string keyToMatch2 = fieldMapping?.DisplayName; 
            string dbFieldToMatch = fieldMapping?.DatabaseFieldName;

            if (lineContext.FieldsInLine != null && lineContext.FieldsInLine.Any())
            {
                return lineContext.FieldsInLine.Any(f =>
                    (!string.IsNullOrEmpty(f.Key) && f.Key.Equals(keyToMatch1, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(keyToMatch2) && !string.IsNullOrEmpty(f.Key) && f.Key.Equals(keyToMatch2, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(dbFieldToMatch) && !string.IsNullOrEmpty(f.Field) && f.Field.Equals(dbFieldToMatch, StringComparison.OrdinalIgnoreCase))
                );
            }
            // Fallback if FieldsInLine is not populated but RegexPattern is: Parse the pattern.
            else if (!string.IsNullOrEmpty(lineContext.RegexPattern))
            {
                var groupsFromPattern = this.ExtractNamedGroupsFromRegex(lineContext.RegexPattern);
                return groupsFromPattern.Any(group =>
                     group.Equals(keyToMatch1, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(keyToMatch2) && group.Equals(keyToMatch2, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(dbFieldToMatch) && group.Equals(dbFieldToMatch, StringComparison.OrdinalIgnoreCase)) // Less likely for group name to be DB field name
                );
            }
            return false;
        }

        #endregion

        #region Enhanced Metadata Integration (Field Mapping Specific)

        /// <summary>
        /// Wraps a DatabaseFieldInfo with associated OCRFieldMetadata (runtime extraction context).
        /// </summary>
        public class EnhancedDatabaseFieldInfo : DatabaseFieldInfo
        {
            public OCRFieldMetadata OCRMetadata { get; } 
            public bool HasOCRContext => OCRMetadata != null;
            public bool CanUpdatePatternsViaContext => OCRMetadata?.LineId.HasValue == true && OCRMetadata?.RegexId.HasValue == true;
            public bool CanUpdateFieldDefinitionViaContext => OCRMetadata?.FieldId.HasValue == true;

            public EnhancedDatabaseFieldInfo(DatabaseFieldInfo baseInfo, OCRFieldMetadata metadata)
                : base(baseInfo.DatabaseFieldName, baseInfo.EntityType, baseInfo.DataType, baseInfo.IsRequired, baseInfo.DisplayName)
            {
                OCRMetadata = metadata;
            }
        }
        
        /// <summary>
        /// Maps a DeepSeek field name to an EnhancedDatabaseFieldInfo object, enriching it with OCR metadata if provided.
        /// </summary>
        public EnhancedDatabaseFieldInfo MapDeepSeekFieldToEnhancedInfo(string deepSeekFieldName, OCRFieldMetadata fieldSpecificMetadata = null)
        {
            var baseInfo = MapDeepSeekFieldToDatabase(deepSeekFieldName);
            if (baseInfo == null)
            {
                if (fieldSpecificMetadata != null && !string.IsNullOrEmpty(fieldSpecificMetadata.Field))
                {
                    // Try to construct baseInfo from metadata if direct mapping failed
                    _logger.Debug("MapDeepSeekFieldToEnhancedInfo: No direct map for '{DeepSeekName}'. Using provided metadata for Field '{MetaField}' as base.", deepSeekFieldName, fieldSpecificMetadata.Field);
                    baseInfo = new DatabaseFieldInfo(
                        fieldSpecificMetadata.Field, 
                        fieldSpecificMetadata.EntityType, 
                        fieldSpecificMetadata.DataType, 
                        fieldSpecificMetadata.IsRequired ?? false, 
                        fieldSpecificMetadata.Key ?? fieldSpecificMetadata.FieldName // Use Key or fallback to FieldName from metadata
                    );
                } else {
                     _logger.Warning("MapDeepSeekFieldToEnhancedInfo: Cannot map DeepSeek field '{DeepSeekName}' and no metadata provided for fallback.", deepSeekFieldName);
                    return null;
                }
            }
            return new EnhancedDatabaseFieldInfo(baseInfo, fieldSpecificMetadata);
        }
        
        #endregion
    }
}