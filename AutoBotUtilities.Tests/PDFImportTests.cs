using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities; // Needed for EntryDataDSContext
using WaterNut.Business.Services.Utils; // For PDFUtils, FileTypeManager
using WaterNut.DataSpace; // For FileTypes enum? Check namespace
using AutoBot; // For PDFUtils if namespace is AutoBot
using Microsoft.Extensions.Configuration; // Added for config builder & SetBasePath
using Serilog; // Added for logging
using Serilog.Sinks.NUnit; // Required for .WriteTo.NUnit()


namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class PDFImportTests
    {
        // Define logger instance for the test class
        private static Serilog.ILogger _logger; // Use fully qualified name

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // Configure Serilog directly in code
            try
            {
                string logFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Logs", "AutoBotTests-.log");
                // Ensure log directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose() // Set default level
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Override specific namespaces
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext() // Enrichers
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console() // Console Sink
                   // .WriteTo.NUnit()   // Add NUnit Sink
                    .WriteTo.File(logFilePath, // File Sink
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 3,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                _logger = Log.ForContext<PDFImportTests>(); // Get logger instance for this class
                _logger.Information("Serilog configured programmatically for tests.");
            }
            catch (Exception ex)
            {
                 // Fallback to console logger if configuration fails
                 Console.WriteLine($"ERROR configuring Serilog programmatically: {ex}");
                 Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Warning()
                     .WriteTo.Console()
                     .CreateLogger();
                 _logger = Log.ForContext<PDFImportTests>();
                 _logger.Error(ex, "Error configuring Serilog programmatically.");
            }

            _logger.Information("--------------------------------------------------");
            _logger.Information("Starting PDFImportTests Test Fixture");
            _logger.Information("--------------------------------------------------");
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
             _logger.Information("--------------------------------------------------");
             _logger.Information("Finished PDFImportTests Test Fixture");
             _logger.Information("--------------------------------------------------");
             Log.CloseAndFlush(); // Ensure logs are written
        }


        [SetUp]
        public void SetUp()
        {
             _logger.Information("=== Starting Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
             // Ensure test settings are applied and DB is cleared before each test in this fixture
             _logger.Debug("Applying test application settings (3) and clearing database.");
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
        public async Task CanImportAmazonMultiSectionInvoice()
        {
            Console.SetOut(TestContext.Progress);
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf";
                 _logger.Information("Test File: {FilePath}", testFile);

                if (!File.Exists(testFile))
                {
                     _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                     Assert.Warn($"Test file not found: {testFile}");
                     return;
                }

                 _logger.Debug("Getting importable file types for PDF.");
                // Assuming FileTypeManager is static
                var fileLst = await FileTypeManager // Removed .Instance
                    .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF,
                        testFile).ConfigureAwait(false);
                var fileTypes = fileLst
                     .OfType<CoreEntities.Business.Entities.FileTypes>() // Ensure correct type
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
                     // Assuming PDFUtils is static
                     await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false); // Removed .Instance
                     _logger.Debug("PDFUtils.ImportPDF completed for FileType ID: {FileTypeId}", fileType.Id);


                     _logger.Debug("Verifying import results in database...");
                     using (var ctx = new EntryDataDSContext())
                     {
                          _logger.Verbose("Checking for ShipmentInvoice with InvoiceNo '114-7827932-2029910'");
                          bool invoiceExists = ctx.ShipmentInvoice.Any(x => x.InvoiceNo == "114-7827932-2029910");
                          Assert.That(invoiceExists, Is.True, "ShipmentInvoice '114-7827932-2029910' not created.");
                          _logger.Verbose("ShipmentInvoice found: {Exists}", invoiceExists);

                          _logger.Verbose("Checking for ShipmentInvoiceDetails count > 2 for InvoiceNo '114-7827932-2029910'");
                          int detailCount = ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == "114-7827932-2029910");
                          Assert.That(detailCount > 2, Is.True, $"Expected > 2 ShipmentInvoiceDetails, but found {detailCount}.");
                          _logger.Verbose("ShipmentInvoiceDetails count: {Count}", detailCount);

                          _logger.Information("Import successful for FileType {FileTypeId}. Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                             fileType.Id, ctx.ShipmentInvoice.Count(), ctx.ShipmentInvoiceDetails.Count());
                     }
                }

                Assert.That(true);
            }
            catch (Exception e)
            {
                 _logger.Error(e, "ERROR in CanImportAmazonMultiSectionInvoice");
                 Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

       

        [Test]
        public async Task CanImportSheinMultiInvoice()
        {
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\Shein - multiple invoices for one shipment .pdf";
                 _logger.Information("Test File: {FilePath}", testFile);

                if (!File.Exists(testFile))
                {
                     _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                     Assert.Warn($"Test file not found: {testFile}");
                     return;
                }

                 _logger.Debug("Getting importable file types for PDF.");
                 var fileLst = await FileTypeManager // Removed .Instance
                     .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF,
                         testFile).ConfigureAwait(false);
                 var fileTypes = fileLst
                     .OfType<CoreEntities.Business.Entities.FileTypes>() // Ensure correct type
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
                    await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false); // Removed .Instance
                     _logger.Debug("PDFUtils.ImportPDF completed for FileType ID: {FileTypeId}", fileType.Id);


                     _logger.Debug("Verifying import results in database...");
                     using (var ctx = new EntryDataDSContext())
                     {
                          _logger.Verbose("Checking if any ShipmentInvoice exists.");
                          bool invoiceExists = ctx.ShipmentInvoice.Any();
                          Assert.That(invoiceExists, Is.True, "No ShipmentInvoice created.");
                          _logger.Verbose("ShipmentInvoice exists: {Exists}", invoiceExists);

                          _logger.Verbose("Checking if any ShipmentInvoiceDetails exists.");
                          bool detailsExist = ctx.ShipmentInvoiceDetails.Any();
                          Assert.That(detailsExist, Is.True, "No ShipmentInvoiceDetails created.");
                          _logger.Verbose("ShipmentInvoiceDetails exist: {Exists}", detailsExist);

                          _logger.Information("Import successful for FileType {FileTypeId}. Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                             fileType.Id, ctx.ShipmentInvoice.Count(), ctx.ShipmentInvoiceDetails.Count());
                     }
                }

                Assert.That(true);
            }
            catch (Exception e)
            {
                 _logger.Error(e, "ERROR in CanImportSheinMultiInvoice");
                 Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }

    } // End Class
} // End Namespace