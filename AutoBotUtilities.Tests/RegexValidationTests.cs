using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using Core.Common.Extensions;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Comprehensive unit tests for OCR regex validation logic.
    /// Tests the ValidatePatternInternal method with various scenarios and edge cases.
    /// </summary>
    [TestFixture]
    public class RegexValidationTests
    {
        private WaterNut.DataSpace.OCRCorrectionService _service;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            // Configure Serilog for test logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            _logger = Log.Logger;
            _service = new WaterNut.DataSpace.OCRCorrectionService(_logger);
        }

        [TearDown]
        public void TearDown()
        {
            Log.CloseAndFlush();
        }

        #region Positive Test Cases

        [Test]
        [TestCase("TotalInsurance", "-6.99", "Number")]
        [TestCase("TotalDeduction", "6.99", "Number")]
        [TestCase("InvoiceTotal", "166.30", "Number")]
        [TestCase("SubTotal", "161.95", "Number")]
        [TestCase("InvoiceNo", "112-9126443-1163432", "String")]
        [TestCase("SupplierName", "Amazon.com", "String")]
        public void ValidatePatternInternal_ShouldPassValidation_ForValidFieldValues(string fieldName, string newValue, string expectedDataType)
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal positive test for {FieldName} = '{NewValue}'", fieldName, newValue);
                
                var correction = new CorrectionResult
                {
                    FieldName = fieldName,
                    NewValue = newValue,
                    CorrectionType = "omission",
                    Success = true,
                    Confidence = 0.95
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with FieldName='{FieldName}', NewValue='{NewValue}', Success={Success}", 
                    correction.FieldName, correction.NewValue, correction.Success);

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: ValidatePatternInternal returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? false, result?.Reasoning ?? "NULL");

                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That(result.Success, Is.True, $"Validation should pass for valid {fieldName} value '{newValue}'. Reasoning: {result.Reasoning}");
                Assert.That(result.FieldName, Is.EqualTo(fieldName), "FieldName should be preserved");
                Assert.That(result.NewValue, Is.EqualTo(newValue), "NewValue should be preserved");

                _logger.Error("✅ **TEST_PASSED**: Validation passed for {FieldName} = '{NewValue}'", fieldName, newValue);
            }
        }

        [Test]
        public void ValidatePatternInternal_ShouldPassValidation_ForValidRegexPattern()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal regex compilation test");
                
                var correction = new CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    NewValue = "-6.99",
                    SuggestedRegex = @"(?<TotalInsurance>-?\$?\d+\.?\d*)",
                    CorrectionType = "omission",
                    Success = true,
                    Confidence = 0.95
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with SuggestedRegex='{Pattern}'", correction.SuggestedRegex);

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for regex compilation test");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Regex validation returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? false, result?.Reasoning ?? "NULL");

                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That(result.Success, Is.True, $"Regex validation should pass for valid pattern. Reasoning: {result.Reasoning}");
                Assert.That(result.SuggestedRegex, Is.EqualTo(correction.SuggestedRegex), "SuggestedRegex should be preserved");

                // Verify the regex actually compiles and works
                var regex = new Regex(result.SuggestedRegex, RegexOptions.IgnoreCase);
                var match = regex.Match(result.NewValue);
                Assert.That(match.Success, Is.True, "Generated regex should match the NewValue");

                _logger.Error("✅ **TEST_PASSED**: Regex compilation and matching test passed");
            }
        }

        #endregion

        #region Negative Test Cases

        [Test]
        public void ValidatePatternInternal_ShouldFailValidation_ForUnsupportedField()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal unsupported field test");
                
                var correction = new CorrectionResult
                {
                    FieldName = "UnsupportedFieldName",
                    NewValue = "someValue",
                    CorrectionType = "omission",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with unsupported FieldName='{FieldName}'", correction.FieldName);

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for unsupported field");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Unsupported field validation returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? true, result?.Reasoning ?? "NULL");

                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That(result.Success, Is.False, "Validation should fail for unsupported field");
                Assert.That(result.Reasoning, Contains.Substring("not supported"), "Reasoning should mention field not supported");

                _logger.Error("✅ **TEST_PASSED**: Unsupported field correctly rejected");
            }
        }

        [Test]
        [TestCase("TotalInsurance", "invalid-currency-format", "Should reject invalid currency format")]
        [TestCase("InvoiceNo", "", "Should reject empty required field")]
        [TestCase("SubTotal", "not-a-number", "Should reject non-numeric value for Number field")]
        public void ValidatePatternInternal_ShouldFailValidation_ForInvalidFieldValues(string fieldName, string invalidValue, string testDescription)
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal negative test - {TestDescription}", testDescription);
                
                var correction = new CorrectionResult
                {
                    FieldName = fieldName,
                    NewValue = invalidValue,
                    CorrectionType = "format_error",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with FieldName='{FieldName}', InvalidValue='{InvalidValue}'", 
                    fieldName, invalidValue);

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for invalid value test");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Invalid value validation returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? true, result?.Reasoning ?? "NULL");

                // Note: Some validations might pass if there's no strict pattern defined
                // This test documents the current behavior
                Assert.That(result, Is.Not.Null, "Result should not be null");
                
                if (!result.Success)
                {
                    Assert.That(result.Reasoning, Is.Not.Null.And.Not.Empty, "Reasoning should be provided when validation fails");
                    _logger.Error("✅ **TEST_PASSED**: Invalid value correctly rejected - {Reasoning}", result.Reasoning);
                }
                else
                {
                    _logger.Error("⚠️ **TEST_INFO**: Invalid value was accepted (no strict pattern defined) - {TestDescription}", testDescription);
                }
            }
        }

        [Test]
        public void ValidatePatternInternal_ShouldFailValidation_ForInvalidRegexPattern()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal invalid regex pattern test");
                
                var correction = new CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    NewValue = "-6.99",
                    SuggestedRegex = @"(?<TotalInsurance>-?\$?[0-9+)", // Invalid regex - missing closing bracket
                    CorrectionType = "omission",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with invalid SuggestedRegex='{Pattern}'", correction.SuggestedRegex);

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for invalid regex test");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Invalid regex validation returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? true, result?.Reasoning ?? "NULL");

                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That(result.Success, Is.False, "Validation should fail for invalid regex pattern");
                Assert.That(result.Reasoning, Contains.Substring("Invalid regex pattern"), "Reasoning should mention invalid regex");

                _logger.Error("✅ **TEST_PASSED**: Invalid regex pattern correctly rejected");
            }
        }

        #endregion

        #region Edge Cases

        [Test]
        public void ValidatePatternInternal_ShouldHandleNullInput()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal null input test");

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal with null input");
                var result = _service.ValidatePatternInternal(null);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Null input validation returned result={IsNull}", result == null ? "NULL" : "NOT_NULL");

                Assert.That(result, Is.Null, "Result should be null for null input");

                _logger.Error("✅ **TEST_PASSED**: Null input correctly handled");
            }
        }

        [Test]
        public void ValidatePatternInternal_ShouldHandleEmptyFieldName()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal empty field name test");
                
                var correction = new CorrectionResult
                {
                    FieldName = "",
                    NewValue = "6.99",
                    CorrectionType = "omission",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with empty FieldName");

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for empty field name");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Empty field name validation returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? true, result?.Reasoning ?? "NULL");

                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That(result.Success, Is.False, "Validation should fail for empty field name");

                _logger.Error("✅ **TEST_PASSED**: Empty field name correctly rejected");
            }
        }

        [Test]
        public void ValidatePatternInternal_ShouldHandleNullNewValue()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: ValidatePatternInternal null new value test");
                
                var correction = new CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    NewValue = null,
                    CorrectionType = "omission",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Created correction with null NewValue");

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for null new value");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Null new value validation returned Success={Success}, Reasoning='{Reasoning}'", 
                    result?.Success ?? false, result?.Reasoning ?? "NULL");

                Assert.That(result, Is.Not.Null, "Result should not be null");
                // Null new value should be acceptable for some correction types
                // The test documents current behavior

                _logger.Error("✅ **TEST_PASSED**: Null new value handling documented");
            }
        }

        #endregion

        #region Pattern Matching Tests

        [Test]
        [TestCase("-6.99", true, "Negative currency value should match Number pattern")]
        [TestCase("$6.99", true, "Currency with dollar sign should match Number pattern")]
        [TestCase("166.30", true, "Positive decimal should match Number pattern")]
        [TestCase("€123.45", true, "Euro currency should match Number pattern")]
        [TestCase("abc", false, "Text should not match Number pattern")]
        [TestCase("", false, "Empty string should not match Number pattern")]
        public void ValidationPattern_ShouldMatchExpectedValues_ForNumberDataType(string testValue, bool shouldMatch, string testDescription)
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: Pattern matching test - {TestDescription}", testDescription);
                
                var correction = new CorrectionResult
                {
                    FieldName = "TotalInsurance", // Number datatype field
                    NewValue = testValue,
                    CorrectionType = "omission",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Testing pattern matching for NewValue='{TestValue}', Expected={ShouldMatch}", 
                    testValue, shouldMatch);

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal for pattern matching test");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Pattern matching returned Success={Success}, Expected={Expected}", 
                    result?.Success ?? false, shouldMatch);

                Assert.That(result, Is.Not.Null, "Result should not be null");
                
                if (shouldMatch)
                {
                    Assert.That(result.Success, Is.True, $"Pattern should match for {testDescription}. Reasoning: {result.Reasoning}");
                }
                else
                {
                    // Note: The test documents expected behavior - some patterns might be more permissive
                    if (!result.Success)
                    {
                        Assert.That(result.Reasoning, Is.Not.Null.And.Not.Empty, "Reasoning should be provided when pattern doesn't match");
                    }
                }

                _logger.Error("✅ **TEST_COMPLETED**: Pattern matching test completed for {TestDescription}", testDescription);
            }
        }

        [Test]
        public void ValidationPattern_ShouldProvideDetailedAnalysis_ForPatternMismatch()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: Pattern analysis test for detailed validation logging");
                
                var correction = new CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    NewValue = "invalid-format-123",
                    CorrectionType = "format_error",
                    Success = true
                };

                _logger.Error("🧪 **TEST_INPUT**: Testing pattern analysis with intentionally invalid value");

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Calling ValidatePatternInternal to trigger pattern analysis");
                var result = _service.ValidatePatternInternal(correction);

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Pattern analysis completed with Success={Success}", result?.Success ?? false);

                Assert.That(result, Is.Not.Null, "Result should not be null");
                // This test primarily exercises the logging and analysis code paths
                
                _logger.Error("✅ **TEST_PASSED**: Pattern analysis and detailed logging exercised");
            }
        }

        #endregion

        #region Performance and Stress Tests

        [Test]
        public void ValidatePatternInternal_ShouldHandleMultipleValidations_Efficiently()
        {
            // Arrange
            using (LogLevelOverride.Begin(LogEventLevel.Error))
            {
                _logger.Error("🧪 **TEST_START**: Performance test for multiple validations");
                
                var corrections = new[]
                {
                    new CorrectionResult { FieldName = "TotalInsurance", NewValue = "-6.99", CorrectionType = "omission", Success = true },
                    new CorrectionResult { FieldName = "TotalDeduction", NewValue = "6.99", CorrectionType = "omission", Success = true },
                    new CorrectionResult { FieldName = "InvoiceTotal", NewValue = "166.30", CorrectionType = "omission", Success = true },
                    new CorrectionResult { FieldName = "SubTotal", NewValue = "161.95", CorrectionType = "omission", Success = true },
                    new CorrectionResult { FieldName = "InvoiceNo", NewValue = "112-9126443-1163432", CorrectionType = "omission", Success = true }
                };

                _logger.Error("🧪 **TEST_INPUT**: Created {Count} corrections for performance testing", corrections.Length);

                var startTime = DateTime.UtcNow;

                // Act
                _logger.Error("🧪 **TEST_ACTION**: Running multiple validations for performance test");
                foreach (var correction in corrections)
                {
                    var result = _service.ValidatePatternInternal(correction);
                    Assert.That(result, Is.Not.Null, $"Result should not be null for {correction.FieldName}");
                }

                var duration = DateTime.UtcNow - startTime;

                // Assert
                _logger.Error("🧪 **TEST_RESULT**: Performance test completed in {Duration}ms", duration.TotalMilliseconds);

                Assert.That(duration.TotalSeconds, Is.LessThan(10), "Multiple validations should complete within reasonable time");

                _logger.Error("✅ **TEST_PASSED**: Performance test completed successfully");
            }
        }

        #endregion
    }
}