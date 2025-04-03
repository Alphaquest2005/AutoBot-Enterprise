using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoBot; // Namespace for Utils, POUtils, SikuliAutomationService etc.
using CoreEntities.Business.Entities;
using WaterNut.DataSpace; // For BaseDataModel
using System.Data.Entity; // For Include
using System.Collections.Generic; // Added for List<>
using AutoBotUtilities.Tests.Infrastructure; // For test Utils
using WaterNut.Business.Services.Importers; // For FileTypeImporter
using WaterNut.Business.Services.Utils; // For FileTypeManager
using DocumentDS.Business.Entities; // For DocumentDSContext & AsycudaDocumentSet

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    [Category("Integration.Sikuli")] // Add category to optionally exclude GUI tests
    public class SikuliIntegrationTests
    {
        private ApplicationSettings _testAppSettings;
        private string _testDataRootPath; // Specific path for test-generated data if needed

        [SetUp]
        public void Setup()
        {
            // Use Infrastructure Utils to set settings and clear DB
            Infrastructure.Utils.SetTestApplicationSettings(3); // Use AppSetting ID 3
            Infrastructure.Utils.ClearDataBase(); // Ensure clean state

            _testAppSettings = BaseDataModel.Instance.CurrentApplicationSettings; // Get the loaded settings
            Assert.That(_testAppSettings, Is.Not.Null, "Failed to load ApplicationSettings ID 3.");

            // Define a root path for any data this test might generate
            _testDataRootPath = Path.Combine(TestContext.CurrentContext.TestDirectory, $"SikuliTestRun_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDataRootPath);
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up test-generated directories/files
            if (Directory.Exists(_testDataRootPath))
            {
                // Use try-catch for robustness during cleanup
                try { Directory.Delete(_testDataRootPath, true); } catch (Exception ex) { Console.WriteLine($"Teardown cleanup error: {ex.Message}"); }
            }
             // Reset static context
             BaseDataModel.Instance.CurrentApplicationSettings = null;
        }

        [Test]
        public void SaveIM7_Script_ExecutesSuccessfully()
        {
            // Arrange
            Console.WriteLine("Starting SaveIM7 Integration Test...");
            // Confirmed Trigger Method: POUtils.AssessPOEntry(string docReference, int asycudaDocumentSetId)
            // This method calls Utils.RunSiKuLi("SaveIM7", ...) after checking Utils.AssessComplete.
            //
            // Prerequisites:
            // 1. Asycuda application must be running and logged in (Manual Step).
            // 2. Database State: Need an existing AsycudaDocumentSet with a specific ID (`testDocSetId`) and Declarant_Reference_Number (`testDocReference`)
            //    that is marked as needing assessment (e.g., present in `ctx.TODO_PODocSetToExport`).
            // 3. Filesystem State:
            //    - The directory `BaseDataModel.GetDocSetDirectoryName(testDocReference)` must exist.
            //    - The files `Instructions.txt` and `InstructionResults.txt` within that directory need to be set up such that `Utils.AssessComplete` initially returns `false`.
            //      This likely means `InstructionResults.txt` should *not* contain "Success" entries for all relevant lines in `Instructions.txt`.
            //
            // Parameters for POUtils.AssessPOEntry:
            // - docReference (string): The Declarant_Reference_Number of the test document set.
            // - asycudaDocumentSetId (int): The ID of the test document set.

            // 1. Import the PO XLSX to create the necessary DB entries
            Console.WriteLine("Importing PO XLSX file...");
            var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "02679.xlsx" }); // Use the known test PO file
            var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Xlsx, testFile);
            Assert.That(fileTypes.Any(), Is.True, "No PO FileType found for 02679.xlsx");
            var poFileType = fileTypes.First(); // Assuming only one matches

            new FileTypeImporter(poFileType).Import(testFile);
            Console.WriteLine("PO Import completed.");

            // 2. Retrieve the created AsycudaDocumentSet details from DB
            Console.WriteLine("Retrieving AsycudaDocumentSet details...");
            DocumentDS.Business.Entities.AsycudaDocumentSet createdDocSet; // Explicitly specify namespace
            using (var ctx = new DocumentDSContext())
            {
                // Find the docset created by the import (logic might need adjustment based on how importer sets reference)
                // Assuming importer sets reference based on filename or a standard pattern
                string expectedReferencePattern = Path.GetFileNameWithoutExtension(testFile); // Adjust if needed
                createdDocSet = ctx.AsycudaDocumentSets
                                   .OrderByDescending(ds => ds.AsycudaDocumentSetId) // Get the latest one
                                   .FirstOrDefault(ds => ds.Declarant_Reference_Number.Contains(expectedReferencePattern));
            }
            Assert.That(createdDocSet, Is.Not.Null, $"Could not find AsycudaDocumentSet created by importing {testFile}");
            int testDocSetId = createdDocSet.AsycudaDocumentSetId;
            string testDocReference = createdDocSet.Declarant_Reference_Number;
            Console.WriteLine($"Found DocSet: ID={testDocSetId}, Ref={testDocReference}");

            // 3. Prepare filesystem for AssessPOEntry
            string testDirectoryName = BaseDataModel.GetDocSetDirectoryName(testDocReference);
            Directory.CreateDirectory(testDirectoryName); // Ensure directory exists
            string instrFilePath = Path.Combine(testDirectoryName, "Instructions.txt");
            string resultsFilePath = Path.Combine(testDirectoryName, "InstructionResults.txt");

            // Create dummy instruction file (AssessPOEntry needs this structure to know what to assess)
            // It expects XML files generated by ExportPOEntries - let's simulate that
            // We need at least one file listed here that doesn't have a "Success" entry in results
            string dummyXmlFileName = $"{testDocReference}-1.xml"; // Example filename pattern
            File.WriteAllText(instrFilePath, $"File\t{dummyXmlFileName}\r\n");
            File.WriteAllText(resultsFilePath, ""); // Empty results file initially
            Console.WriteLine($"Created dummy instruction files in: {testDirectoryName}");

            bool enableDebugScreenshots = true; // Set to true to test the screenshot feature

            // Act
            Console.WriteLine($"Triggering POUtils.AssessPOEntry for DocSetId: {testDocSetId}, Ref: {testDocReference}...");
            // Call the identified trigger method
            POUtils.AssessPOEntry(testDocReference, testDocSetId);
            Console.WriteLine("POUtils.AssessPOEntry call completed.");

            // Assert
            Console.WriteLine("Performing Assertions...");
            // Verification Logic:
            // 1. Check InstructionResults.txt: It should now contain "Success" entry for the dummy XML file.
            Assert.That(File.Exists(resultsFilePath), Is.True, "InstructionResults.txt was not created or found.");
            string[] resultsLines = File.ReadAllLines(resultsFilePath);
            Assert.That(resultsLines.Any(line => line.Contains($"File\t{dummyXmlFileName}") && line.EndsWith("\tSuccess")), Is.True, $"Success entry for {dummyXmlFileName} not found in results.");
            Console.WriteLine("Verified InstructionResults.txt updated successfully.");

            // 2. Check for Debug Screenshots (if enabled)
            if (enableDebugScreenshots)
            {
                string screenshotDir = Path.Combine(@"D:\SikuliX_Debug_Screenshots", "SaveIM7"); // Match path in debug_utils.py
                Assert.That(Directory.Exists(screenshotDir), Is.True, $"Screenshot directory '{screenshotDir}' does not exist.");
                Assert.That(Directory.EnumerateFiles(screenshotDir, "*.png").Any(), Is.True, "Debug screenshots were not generated in the expected directory.");
                Console.WriteLine($"Debug screenshots found in: {screenshotDir}");
            }

            // 3. (Optional/Advanced) Check Database State: Verify relevant flags/status updated for the testDocSetId or related entries.
            //    using (var ctx = new CoreEntitiesContext()) { ... check DB ... }

            // 4. (Optional/Manual) Check Asycuda GUI for visual confirmation.

            // If we reach here without exceptions and primary checks pass, the test is considered successful.
            Console.WriteLine("SaveIM7 Integration Test checks passed.");
        }

        // Add more tests for C71, AssessC71, LIC, AssessLIC later
    }
}