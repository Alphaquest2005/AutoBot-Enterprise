using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace WaterNut.Business.Services.Utils.Claude
{
    /// <summary>
    /// Claude Code API client for high-quality JSON extraction
    /// Uses Claude's superior structured data extraction capabilities
    /// </summary>
    public class ClaudeCodeApiClient
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.anthropic.com/v1";

        public ClaudeCodeApiClient(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = new HttpClient();
            
            // Get API key from environment variable
            _apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("ANTHROPIC_API_KEY environment variable is required");
            }

            // Set up HTTP client headers
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            
            _logger.Information("🔧 **CLAUDE_CODE_CLIENT_INITIALIZED**: Base URL: {BaseUrl}", _baseUrl);
        }

        /// <summary>
        /// Call Claude API for structured JSON extraction
        /// </summary>
        public async Task<string> GetJsonExtractionAsync(string prompt)
        {
            _logger.Information("🎯 **CLAUDE_JSON_REQUEST_START**: Sending JSON extraction request to Claude");

            try
            {
                var requestBody = new
                {
                    model = "claude-3-5-sonnet-20241022",
                    max_tokens = 8192,
                    temperature = 0.1, // Low temperature for consistent structured output
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    }
                };

                var requestJson = JsonConvert.SerializeObject(requestBody, Formatting.Indented);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                _logger.Information("📊 **CLAUDE_REQUEST_DETAILS**: Model=claude-3-5-sonnet, Temperature=0.1, MaxTokens=8192, PromptLength={Length}", 
                    prompt.Length);

                var response = await _httpClient.PostAsync($"{_baseUrl}/messages", requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.Information("🔍 **CLAUDE_HTTP_RESPONSE**: StatusCode={StatusCode}, IsSuccess={IsSuccess}", 
                    response.StatusCode, response.IsSuccessStatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error("❌ **CLAUDE_API_ERROR**: HTTP {StatusCode} - {ResponseContent}", 
                        response.StatusCode, responseContent);
                    throw new HttpRequestException($"Claude API request failed: {response.StatusCode} - {responseContent}");
                }

                // Parse Claude's response
                var claudeResponse = JsonConvert.DeserializeObject<ClaudeResponse>(responseContent);
                var extractedContent = claudeResponse?.Content?.FirstOrDefault()?.Text;

                if (string.IsNullOrEmpty(extractedContent))
                {
                    _logger.Error("❌ **CLAUDE_EMPTY_RESPONSE**: No content in Claude response");
                    throw new InvalidOperationException("Claude API returned empty content");
                }

                _logger.Information("✅ **CLAUDE_JSON_SUCCESS**: Received response, content length: {Length}", 
                    extractedContent.Length);

                return extractedContent;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **CLAUDE_API_EXCEPTION**: Claude API call failed");
                throw;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    #region Response Models

    public class ClaudeResponse
    {
        [JsonProperty("content")]
        public List<ClaudeContent> Content { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("usage")]
        public ClaudeUsage Usage { get; set; }
    }

    public class ClaudeContent
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ClaudeUsage
    {
        [JsonProperty("input_tokens")]
        public int InputTokens { get; set; }

        [JsonProperty("output_tokens")]
        public int OutputTokens { get; set; }
    }

    #endregion
}