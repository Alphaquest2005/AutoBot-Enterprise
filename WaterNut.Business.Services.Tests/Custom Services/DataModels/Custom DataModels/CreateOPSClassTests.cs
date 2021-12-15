namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using WaterNut.Business.Entities;
    using DocumentDS.Business.Entities;
    using System.Collections.Generic;
    using DocumentItemDS.Business.Entities;
    using EntryDataDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class CreateOPSClassTests
    {
        private CreateOPSClass _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CreateOPSClass();
        }

        [Test]
        public void CanCallOPSIntializeCdoc()
        {
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var ads = new AsycudaDocumentSet();
            _testClass.OPSIntializeCdoc(cdoc, ads);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallOPSIntializeCdocWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.OPSIntializeCdoc(default(DocumentCT), new AsycudaDocumentSet()));
        }

        [Test]
        public void CannotCallOPSIntializeCdocWithNullAds()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.OPSIntializeCdoc(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, default(AsycudaDocumentSet)));
        }

        [Test]
        public async Task CanCallCreateOPS()
        {
            var filterExpression = "TestValue2058559425";
            var docSet = new AsycudaDocumentSet();
            await _testClass.CreateOPS(filterExpression, docSet);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateOPSWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateOPS("TestValue1521244886", default(AsycudaDocumentSet)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateOPSWithInvalidFilterExpression(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateOPS(value, new AsycudaDocumentSet()));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CreateOPSClass.Instance, Is.InstanceOf<CreateOPSClass>());
            Assert.Fail("Create or modify test");
        }
    }
}