#nullable disable
using Serilog; // Added
using NUnit.Framework; // Changed
using Moq;
using System;
using System.Net.Http;

namespace WaterNut.Business.Services.Utils.LlmApi.Tests
{
    [TestFixture] // Changed
    public class LlmApiClientFactoryTests
    {
        // NOTE: Testing environment variables directly in unit tests has limitations.
        // Consider abstracting environment access if more complex testing is needed.

        [SetUp] // Changed
        public void TestInitialize()
        {
            // Clear vars before each test for isolation
            Environment.SetEnvironmentVariable("DEEPSEEK_API_KEY", null);
            Environment.SetEnvironmentVariable("GEMINI_API_KEY", null);
        }

        [Test] // Changed
        public void CreateClient_DeepSeek_MissingApiKey_ThrowsException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("GEMINI_API_KEY", "dummy_gemini_key"); // Set other one

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => // Changed
            {
                LlmApiClientFactory.CreateClient(LLMProvider.DeepSeek);
            });
        }

        [Test] // Changed
        public void CreateClient_Gemini_MissingApiKey_ThrowsException()
        {
            // Arrange
            Environment.SetEnvironmentVariable("DEEPSEEK_API_KEY", "dummy_deepseek_key");
            // Environment.SetEnvironmentVariable("GEMINI_API_KEY", ""); // Null is sufficient

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => // Changed
            {
                LlmApiClientFactory.CreateClient(LLMProvider.Gemini);
            });
        }

        [Test] // Changed
        public void CreateClient_DeepSeek_ValidKey_CreatesClientWithDeepSeekStrategy()
        {
            // Arrange
            Environment.SetEnvironmentVariable("DEEPSEEK_API_KEY", "test-ds-key");
            var mockLogger = new Mock<Serilog.ILogger>(); // Changed to Mock<Serilog.ILogger>

            // Act
            var client = LlmApiClientFactory.CreateClient(LLMProvider.DeepSeek, mockLogger.Object); // Pass the mock Serilog logger directly

            // Assert
            Assert.That(client, Is.Not.Null);
            Assert.That(client.Model, Is.EqualTo("deepseek-chat"));
        }

        [Test] // Changed
        public void CreateClient_Gemini_ValidKey_CreatesClientWithGeminiStrategy()
        {
            // Arrange
            Environment.SetEnvironmentVariable("GEMINI_API_KEY", "test-gem-key");
            var mockLogger = new Mock<Serilog.ILogger>(); // Changed to Mock<Serilog.ILogger>

            // Act
            var client = LlmApiClientFactory.CreateClient(LLMProvider.Gemini, mockLogger.Object); // Pass the mock Serilog logger directly

            // Assert
            Assert.That(client, Is.Not.Null);
            Assert.That(client.Model, Is.EqualTo("gemini-1.5-flash-latest"));
        }

        [TearDown] // Changed
        public void TestCleanup()
        {
            // Clean up environment variables after tests
            Environment.SetEnvironmentVariable("DEEPSEEK_API_KEY", null);
            Environment.SetEnvironmentVariable("GEMINI_API_KEY", null);
        }
    }
}