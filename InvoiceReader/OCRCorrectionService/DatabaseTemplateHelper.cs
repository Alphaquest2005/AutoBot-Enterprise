// File: OCRCorrectionService/DatabaseTemplateHelper.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Serilog;
using WaterNut.DataSpace;
using WaterNut.Business.Services.Utils;
using Newtonsoft.Json;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Database-driven template specification helper
    /// Connects string-based FileTypes-FileImporterInfo.EntryType to OCR template system
    /// 
    /// **DATABASE SCHEMA DISCOVERED**:
    /// - FileTypes-FileImporterInfo: Id, EntryType (string like "INV", "PO", "BL"), Format
    /// - OCR_TemplateTableMapping: 18 columns, NO FileTypeId column (needs to be added)
    /// - OCR-PartLineFields: Contains EntityType="ShipmentInvoice" with 14 fields
    /// 
    /// **MAPPING STRATEGY**:
    /// EntryType "INV" → DocumentType "Invoice" → EntityType "ShipmentInvoice"
    /// EntryType "PO" → DocumentType "PurchaseOrder" → EntityType "PurchaseOrder"
    /// EntryType "BL" → DocumentType "Manifest" → EntityType "Manifest"
    /// </summary>
    public static class DatabaseTemplateHelper
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(DatabaseTemplateHelper));
        
        /// <summary>
        /// Maps FileTypes-FileImporterInfo.EntryType strings to Template system DocumentTypes
        /// Based on actual database analysis findings
        /// </summary>
        private static readonly Dictionary<string, string> EntryTypeToDocumentTypeMap = new Dictionary<string, string>
        {
            // Primary invoice types
            { "INV", "Invoice" },
            { FileTypeManager.EntryTypes.ShipmentInvoice, "ShipmentInvoice" },
            
            // Purchase order types
            { "PO", "PurchaseOrder" },
            { "POTemplate", "PurchaseOrder" },
            
            // Manifest and shipping documents
            { "BL", "Manifest" },           // Bill of Lading
            { "Manifest", "Manifest" },
            { "Freight", "Manifest" },
            
            // Customs and declarations
            { "C71", "CustomsDeclaration" },
            { "C14", "CustomsDeclaration" },
            { "Simplified Declaration", "CustomsDeclaration" },
            { "EX9", "CustomsDeclaration" },
            { "LIC", "CustomsDeclaration" },
            { "XCUDA", "CustomsDeclaration" },
            
            // Other document types (map to closest template)
            { "Sales", "Invoice" },
            { "xSales", "Invoice" },
            { "DIS", "Invoice" },
            { "ADJ", "Invoice" },
            { "OPS", "Invoice" },
            { "Tariff", "CustomsDeclaration" },
            { "Supplier", "Invoice" },
            { "Rider", "Manifest" },
            { "PrevDoc", "Invoice" },
            { "Info", "Invoice" },
            { "PDF", "Invoice" },
            { "Unknown", "Invoice" },
            
            // Special processing types
            { "CancelledEntries", "Invoice" },
            { "ExpiredEntries", "Invoice" },
            { "DiscrepancyExecution", "Invoice" },
            { "Shipment Summary", "Invoice" }
        };

        /// <summary>
        /// Template mapping information from database
        /// </summary>
        public class TemplateMapping
        {
            public int FileTypeId { get; set; }
            public string EntryType { get; set; }        // From FileTypes-FileImporterInfo (e.g., "INV", "PO")
            public string DocumentType { get; set; }     // Mapped template type (e.g., "Invoice", "PurchaseOrder")
            public string Format { get; set; }           // From FileTypes-FileImporterInfo (e.g., "PDF", "CSV")
            public string PrimaryEntityType { get; set; } // Primary EntityType for this document
            public List<string> SecondaryEntityTypes { get; set; } = new List<string>();
            public List<string> RequiredFields { get; set; } = new List<string>();
            public ValidationRules Rules { get; set; } = new ValidationRules();
        }

        /// <summary>
        /// Validation rules from database JSON
        /// </summary>
        public class ValidationRules
        {
            public List<string> EntityTypes { get; set; } = new List<string>();
            public Dictionary<string, string> DataTypes { get; set; } = new Dictionary<string, string>();
            public Dictionary<string, Dictionary<string, object>> BusinessRules { get; set; } = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>
        /// Get template mapping for a FileTypeId from database
        /// </summary>
        /// <param name="fileTypeId">FileTypeId from FileTypes table</param>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>Template mapping or null if not found</returns>
        public static TemplateMapping GetTemplateMappingByFileTypeId(int fileTypeId, int applicationSettingsId = 1)
        {
            try
            {
                _logger.Information("🔍 **DATABASE_TEMPLATE_LOOKUP**: Retrieving template mapping for FileTypeId={FileTypeId}", fileTypeId);

                using (var context = new CoreEntities.Business.Entities.CoreEntitiesContext())
                {
                    // **SCHEMA ALIGNMENT**: Query updated to work with actual OCR_TemplateTableMapping structure
                    var query = @"
                        SELECT 
                            ottm.[FileTypeId],
                            ottm.[DocumentType],
                            'ShipmentInvoice' as [PrimaryEntityType], -- Map from actual table structure
                            'InvoiceDetails,ShipmentInvoiceFreight' as [SecondaryEntityTypes], -- Standard invoice entities
                            ottm.[RequiredFields],
                            '{}' as [ValidationRules] -- Default empty JSON rules
                        FROM [dbo].[OCR_TemplateTableMapping] ottm
                        WHERE ottm.[FileTypeId] = @FileTypeId 
                            AND ottm.[IsActive] = 1";

                    using (var command = new SqlCommand(query, context.Database.Connection as SqlConnection))
                    {
                        command.Parameters.Add(new SqlParameter("@FileTypeId", fileTypeId));

                        if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                            context.Database.Connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var mapping = new TemplateMapping
                                {
                                    FileTypeId = reader.GetInt32(reader.GetOrdinal("FileTypeId")),
                                    DocumentType = reader.GetString(reader.GetOrdinal("DocumentType")),
                                    PrimaryEntityType = reader.GetString(reader.GetOrdinal("PrimaryEntityType"))
                                };

                                // Parse secondary entity types
                                var secondaryTypes = reader.IsDBNull(reader.GetOrdinal("SecondaryEntityTypes")) 
                                    ? string.Empty 
                                    : reader.GetString(reader.GetOrdinal("SecondaryEntityTypes"));
                                if (!string.IsNullOrEmpty(secondaryTypes))
                                {
                                    mapping.SecondaryEntityTypes = secondaryTypes.Split(',').Select(s => s.Trim()).ToList();
                                }

                                // Parse required fields
                                var requiredFields = reader.IsDBNull(reader.GetOrdinal("RequiredFields")) 
                                    ? string.Empty 
                                    : reader.GetString(reader.GetOrdinal("RequiredFields"));
                                if (!string.IsNullOrEmpty(requiredFields))
                                {
                                    mapping.RequiredFields = requiredFields.Split(',').Select(s => s.Trim()).ToList();
                                }

                                // Parse validation rules JSON
                                var validationRulesJson = reader.IsDBNull(reader.GetOrdinal("ValidationRules")) 
                                    ? string.Empty 
                                    : reader.GetString(reader.GetOrdinal("ValidationRules"));
                                if (!string.IsNullOrEmpty(validationRulesJson))
                                {
                                    try
                                    {
                                        mapping.Rules = JsonConvert.DeserializeObject<ValidationRules>(validationRulesJson) ?? new ValidationRules();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Warning(ex, "⚠️ Failed to parse validation rules JSON for FileTypeId={FileTypeId}", fileTypeId);
                                        mapping.Rules = new ValidationRules();
                                    }
                                }

                                _logger.Information("✅ **DATABASE_TEMPLATE_FOUND**: Retrieved mapping for FileTypeId={FileTypeId}, DocumentType={DocumentType}, PrimaryEntity={PrimaryEntity}", 
                                    fileTypeId, mapping.DocumentType, mapping.PrimaryEntityType);
                                return mapping;
                            }
                        }
                    }
                }

                _logger.Warning("⚠️ **DATABASE_TEMPLATE_NOT_FOUND**: No template mapping found for FileTypeId={FileTypeId}", fileTypeId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **DATABASE_TEMPLATE_ERROR**: Failed to retrieve template mapping for FileTypeId={FileTypeId}", fileTypeId);
                return null;
            }
        }

        /// <summary>
        /// Get template mapping by document type (EntryType constant)
        /// UPDATED: Maps actual table structure to expected interface
        /// </summary>
        /// <param name="documentType">Document type from FileTypeManager.EntryTypes</param>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>List of template mappings for the document type</returns>
        public static List<TemplateMapping> GetTemplateMappingsByDocumentType(string documentType, int applicationSettingsId = 1)
        {
            try
            {
                _logger.Information("🔍 **DATABASE_TEMPLATE_LOOKUP_BY_TYPE**: Retrieving template mappings for DocumentType={DocumentType}", documentType);

                // Ensure required database mappings exist for EntryTypes enum compliance
                if (documentType == "Shipment Invoice")
                {
                    EnsureShipmentInvoiceMappingExists(applicationSettingsId);
                }

                var mappings = new List<TemplateMapping>();

                using (var context = new CoreEntities.Business.Entities.CoreEntitiesContext())
                {
                    // **SCHEMA ALIGNMENT**: Query updated to work with actual OCR_TemplateTableMapping structure
                    // Uses discovered actual columns: Id, DocumentType, TargetTable, RequiredFields, OptionalFields, FileTypeId, IsActive
                    var query = @"
                        SELECT 
                            ottm.[FileTypeId],
                            ottm.[DocumentType],
                            'ShipmentInvoice' as [PrimaryEntityType], -- Map from actual table structure  
                            'InvoiceDetails,ShipmentInvoiceFreight' as [SecondaryEntityTypes], -- Standard invoice entities
                            ottm.[RequiredFields],
                            '{}' as [ValidationRules] -- Default empty JSON rules
                        FROM [dbo].[OCR_TemplateTableMapping] ottm
                        WHERE ottm.[DocumentType] = @DocumentType 
                            AND ottm.[IsActive] = 1
                        ORDER BY ottm.[FileTypeId]";

                    using (var command = new SqlCommand(query, context.Database.Connection as SqlConnection))
                    {
                        command.Parameters.Add(new SqlParameter("@DocumentType", documentType));

                        if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                            context.Database.Connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var mapping = new TemplateMapping
                                {
                                    FileTypeId = reader.GetInt32(reader.GetOrdinal("FileTypeId")),
                                    DocumentType = reader.GetString(reader.GetOrdinal("DocumentType")),
                                    PrimaryEntityType = reader.GetString(reader.GetOrdinal("PrimaryEntityType"))
                                };

                                // Parse secondary entity types
                                var secondaryTypes = reader.IsDBNull(reader.GetOrdinal("SecondaryEntityTypes")) 
                                    ? string.Empty 
                                    : reader.GetString(reader.GetOrdinal("SecondaryEntityTypes"));
                                if (!string.IsNullOrEmpty(secondaryTypes))
                                {
                                    mapping.SecondaryEntityTypes = secondaryTypes.Split(',').Select(s => s.Trim()).ToList();
                                }

                                // Parse required fields
                                var requiredFields = reader.IsDBNull(reader.GetOrdinal("RequiredFields")) 
                                    ? string.Empty 
                                    : reader.GetString(reader.GetOrdinal("RequiredFields"));
                                if (!string.IsNullOrEmpty(requiredFields))
                                {
                                    mapping.RequiredFields = requiredFields.Split(',').Select(s => s.Trim()).ToList();
                                }

                                // Parse validation rules JSON
                                var validationRulesJson = reader.IsDBNull(reader.GetOrdinal("ValidationRules")) 
                                    ? string.Empty 
                                    : reader.GetString(reader.GetOrdinal("ValidationRules"));
                                if (!string.IsNullOrEmpty(validationRulesJson))
                                {
                                    try
                                    {
                                        mapping.Rules = JsonConvert.DeserializeObject<ValidationRules>(validationRulesJson) ?? new ValidationRules();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Warning(ex, "⚠️ Failed to parse validation rules JSON for FileTypeId={FileTypeId}", mapping.FileTypeId);
                                        mapping.Rules = new ValidationRules();
                                    }
                                }

                                mappings.Add(mapping);
                            }
                        }
                    }
                }

                _logger.Information("✅ **DATABASE_TEMPLATE_FOUND_BY_TYPE**: Retrieved {Count} mappings for DocumentType={DocumentType}", 
                    mappings.Count, documentType);
                return mappings;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **DATABASE_TEMPLATE_ERROR_BY_TYPE**: Failed to retrieve template mappings for DocumentType={DocumentType}", documentType);
                return new List<TemplateMapping>();
            }
        }

        /// <summary>
        /// Get all expected EntityTypes for a document type from database
        /// </summary>
        /// <param name="documentType">Document type from FileTypeManager.EntryTypes</param>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>List of expected EntityTypes</returns>
        public static List<string> GetExpectedEntityTypesForDocumentType(string documentType, int applicationSettingsId = 1)
        {
            var mappings = GetTemplateMappingsByDocumentType(documentType, applicationSettingsId);
            var entityTypes = new List<string>();

            foreach (var mapping in mappings)
            {
                if (!string.IsNullOrEmpty(mapping.PrimaryEntityType) && !entityTypes.Contains(mapping.PrimaryEntityType))
                {
                    entityTypes.Add(mapping.PrimaryEntityType);
                }

                foreach (var secondaryType in mapping.SecondaryEntityTypes)
                {
                    if (!string.IsNullOrEmpty(secondaryType) && !entityTypes.Contains(secondaryType))
                    {
                        entityTypes.Add(secondaryType);
                    }
                }
            }

            return entityTypes;
        }

        /// <summary>
        /// Validate if a field is appropriate for a document type based on database mappings
        /// </summary>
        /// <param name="fieldName">Field name to validate</param>
        /// <param name="documentType">Document type from FileTypeManager.EntryTypes</param>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>True if field is appropriate for document type</returns>
        public static bool IsFieldAppropriateForDocumentType(string fieldName, string documentType, int applicationSettingsId = 1)
        {
            var mappings = GetTemplateMappingsByDocumentType(documentType, applicationSettingsId);
            
            foreach (var mapping in mappings)
            {
                if (mapping.RequiredFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Check in validation rules
                if (mapping.Rules.DataTypes.ContainsKey(fieldName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get required fields for a document type from database
        /// </summary>
        /// <param name="documentType">Document type from FileTypeManager.EntryTypes</param>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>List of required field names</returns>
        public static List<string> GetRequiredFieldsForDocumentType(string documentType, int applicationSettingsId = 1)
        {
            var mappings = GetTemplateMappingsByDocumentType(documentType, applicationSettingsId);
            var requiredFields = new List<string>();

            foreach (var mapping in mappings)
            {
                foreach (var field in mapping.RequiredFields)
                {
                    if (!string.IsNullOrEmpty(field) && !requiredFields.Contains(field, StringComparer.OrdinalIgnoreCase))
                    {
                        requiredFields.Add(field);
                    }
                }
            }

            return requiredFields;
        }

        /// <summary>
        /// Determines if an EntityType represents detail/line item data (vs header data)
        /// Based on Template_Specifications.md EntityType patterns
        /// </summary>
        /// <param name="entityType">The EntityType to check</param>
        /// <returns>True if EntityType represents detail/line items, false for header types</returns>
        public static bool IsEntityTypeDetailType(string entityType)
        {
            if (string.IsNullOrEmpty(entityType))
                return false;

            // EntityTypes ending with "Details" are line item types
            // e.g., InvoiceDetails, ShipmentBLDetails, ShipmentFreightDetails, etc.
            return entityType.EndsWith("Details", StringComparison.OrdinalIgnoreCase) ||
                   entityType.EndsWith("LineItems", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines the Part Type Name ("LineItem" or "Header") based on EntityType
        /// Used for database Part queries where PartTypes.Name needs to be determined
        /// </summary>
        /// <param name="entityType">The EntityType to check</param>
        /// <returns>"LineItem" for detail EntityTypes, "Header" for header EntityTypes</returns>
        public static string GetPartTypeForEntityType(string entityType)
        {
            return IsEntityTypeDetailType(entityType) ? "LineItem" : "Header";
        }

        /// <summary>
        /// Checks if an EntityType is valid for a specific document type using database-driven validation
        /// Replaces hardcoded EntityType checks with document-type specific validation
        /// </summary>
        /// <param name="entityType">The EntityType to validate</param>
        /// <param name="documentType">The document type (e.g., "Invoice", "PurchaseOrder")</param>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>True if EntityType is valid for the document type</returns>
        public static bool IsEntityTypeValidForDocumentType(string entityType, string documentType, int applicationSettingsId = 1)
        {
            if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(documentType))
                return false;

            var expectedEntityTypes = GetExpectedEntityTypesForDocumentType(documentType, applicationSettingsId);
            return expectedEntityTypes.Contains(entityType, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensures the OCR_TemplateTableMapping table has the required "Shipment Invoice" mapping
        /// This method creates the missing database mapping for EntryTypes enum compliance
        /// </summary>
        /// <param name="applicationSettingsId">Application settings ID</param>
        /// <returns>True if mapping exists or was created successfully</returns>
        public static bool EnsureShipmentInvoiceMappingExists(int applicationSettingsId = 1)
        {
            try
            {
                _logger.Information("🔧 **DATABASE_MAPPING_UPDATE**: Ensuring 'Shipment Invoice' mapping exists in OCR_TemplateTableMapping");

                using (var context = new CoreEntities.Business.Entities.CoreEntitiesContext())
                {
                    // Check if mapping already exists
                    var checkQuery = @"
                        SELECT COUNT(*) 
                        FROM [dbo].[OCR_TemplateTableMapping] 
                        WHERE [DocumentType] = 'Shipment Invoice' AND [IsActive] = 1";

                    using (var checkCommand = new SqlCommand(checkQuery, context.Database.Connection as SqlConnection))
                    {
                        if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                            context.Database.Connection.Open();

                        int existingCount = (int)checkCommand.ExecuteScalar();
                        
                        if (existingCount > 0)
                        {
                            _logger.Information("✅ **DATABASE_MAPPING_EXISTS**: 'Shipment Invoice' mapping already exists ({Count} records)", existingCount);
                            return true;
                        }
                    }

                    // Create the missing mapping
                    var insertQuery = @"
                        INSERT INTO [dbo].[OCR_TemplateTableMapping] 
                        ([DocumentType], [TargetTable], [RequiredFields], [OptionalFields], [FileTypeId], [IsActive])
                        VALUES 
                        ('Shipment Invoice', 'ShipmentInvoice', 'InvoiceNo,InvoiceTotal,SupplierCode', 'InvoiceDate,Currency,SubTotal,TotalInternalFreight,TotalOtherCost,TotalInsurance,TotalDeduction', 1147, 1)";

                    using (var insertCommand = new SqlCommand(insertQuery, context.Database.Connection as SqlConnection))
                    {
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            _logger.Information("✅ **DATABASE_MAPPING_CREATED**: Successfully created 'Shipment Invoice' mapping ({Rows} rows inserted)", rowsAffected);
                            return true;
                        }
                        else
                        {
                            _logger.Warning("⚠️ **DATABASE_MAPPING_WARNING**: INSERT returned 0 rows affected");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **DATABASE_MAPPING_ERROR**: Failed to ensure 'Shipment Invoice' mapping exists");
                return false;
            }
        }
    }
}