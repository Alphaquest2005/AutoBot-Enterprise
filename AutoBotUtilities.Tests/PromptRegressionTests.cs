using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Regression prevention tests to ensure critical prompt requirements remain in place.
    /// These tests prevent accidental removal of mandatory completion requirements.
    /// </summary>
    [TestFixture]
    public class PromptRegressionTests
    {
        private readonly string _promptFilePath = @"C:\Insight Software\AutoBot-Enterprise\InvoiceReader\OCRCorrectionService\OCRPromptCreation.cs";

        [Test]
        [Category("Regression")]
        [Category("Critical")]
        public void V14_MandatoryCompletionRequirements_MustExistInBothPromptMethods()
        {
            // Arrange
            Assert.That(File.Exists(_promptFilePath), Is.True, $"OCRPromptCreation.cs file not found at: {_promptFilePath}");
            string fileContent = File.ReadAllText(_promptFilePath);

            // Act & Assert - Check for V14.0 mandatory completion requirements (allow some flexibility)
            var v14RequirementsCount = CountOccurrences(fileContent, "V14.0 MANDATORY COMPLETION REQUIREMENTS");
            Assert.That(v14RequirementsCount, Is.GreaterThanOrEqualTo(2), 
                "REGRESSION DETECTED: V14.0 MANDATORY COMPLETION REQUIREMENTS must exist in BOTH CreateHeaderErrorDetectionPrompt AND CreateProductErrorDetectionPrompt methods");

            // Verify critical forbidden null value protections exist
            var forbiddenNullCount = CountOccurrences(fileContent, "ABSOLUTELY FORBIDDEN");
            Assert.That(forbiddenNullCount, Is.GreaterThanOrEqualTo(2), 
                "REGRESSION DETECTED: ABSOLUTELY FORBIDDEN null value protection sections are missing");

            // Verify specific null value protection exists
            Assert.That(fileContent.Contains("\"Reasoning\": null"), Is.True, "Missing protection against null Reasoning");
            Assert.That(fileContent.Contains("\"LineNumber\": 0"), Is.True, "Missing protection against LineNumber: 0");
            Assert.That(fileContent.Contains("\"LineText\": null"), Is.True, "Missing protection against null LineText");
            Assert.That(fileContent.Contains("\"SuggestedRegex\": null"), Is.True, "Missing protection against null SuggestedRegex");
        }

        [Test]
        [Category("Regression")]
        [Category("Critical")]
        public void NamedCaptureGroup_Requirements_MustExistInPrompts()
        {
            // Arrange
            string fileContent = File.ReadAllText(_promptFilePath);

            // Act & Assert - Check for named capture group requirements
            Assert.That(fileContent.Contains("named capture groups"), Is.True, 
                "REGRESSION DETECTED: Named capture group requirements are missing from prompts");
            
            Assert.That(fileContent.Contains("(?<FieldName>pattern)"), Is.True, 
                "REGRESSION DETECTED: Named capture group syntax examples are missing");

            Assert.That(fileContent.Contains("FORBIDDEN**: Never use numbered capture groups"), Is.True, 
                "REGRESSION DETECTED: Warning against numbered capture groups is missing");
        }

        [Test]
        [Category("Regression")]
        [Category("Critical")]
        public void MandatoryFieldRequirements_MustBeComplete()
        {
            // Arrange
            string fileContent = File.ReadAllText(_promptFilePath);

            // Act & Assert - Verify all 7 mandatory fields are specified
            string[] mandatoryFields = {
                "field**: The exact field name",
                "correct_value**: The actual value from the OCR text", 
                "error_type**: \"omission\" or \"format_correction\" or \"multi_field_omission\"",
                "line_number**: The actual line number where the value appears",
                "line_text**: The complete text of that line from the OCR",
                "suggested_regex**: A working regex pattern that captures the value",
                "reasoning**: Explain why this value was missed"
            };

            foreach (var requiredField in mandatoryFields)
            {
                Assert.That(fileContent.Contains(requiredField), Is.True, 
                    $"REGRESSION DETECTED: Mandatory field requirement missing: {requiredField}");
            }

            // Verify the "NEVER null" requirement exists for each field
            var neverNullCount = CountOccurrences(fileContent, "NEVER null");
            Assert.That(neverNullCount, Is.GreaterThanOrEqualTo(14), // 7 fields × 2 methods = 14 minimum
                "REGRESSION DETECTED: 'NEVER null' requirements are insufficient - should be at least 14 occurrences (7 fields × 2 methods)");
        }

        [Test]
        [Category("Regression")]
        [Category("Warning")]
        public void PromptVersioning_ShouldMaintainV14OrHigher()
        {
            // Arrange
            string fileContent = File.ReadAllText(_promptFilePath);

            // Act & Assert - Check that prompt versions haven't regressed below V14.0
            var v14Count = CountOccurrences(fileContent, "V14.0");
            var v13Count = CountOccurrences(fileContent, "V13.0");
            var v12Count = CountOccurrences(fileContent, "V12.0");
            var v11Count = CountOccurrences(fileContent, "V11.0");

            Assert.That(v14Count, Is.GreaterThan(0), 
                "WARNING: V14.0 prompts should be maintained - these contain critical null value prevention");
            
            if (v13Count > v14Count || v12Count > v14Count || v11Count > v14Count)
            {
                Assert.Warn("Detected older prompt versions - ensure V14.0 enhancements haven't been lost");
            }
        }

        [Test]
        [Category("Regression")]
        [Category("Critical")]
        public void ErrorLevelRequirement_MustExistForEnforcement()
        {
            // Arrange
            string fileContent = File.ReadAllText(_promptFilePath);

            // Act & Assert - Verify error level enforcement exists
            Assert.That(fileContent.Contains("ERROR LEVEL REQUIREMENT"), Is.True, 
                "REGRESSION DETECTED: ERROR LEVEL REQUIREMENT enforcement is missing");

            Assert.That(fileContent.Contains("If you cannot provide complete information for an error, DO NOT report that error"), Is.True, 
                "REGRESSION DETECTED: Instruction to not report incomplete errors is missing");
        }

        private int CountOccurrences(string text, string searchString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchString))
                return 0;

            int count = 0;
            int index = 0;

            while ((index = text.IndexOf(searchString, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index += searchString.Length;
            }

            return count;
        }
    }
}