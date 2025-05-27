﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Added for ILogger
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;
using InventoryQS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using OCR.Business.Entities;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using static java.util.Locale;

namespace WaterNut.DataSpace
{
    using System.Data.Entity.Infrastructure.Design;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;

    using MoreLinq;

    public class ShipmentInvoiceImporter
    {
        static ShipmentInvoiceImporter()
        {
        }

        public ShipmentInvoiceImporter()
        {
        }

        public async Task<bool> ProcessShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst, Dictionary<string, string> invoicePOs, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ProcessShipmentInvoice), "Process shipment invoice data from email or file", $"FileType: {fileType?.Description}, EmailId: {emailId}, FilePath: {droppedFilePath}, DocSetCount: {docSet?.Count ?? 0}");

            try
            {
                logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ProcessShipmentInvoice), $"Processing shipment invoice from File: {droppedFilePath ?? emailId}");

                var invoiceData = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).ToList();
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(ProcessShipmentInvoice), "DataExtraction", "Extracted invoice data from input list.", $"RawItemCount: {invoiceData.Count}");

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ExtractShipmentInvoices", "ASYNC_EXPECTED");
                var extractStopwatch = Stopwatch.StartNew();
                var shipmentInvoices = await ExtractShipmentInvoices(fileType, emailId, droppedFilePath, invoiceData, logger).ConfigureAwait(false); // Pass logger




                extractStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "ExtractShipmentInvoices", extractStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                var invoices = ReduceLstByInvoiceNo(shipmentInvoices);
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(ProcessShipmentInvoice), "DataReduction", "Reduced list by invoice number.", $"UniqueInvoiceCount: {invoices.Count}");

                var goodInvoices = invoices.Where(x => x.InvoiceDetails.All(z => !string.IsNullOrEmpty(z.ItemDescription)))
                                            .Where(x => x.InvoiceDetails.Any())
                                            .ToList();
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(ProcessShipmentInvoice), "DataFiltering", "Filtered for good invoices.", $"GoodInvoiceCount: {goodInvoices.Count}");


                //Create Shipment Invoice Corrector
                var correctedInvoices = CorrectInvoices(goodInvoices, droppedFilePath);

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "SaveInvoicePOsAsync", "ASYNC_EXPECTED");
                var saveStopwatch = Stopwatch.StartNew();
                await SaveInvoicePOsAsync(invoicePOs, correctedInvoices).ConfigureAwait(false);
                saveStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "SaveInvoicePOsAsync", saveStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                methodStopwatch.Stop(); // Stop stopwatch on success
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ProcessShipmentInvoice), "Shipment invoice processed successfully", $"ProcessedGoodInvoiceCount: {goodInvoices.Count}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ProcessShipmentInvoice), $"Successfully processed {goodInvoices.Count} good invoices", methodStopwatch.ElapsedMilliseconds);

                return true;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ProcessShipmentInvoice), "Process shipment invoice data from email or file", methodStopwatch.ElapsedMilliseconds, $"Error processing shipment invoice: {e.Message}");
                logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ProcessShipmentInvoice), "Overall shipment invoice processing", methodStopwatch.ElapsedMilliseconds, $"Error processing shipment invoice: {e.Message}");
                Console.WriteLine(e); // Keep Console.WriteLine for now
                return false;
            }
        }

        private List<ShipmentInvoice> CorrectInvoices(List<ShipmentInvoice> goodInvoices, string droppedFilePath)
        {
            var invoicesWithIssues = goodInvoices.Where(x => x.TotalsZero != 0).ToList();
            Console.WriteLine($"CorrectInvoices: Found {invoicesWithIssues.Count} invoices with TotalsZero != 0");

            invoicesWithIssues.ForEach(
                x =>
                    {
                        Console.WriteLine($"Processing invoice {x.InvoiceNo} with TotalsZero: {x.TotalsZero}");
                        Console.WriteLine($"  Current values - SubTotal: {x.SubTotal}, Freight: {x.TotalInternalFreight}, OtherCost: {x.TotalOtherCost}, Insurance: {x.TotalInsurance}, Deduction: {x.TotalDeduction}");

                        var txtFile = droppedFilePath + ".txt";
                        if (!File.Exists(txtFile))
                        {
                            Console.WriteLine($"ERROR: Text file not found: {txtFile}");
                            return;
                        }

                        var fileTxt = File.ReadAllText(txtFile);
                        Console.WriteLine($"Read text file with {fileTxt.Length} characters");

                        List<(string Field, string Error, string Value)> errors = GetInvoiceDataErrors(x, fileTxt);
                        Console.WriteLine($"GetInvoiceDataErrors found {errors.Count} errors:");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"  Error: Field={error.Field}, Error={error.Error}, Value={error.Value}");
                        }

                        UpdateInvoice(x, errors);
                        Console.WriteLine($"After UpdateInvoice - TotalsZero: {x.TotalsZero}");
                        Console.WriteLine($"  Updated values - SubTotal: {x.SubTotal}, Freight: {x.TotalInternalFreight}, OtherCost: {x.TotalOtherCost}, Insurance: {x.TotalInsurance}, Deduction: {x.TotalDeduction}");

                        UpdateRegex(errors, fileTxt, droppedFilePath);
                        Console.WriteLine($"UpdateRegex completed for {errors.Count} errors");
                    });
            return goodInvoices;
        }

        private async void UpdateRegex(List<(string Field, string Error, string Value)> errors, string fileTxt, string droppedFilePath)
        {
            if (errors == null || !errors.Any() || string.IsNullOrEmpty(fileTxt))
                return;

            try
            {
                // Get the OCR Invoice from the dropped file path
                var ocrInvoice = GetOCRInvoiceFromPath(droppedFilePath);
                if (ocrInvoice?.Parts == null)
                {
                    Console.WriteLine("Could not find OCR invoice template for regex correction");
                    return;
                }

                var ocrCorrectionService = new OCRCorrectionService();
                await ocrCorrectionService.UpdateRegexPatternsAsync(errors, fileTxt, ocrInvoice);
                Console.WriteLine($"Successfully processed {errors.Count} field errors using OCR correction service");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateRegex: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the OCR Invoice template associated with the dropped file path
        /// </summary>
        private OCR.Business.Entities.Invoices GetOCRInvoiceFromPath(string droppedFilePath)
        {
            try
            {
                // Strategy-based OCR template selection
                using (var ctx = new OCRContext())
                {
                    var fileName = Path.GetFileNameWithoutExtension(droppedFilePath)?.ToLower() ?? "";
                    OCR.Business.Entities.Invoices invoice = null;

                    // Strategy 1: Match by filename patterns for specific suppliers
                    var supplierPatterns = new Dictionary<string, string[]>
                    {
                        { "amazon", new[] { "amazon", "amzn" } },
                        { "walmart", new[] { "walmart", "wal-mart" } },
                        { "target", new[] { "target" } },
                        { "costco", new[] { "costco" } },
                        { "fedex", new[] { "fedex", "fed-ex" } },
                        { "ups", new[] { "ups" } },
                        { "dhl", new[] { "dhl" } }
                    };

                    foreach (var pattern in supplierPatterns)
                    {
                        if (pattern.Value.Any(p => fileName.Contains(p)))
                        {
                            invoice = ctx.Invoices
                                .Include(x => x.Parts)
                                .Include("Parts.Lines")
                                .Include("Parts.Lines.Fields")
                                .Include("Parts.Lines.RegularExpressions")
                                .FirstOrDefault(x => x.IsActive && x.Name.ToLower().Contains(pattern.Key));

                            if (invoice != null)
                            {
                                Console.WriteLine($"Selected supplier-specific template: {invoice.Name} for file: {Path.GetFileName(droppedFilePath)}");
                                break;
                            }
                        }
                    }

                    // Strategy 2: Find generic/default template if no specific match
                    if (invoice == null)
                    {
                        invoice = ctx.Invoices
                            .Include(x => x.Parts)
                            .Include("Parts.Lines")
                            .Include("Parts.Lines.Fields")
                            .Include("Parts.Lines.RegularExpressions")
                            .FirstOrDefault(x => x.IsActive &&
                                (x.Name.ToLower().Contains("generic") ||
                                 x.Name.ToLower().Contains("default") ||
                                 x.Name.ToLower().Contains("standard")));

                        if (invoice != null)
                        {
                            Console.WriteLine($"Selected generic template: {invoice.Name} for file: {Path.GetFileName(droppedFilePath)}");
                        }
                    }

                    // Strategy 3: Get first active template as fallback
                    if (invoice == null)
                    {
                        invoice = ctx.Invoices
                            .Include(x => x.Parts)
                            .Include("Parts.Lines")
                            .Include("Parts.Lines.Fields")
                            .Include("Parts.Lines.RegularExpressions")
                            .FirstOrDefault(x => x.IsActive);

                        if (invoice != null)
                        {
                            Console.WriteLine($"Selected fallback template: {invoice.Name} for file: {Path.GetFileName(droppedFilePath)}");
                        }
                    }

                    if (invoice == null)
                    {
                        Console.WriteLine($"No active OCR invoice template found for {droppedFilePath}");
                    }

                    return invoice;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting OCR Invoice from path: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a regex pattern to match similar OCR errors
        /// </summary>
        private string CreateRegexPattern(string originalError, string correctedValue)
        {
            try
            {
                // Simple pattern creation - can be enhanced based on OCR error patterns
                // This creates a pattern that matches the original error text
                var escapedError = System.Text.RegularExpressions.Regex.Escape(originalError);

                // Add some flexibility for common OCR errors
                var pattern = escapedError
                    .Replace("0", "[0O]")  // 0 and O confusion
                    .Replace("1", "[1Il]") // 1, I, l confusion
                    .Replace("5", "[5S]")  // 5 and S confusion
                    .Replace("6", "[6G]")  // 6 and G confusion
                    .Replace("8", "[8B]")  // 8 and B confusion
                    .Replace("\\.", "[\\.,]"); // Period and comma confusion

                return pattern;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating regex pattern: {ex.Message}");
                return System.Text.RegularExpressions.Regex.Escape(originalError);
            }
        }

        private void UpdateInvoice(ShipmentInvoice shipmentInvoice, List<(string Field, string Error, string Value)> errors)
        {
            if (shipmentInvoice == null || errors == null || !errors.Any())
                return;

            foreach (var error in errors)
            {
                try
                {
                    // Apply corrections based on the field name and corrected value
                    switch (error.Field.ToUpperInvariant())
                    {
                        case "TOTALINTERNALFREIGHT":
                            if (double.TryParse(error.Value, out var internalFreight))
                            {
                                shipmentInvoice.TotalInternalFreight = internalFreight;
                            }
                            break;

                        case "TOTALOTHERCOST":
                            if (double.TryParse(error.Value, out var otherCost))
                            {
                                shipmentInvoice.TotalOtherCost = otherCost;
                            }
                            break;

                        case "TOTALINSURANCE":
                            if (double.TryParse(error.Value, out var insurance))
                            {
                                shipmentInvoice.TotalInsurance = insurance;
                            }
                            break;

                        case "TOTALDEDUCTION":
                            if (double.TryParse(error.Value, out var deduction))
                            {
                                shipmentInvoice.TotalDeduction = deduction;
                            }
                            break;

                        case "INVOICETOTAL":
                            if (double.TryParse(error.Value, out var invoiceTotal))
                            {
                                shipmentInvoice.InvoiceTotal = invoiceTotal;
                            }
                            break;

                        case "SUBTOTAL":
                            if (double.TryParse(error.Value, out var subTotal))
                            {
                                shipmentInvoice.SubTotal = subTotal;
                            }
                            break;

                        default:
                            // Handle invoice detail line corrections
                            if (error.Field.StartsWith("InvoiceDetail_Line"))
                            {
                                ApplyInvoiceDetailCorrection(shipmentInvoice, error);
                            }
                            else
                            {
                                // Log unhandled field types for future enhancement
                                Console.WriteLine($"Unhandled field correction: {error.Field} = {error.Value}");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue processing other corrections
                    Console.WriteLine($"Error applying correction for field {error.Field}: {ex.Message}");
                }
            }

            // Mark the invoice as modified for Entity Framework tracking
            shipmentInvoice.TrackingState = TrackingState.Modified;
        }

        private void ApplyInvoiceDetailCorrection(ShipmentInvoice shipmentInvoice, (string Field, string Error, string Value) error)
        {
            try
            {
                // Parse field name: InvoiceDetail_Line{LineNumber}_{FieldName}
                var parts = error.Field.Split('_');
                if (parts.Length < 3) return;

                var lineNumberStr = parts[1].Replace("Line", "");
                if (!int.TryParse(lineNumberStr, out var lineNumber)) return;

                var fieldName = parts[2];
                var detail = shipmentInvoice.InvoiceDetails?.FirstOrDefault(d => d.LineNumber == lineNumber);
                if (detail == null) return;

                switch (fieldName.ToUpperInvariant())
                {
                    case "QUANTITY":
                        if (double.TryParse(error.Value, out var quantity))
                        {
                            detail.Quantity = quantity;
                            // Recalculate total cost
                            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
                            detail.TrackingState = TrackingState.Modified;
                        }
                        break;

                    case "COST":
                        if (double.TryParse(error.Value, out var cost))
                        {
                            detail.Cost = cost;
                            // Recalculate total cost
                            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
                            detail.TrackingState = TrackingState.Modified;
                        }
                        break;

                    case "TOTALCOST":
                        if (double.TryParse(error.Value, out var totalCost))
                        {
                            detail.TotalCost = totalCost;
                            detail.TrackingState = TrackingState.Modified;
                        }
                        break;

                    case "DISCOUNT":
                        if (double.TryParse(error.Value, out var discount))
                        {
                            detail.Discount = discount;
                            // Recalculate total cost
                            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
                            detail.TrackingState = TrackingState.Modified;
                        }
                        break;

                    default:
                        Console.WriteLine($"Unhandled invoice detail field correction: {fieldName} = {error.Value}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying invoice detail correction for {error.Field}: {ex.Message}");
            }
        }

        private List<(string Field, string Error, string Value)> GetInvoiceDataErrors(
            ShipmentInvoice shipmentInvoice,
            string fileTxt)
        {
            var errors = new List<(string Field, string Error, string Value)>();

            try
            {
                Console.WriteLine($"GetInvoiceDataErrors: Starting error detection for invoice {shipmentInvoice?.InvoiceNo}");

                if (shipmentInvoice == null || string.IsNullOrEmpty(fileTxt))
                {
                    Console.WriteLine("GetInvoiceDataErrors: Null invoice or empty file text, returning empty errors");
                    return errors;
                }

                Console.WriteLine("GetInvoiceDataErrors: Checking header field errors...");
                // Check header-level fields first
                CheckHeaderFieldErrors(shipmentInvoice, fileTxt, errors);
                Console.WriteLine($"GetInvoiceDataErrors: Header check found {errors.Count} errors");

                Console.WriteLine("GetInvoiceDataErrors: Checking invoice detail errors...");
                // Check invoice detail line items for price/quantity errors
                CheckInvoiceDetailErrors(shipmentInvoice, fileTxt, errors);
                Console.WriteLine($"GetInvoiceDataErrors: Detail check found {errors.Count} total errors");

                Console.WriteLine("GetInvoiceDataErrors: Checking mathematical consistency...");
                // Check mathematical consistency
                CheckMathematicalConsistency(shipmentInvoice, errors);
                Console.WriteLine($"GetInvoiceDataErrors: Math check found {errors.Count} total errors");

                Console.WriteLine($"GetInvoiceDataErrors: Completed with {errors.Count} total errors");
                return errors;
            }
            catch (Exception ex)
            {
                // Log the error but continue processing
                Console.WriteLine($"Error in GetInvoiceDataErrors: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return errors;
            }
        }

        /// <summary>
        /// Creates DeepSeek prompt to compare invoice data with original text
        /// </summary>
        private string CreateErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            var invoiceJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                InvoiceNo = invoice.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceTotal = invoice.InvoiceTotal,
                SubTotal = invoice.SubTotal,
                TotalInternalFreight = invoice.TotalInternalFreight,
                TotalOtherCost = invoice.TotalOtherCost,
                TotalInsurance = invoice.TotalInsurance,
                TotalDeduction = invoice.TotalDeduction,
                Currency = invoice.Currency,
                SupplierName = invoice.SupplierName,
                SupplierAddress = invoice.SupplierAddress
            });

            return $@"INVOICE DATA COMPARISON AND ERROR DETECTION:

Compare the extracted invoice data with the original text and identify discrepancies.

EXTRACTED DATA:
{invoiceJson}

ORIGINAL TEXT:
{fileText}

FIELD MAPPING GUIDANCE:
- TotalInternalFreight: Shipping + Handling + Transportation fees
- TotalOtherCost: Taxes + Fees + Duties
- TotalInsurance: Insurance costs
- TotalDeduction: Coupons, credits, free shipping markers

Return ONLY a JSON object with errors found:
{{
  ""errors"": [
    {{
      ""field"": ""FieldName"",
      ""extracted_value"": ""WrongValue"",
      ""correct_value"": ""CorrectValue"",
      ""error_description"": ""Description of the discrepancy""
    }}
  ]
}}

If no errors found, return: {{""errors"": []}}";
        }

        private void CheckHeaderFieldErrors(ShipmentInvoice shipmentInvoice, string fileTxt, List<(string Field, string Error, string Value)> errors)
        {
            try
            {
                Console.WriteLine("CheckHeaderFieldErrors: Creating error detection prompt...");
                // Create comparison prompt for header fields
                var prompt = CreateErrorDetectionPrompt(shipmentInvoice, fileTxt);
                Console.WriteLine($"CheckHeaderFieldErrors: Created prompt with {prompt.Length} characters");

                Console.WriteLine("CheckHeaderFieldErrors: Initializing DeepSeek API...");
                // Use DeepSeek API to compare header data
                using (var deepSeekApi = new WaterNut.Business.Services.Utils.DeepSeekInvoiceApi())
                {
                    Console.WriteLine("CheckHeaderFieldErrors: DeepSeek API initialized successfully");
                    var originalTemplate = deepSeekApi.PromptTemplate;
                    deepSeekApi.PromptTemplate = prompt;

                    Console.WriteLine("CheckHeaderFieldErrors: Calling DeepSeek API...");
                    var response = deepSeekApi.ExtractShipmentInvoice(new List<string> { fileTxt }).Result;
                    Console.WriteLine($"CheckHeaderFieldErrors: DeepSeek API returned {response?.Count ?? 0} results");

                    deepSeekApi.PromptTemplate = originalTemplate;

                    Console.WriteLine("CheckHeaderFieldErrors: Parsing response...");
                    ParseHeaderErrorResponse(response, shipmentInvoice, errors);
                    Console.WriteLine("CheckHeaderFieldErrors: Response parsing completed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking header fields: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void CheckInvoiceDetailErrors(ShipmentInvoice shipmentInvoice, string fileTxt, List<(string Field, string Error, string Value)> errors)
        {
            try
            {
                if (shipmentInvoice.InvoiceDetails == null || !shipmentInvoice.InvoiceDetails.Any())
                    return;

                // Create prompt to re-extract invoice details and compare
                var prompt = CreateDetailErrorDetectionPrompt(shipmentInvoice, fileTxt);

                using (var deepSeekApi = new WaterNut.Business.Services.Utils.DeepSeekInvoiceApi())
                {
                    var originalTemplate = deepSeekApi.PromptTemplate;
                    deepSeekApi.PromptTemplate = prompt;

                    var response = deepSeekApi.ExtractShipmentInvoice(new List<string> { fileTxt }).Result;
                    deepSeekApi.PromptTemplate = originalTemplate;

                    ParseDetailErrorResponse(response, shipmentInvoice, errors);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking invoice details: {ex.Message}");
            }
        }

        private void CheckMathematicalConsistency(ShipmentInvoice shipmentInvoice, List<(string Field, string Error, string Value)> errors)
        {
            try
            {
                // Check if detail totals match subtotal
                if (shipmentInvoice.InvoiceDetails?.Any() == true)
                {
                    var calculatedSubTotal = shipmentInvoice.InvoiceDetails.Sum(d => d.TotalCost ?? 0);
                    var reportedSubTotal = shipmentInvoice.SubTotal ?? 0;

                    if (Math.Abs(calculatedSubTotal - reportedSubTotal) > 0.01)
                    {
                        errors.Add(("SubTotal",
                            $"Calculated: {calculatedSubTotal:F2}, Reported: {reportedSubTotal:F2}",
                            calculatedSubTotal.ToString("F2")));
                    }
                }

                // Check individual line calculations
                foreach (var detail in shipmentInvoice.InvoiceDetails ?? new List<InvoiceDetails>())
                {
                    var calculatedTotal = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
                    var reportedTotal = detail.TotalCost ?? 0;

                    if (Math.Abs(calculatedTotal - reportedTotal) > 0.01)
                    {
                        errors.Add(($"InvoiceDetail_Line{detail.LineNumber}_TotalCost",
                            $"Calculated: {calculatedTotal:F2}, Reported: {reportedTotal:F2}",
                            calculatedTotal.ToString("F2")));
                    }
                }

                // Check if invoice total matches subtotal + fees - deductions
                var calculatedInvoiceTotal = (shipmentInvoice.SubTotal ?? 0) +
                                           (shipmentInvoice.TotalInternalFreight ?? 0) +
                                           (shipmentInvoice.TotalOtherCost ?? 0) +
                                           (shipmentInvoice.TotalInsurance ?? 0) -
                                           (shipmentInvoice.TotalDeduction ?? 0);

                var reportedInvoiceTotal = shipmentInvoice.InvoiceTotal ?? 0;

                if (Math.Abs(calculatedInvoiceTotal - reportedInvoiceTotal) > 0.01)
                {
                    errors.Add(("InvoiceTotal",
                        $"Calculated: {calculatedInvoiceTotal:F2}, Reported: {reportedInvoiceTotal:F2}",
                        calculatedInvoiceTotal.ToString("F2")));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking mathematical consistency: {ex.Message}");
            }
        }

        private void ParseHeaderErrorResponse(List<dynamic> response, ShipmentInvoice originalInvoice, List<(string Field, string Error, string Value)> errors)
        {
            try
            {
                if (response?.Any() != true) return;

                var extractedInvoice = response.First() as IDictionary<string, object>;
                if (extractedInvoice == null) return;

                // Compare key header fields
                CompareField(errors, "InvoiceTotal", originalInvoice.InvoiceTotal, GetDecimalFromExtracted(extractedInvoice, "InvoiceTotal"));
                CompareField(errors, "SubTotal", originalInvoice.SubTotal, GetDecimalFromExtracted(extractedInvoice, "SubTotal"));
                CompareField(errors, "TotalInternalFreight", originalInvoice.TotalInternalFreight, GetDecimalFromExtracted(extractedInvoice, "TotalInternalFreight"));
                CompareField(errors, "TotalOtherCost", originalInvoice.TotalOtherCost, GetDecimalFromExtracted(extractedInvoice, "TotalOtherCost"));
                CompareField(errors, "TotalInsurance", originalInvoice.TotalInsurance, GetDecimalFromExtracted(extractedInvoice, "TotalInsurance"));
                CompareField(errors, "TotalDeduction", originalInvoice.TotalDeduction, GetDecimalFromExtracted(extractedInvoice, "TotalDeduction"));
                CompareField(errors, "Currency", originalInvoice.Currency, GetStringFromExtracted(extractedInvoice, "Currency"));
                CompareField(errors, "SupplierName", originalInvoice.SupplierName, GetStringFromExtracted(extractedInvoice, "SupplierName"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing header error response: {ex.Message}");
            }
        }

        private string CreateDetailErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            var details = invoice.InvoiceDetails?.Select(d => new
            {
                LineNumber = d.LineNumber,
                ItemDescription = d.ItemDescription,
                Quantity = d.Quantity,
                Cost = d.Cost,
                TotalCost = d.TotalCost,
                Discount = d.Discount
            }).ToList();

            var detailsJson = details != null
                ? System.Text.Json.JsonSerializer.Serialize(details)
                : "[]";

            return $@"INVOICE DETAIL COMPARISON AND ERROR DETECTION:

Compare the extracted invoice detail lines with the original text and identify discrepancies in quantities, prices, and calculations.

EXTRACTED DETAILS:
{detailsJson}

ORIGINAL TEXT:
{fileText}

Focus on:
1. Item quantities - check if OCR misread numbers
2. Unit prices - verify decimal places and currency formatting
3. Line totals - ensure (Quantity × Cost) - Discount = TotalCost
4. Item descriptions - verify they match the text

Return ONLY a JSON object with errors found:
{{
  ""errors"": [
    {{
      ""field"": ""InvoiceDetail_Line1_Quantity"",
      ""extracted_value"": ""WrongValue"",
      ""correct_value"": ""CorrectValue"",
      ""error_description"": ""Description of the discrepancy""
    }}
  ]
}}

If no errors found, return: {{""errors"": []}}";
        }

        private void ParseDetailErrorResponse(List<dynamic> response, ShipmentInvoice originalInvoice, List<(string Field, string Error, string Value)> errors)
        {
            try
            {
                if (response?.Any() != true) return;

                var extractedData = response.First() as IDictionary<string, object>;
                if (extractedData == null) return;

                // Compare invoice details if available
                if (extractedData.ContainsKey("InvoiceDetails"))
                {
                    var extractedDetails = extractedData["InvoiceDetails"] as List<object>;
                    if (extractedDetails != null && originalInvoice.InvoiceDetails != null)
                    {
                        CompareInvoiceDetails(originalInvoice.InvoiceDetails, extractedDetails, errors);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing detail error response: {ex.Message}");
            }
        }

        private void CompareInvoiceDetails(List<InvoiceDetails> originalDetails, List<object> extractedDetails, List<(string Field, string Error, string Value)> errors)
        {
            for (int i = 0; i < Math.Min(originalDetails.Count, extractedDetails.Count); i++)
            {
                var original = originalDetails[i];
                var extracted = extractedDetails[i] as IDictionary<string, object>;
                if (extracted == null) continue;

                var linePrefix = $"InvoiceDetail_Line{original.LineNumber}";

                CompareField(errors, $"{linePrefix}_Quantity", original.Quantity, GetDecimalFromExtracted(extracted, "Quantity"));
                CompareField(errors, $"{linePrefix}_Cost", original.Cost, GetDecimalFromExtracted(extracted, "Cost"));
                CompareField(errors, $"{linePrefix}_TotalCost", original.TotalCost, GetDecimalFromExtracted(extracted, "TotalCost"));
                CompareField(errors, $"{linePrefix}_Discount", original.Discount, GetDecimalFromExtracted(extracted, "Discount"));
            }
        }

        private void CompareField(List<(string Field, string Error, string Value)> errors, string fieldName, object originalValue, object extractedValue)
        {
            if (!AreValuesEqual(originalValue, extractedValue))
            {
                var error = $"Original: {originalValue ?? "null"}, Extracted: {extractedValue ?? "null"}";
                errors.Add((fieldName, error, extractedValue?.ToString() ?? ""));
            }
        }

        private bool AreValuesEqual(object original, object extracted)
        {
            if (original == null && extracted == null) return true;
            if (original == null || extracted == null) return false;

            // For decimal comparisons, allow small tolerance
            if (original is decimal origDecimal && extracted is decimal extDecimal)
            {
                return Math.Abs(origDecimal - extDecimal) < 0.01m;
            }

            if (original is double origDouble && extracted is double extDouble)
            {
                return Math.Abs(origDouble - extDouble) < 0.01;
            }

            return original.ToString().Equals(extracted?.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private decimal? GetDecimalFromExtracted(IDictionary<string, object> data, string key)
        {
            if (data.TryGetValue(key, out var value) && value != null)
            {
                if (decimal.TryParse(value.ToString(), out var result))
                    return result;
            }
            return null;
        }

        private string GetStringFromExtracted(IDictionary<string, object> data, string key)
        {
            if (data.TryGetValue(key, out var value))
                return value?.ToString();
            return null;
        }

        private static async Task<List<ShipmentInvoice>> ExtractShipmentInvoices(FileTypes fileType, string emailId, string droppedFilePath, List<IDictionary<string, object>> itms, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ExtractShipmentInvoices), "Extract shipment invoices from raw data", $"FileType: {fileType?.Description}, EmailId: {emailId}, FilePath: {droppedFilePath}, RawItemCount: {itms?.Count ?? 0}");

            try
            {
                logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ExtractShipmentInvoices), $"Extracting shipment invoices from {itms?.Count ?? 0} raw items");

                var tasks = itms.Select(x => (IDictionary<string, object>)x)
                    .Where(x => x != null && x.Any())
                    .Select(x => ProcessInvoiceItem(x, logger)) // Pass logger to ProcessInvoiceItem
                    .Where(x => x != null)
                    .ToList();

                if (!tasks.Any())
                {
                     logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                        nameof(ExtractShipmentInvoices), "TaskCreation", "InvoiceItemTasks", "No valid invoice items found to process.");
                }

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Task.WhenAll(ProcessInvoiceItem tasks)", "ASYNC_EXPECTED");
                var whenAllStopwatch = Stopwatch.StartNew();
                var results = await Task.WhenAll(tasks).ConfigureAwait(false);
                whenAllStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "Task.WhenAll(ProcessInvoiceItem tasks)", whenAllStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                var validResults = results.Where(x => x != null).ToList();
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(ExtractShipmentInvoices), "TaskCompletion", "Completed processing invoice item tasks.", $"ValidInvoiceCount: {validResults.Count}");

                methodStopwatch.Stop(); // Stop stopwatch on success
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ExtractShipmentInvoices), "Shipment invoices extracted successfully", $"ExtractedInvoiceCount: {validResults.Count}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ExtractShipmentInvoices), $"Successfully extracted {validResults.Count} shipment invoices", methodStopwatch.ElapsedMilliseconds);

                return validResults;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ExtractShipmentInvoices), "Extract shipment invoices from raw data", methodStopwatch.ElapsedMilliseconds, $"Error extracting shipment invoices: {e.Message}");
                logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ExtractShipmentInvoices), "Shipment invoice extraction process", methodStopwatch.ElapsedMilliseconds, $"Error extracting shipment invoices: {e.Message}");
                Console.WriteLine(e); // Keep Console.WriteLine for now
                throw;
            }

            async Task<ShipmentInvoice> ProcessInvoiceItem(IDictionary<string, object> x, ILogger itemLogger) // Added ILogger parameter
            {
                var itemMethodStopwatch = Stopwatch.StartNew(); // Start stopwatch for item method
                itemLogger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                    nameof(ProcessInvoiceItem), "Process a single invoice item dictionary", $"Keys: {string.Join(",", x?.Keys ?? Enumerable.Empty<string>())}");

                try
                {
                    itemLogger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                        nameof(ProcessInvoiceItem), $"Processing invoice item with keys: {string.Join(",", x?.Keys ?? Enumerable.Empty<string>())}");

                    var invoice = new ShipmentInvoice();

                    if (x["InvoiceDetails"] == null)
                    {
                        itemLogger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(ProcessInvoiceItem), "Validation", "InvoiceDetails is null, skipping item.", "");
                        itemMethodStopwatch.Stop(); // Stop stopwatch
                        itemLogger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                            nameof(ProcessInvoiceItem), "Skipped item due to null InvoiceDetails", "Returned null", itemMethodStopwatch.ElapsedMilliseconds);
                        itemLogger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                            nameof(ProcessInvoiceItem), "Skipped item due to null InvoiceDetails", "Returned null", itemMethodStopwatch.ElapsedMilliseconds);
                        return null; // throw new ApplicationException("Template Details is null");
                    }

                    var items = ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                        .Where(z => z != null)
                        .Where(z => z.ContainsKey("ItemDescription"))
                        .Select(z => (
                            ItemNumber: z["ItemNumber"]?.ToString(),
                            ItemDescription: z["ItemDescription"].ToString(),
                            TariffCode: x.ContainsKey("TariffCode") ? x["TariffCode"]?.ToString() : ""
                        )).ToList();

                    if (!items.Any())
                    {
                         itemLogger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                            nameof(ProcessInvoiceItem), "ItemExtraction", "ExtractedItems", "No valid items found within InvoiceDetails.");
                    }

                    itemLogger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "InventoryItemsExService.ClassifiedItms", "ASYNC_EXPECTED");
                    var classifiedStopwatch = Stopwatch.StartNew();
                    var classifiedItms = await InventoryItemsExService.ClassifiedItms(items, itemLogger).ConfigureAwait(false); // Pass logger
                    classifiedStopwatch.Stop();
                    itemLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "InventoryItemsExService.ClassifiedItms", classifiedStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                    invoice.ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;
                    invoice.InvoiceNo = x.ContainsKey("InvoiceNo") && x["InvoiceNo"] != null ? x["InvoiceNo"].ToString().Truncate(50) : "Unknown";
                    invoice.PONumber = x.ContainsKey("PONumber") && x["PONumber"] != null ? x["PONumber"].ToString() : null;
                    invoice.InvoiceDate = x.ContainsKey("InvoiceDate") ? DateTime.Parse(x["InvoiceDate"].ToString()) : DateTime.MinValue;
                    invoice.InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null;
                    invoice.SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null;
                    invoice.ImportedLines = !x.ContainsKey("InvoiceDetails") ? 0 : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).Count;
                    invoice.SupplierCode = x.ContainsKey("SupplierCode") ? x["SupplierCode"]?.ToString() : null;
                    invoice.SupplierName = (x.ContainsKey("SupplierName") ? x["SupplierName"]?.ToString() : null) ?? (x.ContainsKey("SupplierCode") ? x["SupplierCode"]?.ToString() : null);
                    invoice.SupplierAddress = x.ContainsKey("SupplierAddress") ? x["SupplierAddress"]?.ToString() : null;
                    invoice.SupplierCountry = x.ContainsKey("SupplierCountryCode") ? x["SupplierCountryCode"]?.ToString() : null;
                    invoice.FileLineNumber = itms.IndexOf(x) + 1;
                    invoice.Currency = x.ContainsKey("Currency") ? x["Currency"].ToString() : null;
                    invoice.TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null;
                    invoice.TotalOtherCost = x.ContainsKey("TotalOtherCost") ? Convert.ToDouble(x["TotalOtherCost"].ToString()) : (double?)null;
                    invoice.TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null;
                    invoice.TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null;

                    itemLogger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ProcessInvoiceDetails", "SYNC_EXPECTED");
                    var processDetailsStopwatch = Stopwatch.StartNew();
                    invoice.InvoiceDetails = ProcessInvoiceDetails(x, classifiedItms);
                    processDetailsStopwatch.Stop();
                    itemLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ProcessInvoiceDetails", processDetailsStopwatch.ElapsedMilliseconds, "Sync call returned");

                    itemLogger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ProcessExtraInfo", "SYNC_EXPECTED");
                    var processExtraInfoStopwatch = Stopwatch.StartNew();
                    invoice.InvoiceExtraInfo = ProcessExtraInfo(x);
                    processExtraInfoStopwatch.Stop();
                    itemLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ProcessExtraInfo", processExtraInfoStopwatch.ElapsedMilliseconds, "Sync call returned");

                    invoice.EmailId = emailId;
                    invoice.SourceFile = droppedFilePath;
                    invoice.FileTypeId = fileType.Id;
                    invoice.TrackingState = TrackingState.Added;

                    itemMethodStopwatch.Stop(); // Stop stopwatch on success
                    itemLogger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(ProcessInvoiceItem), "Invoice item processed successfully", $"InvoiceNo: {invoice.InvoiceNo}, ItemCount: {invoice.InvoiceDetails.Count}", itemMethodStopwatch.ElapsedMilliseconds);
                    itemLogger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ProcessInvoiceItem), $"Successfully processed invoice item for InvoiceNo: {invoice.InvoiceNo}", itemMethodStopwatch.ElapsedMilliseconds);

                    return invoice;
                }
                catch (Exception e)
                {
                    itemMethodStopwatch.Stop(); // Stop stopwatch on failure
                    itemLogger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessInvoiceItem), "Process a single invoice item dictionary", itemMethodStopwatch.ElapsedMilliseconds, $"Error processing invoice item: {e.Message}");
                    itemLogger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessInvoiceItem), "Invoice item processing", itemMethodStopwatch.ElapsedMilliseconds, $"Error processing invoice item: {e.Message}");
                    Console.WriteLine(e); // Keep Console.WriteLine for now
                    throw;
                }
            }

            List<InvoiceDetails> ProcessInvoiceDetails(IDictionary<string, object> x, Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> classifiedItms)
            {
                if (!x.ContainsKey("InvoiceDetails"))
                    return new List<InvoiceDetails>();

                return ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                    .Where(z => z != null)
                    .Where(z => z.ContainsKey("ItemDescription") && z["ItemDescription"] != null)
                    .Select(z =>
                    {
                        var details = new InvoiceDetails();
                        var qty = z.ContainsKey("Quantity") ? Convert.ToDouble(z["Quantity"].ToString()) : 1;
                        details.Quantity = qty;

                        var classifiedItm = classifiedItms.ContainsKey(z["ItemDescription"].ToString())
                            ? classifiedItms[z["ItemDescription"].ToString()]
                            : (
                                ItemNumber: z.ContainsKey("ItemNumber") ? z["ItemNumber"].ToString().ToUpper().Truncate(20) : null,
                                ItemDescription: z["ItemDescription"].ToString().Truncate(255),
                                TariffCode: z.ContainsKey("TariffCode") ? z["TariffCode"].ToString().ToUpper().Truncate(20) : null,
                                Category: string.Empty,
                                CategoryTariffCode: string.Empty
                            );

                        var dbCategoryTariffs = BaseDataModel.Instance.CategoryTariffs.FirstOrDefault(x => x.Category == classifiedItm.Category);

                        details.ItemNumber = classifiedItm.ItemNumber;
                        details.ItemDescription = classifiedItm.ItemDescription.Truncate(255);
                        details.TariffCode = classifiedItm.TariffCode;
                        details.Category = dbCategoryTariffs?.Category ?? classifiedItm.Category;
                        details.CategoryTariffCode = dbCategoryTariffs?.TariffCode ?? classifiedItm.CategoryTariffCode;
                        details.Units = z.ContainsKey("Units") ? z["Units"].ToString() : null;
                        details.Cost = z.ContainsKey("Cost")
                            ? Convert.ToDouble(z["Cost"].ToString())
                            : Convert.ToDouble(z["TotalCost"].ToString()) / (qty == 0 ? 1 : qty);
                        details.TotalCost = z.ContainsKey("TotalCost")
                            ? Convert.ToDouble(z["TotalCost"].ToString())
                            : Convert.ToDouble(z["Cost"].ToString()) * qty;
                        details.Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0;
                        details.Volume = z.ContainsKey("Gallons")
                            ? new InvoiceDetailsVolume()
                            {
                                Quantity = Convert.ToDouble(z["Gallons"].ToString()),
                                Units = "Gallons",
                                TrackingState = TrackingState.Added
                            }
                            : null;
                        details.SalesFactor = (z.ContainsKey("SalesFactor") && z.ContainsKey("Units") && z["Units"].ToString() != "EA")
                            || (z.ContainsKey("SalesFactor") && !z.ContainsKey("Units"))
                            ? Convert.ToInt32(z["SalesFactor"].ToString())
                            : 1;
                        details.LineNumber = z.ContainsKey("Instance")
                            ? Convert.ToInt32(
                                z["Instance"].ToString().Contains("-")
                                ? z["Instance"].ToString().Split('-')[1]
                                : z["Instance"].ToString())
                            : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).IndexOf(z) + 1;
                        details.FileLineNumber = z.ContainsKey("FileLineNumber") ? Convert.ToInt32(z["FileLineNumber"].ToString()) : -1;
                        details.Section = z.ContainsKey("Section") ? z["Section"].ToString() : null;
                        details.InventoryItemId = z.ContainsKey("InventoryItemId") ? (int)z["InventoryItemId"] : (int?)null;
                        details.TrackingState = TrackingState.Added;
                        return details;
                    }).ToList();
            }

            List<InvoiceExtraInfo> ProcessExtraInfo(IDictionary<string, object> x)
            {
                if (!x.ContainsKey("ExtraInfo"))
                    return new List<InvoiceExtraInfo>();

                return ((List<IDictionary<string, object>>)x["ExtraInfo"])
                    .Where(z => z.Keys.Any())
                    .SelectMany(z => z)
                    .Where(z => z.Value != null)
                    .Select(z => new InvoiceExtraInfo
                    {
                        Info = z.Key.ToString(),
                        Value = z.Value.ToString(),
                        TrackingState = TrackingState.Added
                    }).ToList();
            }
        }




        private static async Task SaveInvoicePOsAsync(Dictionary<string, string> invoicePOs, List<ShipmentInvoice> lst)
        {
            using (var ctx = new EntryDataDSContext())
            {
                // 1. Collect all details and ensure categories exist
                var allDetails = lst.SelectMany(inv => inv.InvoiceDetails).ToList();

                if (allDetails.Any())
                {
                    await EnsureCategoriesExistAsync(ctx, allDetails).ConfigureAwait(false);
                }

                // 2. Process invoices - much simpler now!
                foreach (var invoice in lst)
                {
                    var existingManifest = ctx.ShipmentInvoice.FirstOrDefault(x => x.InvoiceNo == invoice.InvoiceNo);
                    if (existingManifest != null)
                    {
                        ctx.ShipmentInvoice.Remove(existingManifest);
                    }

                    invoice.InvoiceDetails = AutoFixImportErrors(invoice);
                    invoice.ImportedLines = invoice.InvoiceDetails.Count;

                    if (Math.Abs(invoice.SubTotal.GetValueOrDefault()) < 0.01)
                    {
                        invoice.SubTotal = invoice.ImportedSubTotal;
                    }

                    if (!invoice.InvoiceDetails.Any())
                        continue;

                    if (invoicePOs != null && lst.Count > 1 && invoice != lst.First())
                    {
                        invoice.SourceFile = invoice.SourceFile.Replace($"{invoicePOs[lst.First().InvoiceNo]}",
                            invoicePOs[invoice.InvoiceNo]);
                    }

                    // 3. Simply normalize the category data - no foreign key complexity!
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        if (!string.IsNullOrEmpty(detail.Category))
                        {
                            detail.Category = detail.Category.ToUpperInvariant().Trim();
                        }
                        if (!string.IsNullOrEmpty(detail.CategoryTariffCode))
                        {
                            detail.CategoryTariffCode = detail.CategoryTariffCode.ToUpperInvariant().Trim();
                        }
                    }

                    ctx.ShipmentInvoice.Add(invoice);
                }

                // 4. Save everything - fast and simple!
                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task EnsureCategoriesExistAsync(EntryDataDSContext ctx, ICollection<InvoiceDetails> details)
        {
            if (details == null || !details.Any()) return;

            // Extract unique (Category, TariffCode) combinations
            var categoryTariffCombinations = details
                .Where(d => !string.IsNullOrEmpty(d.Category) && !string.IsNullOrEmpty(d.CategoryTariffCode))
                .Select(d => new {
                    Category = d.Category.ToUpperInvariant().Trim(),
                    TariffCode = d.CategoryTariffCode.ToUpperInvariant().Trim()
                })
                .GroupBy(x => new { x.Category, x.TariffCode })
                .Select(g => g.Key)
                .ToList();

            if (!categoryTariffCombinations.Any()) return;

            // Use MERGE statement to handle upserts at the database level
            foreach (var combo in categoryTariffCombinations)
            {
                try
                {
                    var sql = @"
                MERGE CategoryTariffs AS target
                USING (SELECT @Category AS Category, @TariffCode AS TariffCode) AS source
                ON target.Category = source.Category AND target.TariffCode = source.TariffCode
                WHEN NOT MATCHED THEN
                    INSERT (Category, TariffCode)
                    VALUES (source.Category, source.TariffCode);";

                    await ctx.Database.ExecuteSqlCommandAsync(sql,
                        new System.Data.SqlClient.SqlParameter("@Category", combo.Category),
                        new System.Data.SqlClient.SqlParameter("@TariffCode", combo.TariffCode))
                        .ConfigureAwait(false);
                }
                catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    // Duplicate key - record already exists, continue
                    Console.WriteLine($"CategoryTariff already exists: ({combo.Category}, {combo.TariffCode})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error ensuring CategoryTariff exists for ({combo.Category}, {combo.TariffCode}): {ex.Message}");
                    // Continue processing other combinations
                }
            }
        }


        private List<ShipmentInvoice> ReduceLstByInvoiceNo(List<ShipmentInvoice> lstData)
        {
            var res = new List<ShipmentInvoice>();

            foreach (var grp in lstData.GroupBy(x => x.InvoiceNo))
            {
                var rinvoice = grp.FirstOrDefault(x => grp.Count() <= 1 || (string.IsNullOrEmpty(x.PONumber) && x.InvoiceNo == x.PONumber) || x.InvoiceNo != x.PONumber);
                if (rinvoice == null) continue;
                foreach (var invoice in grp.Where(x => grp.Count() > 1 && x.InvoiceNo != x.PONumber && x.InvoiceNo != rinvoice.InvoiceNo))
                {
                    rinvoice.InvoiceDetails.AddRange(invoice.InvoiceDetails);
                    rinvoice.PONumber = invoice.PONumber; // don't think this makes a difference
                }
                res.Add(rinvoice);
            }

            return res;
        }

        private static List<InvoiceDetails> AutoFixImportErrors(ShipmentInvoice invoice)
        {
            var details = invoice.InvoiceDetails;

            foreach (var err in details.Where(x =>x.TotalCost.GetValueOrDefault() > 0 && Math.Abs(((x.Quantity * x.Cost) - x.Discount.GetValueOrDefault()) - x.TotalCost.GetValueOrDefault()) > 0.01))
            {

            }

            //if (invoice.SubTotal > 0
            //    && Math.Abs((double) (invoice.SubTotal - details.Sum(x => x.Cost * x.Quantity))) > 0.01)
            var secList = details.GroupBy(x => x.Section).ToList();
            var lst = new List<InvoiceDetails>();
            foreach (var section in secList)
            {
                foreach (var detail in section)
                {
                    if (lst.Any(x =>
                            x.TotalCost == detail.TotalCost && x.ItemNumber == detail.ItemNumber &&
                            x.Quantity == detail.Quantity && x.Section != detail.Section)) continue;
                    lst.Add(detail);
                }
            }

            //details = details.DistinctBy(x => new { ItemNumber = x.ItemNumber.ToUpper(), x.Quantity, x.TotalCost})
            //    .ToList();


            //  var invoiceInvoiceDetails = lst ;
            return lst;
        }
    }
}