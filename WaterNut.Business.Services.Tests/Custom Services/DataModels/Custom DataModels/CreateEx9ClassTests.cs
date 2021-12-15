namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using DocumentDS.Business.Entities;
    using System.Collections.Generic;
    using AllocationDS.Business.Entities;
    using WaterNut.Interfaces;
    using WaterNut.Business.Entities;
    using DocumentItemDS.Business.Entities;
    using EntryDataDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class CreateEx9ClassTests
    {
        private CreateEx9Class _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CreateEx9Class();
        }

        [Test]
        public async Task CanCallCreateEx9()
        {
            var filterExpression = "TestValue1485168107";
            var perIM7 = true;
            var process7100 = false;
            var applyCurrentChecks = false;
            var docSet = new AsycudaDocumentSet();
            var documentType = "TestValue1508265024";
            var ex9BucketType = "TestValue2146054816";
            var isGrouped = false;
            var checkQtyAllocatedGreaterThanPiQuantity = false;
            var checkForMultipleMonths = true;
            var applyEx9Bucket = true;
            var applyHistoricChecks = false;
            var perInvoice = true;
            var autoAssess = true;
            var overPIcheck = false;
            var universalPIcheck = true;
            var itemPIcheck = true;
            await _testClass.CreateEx9(filterExpression, perIM7, process7100, applyCurrentChecks, docSet, documentType, ex9BucketType, isGrouped, checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket, applyHistoricChecks, perInvoice, autoAssess, overPIcheck, universalPIcheck, itemPIcheck);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateEx9WithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9("TestValue1009330071", false, true, true, default(AsycudaDocumentSet), "TestValue1246580303", "TestValue2053553355", false, true, true, false, true, false, false, false, false, false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9WithInvalidFilterExpression(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9(value, false, true, false, new AsycudaDocumentSet(), "TestValue947024066", "TestValue1804216489", true, true, false, false, true, true, true, true, false, true));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9WithInvalidDocumentType(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9("TestValue1318347143", true, true, true, new AsycudaDocumentSet(), value, "TestValue1407180067", false, false, true, true, false, false, false, false, true, true));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9WithInvalidEx9BucketType(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9("TestValue1283659314", false, false, false, new AsycudaDocumentSet(), "TestValue888035477", value, false, true, true, true, true, true, true, true, true, false));
        }

        [Test]
        public void CanCallGetItemSalesPiSummary()
        {
            var applicationSettingsId = 1832489058;
            var startDate = new DateTime(1402510088);
            var endDate = new DateTime(291969937);
            var dfp = "TestValue1063066659";
            var entryDataType = "TestValue2106985715";
            var result = _testClass.GetItemSalesPiSummary(applicationSettingsId, startDate, endDate, dfp, entryDataType);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetItemSalesPiSummaryWithInvalidDfp(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetItemSalesPiSummary(1775880745, new DateTime(1108238548), new DateTime(528069733), value, "TestValue520279251"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetItemSalesPiSummaryWithInvalidEntryDataType(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.GetItemSalesPiSummary(823603451, new DateTime(784898470), new DateTime(360148279), "TestValue1597212682", value));
        }

        [Test]
        public async Task CanCallCreateDutyFreePaidDocument()
        {
            var dfp = "TestValue1044359481";
            var slst = new[] { new AllocationDataBlock { MonthYear = "TestValue1860734990", DutyFreePaid = "TestValue1955469977", Allocations = new List<EX9Allocations>(), CNumber = "TestValue367550345", Type = "TestValue1899185574" }, new AllocationDataBlock { MonthYear = "TestValue567424929", DutyFreePaid = "TestValue654972041", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1899216260", Type = "TestValue2043185890" }, new AllocationDataBlock { MonthYear = "TestValue2075480998", DutyFreePaid = "TestValue890239055", Allocations = new List<EX9Allocations>(), CNumber = "TestValue690815342", Type = "TestValue1352395343" } };
            var docSet = new AsycudaDocumentSet();
            var documentType = "TestValue1681447909";
            var isGrouped = false;
            var itemSalesPiSummaryLst = new List<CreateEx9Class.ItemSalesPiSummary>();
            var checkQtyAllocatedGreaterThanPiQuantity = true;
            var checkForMultipleMonths = false;
            var applyEx9Bucket = true;
            var ex9BucketType = "TestValue2103435996";
            var applyHistoricChecks = false;
            var applyCurrentChecks = true;
            var autoAssess = false;
            var perInvoice = false;
            var overPIcheck = false;
            var universalPIcheck = false;
            var itemPIcheck = true;
            var prefix = "TestValue1022052745";
            var result = await _testClass.CreateDutyFreePaidDocument(dfp, slst, docSet, documentType, isGrouped, itemSalesPiSummaryLst, checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks, autoAssess, perInvoice, overPIcheck, universalPIcheck, itemPIcheck, prefix);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateDutyFreePaidDocumentWithNullSlst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument("TestValue175420766", default(IEnumerable<AllocationDataBlock>), new AsycudaDocumentSet(), "TestValue1387925421", true, new List<CreateEx9Class.ItemSalesPiSummary>(), false, true, true, "TestValue516222765", true, false, true, false, false, true, false, "TestValue634092975"));
        }

        [Test]
        public void CannotCallCreateDutyFreePaidDocumentWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument("TestValue1936786815", new[] { new AllocationDataBlock { MonthYear = "TestValue1474512737", DutyFreePaid = "TestValue1549214529", Allocations = new List<EX9Allocations>(), CNumber = "TestValue372251317", Type = "TestValue754590372" }, new AllocationDataBlock { MonthYear = "TestValue94432836", DutyFreePaid = "TestValue613215138", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1141038865", Type = "TestValue42474893" }, new AllocationDataBlock { MonthYear = "TestValue591473062", DutyFreePaid = "TestValue828946121", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1948600988", Type = "TestValue403939871" } }, default(AsycudaDocumentSet), "TestValue826700307", false, new List<CreateEx9Class.ItemSalesPiSummary>(), false, false, true, "TestValue1692855831", true, true, true, false, false, false, false, "TestValue1906976437"));
        }

        [Test]
        public void CannotCallCreateDutyFreePaidDocumentWithNullItemSalesPiSummaryLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument("TestValue1330112521", new[] { new AllocationDataBlock { MonthYear = "TestValue301149268", DutyFreePaid = "TestValue2063462641", Allocations = new List<EX9Allocations>(), CNumber = "TestValue816165250", Type = "TestValue95812757" }, new AllocationDataBlock { MonthYear = "TestValue1906054031", DutyFreePaid = "TestValue559179836", Allocations = new List<EX9Allocations>(), CNumber = "TestValue428356006", Type = "TestValue1361425968" }, new AllocationDataBlock { MonthYear = "TestValue466291345", DutyFreePaid = "TestValue986159415", Allocations = new List<EX9Allocations>(), CNumber = "TestValue154113165", Type = "TestValue927619909" } }, new AsycudaDocumentSet(), "TestValue1709689275", false, default(List<CreateEx9Class.ItemSalesPiSummary>), false, false, true, "TestValue1466607306", false, false, false, false, true, true, false, "TestValue320909279"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateDutyFreePaidDocumentWithInvalidDfp(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument(value, new[] { new AllocationDataBlock { MonthYear = "TestValue122802136", DutyFreePaid = "TestValue461007672", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1343196885", Type = "TestValue762112481" }, new AllocationDataBlock { MonthYear = "TestValue949160622", DutyFreePaid = "TestValue2065901441", Allocations = new List<EX9Allocations>(), CNumber = "TestValue334940046", Type = "TestValue1430586264" }, new AllocationDataBlock { MonthYear = "TestValue839889597", DutyFreePaid = "TestValue126495899", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1922791459", Type = "TestValue733133364" } }, new AsycudaDocumentSet(), "TestValue42546957", false, new List<CreateEx9Class.ItemSalesPiSummary>(), true, true, true, "TestValue170463845", false, false, true, true, true, true, false, "TestValue103967036"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateDutyFreePaidDocumentWithInvalidDocumentType(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument("TestValue723889684", new[] { new AllocationDataBlock { MonthYear = "TestValue811721625", DutyFreePaid = "TestValue1189997917", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1155231771", Type = "TestValue510776890" }, new AllocationDataBlock { MonthYear = "TestValue243198501", DutyFreePaid = "TestValue1928528996", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1585144752", Type = "TestValue436377700" }, new AllocationDataBlock { MonthYear = "TestValue2114831981", DutyFreePaid = "TestValue18229083", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1851662511", Type = "TestValue36998793" } }, new AsycudaDocumentSet(), value, true, new List<CreateEx9Class.ItemSalesPiSummary>(), true, true, false, "TestValue398435955", false, true, true, true, true, false, false, "TestValue1382221906"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateDutyFreePaidDocumentWithInvalidEx9BucketType(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument("TestValue1884081524", new[] { new AllocationDataBlock { MonthYear = "TestValue1697015133", DutyFreePaid = "TestValue1551047279", Allocations = new List<EX9Allocations>(), CNumber = "TestValue2005881500", Type = "TestValue1657644030" }, new AllocationDataBlock { MonthYear = "TestValue417089470", DutyFreePaid = "TestValue1396684966", Allocations = new List<EX9Allocations>(), CNumber = "TestValue845193586", Type = "TestValue1342011757" }, new AllocationDataBlock { MonthYear = "TestValue1670702068", DutyFreePaid = "TestValue240588347", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1832141473", Type = "TestValue1763202656" } }, new AsycudaDocumentSet(), "TestValue1679592958", true, new List<CreateEx9Class.ItemSalesPiSummary>(), false, false, true, value, false, true, false, true, true, false, false, "TestValue552411334"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateDutyFreePaidDocumentWithInvalidPrefix(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDutyFreePaidDocument("TestValue1496206790", new[] { new AllocationDataBlock { MonthYear = "TestValue824369993", DutyFreePaid = "TestValue827390334", Allocations = new List<EX9Allocations>(), CNumber = "TestValue456366825", Type = "TestValue114811114" }, new AllocationDataBlock { MonthYear = "TestValue860737277", DutyFreePaid = "TestValue1482987032", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1276038631", Type = "TestValue506600624" }, new AllocationDataBlock { MonthYear = "TestValue231513863", DutyFreePaid = "TestValue34097473", Allocations = new List<EX9Allocations>(), CNumber = "TestValue577979847", Type = "TestValue457187951" } }, new AsycudaDocumentSet(), "TestValue1748623260", true, new List<CreateEx9Class.ItemSalesPiSummary>(), false, true, false, "TestValue38514663", false, false, true, true, true, false, false, value));
        }

        [Test]
        public async Task CreateDutyFreePaidDocumentPerformsMapping()
        {
            var dfp = "TestValue536635759";
            var slst = new[] { new AllocationDataBlock { MonthYear = "TestValue2050234492", DutyFreePaid = "TestValue1248756854", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1283408023", Type = "TestValue1524459074" }, new AllocationDataBlock { MonthYear = "TestValue912804643", DutyFreePaid = "TestValue1148474176", Allocations = new List<EX9Allocations>(), CNumber = "TestValue1105232696", Type = "TestValue1068366327" }, new AllocationDataBlock { MonthYear = "TestValue572314973", DutyFreePaid = "TestValue17803251", Allocations = new List<EX9Allocations>(), CNumber = "TestValue885644932", Type = "TestValue1555890954" } };
            var docSet = new AsycudaDocumentSet();
            var documentType = "TestValue1527334717";
            var isGrouped = true;
            var itemSalesPiSummaryLst = new List<CreateEx9Class.ItemSalesPiSummary>();
            var checkQtyAllocatedGreaterThanPiQuantity = true;
            var checkForMultipleMonths = false;
            var applyEx9Bucket = false;
            var ex9BucketType = "TestValue570788379";
            var applyHistoricChecks = true;
            var applyCurrentChecks = true;
            var autoAssess = false;
            var perInvoice = false;
            var overPIcheck = true;
            var universalPIcheck = true;
            var itemPIcheck = false;
            var prefix = "TestValue599727972";
            var result = await _testClass.CreateDutyFreePaidDocument(dfp, slst, docSet, documentType, isGrouped, itemSalesPiSummaryLst, checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks, autoAssess, perInvoice, overPIcheck, universalPIcheck, itemPIcheck, prefix);
            Assert.That(result.Capacity, Is.EqualTo(itemSalesPiSummaryLst.Capacity));
            Assert.That(result.Count, Is.EqualTo(itemSalesPiSummaryLst.Count));
            Assert.That(result.System.Collections.IList.IsFixedSize, Is.EqualTo(itemSalesPiSummaryLst.System.Collections.IList.IsFixedSize));
            Assert.That(result.System.Collections.Generic.ICollection<T>.IsReadOnly, Is.EqualTo(itemSalesPiSummaryLst.System.Collections.Generic.ICollection<T>.IsReadOnly));
            Assert.That(result.System.Collections.IList.IsReadOnly, Is.EqualTo(itemSalesPiSummaryLst.System.Collections.IList.IsReadOnly));
            Assert.That(result.System.Collections.ICollection.IsSynchronized, Is.EqualTo(itemSalesPiSummaryLst.System.Collections.ICollection.IsSynchronized));
            Assert.That(result.System.Collections.ICollection.SyncRoot, Is.EqualTo(itemSalesPiSummaryLst.System.Collections.ICollection.SyncRoot));
            Assert.That(result.System.Collections.IList.Item, Is.EqualTo(itemSalesPiSummaryLst.System.Collections.IList.Item));
        }

        [Test]
        public void CanCallMaxLineCount()
        {
            var itmcount = 1135068191;
            var result = _testClass.MaxLineCount(itmcount);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallCreateEx9EntryAsync()
        {
            var mypod = new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 1898006254.65, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 1582431321.24, InternalFreight = 1077037670.61, Freight = 8133678.629999999, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue1744709948", ItemNumber = "TestValue1996380217", PreviousDocumentItemId = 324101518, Quantity = 1564433825.1299999, TariffCode = "TestValue27281777", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 1370676248, DPQtyAllocated = 845993051.64, previousItems = new List<previousItems>(), DFQtyAllocated = 1256088712.23, AssessmentDate = new DateTime(1576989318), ItemQuantity = 1484444512.98, ItemNumber = "TestValue680256933", xcuda_ItemId = 2030146219, Description = "TestValue1321485602", ExpiryDate = new DateTime(155522329) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue1661054135", pQuantity = 652076958.06, Country_of_origin_code = "TestValue1520668339", Total_CIF_itm = 292597391.79, pCNumber = "TestValue872282627", pRegistrationDate = new DateTime(1163362649), Customs_clearance_office_code = "TestValue248386118", Net_weight_itm = 2008450655.6399999, pQtyAllocated = 289185462.71999997, SalesFactor = 341698986.09 }, FileTypeId = 588234496, EmailId = 307097009 }, AllNames = new List<string>() };
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var itmcount = 1723901783;
            var dfp = "TestValue273880810";
            var monthyear = "TestValue313076671";
            var documentType = "TestValue13405985";
            var itemSalesPiSummaryLst = new List<CreateEx9Class.ItemSalesPiSummary>();
            var checkQtyAllocatedGreaterThanPiQuantity = false;
            var applyEx9Bucket = false;
            var ex9BucketType = "TestValue1631881781";
            var applyHistoricChecks = true;
            var applyCurrentChecks = true;
            var overPIcheck = false;
            var universalPIcheck = true;
            var itemPIcheck = true;
            var result = await _testClass.CreateEx9EntryAsync(mypod, cdoc, itmcount, dfp, monthyear, documentType, itemSalesPiSummaryLst, checkQtyAllocatedGreaterThanPiQuantity, applyEx9Bucket, ex9BucketType, applyHistoricChecks, applyCurrentChecks, overPIcheck, universalPIcheck, itemPIcheck);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateEx9EntryAsyncWithNullMypod()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(default(MyPodData), new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, 459882248, "TestValue1949799733", "TestValue2013519363", "TestValue1449495549", new List<CreateEx9Class.ItemSalesPiSummary>(), false, true, "TestValue1658711982", false, false, false, false, true));
        }

        [Test]
        public void CannotCallCreateEx9EntryAsyncWithNullCdoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 1744264664.01, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 1605408862.86, InternalFreight = 718871520.51, Freight = 2051317458.6299999, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue1907313366", ItemNumber = "TestValue1863455559", PreviousDocumentItemId = 1704273689, Quantity = 2105170327.26, TariffCode = "TestValue992001691", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 1867668415, DPQtyAllocated = 1776315530.8799999, previousItems = new List<previousItems>(), DFQtyAllocated = 574851049.74, AssessmentDate = new DateTime(955699806), ItemQuantity = 1547947499.67, ItemNumber = "TestValue163900594", xcuda_ItemId = 1210306082, Description = "TestValue398264438", ExpiryDate = new DateTime(1311898187) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue1014587345", pQuantity = 2080527964.02, Country_of_origin_code = "TestValue1602914042", Total_CIF_itm = 598223773.62, pCNumber = "TestValue1983154320", pRegistrationDate = new DateTime(1069966541), Customs_clearance_office_code = "TestValue1297326933", Net_weight_itm = 936910473.84, pQtyAllocated = 475553996.28, SalesFactor = 1799560039.86 }, FileTypeId = 833922156, EmailId = 1156380719 }, AllNames = new List<string>() }, default(DocumentCT), 686922766, "TestValue172434225", "TestValue684356681", "TestValue372012293", new List<CreateEx9Class.ItemSalesPiSummary>(), true, true, "TestValue891301649", true, false, true, true, false));
        }

        [Test]
        public void CannotCallCreateEx9EntryAsyncWithNullItemSalesPiSummaryLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 2051165018.43, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 732300505.2, InternalFreight = 153986204.79, Freight = 1418533119.6299999, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue493795743", ItemNumber = "TestValue2132280778", PreviousDocumentItemId = 457952167, Quantity = 443921841, TariffCode = "TestValue1181072986", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 455759703, DPQtyAllocated = 531511410.87, previousItems = new List<previousItems>(), DFQtyAllocated = 1765118544.75, AssessmentDate = new DateTime(485633983), ItemQuantity = 1146040890.39, ItemNumber = "TestValue1785954441", xcuda_ItemId = 1803649955, Description = "TestValue774710904", ExpiryDate = new DateTime(960939150) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue1383097987", pQuantity = 2013679722.78, Country_of_origin_code = "TestValue1292512518", Total_CIF_itm = 1963273572.81, pCNumber = "TestValue1180745649", pRegistrationDate = new DateTime(1621823887), Customs_clearance_office_code = "TestValue2043784592", Net_weight_itm = 577850637.87, pQtyAllocated = 1152931428, SalesFactor = 445487526 }, FileTypeId = 319004433, EmailId = 838420494 }, AllNames = new List<string>() }, new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, 1062140491, "TestValue1589624273", "TestValue2098822703", "TestValue378631870", default(List<CreateEx9Class.ItemSalesPiSummary>), false, false, "TestValue914424920", true, true, true, true, false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9EntryAsyncWithInvalidDfp(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 2101563466.2, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 228851432.37, InternalFreight = 1765207282.41, Freight = 1038403379.97, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue2033861957", ItemNumber = "TestValue1388872719", PreviousDocumentItemId = 75442400, Quantity = 1222783987.59, TariffCode = "TestValue932616687", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 1436302193, DPQtyAllocated = 187455244.68, previousItems = new List<previousItems>(), DFQtyAllocated = 115022575.8, AssessmentDate = new DateTime(2079253301), ItemQuantity = 87891445.62, ItemNumber = "TestValue1706435478", xcuda_ItemId = 681201381, Description = "TestValue1536560792", ExpiryDate = new DateTime(2057591877) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue967703578", pQuantity = 7885119.33, Country_of_origin_code = "TestValue129401467", Total_CIF_itm = 339225967.08, pCNumber = "TestValue1541102859", pRegistrationDate = new DateTime(1094739587), Customs_clearance_office_code = "TestValue1831608969", Net_weight_itm = 105932091.87, pQtyAllocated = 472973324.67, SalesFactor = 537072624.99 }, FileTypeId = 889225035, EmailId = 910245708 }, AllNames = new List<string>() }, new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, 508360677, value, "TestValue887537546", "TestValue674234675", new List<CreateEx9Class.ItemSalesPiSummary>(), false, false, "TestValue949582586", true, true, true, false, false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9EntryAsyncWithInvalidMonthyear(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 1219039211.61, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 2032754579.46, InternalFreight = 1755588725.55, Freight = 1386272785.59, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue1982638283", ItemNumber = "TestValue446862216", PreviousDocumentItemId = 591086990, Quantity = 1667659958.91, TariffCode = "TestValue233223539", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 475388051, DPQtyAllocated = 537045540.57, previousItems = new List<previousItems>(), DFQtyAllocated = 1658587104.9, AssessmentDate = new DateTime(1351820480), ItemQuantity = 253553641.10999998, ItemNumber = "TestValue1780138888", xcuda_ItemId = 837543851, Description = "TestValue688298072", ExpiryDate = new DateTime(1364766981) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue1926859844", pQuantity = 902010667.14, Country_of_origin_code = "TestValue1680428446", Total_CIF_itm = 342451202.94, pCNumber = "TestValue22370979", pRegistrationDate = new DateTime(927941516), Customs_clearance_office_code = "TestValue1449294833", Net_weight_itm = 1573539058.08, pQtyAllocated = 1160534468.61, SalesFactor = 226181339.01 }, FileTypeId = 756852892, EmailId = 842415153 }, AllNames = new List<string>() }, new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, 541668503, "TestValue1360282674", value, "TestValue1191999097", new List<CreateEx9Class.ItemSalesPiSummary>(), false, true, "TestValue436848685", true, true, true, false, false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9EntryAsyncWithInvalidDocumentType(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 995421476.61, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 649441481.04, InternalFreight = 430509080.43, Freight = 2092240340.19, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue1359680683", ItemNumber = "TestValue1469897842", PreviousDocumentItemId = 650881473, Quantity = 225381418.02, TariffCode = "TestValue112038735", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 1297971803, DPQtyAllocated = 1759832865.45, previousItems = new List<previousItems>(), DFQtyAllocated = 908753675.93999994, AssessmentDate = new DateTime(1012066242), ItemQuantity = 833184198.99, ItemNumber = "TestValue885442433", xcuda_ItemId = 2030916475, Description = "TestValue845380429", ExpiryDate = new DateTime(2098464355) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue393204891", pQuantity = 1407867935.76, Country_of_origin_code = "TestValue362620991", Total_CIF_itm = 918375595.83, pCNumber = "TestValue1538292033", pRegistrationDate = new DateTime(2081203195), Customs_clearance_office_code = "TestValue1329671216", Net_weight_itm = 478507998.87, pQtyAllocated = 1701216705.87, SalesFactor = 1344485400.39 }, FileTypeId = 1343290203, EmailId = 1069762937 }, AllNames = new List<string>() }, new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, 993834026, "TestValue1515796295", "TestValue119236300", value, new List<CreateEx9Class.ItemSalesPiSummary>(), true, true, "TestValue1837392457", true, false, false, false, false));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEx9EntryAsyncWithInvalidEx9BucketType(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEx9EntryAsync(new MyPodData { Allocations = new List<AsycudaSalesAllocations>(), EntlnData = new CreateEx9Class.AlloEntryLineData { Cost = 799920.99, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 638366015.43, InternalFreight = 1675007727.03, Freight = 902427763.05, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue442349668", ItemNumber = "TestValue179932855", PreviousDocumentItemId = 939303213, Quantity = 158489042.58, TariffCode = "TestValue553415857", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 1069884132, DPQtyAllocated = 1872057315.15, previousItems = new List<previousItems>(), DFQtyAllocated = 1587216947.58, AssessmentDate = new DateTime(385534720), ItemQuantity = 1654811234.01, ItemNumber = "TestValue77824122", xcuda_ItemId = 1265193110, Description = "TestValue501138109", ExpiryDate = new DateTime(783702865) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue876339270", pQuantity = 1879990968.24, Country_of_origin_code = "TestValue1440299059", Total_CIF_itm = 1021532927.58, pCNumber = "TestValue1189759445", pRegistrationDate = new DateTime(304137777), Customs_clearance_office_code = "TestValue261812660", Net_weight_itm = 790709738.93999994, pQtyAllocated = 2113740927.54, SalesFactor = 164981514.06 }, FileTypeId = 1195533623, EmailId = 295919506 }, AllNames = new List<string>() }, new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, 2030812994, "TestValue2008850205", "TestValue273669341", "TestValue900713258", new List<CreateEx9Class.ItemSalesPiSummary>(), false, false, value, true, false, false, false, false));
        }

        [Test]
        public void CanCallEx9InitializeCdoc()
        {
            var dfp = "TestValue1197976119";
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var ads = new AsycudaDocumentSet();
            var DocumentType = "TestValue789874346";
            var prefix = "TestValue2147362452";
            _testClass.Ex9InitializeCdoc(dfp, cdoc, ads, DocumentType, prefix);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallEx9InitializeCdocWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Ex9InitializeCdoc("TestValue1250350688", default(DocumentCT), new AsycudaDocumentSet(), "TestValue2060030500", "TestValue1881185227"));
        }

        [Test]
        public void CannotCallEx9InitializeCdocWithNullAds()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Ex9InitializeCdoc("TestValue1500415191", new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, default(AsycudaDocumentSet), "TestValue597128085", "TestValue572199347"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallEx9InitializeCdocWithInvalidDfp(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Ex9InitializeCdoc(value, new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, new AsycudaDocumentSet(), "TestValue1233858860", "TestValue178348217"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallEx9InitializeCdocWithInvalidDocumentType(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Ex9InitializeCdoc("TestValue1360758527", new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, new AsycudaDocumentSet(), value, "TestValue368038421"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallEx9InitializeCdocWithInvalidPrefix(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Ex9InitializeCdoc("TestValue1228560154", new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, new AsycudaDocumentSet(), "TestValue1116440206", value));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CreateEx9Class.Instance, Is.InstanceOf<CreateEx9Class>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetPerIM7()
        {
            var testValue = true;
            _testClass.PerIM7 = testValue;
            Assert.That(_testClass.PerIM7, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetProcess7100()
        {
            var testValue = false;
            _testClass.Process7100 = testValue;
            Assert.That(_testClass.Process7100, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class SummaryDataTests
    {
        private SummaryData _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SummaryData();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new SummaryData();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetSummary()
        {
            var testValue = new ItemSalesAsycudaPiSummary();
            _testClass.Summary = testValue;
            Assert.That(_testClass.Summary, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpIData()
        {
            var testValue = new[] { new AsycudaItemPiQuantityData(), new AsycudaItemPiQuantityData(), new AsycudaItemPiQuantityData() };
            _testClass.pIData = testValue;
            Assert.That(_testClass.pIData, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class x7100SalesPiTests
    {
        private x7100SalesPi _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new x7100SalesPi();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new x7100SalesPi();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetItemNumber()
        {
            var testValue = "TestValue649730535";
            _testClass.ItemNumber = testValue;
            Assert.That(_testClass.ItemNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemQuantity()
        {
            var testValue = 1773225242.91;
            _testClass.ItemQuantity = testValue;
            Assert.That(_testClass.ItemQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPiQuantity()
        {
            var testValue = 190401409.44;
            _testClass.PiQuantity = testValue;
            Assert.That(_testClass.PiQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDPQtyAllocated()
        {
            var testValue = 764928666.81;
            _testClass.DPQtyAllocated = testValue;
            Assert.That(_testClass.DPQtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDPPi()
        {
            var testValue = 1100919566.34;
            _testClass.DPPi = testValue;
            Assert.That(_testClass.DPPi, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDFQtyAllocated()
        {
            var testValue = 254921387.49;
            _testClass.DFQtyAllocated = testValue;
            Assert.That(_testClass.DFQtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDFPi()
        {
            var testValue = 1174689237.15;
            _testClass.DFPi = testValue;
            Assert.That(_testClass.DFPi, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class PiSummaryTests
    {
        private PiSummary _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new PiSummary();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new PiSummary();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetItemNumber()
        {
            var testValue = "TestValue2029600427";
            _testClass.ItemNumber = testValue;
            Assert.That(_testClass.ItemNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDutyFreePaid()
        {
            var testValue = "TestValue1329580101";
            _testClass.DutyFreePaid = testValue;
            Assert.That(_testClass.DutyFreePaid, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTotalQuantity()
        {
            var testValue = 1615982057.91;
            _testClass.TotalQuantity = testValue;
            Assert.That(_testClass.TotalQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetType()
        {
            var testValue = "TestValue1309713509";
            _testClass.Type = testValue;
            Assert.That(_testClass.Type, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpCNumber()
        {
            var testValue = "TestValue1942469809";
            _testClass.pCNumber = testValue;
            Assert.That(_testClass.pCNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpLineNumber()
        {
            var testValue = "TestValue1418828074";
            _testClass.pLineNumber = testValue;
            Assert.That(_testClass.pLineNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpOffice()
        {
            var testValue = "TestValue1478961756";
            _testClass.pOffice = testValue;
            Assert.That(_testClass.pOffice, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpItemQuantity()
        {
            var testValue = 2023426314.36;
            _testClass.pItemQuantity = testValue;
            Assert.That(_testClass.pItemQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpAssessmentDate()
        {
            var testValue = new DateTime(572930712);
            _testClass.pAssessmentDate = testValue;
            Assert.That(_testClass.pAssessmentDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPreviousItem_Id()
        {
            var testValue = 1119883276;
            _testClass.PreviousItem_Id = testValue;
            Assert.That(_testClass.PreviousItem_Id, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class previousItemsTests
    {
        private previousItems _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new previousItems();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new previousItems();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetDutyFreePaid()
        {
            var testValue = "TestValue242322940";
            _testClass.DutyFreePaid = testValue;
            Assert.That(_testClass.DutyFreePaid, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSuplementary_Quantity()
        {
            var testValue = 239557325.04;
            _testClass.Suplementary_Quantity = testValue;
            Assert.That(_testClass.Suplementary_Quantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNet_weight()
        {
            var testValue = 1177981833.6;
            _testClass.Net_weight = testValue;
            Assert.That(_testClass.Net_weight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPreviousItem_Id()
        {
            var testValue = 1201270736;
            _testClass.PreviousItem_Id = testValue;
            Assert.That(_testClass.PreviousItem_Id, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class EX9AllocationsTests
    {
        private EX9Allocations _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EX9Allocations();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EX9Allocations();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetpTariffCode()
        {
            var testValue = "TestValue349265807";
            _testClass.pTariffCode = testValue;
            Assert.That(_testClass.pTariffCode, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDutyFreePaid()
        {
            var testValue = "TestValue1064995126";
            _testClass.DutyFreePaid = testValue;
            Assert.That(_testClass.DutyFreePaid, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpCNumber()
        {
            var testValue = "TestValue634676655";
            _testClass.pCNumber = testValue;
            Assert.That(_testClass.pCNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetInvoiceDate()
        {
            var testValue = new DateTime(1717003556);
            _testClass.InvoiceDate = testValue;
            Assert.That(_testClass.InvoiceDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPreviousItem_Id()
        {
            var testValue = 623334276;
            _testClass.PreviousItem_Id = testValue;
            Assert.That(_testClass.PreviousItem_Id, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSalesQuantity()
        {
            var testValue = 1197448850.07;
            _testClass.SalesQuantity = testValue;
            Assert.That(_testClass.SalesQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAllocationId()
        {
            var testValue = 1852464777;
            _testClass.AllocationId = testValue;
            Assert.That(_testClass.AllocationId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEntryDataDetailsId()
        {
            var testValue = 648091678;
            _testClass.EntryDataDetailsId = testValue;
            Assert.That(_testClass.EntryDataDetailsId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetStatus()
        {
            var testValue = "TestValue2053114567";
            _testClass.Status = testValue;
            Assert.That(_testClass.Status, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetInvoiceNo()
        {
            var testValue = "TestValue1819636751";
            _testClass.InvoiceNo = testValue;
            Assert.That(_testClass.InvoiceNo, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemNumber()
        {
            var testValue = "TestValue1392856431";
            _testClass.ItemNumber = testValue;
            Assert.That(_testClass.ItemNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpItemNumber()
        {
            var testValue = "TestValue827783248";
            _testClass.pItemNumber = testValue;
            Assert.That(_testClass.pItemNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemDescription()
        {
            var testValue = "TestValue606059463";
            _testClass.ItemDescription = testValue;
            Assert.That(_testClass.ItemDescription, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpItemCost()
        {
            var testValue = 514730170.35;
            _testClass.pItemCost = testValue;
            Assert.That(_testClass.pItemCost, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetQtyAllocated()
        {
            var testValue = 1231231900.14;
            _testClass.QtyAllocated = testValue;
            Assert.That(_testClass.QtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCommercial_Description()
        {
            var testValue = "TestValue111083902";
            _testClass.Commercial_Description = testValue;
            Assert.That(_testClass.Commercial_Description, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSalesQtyAllocated()
        {
            var testValue = 6377858.1899999995;
            _testClass.SalesQtyAllocated = testValue;
            Assert.That(_testClass.SalesQtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDFQtyAllocated()
        {
            var testValue = 85076791.47;
            _testClass.DFQtyAllocated = testValue;
            Assert.That(_testClass.DFQtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDPQtyAllocated()
        {
            var testValue = 527054215.05;
            _testClass.DPQtyAllocated = testValue;
            Assert.That(_testClass.DPQtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetLineNumber()
        {
            var testValue = 960807667;
            _testClass.LineNumber = testValue;
            Assert.That(_testClass.LineNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpreviousItems()
        {
            var testValue = new List<previousItems>();
            _testClass.previousItems = testValue;
            Assert.That(_testClass.previousItems, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCountry_of_origin_code()
        {
            var testValue = "TestValue1368233908";
            _testClass.Country_of_origin_code = testValue;
            Assert.That(_testClass.Country_of_origin_code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCustoms_clearance_office_code()
        {
            var testValue = "TestValue730450399";
            _testClass.Customs_clearance_office_code = testValue;
            Assert.That(_testClass.Customs_clearance_office_code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpQuantity()
        {
            var testValue = 246391316.82;
            _testClass.pQuantity = testValue;
            Assert.That(_testClass.pQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpRegistrationDate()
        {
            var testValue = new DateTime(190701682);
            _testClass.pRegistrationDate = testValue;
            Assert.That(_testClass.pRegistrationDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNet_weight_itm()
        {
            var testValue = 794120503.77;
            _testClass.Net_weight_itm = testValue;
            Assert.That(_testClass.Net_weight_itm, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTotal_CIF_itm()
        {
            var testValue = 1287846370.8;
            _testClass.Total_CIF_itm = testValue;
            Assert.That(_testClass.Total_CIF_itm, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetFreight()
        {
            var testValue = 1883356943.49;
            _testClass.Freight = testValue;
            Assert.That(_testClass.Freight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetInternalFreight()
        {
            var testValue = 279147231;
            _testClass.InternalFreight = testValue;
            Assert.That(_testClass.InternalFreight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetWeight()
        {
            var testValue = 779949536.85;
            _testClass.Weight = testValue;
            Assert.That(_testClass.Weight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTariffSupUnitLkps()
        {
            var testValue = new List<TariffSupUnitLkps>();
            _testClass.TariffSupUnitLkps = testValue;
            Assert.That(_testClass.TariffSupUnitLkps, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSalesFactor()
        {
            var testValue = 1381682404.08;
            _testClass.SalesFactor = testValue;
            Assert.That(_testClass.SalesFactor, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpAssessmentDate()
        {
            var testValue = new DateTime(986237214);
            _testClass.pAssessmentDate = testValue;
            Assert.That(_testClass.pAssessmentDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEffectiveDate()
        {
            var testValue = new DateTime(874463110);
            _testClass.EffectiveDate = testValue;
            Assert.That(_testClass.EffectiveDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpLineNumber()
        {
            var testValue = 412657719;
            _testClass.pLineNumber = testValue;
            Assert.That(_testClass.pLineNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCurrency()
        {
            var testValue = "TestValue186518484";
            _testClass.Currency = testValue;
            Assert.That(_testClass.Currency, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetFileTypeId()
        {
            var testValue = 300464728;
            _testClass.FileTypeId = testValue;
            Assert.That(_testClass.FileTypeId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEmailId()
        {
            var testValue = 726141483;
            _testClass.EmailId = testValue;
            Assert.That(_testClass.EmailId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEntryData_Id()
        {
            var testValue = 1705131648;
            _testClass.EntryData_Id = testValue;
            Assert.That(_testClass.EntryData_Id, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetComment()
        {
            var testValue = "TestValue1203135310";
            _testClass.Comment = testValue;
            Assert.That(_testClass.Comment, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetType()
        {
            var testValue = "TestValue488705690";
            _testClass.Type = testValue;
            Assert.That(_testClass.Type, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetInventoryItemId()
        {
            var testValue = 1195435705;
            _testClass.InventoryItemId = testValue;
            Assert.That(_testClass.InventoryItemId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpPrecision1()
        {
            var testValue = "TestValue1481573655";
            _testClass.pPrecision1 = testValue;
            Assert.That(_testClass.pPrecision1, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPIData()
        {
            var testValue = new List<AsycudaSalesAllocationsPIData>();
            _testClass.PIData = testValue;
            Assert.That(_testClass.PIData, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetpExpiryDate()
        {
            var testValue = new DateTime(1381508749);
            _testClass.pExpiryDate = testValue;
            Assert.That(_testClass.pExpiryDate, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class AllocationDataBlockTests
    {
        private AllocationDataBlock _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new AllocationDataBlock();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new AllocationDataBlock();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetMonthYear()
        {
            var testValue = "TestValue1423949181";
            _testClass.MonthYear = testValue;
            Assert.That(_testClass.MonthYear, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDutyFreePaid()
        {
            var testValue = "TestValue1960152005";
            _testClass.DutyFreePaid = testValue;
            Assert.That(_testClass.DutyFreePaid, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAllocations()
        {
            var testValue = new List<EX9Allocations>();
            _testClass.Allocations = testValue;
            Assert.That(_testClass.Allocations, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCNumber()
        {
            var testValue = "TestValue2064340725";
            _testClass.CNumber = testValue;
            Assert.That(_testClass.CNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetType()
        {
            var testValue = "TestValue1114686561";
            _testClass.Type = testValue;
            Assert.That(_testClass.Type, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class MyPodDataTests
    {
        private MyPodData _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new MyPodData();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new MyPodData();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetAllocations()
        {
            var testValue = new List<AsycudaSalesAllocations>();
            _testClass.Allocations = testValue;
            Assert.That(_testClass.Allocations, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEntlnData()
        {
            var testValue = new CreateEx9Class.AlloEntryLineData { Cost = 804640924.89, EntryDataDetails = new List<EntryDataDetailSummary>(), Weight = 949441968.09, InternalFreight = 792194414.22, Freight = 457228341.9, TariffSupUnitLkps = new List<ITariffSupUnitLkp>(), ItemDescription = "TestValue1113440995", ItemNumber = "TestValue156386787", PreviousDocumentItemId = 1857082748, Quantity = 226782243.27, TariffCode = "TestValue1980520475", pDocumentItem = new CreateEx9Class.pDocumentItem { LineNumber = 1766299055, DPQtyAllocated = 807953857.92, previousItems = new List<previousItems>(), DFQtyAllocated = 439535223.27, AssessmentDate = new DateTime(914861633), ItemQuantity = 1455263372.97, ItemNumber = "TestValue1454647189", xcuda_ItemId = 1570497877, Description = "TestValue2007747510", ExpiryDate = new DateTime(657475775) }, EX9Allocation = new CreateEx9Class.EX9Allocation { pTariffCode = "TestValue833399537", pQuantity = 513209387.79, Country_of_origin_code = "TestValue415125529", Total_CIF_itm = 415345569.21, pCNumber = "TestValue219464737", pRegistrationDate = new DateTime(517527103), Customs_clearance_office_code = "TestValue553435901", Net_weight_itm = 941282711.81999993, pQtyAllocated = 1727266968.9, SalesFactor = 1469581677.6299999 }, FileTypeId = 1626717659, EmailId = 2134208806 };
            _testClass.EntlnData = testValue;
            Assert.That(_testClass.EntlnData, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAllNames()
        {
            var testValue = new List<string>();
            _testClass.AllNames = testValue;
            Assert.That(_testClass.AllNames, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class AlloEntryLineDataTests
    {
        private AlloEntryLineData _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new AlloEntryLineData();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new AlloEntryLineData();
            Assert.That(instance, Is.Not.Null);
        }
    }
}