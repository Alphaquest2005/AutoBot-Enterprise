using System.Collections.Generic;

namespace WaterNut.Business.Services.Utils.LlmApi;

/// <summary>
/// Represents the overall response from a batch classification API call.
/// </summary>
public class BatchClassificationResponse
{
    /// <summary>
    /// Dictionary mapping Original Description to its ClassificationResult.
    /// Contains only successfully processed items from the batch.
    /// </summary>
    public Dictionary<string, ClassificationResult> Results { get; set; } = new Dictionary<string, ClassificationResult>();
    public List<string> FailedDescriptions { get; set; } = new List<string>(); // Descriptions that failed within the batch
    public decimal TotalCost { get; set; }
    public TokenUsage AggregatedUsage { get; set; } // Optional: Sum of usage if relevant
    public bool IsSuccess { get; set; } // Indicates if the overall batch API call was successful (may have partial item failures)
    public string ErrorMessage { get; set; } // Error for the whole batch call
}