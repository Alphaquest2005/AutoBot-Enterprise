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
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Main test class for OCR Correction Service with setup and teardown
    /// </summary>
    [TestFixture]
    public partial class OCRCorrectionService_ProductionTests
    {
        #region Test Setup and Configuration

        private static ILogger _logger;
        private OCRCorrectionService _service;
        private string _tempConfigDirectory;
        private string _testDataDirectory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRCorrectionServiceTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting OCR Correction Service Production Tests");

            // Create temporary directories for test data
            _tempConfigDirectory = Path.Combine(Path.GetTempPath(), $"OCRTests_{Guid.NewGuid():N}");
            _testDataDirectory = Path.Combine(_tempConfigDirectory, "TestData");

            Directory.CreateDirectory(_tempConfigDirectory);
            Directory.CreateDirectory(_testDataDirectory);

            _logger.Information("Created temporary directories:");
            _logger.Information("  Config: {ConfigDir}", _tempConfigDirectory);
            _logger.Information("  Data: {DataDir}", _testDataDirectory);
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Created new OCRCorrectionService instance for test with shared logger");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _service?.Dispose();
                _logger.Information("Disposed OCRCorrectionService instance");
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error disposing OCRCorrectionService");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            try
            {
                if (Directory.Exists(_tempConfigDirectory))
                {
                    Directory.Delete(_tempConfigDirectory, true);
                    _logger.Information("Cleaned up temporary directory: {TempDir}", _tempConfigDirectory);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error cleaning up temporary directory");
            }

            _logger.Information("Completed OCR Correction Service Production Tests");
        }

        #endregion

        // All test methods are now in partial class files:
        // - OCRCorrectionServiceTests.CoreFunctionality.cs
        // - OCRCorrectionServiceTests.RegexLearning.cs
        // - OCRCorrectionServiceTests.Performance.cs
        // - OCRCorrectionServiceTests.Helpers.cs
        // - OCRCorrectionService.DatabaseRegexTests.cs
        // - OCRCorrectionService.MetadataExtractionTests.cs
        // - OCRCorrectionService.TemplateUpdateTests.cs
        // - OCRCorrectionService.FieldFormatPatternTests.cs
    }
}
