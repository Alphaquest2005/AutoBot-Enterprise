namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using System.Threading.Tasks;

    [TestFixture]
    public class SalesReportModelTests
    {
        private SalesReportModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SalesReportModel();
        }

        [Test]
        public async Task CanCallGetSalesDocuments()
        {
            var docSetId = 1116322759;
            var result = await _testClass.GetSalesDocuments(docSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallGetSalesDocument()
        {
            var docId = 1444815270;
            var result = await _testClass.GetSalesDocument(docId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(SalesReportModel.Instance, Is.InstanceOf<SalesReportModel>());
            Assert.Fail("Create or modify test");
        }
    }
}