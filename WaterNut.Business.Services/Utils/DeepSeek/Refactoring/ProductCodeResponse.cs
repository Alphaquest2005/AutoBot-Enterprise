namespace WaterNut.Business.Services.Utils.LlmApi;

/// <summary>
/// Represents the response for generating a product code.
/// </summary>
public class ProductCodeResponse
{
    public string ProductCode { get; set; } // Can be "ERROR-CODE" etc. on failure
    public decimal Cost { get; set; }
    public TokenUsage Usage { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}