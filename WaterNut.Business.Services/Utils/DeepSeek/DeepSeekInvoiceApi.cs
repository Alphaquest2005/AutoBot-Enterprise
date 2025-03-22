using System;
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
        public int DefaultMaxTokens { get; set; } = 1500;
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

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult(r => HttpStatusCodesToRetry.Contains(r.StatusCode))
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount}/3. Error: {Error}",
                            retryCount, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    });
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
   - Focus sections between: ""SHOP FASTER WITH THE APP"" and ""For Comptroller of Customs""
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

3. CUSTOMS DECLARATION RULES:
   - Packages = Count from ""No. of Packages"" or ""Package Count""
   - GrossWeightKG = Numeric value from ""Gross Weight"" with KG units
   - Freight: Extract numeric value after ""FREIGHT""
   - FreightCurrency: Currency from freight context (e.g., ""US"" = USD)

4. DATA VALIDATION REQUIREMENTS:
   - Reject if: SupplierCode == ConsigneeName
   - Required fields: 
     * InvoiceDetails.TariffCode (use ""000000"" if missing)
     * CustomsDeclarations.Freight (0.0 if not found)

6. JSON STRUCTURE VALIDATION:
   - MUST close all arrays/objects
   - REQUIRED fields:
     * Invoices[]
     * CustomsDeclarations[] (can be empty)
   - If no customs data: 
     """"CustomsDeclarations"""": []

5. JSON SCHEMA WITH EXTRACTION GUIDANCE:
{{
  ""Invoices"": [{{
    // INVOICE HEADER DATA //
    ""InvoiceNo"": ""<str>"",                    // From ""Order #"" value
    ""PONumber"": ""<str|null>"",                 // ""PO Number"" if exists
    ""InvoiceDate"": ""<YYYY-MM-DD>"",            // ""Date Placed:"" value
    ""Currency"": ""<ISO_CODE>"",                 // Symbol analysis ($=USD)
    
    // FINANCIAL BREAKDOWN //
    ""SubTotal"": <float>,                       // Sum before deductions
    ""Total"": <float>,                          // Final payable amount
    ""TotalDeduction"": <float|null>,            // SUM(Coupons + Credits)
    ""TotalOtherCost"": <float|null>,            // Taxes + Fees + Duties
    ""TotalInternalFreight"": <float|null>,      // Shipping + Handling
    ""TotalInsurance"": <float|null>,            // Insurance fees if present
    
    // SUPPLIER INFORMATION //
    ""SupplierCode"": ""<str>"",                 // Merchant name (cleaned)
    ""SupplierAddress"": ""<str>"",              // From header/footer
    ""SupplierCountryCode"": ""<ISO3166-2>"",    // From supplier address
    
    // LINE ITEMS //
    ""InvoiceDetails"": [{{
      ""ItemNumber"": ""<str|null>"",           // SKU/Part number
      ""ItemDescription"": ""<str>"",           // Full item text
      ""Quantity"": <float>,                    // ""Qty:"" value
      ""Cost"": <float>,                        // Unit price
      ""TotalCost"": <float>,                   // Quantity * Cost
      ""Units"": ""<str>"",                     // Size→Units mapping
      ""TariffCode"": ""<str>"",                // From ""Tariff No."" column
      ""Discount"": <float|null>                // Item-level discounts
    }}]
  }}],
  
  ""CustomsDeclarations"": [{{
    // SHIPPING INFO //
    ""Consignee"": ""<str>"",                   // Delivery address name
    ""BLNumber"": ""<str>"",                    // ""WayBill Number"" value
    ""Freight"": <float>,                       // From ""FREIGHT"" marker
    ""FreightCurrency"": ""<ISO>"",             // Matches invoice currency
    
    // PACKAGE DETAILS //
    ""PackageInfo"": {{
      ""Packages"": <int>,                      // Former 'Count' field
      ""GrossWeightKG"": <float>                // Former 'WeightKG' field
    }},
    
    // ITEM CLASSIFICATION //
    ""Goods"": [{{
      ""Description"": ""<str>"",               // Must match invoice items
      ""TariffCode"": ""<str>""                 // Must match item tariff code
    }}]
  }}]
}}";
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
            var escapedText = EscapeBraces(text);
            var prompt = string.Format(PromptTemplate, escapedText);
            var response = await GetCompletionAsync(prompt, DefaultTemperature, DefaultMaxTokens).ConfigureAwait(false);
            return ParseApiResponse(response);
        }

        private static string EscapeBraces(string input) => input.Replace("{", "{{").Replace("}", "}}");

        private List<IDictionary<string, object>> ParseApiResponse(string jsonResponse)
        {
            var documents = new List<IDictionary<string, object>>();

            try
            {
                var cleanJson = CleanJsonResponse(jsonResponse);
                if (string.IsNullOrWhiteSpace(cleanJson)) return documents;

                using var doc = JsonDocument.Parse(cleanJson);
                var root = doc.RootElement;

                ProcessInvoices(root, documents);
                ProcessCustomsDeclarations(root, documents);
                ValidateAndEnhanceData(documents);

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Response parsing failed");
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
                    dict["Total"] = GetDecimalValue(inv, "Total");
                    dict["Currency"] = GetStringValue(inv, "Currency");
                    dict["SupplierCode"] = GetStringValue(inv, "SupplierCode");
                    dict["SupplierAddress"] = GetStringValue(inv, "SupplierAddress");
                    dict["SupplierCountryCode"] = GetStringValue(inv, "SupplierCountryCode");
                    dict["TotalDeduction"] = GetNullableDecimalValue(inv, "TotalDeduction");
                    dict["TotalOtherCost"] = GetNullableDecimalValue(inv, "TotalOtherCost");
                    dict["TotalInternalFreight"] = GetNullableDecimalValue(inv, "TotalInternalFreight");
                    dict["TotalInsurance"] = GetNullableDecimalValue(inv, "TotalInsurance");
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
                    item["Units"] = GetStringValue(det, "Units");
                    item["TariffCode"] = ValidateTariffCode(GetStringValue(det, "TariffCode"));
                    item["Discount"] = GetNullableDecimalValue(det, "Discount");
                   
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

                    dict["DocumentType"] = "CustomsDeclaration";
                    dict["Consignee"] = GetStringValue(cd, "Consignee");
                    dict["BLNumber"] = GetStringValue(cd, "BLNumber");
                    dict["Goods"] = ParseGoodsClassifications(cd);
                    dict["PackageInfo"] = ParsePackageInfo(cd);
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
                var subTotal = Convert.ToDecimal(doc["SubTotal"]);
                var total = Convert.ToDecimal(doc["Total"]);
                var calculatedTotal = subTotal
                                      - Convert.ToDecimal(doc["TotalDeduction"] ?? 0m)
                                      + Convert.ToDecimal(doc["TotalOtherCost"] ?? 0m)
                                      + Convert.ToDecimal(doc["TotalInternalFreight"] ?? 0m);

                if (Math.Abs(total - calculatedTotal) > 0.01m)
                {
                    _logger.LogWarning("Total mismatch in invoice {InvoiceNo}: Expected {Calculated}, Actual {Actual}",
                        doc["InvoiceNo"], calculatedTotal, total);
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
            if (!element.TryGetProperty(propertyName, out var value))
                return null;

            if (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined)
                return null;

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

                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    return await _httpClient.PostAsync(
                        $"{_baseUrl}/chat/completions",
                        content
                    ).ConfigureAwait(false);
                });

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
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError("API Error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                return "";
            }

            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using var doc = JsonDocument.Parse(responseJson);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
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
            return HandleWrappedResponse(RemoveMarkDCleanJsonResponse(jsonResponse));
        }

        private string HandleWrappedResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

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
                _logger.LogWarning("Invalid JSON structure after cleaning");
                return string.Empty;
            }

            return clean;
        }

        private string RemoveMarkDCleanJsonResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

            var clean = Regex.Replace(jsonResponse,
                @"```json|```|'''|\uFEFF",
                string.Empty,
                RegexOptions.IgnoreCase
            );

            var startIndex = clean.IndexOf('{');
            var endIndex = clean.LastIndexOf('}');

            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            {
                _logger.LogWarning("No valid JSON boundaries detected");
                return string.Empty;
            }

            return clean.Substring(startIndex, endIndex - startIndex + 1)
                .Trim(new[] { '\n', '\r', ' ', '\t', '`', '\'', '"' });
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}