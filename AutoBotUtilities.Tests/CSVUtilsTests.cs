using System.IO;
using System.Threading.Tasks;
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
    public class CSVUtilsTests
    {
        private CSVUtils _testClass;

        [SetUp]
        public void SetUp()
        {
             Infrastructure.Utils.SetTestApplicationSettings(2);
             SetupTest();


            _testClass = new CSVUtils();
           
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
        public async Task CanImportPOFileOldWay()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>(){ "TestPOCSVFile.csv" });
                var fileTypes = await Infrastructure.Utils.GetPOCSVFileType(testFile).ConfigureAwait(false);
                foreach (var fileType in fileTypes)
                {
                    await CSVUtils.SaveCsv(new List<FileInfo>() { new FileInfo(testFile) }, fileType).ConfigureAwait(false);


                    AssertPOExists();
                }

                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        private static void AssertPOExists()
        {
            using (var ctx = new EntryDataDSContext())
            {
                Assert.Multiple(() =>
                {
                    Assert.Equals(ctx.EntryData.Count(), 1);
                    Assert.Equals(ctx.EntryDataDetails.Count(), 10);
                });
            }
        }

        [Test]
        public async Task CanImportShipmentInvoiceOldWay()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "02679.pdf" });
                var fileTypes = (IEnumerable<FileTypes>)await FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF, testFile).ConfigureAwait(false);
                foreach (var fileType in fileTypes)
                {
                    await PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType).ConfigureAwait(false);


                    using (var ctx = new EntryDataDSContext())
                    {
                        Assert.Multiple(() =>
                        {

                            Assert.Equals(ctx.ShipmentInvoice.Count(), 1);
                            Assert.Equals(ctx.ShipmentInvoiceDetails.Count(), 10);
                        });
                    }


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
        public async Task CanImportPOFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                // Infrastructure.Utils.ImportEntryDataOldWay(new List<string>() { "TestPOCSVFile.csv" }, FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Csv);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "TestPOCSVFile.csv" });
                var fileTypes = await Infrastructure.Utils.GetPOCSVFileType(testFile).ConfigureAwait(false);
                foreach (var fileType in fileTypes)
                {
                    await new FileTypeImporter(fileType).Import(testFile).ConfigureAwait(false);

                }
                AssertPOExists();
               //Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }



        [Test]
        public async Task CanGetUnknownFileType()
        {
            try
            {
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "TestPOCSVFile.csv" });
                var fileTypes = await Infrastructure.Utils.GetUnknownCSVFileType(testFile).ConfigureAwait(false);
                Assert.That(fileTypes.Any());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [Test]
        public async Task CanGetPOFileType()
        {
            try
            {
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "TestPOCSVFile.csv" });
                var fileTypes =await Infrastructure.Utils.GetPOCSVFileType(testFile).ConfigureAwait(false);
                Assert.That(fileTypes.Any());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

    }

   
}