﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common.Extensions;
using Polly;
using Polly.Retry;
using Serilog; // Added Serilog
using Serilog.Extensions;

namespace WaterNut.Business.Services.Utils
{
    public class DeepSeekInvoiceApi : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Serilog.ILogger _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public string PromptTemplate { get; set; }
        public string Model { get; set; } = "deepseek-chat";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 8192;
        public string HsCodePattern { get; set; } = @"\b\d{4}(?:[\.\-]\d{2,4})*\b";

        private static readonly HttpStatusCode[] HttpStatusCodesToRetry =
        {
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.GatewayTimeout
        };

        public DeepSeekInvoiceApi(HttpClient httpClient = null)
        {
            _apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
                                ?? throw new InvalidOperationException("API key not found in environment variables");

            _baseUrl = "https://api.deepseek.com/v1";
            _logger = Log.Logger.ForContext<DeepSeekInvoiceApi>(); // Use Serilog context logger
            _httpClient = httpClient ?? new HttpClient();

            ConfigureHttpClient();
            SetDefaultPrompts();

            _retryPolicy = CreateRetryPolicy(); // Call the new method
        }

        public DeepSeekInvoiceApi(Serilog.ILogger logger, HttpClient httpClient = null)
        {
            _apiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")
                                ?? throw new InvalidOperationException("API key not found in environment variables");

            _baseUrl = "https://api.deepseek.com/v1";
            _logger = logger ?? Log.Logger.ForContext<DeepSeekInvoiceApi>(); // Use provided logger or fallback to Serilog context logger
            _httpClient = httpClient ?? new HttpClient();

            ConfigureHttpClient();
            SetDefaultPrompts();

            _retryPolicy = CreateRetryPolicy(); // Call the new method
        }

        // --- New CreateRetryPolicy method based on DeepSeekApi.cs ---
        private AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
        {
            // Standard Exponential Backoff
            Func<int, TimeSpan> calculateDelay = (retryAttempt) =>
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                _logger.Verbose("Retry attempt {RetryCount}: Calculated delay {DelaySeconds}s", retryAttempt, delay.TotalSeconds);
                return delay;
            };

            // Exception-Specific Logging for Retries
            Action<DelegateResult<HttpResponseMessage>, TimeSpan> logRetryAction = (outcome, calculatedDelay) =>
            {
                var exception = outcome.Exception; // Exception that caused the retry
                var response = outcome.Result;     // Result if the delegate completed but triggered retry (e.g., status code)

                if (exception is RateLimitException rle)
                {
                    _logger.Warning(rle, "Retry needed due to Rate Limit (HTTP {StatusCode}). Delaying for {DelaySeconds}s...", rle.StatusCode, calculatedDelay.TotalSeconds);
                }
                else if (exception is TaskCanceledException tce)
                {
                    if (tce.CancellationToken.IsCancellationRequested)
                    {
                        _logger.Warning(tce, "Retry triggered for Task Cancellation (possibly user initiated). Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                    else
                    {
                        _logger.Warning(tce, "Retry needed due to operation Timeout. Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                }
                else if (exception is HttpRequestException httpEx) // Includes exceptions where Data["StatusCode"] might be set
                {
                    // Try to get status code from Data if available, otherwise log generic HttpRequestException
                    var statusCode = httpEx.Data.Contains("StatusCode") ? httpEx.Data["StatusCode"] : "(unknown)";
                    _logger.Warning(httpEx, "Retry needed due to HTTP Request Error (Code: {StatusCode}). Delaying for {DelaySeconds}s...", statusCode, calculatedDelay.TotalSeconds);
                }
                else if (response != null) // Handle retries triggered by OrResult (status code)
                {
                     _logger.Warning("Retry needed due to Response Status Code {StatusCode}. Delaying for {DelaySeconds}s...", response.StatusCode, calculatedDelay.TotalSeconds);
                }
                else // Default logging for other handled exceptions
                {
                    _logger.Warning(exception, "Retry needed due to handled Transient Error ({ExceptionType}). Delaying for {DelaySeconds}s...", exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
                }
            };

            // Build the Policy
            return Policy<HttpResponseMessage>
               .Handle<RateLimitException>()
               .Or<HttpRequestException>() // Handle general HttpRequestExceptions
               .Or<TaskCanceledException>(ex => !ex.CancellationToken.IsCancellationRequested) // Only retry timeouts, not user cancellations
               .OrResult(r => r.StatusCode >= HttpStatusCode.InternalServerError || HttpStatusCodesToRetry.Contains(r.StatusCode)) // Retry on 5xx errors or specific configured codes
               .WaitAndRetryAsync(
                   retryCount: 3,
                   sleepDurationProvider: calculateDelay,
                   onRetry: logRetryAction
               );
        }

        private void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-DeepSeek-Version", "2023-07-01");
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        private void SetDefaultPrompts()
        {
            PromptTemplate = @"DOCUMENT PROCESSING RULES:

0. PROCESS THIS TEXT INPUT:
{0}

1. TEXT STRUCTURE ANALYSIS:

   - Priority order:
     1. Item tables with prices/quantities
     2. Customs declaration forms
     3. Address blocks
     4. Payment/header sections

2. FIELD EXTRACTION GUIDANCE:
   - SupplierCode:
     * Source: Store/merchant name in header/footer (e.g., ""FASHIONNOWVA"")
     * NEVER use consignee name
     * Fallback: Email domain analysis (@company.com)

   - TotalDeduction:
     * Look for: Coupons, credits, free shipping markers
     * Calculate: Sum of all price reductions

   - TotalInternalFreight:
     * Combine: Shipping + Handling + Transportation fees
     * Source: ""FREIGHT"" values in consignee line

   - TotalOtherCost:
     * Include: Taxes + Fees + Duties
     * Look for: ""Tax"", ""Duty"", ""Fee"" markers
     * Calculate: Sum of all non-freight additional costs

3. CUSTOMS DECLARATION RULES:
   - Packages = Count from ""No. of Packages"" or ""Package Count""
   - GrossWeightKG = Numeric value from ""Gross Weight"" with KG units
   - Freight: Extract numeric value after ""FREIGHT""
   - FreightCurrency: Currency from freight context (e.g., ""US"" = USD)
   - BLNumber: Full value from ""WayBill Number"" including letters/numbers
   - ManifestYear/Number: Split ""Man Reg Number"" (e.g., 2024/1253 → 2024 & 1253)

4. DATA VALIDATION REQUIREMENTS:
   - Reject if:
     * SupplierCode == ConsigneeName
     * JSON contains unclosed brackets/braces
     * Any field is truncated mid-name
   - Required fields:
     * InvoiceDetails.TariffCode (use ""000000"" if missing)
     * CustomsDeclarations.Freight (0.0 if not found)
     * CustomsDeclarations[] (must exist even if empty)

5. JSON STRUCTURE VALIDATION:
   - MUST close all arrays/objects - CRITICAL REQUIREMENT
   - REQUIRED fields:
     * Invoices[]
     * CustomsDeclarations[]
   - Field completion examples:
     Good: ""GrossWeightKG"": 1.0}}
     Bad: ""Gross""
   - Final JSON must end with: }}]}}

6. JSON SCHEMA WITH COMPLETION GUARANTEES:
{{
  ""Invoices"": [{{
    ""InvoiceNo"": ""<str>"",
    ""PONumber"": ""<str|null>"",
    ""InvoiceDate"": ""<YYYY-MM-DD>"",
    ""Currency"": ""<ISO_CODE>"",
    ""SubTotal"": <float>,
    ""Total"": <float>,
    ""TotalDeduction"": <float|null>,
    ""TotalOtherCost"": <float|null>,            // Taxes + Fees + Duties
    ""TotalInternalFreight"": <float|null>,      //Shipping + Handling + Transportation fees
    ""TotalInsurance"": <float|null>,
    ""SupplierCode"": ""<str>"",     //One word name that is unique eg. ""Shien"" or ""Amazon"" or ""Walmart""
    ""SupplierName"": ""<str>"",     //Full Business name of supplier
    ""SupplierAddress"": ""<str>"",  //Full address of supplier IF NOT available use email address domain
    ""SupplierCountryCode"": ""<ISO3166-2>"",
    ""InvoiceDetails"": [{{
      ""ItemNumber"": ""<str|null>"",
      ""ItemDescription"": ""<str>"",
      ""Quantity"": <float>,
      ""Cost"": <float>,
      ""TotalCost"": <float>,
      ""Units"": ""<str>"",
      ""TariffCode"": ""<str>"",
      ""Discount"": <float|null>
    }}]
  }}],

  ""CustomsDeclarations"": [{{
    ""Consignee"": ""<str>"",
    ""CustomsOffice"": ""<str>"",
    ""ManifestYear"": <int>,
    ""ManifestNumber"": <int>,
    ""BLNumber"": ""<str>"",
    ""Freight"": <float>,
    ""FreightCurrency"": ""<ISO>"",
    ""PackageType"": ""<str>"",
    ""Packages"": <int>,
    ""GrossWeightKG"": <float>,
    ""Goods"": [{{
      ""Description"": ""<str>"",
      ""TariffCode"": ""<str>""
    }}]
  }}]
}}

7. OUTPUT INSTRUCTIONS (STRICT):
   - The entire response MUST be *only* the valid JSON object specified in the schema.
   - Do NOT include any introductory text, explanations, apologies, summaries, or markdown formatting (like ```json or ```).
   - Ensure all strings are properly escaped within the JSON.
   - Validate field endings and ensure all objects and arrays are correctly closed before finalizing.
   - The final output MUST be a single, complete, valid JSON structure ending precisely with `}}]}}`.";
        }




        public async Task<List<dynamic>> ExtractShipmentInvoice(List<string> pdfTextVariants)
        {
            var results = new List<IDictionary<string, object>>();

            foreach (var text in pdfTextVariants)
            {
                try
                {
                    var cleanedText = CleanText(text);
                    var response = await ProcessTextVariant(cleanedText).ConfigureAwait(false);
                    results.AddRange(response);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to process text variant");
                }
            }

            return new List<dynamic> { MergeDocuments(results)};
        }

        private string CleanText(string rawText)
        {
            try
            {
                // Fix: Use greedy quantifier and handle multiple dash sections properly
                // This pattern removes individual sections surrounded by 30+ dashes, not everything between first and last
                var cleaned = Regex.Replace(rawText, @"-{30,}[^-]*-{30,}", "", RegexOptions.Multiline);

                var match = Regex.Match(cleaned,
                    @"(?<=SHOP FASTER WITH THE APP)(.*?)(?=For Comptroller of Customs)",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                return match.Success ? match.Value : cleaned;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Text cleaning failed");
                return rawText;
            }
        }

        private async Task<List<IDictionary<string, object>>> ProcessTextVariant(string text)
        {
            // Add a check for potentially incorrect input type (heuristic)
            if (text != null && (text.StartsWith("System.Threading.Tasks.Task") || text.StartsWith("System.Text.StringBuilder")))
            {
                _logger.Warning("ProcessTextVariant received input that looks like a type name instead of content: {InputText}", TruncateForLog(text, 100));
                // Depending on desired behavior, could return empty list or throw exception here.
                // For now, let it proceed but the log indicates the upstream issue.
            }

            var escapedText = EscapeBraces(text);

            // Check if PromptTemplate looks like a custom prompt (not the default invoice extraction template)
            if (!PromptTemplate.Contains("DOCUMENT PROCESSING RULES:") && PromptTemplate.Contains("{0}"))
            {
                Console.WriteLine("⚠️  PROMPT CORRUPTION WARNING: Custom prompt being used as format template!");
                Console.WriteLine($"📝 Custom prompt: {PromptTemplate.Substring(0, Math.Min(100, PromptTemplate.Length))}...");
                Console.WriteLine("💡 ARCHITECTURAL ISSUE: Should use GetResponseAsync for custom prompts, not ExtractShipmentInvoice");
            }

            var prompt = string.Format(PromptTemplate, escapedText);
            // Log the final prompt being sent (Debug level recommended due to potential length/sensitivity)
            _logger.Debug("ProcessTextVariant - Generated Prompt: {Prompt}", prompt);
            var response = await GetCompletionAsync(prompt, DefaultTemperature, DefaultMaxTokens).ConfigureAwait(false);
            return ParseApiResponse(response);
        }

        private static string EscapeBraces(string input) => input.Replace("{", "{{").Replace("}", "}}");

        private List<IDictionary<string, object>> ParseApiResponse(string jsonResponse)
        {
            var documents = new List<IDictionary<string, object>>();
string cleanJson = null; // Declare outside the try block
try
{
    cleanJson = CleanJsonResponse(jsonResponse); // Assign inside
                cleanJson = CleanJsonResponse(jsonResponse); // Assign to the existing variable
                if (string.IsNullOrWhiteSpace(cleanJson)) return documents;

                // Log the cleaned JSON before attempting to parse
                _logger.Debug("Attempting to parse cleaned JSON: {CleanJson}", TruncateForLog(cleanJson));
                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                ProcessInvoices(root, documents);
                ProcessCustomsDeclarations(root, documents);
                ValidateAndEnhanceData(documents);

                return documents;
            }
            catch (JsonException jsonEx) // Catch specific JsonException
            {
                // Log the parsing error along with the JSON that caused it
                _logger.Error(jsonEx, "Failed to parse JSON response. Content was: {CleanJson}", TruncateForLog(cleanJson ?? jsonResponse));
                return new List<IDictionary<string, object>>(); // Return empty list on error
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                _logger.Error(ex, "Unexpected error during response parsing. Content was: {CleanJson}", TruncateForLog(cleanJson ?? jsonResponse));
                return new List<IDictionary<string, object>>();
            }
        }



        private void ProcessInvoices(JsonElement root, List<IDictionary<string, object>> documents)
        {
            if (root.TryGetProperty("Invoices", out var invoices))
            {
                foreach (var inv in invoices.EnumerateArray())
                {
                    IDictionary<string,object> dict = new BetterExpando();
                    dict["DocumentType"] = "Template";
                    dict["InvoiceNo"] = GetStringValue(inv, "InvoiceNo");
                    dict["PONumber"] = GetStringValue(inv, "PONumber");
                    dict["InvoiceDate"] = ParseDate(GetStringValue(inv, "InvoiceDate"));
                    dict["SubTotal"] = GetDecimalValue(inv, "SubTotal");
                    dict["InvoiceTotal"] = GetDecimalValue(inv, "Total");
                    dict["Currency"] = GetStringValue(inv, "Currency");
                    dict["SupplierCode"] = GetStringValue(inv, "SupplierCode");
                    dict["SupplierCode"] = GetStringValue(inv, "SupplierCode");
                    dict["SupplierName"] = GetStringValue(inv, "SupplierName");
                    dict["SupplierAddress"] = GetStringValue(inv, "SupplierAddress");
                    dict["SupplierCountryCode"] = GetStringValue(inv, "SupplierCountryCode");
                    if (!jsonIsNull(inv, "TotalDeduction", out var deduction))
                        dict["TotalDeduction"] = deduction.GetDecimal();//GetNullableDecimalValue(inv, "TotalDeduction");
                    if (!jsonIsNull(inv, "TotalOtherCost", out var otherCost))
                        dict["TotalOtherCost"] = otherCost.GetDecimal();//GetNullableDecimalValue(inv, "TotalOtherCost");
                    if(!jsonIsNull( inv, "TotalInternalFreight", out var freight))
                        dict["TotalInternalFreight"] = freight.GetDecimal();
                    if (!jsonIsNull(inv, "TotalInsurance", out var insurance))
                        dict["TotalInsurance"] = insurance.GetDecimal(); //GetNullableDecimalValue(inv, "TotalInsurance");
                    dict["InvoiceDetails"] = ParseInvoiceDetails(inv);
                    documents.Add(dict);
                }
            }
        }

        private List<IDictionary<string, object>> ParseInvoiceDetails(JsonElement invoiceElement)
        {
            var details = new List<IDictionary<string, object>>();
            if (invoiceElement.TryGetProperty("InvoiceDetails", out var detailsElement))
            {
                foreach (var det in detailsElement.EnumerateArray())
                {
                    IDictionary<string, object> item = new BetterExpando();

                    item["ItemDescription"] = GetStringValue(det, "ItemDescription");
                    item["Quantity"] = GetDecimalValue(det, "Quantity");
                    item["Cost"] = GetDecimalValue(det, "Cost");
                    item["TotalCost"] = GetDecimalValue(det, "TotalCost");
                    if (!jsonIsNull(det, "Units", out var units))
                        item["Units"] = units.GetString();//GetStringValue(det, "Units");
                    if(!jsonIsNull(det, "TariffCode", out var tariffcode))
                        item["TariffCode"] = ValidateTariffCode(tariffcode.GetString());//GetStringValue(det, "TariffCode");
                    if (!jsonIsNull(det, "Discount", out var discount))
                        item["Discount"] = discount.GetDecimal();//GetNullableDecimalValue(det, "Discount");

                    item["ItemNumber"] = GetStringValue(det, "ItemNumber");
                    details.Add(item);
                }
            }
            return details;
        }

        private void ProcessCustomsDeclarations(JsonElement root, List<IDictionary<string, object>> documents)
        {
            if (root.TryGetProperty("CustomsDeclarations", out var customs))
            {
                foreach (var cd in customs.EnumerateArray())
                {
                    IDictionary<string, object> dict = new BetterExpando();

                    if (string.IsNullOrEmpty(GetStringValue(cd, "BLNumber"))) continue;

                    dict["DocumentType"] = "CustomsDeclaration";
                    dict["CustomsOffice"] = GetStringValue(cd, "CustomsOffice");
                    dict["ManifestYear"] = GetIntValue(cd, "ManifestYear");
                    dict["ManifestNumber"] = GetIntValue(cd, "ManifestNumber");
                    dict["Consignee"] = GetStringValue(cd, "Consignee");
                    dict["BLNumber"] = GetStringValue(cd, "BLNumber");
                    dict["Goods"] = ParseGoodsClassifications(cd);
                    //dict["PackageInfo"] = ParsePackageInfo(cd);
                    dict["PackageType"] = GetStringValue(cd, "PackageType");
                    dict["Packages"] = GetIntValue(cd, "Packages");
                    dict["GrossWeightKG"] = GetDecimalValue(cd, "GrossWeightKG");
                    dict["FreightCurrency"] = GetStringValue(cd, "FreightCurrency");
                    dict["Freight"] = GetDecimalValue(cd, "Freight");
                    documents.Add(dict);
                }
            }
        }

        private List<IDictionary<string, object>> ParseGoodsClassifications(JsonElement customsElement)
        {
            var goods = new List<IDictionary<string, object>>();
            if (customsElement.TryGetProperty("Goods", out var goodsElement))
            {
                foreach (var g in goodsElement.EnumerateArray())
                {
                    IDictionary<string, object> item = new BetterExpando();
                    item["Description"] = GetStringValue(g, "Description");
                    item["TariffCode"] = ValidateTariffCode(GetStringValue(g, "TariffCode"));
                    goods.Add(item);
                }
            }
            return goods;
        }

        private IDictionary<string, object> ParsePackageInfo(JsonElement customsElement)
        {
            IDictionary<string, object> pkgInfo = new BetterExpando();
            if (customsElement.TryGetProperty("PackageInfo", out var packageElement))
            {
                pkgInfo["Packages"] = GetIntValue(packageElement, "Packages");
                pkgInfo["GrossWeightKG"] = GetDecimalValue(packageElement, "GrossWeightKG");
            }
            return pkgInfo;
        }

        private void ValidateAndEnhanceData(List<IDictionary<string, object>> documents)
        {
            CleanTotals(documents);
            CleanSupplierCodeConsignee(documents);
        }

        private void CleanSupplierCodeConsignee(List<IDictionary<string, object>> documents)
        {
            foreach (var doc in documents.Where(d => d["DocumentType"].ToString() == "Template"))
            {
                // Prevent consignee/supplier mismatch
                var supplier = doc["SupplierCode"]?.ToString() ?? "";
                var consignee = documents
                    .FirstOrDefault(d => d["DocumentType"].ToString() == "CustomsDeclaration")?["Consignee"]?.ToString() ?? "";

                if (supplier.Equals(consignee, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Warning("Supplier matches consignee: {Name}. Resetting to UNKNOWN.", supplier);
                    doc["SupplierCode"] = "UNKNOWN";
                    doc["SupplierAddress"] = null;
                }

                // Clean freight info from names
                if (doc["SupplierCode"] is string sc)
                    doc["SupplierCode"] = Regex.Replace(sc, @"\s*\(FREIGHT.*?\)", "");
            }
        }

        private void CleanTotals(List<IDictionary<string, object>> documents)
        {
            foreach (var doc in documents.Where(d => d["DocumentType"].ToString() == "Template"))
            {
                try
                {
                    var subTotal = Convert.ToDecimal(doc["SubTotal"]);
                    var total = Convert.ToDecimal(doc["InvoiceTotal"]);
                    var totalDeduction = Convert.ToDecimal(doc["TotalDeduction"] ?? 0m);
                    var totalOtherCost = Convert.ToDecimal(doc["TotalOtherCost"] ?? 0m);
                    var totalFreight = Convert.ToDecimal(doc["TotalInternalFreight"] ?? 0m);
                    var totalInsurance = Convert.ToDecimal(doc["TotalInsurance"] ?? 0m);

                    var calculatedTotal = subTotal
                                          - totalDeduction
                                          + totalOtherCost
                                          + totalFreight
                                          + totalInsurance;

                    if (Math.Abs(total - calculatedTotal) > 0.01m)
                    {
                        _logger.Warning("Invoice Total mismatch for InvoiceNo {InvoiceNo}. Declared: {DeclaredTotal}, Calculated: {CalculatedTotal}",
                            doc.TryGetValue("InvoiceNo", out var invNo) ? invNo : "N/A", total, calculatedTotal);
                        // Decide how to handle mismatch - log, flag, or adjust?
                        // For now, just log and keep the declared total.
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error cleaning totals for a document.");
                }
            }
        }

        private string DeriveCountryCode(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;

            // Simple heuristic: look for common country names or codes at the end of the address
            // This is a very basic implementation and might need enhancement
            var lowerAddress = address.ToLower();
            if (lowerAddress.EndsWith(" usa") || lowerAddress.EndsWith(", usa") || lowerAddress.EndsWith(" united states")) return "US";
            if (lowerAddress.EndsWith(" canada") || lowerAddress.EndsWith(", canada")) return "CA";
            // Add more countries as needed

            return null; // Return null if country cannot be derived
        }


        private decimal GetDecimalValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetDecimal(out var value)) return value;
                if (prop.TryGetDouble(out var doubleValue)) return (decimal)doubleValue; // Handle doubles
                if (prop.TryGetInt32(out var intValue)) return (decimal)intValue; // Handle integers
            }
            return 0m; // Default to 0 if property is missing, null, or not a valid number
        }

        private decimal? GetNullableDecimalValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetDecimal(out var value)) return value;
                if (prop.TryGetDouble(out var doubleValue)) return (decimal)doubleValue; // Handle doubles
                if (prop.TryGetInt32(out var intValue)) return (decimal)intValue; // Handle integers
            }
            return null; // Return null if property is missing, null, or not a valid number
        }

        private int GetIntValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.TryGetInt32(out var value)) return value;
                if (prop.TryGetDouble(out var doubleValue)) return (int)doubleValue; // Handle doubles by truncating
            }
            return 0; // Default to 0
        }

        private string GetStringValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                return prop.GetString();
            }
            return null; // Default to null
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return null;
            if (DateTime.TryParse(dateString, out var date)) return date;
            return null;
        }

        private static bool jsonIsNull(JsonElement element, string propertyName, out JsonElement value)
        {
            if (element.TryGetProperty(propertyName, out value))
            {
                return value.ValueKind == JsonValueKind.Null;
            }
            value = default; // Assign default value if property doesn't exist
            return true; // Treat missing property as null
        }

        public string ValidateTariffCode(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode)) return "000000"; // Default if empty

            // Remove non-digit characters
            var cleaned = Regex.Replace(rawCode, @"\D", "");

            if (cleaned.Length >= 6)
            {
                // Take the first 6 digits
                return cleaned.Substring(0, 6);
            }
            else if (cleaned.Length > 0)
            {
                // If less than 6 but has digits, pad with zeros
                _logger.Warning("Tariff code '{RawCode}' is too short ({Length} digits). Padding with zeros.", rawCode, cleaned.Length);
                return cleaned.PadRight(6, '0');
            }
            else
            {
                // No digits found
                _logger.Warning("Tariff code '{RawCode}' contains no digits. Returning default '000000'.", rawCode);
                return "000000";
            }
        }


        /// <summary>
        /// Public method for getting responses from DeepSeek API
        /// Used by OCRCorrectionService and other components
        /// </summary>
        public async Task<string> GetResponseAsync(string prompt)
        {
            return await GetCompletionAsync(prompt, DefaultTemperature, DefaultMaxTokens).ConfigureAwait(false);
        }

        public async Task<string> GetCompletionAsync(string prompt, double temperature, int maxTokens)
        {
            if (_httpClient == null) throw new InvalidOperationException("HttpClient is not initialized.");
            if (string.IsNullOrWhiteSpace(_baseUrl)) throw new InvalidOperationException("Base URL is not set.");

            try
            {
                var requestBody = new
                {
                    model = Model,
                    messages = new[] { new { role = "user", content = prompt } },
                    temperature = temperature,
                    max_tokens = maxTokens,
                    stream = false // Not using streaming responses here
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use the retry policy for the HTTP request
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions");
                    requestMessage.Content = content;
                    return await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                }).ConfigureAwait(false);

                return await HandleApiResponse(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "API request failed after retries.");
                throw; // Re-throw the exception after logging
            }
        }

        private async Task<string> HandleApiResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.Debug("API Response Status: {StatusCode}", response.StatusCode);
            _logger.Verbose("Raw API Response Content: {ResponseContent}", TruncateForLog(responseContent));

            // Check for specific error status codes
            if (response.StatusCode == (HttpStatusCode)429) // Too Many Requests
            {
                _logger.Warning("API rate limit exceeded (HTTP 429).");
                throw new RateLimitException((int)response.StatusCode, $"API rate limit exceeded. Response: {responseContent}");
            }

            // Check for other non-success status codes
            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("API Error: {StatusCode} - {Content}", response.StatusCode, TruncateForLog(responseContent));
                throw new HttpRequestException($"API request failed with status {(int)response.StatusCode}. Response: {responseContent}");
            }

            // Parse the successful response
            try
            {
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                var messageContent = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                if (string.IsNullOrWhiteSpace(messageContent))
                {
                    _logger.Warning("API response content is empty or null.");
                    return string.Empty;
                }

                _logger.Verbose("Extracted content: {MessageContent}", TruncateForLog(messageContent));
                return messageContent;
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(jsonEx, "Failed to parse successful API response JSON. Content was: {ResponseContent}", TruncateForLog(responseContent));
                throw new HttpRequestException("Failed to parse API response JSON.", jsonEx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error parsing successful API response. Content was: {ResponseContent}", TruncateForLog(responseContent));
                throw new HttpRequestException("Error processing API response.", ex);
            }
        }

        private List<IDictionary<string, object>> MergeDocuments(List<IDictionary<string, object>> documents)
        {
            if (documents == null || !documents.Any()) return new List<IDictionary<string, object>>();

            var merged = new BetterExpando() as IDictionary<string, object>;
            merged["Invoices"] = new List<IDictionary<string, object>>();
            merged["CustomsDeclarations"] = new List<IDictionary<string, object>>();

            foreach (var doc in documents)
            {
                if (doc.TryGetValue("DocumentType", out var docType))
                {
                    if (docType.ToString() == "Template")
                    {
                        // Add invoice documents
                        if (doc.TryGetValue("Invoices", out var invoices) && invoices is List<IDictionary<string, object>> invList)
                        {
                            ((List<IDictionary<string, object>>)merged["Invoices"]).AddRange(invList);
                        }
                        // Also add top-level invoice fields if they exist (handle cases where LLM doesn't nest correctly)
                        else if (doc.ContainsKey("InvoiceNo"))
                        {
                             ((List<IDictionary<string, object>>)merged["Invoices"]).Add(doc);
                        }
                    }
                    else if (docType.ToString() == "CustomsDeclaration")
                    {
                         // Add customs declaration documents
                        if (doc.TryGetValue("CustomsDeclarations", out var customs) && customs is List<IDictionary<string, object>> customsList)
                        {
                            ((List<IDictionary<string, object>>)merged["CustomsDeclarations"]).AddRange(customsList);
                        }
                         // Also add top-level customs fields if they exist
                        else if (doc.ContainsKey("BLNumber"))
                        {
                             ((List<IDictionary<string, object>>)merged["CustomsDeclarations"]).Add(doc);
                        }
                    }
                }
            }

            // Simple deduplication for invoices based on InvoiceNo (basic)
            if (merged["Invoices"] is List<IDictionary<string, object>> finalInvoices)
            {
                merged["Invoices"] = finalInvoices
                    .GroupBy(inv => inv.TryGetValue("InvoiceNo", out var invNo) ? invNo?.ToString() : null)
                    .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                    .Select(g => g.First()) // Take the first occurrence of each unique InvoiceNo
                    .ToList();
            }

             // Simple deduplication for customs declarations based on BLNumber (basic)
            if (merged["CustomsDeclarations"] is List<IDictionary<string, object>> finalCustoms)
            {
                merged["CustomsDeclarations"] = finalCustoms
                    .GroupBy(cd => cd.TryGetValue("BLNumber", out var blNo) ? blNo?.ToString() : null)
                    .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                    .Select(g => g.First()) // Take the first occurrence of each unique BLNumber
                    .ToList();
            }


            return new List<IDictionary<string, object>> { merged };
        }


        private string CleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            // Remove common markdown code block indicators and BOM
            var cleaned = Regex.Replace(jsonResponse, @"```json|```|'''|\uFEFF", string.Empty, RegexOptions.IgnoreCase);

            // Attempt to find the actual JSON object boundaries
            var startIndex = cleaned.IndexOf('{');
            var endIndex = cleaned.LastIndexOf('}');

            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                _logger.Warning("No valid JSON boundaries detected in API response after initial cleaning. Content: {Content}", TruncateForLog(cleaned));
                return string.Empty; // No valid JSON found
            }

            // Extract the potential JSON string
            var jsonString = cleaned.Substring(startIndex, endIndex - startIndex + 1);

            // Trim whitespace and potentially stray quotes/backticks from the ends
            return jsonString.Trim(new[] { '\n', '\r', ' ', '\t', '`', '\'', '"' });
        }

        private string HandleWrappedResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            // Check if the response is wrapped in extra text or markdown
            // This is a heuristic and might need refinement based on actual API responses
            if (jsonResponse.TrimStart().StartsWith("```json", StringComparison.OrdinalIgnoreCase) && jsonResponse.TrimEnd().EndsWith("```"))
            {
                _logger.Debug("Response appears to be wrapped in ```json markdown. Attempting to extract.");
                return RemoveMarkDCleanJsonResponse(jsonResponse);
            }
            else if (jsonResponse.TrimStart().StartsWith("{") && jsonResponse.TrimEnd().EndsWith("}"))
            {
                // Looks like a raw JSON object, no wrapping
                return jsonResponse;
            }
            else
            {
                _logger.Warning("Response does not appear to be standard JSON or ```json wrapped. Attempting aggressive cleaning.");
                // Aggressive cleaning: try to find the first '{' and last '}'
                var startIndex = jsonResponse.IndexOf('{');
                var endIndex = jsonResponse.LastIndexOf('}');
                if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
                {
                    return jsonResponse.Substring(startIndex, endIndex - startIndex + 1);
                }
                else
                {
                    _logger.Error("Aggressive cleaning failed to find JSON object boundaries. Returning empty string.");
                    return string.Empty; // Cannot find JSON boundaries
                }
            }
        }

        private string RemoveMarkDCleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            // Remove ```json and ``` markers
            var cleaned = Regex.Replace(jsonResponse, @"```json|```", string.Empty, RegexOptions.IgnoreCase).Trim();

            // Remove BOM if present
            if (cleaned.StartsWith("\uFEFF"))
            {
                cleaned = cleaned.Substring(1);
            }

            return cleaned;
        }

        private string TruncateForLog(string text, int maxLength = 500)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
        }

        // Custom Exception for Rate Limiting
        public class RateLimitException : HttpRequestException
        {
            public int StatusCode { get; }
            public RateLimitException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
            public RateLimitException(int statusCode, string message, Exception inner) : base(message, inner) { StatusCode = statusCode; }
        }

        // IDisposable implementation
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}