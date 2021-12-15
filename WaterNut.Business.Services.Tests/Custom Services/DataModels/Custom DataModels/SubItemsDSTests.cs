namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class SubItemsDSTests
    {
        private SubItemsDS _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SubItemsDS();
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(SubItemsDS.Instance, Is.InstanceOf<SubItemsDS>());
            Assert.Fail("Create or modify test");
        }
    }
}