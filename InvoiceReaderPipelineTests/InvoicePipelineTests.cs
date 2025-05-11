using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities; // Assuming ShipmentInvoice, ShipmentInvoiceDetails, EntryDataDSContext are here
using Serilog;
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Assuming InvoiceReader is here
using NUnit.Framework;
using CoreEntities.Business.Entities;
using FileTypes = CoreEntities.Business.Entities.FileTypes; // Assuming FileTypes is here

namespace InvoiceReaderPipelineTests
{
    public class InvoicePipelineTests
    {

        [Test]
        public async Task CanImportAmazonMultiSectionInvoice_WithLogging()
        {
            var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\one amazon with muliple invoice details sections.pdf";
            var expectedInvoiceNo = "114-7827932-2029910";
            Func<int, bool> expectedDetailCountAssertion = count => count == 8;
            string assertionDescription = "equal to 8";

            await RunImportAndVerificationTest(testFile, expectedInvoiceNo, expectedDetailCountAssertion, assertionDescription).ConfigureAwait(false);
        }

        [Test]
        public async Task CanImportSheinMultiSectionInvoice_WithLogging()
        {
            var testFile = @"D:\OneDrive\Clients\WebSource\Emails\Downloads\Test cases\Shein - multiple invoices for one shipment .pdf";
            var expectedInvoiceNo = "INVUS 20240 7060027 86 676";
            Func<int, bool> expectedDetailCountAssertion = count => count == 1;
            string assertionDescription = "equal to 1";

            await RunImportAndVerificationTest(testFile, expectedInvoiceNo, expectedDetailCountAssertion, assertionDescription).ConfigureAwait(false);
        }

        [Test]
        public async Task CanImportCalaMarineInvoice()
        {
            var testFile = @"C:\Users\josep\OneDrive\Clients\AutoBrokerage\Emails\Downloads\Test Cases\24967.pdf";
            var expectedInvoiceNo = "24967";
            Func<int, bool> expectedDetailCountAssertion = count => count == 1;
            string assertionDescription = "exactly 1";

            await RunImportAndVerificationTest(testFile, expectedInvoiceNo, expectedDetailCountAssertion, assertionDescription).ConfigureAwait(false);
        }
        
        
        
        private void LogTestStart()
        {
            _logger.Information("=== Starting Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
        }
        private static Serilog.ILogger _logger;

        // ========================================================================
        // Fixture Setup & Teardown (Unchanged from previous refactoring)
        // ========================================================================

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            ConfigureSerilog();
            LogFixtureStart();
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            LogFixtureEnd();
            FlushLogs();
        }

        private static void ConfigureSerilog()
        {
            try
            {
                string logFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Logs", "AutoBotTests-.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Ensure log directory exists

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("InvoiceReaderPipelineTests", Serilog.Events.LogEventLevel.Verbose)
                    .MinimumLevel.Override("WaterNut.DataSpace.PipelineInfrastructure.PipelineRunner", Serilog.Events.LogEventLevel.Verbose)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}")
                    .WriteTo.File(logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 3,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                _logger = Log.ForContext<InvoicePipelineTests>();
                _logger.Information("Serilog configured programmatically for tests.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR configuring Serilog programmatically: {ex}");
                Log.Logger = new LoggerConfiguration() // Fallback
                    .MinimumLevel.Warning()
                    .WriteTo.Console()
                    .CreateLogger();
                _logger = Log.ForContext<InvoicePipelineTests>();
                _logger.Error(ex, "Error configuring Serilog programmatically.");
            }
        }

        private static void LogFixtureStart()
        {
            _logger.Information("--------------------------------------------------");
            _logger.Information("Starting PDFImportTests Test Fixture");
            _logger.Information("--------------------------------------------------");
        }

        private static void LogFixtureEnd()
        {
            _logger.Information("--------------------------------------------------");
            _logger.Information("Finished PDFImportTests Test Fixture");
            _logger.Information("--------------------------------------------------");
        }

        private static void FlushLogs()
        {
            Log.CloseAndFlush();
        }

        // ========================================================================
        // Test Setup & Teardown (Unchanged from previous refactoring)
        // ========================================================================

        [SetUp]
        public void SetUp()
        {
            LogTestStart();
            PrepareTestEnvironment();
        }

        [TearDown]
        public void TearDown()
        {
            LogTestEnd();
        }



        private void PrepareTestEnvironment()
        {
            _logger.Debug("Preparing test environment: Applying settings and clearing database.");
            try
            {
                ApplyApplicationSettings(); // Apply test application settings
                ClearTestDatabaseTables();  // Explicitly clear tables
                _logger.Debug("Test environment preparation complete.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during test environment preparation (SetTestApplicationSettings or ClearDataBase).");
                Assert.Fail($"Test setup failed: {ex.Message}");
            }
        }

        private void ApplyApplicationSettings()
        {
            // Placeholder for applying test settings if needed
            _logger.Debug("Applying test application settings (e.g., setting ID 3).");
            // Example: ApplicationSettings.SetTestSettings(3);
            _logger.Debug("Test application settings applied.");
        }

        private void ClearTestDatabaseTables()
        {
            _logger.Debug("Explicitly clearing ShipmentInvoice and ShipmentInvoiceDetails tables.");
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    // Clear details first due to potential foreign key constraints
                    ctx.Database.ExecuteSqlCommand("DELETE FROM ShipmentInvoiceDetails");
                    _logger.Debug("ShipmentInvoiceDetails table cleared.");
                    ctx.Database.ExecuteSqlCommand("DELETE FROM ShipmentInvoice");
                    _logger.Debug("ShipmentInvoice table cleared.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to clear database tables.");
                throw; // Re-throw to fail the setup
            }
        }

        private void LogTestEnd()
        {
            _logger.Information("=== Finished Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
        }

        // ========================================================================
        // Test Methods (Unchanged from previous refactoring)
        // ========================================================================



        // ========================================================================
        // Reusable Test Logic Workflow (Unchanged from previous refactoring)
        // ========================================================================

        private async Task RunImportAndVerificationTest(string testFilePath, string expectedInvoiceNo, Func<int, bool> detailCountAssertion, string assertionDescription)
        {
            LogTestExecutionStart(TestContext.CurrentContext.Test.Name);
            Console.SetOut(TestContext.Progress);

            try
            {
                if (!CheckTestFileExists(testFilePath)) return;

                var fileTypes = await GetImportableFileTypes(testFilePath).ConfigureAwait(false);
                if (!CheckFileTypesFound(fileTypes, testFilePath)) return;

                foreach (var fileType in fileTypes)
                {
                    await ProcessAndVerifyImportForFileType(fileType, testFilePath, expectedInvoiceNo, detailCountAssertion, assertionDescription).ConfigureAwait(false);
                }

                LogTestExecutionSuccess(TestContext.CurrentContext.Test.Name);
                Assert.That(true);
            }
            catch (Exception e)
            {
                HandleTestExecutionError(e, TestContext.CurrentContext.Test.Name);
            }
            finally
            {
                LogTestExecutionEnd(TestContext.CurrentContext.Test.Name);
            }
        }

        // ========================================================================
        // Helper Methods for Test Workflow Steps (Refined GetImportableFileTypes)
        // ========================================================================

        private void LogTestExecutionStart(string testName)
        {
            _logger.Information("Starting Test Logic: {TestName}", testName);
        }

        private void LogTestExecutionEnd(string testName)
        {
            _logger.Information("Finished Test Logic: {TestName}", testName);
        }

        private bool CheckTestFileExists(string testFile)
        {
            _logger.Information("Test File: {FilePath}", testFile);
            _logger.Debug("Checking if test file exists at: {FilePath}", testFile);
            if (!File.Exists(testFile))
            {
                _logger.Warning("Test file not found: {FilePath}. Skipping test.", testFile);
                Assert.Warn($"Test file not found: {testFile}");
                return false;
            }
            _logger.Debug("Test file found at: {FilePath}", testFile);
            return true;
        }

        /// <summary>
        /// Gets suitable importable file types, orchestrating fetch and filter steps.
        /// </summary>
        private async Task<List<FileTypes>> GetImportableFileTypes(string testFile)
        {
            _logger.Debug("Getting importable file types for PDF: {FilePath}", testFile);
            var rawFileTypes = await FetchRawFileTypes(testFile).ConfigureAwait(false);
            var filteredFileTypes = FilterAndConvertFileTypes(rawFileTypes);
            return filteredFileTypes;
        }

        /// <summary>
        /// Fetches raw file types from the FileTypeManager.
        /// </summary>
        private async Task<IEnumerable<object>> FetchRawFileTypes(string testFile) // Return type based on original code's usage
        {
            _logger.Debug("Calling FileTypeManager.GetImportableFileType with EntryType: {EntryType}, FileFormat: {FileFormat}, FilePath: {FilePath}",
                FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile);

            // Assuming FileTypeManager is static and thread-safe for tests
            var rawFileTypes =await FileTypeManager
                .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile).ConfigureAwait(false);

            _logger.Debug("FileTypeManager.GetImportableFileType returned {Count} raw file types.", rawFileTypes?.Count() ?? 0); // Added null check
            return rawFileTypes ?? Enumerable.Empty<object>(); // Return empty if null
        }

        /// <summary>
        /// Filters the raw file types to the desired criteria (Unknown PDF types).
        /// </summary>
        private List<CoreEntities.Business.Entities.FileTypes> FilterAndConvertFileTypes(IEnumerable<object> rawFileTypes)
        {
            _logger.Debug("Filtering raw file types to 'Unknown' PDF CoreEntities.Business.Entities.FileTypes.");
            var fileTypes = rawFileTypes
                .OfType<CoreEntities.Business.Entities.FileTypes>() // Ensure correct type
                .Where(x => x.Description == "Unknown") // Specific filter used in original code
                .ToList();

            _logger.Debug("Filtered to {Count} 'Unknown' PDF file types.", fileTypes.Count);
            return fileTypes;
        }

        private bool CheckFileTypesFound(List<CoreEntities.Business.Entities.FileTypes> fileTypes, string testFile)
        {
            if (!fileTypes.Any())
            {
                _logger.Warning("No suitable 'Unknown' PDF FileType found after filtering for: {FilePath}. Skipping test.", testFile);
                Assert.Warn($"No suitable 'Unknown' PDF FileType found after filtering for: {testFile}");
                return false;
            }
            _logger.Debug("Suitable file types found for processing.");
            return true;
        }

        private async Task ProcessAndVerifyImportForFileType(CoreEntities.Business.Entities.FileTypes fileType, string testFilePath, string expectedInvoiceNo, Func<int, bool> detailCountAssertion, string assertionDescription)
        {
            _logger.Information("Processing with FileType: {FileTypeDescription} (ID: {FileTypeId})", fileType.Description, fileType.Id);
            await ImportFile(fileType, testFilePath).ConfigureAwait(false);
            VerifyDatabaseState(fileType, expectedInvoiceNo, detailCountAssertion, assertionDescription);
        }

        private async Task ImportFile(CoreEntities.Business.Entities.FileTypes fileType, string testFilePath)
        {
            _logger.Information("Attempting import via InvoiceReader.InvoiceReader.ImportPDF with file: {FilePath} and FileType ID: {FileTypeId}", testFilePath, fileType.Id);
            try
            {
                // Assuming InvoiceReader is static and thread-safe for tests
                await InvoiceReader.InvoiceReader.ImportPDF(new[] { new FileInfo(testFilePath) }, fileType).ConfigureAwait(false);
                _logger.Information("InvoiceReader.InvoiceReader.ImportPDF completed successfully for FileType ID: {FileTypeId}", fileType.Id);
            }
            catch (Exception importEx)
            {
                _logger.Error(importEx, "Error during InvoiceReader.InvoiceReader.ImportPDF for FileType ID: {FileTypeId}", fileType.Id);
                throw; // Re-throw to fail the test if import fails
            }
        }

        private void VerifyDatabaseState(CoreEntities.Business.Entities.FileTypes fileType, string expectedInvoiceNo, Func<int, bool> detailCountAssertion, string assertionDescription)
        {
            _logger.Information("Verifying database state after import for FileType ID: {FileTypeId}...", fileType.Id);
            try
            {
                using (var ctx = CreateDbContext()) // Encapsulated context creation
                {
                    _logger.Information("--- Database Verification Start ---");
                    _logger.Debug("Target InvoiceNo: '{InvoiceNo}'", expectedInvoiceNo);

                    VerifyInvoiceExistence(ctx, expectedInvoiceNo);
                    VerifyDetailCount(ctx, expectedInvoiceNo, detailCountAssertion, assertionDescription);
                    LogOverallDbState(ctx, fileType.Id);

                    _logger.Information("--- Database Verification End ---");
                } // Context disposed automatically
            }
            catch (Exception dbEx)
            {
                _logger.Error(dbEx, "Error during database verification for FileType ID: {FileTypeId}", fileType.Id);
                throw; // Re-throw to fail the test if DB check fails
            }
        }

        private EntryDataDSContext CreateDbContext()
        {
            _logger.Debug("Creating database context (EntryDataDSContext)...");
            // Add error handling if context creation can fail
            var ctx = new EntryDataDSContext();
            _logger.Debug("Database context created successfully.");
            return ctx;
        }


        private void VerifyInvoiceExistence(EntryDataDSContext ctx, string expectedInvoiceNo)
        {
            _logger.Debug("Querying: Checking existence of ShipmentInvoice with InvoiceNo '{InvoiceNo}'.", expectedInvoiceNo);
            bool invoiceExists = false;
            try
            {
                invoiceExists = ctx.ShipmentInvoice.Any(x => x.InvoiceNo == expectedInvoiceNo);
                _logger.Information("Result: ShipmentInvoice '{InvoiceNo}' exists = {Exists}", expectedInvoiceNo, invoiceExists);
            }
            catch (Exception queryEx)
            {
                _logger.Error(queryEx, "Query failed while checking for ShipmentInvoice '{InvoiceNo}' existence.", expectedInvoiceNo);
                throw;
            }
            _logger.Debug("Asserting: ShipmentInvoice '{InvoiceNo}' exists.", expectedInvoiceNo);
            Assert.That(invoiceExists, Is.True, $"ASSERT FAILED: ShipmentInvoice '{expectedInvoiceNo}' was not found.");
            _logger.Debug("Assertion passed.");
        }

        private void VerifyDetailCount(EntryDataDSContext ctx, string expectedInvoiceNo, Func<int, bool> detailCountAssertion, string assertionDescription)
        {
            _logger.Debug("Querying: Counting ShipmentInvoiceDetails for InvoiceNo '{InvoiceNo}'.", expectedInvoiceNo);
            int detailCount = -1;
            try
            {
                detailCount = ctx.ShipmentInvoiceDetails.Count(x => x.Invoice.InvoiceNo == expectedInvoiceNo);
                _logger.Information("Result: Found {Count} ShipmentInvoiceDetails for InvoiceNo '{InvoiceNo}'.", detailCount, expectedInvoiceNo);
            }
            catch (Exception queryEx)
            {
                _logger.Error(queryEx, "Query failed while counting ShipmentInvoiceDetails for InvoiceNo '{InvoiceNo}'.", expectedInvoiceNo);
                throw;
            }

            _logger.Debug("Asserting: Detail count ({Count}) satisfies condition '{AssertionDescription}' for InvoiceNo '{InvoiceNo}'.", detailCount, assertionDescription, expectedInvoiceNo);
            Assert.That(detailCountAssertion(detailCount), Is.True, $"ASSERT FAILED: Expected {assertionDescription} ShipmentInvoiceDetails for InvoiceNo '{expectedInvoiceNo}', but found {detailCount}.");
            _logger.Debug("Assertion passed.");
        }

        private void LogOverallDbState(EntryDataDSContext ctx, int fileTypeId)
        {
            try
            {
                _logger.Debug("Querying: Counting total ShipmentInvoices.");
                int totalInvoices = ctx.ShipmentInvoice.Count();
                _logger.Debug("Querying: Counting total ShipmentInvoiceDetails.");
                int totalDetails = ctx.ShipmentInvoiceDetails.Count();

                _logger.Information("Overall DB state for FileType {FileTypeId} verification - Total Invoices: {InvoiceCount}, Total Details: {DetailCount}",
                   fileTypeId, totalInvoices, totalDetails);
            }
            catch (Exception queryEx)
            {
                _logger.Error(queryEx, "Query failed while getting overall DB counts.");
                // Decide if this failure should fail the test or just be logged
            }
        }

        private void LogTestExecutionSuccess(string testName)
        {
            _logger.Information("Test Logic {TestName} completed successfully.", testName);
        }

        private void HandleTestExecutionError(Exception e, string testName)
        {
            _logger.Error(e, "ERROR in Test Logic {TestName}", testName);
            Assert.Fail($"Test {testName} failed with exception: {e.Message}");
        }

        // ========================================================================
        // End of Class
        // ========================================================================
    }
}