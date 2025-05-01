using Newtonsoft.Json;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class GeminiPart
{
    [JsonProperty("text")]
    public string Text { get; set; }
    // Add other part types like 'inlineData' if needed later
}