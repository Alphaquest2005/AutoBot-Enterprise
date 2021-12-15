namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class SaveCsvSubItemsTests
    {
        private SaveCsvSubItems _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SaveCsvSubItems();
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(SaveCsvSubItems.Instance, Is.InstanceOf<SaveCsvSubItems>());
            Assert.Fail("Create or modify test");
        }
    }
}