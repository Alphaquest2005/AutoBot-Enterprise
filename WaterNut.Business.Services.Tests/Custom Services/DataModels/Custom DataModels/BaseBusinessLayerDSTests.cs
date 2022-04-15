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
    }
}