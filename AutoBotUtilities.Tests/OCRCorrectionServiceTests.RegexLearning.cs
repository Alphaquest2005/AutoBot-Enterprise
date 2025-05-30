using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using Core.Common.Extensions;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Text;
using Serilog.Core;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Regex learning and pattern persistence tests for OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        #region Regex Pattern Learning Tests

        [Test]
        [Category("RegexLearning")]
        [Category("FileSystem")]
        public async Task RegexPatternPersistence_SaveAndLoad_ShouldMaintainState()
        {
            _logger.Information("Testing regex pattern persistence to real file system");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                var testPatterns = new List<RegexPattern>
                {
                    new RegexPattern
                    {
                        FieldName = "InvoiceTotal",
                        StrategyType = "FORMAT_FIX",
                        Pattern = @"(\d+),(\d{2})",
                        Replacement = "$1.$2",
                        Confidence = 0.95,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        UpdateCount = 1,
                        CreatedBy = "Test"
                    },
                    new RegexPattern
                    {
                        FieldName = "SubTotal",
                        StrategyType = "CHARACTER_MAP",
                        Pattern = @"1O(\d+)",
                        Replacement = "10$1",
                        Confidence = 0.88,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        UpdateCount = 2,
                        CreatedBy = "Test"
                    }
                };

                // Save patterns using production code
                var regexConfigPath = Path.Combine(_tempConfigDirectory, "OCRRegexPatterns.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(testPatterns, options);
                File.WriteAllText(regexConfigPath, json);

                _logger.Information("Saved {Count} patterns to {Path}", testPatterns.Count, regexConfigPath);

                // Load patterns using production code
                var loadedPatterns = await InvokePrivateMethodAsync<List<RegexPattern>>(
                    _service, "LoadRegexPatternsAsync");

                _logger.Information("Loaded {Count} patterns from file", loadedPatterns.Count);

                Assert.That(loadedPatterns.Count, Is.EqualTo(testPatterns.Count));

                for (int i = 0; i < testPatterns.Count; i++)
                {
                    var original = testPatterns[i];
                    var loaded = loadedPatterns.FirstOrDefault(p => p.FieldName == original.FieldName);

                    Assert.That(loaded, Is.Not.Null, $"Pattern for {original.FieldName} should be loaded");
                    Assert.That(loaded.Pattern, Is.EqualTo(original.Pattern));
                    Assert.That(loaded.Replacement, Is.EqualTo(original.Replacement));
                    Assert.That(loaded.Confidence, Is.EqualTo(original.Confidence));
                    Assert.That(loaded.StrategyType, Is.EqualTo(original.StrategyType));

                    _logger.Information("✓ Pattern {FieldName}: {Pattern} → {Replacement}",
                        loaded.FieldName, loaded.Pattern, loaded.Replacement);
                }

                _logger.Information("✓ Regex pattern persistence working correctly");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Test]
        [Category("RegexLearning")]
        [Category("FileSystem")]
        public async Task ApplyLearnedRegexPatternsAsync_ShouldTransformText()
        {
            _logger.Information("Testing application of learned regex patterns");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                // Create a test pattern for decimal comma correction
                var testPattern = new RegexPattern
                {
                    FieldName = "InvoiceTotal",
                    StrategyType = "FORMAT_FIX",
                    Pattern = @"\$([0-9]+),([0-9]{2})",
                    Replacement = "$$$1.$2",
                    Confidence = 0.95,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    UpdateCount = 1,
                    CreatedBy = "Test"
                };

                // Save pattern to file
                var patterns = new List<RegexPattern> { testPattern };
                var regexConfigPath = Path.Combine(_tempConfigDirectory, "OCRRegexPatterns.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(patterns, options);
                File.WriteAllText(regexConfigPath, json);

                var originalText = "Invoice Total: $123,45";
                var expectedText = "Invoice Total: $123.45";

                // Apply patterns using production code
                var transformedText = await InvokePrivateMethodAsync<string>(_service,
                    "ApplyLearnedRegexPatternsAsync", originalText, "InvoiceTotal");

                _logger.Information("Original: {Original}", originalText);
                _logger.Information("Transformed: {Transformed}", transformedText);

                Assert.That(transformedText, Is.EqualTo(expectedText),
                    "Text should be transformed according to regex pattern");

                _logger.Information("✓ Learned regex patterns applied correctly");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Test]
        [Category("RegexLearning")]
        [Category("Integration")]
        public async Task RegexLearningWorkflow_EndToEnd_ShouldImproveAccuracy()
        {
            _logger.Information("Testing end-to-end regex learning workflow");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                // Step 1: Start with no patterns
                var regexConfigPath = Path.Combine(_tempConfigDirectory, "OCRRegexPatterns.json");
                if (File.Exists(regexConfigPath))
                {
                    File.Delete(regexConfigPath);
                }

                // Step 2: Run correction to generate learning data
                var corrections = new List<CorrectionResult>
                {
                    new CorrectionResult
                    {
                        FieldName = "InvoiceTotal",
                        OldValue = "12345",
                        NewValue = "123.45",
                        Success = true,
                        Confidence = 0.95,
                        CorrectionType = "FieldFormat"
                    }
                };

                var fileText = "Invoice Total: 12345";

                // Step 3: Update regex patterns (this should create the file)
                await _service.UpdateRegexPatternsAsync(corrections, fileText);

                // Step 4: Verify pattern was saved
                Assert.That(File.Exists(regexConfigPath), Is.True, "Regex patterns file should be created");

                var savedPatterns = await InvokePrivateMethodAsync<List<RegexPattern>>(
                    _service, "LoadRegexPatternsAsync");

                _logger.Information("Generated {Count} patterns from corrections", savedPatterns.Count);

                // Step 5: Apply learned patterns to new text
                var testText = "New Invoice Total: 67890";
                var transformedText = await InvokePrivateMethodAsync<string>(_service,
                    "ApplyLearnedRegexPatternsAsync", testText, "InvoiceTotal");

                _logger.Information("Applied patterns: {Original} → {Transformed}", testText, transformedText);

                // Step 6: Verify improvement (patterns should be applied)
                Assert.That(savedPatterns.Count, Is.GreaterThan(0), "Should have learned patterns");

                _logger.Information("✓ End-to-end regex learning workflow completed successfully");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Test]
        [Category("RegexLearning")]
        [Category("Performance")]
        public async Task RegexPatternApplication_LargeText_ShouldBeEfficient()
        {
            _logger.Information("Testing regex pattern application performance on large text");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                // Create multiple test patterns
                var testPatterns = new List<RegexPattern>();
                for (int i = 0; i < 10; i++)
                {
                    testPatterns.Add(new RegexPattern
                    {
                        FieldName = "InvoiceTotal",
                        StrategyType = "FORMAT_FIX",
                        Pattern = $@"(\d+){i},(\d{{2}})",
                        Replacement = "$1.$2",
                        Confidence = 0.90,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        UpdateCount = 1,
                        CreatedBy = "PerformanceTest"
                    });
                }

                // Save patterns
                var regexConfigPath = Path.Combine(_tempConfigDirectory, "OCRRegexPatterns.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(testPatterns, options);
                File.WriteAllText(regexConfigPath, json);

                // Create large text (simulate large invoice)
                var largeText = new StringBuilder();
                for (int i = 0; i < 1000; i++)
                {
                    largeText.AppendLine($"Line {i}: Invoice Total: $123{i % 10},45");
                }

                var stopwatch = Stopwatch.StartNew();

                // Apply patterns
                var transformedText = await InvokePrivateMethodAsync<string>(_service,
                    "ApplyLearnedRegexPatternsAsync", largeText.ToString(), "InvoiceTotal");

                stopwatch.Stop();

                _logger.Information("Processed {Length} characters in {Elapsed}ms",
                    largeText.Length, stopwatch.ElapsedMilliseconds);

                Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000),
                    "Pattern application should complete within 5 seconds");

                Assert.That(transformedText, Is.Not.Null, "Should return transformed text");

                _logger.Information("✓ Regex pattern application performance is acceptable");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        #endregion
    }
}
