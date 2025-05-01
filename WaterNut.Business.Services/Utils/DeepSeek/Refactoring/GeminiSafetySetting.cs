using Newtonsoft.Json;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class GeminiSafetySetting
{
    [JsonProperty("category")]
    public string Category { get; set; } // e.g., "HARM_CATEGORY_HARASSMENT"

    [JsonProperty("threshold")]
    public string Threshold { get; set; } // e.g., "BLOCK_MEDIUM_AND_ABOVE", "BLOCK_ONLY_HIGH", "BLOCK_NONE"
}