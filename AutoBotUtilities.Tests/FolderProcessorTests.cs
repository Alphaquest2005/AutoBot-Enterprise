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
        private string _testShipmentInputPath; // Added for shipment folder input
        private string _testArchivePath; // Added for archive check
        private string _sourceTestPdfPath;
        private string _sourceShipmentFolderPath; // Added for sample shipment folder
        private ApplicationSettings _testAppSettings;

        [SetUp]
        public void Setup()
        {
            // 1. Define Paths
            _testRootPath = Path.Combine(TestContext.CurrentContext.TestDirectory, $"TestRun_{Guid.NewGuid()}");
            _testDownloadsPath = Path.Combine(_testRootPath, "Downloads"); // Keep for existing test
            _testDocumentsPath = Path.Combine(_testRootPath, "Documents"); // Keep for existing test
            _testShipmentInputPath = Path.Combine(_testRootPath, "ShipmentInput"); // New input folder
            _testArchivePath = Path.Combine(_testShipmentInputPath, "Archive"); // Expected archive location
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Correct path calculation: Go up from assembly dir to project dir, then to Test Data
            // Correct path calculation: Go up from assembly dir (bin\x64\Debug\net48) to project dir
            // Correct path calculation: Go up from assembly dir (bin\x64\Debug\net48) to project dir
            // Corrected path: Go up 3 levels from assembly dir (e.g., bin/Debug/net48) to the project directory
            _sourceShipmentFolderPath = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "Test Data", "Test Shipment files"));
            _sourceTestPdfPath = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "Test Data", "TestInvoice.pdf"));

            // 2. Create Directories
            Directory.CreateDirectory(_testRootPath);
            Directory.CreateDirectory(_testDownloadsPath);
            Directory.CreateDirectory(_testDocumentsPath);
            Directory.CreateDirectory(_testShipmentInputPath); // Create ShipmentInput folder
            // Archive folder will be created by the processor if needed

            // 3. Copy Test Files
            // Copy single PDF for existing test
            // Remove redundant assignment - _sourceTestPdfPath is correctly defined above
            Console.WriteLine($"Checking for test PDF at: {_sourceTestPdfPath}");
            Assert.That(File.Exists(_sourceTestPdfPath), Is.True, $"Test PDF not found at {_sourceTestPdfPath}");
            File.Copy(_sourceTestPdfPath, Path.Combine(_testDownloadsPath, Path.GetFileName(_sourceTestPdfPath)), true); // Allow overwrite

            // Copy sample shipment folder contents for new test
            Console.WriteLine($"Checking for sample shipment folder at: {_sourceShipmentFolderPath}");
            Assert.That(Directory.Exists(_sourceShipmentFolderPath), Is.True, $"Sample shipment folder not found at {_sourceShipmentFolderPath}");
            var targetShipmentFolder = Path.Combine(_testShipmentInputPath, "SampleShipment1");
            Directory.CreateDirectory(targetShipmentFolder);
            CopyDirectory(_sourceShipmentFolderPath, targetShipmentFolder);
            Console.WriteLine($"Copied sample shipment folder to: {targetShipmentFolder}");

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

        [Test]
        public async Task ProcessShipmentFolders_GeneratesXml_Test()
        {
            // Arrange
            var folderProcessor = new FolderProcessor();
            var inputSubfolderPath = Path.Combine(_testShipmentInputPath, "SampleShipment1");
            var expectedArchiveSubfolderPath = Path.Combine(_testArchivePath, "SampleShipment1");

            // Determine expected XML output path - requires BL number from Info.txt
            var infoFilePath = Path.Combine(inputSubfolderPath, "Info.txt");
            string blNumber = null;
             try
             {
                 var lines = File.ReadAllLines(infoFilePath);
                 foreach (var line in lines)
                 {
                      if (string.IsNullOrWhiteSpace(line)) continue;
                      var parts = line.Split(new[] { ':' }, 2);
                      if (parts.Length == 2 && parts[0].Trim().Equals("BL", StringComparison.OrdinalIgnoreCase))
                      {
                          blNumber = parts[1].Trim();
                          break;
                      }
                 }
                 Assert.That(blNumber, Is.Not.Null.Or.Empty, "Could not extract BL number from sample Info.txt");
             }
             catch (Exception ex)
             {
                 Assert.Fail($"Failed to read BL number from sample Info.txt: {ex.Message}");
             }

            // Assuming GetDocSetDirectoryName uses the Declarant_Reference_Number (which is our BL)
            var expectedXmlDirectory = BaseDataModel.GetDocSetDirectoryName(blNumber);
            var expectedXmlFileName = $"{blNumber}.xml"; // Assuming this naming convention
            var expectedXmlFilePath = Path.Combine(expectedXmlDirectory, expectedXmlFileName);

            Console.WriteLine($"Expected XML Path: {expectedXmlFilePath}");
            // Ensure expected output directory exists for the assertion, although ExportPOEntries should create it
             if (!Directory.Exists(expectedXmlDirectory)) Directory.CreateDirectory(expectedXmlDirectory);
             // Clean up potential previous XML file
             if(File.Exists(expectedXmlFilePath)) File.Delete(expectedXmlFilePath);


            Assert.That(Directory.Exists(inputSubfolderPath), Is.True, "Input shipment subfolder does not exist before test.");
            Assert.That(File.Exists(expectedXmlFilePath), Is.False, "Expected XML file already exists before test.");

            // Act
            await folderProcessor.ProcessShipmentFolders(_testAppSettings);

            // Assert
            Assert.That(Directory.Exists(inputSubfolderPath), Is.False, "Input shipment subfolder was not moved/deleted.");
            // Check archive folder existence (might have timestamp if collision occurred)
            var archiveExists = Directory.Exists(expectedArchiveSubfolderPath) || Directory.GetDirectories(_testArchivePath, "SampleShipment1_*").Any();
            Assert.That(archiveExists, Is.True, "Input shipment subfolder was not moved to Archive.");

            // Corrected Assertion: Check if *any* XML file exists in the expected directory, as filename uses CNumber, not BLNumber.
            var xmlFiles = Directory.GetFiles(expectedXmlDirectory, "*.xml");
            Assert.That(xmlFiles.Any(), Is.True, $"No XML file was generated in the expected directory: {expectedXmlDirectory}");
            Console.WriteLine($"Found generated XML file(s): {string.Join(", ", xmlFiles.Select(Path.GetFileName))}");

            // Optional: Add XML content validation
        }

        // Helper method to copy directory contents
        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}