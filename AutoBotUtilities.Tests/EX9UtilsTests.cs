using System.IO;
using Core.Common.Utils;
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
    public class EX9UtilsTests
    {
        private EX9Utils _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(2);
            _testClass = new EX9Utils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EX9Utils();
            Assert.That(instance, Is.Not.Null);
        }



       
        [Test]
        public void CanImportXSalesFile()
        {
            try
            {
                var testFile = Infrastructure.Utils.GetTestSalesFile("TestXSalesFile.csv");
                EX9Utils.ImportXSalesFiles(testFile);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void CanGetXSalesFileType()
        {
            try
            {
                var fileType = EX9Utils.GetxSalesFileType();
                Assert.AreEqual(fileType.FileImporterInfos.EntryType, FileTypeManager.EntryTypes.xSales);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void GetXSalesTestFile()
        {
            try
            {
                var fileType = Infrastructure.Utils.GetTestSalesFile("TestXSalesFile.csv");
                Assert.IsTrue(fileType.Contains("TestXSalesFile.csv"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }





      
    }
}