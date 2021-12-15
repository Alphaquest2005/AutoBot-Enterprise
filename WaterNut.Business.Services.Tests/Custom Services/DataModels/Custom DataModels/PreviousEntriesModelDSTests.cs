namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class PreviousDocumentItemsModelTests
    {
        private PreviousDocumentItemsModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new PreviousDocumentItemsModel();
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(PreviousDocumentItemsModel.Instance, Is.InstanceOf<PreviousDocumentItemsModel>());
            Assert.Fail("Create or modify test");
        }
    }
}