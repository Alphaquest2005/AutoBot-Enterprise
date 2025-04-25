using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using Serilog;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using NUnit.Framework;


namespace InvoiceReaderPipelineTests
{
    public class InvoicePipelineTests
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
                    .MinimumLevel.Override("InvoiceReaderPipelineTests", Serilog.Events.LogEventLevel.Verbose) // Ensure test utilities logs are captured
                    .MinimumLevel.Override("WaterNut.DataSpace.PipelineInfrastructure.PipelineRunner", Serilog.Events.LogEventLevel.Verbose) // Explicitly set level for PipelineRunner
                    .Enrich.FromLogContext() // Enrichers
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}") // Console Sink - Added output template
                    .WriteTo.File(logFilePath, // File Sink
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 3,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                _logger = Log.ForContext<InvoicePipelineTests>(); // Get logger instance for this class
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
                _logger = Log.ForContext<InvoicePipelineTests>();
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
                // Apply test application settings
                _logger.Debug("Applying test application settings (3).");
                
                _logger.Debug("Test application settings applied.");

                // Explicitly clear tables for this test fixture
                _logger.Debug("Explicitly clearing ShipmentInvoice and ShipmentInvoiceDetails tables.");
                using (var ctx = new EntryDataDSContext())
                {
                    // Clear details first due to potential foreign key constraints
                    ctx.Database.ExecuteSqlCommand("DELETE FROM ShipmentInvoiceDetails");
                    _logger.Debug("ShipmentInvoiceDetails table cleared.");
                    ctx.Database.ExecuteSqlCommand("DELETE FROM ShipmentInvoice");
                    _logger.Debug("ShipmentInvoice table cleared.");
                }
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

        // New test based on CanImportAmazonMultiSectionInvoice
        [Test]

        public async Task CanImportAmazonMultiSectionInvoice_WithLogging()
        {
            Console.SetOut(TestContext.Progress);
            _logger.Information("Starting CanImportAmazonMultiSectionInvoice_WithLogging test.");
            try
            {
                var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf";
                _logger.Information("Test File: {FilePath}", testFile);

                _logger.Debug("Checking if test file exists at: {FilePath}", testFile);
                if (!File.Exists(testFile))
                {
                    _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                    Assert.Warn($"Test file not found: {testFile}");
                    return;
                }
                else
                {
                    _logger.Debug("Test file found at: {FilePath}", testFile);
                }

                _logger.Debug("Getting importable file types for PDF.");
                // Assuming FileTypeManager is static
                _logger.Debug("Calling FileTypeManager.GetImportableFileType with EntryType: {EntryType}, FileFormat: {FileFormat}, FilePath: {FilePath}",
                    FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);
                _logger.Debug("Getting importable file types for PDF.");
                // Assuming FileTypeManager is static
                _logger.Debug("Calling FileTypeManager.GetImportableFileType with EntryType: {EntryType}, FileFormat: {FileFormat}, FilePath: {FilePath}",
                    FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);
                var rawFileTypes = FileTypeManager // Removed .Instance
                    .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);
                _logger.Debug("FileTypeManager.GetImportableFileType returned {Count} raw file types.", rawFileTypes.Count());
                var fileTypes = rawFileTypes
                    .OfType<CoreEntities.Business.Entities.FileTypes>() // Ensure correct type
                    .Where(x => x.Description == "Unknown")
                    .ToList();
                _logger.Debug("Found {Count} 'Unknown' PDF file types.", fileTypes.Count);

                if (!fileTypes.Any()) // Commented out as it depends on fileTypes
                {
                    _logger.Warning("No suitable 'Unknown' PDF FileType found for: {FilePath}. Skipping test.", testFile);
                    Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                    return;
                }

                foreach (var fileType in fileTypes)
                {
                    _logger.Information("Testing with FileType: {FileTypeDescription} (ID: {FileTypeId})", fileType.Description, fileType.Id);
                    _logger.Debug("Calling InvoiceReader.InvoiceReader.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                    // Assuming PDFUtils is static
                    _logger.Information("Calling InvoiceReader.InvoiceReader.ImportPDF with file: {FilePath} and FileType ID: {FileTypeId}", testFile, fileType.Id);
                    try
                    {
                        await InvoiceReader.InvoiceReader.ImportPDF(new[] {new FileInfo(testFile)}, fileType).ConfigureAwait(false); // Removed .Instance
                        _logger.Information("InvoiceReader.InvoiceReader.ImportPDF completed successfully for FileType ID: {FileTypeId}", fileType.Id);
                    }
                    catch (Exception importEx)
                    {
                        _logger.Error(importEx, "Error during InvoiceReader.InvoiceReader.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                        // Depending on the expected behavior, you might want to re-throw or handle this differently
                        throw; // Re-throw to fail the test if import fails
                    }

                    _logger.Information("Verifying import results in database for FileType ID: {FileTypeId}...", fileType.Id);
                    try
                    {
                        _logger.Debug("Creating database context (EntryDataDSContext)...");
                        using (var ctx = new EntryDataDSContext())
                        {
                            _logger.Debug("Database context created successfully.");

                            _logger.Information("--- Database Verification Start ---");
                            _logger.Debug("Target InvoiceNo: '114-7827932-2029910'");

                            _logger.Debug("Querying database: Checking existence of ShipmentInvoice with InvoiceNo '114-7827932-2029910'.");
                            bool invoiceExists = false;
                            try
                            {
                                invoiceExists = ctx.ShipmentInvoice.Any(x => x.InvoiceNo == "114-7827932-2029910");
                                _logger.Information("Database query result: ShipmentInvoice '114-7827932-2029910' exists = {Exists}", invoiceExists);
                            }
                            catch (Exception queryEx)
                            {
                                _logger.Error(queryEx, "Database query failed while checking for ShipmentInvoice existence.");
                                throw; // Re-throw to fail the test
                            }
                            _logger.Debug("Assertion: Checking if invoiceExists is True.");
                            Assert.That(invoiceExists, Is.True, "ASSERT FAILED: ShipmentInvoice '114-7827932-2029910' was not found in the database after import.");
                            _logger.Debug("Assertion passed: ShipmentInvoice '114-7827932-2029910' exists.");

                            _logger.Debug("Querying database: Counting ShipmentInvoiceDetails for InvoiceNo '114-7827932-2029910'.");
                            int detailCount = -1; // Initialize to indicate query hasn't run or failed
                            try
                            {
                                detailCount = ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == "114-7827932-2029910");
                                _logger.Information("Database query result: Found {Count} ShipmentInvoiceDetails for InvoiceNo '114-7827932-2029910'.", detailCount);
                            }
                            catch (Exception queryEx)
                            {
                                _logger.Error(queryEx, "Database query failed while counting ShipmentInvoiceDetails.");
                                throw; // Re-throw to fail the test
                            }
                            _logger.Debug("Assertion: Checking if detailCount ({Count}) is greater than 2.", detailCount);
                            Assert.That(detailCount > 2, Is.True, $"ASSERT FAILED: Expected more than 2 ShipmentInvoiceDetails for InvoiceNo '114-7827932-2029910', but found {detailCount}.");
                            _logger.Debug("Assertion passed: ShipmentInvoiceDetails count ({Count}) is greater than 2.", detailCount);

                            _logger.Debug("Querying database: Counting total ShipmentInvoices.");
                            int totalInvoices = ctx.ShipmentInvoice.Count();
                            _logger.Debug("Querying database: Counting total ShipmentInvoiceDetails.");
                            int totalDetails = ctx.ShipmentInvoiceDetails.Count();

                            _logger.Information("Import verification successful for FileType {FileTypeId}. Overall DB state - Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                               fileType.Id, totalInvoices, totalDetails);
                            _logger.Information("--- Database Verification End ---");
                        }
                        _logger.Debug("Database context disposed successfully.");
                    }
                    catch (Exception dbEx)
                    {
                        _logger.Error(dbEx, "Error during database verification for FileType ID: {FileTypeId}", fileType.Id);
                        throw; // Re-throw to fail the test if DB check fails
                    }
                }

                _logger.Information("CanImportAmazonMultiSectionInvoice_WithLogging test completed successfully.");
                Assert.That(true);
            }
            catch (Exception e)
            {
                _logger.Error(e, "ERROR in CanImportAmazonMultiSectionInvoice_WithLogging");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
            _logger.Information("Finished CanImportAmazonMultiSectionInvoice_WithLogging test.");
        }


        [Test]
        public async Task CanImportCalaMarineInvoice()
        {
            Console.SetOut(TestContext.Progress);
            _logger.Information("Starting CanImportCalaMarineInvoice test.");
            try
            {
                var testFile = @"C:\Users\josep\OneDrive\Clients\AutoBrokerage\Emails\Downloads\Test Cases\24967.pdf";
                _logger.Information("Test File: {FilePath}", testFile);

                _logger.Debug("Checking if test file exists at: {FilePath}", testFile);
                if (!File.Exists(testFile))
                {
                    _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                    Assert.Warn($"Test file not found: {testFile}");
                    return;
                }
                else
                {
                    _logger.Debug("Test file found at: {FilePath}", testFile);
                }

                _logger.Debug("Getting importable file types for PDF.");
                // Assuming FileTypeManager is static
                _logger.Debug("Calling FileTypeManager.GetImportableFileType with EntryType: {EntryType}, FileFormat: {FileFormat}, FilePath: {FilePath}",
                    FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);
                _logger.Debug("Getting importable file types for PDF.");
                // Assuming FileTypeManager is static
                _logger.Debug("Calling FileTypeManager.GetImportableFileType with EntryType: {EntryType}, FileFormat: {FileFormat}, FilePath: {FilePath}",
                    FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);
                var rawFileTypes = FileTypeManager // Removed .Instance
                    .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);
                _logger.Debug("FileTypeManager.GetImportableFileType returned {Count} raw file types.", rawFileTypes.Count());
                var fileTypes = rawFileTypes
                    .OfType<CoreEntities.Business.Entities.FileTypes>() // Ensure correct type
                    .Where(x => x.Description == "Unknown")
                    .ToList();
                _logger.Debug("Found {Count} 'Unknown' PDF file types.", fileTypes.Count);

                if (!fileTypes.Any()) // Commented out as it depends on fileTypes
                {
                    _logger.Warning("No suitable 'Unknown' PDF FileType found for: {FilePath}. Skipping test.", testFile);
                    Assert.Warn($"No suitable PDF FileType found for: {testFile}");
                    return;
                }

                foreach (var fileType in fileTypes)
                {
                    _logger.Information("Testing with FileType: {FileTypeDescription} (ID: {FileTypeId})", fileType.Description, fileType.Id);
                    _logger.Debug("Calling InvoiceReader.InvoiceReader.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                    // Assuming PDFUtils is static
                    _logger.Information("Calling InvoiceReader.InvoiceReader.ImportPDF with file: {FilePath} and FileType ID: {FileTypeId}", testFile, fileType.Id);
                    try
                    {
                        await InvoiceReader.InvoiceReader.ImportPDF(new[] { new FileInfo(testFile) }, fileType).ConfigureAwait(false); // Removed .Instance
                        _logger.Information("InvoiceReader.InvoiceReader.ImportPDF completed successfully for FileType ID: {FileTypeId}", fileType.Id);
                    }
                    catch (Exception importEx)
                    {
                        _logger.Error(importEx, "Error during InvoiceReader.InvoiceReader.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                        // Depending on the expected behavior, you might want to re-throw or handle this differently
                        throw; // Re-throw to fail the test if import fails
                    }

                    _logger.Information("Verifying import results in database for FileType ID: {FileTypeId}...", fileType.Id);
                    try
                    {
                        _logger.Debug("Creating database context (EntryDataDSContext)...");
                        using (var ctx = new EntryDataDSContext())
                        {
                            _logger.Debug("Database context created successfully.");

                            _logger.Information("--- Database Verification Start ---");
                            _logger.Debug("Target InvoiceNo: '24967'");

                            _logger.Debug("Querying database: Checking existence of ShipmentInvoice with InvoiceNo '24967'.");
                            bool invoiceExists = false;
                            try
                            {
                                invoiceExists = ctx.ShipmentInvoice.Any(x => x.InvoiceNo == "24967");
                                _logger.Information("Database query result: ShipmentInvoice '24967' exists = {Exists}", invoiceExists);
                            }
                            catch (Exception queryEx)
                            {
                                _logger.Error(queryEx, "Database query failed while checking for ShipmentInvoice existence.");
                                throw; // Re-throw to fail the test
                            }
                            _logger.Debug("Assertion: Checking if invoiceExists is True.");
                            Assert.That(invoiceExists, Is.True, "ASSERT FAILED: ShipmentInvoice '24967' was not found in the database after import.");
                            _logger.Debug("Assertion passed: ShipmentInvoice '24967' exists.");

                            _logger.Debug("Querying database: Counting ShipmentInvoiceDetails for InvoiceNo '24967'.");
                            int detailCount = -1; // Initialize to indicate query hasn't run or failed
                            try
                            {
                                detailCount = ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == "24967");
                                _logger.Information("Database query result: Found {Count} ShipmentInvoiceDetails for InvoiceNo '24967'.", detailCount);
                            }
                            catch (Exception queryEx)
                            {
                                _logger.Error(queryEx, "Database query failed while counting ShipmentInvoiceDetails.");
                                throw; // Re-throw to fail the test
                            }
                            _logger.Debug("Assertion: Checking if detailCount ({Count}) is greater than 2.", detailCount);
                            Assert.That(detailCount == 1, Is.True, $"ASSERT FAILED: Expected != 1 ShipmentInvoiceDetails for InvoiceNo '24967', but found {detailCount}.");
                            _logger.Debug("Assertion passed: ShipmentInvoiceDetails count ({Count}) is == 1.", detailCount);

                            _logger.Debug("Querying database: Counting total ShipmentInvoices.");
                            int totalInvoices = ctx.ShipmentInvoice.Count();
                            _logger.Debug("Querying database: Counting total ShipmentInvoiceDetails.");
                            int totalDetails = ctx.ShipmentInvoiceDetails.Count();

                            _logger.Information("Import verification successful for FileType {FileTypeId}. Overall DB state - Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                               fileType.Id, totalInvoices, totalDetails);
                            _logger.Information("--- Database Verification End ---");
                        }
                        _logger.Debug("Database context disposed successfully.");
                    }
                    catch (Exception dbEx)
                    {
                        _logger.Error(dbEx, "Error during database verification for FileType ID: {FileTypeId}", fileType.Id);
                        throw; // Re-throw to fail the test if DB check fails
                    }
                }

                _logger.Information("CanImportAmazonMultiSectionInvoice_WithLogging test completed successfully.");
                Assert.That(true);
            }
            catch (Exception e)
            {
                _logger.Error(e, "ERROR in CanImportAmazonMultiSectionInvoice_WithLogging");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
            _logger.Information("Finished CanImportAmazonMultiSectionInvoice_WithLogging test.");
        }

    }
}