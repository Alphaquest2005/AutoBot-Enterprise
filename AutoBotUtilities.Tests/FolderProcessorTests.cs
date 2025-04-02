using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection; // Added for Assembly location
using System.Threading.Tasks;
using AutoBot; // Namespace for FolderProcessor
using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums; // Added for ImportStatus
using WaterNut.DataSpace; // For BaseDataModel (assuming static access is needed)
using Core.Common.Utils; // For StringExtensions
using System.Data.Entity; // Added for Include

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class FolderProcessorTests
    {
        private string _testRootPath;
        private string _testDownloadsPath;
        private string _testDocumentsPath;
        private string _sourceTestPdfPath;
        private ApplicationSettings _testAppSettings;

        [SetUp]
        public void Setup()
        {
            // 1. Define Paths
            _testRootPath = Path.Combine(TestContext.CurrentContext.TestDirectory, $"TestRun_{Guid.NewGuid()}");
            _testDownloadsPath = Path.Combine(_testRootPath, "Downloads");
            _testDocumentsPath = Path.Combine(_testRootPath, "Documents");
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _sourceTestPdfPath = Path.Combine(assemblyDir, "Test Data", "TestInvoice.pdf"); // Use assembly location

            // 2. Create Directories
            Directory.CreateDirectory(_testRootPath);
            Directory.CreateDirectory(_testDownloadsPath);
            Directory.CreateDirectory(_testDocumentsPath);
            Directory.CreateDirectory(Path.Combine(_testRootPath, "Imports")); // Create expected Imports folder

            // 3. Copy Test File
            Console.WriteLine($"Checking for test PDF at: {_sourceTestPdfPath}"); // Add logging
            Assert.That(File.Exists(_sourceTestPdfPath), Is.True, $"Test PDF not found at {_sourceTestPdfPath}");
            File.Copy(_sourceTestPdfPath, Path.Combine(_testDownloadsPath, Path.GetFileName(_sourceTestPdfPath)));

            // 4. Load REAL ApplicationSettings from DB
            // WARNING: This uses the actual DB connection from App.config
            int targetApplicationSettingsId = 3; // Changed ID to 3 as requested
            using (var ctx = new CoreEntitiesContext()) // Uses connection string from App.config
            {
                 // Eager load necessary related entities based on Program.cs
                 _testAppSettings = ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes.Select(ft => ft.FileTypeReplaceRegex))
                    .Include(x => x.FileTypes.Select(ft => ft.FileImporterInfos))
                    .Include(x => x.Declarants)
                    .Include(x => x.EmailMapping.Select(em => em.EmailMappingRexExs))
                    .Include(x => x.EmailMapping.Select(em => em.EmailMappingActions.Select(a => a.Actions)))
                    .Include(x => x.EmailMapping.Select(em => em.EmailInfoMappings.Select(im => im.InfoMapping.InfoMappingRegEx)))
                    .Include(x => x.EmailMapping.Select(em => em.EmailFileTypes.Select(eft => eft.FileTypes))) // Include FileTypes linked via EmailMapping
                    .FirstOrDefault(x => x.ApplicationSettingsId == targetApplicationSettingsId && x.IsActive);
            }

            Assert.That(_testAppSettings, Is.Not.Null, $"ApplicationSettingsId {targetApplicationSettingsId} not found or inactive in the database.");

            // Override DataFolder to use the temporary test path
            _testAppSettings.DataFolder = _testRootPath;

             // 5. Set Static Context
             BaseDataModel.Instance.CurrentApplicationSettings = _testAppSettings;
             // Load lookups if needed by the processing logic - Assuming they load automatically or via context.

        }

        [TearDown]
        public void Teardown()
        {
            // Clean up test directories
            if (Directory.Exists(_testRootPath))
            {
                Directory.Delete(_testRootPath, true);
            }
             // Reset static context if necessary
             BaseDataModel.Instance.CurrentApplicationSettings = null;
        }

        [Test]
        public async Task ProcessDownloadFolder_MovesFileToDocuments_WhenProcessedSuccessfully()
        {
            // Arrange
            var folderProcessor = new FolderProcessor();
            var sourceFileName = Path.GetFileName(_sourceTestPdfPath);
            var sourceFilePath = Path.Combine(_testDownloadsPath, sourceFileName);
            var expectedDocsSubfolderName = string.Join("_", sourceFileName.Replace(".pdf", "").Split(Path.GetInvalidFileNameChars()));
            var expectedDocsFolderPath = Path.Combine(_testDocumentsPath, expectedDocsSubfolderName);
            var expectedDestFilePath = Path.Combine(expectedDocsFolderPath, sourceFileName);

            Assert.That(File.Exists(sourceFilePath), Is.True, "Source PDF not found in test downloads folder before test.");
            Assert.That(Directory.Exists(expectedDocsFolderPath), Is.False, "Destination documents subfolder already exists before test.");

            // Act
            await folderProcessor.ProcessDownloadFolder(_testAppSettings);

            // Assert
            // Assuming successful processing leads to deletion of the source file
            Assert.That(File.Exists(sourceFilePath), Is.False, "Source PDF file was not deleted from downloads folder.");
            Assert.That(Directory.Exists(expectedDocsFolderPath), Is.True, "Expected documents subfolder was not created.");
            Assert.That(File.Exists(expectedDestFilePath), Is.True, "PDF file was not copied to the expected documents subfolder.");

            // Add more specific assertions if possible:
            // - Check for generated .txt file if OCR is expected
            // - Check database entries if applicable (requires DB context setup)
        }

        // Add more tests for error conditions, different file types, etc.
    }
}