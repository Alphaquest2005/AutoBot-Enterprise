#nullable disable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Added using for ModelPricing
using WaterNut.Business.Services.Utils.LlmApi;


namespace WaterNut.Business.Services.Utils.LlmApi
{
    public interface ILLMProviderStrategy
    {
        LLMProvider ProviderType { get; }
        string Model { get; set; }

        // --- Configuration ---
        string SingleItemPromptTemplate { get; set; }
        string BatchItemPromptTemplate { get; set; }
        string GenerateCodePromptTemplate { get; set; }
        double DefaultTemperature { get; set; }
        int DefaultMaxTokens { get; set; }

        // *** ADDED GetPricing to the interface ***
        /// <summary>
        /// Gets the pricing information for a specific model used by this provider.
        /// </summary>
        /// <param name="modelName">The specific model name.</param>
        /// <returns>Pricing information or null if not found.</returns>
        ModelPricing? GetPricing(string modelName);
        // *** END ADD ***

        // --- Core Methods ---
        Task<ClassificationResponse> GetSingleClassificationAsync(
            string itemDescription, string productCode,
            double? temperatureOverride, int? maxTokensOverride,
            CancellationToken cancellationToken);

        Task<BatchClassificationResponse> GetBatchClassificationAsync(
            List<(string ItemNumber, string ItemDescription, string TariffCode)> items,
            double? temperatureOverride, int? maxTokensOverride,
            CancellationToken cancellationToken);

        Task<ProductCodeResponse> GenerateProductCodeAsync(
            string description,
            CancellationToken cancellationToken);

        // --- Helper ---
        int EstimateTokenCount(string text);
    }
}