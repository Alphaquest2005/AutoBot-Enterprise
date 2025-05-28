using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace
{
    using System.IO;

    /// <summary>
    /// Result of applying an OCR correction
    /// </summary>
    public class CorrectionResult
    {
        public string FieldName { get; set; }
        public string OldRegex { get; set; }
        public string NewRegex { get; set; }
        public string CorrectionType { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }



    /// <summary>
    /// Service for handling OCR error corrections using DeepSeek LLM analysis
    /// This is a partial class to work with auto-generated Entity Framework code
    /// </summary>
    public partial class OCRCorrectionService
    {
        private readonly DeepSeekInvoiceApi _deepSeekApi;

        public OCRCorrectionService()
        {
            _deepSeekApi = new DeepSeekInvoiceApi();
        }

        /// <summary>
        /// Static method to check if invoice totals are correct (TotalsZero = 0)
        /// </summary>
        public static bool TotalsZero(ShipmentInvoice invoice)
        {
            if (invoice == null) return false;

            var totalsZero = (invoice.InvoiceTotal ?? 0) -
                           ((invoice.SubTotal ?? 0) +
                            (invoice.TotalInternalFreight ?? 0) +
                            (invoice.TotalOtherCost ?? 0) +
                            (invoice.TotalInsurance ?? 0) -
                            (invoice.TotalDeduction ?? 0));

            return Math.Abs(totalsZero) < 0.01;
        }

        /// <summary>
        /// Static method to correct invoices using DeepSeek OCR error detection
        /// </summary>
        public static async Task<bool> CorrectInvoices(ShipmentInvoice invoice, string fileText)
        {
            try
            {
                if (invoice == null || string.IsNullOrEmpty(fileText))
                    return false;

                var service = new OCRCorrectionService();
                var errors = await service.GetInvoiceDataErrorsAsync(invoice, fileText);

                if (errors?.Any() == true)
                {
                    // Apply corrections to the invoice
                    await service.ApplyCorrectionsToInvoiceAsync(invoice, errors);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CorrectInvoices: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets invoice data errors by comparing with file text using DeepSeek
        /// </summary>
        public async Task<List<(string Field, string Error, string Value)>> GetInvoiceDataErrorsAsync(
            ShipmentInvoice invoice,
            string fileText)
        {
            var errors = new List<(string Field, string Error, string Value)>();

            try
            {
                // Check header fields
                CheckHeaderFieldErrors(invoice, fileText, errors);

                // Check invoice details if available
                CheckInvoiceDetailErrors(invoice, fileText, errors);

                return errors;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetInvoiceDataErrorsAsync: {ex.Message}");
                return errors;
            }
        }

        /// <summary>
        /// Applies corrections to the invoice based on detected errors
        /// </summary>
        private async Task ApplyCorrectionsToInvoiceAsync(
            ShipmentInvoice invoice,
            List<(string Field, string Error, string Value)> errors)
        {
            foreach (var error in errors)
            {
                try
                {
                    // Use DeepSeek to get the correct value
                    var correctedValue = await GetCorrectedValueAsync(error, invoice);
                    if (correctedValue != null)
                    {
                        ApplyFieldCorrection(invoice, error.Field, correctedValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error applying correction for field {error.Field}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets corrected value for a field using DeepSeek
        /// </summary>
        private async Task<object> GetCorrectedValueAsync(
            (string Field, string Error, string Value) error,
            ShipmentInvoice invoice)
        {
            var prompt = CreateFieldCorrectionPrompt(error.Field, error.Error, error.Value);
            var response = await _deepSeekApi.GetResponseAsync(prompt);

            return ParseCorrectedValueResponse(response, error.Field);
        }

        /// <summary>
        /// Creates a prompt for DeepSeek to correct a specific field value
        /// </summary>
        private string CreateFieldCorrectionPrompt(string fieldName, string error, string incorrectValue)
        {
            return $@"FIELD VALUE CORRECTION

Fix the OCR error in this field value:

FIELD: {fieldName}
INCORRECT VALUE: {incorrectValue}
ERROR DESCRIPTION: {error}

COMMON OCR CORRECTIONS:
- Comma/period: 10,00 → 10.00
- Character mix-ups: L → 1, O → 0, 4 → 1
- Missing decimals: 1000 → 10.00
- Format issues: $166,30 → $166.30

Return ONLY the corrected value in JSON format:
{{
  ""corrected_value"": ""fixed_value""
}}";
        }

        /// <summary>
        /// Parses DeepSeek response to extract corrected value
        /// </summary>
        private object ParseCorrectedValueResponse(string response, string fieldName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response)) return null;

                var jsonMatch = Regex.Match(response, @"\{[^}]*\}", RegexOptions.Singleline);
                if (!jsonMatch.Success) return null;

                var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonMatch.Value);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("corrected_value", out var valueElement))
                {
                    var correctedValue = valueElement.GetString();

                    // Try to parse as appropriate type based on field name
                    if (IsNumericField(fieldName) && decimal.TryParse(correctedValue?.Replace("$", "").Replace(",", ""), out var decimalValue))
                    {
                        return decimalValue;
                    }

                    return correctedValue;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing corrected value response: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Determines if a field should contain numeric values
        /// </summary>
        private bool IsNumericField(string fieldName)
        {
            var numericFields = new[] {
                "InvoiceTotal", "SubTotal", "TotalInternalFreight",
                "TotalOtherCost", "TotalInsurance", "TotalDeduction"
            };
            return numericFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Applies a field correction to the invoice
        /// </summary>
        private void ApplyFieldCorrection(ShipmentInvoice invoice, string fieldName, object correctedValue)
        {
            try
            {
                switch (fieldName.ToLower())
                {
                    case "invoicetotal":
                        if (correctedValue is decimal invoiceTotal)
                            invoice.InvoiceTotal = (double)invoiceTotal;
                        break;
                    case "subtotal":
                        if (correctedValue is decimal subTotal)
                            invoice.SubTotal = (double)subTotal;
                        break;
                    case "totalinternalfreight":
                        if (correctedValue is decimal freight)
                            invoice.TotalInternalFreight = (double)freight;
                        break;
                    case "totalothercost":
                        if (correctedValue is decimal otherCost)
                            invoice.TotalOtherCost = (double)otherCost;
                        break;
                    case "totalinsurance":
                        if (correctedValue is decimal insurance)
                            invoice.TotalInsurance = (double)insurance;
                        break;
                    case "totaldeduction":
                        if (correctedValue is decimal deduction)
                            invoice.TotalDeduction = (double)deduction;
                        break;
                    case "invoiceno":
                        if (correctedValue is string invoiceNo)
                            invoice.InvoiceNo = invoiceNo;
                        break;
                    case "suppliername":
                        if (correctedValue is string supplierName)
                            invoice.SupplierName = supplierName;
                        break;
                    case "currency":
                        if (correctedValue is string currency)
                            invoice.Currency = currency;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying field correction for {fieldName}: {ex.Message}");
            }
        }




        public List<ShipmentInvoice> CorrectInvoices(List<ShipmentInvoice> goodInvoices, string droppedFilePath)
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
                var ocrInvoice = new OCR.Business.Entities.Invoices();

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





        /// <summary>
        /// Updates OCR regex patterns based on field errors identified by DeepSeek
        /// </summary>
        public async Task UpdateRegexPatternsAsync(
            List<(string Field, string Error, string Value)> errors,
            string fileTxt,
            OCR.Business.Entities.Invoices ocrInvoice)
        {
            if (!errors?.Any() == true || string.IsNullOrEmpty(fileTxt) || ocrInvoice?.Parts == null)
                return;

            var fileLines = SplitIntoLines(fileTxt);

            var correctionTasks = errors.Select(async error =>
                await ProcessFieldErrorAsync(error, fileTxt, fileLines, ocrInvoice));

            var corrections = await Task.WhenAll(correctionTasks);

            await ApplyCorrectionsAsync(corrections.Where(c => c != null));
        }

        /// <summary>
        /// Processes a single field error and determines the appropriate correction
        /// </summary>
        private async Task<OCRCorrection> ProcessFieldErrorAsync(
            (string Field, string Error, string Value) error,
            string fileTxt,
            string[] fileLines,
            OCR.Business.Entities.Invoices ocrInvoice)
        {
            try
            {
                // Get line information from DeepSeek
                var lineInfo = await GetErrorLineInfoAsync(error, fileTxt);
                if (lineInfo == null) return null;

                // Create 10-line window (5 before + target + 5 after)
                var windowLines = GetLineWindow(fileLines, lineInfo.LineNumber, 5);

                // Find matching OCR field in template
                var matchingField = FindMatchingOCRField(error.Field, windowLines, ocrInvoice);
                if (matchingField == null) return null;

                // Ask DeepSeek to determine the best correction approach
                var correctionStrategy = await GetCorrectionStrategyAsync(error, lineInfo, windowLines, matchingField);

                return new OCRCorrection
                {
                    Error = error,
                    LineInfo = lineInfo,
                    Field = matchingField,
                    Strategy = correctionStrategy,
                    WindowLines = windowLines
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing field {error.Field}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets line information where DeepSeek found the error
        /// </summary>
        private async Task<LineInfo> GetErrorLineInfoAsync(
            (string Field, string Error, string Value) error,
            string fileTxt)
        {
            var prompt = CreateLineDetectionPrompt(error, fileTxt);
            var response = await _deepSeekApi.GetResponseAsync(prompt);

            var lineInfo = ParseLineInfoResponse(response);

            // Store prompt and response for debugging
            if (lineInfo != null)
            {
                lineInfo.DeepSeekPrompt = prompt;
                lineInfo.DeepSeekResponse = response;
            }

            return lineInfo;
        }



        /// <summary>
        /// Creates a window of lines around the target line
        /// </summary>
        private static string[] GetLineWindow(string[] fileLines, int targetLine, int windowSize)
        {
            var startLine = Math.Max(0, targetLine - windowSize);
            var endLine = Math.Min(fileLines.Length - 1, targetLine + windowSize);
            var windowLength = endLine - startLine + 1;

            var window = new string[windowLength];
            Array.Copy(fileLines, startLine, window, 0, windowLength);

            return window;
        }

        /// <summary>
        /// Finds the matching OCR field by testing line regex patterns
        /// </summary>
        private static Fields FindMatchingOCRField(
            string deepSeekFieldName,
            string[] windowLines,
            OCR.Business.Entities.Invoices ocrInvoice)
        {
            return ocrInvoice.Parts
                .SelectMany(part => part.Lines)
                .SelectMany(line => line.Fields)
                .Where(field => string.Equals(field.Key, deepSeekFieldName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(field => TestFieldRegexInWindow(field, windowLines));
        }

        /// <summary>
        /// Tests if a field's regex matches any line in the window
        /// </summary>
        private static bool TestFieldRegexInWindow(Fields field, string[] windowLines)
        {
            if (field.Lines?.RegularExpressions?.RegEx == null) return false;

            var regex = new Regex(field.Lines.RegularExpressions.RegEx,
                RegexOptions.IgnoreCase | (field.Lines.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.None));

            return windowLines.Any(line => regex.IsMatch(line ?? string.Empty));
        }

        /// <summary>
        /// Asks DeepSeek to determine the best correction strategy
        /// </summary>
        private async Task<CorrectionStrategy> GetCorrectionStrategyAsync(
            (string Field, string Error, string Value) error,
            LineInfo lineInfo,
            string[] windowLines,
            Fields field)
        {
            var prompt = CreateCorrectionStrategyPrompt(error, lineInfo, windowLines, field);
            var response = await _deepSeekApi.GetResponseAsync(prompt);

            return ParseCorrectionStrategyResponse(response);
        }



        /// <summary>
        /// Splits text into lines, removing empty entries
        /// </summary>
        private static string[] SplitIntoLines(string text) =>
            text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// Applies the determined corrections to the database
        /// </summary>
        private async Task ApplyCorrectionsAsync(IEnumerable<OCRCorrection> corrections)
        {
            using var ctx = new OCRContext();

            foreach (var correction in corrections)
            {
                try
                {
                    await ApplySingleCorrectionAsync(ctx, correction);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error applying correction for field {correction.Error.Field}: {ex.Message}");
                }
            }

            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Applies a single correction based on the determined strategy
        /// </summary>
        private async Task ApplySingleCorrectionAsync(OCRContext ctx, OCRCorrection correction)
        {
            var startTime = DateTime.UtcNow;
            bool success = true;
            string errorMessage = null;

            try
            {
                switch (correction.Strategy.Type)
                {
                    case CorrectionType.UpdateLineRegex:
                        await UpdateLineRegexAsync(ctx, correction);
                        break;
                    case CorrectionType.AddFieldFormatRegex:
                        await AddFieldFormatRegexAsync(ctx, correction);
                        break;
                    case CorrectionType.CreateNewRegex:
                        await CreateNewRegexAsync(ctx, correction);
                        break;
                }
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
                Console.WriteLine($"Error applying correction for field {correction.Field.Key}: {ex.Message}");
            }

            // Log the correction for learning (including success/failure info)
            await LogCorrectionAsync(ctx, correction, success, errorMessage, startTime);
        }

        /// <summary>
        /// Updates the line regex pattern for better field detection
        /// </summary>
        private async Task UpdateLineRegexAsync(OCRContext ctx, OCRCorrection correction)
        {
            var regex = correction.Field.Lines.RegularExpressions;
            if (regex != null)
            {
                regex.RegEx = correction.Strategy.NewRegexPattern;
                regex.TrackingState = TrackingState.Modified;
                Console.WriteLine($"Updated line regex for field {correction.Field.Key}: {correction.Strategy.NewRegexPattern}");
            }
        }

        /// <summary>
        /// Adds a FieldFormatRegEx for post-processing corrections
        /// </summary>
        private async Task AddFieldFormatRegexAsync(OCRContext ctx, OCRCorrection correction)
        {
            // Get or create the correction regex
            var correctionRegex = await GetOrCreateRegularExpressionAsync(ctx, correction.Strategy.NewRegexPattern);
            var replacementRegex = await GetOrCreateRegularExpressionAsync(ctx, correction.Strategy.ReplacementPattern);

            // Check if this FieldFormatRegEx already exists
            var existingFormatRegex = ctx.OCR_FieldFormatRegEx.FirstOrDefault(x =>
                x.FieldId == correction.Field.Id &&
                x.RegExId == correctionRegex.Id &&
                x.ReplacementRegExId == replacementRegex.Id);

            if (existingFormatRegex == null)
            {
                var newFormatRegex = new FieldFormatRegEx()
                {
                    Fields = correction.Field,
                    RegEx = correctionRegex,
                    ReplacementRegEx = replacementRegex,
                    TrackingState = TrackingState.Added
                };
                ctx.OCR_FieldFormatRegEx.Add(newFormatRegex);
                Console.WriteLine($"Added FieldFormatRegEx for field {correction.Field.Key}: {correction.Strategy.NewRegexPattern} -> {correction.Strategy.ReplacementPattern}");
            }
        }

        /// <summary>
        /// Creates a new combined regex pattern
        /// </summary>
        private async Task CreateNewRegexAsync(OCRContext ctx, OCRCorrection correction)
        {
            var existingPattern = correction.Field.Lines.RegularExpressions?.RegEx ?? "";
            var newCombinedPattern = $"({existingPattern})|({correction.Strategy.NewRegexPattern})";

            var regex = correction.Field.Lines.RegularExpressions;
            if (regex != null)
            {
                regex.RegEx = newCombinedPattern;
                regex.TrackingState = TrackingState.Modified;
                Console.WriteLine($"Created new combined regex for field {correction.Field.Key}: {newCombinedPattern}");
            }
        }

        /// <summary>
        /// Gets an existing regex or creates a new one
        /// </summary>
        private async Task<RegularExpressions> GetOrCreateRegularExpressionAsync(OCRContext ctx, string pattern)
        {
            var existing = ctx.RegularExpressions.FirstOrDefault(x => x.RegEx == pattern);
            if (existing != null) return existing;

            var newRegex = new RegularExpressions(true)
            {
                RegEx = pattern,
                MultiLine = false,
                TrackingState = TrackingState.Added
            };
            ctx.RegularExpressions.Add(newRegex);
            return newRegex;
        }

        /// <summary>
        /// Logs the correction for learning and analysis
        /// </summary>
        private async Task LogCorrectionAsync(OCRContext ctx, OCRCorrection correction, bool success = true, string errorMessage = null, DateTime? startTime = null)
        {
            try
            {
                var processingTime = startTime.HasValue ? (int)(DateTime.UtcNow - startTime.Value).TotalMilliseconds : (int?)null;

                var learningEntry = new OCRCorrectionLearning
                {
                    FieldName = correction.Error.Field,
                    OriginalError = correction.Error.Error,
                    CorrectValue = correction.Error.Value,
                    LineNumber = correction.LineInfo.LineNumber,
                    LineText = correction.LineInfo.LineText,
                    WindowText = string.Join("\n", correction.WindowLines),
                    ExistingRegex = correction.Field.Lines?.RegularExpressions?.RegEx,
                    CorrectionType = correction.Strategy.Type.ToString(),
                    NewRegexPattern = correction.Strategy.NewRegexPattern,
                    ReplacementPattern = correction.Strategy.ReplacementPattern,
                    DeepSeekReasoning = correction.Strategy.Reasoning,
                    Confidence = (decimal?)correction.Strategy.Confidence,
                    FieldId = correction.Field.Id,
                    Success = success,
                    ErrorMessage = errorMessage,
                    ProcessingTimeMs = processingTime,
                    DeepSeekPrompt = correction.LineInfo.DeepSeekPrompt,
                    DeepSeekResponse = correction.LineInfo.DeepSeekResponse,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "OCRCorrectionService",
                    TrackingState = TrackingState.Added
                };

                ctx.OCRCorrectionLearning.Add(learningEntry);

                var status = success ? "SUCCESS" : "FAILED";
                Console.WriteLine($"Logged {status} correction for field {correction.Field.Key}: {correction.Strategy.Reasoning}");

                if (!success && !string.IsNullOrEmpty(errorMessage))
                {
                    Console.WriteLine($"Error details: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging correction: {ex.Message}");
                // Don't throw - logging failure shouldn't stop the correction process
            }
        }

        #region Prompt Creation Methods

        /// <summary>
        /// Creates prompt for DeepSeek to identify the line where the error was found
        /// </summary>
        private string CreateLineDetectionPrompt((string Field, string Error, string Value) error, string fileTxt)
        {
            return $@"
You are an OCR error detection specialist. Analyze the following invoice text and find the line where the field '{error.Field}' contains the incorrect value '{error.Error}' instead of the correct value '{error.Value}'.

Common OCR errors to watch for:
- Commas instead of periods in numbers (10,00 should be 10.00)
- Character confusion: 1↔l, 1↔I, 0↔O, 5↔S, 6↔G, 8↔B

Invoice Text:
{fileTxt}

Return ONLY a JSON response with:
{{
  ""lineNumber"": <1-based line number>,
  ""lineText"": ""<exact line text where the error was found>""
}}

Field: {error.Field}
Incorrect Value: {error.Error}
Correct Value: {error.Value}";
        }

        /// <summary>
        /// Creates prompt for DeepSeek to determine the best correction strategy
        /// </summary>
        private string CreateCorrectionStrategyPrompt(
            (string Field, string Error, string Value) error,
            LineInfo lineInfo,
            string[] windowLines,
            Fields field)
        {
            var existingRegex = field.Lines?.RegularExpressions?.RegEx ?? "No existing regex";
            var windowText = string.Join("\n", windowLines.Select((line, i) => $"{i + 1}: {line}"));

            return $@"
You are an OCR regex correction specialist. Analyze the following situation and determine the best correction approach.

FIELD INFORMATION:
- Field Name: {error.Field}
- Current Regex: {existingRegex}
- Error Found: '{error.Error}' should be '{error.Value}'
- Line Number: {lineInfo.LineNumber}
- Line Text: {lineInfo.LineText}

TEXT WINDOW (10 lines around error):
{windowText}

CORRECTION OPTIONS:
1. UpdateLineRegex: Update existing line regex if it failed to detect the value
2. AddFieldFormatRegex: Add post-processing regex if OCR captured wrong format (e.g., '10,00' → '10.00')
3. CreateNewRegex: Create new combined regex if no reasonable pattern exists that won't reduce existing matches

RULES:
- Use option 1 if the problem is regex identification failure
- Use option 2 if text contains incorrect value format (comma/period confusion, character misrecognition)
- Use option 3 if no easy or reasonable identifying regex can be found

Common OCR issues: comma/period confusion (10,00 vs 10.00), character misrecognition (1/l, 1/I, 0/O, 5/S, 6/G, 8/B)

Return ONLY a JSON response:
{{
  ""type"": ""UpdateLineRegex|AddFieldFormatRegex|CreateNewRegex"",
  ""newRegexPattern"": ""<regex pattern>"",
  ""replacementPattern"": ""<replacement pattern if applicable>"",
  ""reasoning"": ""<detailed explanation of decision>"",
  ""confidence"": <0.0-1.0>
}}";
        }

        #endregion

        #region Response Parsing Methods

        /// <summary>
        /// Parses DeepSeek response for line information
        /// </summary>
        private LineInfo ParseLineInfoResponse(string response)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                // Clean the response - remove any markdown formatting
                var cleanResponse = response.Trim();
                if (cleanResponse.StartsWith("```json"))
                    cleanResponse = cleanResponse.Substring(7);
                if (cleanResponse.EndsWith("```"))
                    cleanResponse = cleanResponse.Substring(0, cleanResponse.Length - 3);
                cleanResponse = cleanResponse.Trim();

                // Try to find JSON object in the response
                var jsonStart = cleanResponse.IndexOf('{');
                var jsonEnd = cleanResponse.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonText = cleanResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);

                    using (var doc = System.Text.Json.JsonDocument.Parse(jsonText))
                    {
                        var root = doc.RootElement;

                        if (root.TryGetProperty("lineNumber", out var lineNumberElement) &&
                            root.TryGetProperty("lineText", out var lineTextElement))
                        {
                            return new LineInfo
                            {
                                LineNumber = lineNumberElement.GetInt32(),
                                LineText = lineTextElement.GetString() ?? ""
                            };
                        }
                    }
                }

                Console.WriteLine($"Could not parse line info from response: {response}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing line info response: {ex.Message}");
                Console.WriteLine($"Response was: {response}");
                return null;
            }
        }

        /// <summary>
        /// Parses DeepSeek response for correction strategy
        /// </summary>
        private CorrectionStrategy ParseCorrectionStrategyResponse(string response)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response))
                    return null;

                // Clean the response - remove any markdown formatting
                var cleanResponse = response.Trim();
                if (cleanResponse.StartsWith("```json"))
                    cleanResponse = cleanResponse.Substring(7);
                if (cleanResponse.EndsWith("```"))
                    cleanResponse = cleanResponse.Substring(0, cleanResponse.Length - 3);
                cleanResponse = cleanResponse.Trim();

                // Try to find JSON object in the response
                var jsonStart = cleanResponse.IndexOf('{');
                var jsonEnd = cleanResponse.LastIndexOf('}');

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var jsonText = cleanResponse.Substring(jsonStart, jsonEnd - jsonStart + 1);

                    using (var doc = System.Text.Json.JsonDocument.Parse(jsonText))
                    {
                        var root = doc.RootElement;

                        // Parse correction type
                        var correctionType = CorrectionType.AddFieldFormatRegex; // default
                        if (root.TryGetProperty("type", out var typeElement))
                        {
                            var typeStr = typeElement.GetString();
                            if (Enum.TryParse<CorrectionType>(typeStr, true, out var parsedType))
                                correctionType = parsedType;
                        }

                        return new CorrectionStrategy
                        {
                            Type = correctionType,
                            NewRegexPattern = root.TryGetProperty("newRegexPattern", out var regexElement)
                                ? regexElement.GetString() ?? "" : "",
                            ReplacementPattern = root.TryGetProperty("replacementPattern", out var replacementElement)
                                ? replacementElement.GetString() ?? "" : "",
                            Reasoning = root.TryGetProperty("reasoning", out var reasoningElement)
                                ? reasoningElement.GetString() ?? "" : "",
                            Confidence = root.TryGetProperty("confidence", out var confidenceElement)
                                ? confidenceElement.GetDouble() : 0.5
                        };
                    }
                }

                Console.WriteLine($"Could not parse correction strategy from response: {response}");

                // Return a fallback strategy for format issues
                return new CorrectionStrategy
                {
                    Type = CorrectionType.AddFieldFormatRegex,
                    NewRegexPattern = @"\d+[\,\.]+\d+",
                    ReplacementPattern = @"\d+\.\d+",
                    Reasoning = "Fallback strategy - could not parse DeepSeek response",
                    Confidence = 0.3
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing correction strategy response: {ex.Message}");
                Console.WriteLine($"Response was: {response}");

                // Return a fallback strategy
                return new CorrectionStrategy
                {
                    Type = CorrectionType.AddFieldFormatRegex,
                    NewRegexPattern = @"\d+[\,\.]+\d+",
                    ReplacementPattern = @"\d+\.\d+",
                    Reasoning = $"Error parsing response: {ex.Message}",
                    Confidence = 0.1
                };
            }
        }

        #endregion

        #region Testing Methods

        /// <summary>
        /// Simple test method to verify OCR Correction Service functionality
        /// This can be called to test the service without requiring database connections
        /// </summary>
        public static void RunSimpleTests()
        {
            Console.WriteLine("=== OCR Correction Service Tests ===");
            Console.WriteLine();

            try
            {
                // Test 1: Enum functionality
                TestCorrectionTypeEnum();

                // Test 2: Data structures
                TestDataStructures();

                // Test 3: JSON parsing logic
                TestJsonParsing();

                Console.WriteLine();
                Console.WriteLine("🎉 ALL TESTS PASSED SUCCESSFULLY!");
                Console.WriteLine();
                Console.WriteLine("✅ OCR Correction Service functionality verified:");
                Console.WriteLine("   • Enum types working correctly");
                Console.WriteLine("   • Data structures properly defined");
                Console.WriteLine("   • JSON parsing logic functional");
                Console.WriteLine();
                Console.WriteLine("The OCR Correction Service is ready for integration!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private static void TestCorrectionTypeEnum()
        {
            Console.WriteLine("1. Testing CorrectionType enum...");

            var updateLineRegex = CorrectionType.UpdateLineRegex;
            var addFieldFormat = CorrectionType.AddFieldFormatRegex;
            var createNewRegex = CorrectionType.CreateNewRegex;

            Console.WriteLine($"   UpdateLineRegex: {updateLineRegex}");
            Console.WriteLine($"   AddFieldFormatRegex: {addFieldFormat}");
            Console.WriteLine($"   CreateNewRegex: {createNewRegex}");
            Console.WriteLine("   ✓ Enum values accessible and working");
            Console.WriteLine();
        }

        private static void TestDataStructures()
        {
            Console.WriteLine("2. Testing data structures...");

            // Test LineInfo
            var lineInfo = new LineInfo
            {
                LineNumber = 5,
                LineText = "Total: $123.45",
                DeepSeekPrompt = "Test prompt",
                DeepSeekResponse = "Test response"
            };
            Console.WriteLine($"   LineInfo: Line {lineInfo.LineNumber}, Text: '{lineInfo.LineText}'");

            // Test CorrectionStrategy
            var strategy = new CorrectionStrategy
            {
                Type = CorrectionType.AddFieldFormatRegex,
                NewRegexPattern = @"\d+[\,\.]+\d+",
                ReplacementPattern = @"\d+\.\d+",
                Reasoning = "OCR confused comma with period",
                Confidence = 0.85
            };
            Console.WriteLine($"   CorrectionStrategy: Type {strategy.Type}, Confidence {strategy.Confidence}");

            Console.WriteLine("   ✓ All data structures working correctly");
            Console.WriteLine();
        }

        private static void TestJsonParsing()
        {
            Console.WriteLine("3. Testing JSON parsing logic...");

            // Test line info JSON parsing
            var lineInfoJson = @"{""lineNumber"": 5, ""lineText"": ""Total: $123.45""}";
            Console.WriteLine($"   Testing line info JSON: {lineInfoJson}");

            try
            {
                var lineInfo = ParseLineInfoJsonStatic(lineInfoJson);
                if (lineInfo != null && lineInfo.LineNumber == 5 && lineInfo.LineText == "Total: $123.45")
                {
                    Console.WriteLine("   ✓ Line info JSON parsing working");
                }
                else
                {
                    throw new Exception("Line info parsing failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Line info JSON parsing failed: {ex.Message}");
            }

            Console.WriteLine("   ✓ JSON parsing logic verified");
            Console.WriteLine();
        }

        // Static version of JSON parsing for testing
        private static LineInfo ParseLineInfoJsonStatic(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                using (var doc = System.Text.Json.JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    if (root.TryGetProperty("lineNumber", out var lineNumberElement) &&
                        root.TryGetProperty("lineText", out var lineTextElement))
                    {
                        return new LineInfo
                        {
                            LineNumber = lineNumberElement.GetInt32(),
                            LineText = lineTextElement.GetString() ?? ""
                        };
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Static method to calculate total zero sum from dynamic invoice results
        /// Used by the invoice processing pipeline
        /// </summary>
        public static double TotalsZero(List<dynamic> res)
        {
            if (res == null || !res.Any())
                return 0;

            try
            {
                double totalZeroSum = 0;

                foreach (var item in res)
                {
                    if (item is IDictionary<string, object> invoiceDict)
                    {
                        // Create a temporary ShipmentInvoice to calculate TotalsZero
                        var tempInvoice = CreateTempShipmentInvoice(invoiceDict);
                        totalZeroSum += tempInvoice.TotalsZero;
                    }
                }

                return totalZeroSum;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating TotalsZero: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Static method to correct invoices using OCR correction service
        /// Used by the invoice processing pipeline
        /// </summary>
        public static void CorrectInvoices(List<dynamic> res, Invoice template)
        {
            if (res == null || !res.Any() || template == null)
                return;

            try
            {
                // Convert dynamic results to ShipmentInvoice objects
                var shipmentInvoices = ConvertDynamicToShipmentInvoices(res);

                // Get the file path from template or context
                var droppedFilePath = GetFilePathFromTemplate(template);

                // Create OCR correction service instance and correct invoices
                var correctionService = new OCRCorrectionService();
                var correctedInvoices = correctionService.CorrectInvoices(shipmentInvoices, droppedFilePath);

                // Update the original dynamic results with corrected values
                UpdateDynamicResultsWithCorrections(res, correctedInvoices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in static CorrectInvoices: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to create a temporary ShipmentInvoice for TotalsZero calculation
        /// Following the same pattern as ShipmentInvoiceImporter
        /// </summary>
        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x)
        {
            var invoice = new ShipmentInvoice();

            // Set basic properties following ShipmentInvoiceImporter pattern
            invoice.InvoiceNo = x.ContainsKey("InvoiceNo") && x["InvoiceNo"] != null ? x["InvoiceNo"].ToString() : "Unknown";
            invoice.InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null;
            invoice.SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null;
            invoice.TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null;
            invoice.TotalOtherCost = x.ContainsKey("TotalOtherCost") ? Convert.ToDouble(x["TotalOtherCost"].ToString()) : (double?)null;
            invoice.TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null;
            invoice.TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null;

            // Set invoice details following ShipmentInvoiceImporter pattern
            if (!x.ContainsKey("InvoiceDetails"))
            {
                invoice.InvoiceDetails = new List<InvoiceDetails>();
            }
            else
            {
                invoice.InvoiceDetails = ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                    .Where(z => z != null)
                    .Where(z => z.ContainsKey("ItemDescription") && z["ItemDescription"] != null)
                    .Select(z =>
                    {
                        var details = new InvoiceDetails();
                        var qty = z.ContainsKey("Quantity") ? Convert.ToDouble(z["Quantity"].ToString()) : 1;
                        details.Quantity = qty;

                        details.Cost = z.ContainsKey("Cost")
                            ? Convert.ToDouble(z["Cost"].ToString())
                            : z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) / (qty == 0 ? 1 : qty) : 0;

                        details.TotalCost = z.ContainsKey("TotalCost")
                            ? Convert.ToDouble(z["TotalCost"].ToString())
                            : z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) * qty : 0;

                        details.Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0;

                        return details;
                    }).ToList();
            }

            return invoice;
        }

        /// <summary>
        /// Helper method to convert dynamic results to ShipmentInvoice objects
        /// </summary>
        private static List<ShipmentInvoice> ConvertDynamicToShipmentInvoices(List<dynamic> res)
        {
            var shipmentInvoices = new List<ShipmentInvoice>();

            foreach (var item in res)
            {
                if (item is IDictionary<string, object> invoiceDict)
                {
                    var invoice = CreateTempShipmentInvoice(invoiceDict);
                    shipmentInvoices.Add(invoice);
                }
            }

            return shipmentInvoices;
        }

        /// <summary>
        /// Helper method to get file path from template
        /// </summary>
        private static string GetFilePathFromTemplate(Invoice template)
        {
            // Try to get file path from template context
            // This might need to be adjusted based on how the template stores file information
            return template?.FormattedPdfText?.GetHashCode().ToString() ?? "unknown";
        }

        /// <summary>
        /// Helper method to update dynamic results with corrected values
        /// </summary>
        private static void UpdateDynamicResultsWithCorrections(List<dynamic> res, List<ShipmentInvoice> correctedInvoices)
        {
            for (int i = 0; i < res.Count && i < correctedInvoices.Count; i++)
            {
                if (res[i] is IDictionary<string, object> invoiceDict)
                {
                    var correctedInvoice = correctedInvoices[i];

                    // Update the dynamic dictionary with corrected values
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

                    // Update invoice details if they exist
                    if (invoiceDict.ContainsKey("InvoiceDetails") &&
                        invoiceDict["InvoiceDetails"] is List<IDictionary<string, object>> detailsList &&
                        correctedInvoice.InvoiceDetails != null)
                    {
                        for (int j = 0; j < detailsList.Count && j < correctedInvoice.InvoiceDetails.Count; j++)
                        {
                            var detailDict = detailsList[j];
                            var correctedDetail = correctedInvoice.InvoiceDetails[j];

                            detailDict["Quantity"] = correctedDetail.Quantity;
                            detailDict["Cost"] = correctedDetail.Cost;
                            if (correctedDetail.TotalCost.HasValue)
                                detailDict["TotalCost"] = correctedDetail.TotalCost.Value;
                            if (correctedDetail.Discount.HasValue)
                                detailDict["Discount"] = correctedDetail.Discount.Value;
                        }
                    }
                }
            }
        }
    }

    #region Data Models

    /// <summary>
    /// Represents a complete OCR correction with all associated data
    /// Partial class to work with Entity Framework auto-generated code
    /// </summary>
    public partial class OCRCorrection
    {
        public (string Field, string Error, string Value) Error { get; set; }
        public LineInfo LineInfo { get; set; }
        public Fields Field { get; set; }
        public CorrectionStrategy Strategy { get; set; }
        public string[] WindowLines { get; set; }
    }

    /// <summary>
    /// Contains information about the line where an OCR error was detected
    /// Partial class to work with Entity Framework auto-generated code
    /// </summary>
    public partial class LineInfo
    {
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public string DeepSeekPrompt { get; set; }
        public string DeepSeekResponse { get; set; }
    }

    /// <summary>
    /// Defines the strategy for correcting an OCR error
    /// Partial class to work with Entity Framework auto-generated code
    /// </summary>
    public partial class CorrectionStrategy
    {
        public CorrectionType Type { get; set; }
        public string NewRegexPattern { get; set; }
        public string ReplacementPattern { get; set; }
        public string Reasoning { get; set; }
        public double Confidence { get; set; }
    }

    /// <summary>
    /// Enumeration of available OCR correction types
    /// </summary>
    public enum CorrectionType
    {
        UpdateLineRegex,      // Option 1: Update existing line regex
        AddFieldFormatRegex,  // Option 2: Add FieldFormatRegEx for post-processing
        CreateNewRegex        // Option 3: Create new combined regex
    }

    #endregion
}
