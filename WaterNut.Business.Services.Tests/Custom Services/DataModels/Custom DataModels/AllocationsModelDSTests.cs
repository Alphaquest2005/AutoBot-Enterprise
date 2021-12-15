namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using WaterNut.Business.Entities;
    using DocumentDS.Business.Entities;
    using System.Collections.Generic;
    using DocumentItemDS.Business.Entities;
    using EntryDataDS.Business.Entities;
    using AllocationDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class AllocationsModelTests
    {
        private AllocationsModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new AllocationsModel();
        }

        [Test]
        public void CanCallAddDutyFreePaidtoRef()
        {
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var dfp = "TestValue375656580";
            var docSet = new AsycudaDocumentSet();
            _testClass.AddDutyFreePaidtoRef(cdoc, dfp, docSet);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAddDutyFreePaidtoRefWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AddDutyFreePaidtoRef(default(DocumentCT), "TestValue1432092077", new AsycudaDocumentSet()));
        }

        [Test]
        public void CannotCallAddDutyFreePaidtoRefWithNullDocSet()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AddDutyFreePaidtoRef(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, "TestValue1161491777", default(AsycudaDocumentSet)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallAddDutyFreePaidtoRefWithInvalidDfp(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AddDutyFreePaidtoRef(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, value, new AsycudaDocumentSet()));
        }

        [Test]
        public async Task CanCallGetAsycudaSalesAllocations()
        {
            var FilterExpression = "TestValue133682881";
            var result = await _testClass.GetAsycudaSalesAllocations(FilterExpression);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetAsycudaSalesAllocationsWithInvalidFilterExpression(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetAsycudaSalesAllocations(value));
        }

        [Test]
        public void CanCallTranslateAllocationWhereExpression()
        {
            var FilterExpression = "TestValue551510146";
            var result = _testClass.TranslateAllocationWhereExpression(FilterExpression);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallTranslateAllocationWhereExpressionWithInvalidFilterExpression(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.TranslateAllocationWhereExpression(value));
        }

        [Test]
        public async Task CanCallManuallyAllocate()
        {
            var currentAsycudaSalesAllocation = new AsycudaSalesAllocations();
            var PreviousItemEx = new xcuda_Item();
            await _testClass.ManuallyAllocate(currentAsycudaSalesAllocation, PreviousItemEx);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallManuallyAllocateWithNullCurrentAsycudaSalesAllocation()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ManuallyAllocate(default(AsycudaSalesAllocations), new xcuda_Item()));
        }

        [Test]
        public void CannotCallManuallyAllocateWithNullPreviousItemEx()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ManuallyAllocate(new AsycudaSalesAllocations(), default(xcuda_Item)));
        }

        [Test]
        public async Task CanCallClearAllocationsWithString()
        {
            var filterExpression = "TestValue1409579751";
            await _testClass.ClearAllocations(filterExpression);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallClearAllocationsWithStringWithInvalidFilterExpression(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ClearAllocations(value));
        }

        [Test]
        public async Task CanCallClearAllAllocations()
        {
            var appSettingsId = 629572054;
            await _testClass.ClearAllAllocations(appSettingsId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallClearDocSetAllocations()
        {
            var lst = "TestValue1218898615";
            await _testClass.ClearDocSetAllocations(lst);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallClearDocSetAllocationsWithInvalidLst(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ClearDocSetAllocations(value));
        }

        [Test]
        public async Task CanCallClearAllocationsWithAlst()
        {
            var alst = new[] { new AsycudaSalesAllocations(), new AsycudaSalesAllocations(), new AsycudaSalesAllocations() };
            await _testClass.ClearAllocations(alst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallClearAllocationsWithAlstWithNullAlst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ClearAllocations(default(IEnumerable<AsycudaSalesAllocations>)));
        }

        [Test]
        public void CanCallSend2Excel()
        {
            var lst = new List<AsycudaSalesAllocations>();
            _testClass.Send2Excel(lst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSend2ExcelWithNullLst()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Send2Excel(default(List<AsycudaSalesAllocations>)));
        }

        [Test]
        public async Task CanCallClearAllocation()
        {
            var allo = new AsycudaSalesAllocations();
            await _testClass.ClearAllocation(allo);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallClearAllocationWithNullAllo()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ClearAllocation(default(AsycudaSalesAllocations)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(AllocationsModel.Instance, Is.InstanceOf<AllocationsModel>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCreateEX9Class()
        {
            Assert.That(_testClass.CreateEX9Class, Is.InstanceOf<CreateEx9Class>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCreateOpsClassClass()
        {
            Assert.That(_testClass.CreateOpsClassClass, Is.InstanceOf<CreateOPSClass>());
            Assert.Fail("Create or modify test");
        }
    }
}