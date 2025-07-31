// File: OCRCorrectionService/FallbackConfiguration.cs
// Fallback Configuration System - Controls logic fallbacks that mask proper system function
using System;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// Controls fallback behavior in OCR correction service - PREVENTS BUILD DEBUGGING NIGHTMARES
    /// Separates legitimate fallbacks (Gemini LLM) from problematic ones (masking system failures)
    /// </summary>
    public class FallbackConfiguration
    {
        /// <summary>
        /// Controls logic fallbacks that mask proper system function
        /// FALSE = Fail-fast when no corrections/templates/mappings found (recommended)
        /// TRUE = Return empty results and continue processing (legacy behavior)
        /// </summary>
        public bool EnableLogicFallbacks { get; set; } = false;
        
        /// <summary>
        /// Controls Gemini LLM fallback when DeepSeek fails (special design requirement)
        /// TRUE = Use Gemini when DeepSeek unavailable (recommended)
        /// FALSE = Fail immediately when DeepSeek unavailable
        /// </summary>
        public bool EnableGeminiFallback { get; set; } = true;
        
        /// <summary>
        /// Controls template system fallback to hardcoded prompts
        /// FALSE = Fail when file-based templates unavailable (recommended for database-driven)
        /// TRUE = Fall back to hardcoded prompts when templates fail
        /// </summary>
        public bool EnableTemplateFallback { get; set; } = false;
        
        /// <summary>
        /// Controls DocumentType assumption fallbacks 
        /// FALSE = Fail when DocumentType cannot be determined (recommended)
        /// TRUE = Assume "Invoice" when DocumentType unknown
        /// </summary>
        public bool EnableDocumentTypeAssumption { get; set; } = false;
        
        /// <summary>
        /// Creates a default configuration with recommended settings for database-driven operation
        /// All fallbacks disabled except Gemini LLM fallback (which is a legitimate design requirement)
        /// </summary>
        public static FallbackConfiguration CreateDefault()
        {
            return new FallbackConfiguration
            {
                EnableLogicFallbacks = false,           // Recommended: Fail-fast on missing data
                EnableGeminiFallback = true,            // Recommended: Keep LLM redundancy
                EnableTemplateFallback = false,         // Recommended: Force template system usage
                EnableDocumentTypeAssumption = false    // Recommended: Force proper mapping
            };
        }
        
        /// <summary>
        /// Creates a legacy-compatible configuration that enables all fallbacks
        /// Use only for backward compatibility during transition period
        /// </summary>
        public static FallbackConfiguration CreateLegacy()
        {
            return new FallbackConfiguration
            {
                EnableLogicFallbacks = true,
                EnableGeminiFallback = true,
                EnableTemplateFallback = true,
                EnableDocumentTypeAssumption = true
            };
        }
    }
}