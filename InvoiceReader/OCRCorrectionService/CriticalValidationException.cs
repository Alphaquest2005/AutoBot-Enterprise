using System;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Exception thrown when critical template specification validation fails
    /// Enables fail-fast architecture without scattered code changes
    /// Designed for LLM-friendly debugging with comprehensive context
    /// </summary>
    public class CriticalValidationException : Exception
    {
        /// <summary>
        /// The validation layer that failed (e.g., "LAYER_2_DATATYPE_VALIDATION")
        /// </summary>
        public string Layer { get; }
        
        /// <summary>
        /// Specific evidence of the failure for debugging
        /// </summary>
        public string Evidence { get; }
        
        /// <summary>
        /// The document type that was being validated
        /// </summary>
        public string DocumentType { get; }
        
        /// <summary>
        /// Method or context where the validation occurred
        /// </summary>
        public string ValidationContext { get; }

        public CriticalValidationException(string layer, string evidence, string documentType = "", string context = "")
            : base($"Critical validation failed: {layer} - {evidence}")
        {
            Layer = layer;
            Evidence = evidence;
            DocumentType = documentType;
            ValidationContext = context;
        }

        public CriticalValidationException(string layer, string evidence, Exception innerException, string documentType = "", string context = "")
            : base($"Critical validation failed: {layer} - {evidence}", innerException)
        {
            Layer = layer;
            Evidence = evidence;
            DocumentType = documentType;
            ValidationContext = context;
        }

        /// <summary>
        /// Returns a comprehensive description for LLM debugging
        /// </summary>
        public string GetLLMFriendlyDescription()
        {
            return $"🚨 **CRITICAL_VALIDATION_FAILURE**: " +
                   $"Layer={Layer}, " +
                   $"Evidence={Evidence}, " +
                   $"DocumentType={DocumentType}, " +
                   $"Context={ValidationContext}, " +
                   $"InnerException={InnerException?.Message ?? "None"}";
        }
    }
}