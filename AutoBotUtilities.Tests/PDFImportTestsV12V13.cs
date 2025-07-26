using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Events;
using Core.Common.Extensions;
using NUnit.Framework;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using AutoBot;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.NUnit;
using Destructurama.SystemTextJson;
using Destructurama;
using Newtonsoft.Json;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class PDFImportTestsV12V13
    {
        private static Serilog.ILogger _logger;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            LogFilterState.TargetSourceContextForDetails = null;
            LogFilterState.TargetMethodNameForDetails = null;
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Fatal;
            LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Debug;

            try
            {
                string logFilePath = Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    "Logs",
                    $"AutoBotTests-{DateTime.Now:yyyyMMdd}.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                var systemTextJsonOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                };

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Filter.ByIncludingOnly(evt =>
                    {
                        bool hasCategory = evt.Properties.TryGetValue("LogCategory", out var categoryValue);
                        LogCategory category = hasCategory && categoryValue is ScalarValue svCat && svCat.Value is LogCategory lc ? lc : LogCategory.Undefined;
                        bool hasSourceContext = evt.Properties.TryGetValue("SourceContext", out var sourceContextValue);
                        string sourceContext = hasSourceContext && sourceContextValue is ScalarValue svSrc ? svSrc.Value?.ToString() : null;
                        bool hasMemberName = evt.Properties.TryGetValue("MemberName", out var memberNameValue);
                        string memberName = hasMemberName && memberNameValue is ScalarValue svMem ? svMem.Value?.ToString() : null;

                        if (!string.IsNullOrEmpty(LogFilterState.TargetSourceContextForDetails) &&
                            sourceContext != null &&
                            sourceContext.StartsWith(LogFilterState.TargetSourceContextForDetails))
                        {
                            if (string.IsNullOrEmpty(LogFilterState.TargetMethodNameForDetails) ||
                                (memberName != null && memberName.Equals(LogFilterState.TargetMethodNameForDetails, StringComparison.OrdinalIgnoreCase)))
                            {
                                return evt.Level >= LogFilterState.DetailTargetMinimumLevel;
                            }
                        }
                        if (LogFilterState.EnabledCategoryLevels.TryGetValue(category, out var minLevelForCategory))
                        {
                            return evt.Level >= minLevelForCategory;
                        }
                        return evt.Level >= LogEventLevel.Verbose;
                    })
                    .Destructure.With(new CustomSystemTextJsonDestructuringPolicy(systemTextJsonOptions))
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                    .WriteTo.File(
                        logFilePath,
                        restrictedToMinimumLevel: LogEventLevel.Verbose,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 3,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                _logger = Log.ForContext<PDFImportTestsV12V13>();
                _logger.Information("Serilog configured with JSON destructuring policy.");
                _logger.Debug("DSTJ_POLICY_OPTS_DBG: This Debug message from FixtureSetup should appear.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR configuring Serilog programmatically: {ex}");
                Log.Logger = new LoggerConfiguration().MinimumLevel.Warning().WriteTo.Console().CreateLogger();
                _logger = Log.ForContext<PDFImportTestsV12V13>();
                _logger.Error(ex, "Error configuring Serilog programmatically.");
            }

            _logger.Information("--------------------------------------------------");
            _logger.Information("Starting PDFImportTestsV12V13 Test Fixture");
            _logger.Information("--------------------------------------------------");
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
             _logger.Information("--------------------------------------------------");
             _logger.Information("Finished PDFImportTestsV12V13 Test Fixture");
             _logger.Information("--------------------------------------------------");
             Log.CloseAndFlush();
        }

        [SetUp]
        public void SetUp()
        {
             _logger.Information("=== Starting Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
             try
             {
                Infrastructure.Utils.SetTestApplicationSettings(3);
                Infrastructure.Utils.ClearDataBase();
                _logger.Debug("Test setup complete.");
             }
             catch (Exception ex)
             {
                 _logger.Error(ex, "Error during test setup (SetTestApplicationSettings or ClearDataBase).");
                 Assert.Fail($"Test setup failed: {ex.Message}");
             }
        }

        [TearDown]
        public void TearDown()
        {
             _logger.Information("=== Finished Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
        }

        [Test]
        public async Task CanImportTropicalVendorsInvoiceWithV12()
        {
            Console.SetOut(TestContext.Progress);

            try
            {
                using (LogLevelOverride.Begin(LogEventLevel.Verbose))
                {
                    string version = "V12"; // Explicitly set version to V12
                    Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", version); // Set environment variable
                    _logger.Information("Testing Tropical Vendors invoice with SetPartLineValues version: {Version}", version);
                    
                    LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Error;
                    LogFilterState.TargetSourceContextForDetails = "InvoiceReader.OCRCorrectionService";
                    LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;
                    _logger.Information("LogFilterState configured: Global=Error, OCR Correction Service=Verbose");
                    _logger.Information("TargetSourceContextForDetails='{TargetContext}', DetailTargetMinimumLevel='{DetailLevel}'",
                                        LogFilterState.TargetSourceContextForDetails, LogFilterState.DetailTargetMinimumLevel);

                    var testFile = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\06FLIP-SO-0016205IN-20250514-000.PDF";
                    _logger.Information("Test File: {FilePath}", testFile);

                    if (!File.Exists(testFile))
                    {
                        _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                        Assert.Warn($"Test file not found: {testFile}");
                        return;
                    }

                    _logger.Debug("Getting importable file types for PDF.");
                    var fileLst = await FileTypeManager
                        .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF,
                            testFile).ConfigureAwait(false);
                    var fileTypes = fileLst
                        .OfType<CoreEntities.Business.Entities.FileTypes>()
                        .Where(x => x.Description == "Unknown")
                        .ToList();
                    _logger.Debug("Found {Count} 'Unknown' PDF file types.", fileTypes.Count);

                    if (!fileTypes.Any())
                    {
                        _logger.Warning("No suitable 'Unknown' PDF FileType found for: {FilePath}. Skipping test.", testFile);
                        Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                        return;
                    }

                    foreach (var fileType in fileTypes)
                    {
                        _logger.Information("Testing with FileType: {FileTypeDescription} (ID: {FileTypeId})", fileType.Description, fileType.Id);
                        _logger.Debug("Calling PDFUtils.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                        await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType, _logger).ConfigureAwait(false);
                        _logger.Debug("PDFUtils.ImportPDF completed for FileType ID: {FileTypeId}", fileType.Id);

                        _logger.Debug("Verifying import results in database...");
                        using (var ctx = new EntryDataDSContext())
                        {
                            _logger.Verbose("Checking for ShipmentInvoice with InvoiceNo '0016205-IN'");
                            var invoice = ctx.ShipmentInvoice.FirstOrDefault(x => x.InvoiceNo == "0016205-IN");
                            Assert.That(invoice, Is.Not.Null, "ShipmentInvoice '0016205-IN' not created.");
                            _logger.Verbose("ShipmentInvoice found: {Exists}", invoice != null);

                            _logger.Verbose("Checking for ShipmentInvoiceDetails count for InvoiceNo '0016205-IN'");
                            int detailCount = ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == "0016205-IN");
                            Assert.That(detailCount, Is.EqualTo(65), $"Expected 65 ShipmentInvoiceDetails, but found {detailCount}.");
                            _logger.Verbose("ShipmentInvoiceDetails count: {Count}", detailCount);

                            Assert.That(invoice.TotalsZero, Is.EqualTo(0).Within(0.01), $"Expected TotalsZero = 0, but found {invoice.TotalsZero}. OCR correction should ensure proper totals calculation.");

                            _logger.Information("Import successful for FileType {FileTypeId}. Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                               fileType.Id, ctx.ShipmentInvoice.Count(), ctx.ShipmentInvoiceDetails.Count());
                        }
                    }

                    Assert.That(true);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "ERROR in CanImportTropicalVendorsInvoiceWithV12");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

        [Test]
        public async Task CanImportTropicalVendorsInvoiceWithV13()
        {
            Console.SetOut(TestContext.Progress);

            try
            {
               // using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // took out the logleveloverride here because the logs is alot just looking at SetPartValuesv13
               // {
                    string version = "V13"; // Explicitly set version to V13
                    Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", version); // Set environment variable
                    _logger.Information("Testing Tropical Vendors invoice with SetPartLineValues version: {Version}", version);
                    
                    LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Error;
                    LogFilterState.TargetSourceContextForDetails = "InvoiceReader.OCRCorrectionService";
                    LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;
                    _logger.Information("LogFilterState configured: Global=Error, OCR Correction Service=Verbose");
                    _logger.Information("TargetSourceContextForDetails='{TargetContext}', DetailTargetMinimumLevel='{DetailLevel}'",
                                        LogFilterState.TargetSourceContextForDetails, LogFilterState.DetailTargetMinimumLevel);

                    var testFile = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\06FLIP-SO-0016205IN-20250514-000.PDF";
                    _logger.Information("Test File: {FilePath}", testFile);

                    if (!File.Exists(testFile))
                    {
                        _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                        Assert.Warn($"Test file not found: {testFile}");
                        return;
                    }

                    _logger.Debug("Getting importable file types for PDF.");
                    var fileLst = await FileTypeManager
                        .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF,
                            testFile).ConfigureAwait(false);
                    var fileTypes = fileLst
                        .OfType<CoreEntities.Business.Entities.FileTypes>()
                        .Where(x => x.Description == "Unknown")
                        .ToList();
                    _logger.Debug("Found {Count} 'Unknown' PDF file types.", fileTypes.Count);

                    if (!fileTypes.Any())
                    {
                        _logger.Warning("No suitable 'Unknown' PDF FileType found for: {FilePath}. Skipping test.", testFile);
                        Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                        return;
                    }

                    foreach (var fileType in fileTypes)
                    {
                        _logger.Information("Testing with FileType: {FileTypeDescription} (ID: {FileTypeId})", fileType.Description, fileType.Id);
                        _logger.Debug("Calling PDFUtils.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                        await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType, _logger).ConfigureAwait(false);
                        _logger.Debug("PDFUtils.ImportPDF completed for FileType ID: {FileTypeId}", fileType.Id);

                        _logger.Debug("Verifying import results in database...");
                        using (var ctx = new EntryDataDSContext())
                        {
                            _logger.Verbose("Checking for ShipmentInvoice with InvoiceNo '06FLIP-SO-0016205IN'");
                            var invoice = ctx.ShipmentInvoice.FirstOrDefault(x => x.InvoiceNo == "0016205-IN");
                            Assert.That(invoice, Is.Not.Null, "ShipmentInvoice '0016205-IN' not created.");
                            _logger.Verbose("ShipmentInvoice found: {Exists}", invoice != null);

                            _logger.Verbose("Checking for ShipmentInvoiceDetails count for InvoiceNo '06FLIP-SO-0016205IN'");
                            int detailCount = ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == "0016205-IN");
                            Assert.That(detailCount, Is.EqualTo(65), $"Expected 65 ShipmentInvoiceDetails, but found {detailCount}.");
                            _logger.Verbose("ShipmentInvoiceDetails count: {Count}", detailCount);

                            Assert.That(invoice.TotalsZero, Is.EqualTo(0).Within(0.01), $"Expected TotalsZero = 0, but found {invoice.TotalsZero}. OCR correction should ensure proper totals calculation.");

                            _logger.Information("Import successful for FileType {FileTypeId}. Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                               fileType.Id, ctx.ShipmentInvoice.Count(), ctx.ShipmentInvoiceDetails.Count());
                        }
                    }

                    Assert.That(true);
               // }
            }
            catch (Exception e)
            {
                _logger.Error(e, "ERROR in CanImportTropicalVendorsInvoiceWithV13");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }
    }
}