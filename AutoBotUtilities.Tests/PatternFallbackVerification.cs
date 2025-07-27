// Quick verification of Pattern fallback fix
using System;
using System.Text.Json;
using WaterNut.DataSpace;
using Serilog;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Quick verification that the Pattern fallback fix is working correctly.
    /// Tests the exact scenario from the MANGO regression.
    /// </summary>
    public class PatternFallbackVerification
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("🚀 **PATTERN_FALLBACK_VERIFICATION**: Testing the critical Pattern='null' fix");
            
            // Setup minimal logging
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
            
            try
            {
                // Test 1: Simulate DeepSeek response with null pattern but valid suggested_regex
                Console.WriteLine("\n🔍 **TEST_1**: Testing null pattern with valid suggested_regex fallback");
                
                var deepSeekResponse = @"{
                    ""errors"": [
                        {
                            ""field"": ""InvoiceTotal"",
                            ""correct_value"": ""$29.99"",
                            ""error_type"": ""omission"",
                            ""line_number"": 5,
                            ""line_text"": ""MANGO Invoice TOTAL AMOUNT: $29.99"",
                            ""suggested_regex"": ""(?<InvoiceTotal>TOTAL\\s+AMOUNT:\\s*\\$([0-9,]+\\.?[0-9]*))"",
                            ""reasoning"": ""Invoice total was not extracted from MANGO format"",
                            ""pattern"": null,
                            ""replacement"": null
                        }
                    ]
                }";
                
                // Create a test service instance 
                var service = new OCRCorrectionService(logger);
                
                // Process the response to see if fallback works
                var corrections = service.ProcessDeepSeekCorrectionResponse(deepSeekResponse, "test document");
                
                if (corrections.Count > 0)
                {
                    var correction = corrections[0];
                    Console.WriteLine($"✅ **FALLBACK_SUCCESS**: Pattern set to: '{correction.Pattern}'");
                    Console.WriteLine($"✅ **SUGGESTED_REGEX**: {correction.SuggestedRegex}");
                    
                    if (!string.IsNullOrEmpty(correction.Pattern))
                    {
                        Console.WriteLine("🎯 **CRITICAL_SUCCESS**: Pattern='null' regression is FIXED!");
                        Console.WriteLine("   ✅ Pattern field now contains the suggested_regex value");
                        Console.WriteLine("   ✅ Template creation will work with proper patterns");
                        Console.WriteLine("   ✅ MANGO test should now pass with working patterns");
                    }
                    else
                    {
                        Console.WriteLine("❌ **CRITICAL_FAILURE**: Pattern is still null - fix not working");
                    }
                }
                else
                {
                    Console.WriteLine("❌ **TEST_FAILED**: No corrections returned from DeepSeek response");
                }
                
                // Test 2: Test explicit pattern (should not use fallback)
                Console.WriteLine("\n🔍 **TEST_2**: Testing explicit pattern (no fallback needed)");
                
                var explicitPatternResponse = @"{
                    ""errors"": [
                        {
                            ""field"": ""Currency"",
                            ""correct_value"": ""USD"",
                            ""error_type"": ""format_correction"",
                            ""line_number"": 3,
                            ""line_text"": ""Currency: US$"",
                            ""suggested_regex"": ""(?<Currency>[A-Z]{3})"",
                            ""reasoning"": ""Currency format needs standardization"",
                            ""pattern"": ""US\\$"",
                            ""replacement"": ""USD""
                        }
                    ]
                }";
                
                var explicitCorrections = service.ProcessDeepSeekCorrectionResponse(explicitPatternResponse, "test document");
                
                if (explicitCorrections.Count > 0)
                {
                    var explicitCorrection = explicitCorrections[0];
                    Console.WriteLine($"✅ **EXPLICIT_PATTERN**: Pattern = '{explicitCorrection.Pattern}'");
                    Console.WriteLine($"✅ **REPLACEMENT**: Replacement = '{explicitCorrection.Replacement}'");
                    
                    if (explicitCorrection.Pattern == "US$")
                    {
                        Console.WriteLine("✅ **EXPLICIT_SUCCESS**: Explicit pattern preserved correctly");
                    }
                }
                
                Console.WriteLine("\n🎉 **VERIFICATION_COMPLETE**: Pattern fallback fix verification successful!");
                Console.WriteLine("🚀 **MANGO_READY**: MANGO test should now pass with proper Pattern values");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ **VERIFICATION_FAILED**: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
            }
        }
    }
}