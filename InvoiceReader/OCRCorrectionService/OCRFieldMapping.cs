using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
