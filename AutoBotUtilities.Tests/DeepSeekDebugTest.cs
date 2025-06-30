using System;
using System.IO;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using System.Collections.Generic;
using System.Linq;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DeepSeekDebugTest
    {
        private ILogger _logger;
        private OCRCorrectionService _service;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
            
            _service = new OCRCorrectionService(_logger);
        }

        [Test]
        public async Task DebugMangoDeepSeekResponse()
        {
            // Load MANGO OCR text
            var ocrPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Extracted Text\03152025_TOTAL AMOUNT.txt";
            var ocrText = File.ReadAllText(ocrPath);
            
            // Create blank invoice like diagnostic test does
            var blankInvoice = new ShipmentInvoice { InvoiceNo = "03152025_TOTAL AMOUNT" };
            
            // Create empty metadata like diagnostic test does
            var metadata = new Dictionary<string, WaterNut.DataSpace.OCRFieldMetadata>();
            
            _logger.Information("🚀 **DEBUG_TEST_START**: Testing DeepSeek with blank invoice and MANGO OCR text");
            _logger.Information("📋 **BLANK_INVOICE_STATE**: InvoiceNo={InvoiceNo}, InvoiceTotal={InvoiceTotal}, SubTotal={SubTotal}", 
                blankInvoice.InvoiceNo, blankInvoice.InvoiceTotal, blankInvoice.SubTotal);
            
            // Call DeepSeek error detection
            var detectedErrors = await this._service.DetectInvoiceErrorsForDiagnosticsAsync(blankInvoice, ocrText, metadata).ConfigureAwait(false);
            
            _logger.Information("✅ **DEBUG_RESULTS**: DeepSeek detected {ErrorCount} errors", detectedErrors.Count);
            
            if (detectedErrors.Any())
            {
                foreach (var error in detectedErrors)
                {
                    _logger.Information("   - Error: Field={Field}, Type={Type}, Correct={Correct}, Line={Line}", 
                        error.Field, error.ErrorType, error.CorrectValue, error.LineNumber);
                }
            }
            else
            {
                _logger.Warning("❌ **PROBLEM_FOUND**: DeepSeek found 0 errors with blank invoice - this indicates a prompt or detection issue");
                _logger.Warning("🔍 **EXPECTED_BEHAVIOR**: With blank invoice, DeepSeek should detect omissions for InvoiceTotal, SubTotal, etc.");
            }
            
            // Verify expectation
            Assert.That(detectedErrors.Count, Is.GreaterThan(0), 
                "DeepSeek should detect omissions when comparing blank invoice to rich OCR text");
        }
    }
}