namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TariffSuppUnitModelTests
    {
        private TariffSuppUnitModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new TariffSuppUnitModel();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new TariffSuppUnitModel();
            Assert.That(instance, Is.Not.Null);
        }
    }
}