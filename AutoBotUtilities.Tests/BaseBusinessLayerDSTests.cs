using System;
using System.Collections.Generic;
using DocumentDS.Business.Entities;
using NUnit.Framework;
using WaterNut.DataSpace;
using EntryDocSetUtils = WaterNut.DataSpace.EntryDocSetUtils;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class BaseDataModelTests
    {
        private BaseDataModel _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(3);
           _testClass = new BaseDataModel();
        }

        [Test]
        public void CanCallRenameDuplicateDocuments()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var docSet = EntryDocSetUtils.GetLatestDocSet();
                BaseDataModel.RenameDuplicateDocuments(docSet.AsycudaDocumentSetId);
                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }

        }

        [Test]
        public void CanCallImportDocuments()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var docSet = EntryDocSetUtils.GetDocSet("Imports");

                var fileNames = new System.Collections.Generic.List<string>()
                {
                    @"D:\OneDrive\Clients\Columbian\Emails\Imports\IM4-GDSGO-17482.xml"
                };

                Infrastructure.Utils.ImportDocuments(docSet, fileNames);

                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }

        }   

  
    }
}