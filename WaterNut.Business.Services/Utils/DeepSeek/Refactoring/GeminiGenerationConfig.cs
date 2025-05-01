using Newtonsoft.Json;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class GeminiGenerationConfig
{
    [JsonProperty("temperature")]
    public double Temperature { get; set; }

    [JsonProperty("maxOutputTokens")]
    public int MaxOutputTokens { get; set; }

    // Add other config options like topP, topK, candidateCount if needed
    // Example: Ensure nulls are ignored if not set using NullValueHandling in JsonSerializerSettings
    // [JsonProperty("candidateCount", NullValueHandling = NullValueHandling.Ignore)]
    // public int? CandidateCount { get; set; }

    // [JsonProperty("topP", NullValueHandling = NullValueHandling.Ignore)]
    // public double? TopP { get; set; }

    // [JsonProperty("topK", NullValueHandling = NullValueHandling.Ignore)]
    // public int? TopK { get; set; }
}