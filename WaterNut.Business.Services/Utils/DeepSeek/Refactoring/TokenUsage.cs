namespace WaterNut.Business.Services.Utils.LlmApi;

public class TokenUsage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public bool IsEstimated { get; set; } = false;
}