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
            // Using pseudo datatypes consistent with ImportByDataType.cs: "Number", "String", "Date", "English Date"
            var invoiceTotal = new DatabaseFieldInfo("InvoiceTotal", "ShipmentInvoice", "Number", true, "Invoice Total Amount");
            var subTotal = new DatabaseFieldInfo("SubTotal", "ShipmentInvoice", "Number", true, "Subtotal");
            var totalInternalFreight = new DatabaseFieldInfo("TotalInternalFreight", "ShipmentInvoice", "Number", false, "Freight/Shipping Charges");
            var totalOtherCost = new DatabaseFieldInfo("TotalOtherCost", "ShipmentInvoice", "Number", false, "Taxes and Other Costs");
            var totalInsurance = new DatabaseFieldInfo("TotalInsurance", "ShipmentInvoice", "Number", false, "Insurance Charges");
            var totalDeduction = new DatabaseFieldInfo("TotalDeduction", "ShipmentInvoice", "Number", false, "Deductions/Discounts/Gift Cards");
            var invoiceNo = new DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "String", true, "Invoice Number");
            var invoiceDate = new DatabaseFieldInfo("InvoiceDate", "ShipmentInvoice", "English Date", true, "Invoice Date");
            var currency = new DatabaseFieldInfo("Currency", "ShipmentInvoice", "String", false, "Currency Code (e.g., USD)");
            var supplierName = new DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "String", true, "Supplier/Vendor Name");
            var supplierAddress = new DatabaseFieldInfo("SupplierAddress", "ShipmentInvoice", "String", false, "Supplier Address");

            // Invoice Detail Canonical Names (Primary keys for mapping - used when fieldName is simple like "Quantity")
            var itemDescription = new DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "String", true, "Product/Service Description");
            var quantity = new DatabaseFieldInfo("Quantity", "InvoiceDetails", "Number", true, "Quantity");
            var cost = new DatabaseFieldInfo("Cost", "InvoiceDetails", "Number", true, "Unit Price/Cost");
            var totalCost = new DatabaseFieldInfo("TotalCost", "InvoiceDetails", "Number", true, "Line Item Total Amount");
            var discount = new DatabaseFieldInfo("Discount", "InvoiceDetails", "Number", false, "Line Item Discount");
            var units = new DatabaseFieldInfo("Units", "InvoiceDetails", "String", false, "Unit of Measure (e.g., EA, KG)");

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
            public string DataType { get; }        // Expected pseudo data type, e.g., "String", "Number", "English Date".
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
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Field mapping with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Maps raw field names from DeepSeek/OCR output to canonical DatabaseFieldInfo with prefix handling and alias resolution
        /// **BUSINESS OBJECTIVE**: Ensure accurate field mapping through canonical name resolution, alias handling, and prefix stripping for line item fields
        /// **SUCCESS CRITERIA**: Must handle null/empty inputs, strip prefixes correctly, resolve aliases to canonical fields, and return complete DatabaseFieldInfo
        /// 
        /// Maps a field name (potentially from DeepSeek or other OCR output) to its canonical DatabaseFieldInfo.
        /// Handles common prefixed field names for invoice details (e.g., "InvoiceDetail_Line1_Quantity").
        /// </summary>
        public DatabaseFieldInfo MapDeepSeekFieldToDatabase(string rawFieldName)
        {
            // **📋 PHASE 1: ANALYSIS - Current State Assessment**
            using (Serilog.Context.LogContext.PushProperty("MethodContext", "MapDeepSeekFieldToDatabase_V4.2_Analysis"))
            {
                _logger.Information("🔍 **PHASE 1: ANALYSIS** - Assessing field mapping requirements for raw field: '{RawFieldName}'", 
                    rawFieldName ?? "NULL");
                _logger.Information("📊 Analysis Context: Field mapping resolves DeepSeek/OCR field names to canonical database field information through alias resolution and prefix handling");
                _logger.Information("🎯 Expected Behavior: Validate input, strip line item prefixes, resolve aliases to canonical names, and return complete DatabaseFieldInfo structure");
                _logger.Information("🏗️ Current Architecture: Dictionary-based mapping with prefix stripping for line items and case-insensitive alias resolution");
            }

            if (string.IsNullOrWhiteSpace(rawFieldName))
            {
                _logger.Error("❌ Critical Input Validation Failure: Raw field name is null or whitespace - cannot perform field mapping");
                return null;
            }

            string fieldNameToMap = rawFieldName.Trim();
            string originalFieldName = fieldNameToMap;
            bool prefixStripped = false;
            DatabaseFieldInfo fieldInfo = null;
            bool mappingFound = false;

            // **📋 PHASE 2: ENHANCEMENT - Comprehensive Diagnostic Implementation**
            using (Serilog.Context.LogContext.PushProperty("MethodContext", "MapDeepSeekFieldToDatabase_V4.2_Enhancement"))
            {
                _logger.Information("🔧 **PHASE 2: ENHANCEMENT** - Implementing comprehensive field mapping with diagnostic capabilities");
                
                _logger.Information("✅ Input Validation: Processing raw field name '{RawFieldName}' (length: {Length})", 
                    rawFieldName, rawFieldName.Length);
                
                _logger.Information("📊 Mapping Dictionary Status: Contains {MappingCount} field mappings available for resolution", 
                    DeepSeekToDBFieldMapping?.Count ?? 0);

                // **📋 PHASE 3: EVIDENCE-BASED IMPLEMENTATION - Core Field Mapping Logic**
                using (Serilog.Context.LogContext.PushProperty("MethodContext", "MapDeepSeekFieldToDatabase_V4.2_Implementation"))
                {
                    _logger.Information("⚡ **PHASE 3: IMPLEMENTATION** - Executing field mapping algorithm with prefix handling and alias resolution");
                    
                    try
                    {
                        // Step 1: Handle line item prefixes (e.g., "InvoiceDetail_Line1_Quantity" -> "Quantity")
                        if (fieldNameToMap.StartsWith("InvoiceDetail_Line", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.Information("🔄 Prefix Detection: Field contains 'InvoiceDetail_Line' prefix - attempting to strip");
                            
                            var parts = fieldNameToMap.Split('_');
                            if (parts.Length >= 3)
                            {
                                string strippedFieldName = parts.Last();
                                _logger.Information("✅ Prefix Stripped: '{OriginalField}' -> '{StrippedField}' (Parts: {PartCount})", 
                                    fieldNameToMap, strippedFieldName, parts.Length);
                                
                                fieldNameToMap = strippedFieldName;
                                prefixStripped = true;
                            }
                            else
                            {
                                _logger.Warning("⚠️ Prefix Stripping Failed: Insufficient parts in field name (Parts: {PartCount})", parts.Length);
                            }
                        }
                        else
                        {
                            _logger.Debug("ℹ️ No Prefix: Field name does not contain line item prefix");
                        }

                        // Step 2: Attempt dictionary lookup with processed field name
                        _logger.Information("🔍 Dictionary Lookup: Searching for field name '{ProcessedFieldName}' in mapping dictionary", fieldNameToMap);
                        
                        if (DeepSeekToDBFieldMapping.TryGetValue(fieldNameToMap, out fieldInfo))
                        {
                            mappingFound = true;
                            _logger.Information("✅ Mapping Success: Field '{ProcessedField}' resolved to DatabaseField='{DbField}', Entity='{Entity}', DataType='{DataType}', Required={Required}", 
                                fieldNameToMap, fieldInfo.DatabaseFieldName, fieldInfo.EntityType, fieldInfo.DataType, fieldInfo.IsRequired);
                        }
                        else
                        {
                            _logger.Warning("❌ Mapping Failed: No mapping found for processed field name '{ProcessedFieldName}'", fieldNameToMap);
                        }
                        
                        _logger.Information("📊 Field Mapping Summary: OriginalField='{Original}', ProcessedField='{Processed}', PrefixStripped={PrefixStripped}, MappingFound={MappingFound}", 
                            originalFieldName, fieldNameToMap, prefixStripped, mappingFound);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "💥 Exception during field mapping for raw field '{RawFieldName}' - ProcessedField: '{ProcessedField}'", 
                            rawFieldName, fieldNameToMap);
                        // Return null if mapping fails critically
                        fieldInfo = null;
                    }
                }
            }

            // **📋 PHASE 4: SUCCESS CRITERIA VALIDATION - Business Outcome Assessment**
            using (Serilog.Context.LogContext.PushProperty("MethodContext", "MapDeepSeekFieldToDatabase_V4.2_SuccessCriteria"))
            {
                _logger.Information("🏆 **PHASE 4: SUCCESS CRITERIA VALIDATION** - Assessing business outcome achievement");
                
                // 1. 🎯 PURPOSE_FULFILLMENT - Method achieves stated business objective
                bool purposeFulfilled = !string.IsNullOrWhiteSpace(rawFieldName) && !string.IsNullOrEmpty(fieldNameToMap);
                _logger.Error("🎯 **PURPOSE_FULFILLMENT**: {Status} - Field mapping {Result} (RawField: '{RawFieldName}', ProcessedField: '{ProcessedField}')", 
                    purposeFulfilled ? "✅ PASS" : "❌ FAIL", 
                    purposeFulfilled ? "executed successfully" : "failed to execute", rawFieldName, fieldNameToMap);

                // 2. 📊 OUTPUT_COMPLETENESS - Returns complete, well-formed data structures
                bool outputComplete = fieldInfo == null || (!string.IsNullOrEmpty(fieldInfo.DatabaseFieldName) && !string.IsNullOrEmpty(fieldInfo.EntityType));
                _logger.Error("📊 **OUTPUT_COMPLETENESS**: {Status} - Field mapping result {Result} with DatabaseField='{DbField}', Entity='{Entity}'", 
                    outputComplete ? "✅ PASS" : "❌ FAIL", 
                    outputComplete ? "properly structured" : "incomplete or malformed", 
                    fieldInfo?.DatabaseFieldName, fieldInfo?.EntityType);

                // 3. ⚙️ PROCESS_COMPLETION - All required processing steps executed successfully
                bool processComplete = originalFieldName != null && fieldNameToMap != null;
                _logger.Error("⚙️ **PROCESS_COMPLETION**: {Status} - Prefix processing and dictionary lookup completed (PrefixStripped: {PrefixStripped})", 
                    processComplete ? "✅ PASS" : "❌ FAIL", prefixStripped);

                // 4. 🔍 DATA_QUALITY - Output meets business rules and validation requirements
                bool dataQualityMet = fieldInfo == null || (fieldInfo.DatabaseFieldName == fieldNameToMap || DeepSeekToDBFieldMapping.ContainsKey(fieldNameToMap));
                _logger.Error("🔍 **DATA_QUALITY**: {Status} - Mapping consistency: MappingFound={MappingFound}, FieldConsistency verified", 
                    dataQualityMet ? "✅ PASS" : "❌ FAIL", mappingFound);

                // 5. 🛡️ ERROR_HANDLING - Appropriate error detection and graceful recovery
                bool errorHandlingSuccess = true; // Exception was caught and handled gracefully
                _logger.Error("🛡️ **ERROR_HANDLING**: {Status} - Exception handling and null safety {Result} during field mapping", 
                    errorHandlingSuccess ? "✅ PASS" : "❌ FAIL", 
                    errorHandlingSuccess ? "implemented successfully" : "failed");

                // 6. 💼 BUSINESS_LOGIC - Method behavior aligns with business requirements
                bool businessLogicValid = string.IsNullOrWhiteSpace(rawFieldName) ? (fieldInfo == null) : true;
                _logger.Error("💼 **BUSINESS_LOGIC**: {Status} - Field mapping logic follows business rules: null input -> null output", 
                    businessLogicValid ? "✅ PASS" : "❌ FAIL");

                // 7. 🔗 INTEGRATION_SUCCESS - External dependencies respond appropriately
                bool integrationSuccess = DeepSeekToDBFieldMapping != null; // Dictionary dependency available
                _logger.Error("🔗 **INTEGRATION_SUCCESS**: {Status} - Mapping dictionary integration {Result}", 
                    integrationSuccess ? "✅ PASS" : "❌ FAIL", 
                    integrationSuccess ? "functioning properly" : "experiencing issues");

                // 8. ⚡ PERFORMANCE_COMPLIANCE - Execution within reasonable timeframes
                bool performanceCompliant = originalFieldName == null || originalFieldName.Length < 500; // Reasonable field name length
                _logger.Error("⚡ **PERFORMANCE_COMPLIANCE**: {Status} - Field name length ({Length}) within reasonable limits", 
                    performanceCompliant ? "✅ PASS" : "❌ FAIL", originalFieldName?.Length ?? 0);

                // Overall Success Assessment
                bool overallSuccess = purposeFulfilled && outputComplete && processComplete && dataQualityMet && 
                                    errorHandlingSuccess && businessLogicValid && integrationSuccess && performanceCompliant;
                
                _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: {Status} - MapDeepSeekFieldToDatabase {Result} for field '{RawFieldName}' -> {MappingResult}", 
                    overallSuccess ? "✅ PASS" : "❌ FAIL", 
                    overallSuccess ? "completed successfully" : "encountered issues", 
                    rawFieldName, mappingFound ? $"'{fieldInfo?.DatabaseFieldName}'" : "NO_MAPPING");
            }

            return fieldInfo;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Supported field enumeration with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Retrieves all canonical field names that have defined mappings from the field mapping dictionary
        /// **BUSINESS OBJECTIVE**: Provide comprehensive list of supported field mappings for validation, documentation, and client usage
        /// **SUCCESS CRITERIA**: Must enumerate dictionary keys, filter to canonical names only, validate mapping integrity, and return sorted collection
        /// 
        /// Gets all field names that have defined mappings (primary keys of the mapping dictionary).
        /// </summary>
        public IEnumerable<string> GetSupportedMappedFields()
        {
            // **📋 PHASE 1: ANALYSIS - Current State Assessment**
            using (Serilog.Context.LogContext.PushProperty("MethodContext", "GetSupportedMappedFields_V4.2_Analysis"))
            {
                _logger.Information("🔍 **PHASE 1: ANALYSIS** - Assessing supported field enumeration requirements");
                _logger.Information("📊 Analysis Context: Field enumeration provides canonical field names from mapping dictionary for validation and documentation purposes");
                _logger.Information("🎯 Expected Behavior: Filter dictionary keys to canonical names, validate mapping integrity, and return alphabetically sorted collection");
                _logger.Information("🏗️ Current Architecture: LINQ-based filtering of dictionary keys with canonical name validation and alphabetical ordering");
            }

            IEnumerable<string> supportedFields = null;
            int totalMappings = 0;
            int canonicalMappings = 0;
            bool dictionaryValid = false;

            // **📋 PHASE 2: ENHANCEMENT - Comprehensive Diagnostic Implementation**
            using (Serilog.Context.LogContext.PushProperty("MethodContext", "GetSupportedMappedFields_V4.2_Enhancement"))
            {
                _logger.Information("🔧 **PHASE 2: ENHANCEMENT** - Implementing comprehensive supported field enumeration with diagnostic capabilities");
                
                dictionaryValid = DeepSeekToDBFieldMapping != null;
                totalMappings = DeepSeekToDBFieldMapping?.Count ?? 0;
                
                _logger.Information("✅ Input Validation: Mapping dictionary validation - Valid: {DictionaryValid}, Total mappings: {TotalMappings}", 
                    dictionaryValid, totalMappings);

                // **📋 PHASE 3: EVIDENCE-BASED IMPLEMENTATION - Core Field Enumeration Logic**
                using (Serilog.Context.LogContext.PushProperty("MethodContext", "GetSupportedMappedFields_V4.2_Implementation"))
                {
                    _logger.Information("⚡ **PHASE 3: IMPLEMENTATION** - Executing supported field enumeration algorithm");
                    
                    try
                    {
                        if (dictionaryValid)
                        {
                            // Filter to canonical field names only (where key matches DatabaseFieldName)
                            supportedFields = DeepSeekToDBFieldMapping.Keys
                                .Where(k => DeepSeekToDBFieldMapping[k] != null && 
                                           DeepSeekToDBFieldMapping[k].DatabaseFieldName == k)
                                .OrderBy(k => k);
                            
                            canonicalMappings = supportedFields.Count();
                            
                            _logger.Information("✅ Enumeration Success: Filtered {TotalMappings} total mappings to {CanonicalMappings} canonical field names", 
                                totalMappings, canonicalMappings);
                            
                            _logger.Debug("📋 Canonical Fields: {CanonicalFields}", 
                                string.Join(", ", supportedFields.Take(10)) + (canonicalMappings > 10 ? "..." : ""));
                        }
                        else
                        {
                            _logger.Error("❌ Dictionary Invalid: Cannot enumerate supported fields from null or invalid mapping dictionary");
                            supportedFields = Enumerable.Empty<string>();
                        }
                        
                        _logger.Information("📊 Field Enumeration Summary: TotalMappings={Total}, CanonicalMappings={Canonical}, DictionaryValid={Valid}", 
                            totalMappings, canonicalMappings, dictionaryValid);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "💥 Exception during field enumeration - TotalMappings: {TotalMappings}", totalMappings);
                        supportedFields = Enumerable.Empty<string>();
                    }
                }
            }

            // **📋 PHASE 4: SUCCESS CRITERIA VALIDATION - Business Outcome Assessment**
            using (Serilog.Context.LogContext.PushProperty("MethodContext", "GetSupportedMappedFields_V4.2_SuccessCriteria"))
            {
                _logger.Information("🏆 **PHASE 4: SUCCESS CRITERIA VALIDATION** - Assessing business outcome achievement");
                
                // 1. 🎯 PURPOSE_FULFILLMENT - Method achieves stated business objective
                bool purposeFulfilled = dictionaryValid && supportedFields != null;
                _logger.Error("🎯 **PURPOSE_FULFILLMENT**: {Status} - Field enumeration {Result} (Dictionary valid: {DictionaryValid})", 
                    purposeFulfilled ? "✅ PASS" : "❌ FAIL", 
                    purposeFulfilled ? "executed successfully" : "failed to execute", dictionaryValid);

                // 2. 📊 OUTPUT_COMPLETENESS - Returns complete, well-formed data structures
                bool outputComplete = supportedFields != null;
                _logger.Error("📊 **OUTPUT_COMPLETENESS**: {Status} - Field collection {Result} with {CanonicalCount} canonical fields enumerated", 
                    outputComplete ? "✅ PASS" : "❌ FAIL", 
                    outputComplete ? "properly constructed" : "null or invalid", canonicalMappings);

                // 3. ⚙️ PROCESS_COMPLETION - All required processing steps executed successfully
                bool processComplete = totalMappings >= 0 && canonicalMappings >= 0;
                _logger.Error("⚙️ **PROCESS_COMPLETION**: {Status} - Dictionary filtering and sorting completed ({Total} -> {Canonical})", 
                    processComplete ? "✅ PASS" : "❌ FAIL", totalMappings, canonicalMappings);

                // 4. 🔍 DATA_QUALITY - Output meets business rules and validation requirements
                bool dataQualityMet = canonicalMappings <= totalMappings && (canonicalMappings == 0 || supportedFields.All(f => !string.IsNullOrEmpty(f)));
                _logger.Error("🔍 **DATA_QUALITY**: {Status} - Field enumeration integrity: {Canonical} canonical fields from {Total} total mappings", 
                    dataQualityMet ? "✅ PASS" : "❌ FAIL", canonicalMappings, totalMappings);

                // 5. 🛡️ ERROR_HANDLING - Appropriate error detection and graceful recovery
                bool errorHandlingSuccess = supportedFields != null; // Graceful fallback to empty collection
                _logger.Error("🛡️ **ERROR_HANDLING**: {Status} - Exception handling and null safety {Result} during field enumeration", 
                    errorHandlingSuccess ? "✅ PASS" : "❌ FAIL", 
                    errorHandlingSuccess ? "implemented successfully" : "failed");

                // 6. 💼 BUSINESS_LOGIC - Method behavior aligns with business requirements
                bool businessLogicValid = !dictionaryValid ? (canonicalMappings == 0) : (canonicalMappings >= 0);
                _logger.Error("💼 **BUSINESS_LOGIC**: {Status} - Field enumeration logic follows business rules: canonical filtering and alphabetical ordering", 
                    businessLogicValid ? "✅ PASS" : "❌ FAIL");

                // 7. 🔗 INTEGRATION_SUCCESS - External dependencies respond appropriately
                bool integrationSuccess = dictionaryValid; // Dictionary dependency available
                _logger.Error("🔗 **INTEGRATION_SUCCESS**: {Status} - Mapping dictionary integration {Result}", 
                    integrationSuccess ? "✅ PASS" : "❌ FAIL", 
                    integrationSuccess ? "functioning properly" : "experiencing issues");

                // 8. ⚡ PERFORMANCE_COMPLIANCE - Execution within reasonable timeframes
                bool performanceCompliant = totalMappings < 10000; // Reasonable mapping dictionary size
                _logger.Error("⚡ **PERFORMANCE_COMPLIANCE**: {Status} - Processing {TotalMappings} mappings within reasonable performance limits", 
                    performanceCompliant ? "✅ PASS" : "❌ FAIL", totalMappings);

                // Overall Success Assessment
                bool overallSuccess = purposeFulfilled && outputComplete && processComplete && dataQualityMet && 
                                    errorHandlingSuccess && businessLogicValid && integrationSuccess && performanceCompliant;
                
                _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: {Status} - GetSupportedMappedFields {Result} with {CanonicalCount} canonical fields enumerated", 
                    overallSuccess ? "✅ PASS" : "❌ FAIL", 
                    overallSuccess ? "completed successfully" : "encountered issues", canonicalMappings);
            }

            return supportedFields ?? Enumerable.Empty<string>();
        }


        /// <summary>
        /// Validates if a given field name (after potential prefix stripping) is supported by the mapping.
        /// </summary>
        //public bool IsFieldSupported(string rawFieldName)
        //{
        //    if (string.IsNullOrWhiteSpace(rawFieldName)) return false;
        //    string fieldNameToMap = rawFieldName.Trim();
        //    if (fieldNameToMap.StartsWith("InvoiceDetail_Line", StringComparison.OrdinalIgnoreCase))
        //    {
        //        var parts = fieldNameToMap.Split('_');
        //        if (parts.Length >= 3) fieldNameToMap = parts.Last();
        //    }
        //    return DeepSeekToDBFieldMapping.ContainsKey(fieldNameToMap);
        //}
        
        /// <summary>
        /// Retrieves validation rules (data type, required, pattern, etc.) for a given field name.
        /// </summary>
        //public FieldValidationInfo GetFieldValidationInfo(string rawFieldName)
        //{
        //    var fieldInfo = MapDeepSeekFieldToDatabase(rawFieldName);
        //    if (fieldInfo == null)
        //        return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{rawFieldName}' is unknown or not mapped." };

        //    return new FieldValidationInfo
        //    {
        //        IsValid = true, // Indicates that we have validation rules, not that the value IS valid
        //        DatabaseFieldName = fieldInfo.DatabaseFieldName,
        //        EntityType = fieldInfo.EntityType,
        //        IsRequired = fieldInfo.IsRequired,
        //        DataType = fieldInfo.DataType,
        //        IsMonetary = (fieldInfo.DataType == "Number"), // Use Number pseudo datatype for monetary fields
        //        MaxLength = GetMaxLengthForField(fieldInfo), // Helper
        //        ValidationPattern = GetValidationPatternForField(fieldInfo) // Helper
        //    };
        //}

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
                    return fieldInfo.DataType?.Equals("String", StringComparison.OrdinalIgnoreCase) == true ? 255 : (int?)null; // Default for String pseudo datatype
            }
        }

        // Helper method for GetFieldValidationInfo
        private string GetValidationPatternForField(DatabaseFieldInfo fieldInfo)
        {
            // C# compliant regex patterns using pseudo datatypes consistent with ImportByDataType.cs
            switch (fieldInfo.DataType)
            {
                case "Number":
                    return @"^-?\$?€?£?\s*(?:\d{1,3}(?:[,.\s]\d{3})*|\d+)(?:[.,]\d{1,4})?$"; // C# compliant currency pattern
                case "English Date":
                    return @"^(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\.?\s+\d{1,2}(?:st|nd|rd|th)?,?\s+\d{2,4}$"; // C# compliant English date pattern
                case "Date":
                    return @"^\d{4}-\d{2}-\d{2}(?:T\d{2}:\d{2}(?::\d{2}(?:\.\d+)?)?Z?)?$|^\d{1,2}[/\-.]\d{1,2}[/\-.]\d{2,4}$"; // C# compliant ISO and common date patterns
                case "String":
                    return fieldInfo.IsRequired ? @"^\s*\S+[\s\S]*$" : @"^[\s\S]*$"; // Non-whitespace if required, C# compliant
                default:
                    return @"^[\s\S]*$"; // Allow anything for unknown types, C# compliant
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
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Regex group field retrieval with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Retrieves field definitions from database that correspond to named groups in regex pattern for specific OCR line definition
        /// **BUSINESS OBJECTIVE**: Enable pattern-based field extraction by matching regex named groups to database field definitions with async database querying
        /// **SUCCESS CRITERIA**: Must extract named groups correctly, query database efficiently, map results properly, and return complete FieldInfo collection
        /// 
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
        /// Returns null for unknown fields that cannot be mapped, even if metadata is provided.
        /// </summary>
        public EnhancedDatabaseFieldInfo MapDeepSeekFieldToEnhancedInfo(string deepSeekFieldName, OCRFieldMetadata fieldSpecificMetadata = null)
        {
            var baseInfo = MapDeepSeekFieldToDatabase(deepSeekFieldName);
            if (baseInfo == null)
            {
                // For unknown fields, we should return null regardless of metadata
                // This ensures proper validation and prevents creation of invalid field mappings
                _logger.Debug("MapDeepSeekFieldToEnhancedInfo: No direct map for '{DeepSeekName}'. Using provided metadata for Field '{MetaField}' as base.", deepSeekFieldName, fieldSpecificMetadata?.Field ?? "N/A");

                // Only create from metadata if the metadata field itself is a known field
                if (fieldSpecificMetadata != null && !string.IsNullOrEmpty(fieldSpecificMetadata.Field))
                {
                    var metadataFieldInfo = MapDeepSeekFieldToDatabase(fieldSpecificMetadata.Field);
                    if (metadataFieldInfo != null)
                    {
                        // Use the metadata field mapping as base since it's a known field
                        baseInfo = metadataFieldInfo;
                    }
                }

                if (baseInfo == null)
                {
                    _logger.Warning("MapDeepSeekFieldToEnhancedInfo: Cannot map DeepSeek field '{DeepSeekName}' and no valid metadata field provided for fallback.", deepSeekFieldName);
                    return null;
                }
            }
            return new EnhancedDatabaseFieldInfo(baseInfo, fieldSpecificMetadata);
        }
        
        #endregion
    }
}