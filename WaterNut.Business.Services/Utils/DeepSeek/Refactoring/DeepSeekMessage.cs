#nullable disable // For .NET FW compatibility if needed
using Newtonsoft.Json; // Use attributes for precise JSON naming if needed

namespace WaterNut.Business.Services.Utils.LlmApi
{
    // --- DeepSeek Request Models ---

    public class DeepSeekMessage
    {
        [JsonProperty("role")] // Ensures correct JSON name serialization
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    // --- Gemini Request Models ---

    // Optional: Add safety settings structure if you need to configure them
}