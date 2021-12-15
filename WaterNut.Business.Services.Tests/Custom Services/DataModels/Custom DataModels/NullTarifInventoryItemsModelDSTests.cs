namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class NullTarifInventoryItemsModelTests
    {
        private NullTarifInventoryItemsModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new NullTarifInventoryItemsModel();
        }

        [Test]
        public async Task CanCallAssignTariffToItms()
        {
            var lst = new List<int>();
            var tariffCode = "TestValue162650377";
            await _testClass.AssignTariffToItms(lst, tariffCode);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAssignTariffToItmsWithNullLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AssignTariffToItms(default(List<int>), "TestValue559156324"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallAssignTariffToItmsWithInvalidTariffCode(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AssignTariffToItms(new List<int>(), value));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(NullTarifInventoryItemsModel.Instance, Is.InstanceOf<NullTarifInventoryItemsModel>());
            Assert.Fail("Create or modify test");
        }
    }
}