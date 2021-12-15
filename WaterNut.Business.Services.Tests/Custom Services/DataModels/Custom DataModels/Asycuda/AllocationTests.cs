namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using Core.Common.Data;
    using System.Collections.Generic;
    using AllocationDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class AllocationsBaseModelTests
    {
        private AllocationsBaseModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new AllocationsBaseModel();
        }

        [Test]
        public async Task CanCallAllocateSales()
        {
            var applicationSettings = new ApplicationSettings();
            var allocateToLastAdjustment = false;
            await _testClass.AllocateSales(applicationSettings, allocateToLastAdjustment);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAllocateSalesWithNullApplicationSettings()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AllocateSales(default(ApplicationSettings), true));
        }

        [Test]
        public void CanCallPrepareDataForAllocation()
        {
            var applicationSettings = new ApplicationSettings();
            AllocationsBaseModel.PrepareDataForAllocation(applicationSettings);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallPrepareDataForAllocationWithNullApplicationSettings()
        {
            Assert.Throws<ArgumentNullException>(() => AllocationsBaseModel.PrepareDataForAllocation(default(ApplicationSettings)));
        }

        [Test]
        public async Task CanCallMarkErrors()
        {
            var applicationSettingsId = 1970005535;
            await _testClass.MarkErrors(applicationSettingsId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallAllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber()
        {
            var applicationSettingsId = 902730285;
            var allocateToLastAdjustment = true;
            var lst = "TestValue1358141234";
            await _testClass.AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(applicationSettingsId, allocateToLastAdjustment, lst);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallAllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumberWithInvalidLst(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(831130616, false, value));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(AllocationsBaseModel.Instance, Is.InstanceOf<AllocationsBaseModel>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetInventoryAliasCache()
        {
            var testValue = new DataCache<InventoryItemAlias>(new[] { new InventoryItemAlias(), new InventoryItemAlias(), new InventoryItemAlias() });
            _testClass.InventoryAliasCache = testValue;
            Assert.That(_testClass.InventoryAliasCache, Is.EqualTo(testValue));
        }
    }
}