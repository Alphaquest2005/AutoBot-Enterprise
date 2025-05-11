#nullable disable
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly; // Required for Policy
using Polly.Retry; // Required for AsyncRetryPolicy

namespace WaterNut.Business.Services.Utils.LlmApi.Tests
{
    [TestFixture]
    public class DeepSeekStrategyTests
    {
        // Mocks for dependencies required by LlmStrategyBase
        private Mock<ILogger> _mockLogger;
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _mockHttpClient;
        private AsyncRetryPolicy _testRetryPolicy; // Correct Type

        // The Strategy under test (as the interface type, but create concrete)
        private DeepSeekStrategy _strategyConcrete; // Keep concrete reference for direct testing if needed
        private ILLMProviderStrategy _strategy => _strategyConcrete; // Use interface for most interactions
        private string _dummyApiKey = "test-ds-key";

        [SetUp]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger>();
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _mockHttpClient = TestHelpers.CreateMockHttpClient(_mockHttpHandler); // Use helper

            // *** CORRECTED: Create a minimal AsyncRetryPolicy that does 0 retries ***
            _testRetryPolicy = Policy.Handle<Exception>()
                                     .WaitAndRetryAsync(0, retryAttempt => TimeSpan.Zero);

            // Create the concrete strategy instance, providing all dependencies
            _strategyConcrete = new DeepSeekStrategy(_dummyApiKey, _mockLogger.Object, _mockHttpClient, _testRetryPolicy);

            // Set required properties (prompts)
            _strategyConcrete.SingleItemPromptTemplate = "Single: __DESCRIPTION_HERE__ Code: __PRODUCT_CODE_HERE__";
            _strategyConcrete.BatchItemPromptTemplate = "Batch:\nProduct List:";
            _strategyConcrete.GenerateCodePromptTemplate = "Generate Code: {0}";
        }

        // Test GetDefaultModelName indirectly via property access
        [Test]
        public void DefaultModel_IsSetCorrectly()
        {
            // Access via interface property
            Assert.That(_strategy.Model, Is.EqualTo("deepseek-chat"));
        }

        // Test Pricing lookup via interface method
        [Test]
        public void GetPricing_ReturnsCorrectPricing()
        {
            // Call via interface variable
            var pricing = _strategy.GetPricing("deepseek-chat");
            Assert.That(pricing, Is.Not.Null);
            Assert.That(pricing.InputPricePerMillionTokens, Is.EqualTo(0.14m));
            Assert.That(pricing.OutputPricePerMillionTokens, Is.EqualTo(0.28m));
        }

        [Test]
        public void GetPricing_UnknownModel_ReturnsNull()
        {
            // Call via interface variable
            var pricing = _strategy.GetPricing("unknown-model");
            Assert.That(pricing, Is.Null);
        }

        // --- Test High-Level Methods (Indirectly Tests Helpers) ---

        [Test]
        public async Task GetSingleClassificationAsync_Success_ReturnsParsedResultAndCost()
        {
            // Arrange: Mock the HTTP response the strategy will receive

            // 1. Define the inner JSON structure the LLM is supposed to return
            var llmOutputObject = new
            {
                items = new[] {
                    new {
                        hs_code = "12345678",
                        category = "Test Cat", // Correct field name
                        category_hs_code = "12340000"
                        // original_description and product_code are added by the client/strategy, not expected from LLM here
                    }
                }
            };
            // 2. Serialize this inner object to a JSON string
            string llmJsonContent = JsonConvert.SerializeObject(llmOutputObject);

            // 3. Construct the outer API response, embedding the inner JSON string correctly
            string fakeApiJsonResponse = $@"{{
                ""id"": ""chatcmpl_xxx"", ""object"": ""chat.completion"", ""created"": 0, ""model"": ""deepseek-chat"",
                ""choices"": [ {{ ""message"": {{ ""content"": {JsonConvert.ToString(llmJsonContent)} }} }} ],
                ""usage"": {{ ""prompt_tokens"": 15, ""completion_tokens"": 25 }} }}";
            // JsonConvert.ToString() handles escaping the inner JSON string for embedding

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeApiJsonResponse) });

            string description = "Test Item";
            string productCode = "T001";

            // Act
            var response = await _strategy.GetSingleClassificationAsync(description, productCode, null, null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Result, Is.Not.Null);
            Assert.That(response.Result.TariffCode, Is.EqualTo("12345678"));
            // *** This assertion should now pass ***
            Assert.That(response.Result.Category, Is.EqualTo("Test Cat"));
            Assert.That(response.Result.CategoryHsCode, Is.EqualTo("12340000"));
            Assert.That(response.Usage.IsEstimated, Is.False);
            Assert.That(response.Usage.InputTokens, Is.EqualTo(15));
            Assert.That(response.Usage.OutputTokens, Is.EqualTo(25));
            Assert.That(response.Cost, Is.GreaterThan(0));

            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GetBatchClassificationAsync_Success_ReturnsParsedResultsAndCost()
        {
            // Arrange
            string fakeLlmJsonOutput = @"{ ""items"": [
                { ""original_description"": ""Product One"", ""product_code"": ""P1-GEN"", ""category"": ""Cat1"", ""category_hs_code"": ""11110000"", ""hs_code"": ""11111111"" },
                { ""original_description"": ""Product Two"", ""product_code"": ""P2"", ""category"": ""Cat2"", ""category_hs_code"": ""22220000"", ""hs_code"": ""22222222"" } ]}";
            string fakeApiJsonResponse = $@"{{
                ""id"": ""chatcmpl_batch"", ""choices"": [ {{ ""message"": {{ ""content"": {JsonConvert.ToString(fakeLlmJsonOutput)} }} }} ],
                ""usage"": {{ ""prompt_tokens"": 150, ""completion_tokens"": 250 }} }}";

            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeApiJsonResponse) });
            var items = new List<(string, string, string)> { ("P1", "Product One", ""), ("P2", "Product Two", "") };

            // Act: Call public interface method
            var response = await _strategy.GetBatchClassificationAsync(items, null, null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.True); Assert.That(response.Results, Is.Not.Null); Assert.That(response.Results.Count, Is.EqualTo(2)); Assert.That(response.FailedDescriptions, Is.Empty);
            Assert.That(response.Results.ContainsKey("Product One")); Assert.That(response.Results["Product One"].TariffCode, Is.EqualTo("11111111"));
            Assert.That(response.Results.ContainsKey("Product Two")); Assert.That(response.Results["Product Two"].TariffCode, Is.EqualTo("22222222"));
            Assert.That(response.AggregatedUsage.IsEstimated, Is.False); Assert.That(response.AggregatedUsage.InputTokens, Is.EqualTo(150)); Assert.That(response.AggregatedUsage.OutputTokens, Is.EqualTo(250));
            Assert.That(response.TotalCost, Is.GreaterThan(0));
            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GenerateProductCodeAsync_Success_ReturnsCodeAndCost()
        {
            // Arrange
            string fakeApiJsonResponse = @"{ ""choices"": [ { ""message"": { ""content"": ""GEN-CODE-123"" } } ], ""usage"": { ""prompt_tokens"": 10, ""completion_tokens"": 5 } }";
            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(fakeApiJsonResponse) });
            string description = "Generate a code for this";

            // Act: Call public interface method
            var response = await _strategy.GenerateProductCodeAsync(description, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.True); Assert.That(response.ProductCode, Is.EqualTo("GEN-CODE-123"));
            Assert.That(response.Usage.IsEstimated, Is.False); Assert.That(response.Usage.InputTokens, Is.EqualTo(10)); Assert.That(response.Usage.OutputTokens, Is.EqualTo(5));
            Assert.That(response.Cost, Is.GreaterThan(0));
            _mockHttpHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task GetSingleClassificationAsync_ApiFailure_ReturnsErrorResponse()
        {
            // Arrange: Mock the HTTP response with an error status code
            _mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Content = new StringContent("{\"error\":{\"message\":\"Server error\"}}")
                });

            string description = "Test Item Fail";
            string productCode = "F001";
            int estimatedInputTokens = _strategyConcrete.EstimateTokenCount( // Estimate input tokens like the code would
                _strategyConcrete.SingleItemPromptTemplate
                    .Replace("__DESCRIPTION_HERE__", description) // Use sanitized description if needed
                    .Replace("__PRODUCT_CODE_HERE__", productCode)
            );
            decimal expectedInputCost = CalculateExpectedCost(estimatedInputTokens, 0); // Calculate cost for input only

            // Act
            var response = await _strategy.GetSingleClassificationAsync(description, productCode, null, null, CancellationToken.None).ConfigureAwait(false);

            // Assert
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Result, Is.Null);
            Assert.That(response.ErrorMessage, Does.Contain("HTTP Request Failed")); // Error from PostRequestAsync exception
            Assert.That(response.Usage.IsEstimated, Is.True);
            Assert.That(response.Usage.InputTokens, Is.EqualTo(estimatedInputTokens)); // Verify estimated input tokens
            Assert.That(response.Usage.OutputTokens, Is.EqualTo(0)); // Output tokens should be 0

            // *** CORRECTED ASSERTION: Check against expected INPUT cost ***
            Assert.That(response.Cost, Is.EqualTo(expectedInputCost).Within(0.000001m), "Cost should reflect estimated input tokens on failure.");

            _mockHttpHandler.Protected().Verify("SendAsync", Times.Exactly(1), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        // Add helper to calculate cost within the test class for clarity
        private decimal CalculateExpectedCost(int inputTokens, int outputTokens)
        {
            var pricing = _strategyConcrete.GetPricing(_strategyConcrete.Model); // Use concrete strategy to get pricing
            if (pricing == null) return 0m;
            decimal inputCost = ((decimal)inputTokens / 1_000_000m) * pricing.InputPricePerMillionTokens;
            decimal outputCost = ((decimal)outputTokens / 1_000_000m) * pricing.OutputPricePerMillionTokens;
            return inputCost + outputCost;
        }
    }
}