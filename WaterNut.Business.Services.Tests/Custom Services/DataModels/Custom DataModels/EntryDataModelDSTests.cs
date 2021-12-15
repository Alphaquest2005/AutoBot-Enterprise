namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class EntryDataModelTests
    {
        private EntryDataModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EntryDataModel();
        }

        [Test]
        public async Task CanCallRemoveSelectedEntryData()
        {
            var lst = new[] { "TestValue739567554", "TestValue282669275", "TestValue2055894556" };
            await _testClass.RemoveSelectedEntryData(lst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallRemoveSelectedEntryDataWithNullLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.RemoveSelectedEntryData(default(IEnumerable<string>)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(EntryDataModel.Instance, Is.InstanceOf<EntryDataModel>());
            Assert.Fail("Create or modify test");
        }
    }
}