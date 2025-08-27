using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;
using static AutoBotUtilities.Tests.TestHelpers;
using Core.Common.Extensions;
using Serilog.Events;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;
    
    [TestFixture]
    [Category("DeepSeekDiagnostic")]
    [Explicit("Run manually to diagnose DeepSeek detection issues")]
    public class OCRCorrectionService_DeepSeekDiagnosticTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;
        private string _actualAmazonInvoiceText;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
            _logger.Information("=== Starting DeepSeek Diagnostic Tests ===");
            
            // Load actual Amazon invoice text data
            var testDataPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", "Amazon.com - Order 112-9126443-1163432.pdf.txt");
            _actualAmazonInvoiceText = File.ReadAllText(testDataPath);
            _logger.Information("Loaded Amazon invoice text: {Length} characters", _actualAmazonInvoiceText.Length);
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        private ShipmentInvoice CreateAmazonInvoiceFromActualData()
        {
            // Create invoice matching the actual Amazon data from logs
            return new ShipmentInvoice
            {
                InvoiceNo = "112-9126443-1163432",
                InvoiceDate = new DateTime(2025, 4, 15),
                InvoiceTotal = 166.30,
                SubTotal = 161.95,
                TotalInternalFreight = 6.99,
                TotalOtherCost = 11.34,
                TotalInsurance = -6.99, // Gift Card Amount (already parsed correctly)
                TotalDeduction = null,   // This is what should be detected as missing (Free Shipping total)
                SupplierName = "Amazon.com Services, Inc",
                Currency = "USD"
            };
        }

        #region Test 1: CleanTextForAnalysis Method
        
        [Test]
        public void CleanTextForAnalysis_WithAmazonInvoiceText_ShouldPreserveGiftCardAndFreeShipping()
        {
            _logger.Information("🔍 **TEST_1_START**: Testing CleanTextForAnalysis method with actual Amazon invoice text");
            
            // Act - CleanTextForAnalysis is a public method, call directly
            var cleanedText = _service.CleanTextForAnalysis(_actualAmazonInvoiceText);
            
            // Log results for analysis
            _logger.Information("🔍 **ORIGINAL_TEXT_LENGTH**: {Length} characters", _actualAmazonInvoiceText.Length);
            _logger.Information("🔍 **CLEANED_TEXT_LENGTH**: {Length} characters", cleanedText?.Length ?? 0);
            
            // Check if critical financial patterns are preserved
            bool originalHasGiftCard = _actualAmazonInvoiceText.Contains("Gift Card Amount: -$6.99");
            bool cleanedHasGiftCard = cleanedText?.Contains("Gift Card Amount: -$6.99") == true;
            _logger.Information("🔍 **GIFT_CARD_CHECK**: Original={Original}, Cleaned={Cleaned}", originalHasGiftCard, cleanedHasGiftCard);
            
            bool originalHasFreeShipping1 = _actualAmazonInvoiceText.Contains("Free Shipping: -$0.46");
            bool cleanedHasFreeShipping1 = cleanedText?.Contains("Free Shipping: -$0.46") == true;
            _logger.Information("🔍 **FREE_SHIPPING_1_CHECK**: Original={Original}, Cleaned={Cleaned}", originalHasFreeShipping1, cleanedHasFreeShipping1);
            
            bool originalHasFreeShipping2 = _actualAmazonInvoiceText.Contains("Free Shipping: -$6.53");
            bool cleanedHasFreeShipping2 = cleanedText?.Contains("Free Shipping: -$6.53") == true;
            _logger.Information("🔍 **FREE_SHIPPING_2_CHECK**: Original={Original}, Cleaned={Cleaned}", originalHasFreeShipping2, cleanedHasFreeShipping2);
            
            // Show first 500 chars of cleaned text for comparison
            if (!string.IsNullOrEmpty(cleanedText))
            {
                _logger.Information("🔍 **CLEANED_TEXT_PREVIEW**: {Preview}", 
                    cleanedText.Length > 500 ? cleanedText.Substring(0, 500) + "..." : cleanedText);
            }
            
            // Assertions
            Assert.That(cleanedText, Is.Not.Null, "CleanTextForAnalysis should not return null");
            Assert.That(cleanedHasGiftCard, Is.EqualTo(originalHasGiftCard), 
                "Gift Card Amount pattern should be preserved after cleaning");
            Assert.That(cleanedHasFreeShipping1, Is.EqualTo(originalHasFreeShipping1), 
                "Free Shipping -$0.46 pattern should be preserved after cleaning");
            Assert.That(cleanedHasFreeShipping2, Is.EqualTo(originalHasFreeShipping2), 
                "Free Shipping -$6.53 pattern should be preserved after cleaning");
                
            _logger.Information("✅ **TEST_1_COMPLETE**: CleanTextForAnalysis preserves critical financial patterns");
        }
        
        #endregion

        #region Test 2: Prompt Generation with Actual Amazon Data
        
        [Test]
        public void CreateHeaderErrorDetectionPrompt_WithAmazonData_ShouldContainFinancialPatterns()
        {
            _logger.Information("🔍 **TEST_2_START**: Testing prompt generation with actual Amazon invoice data");
            
            var invoice = CreateAmazonInvoiceFromActualData();
            
            // Act
            var prompt = InvokePrivateMethod<string>(_service, "CreateHeaderErrorDetectionPrompt", invoice, _actualAmazonInvoiceText);
            
            // Log prompt analysis
            _logger.Information("🔍 **PROMPT_LENGTH**: {Length} characters", prompt?.Length ?? 0);
            _logger.Information("🔍 **PROMPT_STRUCTURE_CHECK**: Contains 'FIND MISSING INVOICE FIELDS'? {Contains}", 
                prompt?.Contains("FIND MISSING INVOICE FIELDS") == true);
            
            // Check if financial patterns are in the prompt
            bool promptHasGiftCard = prompt?.Contains("Gift Card Amount: -$6.99") == true;
            bool promptHasFreeShipping = prompt?.Contains("Free Shipping: -$0.46") == true || prompt?.Contains("Free Shipping: -$6.53") == true;
            bool promptHasInvoiceData = prompt?.Contains("\"TotalDeduction\": null") == true;
            
            _logger.Information("🔍 **PROMPT_CONTENT_CHECK**: GiftCard={GiftCard}, FreeShipping={FreeShipping}, InvoiceData={InvoiceData}", 
                promptHasGiftCard, promptHasFreeShipping, promptHasInvoiceData);
            
            // Log the complete prompt for analysis
            _logger.Information("🔍 **COMPLETE_PROMPT**: {Prompt}", prompt);
            
            // Assertions
            Assert.That(prompt, Is.Not.Null.And.Not.Empty, "Prompt should be generated");
            Assert.That(prompt, Does.Contain("FIND MISSING INVOICE FIELDS"), "Should use simplified prompt structure");
            Assert.That(promptHasGiftCard || promptHasFreeShipping, Is.True, 
                "Prompt should contain either Gift Card or Free Shipping patterns from original text");
            Assert.That(promptHasInvoiceData, Is.True, 
                "Prompt should show TotalDeduction as null/missing in extracted data");
                
            _logger.Information("✅ **TEST_2_COMPLETE**: Prompt generation includes critical Amazon financial data");
        }
        
        #endregion

        #region Test 3: Amazon-Specific Regex Patterns
        
        [Test]
        public void DetectAmazonSpecificErrors_WithActualText_ShouldFindGiftCardAndFreeShipping()
        {
            _logger.Information("🔍 **TEST_3_START**: Testing Amazon-specific regex patterns with actual invoice text");
            
            var invoice = CreateAmazonInvoiceFromActualData();
            // Set TotalInsurance to null to trigger gift card detection
            invoice.TotalInsurance = null;
            // Set TotalDeduction to null to trigger free shipping detection  
            invoice.TotalDeduction = null;
            
            // Act - Call the private DetectAmazonSpecificErrors method
            var amazonErrors = InvokePrivateMethod<List<InvoiceError>>(_service, "DetectAmazonSpecificErrors", invoice, _actualAmazonInvoiceText);
            
            // Log results
            _logger.Information("🔍 **AMAZON_ERRORS_COUNT**: Detected {Count} Amazon-specific errors", amazonErrors?.Count ?? 0);
            
            if (amazonErrors != null)
            {
                foreach (var error in amazonErrors)
                {
                    _logger.Information("🔍 **AMAZON_ERROR_DETAIL**: Field={Field}, ErrorType={ErrorType}, CorrectValue={CorrectValue}, Confidence={Confidence}, Reasoning={Reasoning}", 
                        error.Field, error.ErrorType, error.CorrectValue, error.Confidence, error.Reasoning);
                }
            }
            
            // Test regex patterns directly on actual text
            _logger.Information("🔍 **DIRECT_REGEX_TEST**: Testing regex patterns on actual Amazon text");
            
            // Test Gift Card pattern
            var giftCardRegex = new Regex(@"Gift Card Amount:\s*(-?\$[\d,]+\.?\d*)", RegexOptions.IgnoreCase);
            var giftCardMatch = giftCardRegex.Match(_actualAmazonInvoiceText);
            _logger.Information("🔍 **GIFT_CARD_REGEX**: Success={Success}, Value={Value}", 
                giftCardMatch.Success, giftCardMatch.Success ? giftCardMatch.Groups[1].Value : "NONE");
            
            // Test Free Shipping pattern
            var freeShippingRegex = new Regex(@"Free Shipping:\s*(-?\$[\d,]+\.?\d*)", RegexOptions.IgnoreCase);
            var freeShippingMatches = freeShippingRegex.Matches(_actualAmazonInvoiceText);
            _logger.Information("🔍 **FREE_SHIPPING_REGEX**: MatchCount={Count}", freeShippingMatches.Count);
            
            for (int i = 0; i < freeShippingMatches.Count; i++)
            {
                _logger.Information("🔍 **FREE_SHIPPING_MATCH_{Index}**: Value={Value}", 
                    i + 1, freeShippingMatches[i].Groups[1].Value);
            }
            
            // Assertions
            Assert.That(amazonErrors, Is.Not.Null, "DetectAmazonSpecificErrors should not return null");
            
            var giftCardErrors = amazonErrors?.Where(e => e.Field == "TotalInsurance").ToList() ?? new List<InvoiceError>();
            var freeShippingErrors = amazonErrors?.Where(e => e.Field == "TotalDeduction").ToList() ?? new List<InvoiceError>();
            
            _logger.Information("🔍 **ERROR_BREAKDOWN**: GiftCardErrors={GiftCard}, FreeShippingErrors={FreeShipping}", 
                giftCardErrors.Count, freeShippingErrors.Count);
            
            Assert.That(giftCardMatch.Success, Is.True, "Gift Card regex should match actual Amazon text");
            Assert.That(freeShippingMatches.Count, Is.GreaterThan(0), "Free Shipping regex should find matches in actual Amazon text");
            
            // These should pass if the regex patterns are working correctly
            Assert.That(giftCardErrors.Count, Is.GreaterThan(0), 
                "Should detect Gift Card Amount as missing TotalInsurance field");
            Assert.That(freeShippingErrors.Count, Is.GreaterThan(0), 
                "Should detect Free Shipping amounts as missing TotalDeduction field");
                
            _logger.Information("✅ **TEST_3_COMPLETE**: Amazon-specific regex patterns working correctly");
        }
        
        #endregion

        #region Test 4: DeepSeek Response Analysis
        
        [Test]
        public async Task DetectHeaderFieldErrorsAsync_WithAmazonData_ShouldReturnGiftCardCorrections()
        {
            _logger.Information("🔍 **TEST_4_START**: Testing complete DeepSeek detection pipeline with actual Amazon data");
            
            var invoice = CreateAmazonInvoiceFromActualData();
            // Set TotalDeduction to null to trigger detection
            invoice.TotalDeduction = null;
            
            // Enable comprehensive logging for DeepSeek calls
            // using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // COMMENTED OUT: Preventing singleton conflicts
            // {
                _logger.Information("🔍 **DEEPSEEK_PIPELINE_TEST**: Starting complete DeepSeek detection test");
                
                // Act - Call the private DetectHeaderFieldErrorsAndOmissionsAsync method
                var headerErrors = await this.InvokePrivateMethodAsync<List<InvoiceError>>(this._service, 
                                       "DetectHeaderFieldErrorsAndOmissionsAsync", invoice, this._actualAmazonInvoiceText, null).ConfigureAwait(false);
                
                // Log DeepSeek results
                _logger.Information("🔍 **DEEPSEEK_HEADER_ERRORS**: Detected {Count} header errors", headerErrors?.Count ?? 0);
                
                if (headerErrors != null)
                {
                    foreach (var error in headerErrors)
                    {
                        _logger.Information("🔍 **DEEPSEEK_ERROR_DETAIL**: Field={Field}, ErrorType={ErrorType}, CorrectValue={CorrectValue}, Confidence={Confidence}, Reasoning={Reasoning}", 
                            error.Field, error.ErrorType, error.CorrectValue, error.Confidence, error.Reasoning);
                    }
                }
                
                // Check if DeepSeek found financial field corrections
                var totalDeductionErrors = headerErrors?.Where(e => e.Field == "TotalDeduction").ToList() ?? new List<InvoiceError>();
                var totalInsuranceErrors = headerErrors?.Where(e => e.Field == "TotalInsurance").ToList() ?? new List<InvoiceError>();
                var otherFinancialErrors = headerErrors?.Where(e => 
                    e.Field == "SubTotal" || e.Field == "InvoiceTotal" || 
                    e.Field == "TotalInternalFreight" || e.Field == "TotalOtherCost").ToList() ?? new List<InvoiceError>();
                
                _logger.Information("🔍 **DEEPSEEK_FINANCIAL_BREAKDOWN**: TotalDeduction={TD}, TotalInsurance={TI}, OtherFinancial={OF}", 
                    totalDeductionErrors.Count, totalInsuranceErrors.Count, otherFinancialErrors.Count);
                
                // Test the complete detection orchestration
                _logger.Information("🔍 **ORCHESTRATION_TEST**: Testing complete DetectInvoiceErrorsAsync");
                var allErrors = await this.InvokePrivateMethodAsync<List<InvoiceError>>(this._service, 
                                    "DetectInvoiceErrorsAsync", invoice, this._actualAmazonInvoiceText, null).ConfigureAwait(false);
                
                _logger.Information("🔍 **ALL_ERRORS_COUNT**: Total detected errors: {Count}", allErrors?.Count ?? 0);
                
                var amazonSpecificErrors = allErrors?.Where(e => 
                    e.Field == "TotalDeduction" || e.Field == "TotalInsurance").ToList() ?? new List<InvoiceError>();
                _logger.Information("🔍 **AMAZON_SPECIFIC_FROM_ORCHESTRATION**: {Count} Amazon-specific errors from full orchestration", 
                    amazonSpecificErrors.Count);
                
                // Assertions
                Assert.That(headerErrors, Is.Not.Null, "DetectHeaderFieldErrorsAndOmissionsAsync should not return null");
                Assert.That(allErrors, Is.Not.Null, "DetectInvoiceErrorsAsync should not return null");
                
                // Check if either DeepSeek or Amazon-specific detection found the missing fields
                bool foundFinancialCorrections = (totalDeductionErrors.Count > 0) || 
                                               (totalInsuranceErrors.Count > 0) || 
                                               (amazonSpecificErrors.Count > 0);
                
                Assert.That(foundFinancialCorrections, Is.True, 
                    "Either DeepSeek or Amazon-specific detection should find missing financial fields (TotalDeduction or TotalInsurance)");
                
                _logger.Information("✅ **TEST_4_COMPLETE**: DeepSeek detection pipeline analysis complete");
            // }
        }
        
        #endregion

        #region Test 5: DeepSeek Response Parsing
        
        [Test]
        public void ProcessDeepSeekCorrectionResponse_WithSampleResponse_ShouldParseCorrectly()
        {
            _logger.Information("🔍 **TEST_5_START**: Testing DeepSeek response parsing with sample Amazon correction");
            
            // Create a realistic DeepSeek response that should detect the missing TotalDeduction
            var sampleDeepSeekResponse = @"{
  ""errors"": [
    {
      ""field"": ""TotalDeduction"",
      ""extracted_value"": ""null"",
      ""correct_value"": ""6.99"",
      ""line_text"": ""Free Shipping: -$0.46\nFree Shipping: -$6.53"",
      ""line_number"": 69,
      ""confidence"": 0.95,
      ""error_type"": ""omission"",
      ""reasoning"": ""Free shipping amounts (-$0.46 + -$6.53 = -$6.99 total) found in text but TotalDeduction field is null. Caribbean customs requires mapping supplier reductions to TotalDeduction.""
    },
    {
      ""field"": ""SupplierName"",
      ""extracted_value"": ""null"",
      ""correct_value"": ""Amazon.com Services, Inc"",
      ""line_text"": ""Sold by: Amazon.com Services, Inc"",
      ""line_number"": 20,
      ""confidence"": 0.98,
      ""error_type"": ""omission"",
      ""reasoning"": ""Supplier name clearly shown in text but missing from extracted data""
    }
  ]
}";
            
            // Act - ProcessDeepSeekCorrectionResponse is a public method, call directly
            var correctionResults = _service.ProcessDeepSeekCorrectionResponse(sampleDeepSeekResponse, _actualAmazonInvoiceText);
            
            // Log parsing results
            _logger.Information("🔍 **PARSING_RESULTS**: Parsed {Count} corrections from sample response", correctionResults?.Count ?? 0);
            
            if (correctionResults != null)
            {
                foreach (var correction in correctionResults)
                {
                    _logger.Information("🔍 **PARSED_CORRECTION**: Field={Field}, OldValue={OldValue}, NewValue={NewValue}, Type={Type}, Confidence={Confidence}", 
                        correction.FieldName, correction.OldValue, correction.NewValue, correction.CorrectionType, correction.Confidence);
                }
            }
            
            // Test conversion to InvoiceError
            var invoiceErrors = correctionResults?.Select(cr => 
                InvokePrivateMethod<InvoiceError>(_service, "ConvertCorrectionResultToInvoiceError", cr)).ToList();
            
            _logger.Information("🔍 **CONVERSION_RESULTS**: Converted to {Count} InvoiceError objects", invoiceErrors?.Count ?? 0);
            
            // Assertions
            Assert.That(correctionResults, Is.Not.Null.And.Not.Empty, "Should parse sample DeepSeek response successfully");
            Assert.That(correctionResults.Count, Is.EqualTo(2), "Should parse exactly 2 corrections from sample response");
            
            var totalDeductionCorrection = correctionResults?.FirstOrDefault(c => c.FieldName == "TotalDeduction");
            Assert.That(totalDeductionCorrection, Is.Not.Null, "Should parse TotalDeduction correction");
            Assert.That(totalDeductionCorrection.NewValue, Is.EqualTo("6.99"), "Should extract correct TotalDeduction value");
            Assert.That(totalDeductionCorrection.CorrectionType, Is.EqualTo("omission"), "Should identify as omission");
            
            var supplierCorrection = correctionResults?.FirstOrDefault(c => c.FieldName == "SupplierName");
            Assert.That(supplierCorrection, Is.Not.Null, "Should parse SupplierName correction");
            Assert.That(supplierCorrection.NewValue, Is.EqualTo("Amazon.com Services, Inc"), "Should extract correct supplier name");
            
            _logger.Information("✅ **TEST_5_COMPLETE**: DeepSeek response parsing working correctly");
        }
        
        #endregion

        #region Test 6: Integration Test - Complete Pipeline
        
        [Test]
        public async Task CompleteDetectionPipeline_WithAmazonData_ShouldIdentifyMissingFields()
        {
            _logger.Information("🔍 **TEST_6_START**: Complete integration test of detection pipeline");
            
            var invoice = CreateAmazonInvoiceFromActualData();
            // Set TotalDeduction to null to simulate the actual test condition
            invoice.TotalDeduction = null;
            
            // using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // COMMENTED OUT: Preventing singleton conflicts
            // {
                _logger.Information("🔍 **INTEGRATION_TEST**: Running complete detection pipeline");
                
                // Step 1: Test Amazon-specific detection
                var amazonErrors = InvokePrivateMethod<List<InvoiceError>>(_service, 
                    "DetectAmazonSpecificErrors", invoice, _actualAmazonInvoiceText);
                
                // Step 2: Test DeepSeek header detection
                var deepSeekErrors = await this.InvokePrivateMethodAsync<List<InvoiceError>>(this._service, 
                                         "DetectHeaderFieldErrorsAndOmissionsAsync", invoice, this._actualAmazonInvoiceText, null).ConfigureAwait(false);
                
                // Step 3: Test complete orchestration
                var allErrors = await this.InvokePrivateMethodAsync<List<InvoiceError>>(this._service, 
                                    "DetectInvoiceErrorsAsync", invoice, this._actualAmazonInvoiceText, null).ConfigureAwait(false);
                
                // Analysis and logging
                _logger.Information("🔍 **PIPELINE_RESULTS**: Amazon={Amazon}, DeepSeek={DeepSeek}, Total={Total}", 
                    amazonErrors?.Count ?? 0, deepSeekErrors?.Count ?? 0, allErrors?.Count ?? 0);
                
                var allFinancialErrors = allErrors?.Where(e => 
                    e.Field == "TotalDeduction" || e.Field == "TotalInsurance" || 
                    e.Field == "SubTotal" || e.Field == "InvoiceTotal").ToList() ?? new List<InvoiceError>();
                
                _logger.Information("🔍 **FINANCIAL_ERRORS_FOUND**: {Count} financial field errors detected", allFinancialErrors.Count);
                
                foreach (var error in allFinancialErrors)
                {
                    _logger.Information("🔍 **FINANCIAL_ERROR**: Field={Field}, CorrectValue={Value}, Source={Source}", 
                        error.Field, error.CorrectValue, error.Reasoning?.Contains("Amazon") == true ? "Amazon-Specific" : "DeepSeek");
                }
                
                // The test goal: Identify why the Amazon test is failing
                bool hasGiftCardCorrection = allFinancialErrors.Any(e => 
                    e.Field == "TotalInsurance" && e.CorrectValue?.Contains("6.99") == true);
                bool hasFreeShippingCorrection = allFinancialErrors.Any(e => 
                    e.Field == "TotalDeduction" && e.CorrectValue?.Contains("6.99") == true);
                
                _logger.Information("🔍 **FINAL_ANALYSIS**: GiftCardCorrection={GiftCard}, FreeShippingCorrection={FreeShipping}", 
                    hasGiftCardCorrection, hasFreeShippingCorrection);
                
                // Determine the root cause
                if (!hasGiftCardCorrection && !hasFreeShippingCorrection)
                {
                    _logger.Error("❌ **ROOT_CAUSE_IDENTIFIED**: Neither Gift Card nor Free Shipping corrections found - this explains the test failure");
                    _logger.Error("🔍 **DIAGNOSTIC_SUMMARY**: Amazon detection found {Amazon} errors, DeepSeek found {DeepSeek} errors, but none targeted the critical financial fields", 
                        amazonErrors?.Count ?? 0, deepSeekErrors?.Count ?? 0);
                }
                else
                {
                    _logger.Information("✅ **DETECTION_SUCCESS**: Financial corrections were found - issue may be in later pipeline stages");
                }
                
                // Final assertion for diagnostic purposes
                Assert.That(allErrors, Is.Not.Null, "Detection pipeline should complete without errors");
                
                _logger.Information("✅ **TEST_6_COMPLETE**: Complete pipeline integration test finished");
            // }
        }
        
        #endregion

        #region Helper Methods
        
        private async Task<T> InvokePrivateMethodAsync<T>(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on {obj.GetType().Name}");
            }
            
            var result = method.Invoke(obj, parameters);
            
            if (result is Task<T> taskResult)
            {
                return await taskResult.ConfigureAwait(false);
            }
            
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
                return default(T);
            }
            
            return (T)result;
        }
        
        #endregion
    }
}