using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WaterNut.Business.Services.Utils
{
    public partial class DeepSeekApi : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public string PromptTemplate { get; set; }
        public string Model { get; set; } = "deepseek-chat";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 150;
        public string HsCodePattern { get; set; } = @"\b\d{4}(?:[\.\-]\d{2,4})*\b";
        public int MaxDescriptionLength { get; set; } = 500;
        public string SanitizePattern { get; set; } = @"[^\p{L}\p{N}\p{P}\p{S}\s]";
        public string ItemNumberPattern { get; set; } = @"^[\w-]{1,20}$";
        public string TariffCodePattern { get; set; } = @"^\d{4}([-.]?\d{2,4})*$";

        public DeepSeekApi()
        {
            _apiKey = "sk-2872e533da794296b127537a6b53607f";
            _baseUrl = "https://api.deepseek.com/v1";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-DeepSeek-Version", "2023-07-01");
            SetDefaultPrompt();
        }

        private void SetDefaultPrompt()
        {
            PromptTemplate = @"Analyze the following product description and determine the accurate HS code according to the latest CARICOM Common External Tariff. 
Consider the item's:
1. Material composition
2. Manufacturing process
3. Primary function
4. Technical specifications
5. Industry classification

Product description: {0}

Respond EXCLUSIVELY in the format: |HS_CODE|full_code_with_dots| |CATEGORY|brief_category_description|
Example: |HS_CODE|8542.31.00| |CATEGORY|Electronic integrated circuits|";
        }

        public async Task<string> GetTariffCode(string itemDescription, double? temperature = null, int? maxTokens = null)
        {
            try
            {
                var cleanDescription = SanitizeInputText(itemDescription);
                var prompt = string.Format(PromptTemplate, cleanDescription);
                var requestBody = new
                {
                    model = Model,
                    messages = new[] { new { role = "user", content = prompt } },
                    temperature = temperature ?? DefaultTemperature,
                    max_tokens = maxTokens ?? DefaultMaxTokens,
                    stream = false
                };

                var response = await PostRequestAsync(requestBody).ConfigureAwait(false);
                return ParseHsCode(response).Replace(".", "");
            }
            catch (Exception ex)
            {
                throw new HSCodeRequestException("Failed to retrieve HS code", ex);
            }
        }

        public Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode)> ClassifyItems(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            var sanitizedItems = items.Select(item => (
                SanitizeItemNumber(item.ItemNumber),
                SanitizeInputText(item.ItemDescription),
                SanitizeTariffCode(item.TariffCode)
            )).ToList();

            var result = new Dictionary<string, (string, string, string)>();

            try
            {
                var batchResult = ProcessBatch(sanitizedItems).GetAwaiter().GetResult();
                foreach (var item in sanitizedItems)
                {
                    var (itemNumber, description, tariffCode) = item;
                    if (batchResult.TryGetValue(description, out var batchValues))
                    {
                        itemNumber = string.IsNullOrWhiteSpace(itemNumber) ? batchValues.ItemNumber : itemNumber;
                        tariffCode = string.IsNullOrWhiteSpace(tariffCode) ? batchValues.TariffCode : tariffCode;
                    }
                    result[description] = (itemNumber, description, tariffCode);
                }
            }
            catch
            {
                foreach (var item in sanitizedItems)
                {
                    var (itemNumber, description, tariffCode) = item;

                    if (string.IsNullOrWhiteSpace(itemNumber))
                        itemNumber = GenerateProductCode(description).GetAwaiter().GetResult();

                    if (string.IsNullOrWhiteSpace(tariffCode))
                        tariffCode = GetTariffCode(description).GetAwaiter().GetResult();

                    result[description] = (itemNumber, description, tariffCode);
                }
            }

            return result;
        }

        private async Task<Dictionary<string, (string ItemNumber, string TariffCode)>> ProcessBatch(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            var batchPrompt = CreateBatchPrompt(items);
            var jsonResponse = await PostRequestAsync(new
            {
                model = Model,
                messages = new[] { new { role = "user", content = batchPrompt } },
                temperature = DefaultTemperature,
                max_tokens = 1000,
                stream = false
            });

            return ParseBatchResponse(jsonResponse);
        }

        private string CreateBatchPrompt(List<(string ItemNumber, string ItemDescription, string TariffCode)> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"Process these product entries according to these rules:");
            sb.AppendLine("1. Generate missing product codes (max 20 alphanumeric chars)");
            sb.AppendLine("2. Determine missing HS codes using CARICOM CET");
            sb.AppendLine("3. Maintain original description meaning");
            sb.AppendLine("4. Use existing valid codes when present");
            sb.AppendLine("\nProduct List:");

            foreach (var item in items)
            {
                var safeItemNumber = SanitizeItemNumber(item.ItemNumber);
                var safeDescription = SanitizeInputText(item.ItemDescription);
                var safeTariffCode = SanitizeTariffCode(item.TariffCode);

                sb.AppendLine(
                    $"- {(string.IsNullOrEmpty(safeItemNumber) ? "[NEW]" : safeItemNumber)} | " +
                    $"{safeDescription} | " +
                    $"{(string.IsNullOrEmpty(safeTariffCode) ? "[NEEDS_HS]" : safeTariffCode)}");
            }

            sb.AppendLine("\nRespond STRICTLY in this JSON format:");
            sb.AppendLine(@"{
    ""items"": [
        {
            ""original_description"": ""exact sanitized description from list"",
            ""product_code"": ""generated or existing code"",
            ""hs_code"": ""full code with dots""
        }
    ]
}");
            sb.AppendLine("Note: Maintain original item order and exact description matching!");

            return sb.ToString();
        }

        private Dictionary<string, (string, string)> ParseBatchResponse(string jsonResponse)
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var batchData = JsonSerializer.Deserialize<JsonElement>(content);
            var result = new Dictionary<string, (string, string)>();

            foreach (var item in batchData.GetProperty("items").EnumerateArray())
            {
                var description = item.GetProperty("original_description").GetString();
                var productCode = item.GetProperty("product_code").GetString();
                var hsCode = item.GetProperty("hs_code").GetString().Replace(".", "");

                result[description] = (SanitizeProductCode(productCode), hsCode);
            }

            return result;
        }

        private string SanitizeInputText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var sanitized = Regex.Replace(input, SanitizePattern, "", RegexOptions.Compiled);
            sanitized = sanitized
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\b", "")
                .Replace("\f", "")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\t", " ");

            return sanitized.Length <= MaxDescriptionLength
                ? sanitized.Trim()
                : sanitized.Substring(0, MaxDescriptionLength).Trim();
        }

        private string SanitizeItemNumber(string itemNumber)
        {
            if (string.IsNullOrWhiteSpace(itemNumber)) return "";
            var sanitized = Regex.Replace(itemNumber, @"[^\w-]", "");
            return sanitized.Length <= 20 ? sanitized : sanitized.Substring(0, 20);
        }

        private string SanitizeTariffCode(string tariffCode)
        {
            if (string.IsNullOrWhiteSpace(tariffCode)) return "";
            var sanitized = Regex.Replace(tariffCode, @"[^\d.-]", "").Replace(".","");
            return Regex.IsMatch(sanitized, TariffCodePattern) ? sanitized : "";
        }

        private async Task<string> GenerateProductCode(string description)
        {
            var cleanDesc = SanitizeInputText(description);
            const string prompt = @"Generate a concise product code under 20 characters for: {0}
                Use alphanumerics and hyphens only. Respond ONLY with the code.";

            var response = await GetCompletionAsync(string.Format(prompt, cleanDesc), maxTokens: 20);
            return SanitizeProductCode(response);
        }

        private string SanitizeProductCode(string code)
        {
            var sanitized = Regex.Replace(code.Trim(), @"[^\w-]", "");
            return sanitized.Length <= 20 ? sanitized : sanitized.Substring(0, 20);
        }

        private async Task<string> GetCompletionAsync(string prompt, double? temperature = null, int? maxTokens = null)
        {
            var requestBody = new
            {
                model = Model,
                messages = new[] { new { role = "user", content = prompt } },
                temperature = temperature ?? DefaultTemperature,
                max_tokens = maxTokens ?? DefaultMaxTokens,
                stream = false
            };

            var jsonResponse = await PostRequestAsync(requestBody);
            return ParseCompletionContent(jsonResponse);
        }

        private string ParseCompletionContent(string jsonResponse)
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                .Trim();
        }

        private async Task<string> PostRequestAsync(object requestBody)
        {
            var json = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpRequestException($"API request failed: {response.StatusCode}\n{errorContent}");
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private string ParseHsCode(string jsonResponse)
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var match = Regex.Match(content, HsCodePattern);
            return match.Success ? match.Value : "";
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class HSCodeRequestException : Exception
    {
        public HSCodeRequestException(string message, Exception inner)
            : base(message, inner) { }
    }
}

