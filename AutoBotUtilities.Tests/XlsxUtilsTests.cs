using System.IO;
using AdjustmentQS.Business.Entities;
using EntryDataDS.Business.Entities;
using OCR.Business.Services;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    using AutoBot;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class XlsxUtilsTests
    {
       

        [SetUp]
        public void SetUp()
        {
             // Changed AppSetting ID from 2 to 3, as ID 3 is known to exist from FolderProcessorTests
             Infrastructure.Utils.SetTestApplicationSettings(3);
             SetupTest();
        }

        private static void SetupTest()
        {
            Infrastructure.Utils.ClearDataBase();

        }

       

        [Test]
        public void CanConstruct()
        {
            var instance = new CSVUtils();
            Assert.That(instance, Is.Not.Null);
        }


        [Test]
        public void CanImportXSLXPOFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                // Construct path relative to the test assembly directory
                string assemblyDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                // Go up 3 levels to the project directory, then into Test Data/HAWB9595443
                string testFilePath = Path.GetFullPath(Path.Combine(assemblyDir, "..", "..", "..", "Test Data", "HAWB9595443", "111-8019845-2302666.xlsx"));
                Assert.That(File.Exists(testFilePath), Is.True, $"Test XLSX file not found at: {testFilePath}");
                var testFile = testFilePath; // Use the constructed path
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Xlsx, testFile);
                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFile);

                }

                using (var ctx = new EntryDataDSContext())
                {
                    // Get the actual counts after the import
                    var actualEntryDataCount = ctx.EntryData.Count();
                    var actualEntryDataDetailsCount = ctx.EntryDataDetails.Count();

                    Console.WriteLine($"Actual Counts - EntryData: {actualEntryDataCount}, EntryDataDetails: {actualEntryDataDetailsCount}");

                    // Assert that some data was imported (counts > 0)
                    // You can make these more specific if you know the exact expected counts
                    // after manually verifying the import of '111-8019845-2302666.xlsx'
                    Assert.Multiple(() =>
                    {
                        Assert.That(actualEntryDataCount, Is.GreaterThan(0), "Expected at least one EntryData record to be created.");
                        Assert.That(actualEntryDataDetailsCount, Is.GreaterThan(0), "Expected at least one EntryDataDetail record to be created.");
                        // Optionally, add more specific count assertions here if known:
                        // Assert.That(actualEntryDataCount, Is.EqualTo(EXPECTED_COUNT), $"Expected exactly {EXPECTED_COUNT} EntryData records.");
                        // Assert.That(actualEntryDataDetailsCount, Is.EqualTo(EXPECTED_DETAILS_COUNT), $"Expected exactly {EXPECTED_DETAILS_COUNT} EntryDataDetail records.");
                    });
                }

                //Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        public void CanImportXSLXSalesFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "Monthly Customs Sales Report as at May 2022.xlsx" });
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Sales, FileTypeManager.FileFormats.Xlsx, testFile);
                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFile);

                }

                using (var ctx = new EntryDataDSContext())
                {
                    Assert.Multiple(() =>
                    {
                        Assert.Equals(ctx.EntryData.Count(), 2213);
                        Assert.Equals(ctx.EntryDataDetails.Count(), 5703);
                    });
                }

                //Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }
        [Test]
        public void CanImportXSLXDiscrepancyFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "INTERNATIONAL PAINT DISCREPANCY- Customs.xlsx" });
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Dis, FileTypeManager.FileFormats.Xlsx, testFile);
                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFile);

                }

                using (var ctx = new EntryDataDSContext())
                {
                    Assert.Multiple(() =>
                    {
                        Assert.Equals(ctx.EntryData.Count(), 1);
                        Assert.Equals(ctx.EntryDataDetails.Count(), 1);
                    });
                }

                //Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }
        [Test]
        public void CanImportXSLXUnknownFile_Discrepancy()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "INTERNATIONAL PAINT DISCREPANCY- Customs.xlsx" });
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.Xlsx, testFile);
                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFile);

                }

                using (var ctx = new EntryDataDSContext())
                {
                    Assert.Multiple(() =>
                    {
                        Assert.Equals(1, ctx.EntryData.Count());
                        Assert.Equals(1, ctx.EntryDataDetails.Count());
                    });
                }

                //Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        public void CanImportXSLXUnknownFile_Sales()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "July-Sept 2020.xlsx" });
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Sales, FileTypeManager.FileFormats.Xlsx, testFile);
                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFile);

                }

                using (var ctx = new EntryDataDSContext())
                {
                    Assert.Multiple(() =>
                    {
                        Assert.Equals(6961, ctx.EntryData.Where(x => x.SourceFile == testFile).Count());
                        Assert.Equals(17958, ctx.EntryDataDetails.Count(x => x.EntryData.SourceFile == testFile));
                    });
                }

                //Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

    }

   
}