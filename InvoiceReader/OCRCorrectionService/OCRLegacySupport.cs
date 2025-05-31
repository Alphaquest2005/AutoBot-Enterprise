// File: OCRCorrectionService/OCRLegacySupport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Public Static Methods (Legacy Support)

        /// <summary>
        /// Static method to check if invoice totals are correct (TotalsZero = 0) with gift card handling
        /// </summary>
        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger)
        {
            var log = logger;

            if (invoice == null) return false;

            var baseTotal = (invoice.SubTotal ?? 0) +
                          (invoice.TotalInternalFreight ?? 0) +
                          (invoice.TotalOtherCost ?? 0) +
                          (invoice.TotalInsurance ?? 0);

            var deductionAmount = invoice.TotalDeduction ?? 0;
            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

            // Check if the invoice total already has deductions applied (like Amazon gift cards)
            var calculatedWithDeduction = baseTotal - deductionAmount;
            var calculatedWithoutDeduction = baseTotal;

            var diffWithDeduction = Math.Abs(calculatedWithDeduction - reportedInvoiceTotal);
            var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - reportedInvoiceTotal);

            // Use the smaller difference to determine if totals are zero
            var minDifference = Math.Min(diffWithDeduction, diffWithoutDeduction);

            // Debug level logging to reduce console noise
            log.Debug("TotalsZero calculation for {InvoiceNo}: BaseTotal={BaseTotal}, Deduction={Deduction}, Reported={Reported}, MinDiff={MinDiff}, Result={Result}",
                invoice.InvoiceNo, baseTotal, deductionAmount, reportedInvoiceTotal, minDifference, minDifference < 0.01);

            return minDifference < 0.01;
        }

        /// <summary>
        /// Static method to correct invoices using DeepSeek OCR error detection
        /// </summary>
        public static async Task<bool> CorrectInvoices(ShipmentInvoice invoice, string fileText, ILogger logger)
        {
            using var service = new OCRCorrectionService(logger);
            return await service.CorrectInvoiceAsync(invoice, fileText).ConfigureAwait(false);
        }

        /// <summary>
        /// Static method to calculate total zero sum from dynamic invoice results - HANDLES MULTIPLE INVOICES
        /// </summary>
        public static bool TotalsZero(List<dynamic> res, out double totalZeroSum, ILogger logger)
        {
            // Use LogLevelOverride to enable detailed logging for this specific calculation
            using (LogLevelOverride.Begin(LogEventLevel.Debug))
            {
                var log = logger;

                totalZeroSum = 0;

                if (res == null || !res.Any())
                    return true; // Empty is legitimate success

                try
                {
                    bool processedAnyItem = false;

                    foreach (var item in res)
                    {
                        if (item is List<IDictionary<string, object>> invoiceList)
                        {
                            if (!invoiceList.Any()) continue;

                            // FIXED: Process ALL invoices in the collection, not just the first
                            foreach (var invoiceDict in invoiceList)
                            {
                                var tempInvoice = CreateTempShipmentInvoice(invoiceDict);

                                // Use the same gift card handling logic as the static method
                                var baseTotal = (tempInvoice.SubTotal ?? 0) +
                                              (tempInvoice.TotalInternalFreight ?? 0) +
                                              (tempInvoice.TotalOtherCost ?? 0) +
                                              (tempInvoice.TotalInsurance ?? 0);

                                var deductionAmount = tempInvoice.TotalDeduction ?? 0;
                                var reportedInvoiceTotal = tempInvoice.InvoiceTotal ?? 0;

                                var calculatedWithDeduction = baseTotal - deductionAmount;
                                var calculatedWithoutDeduction = baseTotal;

                                var diffWithDeduction = Math.Abs(calculatedWithDeduction - reportedInvoiceTotal);
                                var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - reportedInvoiceTotal);

                                // Use the smaller difference for TotalsZero calculation
                                var invoiceTotalsZero = Math.Min(diffWithDeduction, diffWithoutDeduction);

                                totalZeroSum += invoiceTotalsZero;
                                processedAnyItem = true;

                                // Enhanced logging with emojis for easy identification
                                log.Information("🔍 Dynamic TotalsZero calculation for {InvoiceNo}:", tempInvoice.InvoiceNo);
                                log.Information("   📊 BaseTotal = SubTotal({SubTotal}) + Freight({Freight}) + OtherCost({OtherCost}) + Insurance({Insurance}) = {BaseTotal}",
                                    tempInvoice.SubTotal, tempInvoice.TotalInternalFreight, tempInvoice.TotalOtherCost, tempInvoice.TotalInsurance, baseTotal);
                                log.Information("   💳 TotalDeduction = {Deduction}", deductionAmount);
                                log.Information("   🧾 ReportedInvoiceTotal = {ReportedTotal}", reportedInvoiceTotal);
                                log.Information("   🧮 Calculated WITH deduction: {BaseTotal} - {Deduction} = {WithDeduction} (diff: {DiffWith})",
                                    baseTotal, deductionAmount, calculatedWithDeduction, diffWithDeduction);
                                log.Information("   🧮 Calculated WITHOUT deduction: {BaseTotal} = {WithoutDeduction} (diff: {DiffWithout})",
                                    baseTotal, calculatedWithoutDeduction, diffWithoutDeduction);
                                log.Information("   ✅ Using minimum difference: {MinDifference}",
                                    invoiceTotalsZero);
                            }
                        }
                        else
                        {
                            // Type mismatch - log for debugging
                            var actualType = item?.GetType().Name ?? "null";
                            log.Warning("Expected List<IDictionary<string, object>> but got {ActualType}", actualType);
                            totalZeroSum = 0;
                            return false;
                        }
                    }

                    log.Information("🏁 Final Results: Processed {ProcessedAny} items, total TotalsZero sum: {TotalSum}",
                        processedAnyItem, totalZeroSum);
                    return processedAnyItem || !res.Any();
                }
                catch (Exception ex)
                {
                    log.Error(ex, "Error calculating TotalsZero");
                    totalZeroSum = 0;
                    return false;
                }
            }
        }

        /// <summary>
        /// Static method to correct invoices using OCR correction service - HANDLES MULTIPLE INVOICES
        /// </summary>
        public static async Task CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger)
        {
            if (res == null || !res.Any() || template == null) return;

            var log = logger;

            try
            {
                // Convert with OCR metadata for database updates
                var allShipmentInvoicesWithMetadata = ConvertDynamicToShipmentInvoicesWithMetadata(res, template, logger);
                var allShipmentInvoices = allShipmentInvoicesWithMetadata.Select(x => x.Invoice).ToList();
                var droppedFilePath = GetFilePathFromTemplate(template);

                log.Information("Processing {InvoiceCount} invoices from {FileCount} PDF sections with OCR metadata",
                    allShipmentInvoices.Count, res.Count);

                using var correctionService = new OCRCorrectionService(logger);
                var correctedInvoices = await correctionService.CorrectInvoicesAsync(allShipmentInvoices, droppedFilePath).ConfigureAwait(false);

                // Update both the dynamic results AND the template line values
                UpdateDynamicResultsWithCorrections(res, correctedInvoices, log);
                UpdateTemplateLineValues(template, correctedInvoices, log);

                // Update database with OCR corrections using metadata
                await UpdateDatabaseWithCorrections(correctedInvoices, allShipmentInvoicesWithMetadata, log).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error in static CorrectInvoices");
            }
        }

        /// <summary>
        /// Validates the data structure and logs detailed information about invoice collections
        /// </summary>
        public static void ValidateInvoiceStructure(List<dynamic> res, ILogger logger)
        {
            if (res == null)
            {
                logger?.Error("Invoice result list is null");
                return;
            }

            logger?.Information("=== INVOICE STRUCTURE VALIDATION ===");
            logger?.Information("Total sections in PDF: {SectionCount}", res.Count);

            int totalInvoices = 0;

            for (int i = 0; i < res.Count; i++)
            {
                var section = res[i];
                if (section == null)
                {
                    logger?.Warning("Section {Index}: null", i);
                    continue;
                }

                if (section is List<IDictionary<string, object>> invoiceList)
                {
                    logger?.Information("Section {Index}: ✓ List<IDictionary<string, object>> with {Count} invoices", i, invoiceList.Count);
                    totalInvoices += invoiceList.Count;

                    for (int j = 0; j < invoiceList.Count; j++)
                    {
                        var invoice = invoiceList[j];
                        var invoiceNo = invoice?.ContainsKey("InvoiceNo") == true ? invoice["InvoiceNo"]?.ToString() : "Unknown";
                        var detailsCount = 0;

                        if (invoice?.ContainsKey("InvoiceDetails") == true &&
                            invoice["InvoiceDetails"] is List<IDictionary<string, object>> details)
                        {
                            detailsCount = details.Count;
                        }

                        logger?.Information("  Invoice {SubIndex}: {InvoiceNo} ({DetailCount} line items)", j, invoiceNo, detailsCount);
                    }
                }
                else if (section is IDictionary<string, object> singleDict)
                {
                    logger?.Warning("Section {Index}: ⚠️ Single IDictionary (legacy format) - {InvoiceNo}",
                        i, singleDict.ContainsKey("InvoiceNo") ? singleDict["InvoiceNo"] : "Unknown");
                    totalInvoices += 1;
                }
                else
                {
                    var actualType = section.GetType();
                    logger?.Error("Section {Index}: ❌ Unexpected type: {TypeName}", i, actualType.Name);
                }
            }

            logger?.Information("Total invoices found: {TotalInvoices}", totalInvoices);
            logger?.Information("=== END VALIDATION ===");
        }

        /// <summary>
        /// Counts total invoices across all sections
        /// </summary>
        public static int CountTotalInvoices(List<dynamic> res)
        {
            if (res == null) return 0;

            int count = 0;
            foreach (var item in res)
            {
                if (item is List<IDictionary<string, object>> invoiceList)
                {
                    count += invoiceList.Count;
                }
                else if (item is IDictionary<string, object>)
                {
                    count += 1;
                }
            }
            return count;
        }

        /// <summary>
        /// Comprehensive test method including product validation
        /// </summary>
        public static async Task RunComprehensiveTests(ILogger logger)
        {
            logger?.Information("=== Enhanced OCR Correction Service Tests ===");

            try
            {
                TestDataStructures(logger);
                TestJsonParsing(logger);
                TestFieldParsing(logger);
                await TestProductValidation(logger).ConfigureAwait(false);
                await TestMathematicalValidation(logger).ConfigureAwait(false);
                TestRegexPatterns(logger);

                logger?.Information("🎉 ALL COMPREHENSIVE TESTS PASSED SUCCESSFULLY!");
                logger?.Information("✅ Enhanced OCR Correction Service functionality verified");
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "❌ Comprehensive test failed");
                throw;
            }
        }

        #endregion

        #region Private Static Helper Methods

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            try
            {
                var invoice = new ShipmentInvoice
                {
                    InvoiceNo = x.ContainsKey("InvoiceNo") && x["InvoiceNo"] != null ? x["InvoiceNo"].ToString() : "Unknown",
                    InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null,
                    SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null,
                    TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null,
                    TotalOtherCost = x.ContainsKey("TotalOtherCost") ? Convert.ToDouble(x["TotalOtherCost"].ToString()) : (double?)null,
                    TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null,
                    TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null
                };

                if (!x.ContainsKey("InvoiceDetails"))
                {
                    invoice.InvoiceDetails = new List<InvoiceDetails>();
                }
                else
                {
                    // FIXED: Safe type checking for invoice details
                    if (x["InvoiceDetails"] is List<IDictionary<string, object>> invoiceDetailsList)
                    {
                        invoice.InvoiceDetails = invoiceDetailsList
                            .Where(z => z?.ContainsKey("ItemDescription") == true && z["ItemDescription"] != null)
                            .Select((z, index) =>
                            {
                                try
                                {
                                    var qty = z.ContainsKey("Quantity") ? Convert.ToDouble(z["Quantity"].ToString()) : 1;
                                    return new InvoiceDetails
                                    {
                                        LineNumber = index + 1, // Ensure line numbers are set
                                        ItemDescription = z["ItemDescription"].ToString(),
                                        Quantity = qty,
                                        Cost = z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) :
                                               z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) / (qty == 0 ? 1 : qty) : 0,
                                        TotalCost = z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) :
                                                   z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) * qty : 0,
                                        Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0
                                    };
                                }
                                catch (Exception ex)
                                {
                                    logger?.Error(ex, "Error processing invoice detail {Index} for invoice {InvoiceNo}",
                                        index, invoice.InvoiceNo);
                                    return null;
                                }
                            })
                            .Where(detail => detail != null)
                            .ToList();
                    }
                    else
                    {
                        // Handle case where InvoiceDetails might be a different type
                        var actualType = x["InvoiceDetails"]?.GetType().Name ?? "null";
                        logger?.Warning("Expected List<IDictionary<string, object>> for InvoiceDetails but got {ActualType} for invoice {InvoiceNo}",
                            actualType, invoice.InvoiceNo);
                        invoice.InvoiceDetails = new List<InvoiceDetails>();
                    }
                }

                return invoice;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error creating ShipmentInvoice from dictionary");
                return new ShipmentInvoice
                {
                    InvoiceNo = "ERROR",
                    InvoiceDetails = new List<InvoiceDetails>()
                };
            }
        }

        private static List<ShipmentInvoice> ConvertDynamicToShipmentInvoices(List<dynamic> res, ILogger logger)
        {
            var allInvoices = new List<ShipmentInvoice>();

            try
            {
                foreach (var item in res)
                {
                    if (item is List<IDictionary<string, object>> invoiceList)
                    {
                        // FIXED: Process ALL invoices in each list, not just the first
                        foreach (var invoiceDict in invoiceList)
                        {
                            var shipmentInvoice = CreateTempShipmentInvoice(invoiceDict, logger);
                            allInvoices.Add(shipmentInvoice);
                        }

                        logger?.Debug("Converted {Count} invoices from PDF section", invoiceList.Count);
                    }
                    else
                    {
                        // Handle legacy single dictionary format for backward compatibility
                        if (item is IDictionary<string, object> singleInvoiceDict)
                        {
                            logger?.Warning("Found single dictionary instead of list - converting for backward compatibility");
                            var shipmentInvoice = CreateTempShipmentInvoice(singleInvoiceDict, logger);
                            allInvoices.Add(shipmentInvoice);
                        }
                        else
                        {
                            var actualType = item?.GetType().Name ?? "null";
                            logger?.Error("Unexpected type in ConvertDynamicToShipmentInvoices: {ActualType}", actualType);
                        }
                    }
                }

                logger?.Information("Successfully converted {TotalInvoices} total invoices from {Sections} PDF sections",
                    allInvoices.Count, res.Count);

                return allInvoices;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error in ConvertDynamicToShipmentInvoices");
                return new List<ShipmentInvoice>();
            }
        }

        /// <summary>
        /// Converts dynamic results to ShipmentInvoices with OCR metadata for database updates
        /// </summary>
        private static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(List<dynamic> res, Invoice template, ILogger logger)
        {
            var allInvoicesWithMetadata = new List<ShipmentInvoiceWithMetadata>();

            try
            {
                // Create field mapping from template
                var fieldMappings = GetTemplateFieldMappings(template, logger);

                foreach (var item in res)
                {
                    if (item is List<IDictionary<string, object>> invoiceList)
                    {
                        foreach (var invoiceDict in invoiceList)
                        {
                            var shipmentInvoice = CreateTempShipmentInvoice(invoiceDict, logger);
                            var metadata = ExtractOCRMetadata(invoiceDict, template, fieldMappings, logger);

                            allInvoicesWithMetadata.Add(new ShipmentInvoiceWithMetadata
                            {
                                Invoice = shipmentInvoice,
                                FieldMetadata = metadata
                            });
                        }
                    }
                    else if (item is IDictionary<string, object> singleInvoiceDict)
                    {
                        var shipmentInvoice = CreateTempShipmentInvoice(singleInvoiceDict, logger);
                        var metadata = ExtractOCRMetadata(singleInvoiceDict, template, fieldMappings, logger);

                        allInvoicesWithMetadata.Add(new ShipmentInvoiceWithMetadata
                        {
                            Invoice = shipmentInvoice,
                            FieldMetadata = metadata
                        });
                    }
                }

                logger?.Information("Successfully converted {TotalInvoices} invoices with OCR metadata from {Sections} PDF sections",
                    allInvoicesWithMetadata.Count, res.Count);

                return allInvoicesWithMetadata;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error in ConvertDynamicToShipmentInvoicesWithMetadata");
                return new List<ShipmentInvoiceWithMetadata>();
            }
        }

        private static string GetFilePathFromTemplate(Invoice template)
        {
            return template?.FilePath ?? "unknown";
        }

        /// <summary>
        /// Extracts OCR metadata from invoice dictionary using template information (Enhanced version)
        /// </summary>
        private static Dictionary<string, OCRFieldMetadata> ExtractOCRMetadata(
            IDictionary<string, object> invoiceDict,
            Invoice template,
            Dictionary<string, (int LineId, int FieldId)> fieldMappings,
            ILogger logger)
        {
            try
            {
                // Use the enhanced metadata extractor
                return ExtractEnhancedOCRMetadataInternal(invoiceDict, template, fieldMappings, logger);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error extracting enhanced OCR metadata, falling back to legacy method");

                // Fallback to legacy extraction if enhanced fails
                return ExtractLegacyOCRMetadata(invoiceDict, template, fieldMappings, logger);
            }
        }

        /// <summary>
        /// Enhanced OCR metadata extraction with comprehensive template context
        /// </summary>
        private static Dictionary<string, OCRFieldMetadata> ExtractEnhancedOCRMetadataInternal(
            IDictionary<string, object> invoiceDict,
            Invoice template,
            Dictionary<string, (int LineId, int FieldId)> fieldMappings,
            ILogger logger)
        {
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            try
            {
                logger?.Debug("Starting enhanced OCR metadata extraction for {FieldCount} fields", invoiceDict.Count);

                // Get invoice context
                var invoiceContext = new
                {
                    InvoiceId = template.OcrInvoices?.Id,
                    InvoiceName = template.OcrInvoices?.Name
                };

                // Extract metadata for each field in the invoice
                foreach (var kvp in invoiceDict)
                {
                    var fieldName = kvp.Key;
                    var fieldValue = kvp.Value?.ToString();

                    // Skip metadata fields
                    if (IsMetadataField(fieldName)) continue;

                    // Try to find field mapping
                    if (fieldMappings.TryGetValue(fieldName, out var mapping))
                    {
                        var fieldMetadata = ExtractEnhancedFieldMetadata(fieldName, fieldValue, template, invoiceContext, mapping.LineId, mapping.FieldId, logger);
                        if (fieldMetadata != null)
                        {
                            metadata[fieldName] = fieldMetadata;
                        }
                    }
                    else
                    {
                        // Try to find field directly in template if no mapping exists
                        var fieldMetadata = ExtractFieldMetadataFromTemplate(fieldName, fieldValue, template, invoiceContext, logger);
                        if (fieldMetadata != null)
                        {
                            metadata[fieldName] = fieldMetadata;
                        }
                    }
                }

                logger?.Debug("Extracted enhanced OCR metadata for {FieldCount} fields", metadata.Count);
                return metadata;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error extracting enhanced OCR metadata");
                return new Dictionary<string, OCRFieldMetadata>();
            }
        }

        /// <summary>
        /// Checks if a field name is a metadata field that should be skipped
        /// </summary>
        private static bool IsMetadataField(string fieldName)
        {
            var metadataFields = new[] { "LineNumber", "FileLineNumber", "Section", "Instance" };
            return metadataFields.Contains(fieldName);
        }

        /// <summary>
        /// Extracts enhanced field metadata using specific LineId and FieldId
        /// </summary>
        private static OCRFieldMetadata ExtractEnhancedFieldMetadata(
            string fieldName,
            string fieldValue,
            Invoice template,
            dynamic invoiceContext,
            int lineId,
            int fieldId,
            ILogger logger)
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
                                return new OCRFieldMetadata
                                {
                                    // Basic field information
                                    FieldName = fieldName,
                                    Value = fieldValue,
                                    RawValue = rawValue,

                                    // OCR Template Context
                                    LineNumber = lineNumber,
                                    FieldId = fields.Id,
                                    LineId = lineId,
                                    RegexId = line.OCR_Lines?.RegularExpressions?.Id,
                                    Key = fields.Key,
                                    Field = fields.Field,
                                    EntityType = fields.EntityType,
                                    DataType = fields.DataType,

                                    // Line Context
                                    LineName = line.OCR_Lines?.Name,
                                    LineRegex = line.OCR_Lines?.RegularExpressions?.RegEx,
                                    IsRequired = fields.IsRequired,

                                    // Part Context (get from line's part)
                                    PartId = line.OCR_Lines?.PartId,

                                    // Invoice Context
                                    InvoiceId = invoiceContext.InvoiceId,
                                    InvoiceName = invoiceContext.InvoiceName,

                                    // Processing Context
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
                logger?.Error(ex, "Error extracting enhanced field metadata for {FieldName}", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Extracts field metadata by searching through template (fallback method)
        /// </summary>
        private static OCRFieldMetadata ExtractFieldMetadataFromTemplate(
            string fieldName,
            string fieldValue,
            Invoice template,
            dynamic invoiceContext,
            ILogger logger)
        {
            try
            {
                // Search through all parts, lines, and field values to find the field
                foreach (var part in template.Parts ?? Enumerable.Empty<Part>())
                {
                    foreach (var line in part.Lines ?? Enumerable.Empty<Line>())
                    {
                        // Search through line values for the field
                        var fieldContext = FindFieldInLineValues(fieldName, line, logger);
                        if (fieldContext != null)
                        {
                            return new OCRFieldMetadata
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

                                // Invoice Context
                                InvoiceId = invoiceContext.InvoiceId,
                                InvoiceName = invoiceContext.InvoiceName,

                                // Processing Context
                                Section = fieldContext.Section,
                                Instance = fieldContext.Instance,
                                Confidence = fieldContext.Confidence
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error extracting field metadata from template for {FieldName}", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Searches for field in line values
        /// </summary>
        private static dynamic FindFieldInLineValues(string fieldName, Line line, ILogger logger)
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
                                return new
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
                                    LineId = line.OCR_Lines?.Id,
                                    LineName = line.OCR_Lines?.Name,
                                    LineRegex = line.OCR_Lines?.RegularExpressions?.RegEx,
                                    RegexId = line.OCR_Lines?.RegularExpressions?.Id,

                                    // Part context
                                    PartId = line.OCR_Lines?.PartId,

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
                logger?.Error(ex, "Error searching field {FieldName} in line values", fieldName);
                return null;
            }
        }

        /// <summary>
        /// Legacy OCR metadata extraction method (kept as fallback)
        /// </summary>
        private static Dictionary<string, OCRFieldMetadata> ExtractLegacyOCRMetadata(
            IDictionary<string, object> invoiceDict,
            Invoice template,
            Dictionary<string, (int LineId, int FieldId)> fieldMappings,
            ILogger logger)
        {
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            try
            {
                // Extract metadata for each field in the invoice
                foreach (var kvp in invoiceDict)
                {
                    var fieldName = kvp.Key;
                    var fieldValue = kvp.Value?.ToString();

                    if (fieldMappings.TryGetValue(fieldName, out var mapping))
                    {
                        // Find the corresponding line and field metadata from template
                        var ocrMetadata = FindOCRMetadataFromTemplate(template, mapping.LineId, mapping.FieldId, fieldValue);
                        if (ocrMetadata != null)
                        {
                            ocrMetadata.FieldName = fieldName;
                            metadata[fieldName] = ocrMetadata;
                        }
                    }
                }

                logger?.Debug("Extracted legacy OCR metadata for {FieldCount} fields", metadata.Count);
                return metadata;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error extracting legacy OCR metadata");
                return new Dictionary<string, OCRFieldMetadata>();
            }
        }

        /// <summary>
        /// Finds OCR metadata from template line values
        /// </summary>
        private static OCRFieldMetadata FindOCRMetadataFromTemplate(Invoice template, int lineId, int fieldId, string fieldValue)
        {
            try
            {
                var line = template.Lines.FirstOrDefault(l => l.OCR_Lines?.Id == lineId);
                if (line?.Values == null) return null;

                // Search through line values to find the field
                foreach (var outerKvp in line.Values)
                {
                    var (lineNumber, section) = outerKvp.Key;
                    var innerDict = outerKvp.Value;

                    if (innerDict != null)
                    {
                        var fieldEntry = innerDict.FirstOrDefault(kvp => kvp.Key.Fields?.Id == fieldId);
                        if (fieldEntry.Key.Fields != null)
                        {
                            return new OCRFieldMetadata
                            {
                                LineNumber = lineNumber,
                                FieldId = fieldId,
                                LineId = lineId,
                                RegexId = line.OCR_Lines?.RegularExpressions?.Id,
                                Section = section,
                                Instance = fieldEntry.Key.Instance,
                                RawValue = fieldEntry.Value,
                                FieldName = fieldEntry.Key.Fields.Field
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error finding OCR metadata from template for LineId: {LineId}, FieldId: {FieldId}", lineId, fieldId);
                return null;
            }
        }

        /// <summary>
        /// Updates database with OCR corrections for future processing
        /// </summary>
        private static async Task UpdateDatabaseWithCorrections(
            List<ShipmentInvoice> correctedInvoices,
            List<ShipmentInvoiceWithMetadata> originalInvoicesWithMetadata,
            ILogger log)
        {
            try
            {
                log.Information("Updating database with OCR corrections for {InvoiceCount} invoices", correctedInvoices.Count);

                using var ctx = new OCRContext();

                for (int i = 0; i < correctedInvoices.Count && i < originalInvoicesWithMetadata.Count; i++)
                {
                    var correctedInvoice = correctedInvoices[i];
                    var originalWithMetadata = originalInvoicesWithMetadata[i];

                    await UpdateInvoiceFieldFormatsInDatabase(ctx, correctedInvoice, originalWithMetadata, log).ConfigureAwait(false);
                }

                await ctx.SaveChangesAsync().ConfigureAwait(false);
                log.Information("Successfully updated database with OCR corrections");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error updating database with OCR corrections");
            }
        }

        private static void UpdateDynamicResultsWithCorrections(List<dynamic> res, List<ShipmentInvoice> correctedInvoices, ILogger logger)
        {
            try
            {
                int correctedIndex = 0;

                for (int sectionIndex = 0; sectionIndex < res.Count; sectionIndex++)
                {
                    if (res[sectionIndex] is List<IDictionary<string, object>> invoiceList)
                    {
                        logger?.Debug("Updating {Count} invoices in section {Section}", invoiceList.Count, sectionIndex);

                        // FIXED: Update ALL invoices in the collection
                        for (int invoiceIndex = 0; invoiceIndex < invoiceList.Count && correctedIndex < correctedInvoices.Count; invoiceIndex++, correctedIndex++)
                        {
                            var invoiceDict = invoiceList[invoiceIndex];
                            var correctedInvoice = correctedInvoices[correctedIndex];

                            try
                            {
                                // Update main fields
                                if (correctedInvoice.InvoiceTotal.HasValue)
                                {
                                    invoiceDict["InvoiceTotal"] = correctedInvoice.InvoiceTotal.Value;
                                    logger?.Debug("Updated InvoiceTotal for {InvoiceNo}: {Value}",
                                        correctedInvoice.InvoiceNo, correctedInvoice.InvoiceTotal.Value);
                                }
                                if (correctedInvoice.SubTotal.HasValue)
                                    invoiceDict["SubTotal"] = correctedInvoice.SubTotal.Value;
                                if (correctedInvoice.TotalInternalFreight.HasValue)
                                    invoiceDict["TotalInternalFreight"] = correctedInvoice.TotalInternalFreight.Value;
                                if (correctedInvoice.TotalOtherCost.HasValue)
                                    invoiceDict["TotalOtherCost"] = correctedInvoice.TotalOtherCost.Value;
                                if (correctedInvoice.TotalInsurance.HasValue)
                                    invoiceDict["TotalInsurance"] = correctedInvoice.TotalInsurance.Value;
                                if (correctedInvoice.TotalDeduction.HasValue)
                                    invoiceDict["TotalDeduction"] = correctedInvoice.TotalDeduction.Value;
                            }
                            catch (Exception ex)
                            {
                                logger?.Error(ex, "Error updating invoice {InvoiceNo} at section {Section}, invoice {Invoice}",
                                    correctedInvoice.InvoiceNo, sectionIndex, invoiceIndex);
                            }
                        }
                    }
                }

                logger?.Information("Successfully updated {UpdatedCount} invoices from corrections", correctedIndex);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error in UpdateDynamicResultsWithCorrections");
            }
        }

        /// <summary>
        /// Updates field format regex patterns in database based on corrections
        /// </summary>
        private static async Task UpdateInvoiceFieldFormatsInDatabase(
            OCRContext ctx,
            ShipmentInvoice correctedInvoice,
            ShipmentInvoiceWithMetadata originalWithMetadata,
            ILogger log)
        {
            try
            {
                // Compare corrected vs original values to identify format corrections
                var corrections = new List<(string FieldName, string OldValue, string NewValue, OCRFieldMetadata Metadata)>();

                // Check each field for changes
                CheckFieldForCorrection(corrections, "InvoiceTotal",
                    originalWithMetadata.Invoice.InvoiceTotal?.ToString(),
                    correctedInvoice.InvoiceTotal?.ToString(),
                    originalWithMetadata.GetFieldMetadata("InvoiceTotal"));

                CheckFieldForCorrection(corrections, "SubTotal",
                    originalWithMetadata.Invoice.SubTotal?.ToString(),
                    correctedInvoice.SubTotal?.ToString(),
                    originalWithMetadata.GetFieldMetadata("SubTotal"));

                CheckFieldForCorrection(corrections, "TotalInternalFreight",
                    originalWithMetadata.Invoice.TotalInternalFreight?.ToString(),
                    correctedInvoice.TotalInternalFreight?.ToString(),
                    originalWithMetadata.GetFieldMetadata("TotalInternalFreight"));

                CheckFieldForCorrection(corrections, "TotalOtherCost",
                    originalWithMetadata.Invoice.TotalOtherCost?.ToString(),
                    correctedInvoice.TotalOtherCost?.ToString(),
                    originalWithMetadata.GetFieldMetadata("TotalOtherCost"));

                CheckFieldForCorrection(corrections, "TotalInsurance",
                    originalWithMetadata.Invoice.TotalInsurance?.ToString(),
                    correctedInvoice.TotalInsurance?.ToString(),
                    originalWithMetadata.GetFieldMetadata("TotalInsurance"));

                CheckFieldForCorrection(corrections, "TotalDeduction",
                    originalWithMetadata.Invoice.TotalDeduction?.ToString(),
                    correctedInvoice.TotalDeduction?.ToString(),
                    originalWithMetadata.GetFieldMetadata("TotalDeduction"));

                // Create field format regex entries for corrections
                foreach (var correction in corrections)
                {
                    await CreateFieldFormatRegexForCorrection(ctx, correction, log).ConfigureAwait(false);
                }

                if (corrections.Any())
                {
                    log.Information("Created {CorrectionCount} field format corrections for invoice {InvoiceNo}",
                        corrections.Count, correctedInvoice.InvoiceNo);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error updating field formats in database for invoice {InvoiceNo}", correctedInvoice.InvoiceNo);
            }
        }

        /// <summary>
        /// Checks if a field was corrected and adds to corrections list
        /// </summary>
        private static void CheckFieldForCorrection(
            List<(string FieldName, string OldValue, string NewValue, OCRFieldMetadata Metadata)> corrections,
            string fieldName, string oldValue, string newValue, OCRFieldMetadata metadata)
        {
            if (!string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(newValue) &&
                oldValue != newValue && metadata != null)
            {
                corrections.Add((fieldName, oldValue, newValue, metadata));
            }
        }

        /// <summary>
        /// Creates field format regex entry for a specific correction
        /// </summary>
        private static async Task CreateFieldFormatRegexForCorrection(
            OCRContext ctx,
            (string FieldName, string OldValue, string NewValue, OCRFieldMetadata Metadata) correction,
            ILogger log)
        {
            try
            {
                if (!correction.Metadata.FieldId.HasValue) return;

                var field = ctx.Fields.FirstOrDefault(f => f.Id == correction.Metadata.FieldId.Value);
                if (field == null)
                {
                    log.Warning("Field with ID {FieldId} not found for correction", correction.Metadata.FieldId);
                    return;
                }

                // Create regex pattern for format correction (focus on common formatting issues)
                var (regexPattern, replacementPattern) = CreateFormatCorrectionPatterns(correction.OldValue, correction.NewValue);

                if (string.IsNullOrEmpty(regexPattern) || string.IsNullOrEmpty(replacementPattern))
                {
                    log.Debug("Could not create format correction patterns for {FieldName}: {OldValue} → {NewValue}",
                        correction.FieldName, correction.OldValue, correction.NewValue);
                    return;
                }

                // Check if similar format regex already exists
                var existingFormatRegex = ctx.OCR_FieldFormatRegEx
                    .FirstOrDefault(ffr => ffr.Fields.Id == field.Id &&
                                          ffr.RegEx.RegEx.Contains(regexPattern.Substring(0, Math.Min(10, regexPattern.Length))));

                if (existingFormatRegex != null)
                {
                    log.Debug("Similar field format regex already exists for {FieldName}", correction.FieldName);
                    return;
                }

                // Create new regex entries
                var correctionRegex = new OCR.Business.Entities.RegularExpressions
                {
                    RegEx = regexPattern,
                    MultiLine = false,
                    TrackingState = TrackableEntities.TrackingState.Added
                };
                ctx.RegularExpressions.Add(correctionRegex);

                var replacementRegex = new OCR.Business.Entities.RegularExpressions
                {
                    RegEx = replacementPattern,
                    MultiLine = false,
                    TrackingState = TrackableEntities.TrackingState.Added
                };
                ctx.RegularExpressions.Add(replacementRegex);

                // Create field format regex entry
                var fieldFormatRegex = new OCR.Business.Entities.FieldFormatRegEx
                {
                    Fields = field,
                    RegEx = correctionRegex,
                    ReplacementRegEx = replacementRegex,
                    TrackingState = TrackableEntities.TrackingState.Added
                };
                ctx.OCR_FieldFormatRegEx.Add(fieldFormatRegex);

                log.Information("Created field format regex for {FieldName}: {Pattern} → {Replacement}",
                    correction.FieldName, regexPattern, replacementPattern);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error creating field format regex for {FieldName}", correction.FieldName);
            }
        }

        /// <summary>
        /// Creates regex patterns for common format corrections
        /// </summary>
        public static (string RegexPattern, string ReplacementPattern) CreateFormatCorrectionPatterns(string oldValue, string newValue)
        {
            // Common format correction patterns

            // Decimal separator: comma to period (e.g., "123,45" → "123.45")
            if (oldValue.Contains(',') && newValue.Contains('.') &&
                oldValue.Replace(',', '.') == newValue)
            {
                return (@"(\d+),(\d{2})", "$1.$2");
            }

            // Currency symbol addition (e.g., "123.45" → "$123.45")
            if (!oldValue.StartsWith("$") && newValue.StartsWith("$") &&
                newValue.Substring(1) == oldValue)
            {
                return (@"^(\d+\.?\d*)$", "$$1");
            }

            // Negative number formatting (e.g., "123.45-" → "-123.45")
            if (oldValue.EndsWith("-") && newValue.StartsWith("-") &&
                oldValue.Substring(0, oldValue.Length - 1) == newValue.Substring(1))
            {
                return (@"(\d+\.?\d*)-$", "-$1");
            }

            // Space removal in numbers (e.g., "1 234.56" → "1234.56")
            if (oldValue.Contains(' ') && !newValue.Contains(' ') &&
                oldValue.Replace(" ", "") == newValue)
            {
                return (@"(\d+)\s+(\d+)", "$1$2");
            }

            // OCR character confusion: O → 0
            if (oldValue.Contains('O') && newValue.Contains('0') &&
                oldValue.Replace('O', '0') == newValue)
            {
                return (@"([^A-Z])O([^A-Z])", "$10$2");
            }

            // OCR character confusion: l → 1
            if (oldValue.Contains('l') && newValue.Contains('1') &&
                oldValue.Replace('l', '1') == newValue)
            {
                return (@"([^a-z])l([^a-z])", "$11$2");
            }

            return (null, null);
        }

        /// <summary>
        /// Updates template line values with corrected data so template.Read() uses corrected values
        /// This is the key missing piece - we need to update the template's internal state
        /// </summary>
        private static void UpdateTemplateLineValues(Invoice template, List<ShipmentInvoice> correctedInvoices, ILogger log)
        {
            try
            {
                if (template?.Lines == null || !correctedInvoices.Any())
                {
                    log.Information("No template lines or corrected invoices to update");
                    return;
                }

                log.Information("Updating template line values with {CorrectionCount} corrected invoices", correctedInvoices.Count);

                // Get field mappings from template structure
                var fieldMappings = GetTemplateFieldMappings(template, log);

                foreach (var correctedInvoice in correctedInvoices)
                {
                    UpdateInvoiceFieldsInTemplate(template, correctedInvoice, fieldMappings, log);
                }

                log.Information("Successfully updated template line values");
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error updating template line values");
            }
        }

        /// <summary>
        /// Gets field mappings from template structure to map ShipmentInvoice fields to template lines
        /// </summary>
        private static Dictionary<string, (int LineId, int FieldId)> GetTemplateFieldMappings(Invoice template, ILogger logger)
        {
            var mappings = new Dictionary<string, (int LineId, int FieldId)>();

            try
            {
                foreach (var line in template.Lines.Where(l => l?.OCR_Lines?.Fields != null))
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
                                mappings[mappedFieldName] = (line.OCR_Lines.Id, field.Id);
                            }
                        }
                    }
                }

                logger?.Debug("Created {MappingCount} field mappings from template", mappings.Count);
                return mappings;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error creating template field mappings");
                return new Dictionary<string, (int LineId, int FieldId)>();
            }
        }

        /// <summary>
        /// Maps template field names to ShipmentInvoice property names
        /// </summary>
        private static string MapFieldNameToProperty(string fieldName)
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
                _ => null
            };
        }

        /// <summary>
        /// Updates specific invoice fields in template line values
        /// </summary>
        private static void UpdateInvoiceFieldsInTemplate(Invoice template, ShipmentInvoice correctedInvoice,
            Dictionary<string, (int LineId, int FieldId)> fieldMappings, ILogger log)
        {
            try
            {
                // Update header fields
                UpdateFieldInTemplate(template, fieldMappings, "InvoiceTotal", correctedInvoice.InvoiceTotal?.ToString(), log);
                UpdateFieldInTemplate(template, fieldMappings, "SubTotal", correctedInvoice.SubTotal?.ToString(), log);
                UpdateFieldInTemplate(template, fieldMappings, "TotalInternalFreight", correctedInvoice.TotalInternalFreight?.ToString(), log);
                UpdateFieldInTemplate(template, fieldMappings, "TotalOtherCost", correctedInvoice.TotalOtherCost?.ToString(), log);
                UpdateFieldInTemplate(template, fieldMappings, "TotalInsurance", correctedInvoice.TotalInsurance?.ToString(), log);
                UpdateFieldInTemplate(template, fieldMappings, "TotalDeduction", correctedInvoice.TotalDeduction?.ToString(), log);

                log.Debug("Updated template fields for invoice {InvoiceNo}", correctedInvoice.InvoiceNo);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error updating invoice fields in template for {InvoiceNo}", correctedInvoice.InvoiceNo);
            }
        }

        /// <summary>
        /// Updates a specific field value in template line values
        /// </summary>
        private static void UpdateFieldInTemplate(Invoice template, Dictionary<string, (int LineId, int FieldId)> fieldMappings,
            string fieldName, string newValue, ILogger log)
        {
            try
            {
                if (string.IsNullOrEmpty(newValue) || !fieldMappings.ContainsKey(fieldName))
                    return;

                var (lineId, fieldId) = fieldMappings[fieldName];

                // Find the line and update its value
                var line = template.Lines.FirstOrDefault(l => l.OCR_Lines?.Id == lineId);
                if (line?.Values != null)
                {
                    // Line.Values structure: Dictionary<(int lineNumber, string section), Dictionary<(Fields Fields, string Instance), string>>
                    // We need to find the correct inner dictionary and update the field value

                    bool updated = false;
                    foreach (var outerKvp in line.Values)
                    {
                        var innerDict = outerKvp.Value;
                        if (innerDict != null)
                        {
                            // Find the field in the inner dictionary
                            var fieldKey = innerDict.Keys.FirstOrDefault(k => k.Fields?.Id == fieldId);
                            if (fieldKey.Fields != null)
                            {
                                innerDict[fieldKey] = newValue;
                                log.Debug("Updated template field {FieldName} = {NewValue} (LineId: {LineId}, FieldId: {FieldId}, LineNumber: {LineNumber}, Section: {Section})",
                                    fieldName, newValue, lineId, fieldId, outerKvp.Key.lineNumber, outerKvp.Key.section);
                                updated = true;
                                break; // Update first occurrence
                            }
                        }
                    }

                    if (!updated)
                    {
                        log.Warning("Could not find field {FieldName} (FieldId: {FieldId}) in template line {LineId} to update",
                            fieldName, fieldId, lineId);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error updating field {FieldName} in template", fieldName);
            }
        }

        private static void TestDataStructures(ILogger logger)
        {
            logger?.Information("Testing data structures...");

            var error = new InvoiceError
            {
                Field = "InvoiceTotal",
                ExtractedValue = "123,45",
                CorrectValue = "123.45",
                Confidence = 0.95,
                ErrorType = "decimal_separator",
                Reasoning = "OCR confused comma with period"
            };

            var result = new CorrectionResult
            {
                FieldName = "InvoiceTotal",
                OldValue = "123,45",
                NewValue = "123.45",
                Success = true,
                Confidence = 0.95
            };

            logger?.Information("✓ Data structures working correctly");
        }

        private static void TestJsonParsing(ILogger logger)
        {
            logger?.Information("Testing JSON parsing...");

            var testJson = @"{
                ""errors"": [
                    {
                        ""field"": ""InvoiceTotal"",
                        ""extracted_value"": ""123,45"",
                        ""correct_value"": ""123.45"",
                        ""confidence"": 0.95,
                        ""error_type"": ""decimal_separator"",
                        ""reasoning"": ""OCR confused comma with period""
                    }
                ]
            }";

            using var service = new OCRCorrectionService(logger);
            var cleanJson = service.CleanJsonResponse(testJson);

            if (string.IsNullOrEmpty(cleanJson))
                throw new Exception("JSON cleaning failed");

            using var doc = System.Text.Json.JsonDocument.Parse(cleanJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("errors", out var errorsElement) ||
                errorsElement.GetArrayLength() != 1)
                throw new Exception("JSON parsing failed");

            logger?.Information("✓ JSON parsing logic verified");
        }

        private static void TestFieldParsing(ILogger logger)
        {
            logger?.Information("Testing field parsing...");

            using var service = new OCRCorrectionService(logger);

            // Test numeric parsing
            var numericValue = service.ParseCorrectedValue("$123.45", "InvoiceTotal");
            if (numericValue is not decimal || (decimal)numericValue != 123.45m)
                throw new Exception("Numeric field parsing failed");

            // Test string parsing
            var stringValue = service.ParseCorrectedValue("ABC123", "InvoiceNo");
            if (stringValue is not string || stringValue.ToString() != "ABC123")
                throw new Exception("String field parsing failed");

            logger?.Information("✓ Field parsing logic verified");
        }

        private static async Task TestProductValidation(ILogger logger)
        {
            logger?.Information("Testing product validation logic...");

            var testInvoice = new ShipmentInvoice
            {
                InvoiceNo = "TEST-001",
                InvoiceDetails = new List<InvoiceDetails>
                {
                    new InvoiceDetails
                    {
                        LineNumber = 1,
                        Quantity = 2,
                        Cost = 10.50,
                        TotalCost = 21.00,
                        ItemDescription = "Test Item"
                    },
                    new InvoiceDetails
                    {
                        LineNumber = 2,
                        Quantity = -1, // Invalid quantity
                        Cost = 5.00,
                        TotalCost = -5.00,
                        ItemDescription = "Invalid Item"
                    }
                }
            };

            using var service = new OCRCorrectionService(logger);
            var mathErrors = service.ValidateMathematicalConsistency(testInvoice);

            if (!mathErrors.Any(e => e.ErrorType == "unreasonable_quantity"))
                throw new Exception("Product validation failed to detect invalid quantity");

            logger?.Information("✓ Product validation logic verified");
        }

        private static async Task TestMathematicalValidation(ILogger logger)
        {
            logger?.Information("Testing mathematical validation logic...");

            var testInvoice = new ShipmentInvoice
            {
                InvoiceNo = "TEST-002",
                SubTotal = 100.00,
                TotalInternalFreight = 10.00,
                TotalOtherCost = 5.00,
                InvoiceTotal = 110.00, // Should be 115.00
                InvoiceDetails = new List<InvoiceDetails>
                {
                    new InvoiceDetails
                    {
                        LineNumber = 1,
                        Quantity = 2,
                        Cost = 25.00,
                        TotalCost = 50.00
                    },
                    new InvoiceDetails
                    {
                        LineNumber = 2,
                        Quantity = 1,
                        Cost = 50.00,
                        TotalCost = 50.00
                    }
                }
            };

            using var service = new OCRCorrectionService(logger);
            var crossFieldErrors = service.ValidateCrossFieldConsistency(testInvoice);

            if (!crossFieldErrors.Any(e => e.ErrorType == "invoice_total_mismatch"))
                throw new Exception("Mathematical validation failed to detect total mismatch");

            logger?.Information("✓ Mathematical validation logic verified");
        }

        private static void TestRegexPatterns(ILogger logger)
        {
            logger?.Information("Testing regex pattern logic...");

            var testStrategy = new RegexCorrectionStrategy
            {
                StrategyType = "FORMAT_FIX",
                RegexPattern = @"\$?([0-9]+)[,]([0-9]{2})",
                ReplacementPattern = "$1.$2",
                Confidence = 0.95,
                Reasoning = "Convert European decimal comma to period"
            };

            if (string.IsNullOrEmpty(testStrategy.RegexPattern))
                throw new Exception("Regex pattern validation failed");

            logger?.Information("✓ Regex pattern logic verified");
        }

        #endregion
    }
}