namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class Ex9SalesDataModelTests
    {
        private Ex9SalesDataModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new Ex9SalesDataModel();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new Ex9SalesDataModel();
            Assert.That(instance, Is.Not.Null);
        }
    }
}