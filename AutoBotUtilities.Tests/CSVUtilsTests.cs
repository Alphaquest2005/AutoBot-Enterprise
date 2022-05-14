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
        public void CanImportPOFileOldWay()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>(){ "TestPOCSVFile.csv" });
                var fileTypes = Infrastructure.Utils.GetPOCSVFileType();
                foreach (var fileType in fileTypes)
                {
                    CSVUtils.SaveCsv(new List<FileInfo>(){new FileInfo(testFile)}, fileType);

                    
                        using (var ctx = new EntryDataDSContext())
                        {
                            Assert.Multiple(() =>
                            {

                                Assert.AreEqual(ctx.EntryData.Count(), 1);
                                Assert.AreEqual(ctx.EntryDataDetails.Count(), 10);
                            });
                        }

                    
                }

                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void CanImportPOFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                Infrastructure.Utils.ImportEntryDataOldWay(new List<string>() { "TestPOCSVFile.csv" }, FileTypeManager.EntryTypes.Po, FileTypeManager.FileFormats.Csv);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

       

        [Test]
        public void CanGetUnknownFileType()
        {
            try
            {
                
                var fileTypes = Infrastructure.Utils.GetUnknownCSVFileType();
                Assert.IsTrue(fileTypes.Any());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }


        [Test]
        public void CanGetPOFileType()
        {
            try
            {

                var fileTypes = Infrastructure.Utils.GetPOCSVFileType();
                Assert.IsTrue(fileTypes.Any());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

    }

   
}