namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class Ex9SalesDataDetailsModelTests
    {
        private Ex9SalesDataDetailsModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new Ex9SalesDataDetailsModel();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new Ex9SalesDataDetailsModel();
            Assert.That(instance, Is.Not.Null);
        }
    }
}