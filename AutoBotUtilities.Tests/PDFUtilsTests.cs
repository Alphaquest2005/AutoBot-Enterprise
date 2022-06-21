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
    public class PDFUtilsTests
    {
        private PDFUtils _testClass;

        [SetUp]
        public void SetUp()
        {
             Infrastructure.Utils.SetTestApplicationSettings(3);
             SetupTest();


            _testClass = new PDFUtils();
           
        }

        private static void SetupTest()
        {
            
            Infrastructure.Utils.ClearDataBase();
            
        }

       

        [Test]
        public void CanConstruct()
        {
            var instance = new PDFUtils();
            Assert.That(instance, Is.Not.Null);
        }


        
        [Test]
        public void CanImportShipmentInvoice()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "01987.pdf" });
                var fileTypes = (IEnumerable<FileTypes>)FileTypeManager.GetImportableFileType(FileTypeManager.EntryTypes.ShipmentInvoice, FileTypeManager.FileFormats.PDF);
                foreach (var fileType in fileTypes)
                {
                    PDFUtils.ImportPDF(new FileInfo[]{new FileInfo(testFile)}, fileType);


                    using (var ctx = new EntryDataDSContext())
                    {
                        Assert.Multiple(() =>
                        {

                            Assert.AreEqual(ctx.ShipmentInvoice.Count(), 1);
                            Assert.AreEqual(ctx.ShipmentInvoiceDetails.Count(), 8);
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

      

    }

   
}