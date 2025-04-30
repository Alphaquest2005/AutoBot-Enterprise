using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Castle.Components.DictionaryAdapter; // Added for Path.Combine
using WaterNut.Business.Services.Importers; // Added for FileTypeImporter
using WaterNut.Business.Services.Utils;    // Added for FileTypeManager
using CoreEntities.Business.Entities; // Core context and entities
using EntryDataDS.Business.Entities; // Needed for EntryData, EntryDataDetails
using InventoryDS.Business.Entities; // Still needed? Check BaseDataModel dependencies if errors occur
using DocumentDS.Business.Entities; // Needed for mapping AsycudaDocumentSet
using NUnit.Framework;
using WaterNut.DataSpace;
using WaterNut.Business.Services;
using EntryDataDS; // Added back for EntryDataDSContext
using Z.EntityFramework.Extensions; // Added for LicenseManager
// using DocumentDS; // Not needed directly if CoreEntitiesContext has AsycudaDocumentSet

namespace WaterNut.Business.Services.Tests
{
    [TestFixture]
    public class BaseBusinessLayerDSTests
    {
        private BaseDataModel _sut; // System Under Test

        // Test Data - Will be populated from DB after import
        private List<string> _testEntryDataList = new EditableList<string>(){};
        private CoreEntities.Business.Entities.AsycudaDocumentSet _testCoreAsycudaDocumentSet; // Use CoreEntities type for fetching
        private DocumentDS.Business.Entities.AsycudaDocumentSet _testAsycudaDocumentSetParam; // Use DocumentDS type for passing to SUT

        [OneTimeSetUp]
        public void FixtureSetup()
        {

            Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");
            // 1. Define Test File Path
            string testFileName = "114-7827932-2029910.xlsx";
            string testFilePath = Path.Combine("TestData", testFileName);
            Assert.That(File.Exists(testFilePath), Is.True, $"Test XLSX file not found at: {testFilePath}");

            // 2. Clean up previous test data (Conceptual - implement if needed)
            /*
            using (var ctx = new EntryDataDSContext()) // Use EntryDataDSContext for cleanup
            {
                // Cleanup logic for EntryData/EntryDataDetails
            }
            */

            // 3. Import Data from XLSX
            try
            {
                // Assuming FileTypeManager/Importer use connection string for the correct DB (likely CoreEntities/WebSource-AutoBot)
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Xlsx, testFilePath);
                Assert.That(fileTypes, Is.Not.Empty, "No suitable import file type found for the test XLSX.");

                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFilePath);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Data import failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }

            // 4. Instantiate SUT
            _sut = new BaseDataModel(); // Assuming default constructor

            // 5. Query DB using Correct Contexts for Test Data
            using (var entryCtx = new EntryDataDSContext()) // Use EntryDataDSContext for EntryData
            using (var coreCtx = new CoreEntitiesContext())   // Use CoreEntitiesContext for AsycudaDocumentSet
            {
                // Find the EntryData record created by the import using SourceFile
                var importedEntryData = entryCtx.EntryData // Use entryCtx
                                                .Include(ed => ed.EntryDataDetails)
                                                .OrderByDescending(ed => ed.EntryData_Id)
                                                .FirstOrDefault(ed => ed.SourceFile == testFilePath); // Compare with full relative path

                Assert.That(importedEntryData, Is.Not.Null, "Could not find EntryData record created by the import in EntryDataDSContext.");
                Assert.That(importedEntryData.EntryDataDetails, Is.Not.Empty, "Imported EntryData has no details.");

                // Get the list of keys for the test input
                _testEntryDataList = importedEntryData.EntryDataDetails
                                                     .Select(d => d.EntryDataDetailsKey)
                                                     .Where(k => !string.IsNullOrEmpty(k))
                                                     .Distinct()
                                                     .ToList();

                Assert.That(_testEntryDataList, Is.Not.Empty, "Could not retrieve EntryDataDetailsKeys from imported data.");

                // Find a suitable AsycudaDocumentSet from CoreEntitiesContext.
                _testCoreAsycudaDocumentSet = coreCtx.AsycudaDocumentSet // Use coreCtx
                                                .Include(ads => ads.Customs_Procedure)
                                                .FirstOrDefault(ads => ads.Customs_Procedure != null);

                Assert.That(_testCoreAsycudaDocumentSet, Is.Not.Null, "Could not find a suitable AsycudaDocumentSet in CoreEntitiesContext.");
                Assert.That(_testCoreAsycudaDocumentSet.Customs_Procedure, Is.Not.Null, "Selected AsycudaDocumentSet has no Customs_Procedure.");

                // Map the CoreEntities.AsycudaDocumentSet to DocumentDS.AsycudaDocumentSet for the SUT parameter
                _testAsycudaDocumentSetParam = MapToDocumentDS(_testCoreAsycudaDocumentSet);
                Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null, "Failed to map AsycudaDocumentSet to DocumentDS type.");


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
            Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null); // Use the mapped param
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSetParam, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

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
            Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null); // Use the mapped param
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSetParam, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

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
            Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null); // Use the mapped param
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSetParam, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

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
            Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null); // Use the mapped param
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSetParam, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

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
            Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null); // Use the mapped param
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSetParam, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

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
            Assert.That(_testAsycudaDocumentSetParam, Is.Not.Null); // Use the mapped param
            await _sut.AddToEntry(_testEntryDataList, _testAsycudaDocumentSetParam, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);

            // Assert
             Assert.Pass("Test executed without exceptions.");
        }

        // Helper method to map CoreEntities.AsycudaDocumentSet to DocumentDS.AsycudaDocumentSet
        private DocumentDS.Business.Entities.AsycudaDocumentSet MapToDocumentDS(CoreEntities.Business.Entities.AsycudaDocumentSet coreSet)
        {
            if (coreSet == null) return null;

            var docDsSet = new DocumentDS.Business.Entities.AsycudaDocumentSet
            {
                AsycudaDocumentSetId = coreSet.AsycudaDocumentSetId,
                Declarant_Reference_Number = coreSet.Declarant_Reference_Number,
                Exchange_Rate = coreSet.Exchange_Rate,
                Description = coreSet.Description,
                Country_of_origin_code = coreSet.Country_of_origin_code,
                Currency_Code = coreSet.Currency_Code,
                EntryTimeStamp = coreSet.EntryTimeStamp,
                // ... map other relevant scalar properties ...

                Customs_Procedure = coreSet.Customs_Procedure == null ? null : new DocumentDS.Business.Entities.Customs_Procedure
                {
                    Customs_ProcedureId = coreSet.Customs_Procedure.Customs_ProcedureId,
                    CustomsProcedure = coreSet.Customs_Procedure.CustomsProcedure,
                    Extended_customs_procedure = coreSet.Customs_Procedure.Extended_customs_procedure,
                    National_customs_procedure = coreSet.Customs_Procedure.National_customs_procedure,
                    CustomsOperationId = coreSet.Customs_Procedure.CustomsOperationId,
                    IsDefault = coreSet.Customs_Procedure.IsDefault,
                    IsObsolete = coreSet.Customs_Procedure.IsObsolete
                    // ... map other relevant Customs_Procedure properties ...
                }
            };

            return docDsSet;
        }
    }
}