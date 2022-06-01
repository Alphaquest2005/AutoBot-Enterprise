namespace WaterNut.Business.Services.Tests1.Tests
{
    using WaterNut.DataSpace.Tests;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class BaseDataModelTestTests
    {
        private BaseDataModelTest _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new BaseDataModelTest();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new BaseDataModelTest();
            Assert.That(instance, Is.Not.Null);
        }
    }
}