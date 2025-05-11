using System.IO;
using System.Threading.Tasks;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
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
            Infrastructure.Utils.ClearDataBase();
            _testClass = new EX9Utils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EX9Utils();
            Assert.That(instance, Is.Not.Null);
        }



       
        [Test]
        public async Task CanImportXSalesFile()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "Sales-TestXSalesFile.csv" });
                await EX9Utils.ImportXSalesFiles(testFile).ConfigureAwait(false);
                using (var ctx = new EntryDataDSContext())
                {
                    Assert.Multiple(() =>
                    {
                        Assert.Equals(ctx.xSalesFiles.Count(), 1);
                        Assert.Equals(ctx.xSalesDetails.Count(), 1);
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        public async Task CanGetXSalesFileType()
        {
            try
            {
                var testFile = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "Sales-TestXSalesFile.csv" });
                var fileType = await EX9Utils.GetxSalesFileType(testFile).ConfigureAwait(false);
                Assert.Equals(fileType.First().FileImporterInfos.EntryType, FileTypeManager.EntryTypes.xSales);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        public void GetXSalesTestFile()
        {
            try
            {
                var fileType = Infrastructure.Utils.GetTestSalesFile(new List<string>() { "TestXSalesFile.csv"});
                Assert.That(fileType.Contains("TestXSalesFile.csv"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }





      
    }
}