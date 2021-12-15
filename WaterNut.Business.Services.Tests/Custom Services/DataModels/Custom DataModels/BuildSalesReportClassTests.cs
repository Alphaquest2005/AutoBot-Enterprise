namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using AllocationDS.Business.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [TestFixture]
    public class BuildSalesReportClassTests
    {
        private BuildSalesReportClass _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new BuildSalesReportClass();
        }

        [Test]
        public void CanCallBuildSalesReport()
        {
            _testClass.BuildSalesReport();
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallReBuildSalesReportsWithId()
        {
            var id = "TestValue1048205798";
            await _testClass.ReBuildSalesReports(id);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallReBuildSalesReportsWithIdWithInvalidId(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ReBuildSalesReports(value));
        }

        [Test]
        public async Task CanCallReBuildSalesReportsWithDoc()
        {
            var doc = new AsycudaDocument();
            await _testClass.ReBuildSalesReports(doc);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallReBuildSalesReportsWithDocWithNullDoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ReBuildSalesReports(default(AsycudaDocument)));
        }

        [Test]
        public void CanCallReBuildSalesReportsWithNoParameters()
        {
            _testClass.ReBuildSalesReports();
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallReLinkPi2Item()
        {
            var piLst = new List<xcuda_PreviousItem>();
            await _testClass.ReLinkPi2Item(piLst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallReLinkPi2ItemWithNullPiLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ReLinkPi2Item(default(List<xcuda_PreviousItem>)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(BuildSalesReportClass.Instance, Is.InstanceOf<BuildSalesReportClass>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInitialization()
        {
            Assert.That(BuildSalesReportClass.Initialization, Is.InstanceOf<Task>());
            Assert.Fail("Create or modify test");
        }
    }
}