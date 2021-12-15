namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using CounterPointQS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class CPPurchaseOrdersModelTests
    {
        private CPPurchaseOrdersModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CPPurchaseOrdersModel();
        }

        [Test]
        public async Task CanCallDownloadCPO()
        {
            var c = new CounterPointPOs();
            var asycudaDocumentSetId = 1361291128;
            await _testClass.DownloadCPO(c, asycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallDownloadCPOWithNullC()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.DownloadCPO(default(CounterPointPOs), 1136453675));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CPPurchaseOrdersModel.Instance, Is.InstanceOf<CPPurchaseOrdersModel>());
            Assert.Fail("Create or modify test");
        }
    }
}