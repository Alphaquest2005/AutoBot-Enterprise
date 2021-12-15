namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using System.Threading.Tasks;

    [TestFixture]
    public class AsycudaEntrySummaryListModelTests
    {
        private AsycudaEntrySummaryListModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new AsycudaEntrySummaryListModel();
        }

        [Test]
        public async Task CanCallReorderDocumentItems()
        {
            var ASYCUDA_Id = 1517605187;
            await AsycudaEntrySummaryListModel.ReorderDocumentItems(ASYCUDA_Id);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(AsycudaEntrySummaryListModel.Instance, Is.InstanceOf<AsycudaEntrySummaryListModel>());
            Assert.Fail("Create or modify test");
        }
    }
}