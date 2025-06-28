using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using Core.Common.Extensions;
using AutoBotUtilities.Tests.Models;
using Newtonsoft.Json;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Simplified Testing Process: Run all tests → Pick one issue → Fix prompts → Retest that issue
    /// This is much more practical than complex regression frameworks
    /// </summary>
    [TestFixture]
    public class SimplifiedTestingProcess
    {
        private static ILogger _logger;
        private const string ExtractedTextPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Extracted Text";
        private const string ReferenceDataPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Reference Data";
        private const string IssueTrackingPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Issue Tracking";

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.NUnitOutput()
                .CreateLogger();

            _logger = Log.ForContext<SimplifiedTestingProcess>();
            Directory.CreateDirectory(IssueTrackingPath);
            
            _logger.Information("🚀 **SIMPLIFIED_TESTING_PROCESS**: Starting systematic issue tracking and resolution");
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Step 1: Run through ALL tests and catalog ALL issues
        /// </summary>
        [Test, Order(1)]
        public async Task Step1_RunAllTestsAndCatalogIssues()
        {
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("📋 **STEP_1_START**: Running all tests to catalog issues");
                
                var allTextFiles = Directory.GetFiles(ExtractedTextPath, "*.txt")
                    .Where(f => !Path.GetFileName(f).StartsWith("extraction_inventory"))
                    .Take(1) // Start with just 1 file for fastest testing
                    .ToArray();
                
                _logger.Information("🔍 **PROCESSING_FILES**: Testing {FileCount} files to identify all issues", allTextFiles.Length);
                
                var allIssues = new List<DetectedIssue>();
                var service = new OCRCorrectionService(_logger);
                
                foreach (var textFile in allTextFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(textFile);
                    _logger.Information("🔧 **TESTING_FILE**: {FileName}", fileName);
                    
                    try
                    {
                        // Load OCR text
                        var ocrText = File.ReadAllText(textFile);
                        
                        // Generate JSON reference
                        var jsonReference = await GenerateJsonReference(ocrText, fileName);
                        
                        // Run DeepSeek detection
                        var blankInvoice = CreateBlankInvoice(fileName);
                        var deepSeekResults = await RunDeepSeekDetection(service, blankInvoice, ocrText);
                        
                        // Compare and identify issues
                        var fileIssues = IdentifyIssues(fileName, jsonReference, deepSeekResults, ocrText);
                        allIssues.AddRange(fileIssues);
                        
                        _logger.Information("📊 **FILE_COMPLETE**: {FileName} - Found {IssueCount} issues", fileName, fileIssues.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "❌ **FILE_ERROR**: {FileName} - Exception during testing", fileName);
                        
                        allIssues.Add(new DetectedIssue
                        {
                            FileName = fileName,
                            IssueType = "PROCESSING_ERROR",
                            Description = $"Exception during testing: {ex.Message}",
                            Severity = "HIGH",
                            AffectedFields = new List<string>()
                        });
                    }
                }
                
                // Catalog and prioritize all issues
                var issueCatalog = CatalogAndPrioritizeIssues(allIssues);
                SaveIssueCatalog(issueCatalog);
                
                // Generate comprehensive issue report
                GenerateIssueReport(issueCatalog);
                
                _logger.Information("✅ **STEP_1_COMPLETE**: Tested {FileCount} files, found {IssueCount} total issues across {CategoryCount} categories", 
                    allTextFiles.Length, allIssues.Count, issueCatalog.IssueCategories.Count);
                
                Assert.That(allIssues.Count, Is.GreaterThan(0), "Should find some issues to work on");
            }
        }

        /// <summary>
        /// Step 2: Pick the highest priority issue and improve prompts
        /// </summary>
        [Test, Order(2)]
        public void Step2_PickIssueAndImprovePrompts()
        {
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("🎯 **STEP_2_START**: Selecting highest priority issue for prompt improvement");
                
                var issueCatalog = LoadIssueCatalog();
                
                // Get highest priority issue
                var highestPriorityIssue = issueCatalog.IssueCategories
                    .OrderByDescending(c => c.Priority)
                    .ThenByDescending(c => c.AffectedFiles.Count)
                    .First();
                
                _logger.Information("🔧 **SELECTED_ISSUE**: {IssueType} affecting {FileCount} files with priority {Priority}", 
                    highestPriorityIssue.IssueType, highestPriorityIssue.AffectedFiles.Count, highestPriorityIssue.Priority);
                
                // Create prompt improvements for this specific issue
                var promptImprovements = CreatePromptImprovements(highestPriorityIssue);
                
                // Apply improvements to both prompts
                ApplyPromptImprovements(promptImprovements);
                
                // Save improvement details
                SavePromptImprovements(highestPriorityIssue.IssueType, promptImprovements);
                
                _logger.Information("✅ **STEP_2_COMPLETE**: Improved prompts to address {IssueType} issue", highestPriorityIssue.IssueType);
                
                Assert.That(promptImprovements, Is.Not.Null, "Should create prompt improvements");
                Assert.That(promptImprovements.DeepSeekImprovements.Count, Is.GreaterThan(0), "Should have DeepSeek improvements");
            }
        }

        /// <summary>
        /// Step 3: Retest ONLY the files that had the selected issue
        /// </summary>
        [Test, Order(3)]
        public async Task Step3_RetestAffectedFiles()
        {
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("🔄 **STEP_3_START**: Retesting files affected by the improved issue");
                
                var issueCatalog = LoadIssueCatalog();
                var targetIssue = issueCatalog.IssueCategories
                    .OrderByDescending(c => c.Priority)
                    .ThenByDescending(c => c.AffectedFiles.Count)
                    .First();
                
                _logger.Information("🎯 **RETESTING_ISSUE**: {IssueType} - Testing {FileCount} affected files", 
                    targetIssue.IssueType, targetIssue.AffectedFiles.Count);
                
                var retestResults = new List<RetestResult>();
                var service = new OCRCorrectionService(_logger);
                
                foreach (var fileName in targetIssue.AffectedFiles)
                {
                    var textFile = Path.Combine(ExtractedTextPath, $"{fileName}.txt");
                    if (!File.Exists(textFile))
                    {
                        _logger.Warning("⚠️ **FILE_NOT_FOUND**: {FileName}", fileName);
                        continue;
                    }
                    
                    _logger.Information("🔧 **RETESTING_FILE**: {FileName}", fileName);
                    
                    try
                    {
                        // Load OCR text
                        var ocrText = File.ReadAllText(textFile);
                        
                        // Generate updated JSON reference (with improved prompt)
                        var newJsonReference = await GenerateJsonReference(ocrText, fileName);
                        
                        // Run DeepSeek detection (with improved prompt)
                        var blankInvoice = CreateBlankInvoice(fileName);
                        var newDeepSeekResults = await RunDeepSeekDetection(service, blankInvoice, ocrText);
                        
                        // Check if the specific issue is resolved
                        var isIssueResolved = CheckIfIssueResolved(fileName, targetIssue.IssueType, newJsonReference, newDeepSeekResults);
                        
                        var retestResult = new RetestResult
                        {
                            FileName = fileName,
                            IssueType = targetIssue.IssueType,
                            WasResolved = isIssueResolved,
                            NewJsonBalance = newJsonReference?.Validation?.BalanceCheck ?? 0,
                            NewDeepSeekBalance = CalculateDeepSeekBalance(newDeepSeekResults),
                            ImprovementDetails = isIssueResolved ? "Issue resolved successfully" : "Issue still present"
                        };
                        
                        retestResults.Add(retestResult);
                        
                        _logger.Information("📊 **RETEST_RESULT**: {FileName} - Issue {Status}", 
                            fileName, isIssueResolved ? "RESOLVED" : "STILL_PRESENT");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "❌ **RETEST_ERROR**: {FileName} - Exception during retest", fileName);
                        
                        retestResults.Add(new RetestResult
                        {
                            FileName = fileName,
                            IssueType = targetIssue.IssueType,
                            WasResolved = false,
                            ImprovementDetails = $"Retest failed: {ex.Message}"
                        });
                    }
                }
                
                // Analyze retest results
                var resolvedCount = retestResults.Count(r => r.WasResolved);
                var totalCount = retestResults.Count;
                var resolutionRate = totalCount > 0 ? (double)resolvedCount / totalCount : 0;
                
                // Save retest results
                SaveRetestResults(targetIssue.IssueType, retestResults);
                
                _logger.Information("✅ **STEP_3_COMPLETE**: {ResolvedCount}/{TotalCount} files resolved ({ResolutionRate:P0}) for issue {IssueType}", 
                    resolvedCount, totalCount, resolutionRate, targetIssue.IssueType);
                
                Assert.That(resolutionRate, Is.GreaterThan(0.5), $"At least 50% of {targetIssue.IssueType} issues should be resolved");
            }
        }

        #region Helper Methods

        private async Task<InvoiceReferenceData> GenerateJsonReference(string ocrText, string fileName)
        {
            _logger.Information("🔍 **JSON_PROMPT_START**: Generating JSON reference data for {FileName}", fileName);
            
            try 
            {
                // Create JSON extraction prompt
                var jsonPrompt = CreateJsonExtractionPrompt(ocrText, fileName);
                
                // Call DeepSeek API with JSON extraction prompt
                var apiClient = new WaterNut.Business.Services.Utils.DeepSeekInvoiceApi(_logger);
                var jsonResponse = await apiClient.GetResponseAsync(jsonPrompt);
                
                _logger.Information("📊 **JSON_API_RESPONSE**: Received JSON extraction response for {FileName}", fileName);
                
                // Parse JSON response into structured data
                var referenceData = ParseJsonResponse(jsonResponse, ocrText, fileName);
                
                _logger.Information("✅ **JSON_PROMPT_COMPLETE**: Generated reference data for {FileName}", fileName);
                return referenceData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **JSON_PROMPT_ERROR**: Failed to generate JSON reference for {FileName}", fileName);
                
                // Return placeholder data on error
                return new InvoiceReferenceData
                {
                    Header = new InvoiceHeader(),
                    Financial = new FinancialFields(),
                    Validation = new CalculatedValidation { BalanceCheck = 999.99, ValidationPassed = false },
                    Metadata = new ExtractionMetadata
                    {
                        SourceText = ocrText.Substring(0, Math.Min(100, ocrText.Length)) + "...",
                        ExtractedFieldCount = 0,
                        TotalFieldCount = 11,
                        ConfidenceLevel = "error",
                        ProcessingNotes = new List<string> { $"JSON prompt failed: {ex.Message}" }
                    }
                };
            }
        }
        
        private string CreateJsonExtractionPrompt(string ocrText, string fileName)
        {
            var prompt = $@"You are an expert invoice data extraction system that follows Caribbean Customs rules and production OCR section precedence logic. Extract all relevant invoice information from the provided OCR text and output a structured JSON object that matches the production ShipmentInvoice model.

**OCR TEXT TO ANALYZE:**
{ocrText}

**CARIBBEAN CUSTOMS FIELD MAPPING RULES:**

1. **TotalInsurance** = Customer-caused reductions (stored as NEGATIVE values):
   - Gift Cards → TotalInsurance = -amount  
   - Store Credits → TotalInsurance = -amount
   - Credits → TotalInsurance = -amount
   - Customer payments/refunds → TotalInsurance = -amount

2. **TotalDeduction** = Supplier-caused reductions (stored as POSITIVE values):
   - Free Shipping → TotalDeduction = +amount
   - Discounts → TotalDeduction = +amount  
   - Promotional reductions → TotalDeduction = +amount

**OUTPUT FORMAT (STRICT JSON):**
{{
  ""invoiceHeader"": {{
    ""InvoiceNo"": ""string or null"",
    ""InvoiceDate"": ""YYYY-MM-DD or null"",
    ""SupplierName"": ""string or null"",
    ""Currency"": ""USD/CNY/XCD or null""
  }},
  ""financialFields"": {{
    ""InvoiceTotal"": 0.00,
    ""SubTotal"": 0.00,
    ""TotalInternalFreight"": 0.00,
    ""TotalOtherCost"": 0.00,
    ""TotalInsurance"": 0.00,
    ""TotalDeduction"": 0.00
  }},
  ""calculatedValidation"": {{
    ""calculatedTotal"": 0.00,
    ""balanceCheck"": 0.00,
    ""validationPassed"": false
  }},
  ""confidence"": ""high/medium/low""
}}

Return ONLY the JSON object, no additional text.";

            _logger.Information("📝 **JSON_PROMPT_CREATED**: Created JSON extraction prompt for {FileName}, length: {Length}", fileName, prompt.Length);
            return prompt;
        }
        
        private InvoiceReferenceData ParseJsonResponse(string jsonResponse, string ocrText, string fileName)
        {
            try
            {
                // Clean JSON response - remove markdown code blocks if present
                var cleanedJson = jsonResponse;
                if (jsonResponse.Contains("```json"))
                {
                    cleanedJson = jsonResponse
                        .Replace("```json", "")
                        .Replace("```", "")
                        .Trim();
                }
                else if (jsonResponse.Contains("```"))
                {
                    // Handle plain code blocks without 'json' specifier
                    cleanedJson = jsonResponse
                        .Replace("```", "")
                        .Trim();
                }
                
                _logger.Information("🧹 **JSON_CLEANUP**: Original length: {OriginalLength}, Cleaned length: {CleanedLength}", 
                    jsonResponse.Length, cleanedJson.Length);
                
                dynamic parsedJson = JsonConvert.DeserializeObject(cleanedJson);
                
                var header = new InvoiceHeader
                {
                    InvoiceNo = parsedJson?.invoiceHeader?.InvoiceNo,
                    InvoiceDate = parsedJson?.invoiceHeader?.InvoiceDate,
                    SupplierName = parsedJson?.invoiceHeader?.SupplierName,
                    Currency = parsedJson?.invoiceHeader?.Currency
                };
                
                var financial = new FinancialFields
                {
                    InvoiceTotal = (double?)parsedJson?.financialFields?.InvoiceTotal,
                    SubTotal = (double?)parsedJson?.financialFields?.SubTotal,
                    TotalInternalFreight = (double?)parsedJson?.financialFields?.TotalInternalFreight,
                    TotalOtherCost = (double?)parsedJson?.financialFields?.TotalOtherCost,
                    TotalInsurance = (double?)parsedJson?.financialFields?.TotalInsurance,
                    TotalDeduction = (double?)parsedJson?.financialFields?.TotalDeduction
                };
                
                var validation = new CalculatedValidation
                {
                    CalculatedTotal = (double?)parsedJson?.calculatedValidation?.calculatedTotal ?? 0,
                    BalanceCheck = (double?)parsedJson?.calculatedValidation?.balanceCheck ?? 0,
                    ValidationPassed = (bool?)parsedJson?.calculatedValidation?.validationPassed ?? false,
                    Formula = "SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction"
                };
                
                return new InvoiceReferenceData
                {
                    Header = header,
                    Financial = financial,
                    Validation = validation,
                    Metadata = new ExtractionMetadata
                    {
                        SourceText = ocrText.Substring(0, Math.Min(100, ocrText.Length)) + "...",
                        ExtractedFieldCount = CountExtractedFields(header, financial),
                        TotalFieldCount = 11,
                        ConfidenceLevel = parsedJson?.confidence ?? "unknown",
                        ProcessingNotes = new List<string> { "Generated with JSON extraction API call" }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **JSON_PARSE_ERROR**: Failed to parse JSON response for {FileName}", fileName);
                throw;
            }
        }

        private ShipmentInvoice CreateBlankInvoice(string fileName)
        {
            return new ShipmentInvoice
            {
                InvoiceNo = fileName,
                SourceFile = fileName,
                SubTotal = null,
                TotalInternalFreight = null,
                TotalOtherCost = null,
                TotalInsurance = null,
                TotalDeduction = null,
                InvoiceTotal = null
            };
        }

        private async Task<List<InvoiceError>> RunDeepSeekDetection(OCRCorrectionService service, ShipmentInvoice invoice, string ocrText)
        {
            // Use reflection to call the private detection method (same as existing tests)
            var methodInfo = typeof(OCRCorrectionService).GetMethod(
                "DetectInvoiceErrorsAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var detectionTask = (Task<List<InvoiceError>>)methodInfo.Invoke(service, new object[] { invoice, ocrText, new Dictionary<string, OCRFieldMetadata>() });
            return await detectionTask;
        }

        private List<DetectedIssue> IdentifyIssues(string fileName, InvoiceReferenceData jsonReference, List<InvoiceError> deepSeekResults, string ocrText)
        {
            var issues = new List<DetectedIssue>();
            
            // Check for multi-order confusion (like COJAY case)
            if (ocrText.Contains("Order ID:") && ocrText.Split(new[] { "Order ID:" }, StringSplitOptions.None).Length > 2)
            {
                issues.Add(new DetectedIssue
                {
                    FileName = fileName,
                    IssueType = "MULTI_ORDER_CONFUSION",
                    Description = "Document contains multiple orders which may cause field mixing",
                    Severity = "HIGH",
                    AffectedFields = new List<string> { "InvoiceTotal", "SubTotal", "TotalOtherCost" }
                });
            }
            
            // Check for balance errors
            var jsonBalance = Math.Abs(jsonReference?.Validation?.BalanceCheck ?? 0);
            var deepSeekBalance = Math.Abs(CalculateDeepSeekBalance(deepSeekResults));
            
            if (jsonBalance > 0.01)
            {
                issues.Add(new DetectedIssue
                {
                    FileName = fileName,
                    IssueType = "JSON_BALANCE_ERROR",
                    Description = $"JSON reference has balance error: {jsonBalance:F4}",
                    Severity = "MEDIUM",
                    AffectedFields = new List<string> { "Financial calculations" }
                });
            }
            
            if (deepSeekBalance > 0.01)
            {
                issues.Add(new DetectedIssue
                {
                    FileName = fileName,
                    IssueType = "DEEPSEEK_BALANCE_ERROR", 
                    Description = $"DeepSeek has balance error: {deepSeekBalance:F4}",
                    Severity = "HIGH",
                    AffectedFields = new List<string> { "Financial calculations" }
                });
            }
            
            // Check for missing patterns
            if (ocrText.ToLower().Contains("credit") && !deepSeekResults.Any(r => r.Field == "TotalInsurance"))
            {
                issues.Add(new DetectedIssue
                {
                    FileName = fileName,
                    IssueType = "MISSED_CREDIT_PATTERN",
                    Description = "Document contains credit but DeepSeek didn't detect TotalInsurance mapping",
                    Severity = "MEDIUM",
                    AffectedFields = new List<string> { "TotalInsurance" }
                });
            }
            
            return issues;
        }

        private double CalculateDeepSeekBalance(List<InvoiceError> errors)
        {
            // Convert DeepSeek errors to financial values and calculate balance
            var financial = new FinancialFields();
            
            foreach (var error in errors)
            {
                if (double.TryParse(error.CorrectValue, out double value))
                {
                    switch (error.Field)
                    {
                        case "InvoiceTotal": financial.InvoiceTotal = value; break;
                        case "SubTotal": financial.SubTotal = value; break;
                        case "TotalInternalFreight": financial.TotalInternalFreight = value; break;
                        case "TotalOtherCost": financial.TotalOtherCost = value; break;
                        case "TotalInsurance": financial.TotalInsurance = value; break;
                        case "TotalDeduction": financial.TotalDeduction = value; break;
                    }
                }
            }
            
            var calculated = (financial.SubTotal ?? 0) + (financial.TotalInternalFreight ?? 0) + 
                           (financial.TotalOtherCost ?? 0) + (financial.TotalInsurance ?? 0) - (financial.TotalDeduction ?? 0);
            return calculated - (financial.InvoiceTotal ?? 0);
        }

        #endregion

        #region Placeholder Methods (To be implemented)

        private int CountExtractedFields(InvoiceHeader header, FinancialFields financial)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(header?.InvoiceNo)) count++;
            if (!string.IsNullOrEmpty(header?.SupplierName)) count++;
            if (!string.IsNullOrEmpty(header?.Currency)) count++;
            if (!string.IsNullOrEmpty(header?.InvoiceDate)) count++;
            if (financial?.InvoiceTotal.HasValue == true) count++;
            if (financial?.SubTotal.HasValue == true) count++;
            if (financial?.TotalInternalFreight.HasValue == true) count++;
            if (financial?.TotalOtherCost.HasValue == true) count++;
            if (financial?.TotalInsurance.HasValue == true) count++;
            if (financial?.TotalDeduction.HasValue == true) count++;
            return count;
        }
        
        private IssueCatalog CatalogAndPrioritizeIssues(List<DetectedIssue> allIssues) => new IssueCatalog();
        private void SaveIssueCatalog(IssueCatalog catalog) { }
        private void GenerateIssueReport(IssueCatalog catalog) { }
        private IssueCatalog LoadIssueCatalog() => new IssueCatalog();
        
        private PromptImprovements CreatePromptImprovements(IssueCategory issue) => new PromptImprovements();
        private void ApplyPromptImprovements(PromptImprovements improvements) { }
        private void SavePromptImprovements(string issueType, PromptImprovements improvements) { }
        
        private bool CheckIfIssueResolved(string fileName, string issueType, InvoiceReferenceData jsonRef, List<InvoiceError> deepSeekResults) => true;
        private void SaveRetestResults(string issueType, List<RetestResult> results) { }

        #endregion
    }

    #region Data Models

    public class DetectedIssue
    {
        public string FileName { get; set; }
        public string IssueType { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; } // HIGH, MEDIUM, LOW
        public List<string> AffectedFields { get; set; } = new();
    }

    public class IssueCategory
    {
        public string IssueType { get; set; }
        public int Priority { get; set; } // 1-10, 10 being highest
        public List<string> AffectedFiles { get; set; } = new();
        public string Description { get; set; }
        public List<string> RecommendedFixes { get; set; } = new();
    }

    public class IssueCatalog
    {
        public DateTime CreatedAt { get; set; }
        public int TotalIssues { get; set; }
        public List<IssueCategory> IssueCategories { get; set; } = new();
    }

    public class PromptImprovements
    {
        public string TargetIssue { get; set; }
        public Dictionary<string, string> DeepSeekImprovements { get; set; } = new();
        public Dictionary<string, string> JsonImprovements { get; set; } = new();
        public string VersionNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RetestResult
    {
        public string FileName { get; set; }
        public string IssueType { get; set; }
        public bool WasResolved { get; set; }
        public double NewJsonBalance { get; set; }
        public double NewDeepSeekBalance { get; set; }
        public string ImprovementDetails { get; set; }
    }

    #endregion
}