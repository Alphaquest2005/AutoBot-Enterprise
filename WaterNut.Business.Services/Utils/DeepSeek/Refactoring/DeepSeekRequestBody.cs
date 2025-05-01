using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaterNut.Business.Services.Utils.LlmApi;

public class DeepSeekRequestBody
{
    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("messages")]
    public IEnumerable<DeepSeekMessage> Messages { get; set; } // Use IEnumerable or List

    [JsonProperty("temperature")]
    public double Temperature { get; set; }

    [JsonProperty("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonProperty("stream")]
    public bool Stream { get; set; } = false;
}