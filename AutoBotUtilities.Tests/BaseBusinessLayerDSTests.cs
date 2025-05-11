using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task CanCallRenameDuplicateDocuments()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var docSet =await EntryDocSetUtils.GetLatestDocSet().ConfigureAwait(false);
                await BaseDataModel.RenameDuplicateDocuments(docSet.AsycudaDocumentSetId).ConfigureAwait(false);
                Assert.That(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }

        }

        [Test]
        public async Task CanCallImportDocuments()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var docSet = await EntryDocSetUtils.GetDocSet("Imports").ConfigureAwait(false);

                var fileNames = new System.Collections.Generic.List<string>()
                {
                    @"D:\OneDrive\Clients\Columbian\Emails\Imports\IM4-GDSGO-17482.xml"
                };

                await Infrastructure.Utils.ImportDocuments(docSet, fileNames).ConfigureAwait(false);

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