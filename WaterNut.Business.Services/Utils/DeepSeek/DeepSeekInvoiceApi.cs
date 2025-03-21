
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
using Microsoft.Extensions.Logging;

namespace WaterNut.Business.Services.Utils
{
    public class DeepSeekInvoiceApi : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeepSeekInvoiceApi> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        // Existing configuration properties
        public string PromptTemplate { get; set; }
        public string Model { get; set; } = "deepseek-chat";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 1500;
        public string HsCodePattern { get; set; } = @"\b\d{4}(?:[\.\-]\d{2,4})*\b";

        public DeepSeekInvoiceApi(ILogger<DeepSeekInvoiceApi> logger, HttpClient httpClient = null)
        {
            _apiKey = "sk-2872e533da794296b127537a6b53607f";
            _baseUrl = "https://api.deepseek.com/v1";
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? new HttpClient();
            ConfigureHttpClient();
            SetDefaultPrompts();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-DeepSeek-Version", "2023-07-01");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        private void SetDefaultPrompts()
        {
            PromptTemplate = @"DOCUMENT PROCESSING PROMPT:
Extract BOTH commercial invoices AND customs declarations from:

{0}

Return JSON with:
{
  ""Invoices"": [{
    ""InvoiceNo"": """",
    ""InvoiceDate"": """",
    ""Total"": 0.00,
    ""Currency"": """",
    ""Supplier"": """",
    ""LineItems"": [{""Description"": """", ""Quantity"": 0}]
  }],
  ""CustomsDeclarations"": [{
    ""Consignee"": """",
    ""BLNumber"": """",
    ""Goods"": [{""Description"": """", ""TariffCode"": """"}],
    ""PackageInfo"": {""Count"": 0, ""WeightKG"": 0.0}
  }]
}";

        }

        public async Task<List<IDictionary<string, object>>> ExtractShipmentInvoice(List<string> pdfTextVariants)
        {
            var results = new List<IDictionary<string, object>>();

            foreach (var text in pdfTextVariants.Take(3))
            {
                try
                {
                    var response = await ProcessTextVariant(text);
                    results.AddRange(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process text variant");
                }
            }

            return MergeDocuments(results);
        }

        private async Task<List<IDictionary<string, object>>> ProcessTextVariant(string text)
        {
            var prompt = string.Format(PromptTemplate, text);
            var response = await GetCompletionAsync(prompt, DefaultTemperature, DefaultMaxTokens);
            return ParseApiResponse(response);
        }

        private List<IDictionary<string, object>> ParseApiResponse(string jsonResponse)
        {
            var documents = new List<IDictionary<string, object>>();

            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;

            // Process invoices
            if (root.TryGetProperty("Invoices", out var invoices))
            {
                foreach (var inv in invoices.EnumerateArray())
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["DocumentType"] = "Invoice",
                        ["InvoiceNo"] = GetStringValue(inv, "InvoiceNo"),
                        ["InvoiceDate"] = ParseDate(GetStringValue(inv, "InvoiceDate")),
                        ["Total"] = GetDecimalValue(inv, "Total"),
                        ["Currency"] = GetStringValue(inv, "Currency"),
                        ["Supplier"] = GetStringValue(inv, "Supplier"),
                        ["LineItems"] = ParseLineItems(inv)
                    };
                    documents.Add(dict);
                }
            }

            // Process customs declarations
            if (root.TryGetProperty("CustomsDeclarations", out var customs))
            {
                foreach (var cd in customs.EnumerateArray())
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["DocumentType"] = "CustomsDeclaration",
                        ["Consignee"] = GetStringValue(cd, "Consignee"),
                        ["BLNumber"] = GetStringValue(cd, "BLNumber"),
                        ["Goods"] = ParseGoodsClassifications(cd),
                        ["PackageInfo"] = ParsePackageInfo(cd)
                    };
                    documents.Add(dict);
                }
            }

            return documents;
        }

        // New helper methods
        private string GetStringValue(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) ? value.GetString() : null;
        }

        private decimal GetDecimalValue(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
                ? value.GetDecimal()
                : 0m;
        }

        private int GetIntValue(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
                ? value.GetInt32()
                : 0;
        }

        // Updated parsing methods
        private List<IDictionary<string, object>> ParseLineItems(JsonElement invoiceElement)
        {
            var items = new List<IDictionary<string, object>>();
            if (invoiceElement.TryGetProperty("LineItems", out var lineItems))
            {
                foreach (var li in lineItems.EnumerateArray())
                {
                    items.Add(new Dictionary<string, object>
                    {
                        ["Description"] = GetStringValue(li, "Description"),
                        ["Quantity"] = GetDecimalValue(li, "Quantity"),
                        ["TariffCode"] = ValidateTariffCode(GetStringValue(li, "TariffCode"))
                    });
                }
            }
            return items;
        }

        private List<IDictionary<string, object>> ParseGoodsClassifications(JsonElement customsElement)
        {
            var goods = new List<IDictionary<string, object>>();
            if (customsElement.TryGetProperty("Goods", out var goodsElement))
            {
                foreach (var g in goodsElement.EnumerateArray())
                {
                    goods.Add(new Dictionary<string, object>
                    {
                        ["Description"] = GetStringValue(g, "Description"),
                        ["TariffCode"] = ValidateTariffCode(GetStringValue(g, "TariffCode"))
                    });
                }
            }
            return goods;
        }

        private IDictionary<string, object> ParsePackageInfo(JsonElement customsElement)
        {
            var pkgInfo = new Dictionary<string, object>();
            if (customsElement.TryGetProperty("PackageInfo", out var packageElement))
            {
                pkgInfo["Count"] = GetIntValue(packageElement, "Count");
                pkgInfo["WeightKG"] = GetDecimalValue(packageElement, "WeightKG");
            }
            return pkgInfo;
        }
        

        private string ValidateTariffCode(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode)) return "";
            var cleanCode = Regex.Replace(rawCode, @"[^\d\.\-]", "");
            return Regex.IsMatch(cleanCode, HsCodePattern) ? cleanCode : "";
        }

        private DateTime? ParseDate(string dateStr)
        {
            if (DateTime.TryParse(dateStr, out var date)) return date;
            _logger.LogWarning("Failed to parse date: {DateString}", dateStr);
            return null;
        }

        private async Task<string> GetCompletionAsync(string prompt, double temperature, int maxTokens)
        {
            var request = new
            {
                model = Model,
                messages = new[] { new { role = "user", content = prompt } },
                temperature,
                max_tokens = maxTokens
            };

            var json = JsonSerializer.Serialize(request);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var responseDoc = JsonDocument.Parse(responseJson);

            return responseDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }

        private List<IDictionary<string, object>> MergeDocuments(List<IDictionary<string, object>> documents)
        {
            var merged = new Dictionary<string, IDictionary<string, object>>();

            foreach (var doc in documents)
            {
                var key = doc["DocumentType"] switch
                {
                    "Invoice" => $"INV_{doc["InvoiceNo"]}",
                    "CustomsDeclaration" => $"CUST_{doc["BLNumber"]}",
                    _ => Guid.NewGuid().ToString()
                };

                if (!merged.ContainsKey(key))
                {
                    merged[key] = doc;
                }
                else
                {
                    // Merge logic for overlapping documents
                }
            }

            return merged.Values.ToList();
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}


