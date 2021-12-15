namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using CounterPointQS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class CPSalesModelTests
    {
        private CPSalesModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CPSalesModel();
        }

        [Test]
        public async Task CanCallDownloadCPSales()
        {
            var c = new CounterPointSales();
            var docSetId = 85901804;
            await _testClass.DownloadCPSales(c, docSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallDownloadCPSalesWithNullC()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.DownloadCPSales(default(CounterPointSales), 1060061551));
        }

        [Test]
        public async Task CanCallDownloadCPSalesDateRange()
        {
            var startDate = new DateTime(1573245264);
            var endDate = new DateTime(406053299);
            var docSetId = 1347472574;
            await _testClass.DownloadCPSalesDateRange(startDate, endDate, docSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CPSalesModel.Instance, Is.InstanceOf<CPSalesModel>());
            Assert.Fail("Create or modify test");
        }
    }
}