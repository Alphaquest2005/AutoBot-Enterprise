namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using DocumentDS.Business.Entities;
    using WaterNut.Business.Entities;
    using System.Collections.Generic;
    using DocumentItemDS.Business.Entities;
    using EntryDataDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class CreateErrOPSTests
    {
        private CreateErrOPS _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CreateErrOPS();
        }

        [Test]
        public async Task CanCallCreateErrorOPS()
        {
            var filterExpression = "TestValue2028956163";
            var docSet = new AsycudaDocumentSet();
            await _testClass.CreateErrorOPS(filterExpression, docSet);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateErrorOPSWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateErrorOPS("TestValue1113560189", default(AsycudaDocumentSet)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateErrorOPSWithInvalidFilterExpression(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateErrorOPS(value, new AsycudaDocumentSet()));
        }

        [Test]
        public void CanCallErrOpsIntializeCdoc()
        {
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var ads = new AsycudaDocumentSet();
            _testClass.ErrOpsIntializeCdoc(cdoc, ads);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallErrOpsIntializeCdocWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ErrOpsIntializeCdoc(default(DocumentCT), new AsycudaDocumentSet()));
        }

        [Test]
        public void CannotCallErrOpsIntializeCdocWithNullAds()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ErrOpsIntializeCdoc(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, default(AsycudaDocumentSet)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CreateErrOPS.Instance, Is.InstanceOf<CreateErrOPS>());
            Assert.Fail("Create or modify test");
        }
    }
}