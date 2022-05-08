using AutoBot;

namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class BaseDataModelTests
    {
        private BaseDataModel _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings();
            _testClass = new BaseDataModel();
        }

        [Test]
        public void CanCallRenameDuplicateDocuments()
        {
            try
            {
                var docSet = EntryDocSetUtils.GetLatestDocSet();
                BaseDataModel.RenameDuplicateDocuments(docSet.AsycudaDocumentSetId);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }

        }

        [Test]
        public void CanCallImportDocuments()
        {
            try
            {
                var docSet = EntryDocSetUtils.GetDocSet("Imports");

                var fileNames = new System.Collections.Generic.List<string>()
                {
                    @"C:\Users\josep\OneDrive\Clients\Columbian\Emails\Imports\IM4-GDSGO-17482.xml"
                };

                bool importOnlyRegisteredDocument = true;

                bool importTariffCodes = true;

                bool noMessages = false;

                bool overwriteExisting = true;

                bool linkPi = true;

                BaseDataModel.Instance.ImportDocuments(docSet, fileNames, importOnlyRegisteredDocument,
                    importTariffCodes, noMessages, overwriteExisting, linkPi).Wait();

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