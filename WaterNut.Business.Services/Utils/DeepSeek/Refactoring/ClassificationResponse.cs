namespace WaterNut.Business.Services.Utils.LlmApi;

/// <summary>
/// Represents the overall response from a single classification API call.
/// </summary>
public class ClassificationResponse
{
    public ClassificationResult Result { get; set; } // Can be null if API call or parsing failed
    public decimal Cost { get; set; }
    public TokenUsage Usage { get; set; }
    public bool IsSuccess { get; set; } // Indicates if the overall operation succeeded
    public string ErrorMessage { get; set; } // Optional error details
}