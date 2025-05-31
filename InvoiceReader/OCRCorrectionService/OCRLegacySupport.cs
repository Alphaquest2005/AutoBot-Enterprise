// File: OCRCorrectionService/OCRLegacySupport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;

namespace WaterNut.DataSpace
{
    using System.IO;

    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Legacy Support)

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
        /// ENHANCED: Static method to correct multiple invoices with omission handling and fallback
        /// </summary>
        public static async Task CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger)
        {
            if (res == null || !res.Any() || template == null) return;

            var log = logger;

            try
            {
                log.Information("Starting enhanced OCR correction for {InvoiceCount} invoices with omission detection",
                    CountTotalInvoices(res));

                // Convert with enhanced metadata for omission detection
                var allShipmentInvoicesWithMetadata = ConvertDynamicToShipmentInvoicesWithMetadata(res, template, logger);
                var allShipmentInvoices = allShipmentInvoicesWithMetadata.Select(x => x.Invoice).ToList();
                var droppedFilePath = GetFilePathFromTemplate(template);
                var originalText = GetOriginalTextFromFile(droppedFilePath);

                log.Information("Processing {InvoiceCount} invoices from {FileCount} PDF sections with enhanced OCR metadata",
                    allShipmentInvoices.Count, res.Count);

                using var correctionService = new OCRCorrectionService(logger);

                // Process corrections with enhanced omission detection
                var correctionResults = await ProcessEnhancedCorrections(
                    allShipmentInvoicesWithMetadata,
                    originalText,
                    correctionService,
                    log);

                // Update both the dynamic results AND the template line values
                UpdateDynamicResultsWithCorrections(res, allShipmentInvoices, logger);
                UpdateTemplateLineValues(template, allShipmentInvoices, logger);

                log.Information("Completed enhanced OCR correction with {SuccessfulCorrections} successful corrections",
                    correctionResults.SuccessfulUpdates);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error in enhanced static CorrectInvoices");
            }
        }

        /// <summary>
        /// NEW: Direct data correction fallback when regex fixes fail
        /// </summary>
        public static List<dynamic> ApplyDirectDataCorrectionFallback(
            List<dynamic> res,
            string originalText,
            ILogger logger)
        {
            logger.Warning("Applying direct data correction fallback - regex fixes failed");

            try
            {
                using var correctionService = new OCRCorrectionService(logger);

                // Process each invoice section
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i] is List<IDictionary<string, object>> invoiceList)
                    {
                        for (int j = 0; j < invoiceList.Count; j++)
                        {
                            var invoiceDict = invoiceList[j];
                            var correctedDict = ApplyDirectCorrectionsToInvoice(
                                invoiceDict,
                                originalText,
                                correctionService,
                                logger);

                            if (correctedDict != null)
                            {
                                invoiceList[j] = correctedDict;
                                logger.Information("Applied direct corrections to invoice {Index}", j);
                            }
                        }
                    }
                }

                logger.Information("Completed direct data correction fallback");
                return res;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in direct data correction fallback");
                return res; // Return original data if fallback fails
            }
        }

        #endregion

        #region Enhanced Private Methods

        /// <summary>
        /// Processes enhanced corrections with omission detection
        /// </summary>
        private static async Task<EnhancedCorrectionResult> ProcessEnhancedCorrections(
            List<ShipmentInvoiceWithMetadata> invoicesWithMetadata,
            string originalText,
            OCRCorrectionService correctionService,
            ILogger logger)
        {
            var result = new EnhancedCorrectionResult
            {
                StartTime = DateTime.UtcNow,
                TotalCorrections = 0,
                SuccessfulUpdates = 0,
                FailedUpdates = 0
            };

            try
            {
                foreach (var invoiceWithMetadata in invoicesWithMetadata)
                {
                    // Detect errors with enhanced omission detection
                    var errors = await correctionService.DetectInvoiceErrorsAsync(
                        invoiceWithMetadata.Invoice,
                        originalText,
                        invoiceWithMetadata.FieldMetadata);

                    if (errors.Any())
                    {
                        // Apply corrections including omissions
                        var corrections = await correctionService.ApplyCorrectionsAsync(
                            invoiceWithMetadata.Invoice,
                            errors,
                            originalText,
                            invoiceWithMetadata.FieldMetadata);

                        result.TotalCorrections += corrections.Count;
                        result.SuccessfulUpdates += corrections.Count(c => c.Success);
                        result.FailedUpdates += corrections.Count(c => !c.Success);

                        logger.Information("Processed {ErrorCount} errors for invoice {InvoiceNo}, {SuccessCount} successful",
                            errors.Count, invoiceWithMetadata.Invoice.InvoiceNo, corrections.Count(c => c.Success));
                    }
                }

                result.EndTime = DateTime.UtcNow;
                result.ProcessingDuration = result.EndTime - result.StartTime;

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in enhanced correction processing");
                result.EndTime = DateTime.UtcNow;
                result.ProcessingDuration = result.EndTime - result.StartTime;
                result.HasErrors = true;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Applies direct corrections to a single invoice dictionary
        /// </summary>
        private static IDictionary<string, object> ApplyDirectCorrectionsToInvoice(
            IDictionary<string, object> invoiceDict,
            string originalText,
            OCRCorrectionService correctionService,
            ILogger logger)
        {
            try
            {
                // Create temporary invoice for analysis
                var tempInvoice = CreateTempShipmentInvoice(invoiceDict, logger);

                // Check if correction is needed
                if (TotalsZero(tempInvoice, logger))
                {
                    return invoiceDict; // Already correct
                }

                // Request direct corrections from DeepSeek
                var corrections = RequestDirectCorrectionsFromDeepSeek(
                    invoiceDict,
                    originalText,
                    correctionService,
                    logger);

                if (corrections?.Any() == true)
                {
                    // Apply corrections directly to dictionary
                    foreach (var correction in corrections)
                    {
                        if (invoiceDict.ContainsKey(correction.FieldName))
                        {
                            invoiceDict[correction.FieldName] = correction.CorrectedValue;
                            logger.Debug("Applied direct correction: {Field} = {Value}",
                                correction.FieldName, correction.CorrectedValue);
                        }
                    }

                    // Verify correction worked
                    var correctedInvoice = CreateTempShipmentInvoice(invoiceDict, logger);
                    if (TotalsZero(correctedInvoice, logger))
                    {
                        logger.Information("Direct correction successful for invoice {InvoiceNo}",
                            correctedInvoice.InvoiceNo);
                        return invoiceDict;
                    }
                }

                return invoiceDict; // Return original if corrections failed
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error applying direct corrections to invoice");
                return invoiceDict;
            }
        }

        /// <summary>
        /// Requests direct corrections from DeepSeek for fallback scenario
        /// </summary>
        private static List<DirectCorrection> RequestDirectCorrectionsFromDeepSeek(
            IDictionary<string, object> invoiceDict,
            string originalText,
            OCRCorrectionService correctionService,
            ILogger logger)
        {
            try
            {
                var invoiceJson = JsonSerializer.Serialize(invoiceDict, new JsonSerializerOptions { WriteIndented = true });

                var prompt = $@"DIRECT DATA CORRECTION - BYPASS REGEX:

The OCR import patterns could not be fixed. Provide direct value corrections to make the invoice math balance correctly.

EXTRACTED DATA:
{invoiceJson}

ORIGINAL TEXT:
{correctionService.CleanTextForAnalysis(originalText)}

REQUIREMENTS:
1. Provide correct values that make the math balance: SubTotal + Freight + Other + Insurance - Deduction = InvoiceTotal
2. Focus on critical fields that affect TotalsZero calculation
3. Use exact values from the original text

RESPONSE FORMAT:
{{
  ""corrections"": [
    {{
      ""field"": ""InvoiceTotal"",
      ""current_value"": ""wrong_value"",
      ""correct_value"": ""right_value"",
      ""reasoning"": ""explanation""
    }}
  ]
}}";

                // This would need to be implemented as part of the correctionService
                // For now, return empty list as placeholder
                logger.Debug("Direct correction prompt created, but DeepSeek call not implemented in this method");
                return new List<DirectCorrection>();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error requesting direct corrections from DeepSeek");
                return new List<DirectCorrection>();
            }
        }

        /// <summary>
        /// Enhanced metadata extraction with PartId and regex pattern information
        /// </summary>
        private static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<dynamic> res,
            Invoice template,
            ILogger logger)
        {
            var allInvoicesWithMetadata = new List<ShipmentInvoiceWithMetadata>();

            try
            {
                // Create enhanced field mapping from template
                var fieldMappings = GetEnhancedTemplateFieldMappings(template, logger);

                foreach (var item in res)
                {
                    if (item is List<IDictionary<string, object>> invoiceList)
                    {
                        foreach (var invoiceDict in invoiceList)
                        {
                            var shipmentInvoice = CreateTempShipmentInvoice(invoiceDict, logger);
                            var metadata = ExtractEnhancedOCRMetadata(invoiceDict, template, fieldMappings, logger);

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
                        var metadata = ExtractEnhancedOCRMetadata(singleInvoiceDict, template, fieldMappings, logger);

                        allInvoicesWithMetadata.Add(new ShipmentInvoiceWithMetadata
                        {
                            Invoice = shipmentInvoice,
                            FieldMetadata = metadata
                        });
                    }
                }

                logger.Information("Successfully converted {TotalInvoices} invoices with enhanced OCR metadata",
                    allInvoicesWithMetadata.Count);

                return allInvoicesWithMetadata;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in ConvertDynamicToShipmentInvoicesWithMetadata");
                return new List<ShipmentInvoiceWithMetadata>();
            }
        }

        /// <summary>
        /// Creates enhanced field mappings with PartId and regex information
        /// </summary>
        private static Dictionary<string, EnhancedFieldMapping> GetEnhancedTemplateFieldMappings(
            Invoice template,
            ILogger logger)
        {
            var mappings = new Dictionary<string, EnhancedFieldMapping>();

            try
            {
                foreach (var line in template.Lines?.Where(l => l?.OCR_Lines?.Fields != null) ?? Enumerable.Empty<Line>())
                {
                    foreach (var field in line.OCR_Lines.Fields)
                    {
                        var fieldName = field.Field?.Trim();
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            var mappedFieldName = MapFieldNameToProperty(fieldName);
                            if (!string.IsNullOrEmpty(mappedFieldName) && !mappings.ContainsKey(mappedFieldName))
                            {
                                mappings[mappedFieldName] = new EnhancedFieldMapping
                                {
                                    LineId = line.OCR_Lines.Id,
                                    FieldId = field.Id,
                                    PartId = line.OCR_Lines.PartId ?? 1, // Default to Header if not specified
                                    RegexPattern = line.OCR_Lines.RegularExpressions?.RegEx,
                                    Key = field.Key,
                                    FieldName = field.Field,
                                    EntityType = field.EntityType,
                                    DataType = field.DataType
                                };
                            }
                        }
                    }
                }

                logger.Debug("Created {MappingCount} enhanced field mappings from template", mappings.Count);
                return mappings;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating enhanced template field mappings");
                return new Dictionary<string, EnhancedFieldMapping>();
            }
        }

        /// <summary>
        /// Extracts enhanced OCR metadata with complete template context
        /// </summary>
        private static Dictionary<string, OCRFieldMetadata> ExtractEnhancedOCRMetadata(
            IDictionary<string, object> invoiceDict,
            Invoice template,
            Dictionary<string, EnhancedFieldMapping> fieldMappings,
            ILogger logger)
        {
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            try
            {
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

                    // Try to find enhanced field mapping
                    if (fieldMappings.TryGetValue(fieldName, out var mapping))
                    {
                        var fieldMetadata = new OCRFieldMetadata
                        {
                            // Basic field information
                            FieldName = fieldName,
                            Value = fieldValue,
                            RawValue = fieldValue,

                            // Enhanced OCR Template Context
                            LineNumber = 0, // Will be set from Lines.Values if available
                            FieldId = mapping.FieldId,
                            LineId = mapping.LineId,
                            RegexId = GetRegexIdFromPattern(mapping.RegexPattern),
                            Key = mapping.Key,
                            Field = mapping.FieldName,
                            EntityType = mapping.EntityType,
                            DataType = mapping.DataType,

                            // Enhanced context
                            PartId = mapping.PartId,

                            // Invoice Context
                            InvoiceId = invoiceContext.InvoiceId,
                            InvoiceName = invoiceContext.InvoiceName,

                            // Processing Context
                            Confidence = 1.0 // Default confidence for successful extraction
                        };

                        metadata[fieldName] = fieldMetadata;
                    }
                }

                logger.Debug("Extracted enhanced OCR metadata for {FieldCount} fields", metadata.Count);
                return metadata;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error extracting enhanced OCR metadata");
                return new Dictionary<string, OCRFieldMetadata>();
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets regex ID from pattern (placeholder - would need database lookup)
        /// </summary>
        private static int? GetRegexIdFromPattern(string regexPattern)
        {
            // This would require database lookup to find the regex ID
            // For now, return null as placeholder
            return null;
        }

        /// <summary>
        /// Creates temporary ShipmentInvoice from dictionary
        /// </summary>
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

                // Handle invoice details
                if (!x.ContainsKey("InvoiceDetails"))
                {
                    invoice.InvoiceDetails = new List<InvoiceDetails>();
                }
                else
                {
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
                                        LineNumber = index + 1,
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
        /// Checks if a field name is a metadata field that should be skipped
        /// </summary>
        private static bool IsMetadataField(string fieldName)
        {
            var metadataFields = new[] { "LineNumber", "FileLineNumber", "Section", "Instance" };
            return metadataFields.Contains(fieldName);
        }

        /// <summary>
        /// Gets file path from template
        /// </summary>
        private static string GetFilePathFromTemplate(Invoice template)
        {
            return template?.FilePath ?? "unknown";
        }

        /// <summary>
        /// Gets original text from file
        /// </summary>
        private static string GetOriginalTextFromFile(string filePath)
        {
            try
            {
                var txtFile = filePath + ".txt";
                if (File.Exists(txtFile))
                {
                    return File.ReadAllText(txtFile);
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
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
        /// Updates dynamic results with corrected values
        /// </summary>
        private static void UpdateDynamicResultsWithCorrections(List<dynamic> res, List<ShipmentInvoice> correctedInvoices, ILogger logger)
        {
            try
            {
                int correctedIndex = 0;

                for (int sectionIndex = 0; sectionIndex < res.Count; sectionIndex++)
                {
                    if (res[sectionIndex] is List<IDictionary<string, object>> invoiceList)
                    {
                        for (int invoiceIndex = 0; invoiceIndex < invoiceList.Count && correctedIndex < correctedInvoices.Count; invoiceIndex++, correctedIndex++)
                        {
                            var invoiceDict = invoiceList[invoiceIndex];
                            var correctedInvoice = correctedInvoices[correctedIndex];

                            try
                            {
                                // Update main fields
                                if (correctedInvoice.InvoiceTotal.HasValue)
                                    invoiceDict["InvoiceTotal"] = correctedInvoice.InvoiceTotal.Value;
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
        /// Updates template line values with corrected data so template.Read() uses corrected values
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
                                log.Debug("Updated template field {FieldName} = {NewValue} (LineId: {LineId}, FieldId: {FieldId})",
                                    fieldName, newValue, lineId, fieldId);
                                updated = true;
                                break;
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

        #endregion

        #region Data Models for Enhanced Support

        /// <summary>
        /// Enhanced field mapping with complete OCR context
        /// </summary>
        public class EnhancedFieldMapping
        {
            public int LineId { get; set; }
            public int FieldId { get; set; }
            public int PartId { get; set; }
            public string RegexPattern { get; set; }
            public string Key { get; set; }
            public string FieldName { get; set; }
            public string EntityType { get; set; }
            public string DataType { get; set; }
        }

        /// <summary>
        /// Direct correction for fallback scenario
        /// </summary>
        public class DirectCorrection
        {
            public string FieldName { get; set; }
            public string CurrentValue { get; set; }
            public string CorrectedValue { get; set; }
            public string Reasoning { get; set; }
        }

        #endregion
    }
}