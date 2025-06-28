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
    /// Systematic AI Prompt Testing & Improvement Framework
    /// Provides automated, regression-proof testing process for continuous prompt improvement
    /// </summary>
    [TestFixture]
    public class SystematicPromptValidationSuite
    {
        private static ILogger _logger;
        private const int BATCH_SIZE = 5; // Process in small batches for manageable analysis
        private const string BASELINE_VERSION = "v1.0";
        private string _currentPromptVersion = BASELINE_VERSION;
        
        // Paths
        private const string ExtractedTextPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Extracted Text";
        private const string ReferenceDataPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Reference Data";
        private const string PromptVersionsPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Prompt Versions";
        private const string TestExecutionPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Execution";
        
        // Test execution tracking
        private static Dictionary<string, TestBatchResult> _testHistory = new();
        private static List<string> _processedFiles = new();
        private static int _currentBatchNumber = 1;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // Initialize logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.NUnitOutput()
                .CreateLogger();

            _logger = Log.ForContext<SystematicPromptValidationSuite>();
            
            // Ensure directories exist
            Directory.CreateDirectory(PromptVersionsPath);
            Directory.CreateDirectory(TestExecutionPath);
            
            _logger.Information("🚀 **SYSTEMATIC_TESTING_FRAMEWORK_INITIALIZED**: Starting systematic prompt validation process");
            _logger.Information("📂 **PATHS_CONFIGURED**: ExtractedText: {ExtractedPath}, Reference: {RefPath}, Versions: {VersionPath}", 
                ExtractedTextPath, ReferenceDataPath, PromptVersionsPath);
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Phase 1: Discover and categorize all test files for systematic processing
        /// </summary>
        [Test, Order(1)]
        public void Phase1_DiscoverAndCategorizeTestFiles()
        {
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("🔍 **PHASE_1_START**: Discovering and categorizing test files");
                
                var allTextFiles = Directory.GetFiles(ExtractedTextPath, "*.txt")
                    .Where(f => !Path.GetFileName(f).StartsWith("extraction_inventory"))
                    .OrderBy(f => f) // Consistent ordering for reproducibility
                    .ToArray();
                
                _logger.Information("📋 **FILE_DISCOVERY**: Found {FileCount} text files for systematic testing", allTextFiles.Length);
                
                // Categorize by vendor/type for systematic coverage
                var categorizedFiles = CategorizeTestFiles(allTextFiles);
                LogFileCategorization(categorizedFiles);
                
                // Create test batches ensuring vendor diversity
                var testBatches = CreateDiverseBatches(categorizedFiles, BATCH_SIZE);
                _logger.Information("📦 **BATCH_CREATION**: Created {BatchCount} diverse test batches", testBatches.Count);
                
                // Save test plan for tracking and recovery
                var testPlan = new TestExecutionPlan
                {
                    TotalFiles = allTextFiles.Length,
                    Batches = testBatches,
                    CreatedAt = DateTime.Now,
                    InitialPromptVersion = _currentPromptVersion
                };
                
                SaveTestExecutionPlan(testPlan);
                
                _logger.Information("✅ **PHASE_1_COMPLETE**: {TotalFiles} files organized into {BatchCount} batches for systematic testing", 
                    allTextFiles.Length, testBatches.Count);
                
                Assert.That(testBatches.Count, Is.GreaterThan(0), "Test batches should be created");
                Assert.That(testBatches.Sum(b => b.Files.Count), Is.EqualTo(allTextFiles.Length), "All files should be included in batches");
            }
        }

        /// <summary>
        /// Phase 2: Create baseline benchmark by testing all files with current prompts
        /// </summary>
        [Test, Order(2)]
        public void Phase2_CreateBaselineBenchmark()
        {
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("🎯 **PHASE_2_START**: Creating baseline benchmark for all test files");
                
                var testPlan = LoadTestExecutionPlan();
                var baselineResults = new BaselineBenchmark
                {
                    PromptVersion = _currentPromptVersion,
                    StartTime = DateTime.Now,
                    BatchResults = new List<TestBatchResult>()
                };
                
                var batchNumber = 1;
                foreach (var batch in testPlan.Batches)
                {
                    _logger.Information("🔧 **PROCESSING_BATCH**: {BatchNumber}/{TotalBatches} - {FileCount} files", 
                        batchNumber, testPlan.Batches.Count, batch.Files.Count);
                    
                    var batchResult = ExecuteBatch(batch, _currentPromptVersion);
                    baselineResults.BatchResults.Add(batchResult);
                    
                    // Log batch performance
                    _logger.Information("📊 **BATCH_RESULTS**: {BatchNumber} - Avg F1: {AvgF1:F3}, Balance Pass Rate: {BalanceRate:F3}", 
                        batchNumber, batchResult.AverageF1Score, batchResult.BalancePassRate);
                    
                    // Save after each batch for recovery capability
                    SaveBaselineResults(baselineResults);
                    batchNumber++;
                }
                
                // Calculate overall baseline metrics
                baselineResults.CompleteTime = DateTime.Now;
                baselineResults.OverallMetrics = CalculateOverallMetrics(baselineResults.BatchResults);
                
                // Establish performance targets based on baseline
                var performanceTargets = EstablishPerformanceTargets(baselineResults);
                SavePerformanceTargets(performanceTargets);
                
                // Analyze baseline for improvement opportunities
                var improvementPlan = AnalyzeBaselineAndCreateImprovementPlan(baselineResults);
                SaveImprovementPlan(improvementPlan);
                
                _logger.Information("✅ **PHASE_2_COMPLETE**: Baseline established - Overall F1: {F1:F3}, Balance Pass: {Balance:F3}", 
                    baselineResults.OverallMetrics.AverageF1Score, baselineResults.OverallMetrics.BalancePassRate);
                
                Assert.That(baselineResults.BatchResults.Count, Is.EqualTo(testPlan.Batches.Count), "All batches should be processed");
                Assert.That(baselineResults.OverallMetrics.TotalFiles, Is.GreaterThan(0), "Baseline should include test results");
            }
        }

        /// <summary>
        /// Phase 3: Systematic improvement cycle with regression protection
        /// </summary>
        [Test, Order(3)]
        public void Phase3_SystematicImprovementCycle()
        {
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("🔄 **PHASE_3_START**: Beginning systematic improvement cycle with regression protection");
                
                var improvementPlan = LoadImprovementPlan();
                var currentResults = LoadBaselineResults();
                var cycleNumber = 1;
                
                foreach (var improvementCycle in improvementPlan.Cycles.Take(3)) // Limit to first 3 cycles for initial testing
                {
                    _logger.Information("🎯 **IMPROVEMENT_CYCLE_START**: Cycle {CycleNumber} - Targeting {IssueType}", 
                        cycleNumber, improvementCycle.TargetIssueType);
                    
                    try
                    {
                        // Test current problem batch
                        var problemBatch = improvementCycle.TargetBatch;
                        var currentPerformance = TestBatch(problemBatch, _currentPromptVersion);
                        
                        _logger.Information("📊 **CURRENT_PERFORMANCE**: Batch F1: {F1:F3}, Balance Pass: {Balance:F3}", 
                            currentPerformance.AverageF1Score, currentPerformance.BalancePassRate);
                        
                        // Analyze failures and create improvements
                        var improvements = AnalyzeFailuresAndCreateImprovements(currentPerformance);
                        
                        if (improvements.Any())
                        {
                            // Apply improvements to prompts
                            var newPromptVersion = ApplyImprovements(improvements);
                            
                            // Test improved prompts on problem batch
                            var improvedPerformance = TestBatch(problemBatch, newPromptVersion);
                            
                            _logger.Information("📈 **IMPROVED_PERFORMANCE**: Batch F1: {F1:F3}, Balance Pass: {Balance:F3}", 
                                improvedPerformance.AverageF1Score, improvedPerformance.BalancePassRate);
                            
                            // CRITICAL: Regression test against ALL previous tests
                            var regressionResults = RunRegressionTests(newPromptVersion);
                            
                            if (HasAcceptableRegression(regressionResults))
                            {
                                // Accept improvements
                                _currentPromptVersion = newPromptVersion;
                                currentResults = UpdateCurrentResults(currentResults, improvedPerformance);
                                
                                _logger.Information("✅ **IMPROVEMENT_ACCEPTED**: Version {Version} - F1 improved by {Improvement:F3}", 
                                    newPromptVersion, improvedPerformance.AverageF1Score - currentPerformance.AverageF1Score);
                            }
                            else
                            {
                                // Reject improvements due to regression
                                RevertPromptChanges(newPromptVersion);
                                
                                _logger.Warning("❌ **IMPROVEMENT_REJECTED**: Version {Version} - Caused {RegressionCount} regressions", 
                                    newPromptVersion, regressionResults.RegressionCount);
                            }
                            
                            // Generate detailed cycle report
                            GenerateCycleReport(cycleNumber, improvementCycle, currentPerformance, improvedPerformance, regressionResults);
                        }
                        else
                        {
                            _logger.Information("ℹ️ **NO_IMPROVEMENTS_IDENTIFIED**: Cycle {CycleNumber} - No actionable improvements found", cycleNumber);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "🚨 **CYCLE_ERROR**: Exception in improvement cycle {CycleNumber}", cycleNumber);
                    }
                    
                    cycleNumber++;
                }
                
                // Generate final progress report
                GenerateProgressReport(_currentPromptVersion, currentResults);
                
                _logger.Information("✅ **PHASE_3_COMPLETE**: Completed {CycleCount} improvement cycles with final version {Version}", 
                    cycleNumber - 1, _currentPromptVersion);
            }
        }

        #region Helper Methods

        private Dictionary<string, List<string>> CategorizeTestFiles(string[] allTextFiles)
        {
            var categories = new Dictionary<string, List<string>>();
            
            foreach (var file in allTextFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var category = DetermineVendorCategory(fileName);
                
                if (!categories.ContainsKey(category))
                    categories[category] = new List<string>();
                
                categories[category].Add(file);
            }
            
            return categories;
        }

        private string DetermineVendorCategory(string fileName)
        {
            fileName = fileName.ToUpper();
            if (fileName.Contains("AMAZON")) return "Amazon";
            if (fileName.Contains("SHEIN")) return "SHEIN";
            if (fileName.Contains("TEMU")) return "TEMU";
            if (fileName.Contains("COJAY")) return "COJAY";
            if (fileName.Contains("FASHIO")) return "FASHIONNOVA";
            if (fileName.Contains("HAWB")) return "Shipping";
            return "Other";
        }

        private void LogFileCategorization(Dictionary<string, List<string>> categorizedFiles)
        {
            foreach (var category in categorizedFiles)
            {
                _logger.Information("📂 **CATEGORY**: {Category} - {Count} files", 
                    category.Key, category.Value.Count);
            }
        }

        private List<TestBatch> CreateDiverseBatches(Dictionary<string, List<string>> categorizedFiles, int batchSize)
        {
            var batches = new List<TestBatch>();
            var allFiles = categorizedFiles.SelectMany(c => c.Value).ToList();
            var batchNumber = 1;
            
            for (int i = 0; i < allFiles.Count; i += batchSize)
            {
                var batchFiles = allFiles.Skip(i).Take(batchSize).ToList();
                var batch = new TestBatch
                {
                    BatchNumber = batchNumber,
                    Files = batchFiles,
                    VendorDistribution = CalculateVendorDistribution(batchFiles)
                };
                
                batches.Add(batch);
                batchNumber++;
            }
            
            return batches;
        }

        private Dictionary<string, int> CalculateVendorDistribution(List<string> files)
        {
            var distribution = new Dictionary<string, int>();
            
            foreach (var file in files)
            {
                var vendor = DetermineVendorCategory(Path.GetFileNameWithoutExtension(file));
                distribution[vendor] = distribution.GetValueOrDefault(vendor, 0) + 1;
            }
            
            return distribution;
        }

        private TestBatchResult ExecuteBatch(TestBatch batch, string promptVersion)
        {
            // Implementation will call existing dual-validation framework
            // This is a placeholder for the actual implementation
            return new TestBatchResult
            {
                BatchNumber = batch.BatchNumber,
                PromptVersion = promptVersion,
                Files = batch.Files,
                Results = new List<AIDetectionValidationResult>(),
                AverageF1Score = 0.67, // Placeholder
                BalancePassRate = 0.85, // Placeholder
                ProcessingTime = TimeSpan.FromMinutes(5)
            };
        }

        #endregion

        #region File Operations

        private void SaveTestExecutionPlan(TestExecutionPlan testPlan)
        {
            var planPath = Path.Combine(TestExecutionPath, "Test_Execution_Plan.json");
            var json = JsonConvert.SerializeObject(testPlan, Formatting.Indented);
            File.WriteAllText(planPath, json);
            
            _logger.Information("💾 **TEST_PLAN_SAVED**: Execution plan saved to {PlanPath}", planPath);
        }

        private TestExecutionPlan LoadTestExecutionPlan()
        {
            var planPath = Path.Combine(TestExecutionPath, "Test_Execution_Plan.json");
            if (!File.Exists(planPath))
                throw new FileNotFoundException("Test execution plan not found. Run Phase1 first.");
            
            var json = File.ReadAllText(planPath);
            return JsonConvert.DeserializeObject<TestExecutionPlan>(json);
        }

        private void SaveBaselineResults(BaselineBenchmark baseline)
        {
            var baselinePath = Path.Combine(TestExecutionPath, $"Baseline_Results_{baseline.PromptVersion}.json");
            var json = JsonConvert.SerializeObject(baseline, Formatting.Indented);
            File.WriteAllText(baselinePath, json);
        }

        private BaselineBenchmark LoadBaselineResults()
        {
            var baselinePath = Path.Combine(TestExecutionPath, $"Baseline_Results_{BASELINE_VERSION}.json");
            if (!File.Exists(baselinePath))
                throw new FileNotFoundException("Baseline results not found. Run Phase2 first.");
            
            var json = File.ReadAllText(baselinePath);
            return JsonConvert.DeserializeObject<BaselineBenchmark>(json);
        }

        #endregion

        #region Placeholder Methods (To be implemented)

        private OverallMetrics CalculateOverallMetrics(List<TestBatchResult> batchResults) => new OverallMetrics();
        private PerformanceTargets EstablishPerformanceTargets(BaselineBenchmark baseline) => new PerformanceTargets();
        private void SavePerformanceTargets(PerformanceTargets targets) { }
        private ImprovementPlan AnalyzeBaselineAndCreateImprovementPlan(BaselineBenchmark baseline) => new ImprovementPlan();
        private void SaveImprovementPlan(ImprovementPlan plan) { }
        private ImprovementPlan LoadImprovementPlan() => new ImprovementPlan();
        private TestBatchResult TestBatch(TestBatch batch, string promptVersion) => new TestBatchResult();
        private List<PromptImprovement> AnalyzeFailuresAndCreateImprovements(TestBatchResult results) => new List<PromptImprovement>();
        private string ApplyImprovements(List<PromptImprovement> improvements) => $"{_currentPromptVersion}.1";
        private RegressionTestResult RunRegressionTests(string promptVersion) => new RegressionTestResult();
        private bool HasAcceptableRegression(RegressionTestResult results) => results.RegressionCount <= 2;
        private void RevertPromptChanges(string version) { }
        private BaselineBenchmark UpdateCurrentResults(BaselineBenchmark current, TestBatchResult improved) => current;
        private void GenerateCycleReport(int cycleNumber, ImprovementCycle cycle, TestBatchResult current, TestBatchResult improved, RegressionTestResult regression) { }
        private void GenerateProgressReport(string version, BaselineBenchmark results) { }

        #endregion
    }

    #region Data Models

    public class TestExecutionPlan
    {
        public int TotalFiles { get; set; }
        public List<TestBatch> Batches { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string InitialPromptVersion { get; set; }
    }

    public class TestBatch
    {
        public int BatchNumber { get; set; }
        public List<string> Files { get; set; } = new();
        public Dictionary<string, int> VendorDistribution { get; set; } = new();
    }

    public class TestBatchResult
    {
        public int BatchNumber { get; set; }
        public string PromptVersion { get; set; }
        public List<string> Files { get; set; } = new();
        public List<AIDetectionValidationResult> Results { get; set; } = new();
        public double AverageF1Score { get; set; }
        public double BalancePassRate { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    public class BaselineBenchmark
    {
        public string PromptVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompleteTime { get; set; }
        public List<TestBatchResult> BatchResults { get; set; } = new();
        public OverallMetrics OverallMetrics { get; set; }
    }

    public class OverallMetrics
    {
        public int TotalFiles { get; set; }
        public double AverageF1Score { get; set; }
        public double BalancePassRate { get; set; }
        public Dictionary<string, double> VendorPerformance { get; set; } = new();
    }

    public class PerformanceTargets
    {
        public double TargetF1Score { get; set; } = 0.80;
        public double TargetBalancePassRate { get; set; } = 0.95;
        public double MaxVendorBias { get; set; } = 0.10;
    }

    public class ImprovementPlan
    {
        public List<ImprovementCycle> Cycles { get; set; } = new();
    }

    public class ImprovementCycle
    {
        public int CycleNumber { get; set; }
        public string TargetIssueType { get; set; }
        public TestBatch TargetBatch { get; set; }
        public List<string> ExpectedImprovements { get; set; } = new();
    }

    public class PromptImprovement
    {
        public string ImprovementType { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> DeepSeekChanges { get; set; } = new();
        public Dictionary<string, string> JsonChanges { get; set; } = new();
    }

    public class RegressionTestResult
    {
        public int RegressionCount { get; set; }
        public List<RegressionFailure> Regressions { get; set; } = new();
        public Dictionary<string, PerformanceComparison> Comparisons { get; set; } = new();
    }

    public class RegressionFailure
    {
        public string FileName { get; set; }
        public double OldF1Score { get; set; }
        public double NewF1Score { get; set; }
        public double PerformanceDelta { get; set; }
        public string FailureReason { get; set; }
    }

    public class PerformanceComparison
    {
        public AIDetectionValidationResult OldResult { get; set; }
        public AIDetectionValidationResult NewResult { get; set; }
        public double F1Delta { get; set; }
        public bool IsRegression { get; set; }
    }

    #endregion
}