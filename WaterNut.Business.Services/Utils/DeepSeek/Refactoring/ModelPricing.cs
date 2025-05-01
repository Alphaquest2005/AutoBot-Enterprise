using System.Collections.Generic;

// Added for RateLimitException base class

namespace WaterNut.Business.Services.Utils.LlmApi
{
    // Enum remains the same

    // Pricing structure remains the same
    public class ModelPricing
    {
        public decimal InputPricePerMillionTokens { get; set; }
        public decimal OutputPricePerMillionTokens { get; set; }
    }

    // Token usage structure remains the same

    // Custom Exceptions (Keep these accessible)
}