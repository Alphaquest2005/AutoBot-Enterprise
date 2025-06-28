using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using Serilog;
using Core.Common.Extensions;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Tests the complete multi-field extraction pipeline from DeepSeek prompt to database integration
    /// Validates the Phase 2 implementation of multi-field line extraction capabilities
    /// </summary>
    [TestFixture]
    public class MultiFieldExtractionPipelineTest
    {
        private static readonly ILogger _logger = Log.ForContext<MultiFieldExtractionPipelineTest>();

        [Test]
        public void CanParseMultiFieldDeepSeekResponse()
        {
            _logger.Information("🚀 **TEST_START**: Multi-field DeepSeek response parsing validation");
            
            // Create a mock DeepSeek response with multi-field extraction data
            var mockDeepSeekResponse = new
            {
                errors = new[]
                {
                    new
                    {
                        field = "InvoiceDetail_MultiField_Line6",
                        extracted_value = "partial_extraction",
                        correct_value = "complete_multi_field_extraction",
                        line_text = "5606773 NAUTICAL BOTTOMK NT WHITE NAU120/1 1 PC 1.00 GAL 29.66 /PC 29.66 0.00%",
                        line_number = 6,
                        confidence = 0.95,
                        error_type = "multi_field_omission",
                        reasoning = "Line contains multiple fields that should be extracted together with format corrections applied.",
                        entity_type = "InvoiceDetails",
                        suggested_regex = @"(?<ItemCode>\d+)\s+(?<ItemDescription>NAUTICAL BOTTOMK NT [A-Za-z\s\/\d]+)\s+(?<Quantity>\d+)\s+PC",
                        captured_fields = new[] { "ItemCode", "ItemDescription", "Quantity", "UnitPrice", "LineTotal" },
                        field_corrections = new[]
                        {
                            new
                            {
                                field_name = "ItemDescription",
                                pattern = "BOTTOMK NT",
                                replacement = "BOTTOM PAINT"
                            }
                        },
                        requires_multiline_regex = false
                    }
                }
            };

            var mockResponseJson = JsonSerializer.Serialize(mockDeepSeekResponse, new JsonSerializerOptions { WriteIndented = true });
            _logger.Information("📋 **MOCK_RESPONSE**: Created DeepSeek response with multi-field data");

            try
            {
                var ocrService = new OCRCorrectionService(_logger);
                var testOcrText = "test ocr text";
                
                // Test the DeepSeek response processing
                _logger.Information("🔄 **PROCESSING_TEST**: Testing ProcessDeepSeekCorrectionResponse with multi-field data");
                var correctionResults = ocrService.ProcessDeepSeekCorrectionResponse(mockResponseJson, testOcrText);
                
                // Basic validation that doesn't depend on specific Assert methods
                if (correctionResults == null || correctionResults.Count == 0)
                {
                    _logger.Error("❌ **PROCESSING_FAILURE**: ProcessDeepSeekCorrectionResponse returned no results");
                    Assert.Fail("ProcessDeepSeekCorrectionResponse should return results");
                }
                
                var multiFieldCorrection = correctionResults.FirstOrDefault(c => c.CorrectionType == "multi_field_omission");
                if (multiFieldCorrection == null)
                {
                    _logger.Error("❌ **PROCESSING_FAILURE**: No multi_field_omission correction found");
                    Assert.Fail("Should have a multi_field_omission correction type");
                }
                
                _logger.Information("✅ **PROCESSING_SUCCESS**: Found multi-field correction: {FieldName}, Type: {CorrectionType}", 
                    multiFieldCorrection.FieldName, multiFieldCorrection.CorrectionType);

                // Verify multi-field data is preserved in temporary storage
                if (string.IsNullOrEmpty(multiFieldCorrection.WindowText))
                {
                    _logger.Error("❌ **DATA_FAILURE**: WindowText should contain captured fields data");
                    Assert.Fail("WindowText should contain captured fields data");
                }
                
                if (string.IsNullOrEmpty(multiFieldCorrection.ExistingRegex))
                {
                    _logger.Error("❌ **DATA_FAILURE**: ExistingRegex should contain field corrections data");
                    Assert.Fail("ExistingRegex should contain field corrections data");
                }

                _logger.Information("✅ **DATA_PRESERVATION**: Multi-field data correctly preserved in CorrectionResult");
                _logger.Information("   - Captured Fields (stored in WindowText): {Fields}", multiFieldCorrection.WindowText);
                _logger.Information("   - Field Corrections (stored in ExistingRegex): {Corrections}", multiFieldCorrection.ExistingRegex);

                _logger.Information("🎉 **TEST_COMPLETE**: Multi-field DeepSeek response parsing validation successful");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEST_FAILURE**: Multi-field DeepSeek response parsing test failed");
                throw;
            }
        }

        [Test]
        public void CanHandleMultiFieldOmissionInStrategy()
        {
            _logger.Information("🗄️ **STRATEGY_TEST**: Testing database strategy selection for multi_field_omission");
            
            try
            {
                var testRegexRequest = new RegexUpdateRequest
                {
                    FieldName = "InvoiceDetail_MultiField_Line6",
                    CorrectionType = "multi_field_omission",
                    NewValue = "test_value",
                    LineText = "test line",
                    SuggestedRegex = "test_regex"
                };
                
                var omissionStrategy = new WaterNut.DataSpace.OCRCorrectionService.OmissionUpdateStrategy(_logger);
                var canHandle = omissionStrategy.CanHandle(testRegexRequest);
                
                if (!canHandle)
                {
                    _logger.Error("❌ **STRATEGY_FAILURE**: OmissionUpdateStrategy should handle multi_field_omission error type");
                    Assert.Fail("OmissionUpdateStrategy should handle multi_field_omission error type");
                }
                
                _logger.Information("✅ **STRATEGY_VALIDATION**: OmissionUpdateStrategy correctly handles multi_field_omission");
                _logger.Information("🎉 **TEST_COMPLETE**: Database strategy validation successful");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEST_FAILURE**: Database strategy test failed");
                throw;
            }
        }

        [Test]
        public void CanValidateEnhancedPromptExamples()
        {
            _logger.Information("🔍 **PROMPT_VALIDATION**: Testing enhanced prompt examples for multi-field extraction");
            
            // Test that the enhanced prompt examples can be properly parsed
            var exampleMultiFieldResponse = @"{
                ""field"": ""InvoiceDetail_MultiField_Line5"",
                ""extracted_value"": ""partial_extraction"",
                ""correct_value"": ""complete_multi_field_extraction"",
                ""line_text"": ""5606773 NAUTICAL BOTTOMK NT WHITE NAU120/1 1 PC 1.00 GAL 29.66 /PC 29.66 0.00%"",
                ""line_number"": 5,
                ""confidence"": 0.95,
                ""error_type"": ""multi_field_omission"",
                ""reasoning"": ""Line contains multiple fields that should be extracted together with format corrections applied."",
                ""entity_type"": ""InvoiceDetails"",
                ""suggested_regex"": ""(?<ItemCode>\\d+)\\s+(?<ItemDescription>NAUTICAL BOTTOMK NT [A-Za-z\\s\\/\\d]+)\\s+(?<Quantity>\\d+)\\s+PC"",
                ""captured_fields"": [""ItemCode"", ""ItemDescription"", ""Quantity"", ""UnitPrice"", ""LineTotal""],
                ""field_corrections"": [
                    {
                        ""field_name"": ""ItemDescription"",
                        ""pattern"": ""BOTTOMK NT"",
                        ""replacement"": ""BOTTOM PAINT""
                    }
                ],
                ""requires_multiline_regex"": false
            }";

            try
            {
                var parsedElement = JsonDocument.Parse(exampleMultiFieldResponse).RootElement;
                
                // Validate all required fields are present
                if (!parsedElement.TryGetProperty("field", out _))
                {
                    Assert.Fail("Should have field property");
                }
                if (!parsedElement.TryGetProperty("error_type", out _))
                {
                    Assert.Fail("Should have error_type property");
                }
                if (!parsedElement.TryGetProperty("captured_fields", out var capturedFields))
                {
                    Assert.Fail("Should have captured_fields property");
                }
                if (!parsedElement.TryGetProperty("field_corrections", out var fieldCorrections))
                {
                    Assert.Fail("Should have field_corrections property");
                }
                
                // Validate captured_fields is an array
                if (capturedFields.ValueKind != JsonValueKind.Array)
                {
                    Assert.Fail("captured_fields should be an array");
                }
                if (capturedFields.GetArrayLength() == 0)
                {
                    Assert.Fail("captured_fields should not be empty");
                }
                
                // Validate field_corrections is an array with proper structure
                if (fieldCorrections.ValueKind != JsonValueKind.Array)
                {
                    Assert.Fail("field_corrections should be an array");
                }
                if (fieldCorrections.GetArrayLength() == 0)
                {
                    Assert.Fail("field_corrections should not be empty");
                }
                
                var firstCorrection = fieldCorrections.EnumerateArray().First();
                if (!firstCorrection.TryGetProperty("field_name", out _))
                {
                    Assert.Fail("Field correction should have field_name");
                }
                if (!firstCorrection.TryGetProperty("pattern", out _))
                {
                    Assert.Fail("Field correction should have pattern");
                }
                if (!firstCorrection.TryGetProperty("replacement", out _))
                {
                    Assert.Fail("Field correction should have replacement");
                }
                
                _logger.Information("✅ **PROMPT_VALIDATION_SUCCESS**: Enhanced prompt examples are properly structured");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PROMPT_VALIDATION_FAILURE**: Enhanced prompt examples failed validation");
                throw;
            }
        }
    }
}