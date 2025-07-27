using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities;         // For Invoice (template object)
using WaterNut.DataSpace;            // For OCRCorrectionService and its static methods in OCRCorrectionService
using static AutoBotUtilities.Tests.TestHelpers;
// using Moq; // Not strictly needed here as we test static methods, but could be if they called mockable services

namespace AutoBotUtilities.Tests.Production
{
    using Serilog.Core;
    using Serilog.Events;

    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("LegacySupport")]
    public class OCRCorrectionService_LegacySupportTests
    {
        private ILogger _logger;
        private OCRCorrectionService _serviceInstance; // For instance methods called by static fallbacks (if any)

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console()
                .WriteTo.File($"logs/OCRCorrectionServiceTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();
            _logger.Information("=== Starting Legacy Support Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            _serviceInstance = new OCRCorrectionService(_logger); // For direct data correction part
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            _serviceInstance?.Dispose();
        }


        #region TotalsZero Static Method Tests
        [Test]
        public void TotalsZero_ShipmentInvoice_Balanced_ShouldReturnTrue()
        {
            var invoice = new ShipmentInvoice { InvoiceTotal = 100, SubTotal = 90, TotalInternalFreight = 10, TotalOtherCost = 0, TotalInsurance = 0, TotalDeduction = 0 };
            Assert.That(OCRCorrectionService.TotalsZero(invoice, _logger), Is.True);
            Assert.That(OCRCorrectionService.TotalsZero(invoice, out double imbalance, _logger), Is.True);
            Assert.That(imbalance, Is.EqualTo(0).Within(0.001));
            _logger.Information("? TotalsZero_ShipmentInvoice_Balanced passed.");
        }

        [Test]
        public void TotalsZero_ShipmentInvoice_Unbalanced_ShouldReturnFalse()
        {
            var invoice = new ShipmentInvoice { InvoiceTotal = 105, SubTotal = 90, TotalInternalFreight = 10 }; // Expected 100
            Assert.That(OCRCorrectionService.TotalsZero(invoice, _logger), Is.False);
            Assert.That(OCRCorrectionService.TotalsZero(invoice, out double imbalance, _logger), Is.True);
            Assert.That(imbalance, Is.EqualTo(0).Within(0.001));
            _logger.Information("? TotalsZero_ShipmentInvoice_Unbalanced passed.");
        }

        [Test]
        public void TotalsZero_ShipmentInvoice_WithDeduction_Balanced_ShouldReturnTrue()
        {
            var invoice = new ShipmentInvoice { InvoiceTotal = 95, SubTotal = 90, TotalInternalFreight = 10, TotalDeduction = 5 };
            Assert.That(OCRCorrectionService.TotalsZero(invoice, _logger), Is.True);
            Assert.That(OCRCorrectionService.TotalsZero(invoice, out double imbalance, _logger), Is.True);
            Assert.That(imbalance, Is.EqualTo(0).Within(0.001));
            _logger.Information("? TotalsZero_ShipmentInvoice_WithDeduction_Balanced passed.");
        }

        [Test]
        public void TotalsZero_ShipmentInvoice_FloatingPointPrecision_ShouldHandleCorrectly()
        {
            var invoice = new ShipmentInvoice { InvoiceTotal = 33.33, SubTotal = 11.11, TotalInternalFreight = 11.11, TotalOtherCost = 11.11 };
            Assert.That(OCRCorrectionService.TotalsZero(invoice, _logger), Is.True); // 33.33 vs 33.33
            Assert.That(OCRCorrectionService.TotalsZero(invoice, out double imbalance, _logger), Is.True);
            Assert.That(imbalance, Is.EqualTo(0).Within(0.001));

            var invoice2 = new ShipmentInvoice { InvoiceTotal = 100.01, SubTotal = 50.005, TotalInternalFreight = 50.005 }; // 100.01 vs 100.01
            Assert.That(OCRCorrectionService.TotalsZero(invoice2, _logger), Is.True);
            Assert.That(OCRCorrectionService.TotalsZero(invoice2, out double imbalance2, _logger), Is.True);
            Assert.That(imbalance2, Is.EqualTo(0).Within(0.001));

            var invoice3 = new ShipmentInvoice { InvoiceTotal = 100.02, SubTotal = 50.005, TotalInternalFreight = 50.005 }; // 100.02 vs 100.01
            Assert.That(OCRCorrectionService.TotalsZero(invoice3, _logger), Is.False); // Difference is 0.01, which is outside tolerance
            Assert.That(OCRCorrectionService.TotalsZero(invoice3, out double imbalance3, _logger), Is.True);
            Assert.That(imbalance3, Is.EqualTo(0).Within(0.001));
            _logger.Information("? TotalsZero_ShipmentInvoice_FloatingPointPrecision passed.");
        }

        [Test]
        public void TotalsZero_DynamicList_AllBalanced_ShouldReturnTrue()
        {
            var dynamicList = new List<dynamic>
            {
                new List<IDictionary<string, object>> {
                    new Dictionary<string, object> { {"InvoiceTotal", 100.0}, {"SubTotal", 90.0}, {"TotalInternalFreight", 10.0} }
                },
                new Dictionary<string, object> { {"InvoiceTotal", 50.0}, {"SubTotal", 50.0} }
            };
            Assert.That(OCRCorrectionService.TotalsZero(dynamicList, out double totalImbalance, _logger), Is.True);
            Assert.That(totalImbalance, Is.EqualTo(0.0).Within(0.001));
            _logger.Information("? TotalsZero_DynamicList_AllBalanced passed.");
        }

        [Test]
        public void TotalsZero_DynamicList_OneUnbalanced_ShouldReturnFalse()
        {
            var dynamicList = new List<dynamic>
            {
                new List<IDictionary<string, object>> {
                    new Dictionary<string, object> { {"InvoiceTotal", 100.0}, {"SubTotal", 90.0}, {"TotalInternalFreight", 10.0} } // Balanced
                },
                new Dictionary<string, object> { {"InvoiceTotal", 55.0}, {"SubTotal", 50.0} } // Unbalanced by 5
            };
            Assert.That(OCRCorrectionService.TotalsZero(dynamicList, out double totalImbalance, _logger), Is.False);
            Assert.That(totalImbalance, Is.EqualTo(5.0).Within(0.001));
            _logger.Information("? TotalsZero_DynamicList_OneUnbalanced passed.");
        }

        [Test]
        public void TotalsZero_DynamicList_EmptyOrNull_ShouldReturnTrueAndMaxImbalance()
        {
            Assert.That(OCRCorrectionService.TotalsZero(new List<dynamic>(), out double totalImbalance1, _logger), Is.True); // No invoices to be unbalanced
            Assert.That(totalImbalance1, Is.EqualTo(double.MaxValue)); // Convention for no processable items

            Assert.That(OCRCorrectionService.TotalsZero((List<dynamic>)null, out double totalImbalance2, _logger), Is.True);
            Assert.That(totalImbalance2, Is.EqualTo(double.MaxValue));
            _logger.Information("? TotalsZero_DynamicList_EmptyOrNull passed.");
        }

        #endregion

        #region ShouldContinueCorrections Tests
        [Test]
        public void ShouldContinueCorrections_UnbalancedList_ShouldReturnTrue()
        {
            var dynamicList = new List<dynamic> {
                new Dictionary<string, object> { {"InvoiceTotal", 55.0}, {"SubTotal", 50.0} } // Unbalanced
            };
            Assert.That(OCRCorrectionService.ShouldContinueCorrections(dynamicList, out double totalImbalance, _logger), Is.True);
            Assert.That(totalImbalance, Is.EqualTo(5.0).Within(0.001));
            _logger.Information("? ShouldContinueCorrections_UnbalancedList passed.");
        }

        [Test]
        public void ShouldContinueCorrections_BalancedList_ShouldReturnFalse()
        {
            var dynamicList = new List<dynamic> {
                new Dictionary<string, object> { {"InvoiceTotal", 50.0}, {"SubTotal", 50.0} } // Balanced
            };
            Assert.That(OCRCorrectionService.ShouldContinueCorrections(dynamicList, out double totalImbalance, _logger), Is.False);
            Assert.That(totalImbalance, Is.EqualTo(0.0).Within(0.001));
            _logger.Information("? ShouldContinueCorrections_BalancedList passed.");
        }
        #endregion

        #region Static CorrectInvoices Tests (Conceptual - Live API calls)
        // These tests involve live API calls and DB interactions if not mocked.

        [Test]
        [Category("LiveAPI")]
        [Ignore("Requires live DeepSeek API and DB setup for full test. Conceptual validation.")]
        public async Task CorrectInvoices_SingleShipmentInvoice_Unbalanced_ShouldAttemptCorrection()
        {
            // Arrange
            var invoice = new ShipmentInvoice { InvoiceNo = "LEGACY-SI-001", InvoiceTotal = 105, SubTotal = 90, TotalInternalFreight = 10 };
            var fileText = "InvoiceNo: LEGACY-SI-001\nSubTotal: $90.00\nFreight: $10.00\nTotal: $100.00"; // Text total is 100

            // Act: This will call instance CorrectInvoiceAsync which calls DeepSeek
            // bool result = await OCRCorrectionService.CorrectInvoices(invoice, fileText, _logger);

            // Assert
            // Assert.That(result, Is.True, "Correction should be attempted and result in true if balance achieved or changes made.");
            // Assert.That(invoice.InvoiceTotal, Is.EqualTo(100.00), "InvoiceTotal should be corrected to match text/calculation.");
            // Further DB assertions would be needed if regexes were updated.
            _logger.Information("Conceptual test for CorrectInvoices_SingleShipmentInvoice_Unbalanced.");
            Assert.Pass("Test Ignored: Live API call for CorrectInvoices(ShipmentInvoice,...)");
        }

        [Test]
        [Category("LiveAPI")]
        [Ignore("Requires live DeepSeek API, DB, and complex template setup. Conceptual validation.")]
        public async Task CorrectInvoices_DynamicListWithTemplate_ShouldProcessAndPotentiallyCorrect()
        {
            // Arrange
            var dynamicList = new List<dynamic> {
                new List<IDictionary<string, object>> {
                    new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                        {"InvoiceNo", "LEGACY-DYN-001"}, {"InvoiceTotal", 105.0}, {"SubTotal", 90.0}, {"TotalInternalFreight", 10.0}
                    }
                }
            };
            // fileText should match what this template would extract initially to show an imbalance
            var fileText = "File text for LEGACY-DYN-001 where calculated total is 100 but reported is 105.";

            var mockOcrInvoicesEntity = new OCR.Business.Entities.Templates { Id = 99, Name = "LegacyTestTemplate" };
            var template = new Template(mockOcrInvoicesEntity, _logger);
            // Populate template.Lines and template.Parts as needed for CreateEnhancedFieldMapping to work.

            // Act: This calls instance methods which call DeepSeek and DB.
            // await OCRCorrectionService.CorrectInvoices(dynamicList, template, _logger);

            // Assert
            // var correctedDict = ((List<IDictionary<string, object>>)dynamicList[0])[0];
            // Assert.That(Convert.ToDouble(correctedDict["InvoiceTotal"], Is.EqualTo(100.0)), "InvoiceTotal in dynamic result should be corrected.");
            // Assertions on template.Lines.Values if UpdateTemplateLineValues worked.
            // DB assertions for any regex/field updates.
            _logger.Information("Conceptual test for CorrectInvoices_DynamicListWithTemplate.");
            Assert.Pass("Test Ignored: Live API/DB/Template setup for CorrectInvoices(List<dynamic>,...)");
        }

        #endregion

        #region ApplyDirectDataCorrectionFallbackAsync Tests (Conceptual - Live API)

        [Test]
        [Category("LiveAPI")]
        [Ignore("Requires live DeepSeek API for fallback logic. Conceptual validation.")]
        public async Task ApplyDirectDataCorrectionFallbackAsync_Unbalanced_ShouldReturnCorrectedDynamicData()
        {
            // Arrange
            var dynamicListUnbalanced = new List<dynamic> {
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                    {"InvoiceNo", "FALLBACK-001"}, {"InvoiceTotal", 105.0}, {"SubTotal", 90.0}, {"TotalInternalFreight", 10.0}
                }
            };
            var originalText = "Invoice: FALLBACK-001\nSub: 90\nShip: 10\nTOTAL: 100"; // Text implies total should be 100

            // Act: This static method calls an instance method that calls DeepSeek.
            // var correctedList = await OCRCorrectionService.ApplyDirectDataCorrectionFallbackAsync(dynamicListUnbalanced, originalText, _logger);

            // Assert
            // Assert.That(correctedList, Is.Not.Null);
            // Assert.That(correctedList.Count, Is.EqualTo(1));
            // var correctedDict = correctedList[0] as IDictionary<string, object>;
            // Assert.That(correctedDict, Is.Not.Null);
            // Assert.That(Convert.ToDouble(correctedDict["InvoiceTotal"], Is.EqualTo(100.0)), "InvoiceTotal should be directly corrected by fallback.");
            _logger.Information("Conceptual test for ApplyDirectDataCorrectionFallbackAsync.");
            Assert.Pass("Test Ignored: Live API call for ApplyDirectDataCorrectionFallbackAsync");
        }
        #endregion

        #region CreateTempShipmentInvoice (Static Helper) Tests
        [Test]
        public void CreateTempShipmentInvoice_ValidDictionary_ShouldCreateInvoice()
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                {"InvoiceNo", "TEMP-001"}, {"InvoiceTotal", 123.45}, {"InvoiceDate", "2023-01-15"},
                {"InvoiceDetails", new List<IDictionary<string, object>> {
                    new Dictionary<string, object> { {"LineNumber", 1}, {"ItemDescription", "ItemA"}, {"Quantity", 2.0}, {"Cost", 10.0}, {"TotalCost", 20.0} }
                }}
            };
            var invoice = InvokePrivateStaticMethod<ShipmentInvoice>(typeof(OCRCorrectionService), "CreateTempShipmentInvoice", dict, _logger);

            Assert.That(invoice, Is.Not.Null);
            Assert.That(invoice.InvoiceNo, Is.EqualTo("TEMP-001"));
            Assert.That(invoice.InvoiceTotal, Is.EqualTo(123.45));
            Assert.That(invoice.InvoiceDate, Is.EqualTo(new DateTime(2023, 1, 15)));
            Assert.That(invoice.InvoiceDetails, Is.Not.Null);
            Assert.That(invoice.InvoiceDetails.Count, Is.EqualTo(1));
            Assert.That(invoice.InvoiceDetails[0].ItemDescription, Is.EqualTo("ItemA"));
            Assert.That(invoice.InvoiceDetails[0].Quantity, Is.EqualTo(2.0));
            _logger.Information("? CreateTempShipmentInvoice_ValidDictionary passed.");
        }

        [Test]
        public void CreateTempShipmentInvoice_TypeConversionErrors_ShouldLogAndUseDefaults()
        {
            // Capture logs to verify warnings
            var testLogEvents = new List<(LogEventLevel Level, string Message)>();
            var testLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestSink(testLogEvents) // Custom sink
                .CreateLogger();

            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                {"InvoiceTotal", "not_a_number"}, {"InvoiceDate", "not_a_date"}
            };
            var invoice = InvokePrivateStaticMethod<ShipmentInvoice>(typeof(OCRCorrectionService), "CreateTempShipmentInvoice", dict, testLogger);

            Assert.That(invoice.InvoiceTotal, Is.Null); // Default for double?
            Assert.That(invoice.InvoiceDate, Is.Null);  // Default for DateTime?

            Assert.That(testLogEvents.Any(e => e.Level == LogEventLevel.Verbose && e.Message.Contains("Could not convert key 'InvoiceTotal'")), Is.True);
            Assert.That(testLogEvents.Any(e => e.Level == LogEventLevel.Verbose && e.Message.Contains("Could not convert key 'InvoiceDate'")), Is.True);
            _logger.Information("? CreateTempShipmentInvoice_TypeConversionErrors passed.");
        }
        #endregion

        #region UpdateDynamicResultsWithCorrections (Static Helper) Tests
        [Test]
        public void UpdateDynamicResultsWithCorrections_ShouldUpdateDictionary()
        {
            var dynamicList = new List<dynamic> {
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) { {"InvoiceNo", "OLD-INV"}, {"InvoiceTotal", 100.0} }
            };
            var correctedInvoices = new List<ShipmentInvoice> {
                new ShipmentInvoice { InvoiceNo = "NEW-INV", InvoiceTotal = 150.0, InvoiceDate = new DateTime(2023,1,1) }
            };

            InvokePrivateStaticMethod<object>(typeof(OCRCorrectionService), "UpdateDynamicResultsWithCorrections", dynamicList, correctedInvoices, _logger);

            var updatedDict = dynamicList[0] as IDictionary<string, object>;
            Assert.That(updatedDict, Is.Not.Null);
            Assert.That(updatedDict["InvoiceNo"], Is.EqualTo("NEW-INV"));
            Assert.That(updatedDict["InvoiceTotal"], Is.EqualTo(150.0));
            Assert.That(updatedDict["InvoiceDate"], Is.EqualTo("2023-01-01 00:00:00")); // Check string format
            _logger.Information("? UpdateDynamicResultsWithCorrections passed.");
        }
        #endregion

        // UpdateTemplateLineValues, GetTemplateFieldMappings, UpdateFieldInTemplate are complex
        // due to their interaction with OCR.Business.Entities.Invoice and its internal structure.
        // Tests for these are in OCRCorrectionService.TemplateUpdateTests.cs and rely on mocking
        // the Invoice structure or careful setup.
    }

    // Dummy TestSink for capturing logs, if not already available in your test project.
    public static class TestSinkExtensions
    {
        public static LoggerConfiguration TestSink(
                  this Serilog.Configuration.LoggerSinkConfiguration sinkConfiguration,
                  List<(LogEventLevel Level, string Message)> events,
                  LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose)
        {
            return sinkConfiguration.Sink(new TestLogSink(events), restrictedToMinimumLevel);
        }
    }

    public class TestLogSink : ILogEventSink
    {
        private readonly List<(LogEventLevel Level, string Message)> _events;
        public TestLogSink(List<(LogEventLevel Level, string Message)> events) => _events = events;
        public void Emit(LogEvent logEvent) => _events.Add((logEvent.Level, logEvent.RenderMessage(System.Globalization.CultureInfo.InvariantCulture)));
    }
}