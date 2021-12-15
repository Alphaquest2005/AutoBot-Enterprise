namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class LicenceSummaryModelTests
    {
        private LicenceSummaryModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new LicenceSummaryModel();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new LicenceSummaryModel();
            Assert.That(instance, Is.Not.Null);
        }
    }
}