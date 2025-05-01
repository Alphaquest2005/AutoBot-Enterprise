using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class GeminiRequestBody
{
    [JsonProperty("contents")]
    public IEnumerable<GeminiContent> Contents { get; set; }

    [JsonProperty("generationConfig")]
    public GeminiGenerationConfig GenerationConfig { get; set; }

    // Example: Uncomment and populate if you need to override safety settings
    // [JsonProperty("safetySettings", NullValueHandling = NullValueHandling.Ignore)]
    // public IEnumerable<GeminiSafetySetting> SafetySettings { get; set; }
}