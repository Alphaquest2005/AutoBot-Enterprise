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
             Infrastructure.Utils.SetTestApplicationSettings(2);
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
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "02679.xlsx" });
                var fileTypes = FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Xlsx, testFile);
                foreach (var fileType in fileTypes)
                {
                    new FileTypeImporter(fileType).Import(testFile);

                }

                using (var ctx = new EntryDataDSContext())
                {
                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(ctx.EntryData.Count(), 1);
                        Assert.AreEqual(ctx.EntryDataDetails.Count(), 1);
                    });
                }

                //Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void CanImportXSLXSalesFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
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
                        Assert.AreEqual(ctx.EntryData.Count(), 2213);
                        Assert.AreEqual(ctx.EntryDataDetails.Count(), 5703);
                    });
                }

                //Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }
        [Test]
        public void CanImportXSLXDiscrepancyFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
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
                        Assert.AreEqual(ctx.EntryData.Count(), 1);
                        Assert.AreEqual(ctx.EntryDataDetails.Count(), 1);
                    });
                }

                //Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }
        [Test]
        public void CanImportXSLXUnknownFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
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
                        Assert.AreEqual(1, ctx.EntryData.Count());
                        Assert.AreEqual(1, ctx.EntryDataDetails.Count());
                    });
                }

                //Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }



    }

   
}