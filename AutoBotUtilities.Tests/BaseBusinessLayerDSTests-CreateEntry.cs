using System;
using System.Collections.Generic;
using System.Data.Entity; // Keep for Include potentially
using System.Linq; // Added for .Any()
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO; // Keep for Path
// using Castle.Components.DictionaryAdapter; // Removed - Path is in System.IO
using WaterNut.Business.Services.Importers; // Keep for FileTypeImporter
using WaterNut.Business.Services.Utils;    // Keep for FileTypeManager
using CoreEntities.Business.Entities; // Keep for AsycudaDocumentSet
using EntryDataDS.Business.Entities; // Keep for EntryData entity
using InventoryDS.Business.Entities; // Keep - might be needed by BaseDataModel
using DocumentDS.Business.Entities; // Keep for AsycudaDocumentSet parameter type
using NUnit.Framework; // Keep for testing attributes and Assert
using WaterNut.DataSpace; // Keep - might be needed by BaseDataModel or contexts
using WaterNut.Business.Services; // Keep - might be needed by BaseDataModel
using EntryDataDS; // Keep for EntryDataDSContext
using Z.EntityFramework.Extensions; // Keep for LicenseManager
// using DocumentDS; // Not needed directly if CoreEntitiesContext has AsycudaDocumentSet

namespace WaterNut.Business.Services.Tests
{
    using Serilog;

    [TestFixture]
    // Renaming class to reflect specific test focus
    public class BaseBusinessLayerDSTests_CreateEntry // Consider renaming file too if this is the only test class
    {
        private BaseDataModel _sut = BaseDataModel.Instance; // Uncommented SUT

        // Test Data
        private List<string> _testEntryDataList = new List<string>(){ "114-7827932-2029910" };
        // Uncommented member variables needed by tests
        private DocumentDS.Business.Entities.AsycudaDocumentSet _testCoreAsycudaDocumentSet;
        

        [OneTimeSetUp]
        public async Task FixtureSetup()
        {
            Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");

            // 1. Define Test File Path (relative to test execution dir)
            string testFileName = "114-7827932-2029910.xlsx";
            // Use TestContext for robust path resolution in test environments
            string testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Test Data", testFileName);
            Assert.That(File.Exists(testFilePath), Is.True, $"Test XLSX file not found at: {testFilePath}");

            // 2. Check if required EntryData exists, import if not
            string targetEntryDataId = _testEntryDataList[0];
            bool entryExists = false;
            try
            {
                using (var ctx = new EntryDataDSContext())
                {
                    // Check using the correct entity and property names
                    // Check using Declarant_Reference_Number based on ID format and original code hints
                    entryExists = ctx.EntryData.Any(e => e.EntryDataId == targetEntryDataId);
                    Console.WriteLine($"DEBUG: Checking for EntryDataID: {targetEntryDataId}. Found: {entryExists}");
                }

                if (!entryExists)
                {
                    Console.WriteLine($"DEBUG: EntryDataID: {targetEntryDataId} not found. Attempting import from {testFilePath}...");
                    // Assuming PO type based on previous examples, adjust if necessary
                    var fileTypes = await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Xlsx, testFilePath).ConfigureAwait(false);
                    if (!fileTypes.Any())
                    {
                         Assert.Fail($"Could not determine importable file type for {testFilePath}");
                    }

                    foreach (var fileType in fileTypes)
                    {
                        // Use ToString() for fileType representation in log
                        Console.WriteLine($"DEBUG: Importing using FileType: {fileType.ToString()}");
                        
                        await new FileTypeImporter(fileType, log).Import(testFilePath, log).ConfigureAwait(false);
                        // Use ToString() for fileType representation in log
                        Console.WriteLine($"DEBUG: Import attempted for EntryDataID: {targetEntryDataId} using FileType: {fileType.ToString()}.");
                    }

                    // Verify import success
                    using (var ctxVerify = new EntryDataDSContext())
                    {
                        // Check using Declarant_Reference_Number based on ID format and original code hints
                        if (!ctxVerify.EntryData.Any(e => e.EntryDataId == targetEntryDataId))
                        {
                            Assert.Fail($"Import verification failed. EntryDataID {targetEntryDataId} still not found after import attempt.");
                        }
                        else
                        {
                             Console.WriteLine($"DEBUG: Confirmed EntryDataID {targetEntryDataId} exists after import.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"DEBUG: EntryDataID: {targetEntryDataId} already exists. Skipping import.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed during data check/import for EntryDataID {targetEntryDataId}: {ex}");
                Assert.Fail($"Failed during data check/import for EntryDataID {targetEntryDataId}: {ex.Message}");
            }

            // 3. Clean up previous test data (Conceptual - implement if needed)
            /*
            using (var ctx = new EntryDataDSContext()) // Use EntryDataDSContext for cleanup
            {
                // Cleanup logic for EntryData/EntryDataDetails specific to this test's side effects
            }
            */

            // Steps 4 & 5 are part of the original setup logic, let's keep them but ensure variables are defined.
            // We need to ensure the check/import happens *before* this logic runs.

            // 4. Instantiate SUT
            _sut = new BaseDataModel(); // Assuming default constructor

            // 5. Query DB using Correct Contexts for Test Data (after ensuring data exists)
            using (var entryCtx = new EntryDataDSContext()) // Use EntryDataDSContext for EntryData
            using (var coreCtx = new DocumentDSContext())   // Use CoreEntitiesContext for AsycudaDocumentSet
            {
                // Find the EntryData record. We need the correct property to identify it.
                // Using SourceFile as per original code for now, but the check above should use the correct ID property.
                var importedEntryData = entryCtx.EntryData // Use entryCtx
                                                .Include(ed => ed.EntryDataDetails)
                                                // Assuming Declarant_Reference_Number is the correct field for the ID "114-..."
                                                // If not, this needs to be adjusted based on the correct property name.
                                                .FirstOrDefault(ed => ed.EntryDataId == targetEntryDataId); // Check using the ID

                Assert.That(importedEntryData, Is.Not.Null, $"Could not find EntryData record with ID {targetEntryDataId} in EntryDataDSContext after check/import.");
                Assert.That(importedEntryData.EntryDataDetails, Is.Not.Empty, "Imported EntryData has no details.");

                // Get the list of keys for the test input
                _testEntryDataList = importedEntryData.EntryDataDetails
                                                     .Select(d => d.EntryDataId)
                                                     .Where(k => !string.IsNullOrEmpty(k))
                                                     .Distinct()
                                                     .ToList();

                Assert.That(_testEntryDataList, Is.Not.Empty, "Could not retrieve EntryDataDetailsKeys from imported data.");

                // Find a suitable AsycudaDocumentSet from CoreEntitiesContext.
                _testCoreAsycudaDocumentSet = coreCtx.AsycudaDocumentSets // Use coreCtx
                                                .Include(ads => ads.Customs_Procedure)
                                                .Include(ads => ads.Customs_Procedure.Document_Type)
                                                .FirstOrDefault(ads => ads.Customs_Procedure != null);

                Assert.That(_testCoreAsycudaDocumentSet, Is.Not.Null, "Could not find a suitable AsycudaDocumentSet in CoreEntitiesContext.");
                Assert.That(_testCoreAsycudaDocumentSet.Customs_Procedure, Is.Not.Null, "Selected AsycudaDocumentSet has no Customs_Procedure.");
                Assert.That(_testCoreAsycudaDocumentSet.Customs_Procedure.Document_Type, Is.Not.Null, "Selected AsycudaDocumentSet.Customs_Procedure has no Document_Type.");


                


                Console.WriteLine($"Setup Complete: Using AsycudaDocumentSetId={_testCoreAsycudaDocumentSet.AsycudaDocumentSetId}, Found {_testEntryDataList.Count} EntryDataDetailsKeys.");
            }
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
             Console.WriteLine("FixtureTearDown: Cleanup (if implemented).");
        }


        // Test Case: All boolean flags false
        [Test]
        public async Task AddToEntry_AllParamsFalse_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = false;
            bool groupItems = false;
            bool checkPackages = false;

            // Act
            Assert.That(_testEntryDataList, Is.Not.Null.And.Not.Empty);
            
            await _sut.AddToEntry(_testEntryDataList, _testCoreAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages).ConfigureAwait(false);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }

        // Test Case: perInvoice = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceTrue_CombineFalse_GroupFalse_CheckFalse_Test()
        {
            // Arrange
            bool perInvoice = true;
            bool combineEntryDataInSameFile = false;
            bool groupItems = false;
            bool checkPackages = false;

            // Act
            Assert.That(_testEntryDataList, Is.Not.Null.And.Not.Empty);
            
            await _sut.AddToEntry(_testEntryDataList, _testCoreAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages).ConfigureAwait(false);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }

        // Test Case: combineEntryDataInSameFile = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceFalse_CombineTrue_GroupFalse_CheckFalse_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = true;
            bool groupItems = false;
            bool checkPackages = false;

            // Act
            Assert.That(_testEntryDataList, Is.Not.Null.And.Not.Empty);
            
            await _sut.AddToEntry(_testEntryDataList, _testCoreAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages).ConfigureAwait(false);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }

        // Test Case: groupItems = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceFalse_CombineFalse_GroupTrue_CheckFalse_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = false;
            bool groupItems = true;
            bool checkPackages = false;

            // Act
            Assert.That(_testEntryDataList, Is.Not.Null.And.Not.Empty);
            
            await _sut.AddToEntry(_testEntryDataList, _testCoreAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages).ConfigureAwait(false);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }

        // Test Case: checkPackages = true, others false
        [Test]
        public async Task AddToEntry_PerInvoiceFalse_CombineFalse_GroupFalse_CheckTrue_Test()
        {
            // Arrange
            bool perInvoice = false;
            bool combineEntryDataInSameFile = false;
            bool groupItems = false;
            bool checkPackages = true;

            // Act
            Assert.That(_testEntryDataList, Is.Not.Null.And.Not.Empty);
            
            await _sut.AddToEntry(_testEntryDataList, _testCoreAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages).ConfigureAwait(false);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }

        // Test Case: All boolean flags true
        [Test]
        public async Task AddToEntry_AllParamsTrue_Test()
        {
            // Arrange
            bool perInvoice = true;
            bool combineEntryDataInSameFile = true;
            bool groupItems = true;
            bool checkPackages = true;

            // Act
            Assert.That(_testEntryDataList, Is.Not.Null.And.Not.Empty);
            
            await _sut.AddToEntry(_testEntryDataList, _testCoreAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages).ConfigureAwait(false);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }


    }
}