using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class GeminiContent
{
    // Gemini API often expects 'role' here too, though maybe optional for simple user prompts
    // Including it can sometimes be necessary depending on the specific API version/endpoint.
    // Uncomment if needed, but often omitted for single-turn user prompts.
    // [JsonProperty("role")]
    // public string Role { get; set; } = "user"; // Default to user

    [JsonProperty("parts")]
    public IEnumerable<GeminiPart> Parts { get; set; }
}