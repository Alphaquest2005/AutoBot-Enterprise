#nullable disable
using Moq;
using Moq.Protected; // Required for mocking HttpMessageHandler
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WaterNut.Business.Services.Utils.LlmApi.Tests
{
    /// <summary>
    /// Provides helper methods for unit testing, particularly for mocking HttpClient.
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Creates a Moq HttpMessageHandler setup to return a specific response.
        /// </summary>
        /// <param name="responseContent">The string content to return in the response.</param>
        /// <param name="statusCode">The HTTP status code to return.</param>
        /// <param name="verifyable">Whether to mark the setup as Verifiable.</param>
        /// <returns>A configured Mock of HttpMessageHandler.</returns>
        public static Mock<HttpMessageHandler> CreateMockHttpHandler(
            string responseContent,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            bool verifyable = true)
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            var setup = mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", // Method name to mock
                    ItExpr.IsAny<HttpRequestMessage>(), // Match any request message
                    ItExpr.IsAny<CancellationToken>() // Match any cancellation token
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent ?? string.Empty), // Ensure content is not null
                });

            if (verifyable)
            {
                setup.Verifiable();
            }

            return mockHandler;
        }

        /// <summary>
        /// Creates an HttpClient instance that uses the provided mocked HttpMessageHandler.
        /// </summary>
        /// <param name="mockHandler">The mocked HttpMessageHandler.</param>
        /// <returns>An HttpClient instance configured for testing.</returns>
        public static HttpClient CreateMockHttpClient(Mock<HttpMessageHandler> mockHandler)
        {
            if (mockHandler == null)
                throw new ArgumentNullException(nameof(mockHandler));

            return new HttpClient(mockHandler.Object)
            {
                // Set a dummy base address. Specific request URLs are usually set directly
                // in the code or strategy, so this often doesn't matter.
                BaseAddress = new Uri("http://dummy-llm-provider.test/")
            };
        }

        /// <summary>
        /// Creates a Moq HttpMessageHandler setup to throw a specific exception.
        /// </summary>
        /// <typeparam name="TException">The type of exception to throw.</typeparam>
        /// <param name="exceptionToThrow">The exception instance to throw.</param>
        /// <returns>A configured Mock of HttpMessageHandler.</returns>
        public static Mock<HttpMessageHandler> CreateMockHttpHandlerThatThrows<TException>(TException exceptionToThrow)
            where TException : Exception
        {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(exceptionToThrow); // Throw the specified exception

            return mockHandler;
        }
    }
}