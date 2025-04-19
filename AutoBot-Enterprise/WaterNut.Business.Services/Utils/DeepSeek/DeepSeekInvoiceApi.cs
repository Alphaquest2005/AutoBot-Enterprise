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
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace WaterNut.Business.Services.Utils
{
    public class DeepSeekInvoiceApi : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeepSeekInvoiceApi> _logger;
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
            _logger =  LoggingConfig.CreateLogger();//logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogTrace("Retry attempt {RetryCount}: Calculated delay {DelaySeconds}s", retryAttempt, delay.TotalSeconds);
                return delay;
            };
 
            // Exception-Specific Logging for Retries
            Action<DelegateResult<HttpResponseMessage>, TimeSpan> logRetryAction = (outcome, calculatedDelay) =>
            {
                var exception = outcome.Exception; // Exception that caused the retry
                var response = outcome.Result;     // Result if the delegate completed but triggered retry (e.g., status code)
 
                if (exception is RateLimitException rle)
                {
                    _logger.LogWarning(rle, "Retry needed due to Rate Limit (HTTP {StatusCode}). Delaying for {DelaySeconds}s...", rle.StatusCode, calculatedDelay.TotalSeconds);
                }
                else if (exception is TaskCanceledException tce)
                {
                    if (tce.CancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning(tce, "Retry triggered for Task Cancellation (possibly user initiated). Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                    else
                    {
                        _logger.LogWarning(tce, "Retry needed due to operation Timeout. Delaying for {DelaySeconds}s...", calculatedDelay.TotalSeconds);
                    }
                }
                else if (exception is HttpRequestException httpEx) // Includes exceptions where Data["StatusCode"] might be set
                {
                    // Try to get status code from Data if available, otherwise log generic HttpRequestException
                    var statusCode = httpEx.Data.Contains("StatusCode") ? httpEx.Data["StatusCode"] : "(unknown)";
                    _logger.LogWarning(httpEx, "Retry needed due to HTTP Request Error (Code: {StatusCode}). Delaying for {DelaySeconds}s...", statusCode, calculatedDelay.TotalSeconds);
                }
                else if (response != null) // Handle retries triggered by OrResult (status code)
                {
                     _logger.LogWarning("Retry needed due to Response Status Code {StatusCode}. Delaying for {DelaySeconds}s...", response.StatusCode, calculatedDelay.TotalSeconds);
                }
                else // Default logging for other handled exceptions
                {
                    _logger.LogWarning(exception, "Retry needed due to handled Transient Error ({ExceptionType}). Delaying for {DelaySeconds}s...", exception?.GetType().Name ?? "Unknown", calculatedDelay.TotalSeconds);
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
                    _logger.LogError(ex, "Failed to process text variant");
                }
            }

            return new List<dynamic> { MergeDocuments(results)};
        }

        private string CleanText(string rawText)
        {
            try
            {
                var cleaned = Regex.Replace(rawText, @"-{30,}.*?-{30,}", "", RegexOptions.Singleline);
                var match = Regex.Match(cleaned,
                    @"(?<=SHOP FASTER WITH THE APP)(.*?)(?=For Comptroller of Customs)",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                return match.Success ? match.Value : cleaned;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Text cleaning failed");
                return rawText;
            }
        }

        private async Task<List<IDictionary<string, object>>> ProcessTextVariant(string text)
        {
            // Add a check for potentially incorrect input type (heuristic)
            if (text != null && (text.StartsWith("System.Threading.Tasks.Task") || text.StartsWith("System.Text.StringBuilder")))
            {
                _logger.LogWarning("ProcessTextVariant received input that looks like a type name instead of content: {InputText}", TruncateForLog(text, 100));
                // Depending on desired behavior, could return empty list or throw exception here.
                // For now, let it proceed but the log indicates the upstream issue.
            }

            var escapedText = EscapeBraces(text);
            var prompt = string.Format(PromptTemplate, escapedText);
            // Log the final prompt being sent (Debug level recommended due to potential length/sensitivity)
            _logger.LogDebug("ProcessTextVariant - Generated Prompt: {Prompt}", prompt); // Consider using TruncateForLog if prompts are very long
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
                _logger.LogDebug("Attempting to parse cleaned JSON: {CleanJson}", TruncateForLog(cleanJson));
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
                _logger.LogError(jsonEx, "Failed to parse JSON response. Content was: {CleanJson}", TruncateForLog(cleanJson ?? jsonResponse)); // Use original if cleanJson is null
                return new List<IDictionary<string, object>>(); // Return empty list on error
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                _logger.LogError(ex, "Unexpected error during response parsing. Content was: {CleanJson}", TruncateForLog(cleanJson ?? jsonResponse)); // Use original if cleanJson is null
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
                    dict["DocumentType"] = "Invoice";
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
            foreach (var doc in documents.Where(d => d["DocumentType"].ToString() == "Invoice"))
            {
                // Prevent consignee/supplier mismatch
                var supplier = doc["SupplierCode"]?.ToString() ?? "";
                var consignee = documents
                    .FirstOrDefault(d => d["DocumentType"].ToString() == "CustomsDeclaration")?["Consignee"]?.ToString() ?? "";

                if (supplier.Equals(consignee, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Supplier matches consignee: {Name}. Resetting to UNKNOWN.", supplier);
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
            foreach (var doc in documents.Where(d => d["DocumentType"].ToString() == "Invoice"))
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
                        _logger.LogWarning("Total mismatch in invoice {InvoiceNo}: Expected {Calculated}, Actual {Actual}",
                            doc["InvoiceNo"], calculatedTotal, total);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to validate totals for invoice {InvoiceNo}", doc["InvoiceNo"]);
                }

                if (string.IsNullOrEmpty(doc["SupplierCountryCode"]?.ToString()))
                {
                    doc["SupplierCountryCode"] = DeriveCountryCode(doc["SupplierAddress"]?.ToString());
                }
            }
        }

        private string DeriveCountryCode(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return "";

            var countryMatch = Regex.Match(address, @"\b([A-Z]{2})$");
            if (countryMatch.Success) return countryMatch.Groups[1].Value;

            return address.Contains("United States") ? "US" :
                   address.Contains("Canada") ? "CA" :
                   address.Contains("Germany") ? "DE" : "";
        }

        private string GetStringValue(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var value) ? value.GetString() : null;

        private decimal GetDecimalValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var value))
                return 0m;

            if (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined)
                return 0m;

            try
            {
                return value.GetDecimal();
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("Failed to parse decimal value for property {PropertyName}", propertyName);
                return 0m;
            }
        }

        // Add this method for nullable decimals
        private decimal? GetNullableDecimalValue(JsonElement element, string propertyName)
        {
            if (jsonIsNull(element, propertyName, out var value)) return null;

            try
            {
                return value.GetDecimal();
            }
            catch (InvalidOperationException)
            {
                _logger.LogWarning("Failed to parse nullable decimal for property {PropertyName}", propertyName);
                return null;
            }
        }

        private static bool jsonIsNull(JsonElement element, string propertyName, out JsonElement value)
        {
            if (!element.TryGetProperty(propertyName, out value))
                return true;

            if (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined)
                return true;
            return false;
        }

        private int GetIntValue(JsonElement element, string propertyName) =>
            element.TryGetProperty(propertyName, out var value) ? value.GetInt32() : 0;

        private DateTime? ParseDate(string dateStr) =>
            DateTime.TryParse(dateStr, out var date) ? date : (DateTime?)null;

        public string ValidateTariffCode(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode)) return "";
            var cleanCode = Regex.Replace(rawCode, @"[^\d\.\-]", "");
            return Regex.IsMatch(cleanCode, HsCodePattern) ? cleanCode : "";
        }

        private async Task<string> GetCompletionAsync(string prompt, double temperature, int maxTokens)
        {
            try
            {
                var request = new
                {
                    model = Model,
                    messages = new[] { new { role = "user", content = prompt } },
                    temperature,
                    max_tokens = maxTokens
                };

                var json = JsonSerializer.Serialize(request);

                // Use the new retry policy directly
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions");
                    requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    // Send the request - Polly will handle retries based on HandleApiResponse throwing exceptions
                    return await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                }).ConfigureAwait(false);
 
                // HandleApiResponse now potentially throws exceptions caught by Polly,
                // or returns the successful content string.
                return await HandleApiResponse(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API request failed after retries");
                return "";
            }
        }

        private async Task<string> HandleApiResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogTrace("Raw API Response Content received: {ResponseContent}", TruncateForLog(responseContent)); // Use TruncateForLog
            _logger.LogDebug("API Response Status: {StatusCode}", response.StatusCode);
 
            // --- Throw specific exceptions for Polly to handle ---
            if (response.StatusCode == (HttpStatusCode)429) // Too Many Requests
            {
                _logger.LogDebug("HandleApiResponse: Throwing RateLimitException for status 429.");
                throw new RateLimitException((int)response.StatusCode, $"API rate limit exceeded. Response: {responseContent}");
            }
 
            // Check for server errors (5xx) or specific retryable codes handled by OrResult in the policy
            if (response.StatusCode >= HttpStatusCode.InternalServerError || HttpStatusCodesToRetry.Contains(response.StatusCode))
            {
                 var httpEx = new HttpRequestException($"API request failed with status {(int)response.StatusCode}. Response: {responseContent}");
                 httpEx.Data["StatusCode"] = (int)response.StatusCode; // Add status code for logging in retry policy
                 _logger.LogDebug(httpEx, "HandleApiResponse: Throwing HttpRequestException for retryable status {StatusCode}.", (int)response.StatusCode);
                 throw httpEx; // Polly will catch this based on Or<HttpRequestException> or OrResult
            }
 
            // If not retried by Polly but still not success, log error and return empty (or throw a non-retryable exception)
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API Error (non-retryable): {StatusCode} - {Content}", response.StatusCode, TruncateForLog(responseContent));
                // Depending on requirements, you might throw a different exception type here
                // or just return empty to indicate failure after non-retryable error.
                return ""; // Return empty for now
            }
            // --- End Polly exception throwing ---

            // Success case: Parse the actual content from the successful response
            try
            {
                 using var doc = JsonDocument.Parse(responseContent); // Parse the already read content
                 var messageContent = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
                 _logger.LogTrace("HandleApiResponse - Extracted content: {MessageContent}", TruncateForLog(messageContent));
                 return messageContent;
            }
            catch(JsonException jsonEx)
            {
                 _logger.LogError(jsonEx, "HandleApiResponse - Failed to parse successful response JSON. Content was: {ResponseContent}", TruncateForLog(responseContent));
                 return ""; // Or throw a specific parsing exception
            }
            catch(Exception ex)
            {
                 _logger.LogError(ex, "HandleApiResponse - Unexpected error parsing successful response. Content was: {ResponseContent}", TruncateForLog(responseContent));
                 return ""; // Or throw
            }
        }

        private List<IDictionary<string,object>> MergeDocuments(List<IDictionary<string, object>> documents)
        {
            var merged = new Dictionary<string, BetterExpando>();

            foreach (var doc in documents)
            {
                var key = doc["DocumentType"] switch
                {
                    "Invoice" => $"INV_{doc["InvoiceNo"]}",
                    "CustomsDeclaration" => $"CUST_{doc["BLNumber"]}",
                    _ => Guid.NewGuid().ToString()
                };

                if (!merged.ContainsKey(key)) merged[key] = (BetterExpando)doc;
            }

            return new List<IDictionary<string, object>>(merged.Values.ToList());
        }

        private string CleanJsonResponse(string jsonResponse)
        {
            // Step 1: Log the initial input to this method
            _logger.LogTrace("CleanJsonResponse - Input: {JsonResponse}", jsonResponse);
            var removedMarkdown = RemoveMarkDCleanJsonResponse(jsonResponse);
            // Step 2: Log the result after removing markdown/BOM and finding boundaries
            _logger.LogTrace("CleanJsonResponse - After RemoveMarkDCleanJsonResponse: {RemovedMarkdown}", removedMarkdown);
            return HandleWrappedResponse(removedMarkdown);
        }

        private string HandleWrappedResponse(string jsonResponse)
        {
            _logger.LogTrace("HandleWrappedResponse - Input: {JsonResponse}", jsonResponse);
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogTrace("HandleWrappedResponse - Returning empty due to null/whitespace input.");
                return string.Empty;
            }

            var sanitized = new StringBuilder();
            var stack = new Stack<char>();
            var lastValidIndex = 0;

            for (int i = 0; i < jsonResponse.Length; i++)
            {
                var c = jsonResponse[i];
                switch (c)
                {
                    case '{':
                    case '[':
                        stack.Push(c);
                        sanitized.Append(c);
                        lastValidIndex = i;
                        break;

                    case '}':
                        if (stack.Count > 0 && stack.Peek() == '{')
                        {
                            stack.Pop();
                            sanitized.Append(c);
                            lastValidIndex = i;
                        }
                        break;

                    case ']':
                        if (stack.Count > 0 && stack.Peek() == '[')
                        {
                            stack.Pop();
                            sanitized.Append(c);
                            lastValidIndex = i;
                        }
                        break;

                    default:
                        sanitized.Append(c);
                        break;
                }
            }

            while (stack.Count > 0)
            {
                var opener = stack.Pop();
                sanitized.Append(opener switch
                {
                    '{' => '}',
                    '[' => ']',
                    _ => ' '
                });
            }

            var clean = sanitized.ToString().Trim();
            if (clean.Length == 0 ||
                (clean.Length > 0 && clean[0] != '{') ||
                (clean.Length > 0 && clean[clean.Length - 1] != '}'))
            {
                // Log the specific string that failed validation
                _logger.LogWarning("Invalid JSON structure after HandleWrappedResponse cleaning. String was: {Clean}", TruncateForLog(clean));
                return string.Empty;
            }

            _logger.LogTrace("HandleWrappedResponse - Returning cleaned string: {Clean}", TruncateForLog(clean));
            return clean;
        }

        private string RemoveMarkDCleanJsonResponse(string jsonResponse)
        {
            _logger.LogTrace("RemoveMarkDCleanJsonResponse - Input: {JsonResponse}", jsonResponse);
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogTrace("RemoveMarkDCleanJsonResponse - Returning empty due to null/whitespace input.");
                return string.Empty;
            }

            var clean = Regex.Replace(jsonResponse,
                @"```json|```|'''|\uFEFF",
                string.Empty,
                RegexOptions.IgnoreCase
            );
            _logger.LogTrace("RemoveMarkDCleanJsonResponse - After Regex Replace: {Clean}", clean);
 
            var startIndex = clean.IndexOf('{');
            var endIndex = clean.LastIndexOf('}');
            _logger.LogTrace("RemoveMarkDCleanJsonResponse - Found startIndex: {StartIndex}, endIndex: {EndIndex}", startIndex, endIndex);

            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                _logger.LogWarning("No valid JSON boundaries detected in string: {Clean}", clean);
                return string.Empty;
            }

            var result = clean.Substring(startIndex, endIndex - startIndex + 1)
                .Trim(new[] { '\n', '\r', ' ', '\t', '`', '\'', '"' });
            _logger.LogTrace("RemoveMarkDCleanJsonResponse - Returning substring: {Result}", TruncateForLog(result));
            return result;
        }
public void Dispose() => _httpClient?.Dispose();

// --- Helper for logging ---
private string TruncateForLog(string text, int maxLength = 500)
{
    if (string.IsNullOrEmpty(text)) return string.Empty;
    return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
}

// --- Custom Exceptions ---
public class RateLimitException : HttpRequestException
{
    public int StatusCode { get; }
    public RateLimitException(int statusCode, string message) : base(message) { StatusCode = statusCode; }
    public RateLimitException(int statusCode, string message, Exception inner) : base(message, inner) { StatusCode = statusCode; }
}
// Potentially add HSCodeRequestException if needed later, similar to DeepSeekApi.cs

}
}






//        private void SetDefaultPrompts()
//        {
//            PromptTemplate = @"DOCUMENT PROCESSING RULES:

//0. PROCESS THIS TEXT INPUT:
//{0}

//1. TEXT STRUCTURE ANALYSIS:
//   - Focus sections between: ""SHOP FASTER WITH THE APP"" and ""For Comptroller of Customs""
//   - Priority order:
//     1. Item tables with prices/quantities
//     2. Customs declaration forms
//     3. Address blocks
//     4. Payment/header sections

//2. FIELD EXTRACTION GUIDANCE:
//   - SupplierCode: 
//     * Source: Store/merchant name in header/footer (e.g., ""FASHIONNOWVA"")
//     * NEVER use consignee name
//     * Fallback: Email domain analysis (@company.com)

//   - TotalDeduction: 
//     * Look for: Coupons, credits, free shipping markers
//     * Calculate: Sum of all price reductions

//   - TotalInternalFreight:
//     * Combine: Shipping + Handling + Transportation fees
//     * Source: ""FREIGHT"" values in consignee line

//3. CUSTOMS DECLARATION RULES:
//   - Packages = Count from ""No. of Packages"" or ""Package Count""
//   - GrossWeightKG = Numeric value from ""Gross Weight"" with KG units
//   - Freight: Extract numeric value after ""FREIGHT""
//   - FreightCurrency: Currency from freight context (e.g., ""US"" = USD)

//4. DATA VALIDATION REQUIREMENTS:
//   - Reject if: SupplierCode == ConsigneeName
//   - Required fields: 
//     * InvoiceDetails.TariffCode (use ""000000"" if missing)
//     * CustomsDeclarations.Freight (0.0 if not found)

//6. JSON STRUCTURE VALIDATION:
//   - MUST close all arrays/objects
//   - REQUIRED fields:
//     * Invoices[]
//     * CustomsDeclarations[] (can be empty)
//   - If no customs data: 
//     """"CustomsDeclarations"""": []

//5. JSON SCHEMA WITH EXTRACTION GUIDANCE:
//{{
//  ""Invoices"": [{{
//    // INVOICE HEADER DATA //
//    ""InvoiceNo"": ""<str>"",                    // From ""Order #"" value
//    ""PONumber"": ""<str|null>"",                 // ""PO Number"" if exists
//    ""InvoiceDate"": ""<YYYY-MM-DD>"",            // ""Date Placed:"" value
//    ""Currency"": ""<ISO_CODE>"",                 // Symbol analysis ($=USD)

//    // FINANCIAL BREAKDOWN //
//    ""SubTotal"": <float>,                       // Sum before deductions
//    ""Total"": <float>,                          // Final payable amount
//    ""TotalDeduction"": <float|null>,            // SUM(Coupons + Credits)
//    ""TotalOtherCost"": <float|null>,            // Taxes + Fees + Duties
//    ""TotalInternalFreight"": <float|null>,      // Shipping + Handling
//    ""TotalInsurance"": <float|null>,            // Insurance fees if present

//    // SUPPLIER INFORMATION //
//    ""SupplierCode"": ""<str>"",                 // Merchant name (cleaned)
//    ""SupplierAddress"": ""<str>"",              // From header/footer
//    ""SupplierCountryCode"": ""<ISO3166-2>"",    // From supplier address

//    // LINE ITEMS //
//    ""InvoiceDetails"": [{{
//      ""ItemNumber"": ""<str|null>"",           // SKU/Part number
//      ""ItemDescription"": ""<str>"",           // Full item text
//      ""Quantity"": <float>,                    // ""Qty:"" value
//      ""Cost"": <float>,                        // Unit price
//      ""TotalCost"": <float>,                   // Quantity * Cost
//      ""Units"": ""<str>"",                     // Size→Units mapping
//      ""TariffCode"": ""<str>"",                // From ""Tariff No."" column
//      ""Discount"": <float|null>                // Item-level discounts
//    }}]
//  }}],

//  ""CustomsDeclarations"": [{{
//    // SHIPPING INFO //
//    ""Consignee"": ""<str>"",                  // Delivery address name
//    ""CustomsOffice"": ""<str>"",              // Customs Office field
//    ""ManifestYear"": <int>,                   // First part of ""Man Reg Number"" Field eg. ""2024/1253"" ManifestYear = 2024 & ManifestNumber = 1253
//    ""ManifestNumber"": <int>,                 // Second part of ""Man Reg Number"" Field eg. ""2024/1253"" ManifestYear = 2024 & ManifestNumber = 1253
//    ""BLNumber"": ""<str>"",                   // ""WayBill Number"" value
//    ""Freight"": <float>,                      // From ""FREIGHT"" marker
//    ""FreightCurrency"": ""<ISO>"",            // Matches invoice currency
//    ""PackageType"": ""<str>"",                // 'Type of Package' field
//    ""Packages"": <int>,                       // 'Count' field
//    ""GrossWeightKG"": <float>                 // 'WeightKG' field

//    // PACKAGE DETAILS //
//    //""PackageInfo"": {{
//    //  ""Packages"": <int>,                      // 'Count' field
//    //  ""GrossWeightKG"": <float>                // 'WeightKG' field
//    //}},

//    // ITEM CLASSIFICATION //
//    ""Goods"": [{{
//      ""Description"": ""<str>"",               // Must match invoice items
//      ""TariffCode"": ""<str>""                 // Must match item tariff code
//    }}]
//  }}]
//}}";
//        } 