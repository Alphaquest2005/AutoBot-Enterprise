namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class EntryDataDetailsModelTests
    {
        private EntryDataDetailsModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EntryDataDetailsModel();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EntryDataDetailsModel();
            Assert.That(instance, Is.Not.Null);
        }
    }
}