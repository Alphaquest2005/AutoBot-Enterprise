namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using InventoryDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class TariffCodesModelTests
    {
        private TariffCodesModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new TariffCodesModel();
        }

        [Test]
        public async Task CanCallAssignTariffCodeToItm()
        {
            var itm = new InventoryItem();
            var t = new TariffCode();
            await _testClass.AssignTariffCodeToItm(itm, t);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAssignTariffCodeToItmWithNullItm()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AssignTariffCodeToItm(default(InventoryItem), new TariffCode()));
        }

        [Test]
        public void CannotCallAssignTariffCodeToItmWithNullT()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AssignTariffCodeToItm(new InventoryItem(), default(TariffCode)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(TariffCodesModel.Instance, Is.InstanceOf<TariffCodesModel>());
            Assert.Fail("Create or modify test");
        }
    }
}