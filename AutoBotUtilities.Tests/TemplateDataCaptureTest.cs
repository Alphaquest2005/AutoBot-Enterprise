using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using Core.Common.Extensions;
using Serilog.Events;
using WaterNut.DataSpace;
using OCR.Business.Entities;
using EntryDataDS.Business.Entities;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class TemplateDataCaptureTest
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = Log.ForContext<TemplateDataCaptureTest>();
        }

        /// <summary>
        /// Captures realistic Amazon template data for use in Pathway 1 testing
        /// This test extracts actual Amazon invoice processing results to create test data
        /// </summary>
        [Test]
        public async Task CaptureAmazonTemplateDataForPathway1Testing()
        {
            _logger.Information("🔍 **TEMPLATE_DATA_CAPTURE_START**: Capturing Amazon template data for Pathway 1 testing");

            try
            {
                using (LogLevelOverride.Begin(LogEventLevel.Verbose))
                {
                    // Load Amazon template (ID 5) from database with comprehensive logging
                    using (var ocrContext = new OCRContext())
                    {
                        var amazonTemplate = ocrContext.Templates.FirstOrDefault(x => x.Id == 5);
                        Assert.That(amazonTemplate, Is.Not.Null, "Amazon template (ID 5) should exist in database");

                        var template = new Template(amazonTemplate, _logger);
                        _logger.Information("✅ **TEMPLATE_LOADED**: Amazon template loaded with {PartCount} parts and {LineCount} lines", 
                            template.Parts?.Count ?? 0, template.Lines?.Count ?? 0);

                        // Create Amazon invoice text that WILL match existing patterns
                        var amazonInvoiceText = GetAmazonInvoiceTextWithKnownPatterns();
                        var textLines = amazonInvoiceText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        
                        _logger.Information("📋 **INPUT_TEXT_PREPARED**: Amazon invoice text with {LineCount} lines", textLines.Count);

                        // Log initial template regex patterns for debugging
                        LogTemplateRegexPatterns(template, "INITIAL", _logger);

                        // Perform initial template read to see what values are extracted
                        _logger.Information("🔍 **INITIAL_READ_START**: Performing initial template.Read() to capture extracted values");
                        var initialResults = template.Read(textLines);
                        _logger.Information("✅ **INITIAL_READ_COMPLETE**: Initial read returned {ResultCount} results", initialResults?.Count ?? 0);

                        // Log the template Lines.Values state after initial read
                        LogComprehensiveLineValuesState(template, "AFTER_INITIAL_READ", _logger);

                        // Log the actual results structure
                        if (initialResults != null && initialResults.Any())
                        {
                            for (int i = 0; i < initialResults.Count; i++)
                            {
                                var result = initialResults[i];
                                if (result is IDictionary<string, object> dict)
                                {
                                    _logger.Information("📋 **RESULT_STRUCTURE_{Index}**: {DictKeys}", i, string.Join(", ", dict.Keys));
                                    foreach (var kvp in dict)
                                    {
                                        _logger.Information("  🔍 **RESULT_FIELD**: {Key} = '{Value}' ({Type})", 
                                            kvp.Key, kvp.Value, kvp.Value?.GetType().Name ?? "NULL");
                                    }
                                }
                            }
                        }

                        // Test clear and re-read functionality
                        _logger.Information("🔍 **CLEAR_TEST_START**: Testing ClearInvoiceForReimport() functionality");
                        template.ClearInvoiceForReimport();
                        LogComprehensiveLineValuesState(template, "AFTER_CLEAR", _logger);

                        _logger.Information("🔍 **REREAD_TEST_START**: Testing template.Read() after clear");
                        var rereadResults = template.Read(textLines);
                        _logger.Information("✅ **REREAD_COMPLETE**: Re-read returned {ResultCount} results", rereadResults?.Count ?? 0);
                        LogComprehensiveLineValuesState(template, "AFTER_REREAD", _logger);

                        // Generate test data summary for Pathway 1
                        GeneratePathway1TestDataSummary(template, textLines, initialResults, rereadResults, _logger);

                        _logger.Information("✅ **TEMPLATE_DATA_CAPTURE_SUCCESS**: Template data capture completed successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEMPLATE_DATA_CAPTURE_FAILED**: Template data capture failed");
                throw;
            }
        }

        private string GetAmazonInvoiceTextWithKnownPatterns()
        {
            // Create invoice text that matches existing Amazon template patterns
            return @"amazoncom

Print this page for your records.

Sold by: Amazon.com Services, Inc
Order Placed: April 15, 2025
Amazon.com order number: 112-9126443-1163432
Order Total: $166.30

Item(s) Subtotal: $161.95
Shipping & Handling: $6.99
Free Shipping: -$0.46
Free Shipping: -$6.53
Estimated tax to be collected: $11.34
Gift Card Amount: -$6.99
Grand Total: $166.30

Billing address
Ship to
Order #: 112-9126443-1163432
Invoice Number: 112-9126443-1163432
Total: $166.30
Amazon.com Services, Inc
SupplierCode: Amazon.com";
        }

        private void LogTemplateRegexPatterns(Template template, string stage, ILogger log)
        {
            log.Information("📋 **TEMPLATE_REGEX_PATTERNS_{Stage}**: Analyzing template patterns", stage);
            
            if (template?.Parts == null)
            {
                log.Warning("⚠️ **NO_TEMPLATE_PARTS**: Template has no parts");
                return;
            }

            foreach (var part in template.Parts)
            {
                var partId = part.OCR_Part?.Id ?? 0;
                var partName = part.OCR_Part?.PartTypes?.Name ?? "Unknown";
                log.Information("📂 **PART_{PartId}**: {PartName}", partId, partName);

                if (part.Lines?.Any() == true)
                {
                    foreach (var line in part.Lines)
                    {
                        var lineId = line.OCR_Lines?.Id ?? 0;
                        var lineName = line.OCR_Lines?.Name ?? "Unknown";
                        var regex = line.OCR_Lines?.RegularExpressions?.RegEx ?? "NO_REGEX";
                        
                        log.Information("  📝 **LINE_{LineId}**: {LineName} | Regex: {Regex}", 
                            lineId, lineName, regex.Length > 100 ? regex.Substring(0, 100) + "..." : regex);

                        // Log fields for this line
                        if (line.OCR_Lines?.Fields?.Any() == true)
                        {
                            foreach (var field in line.OCR_Lines.Fields)
                            {
                                log.Information("    🔍 **FIELD_{FieldId}**: Key='{Key}' | Field='{Field}' | EntityType='{EntityType}'",
                                    field.Id, field.Key, field.Field, field.EntityType);
                            }
                        }
                    }
                }
            }
        }

        private void LogComprehensiveLineValuesState(Template template, string stage, ILogger log)
        {
            log.Information("📊 **LINES_VALUES_STATE_{Stage}**: Comprehensive Lines.Values analysis", stage);

            if (template?.Lines == null)
            {
                log.Information("⚠️ **NO_TEMPLATE_LINES**: Template has no lines");
                return;
            }

            int totalLines = template.Lines.Count;
            int linesWithValues = 0;
            int totalValueEntries = 0;

            foreach (var line in template.Lines)
            {
                var lineId = line.OCR_Lines?.Id ?? 0;
                var lineName = line.OCR_Lines?.Name ?? "Unknown";
                var valueCount = line.Values?.Sum(kvp => kvp.Value?.Count ?? 0) ?? 0;

                if (valueCount > 0)
                {
                    linesWithValues++;
                    totalValueEntries += valueCount;
                    
                    log.Information("📋 **LINE_{LineId}_VALUES**: {LineName} has {ValueCount} values", lineId, lineName, valueCount);

                    // Log detailed value structure
                    if (line.Values?.Any() == true)
                    {
                        foreach (var valueKvp in line.Values)
                        {
                            var (lineNumber, section) = valueKvp.Key;
                            var fieldsDict = valueKvp.Value;
                            
                            log.Information("    🔍 **VALUE_ENTRY**: LineNumber={LineNumber}, Section='{Section}', FieldCount={FieldCount}",
                                lineNumber, section, fieldsDict?.Count ?? 0);

                            if (fieldsDict?.Any() == true)
                            {
                                foreach (var fieldKvp in fieldsDict)
                                {
                                    var (fields, instance) = fieldKvp.Key;
                                    var value = fieldKvp.Value;
                                    
                                    log.Information("      📝 **FIELD_VALUE**: FieldId={FieldId}, Field='{Field}', Key='{Key}', Value='{Value}', Instance='{Instance}'",
                                        fields?.Id ?? 0, fields?.Field ?? "NULL", fields?.Key ?? "NULL", value, instance);
                                }
                            }
                        }
                    }
                }
                else
                {
                    log.Information("📋 **LINE_{LineId}_NO_VALUES**: {LineName} has no values", lineId, lineName);
                }
            }

            log.Information("📊 **SUMMARY_{Stage}**: {TotalLines} total lines, {LinesWithValues} with values, {TotalValueEntries} total value entries",
                stage, totalLines, linesWithValues, totalValueEntries);
        }

        private void GeneratePathway1TestDataSummary(Template template, List<string> textLines, List<dynamic> initialResults, List<dynamic> rereadResults, ILogger log)
        {
            log.Information("📋 **PATHWAY_1_TEST_DATA_SUMMARY**: Generating test data summary for Pathway 1");

            log.Information("🔍 **TEXT_LINES_COUNT**: {Count} text lines prepared", textLines.Count);
            log.Information("🔍 **INITIAL_RESULTS_COUNT**: {Count} initial results", initialResults?.Count ?? 0);
            log.Information("🔍 **REREAD_RESULTS_COUNT**: {Count} reread results", rereadResults?.Count ?? 0);

            // Check if template has any regex patterns that would actually match
            var linesWithRegex = template.Lines?.Where(l => !string.IsNullOrEmpty(l.OCR_Lines?.RegularExpressions?.RegEx)).ToList() ?? new List<Line>();
            log.Information("🔍 **LINES_WITH_REGEX**: {Count} lines have regex patterns", linesWithRegex.Count);

            // Check for Lines.Values population
            var linesWithValuesInitial = template.Lines?.Where(l => l.Values?.Any() == true).ToList() ?? new List<Line>();
            log.Information("🔍 **LINES_WITH_VALUES_AFTER_READ**: {Count} lines have values", linesWithValuesInitial.Count);

            if (linesWithValuesInitial.Count == 0)
            {
                log.Warning("⚠️ **PATHWAY_1_ISSUE**: No Lines.Values populated - template regex patterns may not match the provided text");
                log.Information("💡 **SUGGESTION**: Use actual Amazon PDF text or modify test text to match existing regex patterns");
                
                // Log sample regex patterns for reference
                if (linesWithRegex.Any())
                {
                    log.Information("📋 **SAMPLE_REGEX_PATTERNS**: First few regex patterns from template:");
                    foreach (var line in linesWithRegex.Take(5))
                    {
                        var regex = line.OCR_Lines?.RegularExpressions?.RegEx ?? "";
                        log.Information("  🔍 **REGEX_{LineId}**: {Regex}", line.OCR_Lines?.Id, 
                            regex.Length > 200 ? regex.Substring(0, 200) + "..." : regex);
                    }
                }
            }
        }
    }
}