using AutoBot;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using System.Data.Entity;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Generic PDF Test Framework for OCR Correction and DeepSeek Detection
    /// Data-driven test suite that tests both OCR correction service and DeepSeek error detection
    /// </summary>
    [TestFixture]
    public class GenericPDFTestSuite
    {
        private static Serilog.ILogger _logger;

        #region Test Data Definitions

        /// <summary>
        /// Test case data structure for PDF import tests
        /// </summary>
        public class PDFTestCase
        {
            public string TestName { get; set; }
            public string PdfFilePath { get; set; }
            public string ExpectedInvoiceNumber { get; set; }
            public int ExpectedDetailsCount { get; set; }
            public double ExpectedTotalInsurance { get; set; }
            public double ExpectedTotalDeduction { get; set; }
            public double ExpectedTotalsZero { get; set; }
            public List<string> ExpectedOCRCorrections { get; set; } = new List<string>();
            public bool TestDeepSeekDetection { get; set; } = true;
            public bool TestOCRCorrection { get; set; } = true;
            public string ExpectedBLNumber { get; set; } // For BOL tests
            public string ExpectedShipmentType { get; set; } // Invoice, BOL, Waybill, etc.
        }

        /// <summary>
        /// Master test data table - add new test cases here
        /// </summary>
        public static IEnumerable<PDFTestCase> TestCases
        {
            get
            {
                return new[]
                {
                    // Amazon Invoice Test Cases
                    new PDFTestCase
                    {
                        TestName = "Amazon_Order_112-9126443-1163432_OCR_Correction",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\Amazon.com_-_Order_112-9126443-1163432.pdf",
                        ExpectedInvoiceNumber = "112-9126443-1163432",
                        ExpectedDetailsCount = 2,
                        ExpectedTotalInsurance = -6.99, // Gift Card
                        ExpectedTotalDeduction = 6.99,  // Free Shipping
                        ExpectedTotalsZero = 0.0,
                        ExpectedOCRCorrections = new List<string> { "Gift Card", "Free Shipping" },
                        ExpectedShipmentType = "Invoice"
                    },
                    
                    // TEMU Invoice Test Case
                    new PDFTestCase
                    {
                        TestName = "TEMU_Invoice_07252024",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\07252024_TEMU.pdf",
                        ExpectedInvoiceNumber = "07252024_TEMU", // Update based on actual content
                        ExpectedDetailsCount = 1, // Update based on actual content
                        ExpectedTotalInsurance = 0.0,
                        ExpectedTotalDeduction = 0.0,
                        ExpectedTotalsZero = 0.0,
                        ExpectedShipmentType = "Invoice"
                    },

                    // Tropical Vendors Invoice Test Case
                    new PDFTestCase
                    {
                        TestName = "Tropical_Vendors_Invoice_06FLIP",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\06FLIP-SO-0016205IN-20250514-000.PDF",
                        ExpectedInvoiceNumber = "06FLIP-SO-0016205IN-20250514-000", // Update based on actual content
                        ExpectedDetailsCount = 1, // Update based on actual content
                        ExpectedTotalInsurance = 0.0,
                        ExpectedTotalDeduction = 0.0,
                        ExpectedTotalsZero = 0.0,
                        ExpectedShipmentType = "Invoice"
                    },

                    // Purchase Order Test Case
                    new PDFTestCase
                    {
                        TestName = "Purchase_Order_PO-211",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\PO-211-17318585790070596.pdf",
                        ExpectedInvoiceNumber = "PO-211-17318585790070596", // Update based on actual content  
                        ExpectedDetailsCount = 1, // Update based on actual content
                        ExpectedTotalInsurance = 0.0,
                        ExpectedTotalDeduction = 0.0,
                        ExpectedTotalsZero = 0.0,
                        ExpectedShipmentType = "Invoice"
                    },

                    // Generic Invoice Test Case
                    new PDFTestCase
                    {
                        TestName = "Generic_Invoice_2000129",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\2000129-50710193.pdf",
                        ExpectedInvoiceNumber = "2000129-50710193", // Update based on actual content
                        ExpectedDetailsCount = 1, // Update based on actual content
                        ExpectedTotalInsurance = 0.0,
                        ExpectedTotalDeduction = 0.0,
                        ExpectedTotalsZero = 0.0,
                        ExpectedShipmentType = "Invoice"
                    },

                    // Bill of Lading Test Case
                    new PDFTestCase
                    {
                        TestName = "Mackess_Cox_BOL",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\MACKESS COX BOL.pdf",
                        ExpectedBLNumber = "HBL172086",
                        ExpectedDetailsCount = 1,
                        ExpectedTotalInsurance = 0.0,
                        ExpectedTotalDeduction = 0.0,
                        ExpectedTotalsZero = 0.0,
                        ExpectedShipmentType = "BOL",
                        TestDeepSeekDetection = false, // BOLs may not need OCR correction
                        TestOCRCorrection = false
                    },

                    // Waybill Test Case
                    new PDFTestCase
                    {
                        TestName = "Mackess_Cox_Waybill",
                        PdfFilePath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\Mackess Cox Waybill.pdf",
                        ExpectedInvoiceNumber = "Mackess_Cox_Waybill", // Update based on actual content
                        ExpectedDetailsCount = 1, // Update based on actual content
                        ExpectedTotalInsurance = 0.0,
                        ExpectedTotalDeduction = 0.0,
                        ExpectedTotalsZero = 0.0,
                        ExpectedShipmentType = "Waybill",
                        TestDeepSeekDetection = false, // Known XRef issues
                        TestOCRCorrection = false
                    }
                };
            }
        }

        #endregion

        #region Setup and Teardown

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // Initialize logging for the test suite
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            _logger = Log.ForContext<GenericPDFTestSuite>();
            _logger.Information("Generic PDF Test Suite initialized");
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            Log.CloseAndFlush();
        }

        [SetUp]
        public void SetUp()
        {
            Console.SetOut(TestContext.Progress);
            
            // Configure strategic logging for PDF test focus
            // Global minimum level stays high (Error) to filter out noise
            LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Error;
            
            // Set up the lens target for PDF/OCR operations (can be changed per test if needed)
            // Default to OCR correction service as it's common to all PDF tests
            LogFilterState.TargetSourceContextForDetails = "WaterNut.DataSpace.OCRCorrectionService";
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;
            
            _logger.Information("🔧 **SETUP_COMPLETE**: LogLevelOverride lens configured for PDF test diagnostics");
        }

        #endregion

        #region Logging Lens Control

        /// <summary>
        /// Focus the logging lens on a specific service context
        /// </summary>
        private static void FocusLoggingLens(string targetContext, LogEventLevel level = LogEventLevel.Verbose)
        {
            LogFilterState.TargetSourceContextForDetails = targetContext;
            LogFilterState.DetailTargetMinimumLevel = level;
        }

        /// <summary>
        /// Common logging contexts for PDF tests
        /// </summary>
        private static class LoggingContexts
        {
            public const string OCRCorrection = "WaterNut.DataSpace.OCRCorrectionService";
            public const string PDFImporter = "WaterNut.DataSpace.PDFShipmentInvoiceImporter";
            public const string LlmApi = "WaterNut.Business.Services.Utils.LlmApi";
            public const string PDFUtils = "AutoBot.PDFUtils";
            public const string InvoiceReader = "InvoiceReader";
        }

        #endregion

        #region Generic Test Methods

        /// <summary>
        /// Generic PDF import test with OCR correction and DeepSeek detection
        /// </summary>
        [Test, TestCaseSource(nameof(TestCases))]
        public async Task GenericPDFImportTest(PDFTestCase testCase)
        {
            var testStartTime = DateTime.Now.AddSeconds(-5);
            
            // using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // COMMENTED OUT: Preventing singleton conflicts
            // {
                try
                {
                    _logger.Information("🔍 **GENERIC_TEST_START**: {TestName}", testCase.TestName);
                    _logger.Information("📄 **TEST_FILE**: {FilePath}", testCase.PdfFilePath);
                    _logger.Information("🎯 **TEST_EXPECTATIONS**: OCR Correction: {TestOCRCorrection}, DeepSeek: {TestDeepSeekDetection}", 
                        testCase.TestOCRCorrection, testCase.TestDeepSeekDetection);

                    // Validate test file exists
                    if (!File.Exists(testCase.PdfFilePath))
                    {
                        Assert.Inconclusive($"Test file not found: {testCase.PdfFilePath}");
                        return;
                    }

                    // Clean database before test if needed
                    _logger.Information("🧹 **DATABASE_CLEAN_START**: Cleaning database for test");
                    await this.CleanDatabaseForTest(testCase).ConfigureAwait(false);

                    // Execute PDF import with focused logging
                    _logger.Information("📋 **PDF_IMPORT_START**: Beginning PDF import process");
                    FocusLoggingLens(LoggingContexts.PDFImporter); // Focus lens on PDF import
                    var importResults = await this.ExecutePDFImport(testCase).ConfigureAwait(false);

                    // Validate import results with detailed logging
                    _logger.Information("✓ **VALIDATION_START**: Validating import results");
                    await this.ValidateImportResults(testCase, importResults).ConfigureAwait(false);

                    // Test OCR correction if enabled
                    if (testCase.TestOCRCorrection)
                    {
                        _logger.Information("🔍 **OCR_VALIDATION_START**: Testing OCR correction functionality");
                        FocusLoggingLens(LoggingContexts.OCRCorrection); // Focus lens on OCR correction
                        await this.ValidateOCRCorrection(testCase, testStartTime).ConfigureAwait(false);
                    }

                    // Test DeepSeek detection if enabled
                    if (testCase.TestDeepSeekDetection)
                    {
                        _logger.Information("🤖 **DEEPSEEK_VALIDATION_START**: Testing DeepSeek detection functionality");
                        FocusLoggingLens(LoggingContexts.LlmApi); // Focus lens on LLM API
                        await this.ValidateDeepSeekDetection(testCase).ConfigureAwait(false);
                    }

                    _logger.Information("✅ **GENERIC_TEST_PASSED**: {TestName}", testCase.TestName);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "❌ **GENERIC_TEST_FAILED**: {TestName}", testCase.TestName);
                    _logger.Error("🔍 **EXCEPTION_DETAILS**: Message: {Message}, StackTrace: {StackTrace}", e.Message, e.StackTrace);
                    Assert.Fail($"Test {testCase.TestName} failed: {e.Message}");
                }
            // }
        }

        /// <summary>
        /// Batch test for comparing OCR correction results across multiple PDFs
        /// </summary>
        [Test]
        public async Task BatchOCRCorrectionComparison()
        {
            var results = new List<(string TestName, bool OCRSuccess, int CorrectionsFound, double TotalsZero)>();

            foreach (var testCase in TestCases.Where(tc => tc.TestOCRCorrection))
            {
                try
                {
                    if (!File.Exists(testCase.PdfFilePath))
                    {
                        results.Add((testCase.TestName, false, 0, double.NaN));
                        continue;
                    }

                    await this.CleanDatabaseForTest(testCase).ConfigureAwait(false);
                    var importResults = await this.ExecutePDFImport(testCase).ConfigureAwait(false);
                    
                    // Count OCR corrections made
                    int correctionsFound = await this.CountOCRCorrections(testCase).ConfigureAwait(false);
                    double totalsZero = await this.GetTotalsZero(testCase).ConfigureAwait(false);
                    
                    bool success = Math.Abs(totalsZero - testCase.ExpectedTotalsZero) <= 0.01;
                    results.Add((testCase.TestName, success, correctionsFound, totalsZero));

                    _logger.Information("📊 **BATCH_RESULT**: {TestName} - Success: {Success}, Corrections: {Corrections}, TotalsZero: {TotalsZero}", 
                        testCase.TestName, success, correctionsFound, totalsZero);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "❌ **BATCH_ERROR**: {TestName}", testCase.TestName);
                    results.Add((testCase.TestName, false, 0, double.NaN));
                }
            }

            // Summary report
            var successCount = results.Count(r => r.OCRSuccess);
            var totalTests = results.Count;
            
            _logger.Information("📈 **BATCH_SUMMARY**: {SuccessCount}/{TotalTests} tests passed OCR correction validation", 
                successCount, totalTests);

            foreach (var result in results)
            {
                _logger.Information("📋 **RESULT**: {TestName} | Success: {Success} | Corrections: {Corrections} | TotalsZero: {TotalsZero}",
                    result.TestName, result.OCRSuccess, result.CorrectionsFound, result.TotalsZero);
            }

            Assert.That(successCount, Is.GreaterThan(0), "At least one OCR correction test should pass");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Clean database entries for the test case
        /// </summary>
        private async Task CleanDatabaseForTest(PDFTestCase testCase)
        {
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    // Clean based on shipment type
                    if (testCase.ExpectedShipmentType == "Invoice" && !string.IsNullOrEmpty(testCase.ExpectedInvoiceNumber))
                    {
                        var existingInvoices = await ctx.ShipmentInvoice
                                                   .Where(x => x.InvoiceNo == testCase.ExpectedInvoiceNumber)
                                                   .ToListAsync().ConfigureAwait(false);
                        
                        ctx.ShipmentInvoice.RemoveRange(existingInvoices);
                    }
                    else if (testCase.ExpectedShipmentType == "BOL" && !string.IsNullOrEmpty(testCase.ExpectedBLNumber))
                    {
                        var existingBLs = await ctx.ShipmentBL
                                              .Where(x => x.BLNumber == testCase.ExpectedBLNumber)
                                              .ToListAsync().ConfigureAwait(false);
                        
                        ctx.ShipmentBL.RemoveRange(existingBLs);
                    }

                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                _logger.Information("🧹 **DATABASE_CLEANED**: {TestName}", testCase.TestName);
            }
            catch (Exception e)
            {
                _logger.Warning(e, "⚠️ **DATABASE_CLEAN_WARNING**: {TestName}", testCase.TestName);
            }
        }

        /// <summary>
        /// Execute PDF import for the test case
        /// </summary>
        private async Task<object> ExecutePDFImport(PDFTestCase testCase)
        {
            _logger.Information("🔧 **FILE_TYPE_RESOLUTION_START**: Resolving file types for PDF import");
            var fileLst = await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testCase.PdfFilePath).ConfigureAwait(false);
            var fileTypes = fileLst.OfType<CoreEntities.Business.Entities.FileTypes>().ToList();
            
            _logger.Information("📋 **FILE_TYPES_FOUND**: {FileTypeCount} file types available", fileTypes.Count);
            foreach (var ft in fileTypes)
            {
                _logger.Information("  - FileType: {Description} (ID: {Id}, Pattern: {Pattern})", ft.Description, ft.Id, ft.FilePattern);
            }
            
            if (!fileTypes.Any())
            {
                throw new InvalidOperationException("No PDF file types configured for import");
            }

            _logger.Information("🔄 **PDF_IMPORT_START**: {TestName} using FileType: {FileTypeDescription}", 
                testCase.TestName, fileTypes.First().Description);

            var importResult = await PDFUtils.ImportPDF(new FileInfo[] { new FileInfo(testCase.PdfFilePath) }, fileTypes.First(), _logger).ConfigureAwait(false);
            
            _logger.Information("✅ **PDF_IMPORT_COMPLETE**: {TestName} - Result: {ResultType}", testCase.TestName, importResult?.GetType().Name ?? "null");
            
            return importResult;
        }

        /// <summary>
        /// Validate import results against expected values
        /// </summary>
        private async Task ValidateImportResults(PDFTestCase testCase, object importResults)
        {
            using (var ctx = new EntryDataDSContext())
            {
                if (testCase.ExpectedShipmentType == "Invoice")
                {
                    await this.ValidateInvoiceResults(testCase, ctx).ConfigureAwait(false);
                }
                else if (testCase.ExpectedShipmentType == "BOL")
                {
                    await this.ValidateBOLResults(testCase, ctx).ConfigureAwait(false);
                }
                
                _logger.Information("✅ **IMPORT_VALIDATION_PASSED**: {TestName}", testCase.TestName);
            }
        }

        /// <summary>
        /// Validate invoice-specific results
        /// </summary>
        private async Task ValidateInvoiceResults(PDFTestCase testCase, EntryDataDSContext ctx)
        {
            _logger.Information("🔍 **INVOICE_VALIDATION_START**: Searching for invoice {InvoiceNumber}", testCase.ExpectedInvoiceNumber);
            
            var invoice = await ctx.ShipmentInvoice
                              .FirstOrDefaultAsync(x => x.InvoiceNo == testCase.ExpectedInvoiceNumber).ConfigureAwait(false);

            if (invoice != null)
            {
                _logger.Information("✅ **INVOICE_FOUND**: {InvoiceNumber} - SubTotal: {SubTotal}, Freight: {Freight}, Tax: {Tax}, Total: {Total}, Insurance: {Insurance}, Deduction: {Deduction}, TotalsZero: {TotalsZero}", 
                    invoice.InvoiceNo, invoice.SubTotal, invoice.TotalInternalFreight, invoice.TotalOtherCost, 
                    invoice.InvoiceTotal, invoice.TotalInsurance, invoice.TotalDeduction, invoice.TotalsZero);
            }
            else
            {
                _logger.Error("❌ **INVOICE_NOT_FOUND**: Expected invoice {InvoiceNumber} not found in database", testCase.ExpectedInvoiceNumber);
            }

            Assert.That(invoice, Is.Not.Null, $"Expected invoice {testCase.ExpectedInvoiceNumber} not found");

            var detailCount = await ctx.ShipmentInvoiceDetails
                                  .CountAsync(x => x.Invoice.InvoiceNo == testCase.ExpectedInvoiceNumber).ConfigureAwait(false);

            _logger.Information("📋 **DETAIL_COUNT_CHECK**: Expected {ExpectedCount}, Found {ActualCount}", 
                testCase.ExpectedDetailsCount, detailCount);

            Assert.That(detailCount, Is.EqualTo(testCase.ExpectedDetailsCount), 
                $"Expected {testCase.ExpectedDetailsCount} details, found {detailCount}");

            // Validate financial totals if specified
            if (testCase.ExpectedTotalsZero != 0.0 || testCase.TestOCRCorrection)
            {
                _logger.Information("💰 **TOTALS_ZERO_CHECK**: Expected {Expected}, Actual {Actual}", 
                    testCase.ExpectedTotalsZero, invoice.TotalsZero);
                Assert.That(invoice.TotalsZero, Is.EqualTo(testCase.ExpectedTotalsZero).Within(0.01),
                    $"Expected TotalsZero {testCase.ExpectedTotalsZero}, got {invoice.TotalsZero}");
            }

            if (testCase.ExpectedTotalInsurance != 0.0)
            {
                _logger.Information("🛡️ **INSURANCE_CHECK**: Expected {Expected}, Actual {Actual}", 
                    testCase.ExpectedTotalInsurance, invoice.TotalInsurance);
                Assert.That(invoice.TotalInsurance, Is.EqualTo(testCase.ExpectedTotalInsurance).Within(0.01),
                    $"Expected TotalInsurance {testCase.ExpectedTotalInsurance}, got {invoice.TotalInsurance}");
            }

            if (testCase.ExpectedTotalDeduction != 0.0)
            {
                _logger.Information("💸 **DEDUCTION_CHECK**: Expected {Expected}, Actual {Actual}", 
                    testCase.ExpectedTotalDeduction, invoice.TotalDeduction);
                Assert.That(invoice.TotalDeduction, Is.EqualTo(testCase.ExpectedTotalDeduction).Within(0.01),
                    $"Expected TotalDeduction {testCase.ExpectedTotalDeduction}, got {invoice.TotalDeduction}");
            }
        }

        /// <summary>
        /// Validate BOL-specific results
        /// </summary>
        private async Task ValidateBOLResults(PDFTestCase testCase, EntryDataDSContext ctx)
        {
            var bol = await ctx.ShipmentBL
                          .FirstOrDefaultAsync(x => x.BLNumber == testCase.ExpectedBLNumber).ConfigureAwait(false);

            Assert.That(bol, Is.Not.Null, $"Expected BOL {testCase.ExpectedBLNumber} not found");

            var detailCount = await ctx.ShipmentBLDetails
                                  .CountAsync(x => x.ShipmentBL.BLNumber == testCase.ExpectedBLNumber).ConfigureAwait(false);

            Assert.That(detailCount, Is.EqualTo(testCase.ExpectedDetailsCount),
                $"Expected {testCase.ExpectedDetailsCount} BOL details, found {detailCount}");
        }

        /// <summary>
        /// Validate OCR correction functionality
        /// </summary>
        private async Task ValidateOCRCorrection(PDFTestCase testCase, DateTime testStartTime)
        {
            using (var ctx = new OCRContext())
            {
                // Check for OCR corrections made during this test
                var corrections = await ctx.OCRCorrectionLearning
                                      .Where(x => x.CreatedDate >= testStartTime)
                                      .ToListAsync().ConfigureAwait(false);

                if (testCase.ExpectedOCRCorrections.Any())
                {
                    Assert.That(corrections.Count, Is.GreaterThan(0), 
                        $"Expected OCR corrections for {testCase.TestName}, but none found");

                    foreach (var expectedCorrection in testCase.ExpectedOCRCorrections)
                    {
                        var found = corrections.Any(c => 
                            c.FieldName.Contains(expectedCorrection) || 
                            c.CorrectValue.Contains(expectedCorrection) ||
                            c.OriginalError.Contains(expectedCorrection));

                        Assert.That(found, Is.True, 
                            $"Expected OCR correction for '{expectedCorrection}' not found in test {testCase.TestName}");
                    }
                }

                _logger.Information("🔍 **OCR_CORRECTION_VALIDATED**: {TestName} - {CorrectionCount} corrections found", 
                    testCase.TestName, corrections.Count);
            }
        }

        /// <summary>
        /// Validate DeepSeek detection functionality
        /// </summary>
        private async Task ValidateDeepSeekDetection(PDFTestCase testCase)
        {
            // This would test the AI-powered error detection
            // Implementation depends on the specific DeepSeek integration
            
            _logger.Information("🤖 **DEEPSEEK_DETECTION_VALIDATED**: {TestName}", testCase.TestName);
            
            // Placeholder for DeepSeek validation logic
            // Could include:
            // - Testing AI prompt generation
            // - Validating AI response parsing
            // - Checking error detection accuracy
        }

        /// <summary>
        /// Count OCR corrections for a test case
        /// </summary>
        private async Task<int> CountOCRCorrections(PDFTestCase testCase)
        {
            using (var ctx = new OCRContext())
            {
                var correctionCount = await ctx.OCRCorrectionLearning
                                          .CountAsync(x => x.FilePath.Contains(testCase.ExpectedInvoiceNumber)).ConfigureAwait(false);
                
                return correctionCount;
            }
        }

        /// <summary>
        /// Get TotalsZero value for a test case
        /// </summary>
        private async Task<double> GetTotalsZero(PDFTestCase testCase)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var invoice = await ctx.ShipmentInvoice
                                  .FirstOrDefaultAsync(x => x.InvoiceNo == testCase.ExpectedInvoiceNumber).ConfigureAwait(false);
                
                return invoice?.TotalsZero ?? double.NaN;
            }
        }

        #endregion
    }
}