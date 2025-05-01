#nullable disable
namespace WaterNut.Business.Services.Utils.LlmApi
{
    /// <summary>
    /// Represents the extracted classification data for a single item.
    /// </summary>
    public class ClassificationResult
    {
        public string OriginalDescription { get; set; } // Include for context if needed
        public string ItemNumber { get; set; }
        public string TariffCode { get; set; }
        public string Category { get; set; }
        public string CategoryHsCode { get; set; }
        public bool ParsedSuccessfully { get; set; } = true; // Flag success/failure of domain parsing
    }
}