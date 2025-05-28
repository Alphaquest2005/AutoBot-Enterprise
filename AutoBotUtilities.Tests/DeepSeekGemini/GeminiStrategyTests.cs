#nullable disable
using Serilog; // Added
using NUnit.Framework;
using Moq;
using Moq.Protected; // Needed for HttpMessageHandler
using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly; // Added
using Polly.Retry; // Added
using Newtonsoft.Json; // Added for escaping test JSON

namespace WaterNut.Business.Services.Utils.LlmApi.Tests
{
    [TestFixture]
    public class GeminiStrategyTests
    {
        // Mocks for dependencies required by LlmStrategyBase
        private Mock<Serilog.ILogger> _mockLogger; // Changed to Mock<Serilog.ILogger>
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _mockHttpClient;
        private AsyncRetryPolicy _testRetryPolicy; // Use a simple policy for tests

        // The Strategy under test (as the interface type)
        private GeminiStrategy _strategyConcrete; // Keep concrete reference if needed
        private ILLMProviderStrategy _strategy => _strategyConcrete; // Use interface
        private string _dummyApiKey = "test-gem-key";

        [SetUp]
        public void TestInitialize()
        {
            _mockLogger = new Mock<Serilog.ILogger>(); // Changed to Mock<Serilog.ILogger>
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = TestHelpers.CreateMockHttpClient(_mockHttpHandler);

            // Create a simple retry policy for testing (0 retries)
            _testRetryPolicy = Policy.Handle<Exception>()
                                     .WaitAndRetryAsync(0, retryAttempt => TimeSpan.Zero);

            // Create the concrete strategy instance, providing all dependencies
            _strategyConcrete = new GeminiStrategy(_dummyApiKey, _mockLogger.Object, _mockHttpClient, _testRetryPolicy); // Pass the mock Serilog logger

            // Set required properties (prompts)
            _strategyConcrete.SingleItemPromptTemplate = "Single: __DESCRIPTION_HERE__ Code: __PRODUCT_CODE_HERE__";
            _strategyConcrete.BatchItemPromptTemplate = "Batch:\nProduct List:";
            _strategyConcrete.GenerateCodePromptTemplate = "Generate Code: {0}";
        }

        // Test GetDefaultModelName indirectly via property access
        [Test]
        public void DefaultModel_IsSetCorrectly()
        {
            Assert.That(_strategy.Model, Is.EqualTo("gemini-1.5-flash-latest"));
        }

        // Test Pricing lookup via interface method
        [Test]
        public void GetPricing_ReturnsCorrectPricing()
        {
            var pricing = _strategy.GetPricing("gemini-1.5-flash-latest"); // Use default model
            Assert.That(pricing, Is.Not.Null);
            Assert.That(pricing.InputPricePerMillionTokens, Is.EqualTo(0.35m)); // Example price
            Assert.That(pricing.OutputPricePerMillionTokens, Is.EqualTo(1.05m)); // Example price

            // Example test for another model if defined
            var proPricing = _strategy.GetPricing("gemini-1.5-pro-latest");
            Assert.That(proPricing, Is.Not.Null);
            Assert.That(proPricing.InputPricePerMillionTokens, Is.EqualTo(3.50m)); // Example price
        }

        [Test]
        public void GetPricing_UnknownModel_ReturnsNull()
        {
            var pricing = _strategy.GetPricing("unknown-model");
            Assert.That(pricing, Is.Null);
        }

        // --- Test High-Level Methods (Indirectly Tests Helpers) ---

        [Test]
        public async Task GetSingleClassificationAsync_Success_ReturnsParsedResultAndCost()
        {
            // Arrange: Mock the HTTP response the strategy will receive
            string fakeLlmInternalJson = @"{ \""items\"":[{\""hs_code\"":\""87654321\"",\""category\"":\""Gemini Cat\"",\""category_hs_code\"":\""87650000\""}]}";
            string fakeApiJsonResponse = $@"{{
                ""candidates"": [ {{
                    ""content"": {{ ""parts"": [ {{ ""text"": {JsonConvert.ToString(fakeLlmInternalJson)} }} ] }},
                    ""finishReason"": ""STOP""
                }} ],
                ""usageMetadata"": {{ ""promptTokenCount"": 20, ""candidatesTokenCount"": 30, ""totalTokenCount"": 50 }}
            }}"; // Structure matches Gemini

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeApiJsonResponse) });

            string description = "Test Gemini Item"; string productCode = "G001";

            // Act: Call public interface method
            var response = await _strategy.GetSingleClassificationAsync(description, productCode, null, null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.True); Assert.That(response.Result, Is.Not.Null);
            Assert.That(response.Result.TariffCode, Is.EqualTo("87654321"));
            Assert.That(response.Result.Category, Is.EqualTo("Gemini Cat"));
            Assert.That(response.Result.CategoryHsCode, Is.EqualTo("87650000"));
            // Assert.That(response.Result.ItemNumber, Is.EqualTo(productCode)); // Check if product code is correctly set inside strategy
            Assert.That(response.Usage.IsEstimated, Is.False); Assert.That(response.Usage.InputTokens, Is.EqualTo(20)); Assert.That(response.Usage.OutputTokens, Is.EqualTo(30)); // Based on candidatesTokenCount
            Assert.That(response.Cost, Is.GreaterThan(0));
            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GetBatchClassificationAsync_Success_ReturnsParsedResultsAndCost()
        {
            // Arrange
            string fakeLlmJsonOutput = @"{ ""items"": [
                { ""original_description"": ""Gem Prod 1"", ""product_code"": ""G1-GEN"", ""category"": ""GCat1"", ""category_hs_code"": ""33330000"", ""hs_code"": ""33333333"" },
                { ""original_description"": ""Gem Prod 2"", ""product_code"": ""G2"", ""category"": ""GCat2"", ""category_hs_code"": ""44440000"", ""hs_code"": ""44444444"" } ]}";
            string fakeApiJsonResponse = $@"{{
                ""candidates"": [ {{
                    ""content"": {{ ""parts"": [ {{ ""text"": {JsonConvert.ToString(fakeLlmJsonOutput)} }} ] }},
                    ""finishReason"": ""STOP""
                }} ],
                ""usageMetadata"": {{ ""promptTokenCount"": 210, ""candidatesTokenCount"": 310, ""totalTokenCount"": 520 }} }}";

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeApiJsonResponse) });
            var items = new List<(string, string, string)> { ("G1", "Gem Prod 1", ""), ("G2", "Gem Prod 2", "") };

            // Act: Call public interface method
            var response = await _strategy.GetBatchClassificationAsync(items, null, null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.True); Assert.That(response.Results, Is.Not.Null); Assert.That(response.Results.Count, Is.EqualTo(2)); Assert.That(response.FailedDescriptions, Is.Empty);
            Assert.That(response.Results.ContainsKey("Gem Prod 1")); Assert.That(response.Results["Gem Prod 1"].TariffCode, Is.EqualTo("33333333"));
            Assert.That(response.Results.ContainsKey("Gem Prod 2")); Assert.That(response.Results["Gem Prod 2"].TariffCode, Is.EqualTo("44444444"));
            Assert.That(response.AggregatedUsage.IsEstimated, Is.False); Assert.That(response.AggregatedUsage.InputTokens, Is.EqualTo(210)); Assert.That(response.AggregatedUsage.OutputTokens, Is.EqualTo(310));
            Assert.That(response.TotalCost, Is.GreaterThan(0));
            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GenerateProductCodeAsync_Success_ReturnsCodeAndCost()
        {
            // Arrange
            string fakeApiJsonResponse = $@"{{
                ""candidates"": [ {{
                    ""content"": {{ ""parts"": [ {{ ""text"": ""GEM-CODE-456"" }} ] }},
                    ""finishReason"": ""STOP""
                }} ],
                ""usageMetadata"": {{ ""promptTokenCount"": 12, ""candidatesTokenCount"": 6, ""totalTokenCount"": 18 }} }}";

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeApiJsonResponse) });
            string description = "Generate a gemini code";

            // Act: Call public interface method
            var response = await _strategy.GenerateProductCodeAsync(description, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.True); Assert.That(response.ProductCode, Is.EqualTo("GEM-CODE-456"));
            Assert.That(response.Usage.IsEstimated, Is.False); Assert.That(response.Usage.InputTokens, Is.EqualTo(12)); Assert.That(response.Usage.OutputTokens, Is.EqualTo(6));
            Assert.That(response.Cost, Is.GreaterThan(0));
            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GetSingleClassificationAsync_ApiReturnsSafetyBlock_ReturnsErrorResponse()
        {
            // Arrange: Mock the HTTP response for a safety block
            string fakeBlockedResponse = @"{
                ""candidates"": [ {
                    /* No ""content"" block */
                    ""finishReason"": ""SAFETY"", ""index"": 0,
                    ""safetyRatings"": [ { ""category"": ""HARM_CATEGORY_DANGEROUS_CONTENT"", ""probability"": ""HIGH"" } ]
                } ],
                 ""promptFeedback"": { ""blockReason"": ""SAFETY"", ""safetyRatings"": [ /*...*/ ] }
                 /* No usageMetadata typically */
            }";
            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeBlockedResponse) }); // API call succeeds (200 OK) but content is blocked

            // Act
            var response = await _strategy.GetSingleClassificationAsync("Risky Item", "R001", null, null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Result, Is.Null);
            Assert.That(response.ErrorMessage, Does.Contain("SAFETY")); // Strategy should throw LlmApiException
            Assert.That(response.Usage.IsEstimated, Is.True); // No usage data available
            Assert.That(response.Cost, Is.EqualTo(0m)); // Cost is 0

            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        // Removed tests for protected methods BuildRequestBody, ParseProviderResponse, AddAuthentication
    }
}