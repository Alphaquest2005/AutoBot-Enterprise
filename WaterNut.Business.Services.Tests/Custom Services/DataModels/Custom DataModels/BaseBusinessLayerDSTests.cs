namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using NSubstitute;
    using CoreEntities.Business.Entities;
    using WaterNut.Business.Entities;
    using DocumentDS.Business.Entities;
    using System.Collections.Generic;
    using DocumentItemDS.Business.Entities;
    using EntryDataDS.Business.Entities;
    using InventoryDS.Business.Entities;
    using System.IO;
    using Core.Common.Data;
    using System.Threading.Tasks;

    [TestFixture]
    public class BaseDataModelTests
    {
        private BaseDataModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new BaseDataModel();
        }

        [Test]
        public void CanCallGetFileTypeWithFileTypes()
        {
            var fileTypes = new FileTypes();
            var result = BaseDataModel.GetFileType(fileTypes);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetFileTypeWithFileTypesWithNullFileTypes()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.GetFileType(default(FileTypes)));
        }

        [Test]
        public void CanCallCurrentSalesInfo()
        {
            var result = BaseDataModel.CurrentSalesInfo();
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallValidateInstallation()
        {
            var result = _testClass.ValidateInstallation();
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallClearAsycudaDocumentSetWithInt()
        {
            var AsycudaDocumentSetId = 2026667327;
            await _testClass.ClearAsycudaDocumentSet(AsycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallAttachCustomProcedure()
        {
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var cp = new Customs_Procedure();
            _testClass.AttachCustomProcedure(cdoc, cp);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAttachCustomProcedureWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AttachCustomProcedure(default(DocumentCT), new Customs_Procedure()));
        }

        [Test]
        public void CannotCallAttachCustomProcedureWithNullCp()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.AttachCustomProcedure(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, default(Customs_Procedure)));
        }

        [Test]
        public async Task CanCallClearAsycudaDocumentSetWithDocset()
        {
            var docset = new AsycudaDocumentSet();
            await _testClass.ClearAsycudaDocumentSet(docset);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallClearAsycudaDocumentSetWithDocsetWithNullDocset()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ClearAsycudaDocumentSet(default(AsycudaDocumentSet)));
        }

        [Test]
        public void CanCallIntCdocWithDocAndAds()
        {
            var doc = new xcuda_ASYCUDA();
            var ads = new AsycudaDocumentSet();
            _testClass.IntCdoc(doc, ads);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallIntCdocWithDocAndAdsWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.IntCdoc(default(xcuda_ASYCUDA), new AsycudaDocumentSet()));
        }

        [Test]
        public void CannotCallIntCdocWithDocAndAdsWithNullAds()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.IntCdoc(new xcuda_ASYCUDA(), default(AsycudaDocumentSet)));
        }

        [Test]
        public async Task CanCallCreateDocumentCt()
        {
            var currentAsycudaDocumentSet = new AsycudaDocumentSet();
            var result = await _testClass.CreateDocumentCt(currentAsycudaDocumentSet);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateDocumentCtWithNullCurrentAsycudaDocumentSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateDocumentCt(default(AsycudaDocumentSet)));
        }

        [Test]
        public void CanCallCreateNewAsycudaDocument()
        {
            var CurrentAsycudaDocumentSet = new AsycudaDocumentSet();
            var result = _testClass.CreateNewAsycudaDocument(CurrentAsycudaDocumentSet);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateNewAsycudaDocumentWithNullCurrentAsycudaDocumentSet()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CreateNewAsycudaDocument(default(AsycudaDocumentSet)));
        }

        [Test]
        public void CreateNewAsycudaDocumentPerformsMapping()
        {
            var CurrentAsycudaDocumentSet = new AsycudaDocumentSet();
            var result = _testClass.CreateNewAsycudaDocument(CurrentAsycudaDocumentSet);
            Assert.That(result.EntityId, Is.EqualTo(CurrentAsycudaDocumentSet.EntityId));
            Assert.That(result.ChangeTracker, Is.EqualTo(CurrentAsycudaDocumentSet.ChangeTracker));
            Assert.That(result.EntityKey, Is.EqualTo(CurrentAsycudaDocumentSet.EntityKey));
            Assert.That(result.EntryTimeStamp, Is.EqualTo(CurrentAsycudaDocumentSet.EntryTimeStamp));
            Assert.That(result.UpgradeKey, Is.EqualTo(CurrentAsycudaDocumentSet.UpgradeKey));
            Assert.That(result.xcuda_ASYCUDA_ExtendedProperties, Is.EqualTo(CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties));
        }

        [Test]
        public void CanCallUpdateAsycudaDocumentSetLastNumber()
        {
            var docSetId = 542907043;
            var num = 1995434988;
            var result = _testClass.UpdateAsycudaDocumentSetLastNumber(docSetId, num);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallIntCdocWithCdocAndAdsAndPrefix()
        {
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            var ads = new AsycudaDocumentSet();
            var prefix = "TestValue781029470";
            _testClass.IntCdoc(cdoc, ads, prefix);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallIntCdocWithCdocAndAdsAndPrefixWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.IntCdoc(default(DocumentCT), new AsycudaDocumentSet(), "TestValue606190233"));
        }

        [Test]
        public void CannotCallIntCdocWithCdocAndAdsAndPrefixWithNullAds()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.IntCdoc(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, default(AsycudaDocumentSet), "TestValue1414324166"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallIntCdocWithCdocAndAdsAndPrefixWithInvalidPrefix(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.IntCdoc(new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }, new AsycudaDocumentSet(), value));
        }

        [Test]
        public async Task CanCallAddToEntryWithEntryDatalstAndCurrentAsycudaDocumentSetAndPerInvoiceAndCombineEntryDataInSameFileAndGroupItemsAndCheckPackages()
        {
            var entryDatalst = new[] { "TestValue213097327", "TestValue760090614", "TestValue1458113440" };
            var currentAsycudaDocumentSet = new AsycudaDocumentSet();
            var perInvoice = true;
            var combineEntryDataInSameFile = true;
            var groupItems = true;
            var checkPackages = true;
            await _testClass.AddToEntry(entryDatalst, currentAsycudaDocumentSet, perInvoice, combineEntryDataInSameFile, groupItems, checkPackages);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAddToEntryWithEntryDatalstAndCurrentAsycudaDocumentSetAndPerInvoiceAndCombineEntryDataInSameFileAndGroupItemsAndCheckPackagesWithNullEntryDatalst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AddToEntry(default(IEnumerable<string>), new AsycudaDocumentSet(), true, true, true, false));
        }

        [Test]
        public void CannotCallAddToEntryWithEntryDatalstAndCurrentAsycudaDocumentSetAndPerInvoiceAndCombineEntryDataInSameFileAndGroupItemsAndCheckPackagesWithNullCurrentAsycudaDocumentSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AddToEntry(new[] { "TestValue31694403", "TestValue1290437", "TestValue1403505651" }, default(AsycudaDocumentSet), false, false, true, true));
        }

        [Test]
        public async Task CanCallAddToEntryWithEntryDatalstAndDocSetIdAndPerInvoiceAndCombineEntryDataInSameFileAndGroupItems()
        {
            var entryDatalst = new[] { 1037808024, 1186415128, 665110427 };
            var docSetId = 2027229472;
            var perInvoice = true;
            var combineEntryDataInSameFile = false;
            var groupItems = true;
            await _testClass.AddToEntry(entryDatalst, docSetId, perInvoice, combineEntryDataInSameFile, groupItems);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAddToEntryWithEntryDatalstAndDocSetIdAndPerInvoiceAndCombineEntryDataInSameFileAndGroupItemsWithNullEntryDatalst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.AddToEntry(default(IEnumerable<int>), 618878510, false, true, false));
        }

        [Test]
        public async Task CanCallValidateExistingTariffCodes()
        {
            var currentAsycudaDocumentSet = new AsycudaDocumentSet();
            await _testClass.ValidateExistingTariffCodes(currentAsycudaDocumentSet);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallValidateExistingTariffCodesWithNullCurrentAsycudaDocumentSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ValidateExistingTariffCodes(default(AsycudaDocumentSet)));
        }

        [Test]
        public void CanCallSetFilename()
        {
            var droppedFilePath = "TestValue1188387946";
            var targetFileName = "TestValue2064716054";
            var nameExtension = "TestValue487395722";
            var result = BaseDataModel.SetFilename(droppedFilePath, targetFileName, nameExtension);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallSetFilenameWithInvalidDroppedFilePath(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.SetFilename(value, "TestValue1916949994", "TestValue17767911"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallSetFilenameWithInvalidTargetFileName(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.SetFilename("TestValue948394450", value, "TestValue1521075914"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallSetFilenameWithInvalidNameExtension(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.SetFilename("TestValue1240880353", "TestValue679780672", value));
        }

        [Test]
        public async Task CanCallCreateEntryItems()
        {
            var slstSource = new List<EntryDataDetails>();
            var currentAsycudaDocumentSet = new AsycudaDocumentSet();
            var perInvoice = false;
            var autoUpdate = true;
            var autoAssess = false;
            var combineEntryDataInSameFile = true;
            var groupItems = true;
            var checkPackages = false;
            var prefix = "TestValue1927623280";
            var result = await _testClass.CreateEntryItems(slstSource, currentAsycudaDocumentSet, perInvoice, autoUpdate, autoAssess, combineEntryDataInSameFile, groupItems, checkPackages, prefix);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallCreateEntryItemsWithNullSlstSource()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEntryItems(default(List<EntryDataDetails>), new AsycudaDocumentSet(), false, true, false, true, true, true, "TestValue158760349"));
        }

        [Test]
        public void CannotCallCreateEntryItemsWithNullCurrentAsycudaDocumentSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEntryItems(new List<EntryDataDetails>(), default(AsycudaDocumentSet), true, false, true, false, false, false, "TestValue1227959997"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCreateEntryItemsWithInvalidPrefix(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.CreateEntryItems(new List<EntryDataDetails>(), new AsycudaDocumentSet(), true, false, false, true, true, false, value));
        }

        [Test]
        public async Task CreateEntryItemsPerformsMapping()
        {
            var slstSource = new List<EntryDataDetails>();
            var currentAsycudaDocumentSet = new AsycudaDocumentSet();
            var perInvoice = false;
            var autoUpdate = true;
            var autoAssess = true;
            var combineEntryDataInSameFile = true;
            var groupItems = true;
            var checkPackages = false;
            var prefix = "TestValue1088108039";
            var result = await _testClass.CreateEntryItems(slstSource, currentAsycudaDocumentSet, perInvoice, autoUpdate, autoAssess, combineEntryDataInSameFile, groupItems, checkPackages, prefix);
            Assert.That(result.Capacity, Is.EqualTo(slstSource.Capacity));
            Assert.That(result.Count, Is.EqualTo(slstSource.Count));
            Assert.That(result.System.Collections.IList.IsFixedSize, Is.EqualTo(slstSource.System.Collections.IList.IsFixedSize));
            Assert.That(result.System.Collections.Generic.ICollection<T>.IsReadOnly, Is.EqualTo(slstSource.System.Collections.Generic.ICollection<T>.IsReadOnly));
            Assert.That(result.System.Collections.IList.IsReadOnly, Is.EqualTo(slstSource.System.Collections.IList.IsReadOnly));
            Assert.That(result.System.Collections.ICollection.IsSynchronized, Is.EqualTo(slstSource.System.Collections.ICollection.IsSynchronized));
            Assert.That(result.System.Collections.ICollection.SyncRoot, Is.EqualTo(slstSource.System.Collections.ICollection.SyncRoot));
            Assert.That(result.System.Collections.IList.Item, Is.EqualTo(slstSource.System.Collections.IList.Item));
        }

        [Test]
        public void CanCallLinkPDFsWithCNumbersAndDocCode()
        {
            var cNumbers = new List<string>();
            var docCode = "TestValue1056353185";
            BaseDataModel.LinkPDFs(cNumbers, docCode);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallLinkPDFsWithCNumbersAndDocCodeWithNullCNumbers()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.LinkPDFs(default(List<string>), "TestValue1048392622"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallLinkPDFsWithCNumbersAndDocCodeWithInvalidDocCode(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.LinkPDFs(new List<string>(), value));
        }

        [Test]
        public void CanCallLinkPDFsWithEntriesAndDocCode()
        {
            var entries = new List<int>();
            var docCode = "TestValue1536678973";
            BaseDataModel.LinkPDFs(entries, docCode);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallLinkPDFsWithEntriesAndDocCodeWithNullEntries()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.LinkPDFs(default(List<int>), "TestValue966682831"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallLinkPDFsWithEntriesAndDocCodeWithInvalidDocCode(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.LinkPDFs(new List<int>(), value));
        }

        [Test]
        public void CanCallConfigureDocSet()
        {
            var docSet = new AsycudaDocumentSet();
            var exportTemplate = new ExportTemplate();
            BaseDataModel.ConfigureDocSet(docSet, exportTemplate);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallConfigureDocSetWithNullDocSet()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.ConfigureDocSet(default(AsycudaDocumentSet), new ExportTemplate()));
        }

        [Test]
        public void CannotCallConfigureDocSetWithNullExportTemplate()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.ConfigureDocSet(new AsycudaDocumentSet(), default(ExportTemplate)));
        }

        [Test]
        public void CanCallAttachToDocument()
        {
            var alst = new List<Attachment>();
            var doc = new xcuda_ASYCUDA();
            var itms = new List<xcuda_Item>();
            BaseDataModel.AttachToDocument(alst, doc, itms);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAttachToDocumentWithNullAlst()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.AttachToDocument(default(List<Attachment>), new xcuda_ASYCUDA(), new List<xcuda_Item>()));
        }

        [Test]
        public void CannotCallAttachToDocumentWithNullDoc()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.AttachToDocument(new List<Attachment>(), default(xcuda_ASYCUDA), new List<xcuda_Item>()));
        }

        [Test]
        public void CannotCallAttachToDocumentWithNullItms()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.AttachToDocument(new List<Attachment>(), new xcuda_ASYCUDA(), default(List<xcuda_Item>)));
        }

        [Test]
        public async Task CanCallGetDocument()
        {
            var ASYCUDA_Id = 2084456044;
            var includeLst = new List<string>();
            var result = await _testClass.GetDocument(ASYCUDA_Id, includeLst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetDocumentWithNullIncludeLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetDocument(1024459333, default(List<string>)));
        }

        [Test]
        public async Task GetDocumentPerformsMapping()
        {
            var ASYCUDA_Id = 1222948999;
            var includeLst = new List<string>();
            var result = await _testClass.GetDocument(ASYCUDA_Id, includeLst);
            Assert.That(result.ASYCUDA_Id, Is.EqualTo(ASYCUDA_Id));
        }

        [Test]
        public async Task CanCallGetAllDocumentItems()
        {
            var includeLst = new List<string>();
            var result = await _testClass.GetAllDocumentItems(includeLst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetAllDocumentItemsWithNullIncludeLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetAllDocumentItems(default(List<string>)));
        }

        [Test]
        public async Task CanCallGetxcuda_Item()
        {
            var p = 322875899;
            var result = await _testClass.Getxcuda_Item(p);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallSaveDocumentCT()
        {
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            await _testClass.SaveDocumentCT(cdoc);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveDocumentCTWithNullCdoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveDocumentCT(default(DocumentCT)));
        }

        [Test]
        public async Task CanCallCalculateDocumentSetFreight()
        {
            var asycudaDocumentSetId = 1936521978;
            await _testClass.CalculateDocumentSetFreight(asycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallLimitFreeText()
        {
            var itm = new xcuda_Item();
            BaseDataModel.LimitFreeText(itm);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallLimitFreeTextWithNullItm()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.LimitFreeText(default(xcuda_Item)));
        }

        [Test]
        public void CanCallCleanText()
        {
            var p = "TestValue664975932";
            var result = _testClass.CleanText(p);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallCleanTextWithInvalidP(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.CleanText(value));
        }

        [Test]
        public void CanCallProcessItemTariff()
        {
            var pod = Substitute.For<BaseDataModel.IEntryLineData>();
            var cdoc = new xcuda_ASYCUDA();
            var itm = new xcuda_Item();
            _testClass.ProcessItemTariff(pod, cdoc, itm);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallProcessItemTariffWithNullPod()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ProcessItemTariff(default(BaseDataModel.IEntryLineData), new xcuda_ASYCUDA(), new xcuda_Item()));
        }

        [Test]
        public void CannotCallProcessItemTariffWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ProcessItemTariff(Substitute.For<BaseDataModel.IEntryLineData>(), default(xcuda_ASYCUDA), new xcuda_Item()));
        }

        [Test]
        public void CannotCallProcessItemTariffWithNullItm()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ProcessItemTariff(Substitute.For<BaseDataModel.IEntryLineData>(), new xcuda_ASYCUDA(), default(xcuda_Item)));
        }

        [Test]
        public async Task CanCallRemoveEntryData()
        {
            var po = "TestValue1929330885";
            await _testClass.RemoveEntryData(po);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallRemoveEntryDataWithInvalidPo(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.RemoveEntryData(value));
        }

        [Test]
        public async Task CanCallRemoveItem()
        {
            var id = 2113001532;
            await _testClass.RemoveItem(id);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallReDoDocumentLineNumbers()
        {
            var ASYCUDA_Id = 265885967;
            await _testClass.ReDoDocumentLineNumbers(ASYCUDA_Id);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallDeleteItem()
        {
            var p = 1905955705;
            await _testClass.DeleteItem(p);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallCreateAsycudaDocumentSet()
        {
            var applicationSettingsId = 744174276;
            var result = await _testClass.CreateAsycudaDocumentSet(applicationSettingsId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CreateAsycudaDocumentSetPerformsMapping()
        {
            var applicationSettingsId = 349691242;
            var result = await _testClass.CreateAsycudaDocumentSet(applicationSettingsId);
            Assert.That(result.ApplicationSettingsId, Is.EqualTo(applicationSettingsId));
        }

        [Test]
        public async Task CanCallDeleteAsycudaDocumentWithInt()
        {
            var ASYCUDA_Id = 1416037819;
            await _testClass.DeleteAsycudaDocument(ASYCUDA_Id);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallSave_xcuda_PreviousItem()
        {
            var pi = new xcuda_PreviousItem();
            await _testClass.Save_xcuda_PreviousItem(pi);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSave_xcuda_PreviousItemWithNullPi()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.Save_xcuda_PreviousItem(default(xcuda_PreviousItem)));
        }

        [Test]
        public async Task CanCallSave_xcuda_Item()
        {
            var Originalitm = new xcuda_Item();
            await _testClass.Save_xcuda_Item(Originalitm);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSave_xcuda_ItemWithNullOriginalitm()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.Save_xcuda_Item(default(xcuda_Item)));
        }

        [Test]
        public async Task CanCallSaveDocument()
        {
            var doc = new xcuda_ASYCUDA();
            await _testClass.SaveDocument(doc);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveDocumentWithNullDoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveDocument(default(xcuda_ASYCUDA)));
        }

        [Test]
        public async Task CanCallSaveInventoryItem()
        {
            var item = new InventoryItem();
            await _testClass.SaveInventoryItem(item);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveInventoryItemWithNullItem()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveInventoryItem(default(InventoryItem)));
        }

        [Test]
        public async Task CanCallDeleteAsycudaDocumentWithXcuda_ASYCUDA()
        {
            var doc = new xcuda_ASYCUDA();
            await _testClass.DeleteAsycudaDocument(doc);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallDeleteAsycudaDocumentWithXcuda_ASYCUDAWithNullDoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.DeleteAsycudaDocument(default(xcuda_ASYCUDA)));
        }

        [Test]
        public async Task CanCallImportDocumentsWithAsycudaDocumentSetIdAndFileNamesAndImportOnlyRegisteredDocumentAndImportTariffCodesAndNoMessagesAndOverwriteExistingAndLinkPi()
        {
            var asycudaDocumentSetId = 615042948;
            var fileNames = new List<string>();
            var importOnlyRegisteredDocument = true;
            var importTariffCodes = true;
            var noMessages = false;
            var overwriteExisting = false;
            var linkPi = true;
            await _testClass.ImportDocuments(asycudaDocumentSetId, fileNames, importOnlyRegisteredDocument, importTariffCodes, noMessages, overwriteExisting, linkPi);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallImportDocumentsWithAsycudaDocumentSetIdAndFileNamesAndImportOnlyRegisteredDocumentAndImportTariffCodesAndNoMessagesAndOverwriteExistingAndLinkPiWithNullFileNames()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ImportDocuments(1419157358, default(List<string>), false, false, true, false, true));
        }

        [Test]
        public async Task CanCallImportDocumentsWithDocSetAndFileNamesAndImportOnlyRegisteredDocumentAndImportTariffCodesAndNoMessagesAndOverwriteExistingAndLinkPi()
        {
            var docSet = new AsycudaDocumentSet();
            var fileNames = new[] { "TestValue846317653", "TestValue802161017", "TestValue712565890" };
            var importOnlyRegisteredDocument = true;
            var importTariffCodes = true;
            var noMessages = true;
            var overwriteExisting = true;
            var linkPi = true;
            await _testClass.ImportDocuments(docSet, fileNames, importOnlyRegisteredDocument, importTariffCodes, noMessages, overwriteExisting, linkPi);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallImportDocumentsWithDocSetAndFileNamesAndImportOnlyRegisteredDocumentAndImportTariffCodesAndNoMessagesAndOverwriteExistingAndLinkPiWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ImportDocuments(default(AsycudaDocumentSet), new[] { "TestValue1930014555", "TestValue396379105", "TestValue637985089" }, false, false, false, false, false));
        }

        [Test]
        public void CannotCallImportDocumentsWithDocSetAndFileNamesAndImportOnlyRegisteredDocumentAndImportTariffCodesAndNoMessagesAndOverwriteExistingAndLinkPiWithNullFileNames()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ImportDocuments(new AsycudaDocumentSet(), default(IEnumerable<string>), false, false, false, false, false));
        }

        [Test]
        public void CanCallImportC71()
        {
            var asycudaDocumentSetId = 714968354;
            var files = new List<string>();
            _testClass.ImportC71(asycudaDocumentSetId, files);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallImportC71WithNullFiles()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ImportC71(1987551900, default(List<string>)));
        }

        [Test]
        public void CanCallImportLicense()
        {
            var asycudaDocumentSetId = 857092023;
            var files = new List<string>();
            _testClass.ImportLicense(asycudaDocumentSetId, files);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallImportLicenseWithNullFiles()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.ImportLicense(349314241, default(List<string>)));
        }

        [Test]
        public async Task CanCallExportDocument()
        {
            var filename = "TestValue1636287364";
            var doc = new xcuda_ASYCUDA();
            await _testClass.ExportDocument(filename, doc);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallExportDocumentWithNullDoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExportDocument("TestValue1653887468", default(xcuda_ASYCUDA)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallExportDocumentWithInvalidFilename(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExportDocument(value, new xcuda_ASYCUDA()));
        }

        [Test]
        public void CanCallIM72Ex9Document()
        {
            var filename = "TestValue366212283";
            _testClass.IM72Ex9Document(filename);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallIM72Ex9DocumentWithInvalidFilename(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.IM72Ex9Document(value));
        }

        [Test]
        public async Task CanCallExportDocSet()
        {
            var AsycudaDocumentSetId = 117521615;
            var directoryName = "TestValue1643980218";
            var overWrite = false;
            await _testClass.ExportDocSet(AsycudaDocumentSetId, directoryName, overWrite);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallExportDocSetWithInvalidDirectoryName(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExportDocSet(1568616199, value, true));
        }

        [Test]
        public async Task CanCallExportLastDocumentInDocSet()
        {
            var AsycudaDocumentSetId = 1406140773;
            var directoryName = "TestValue1090204325";
            var overWrite = false;
            await _testClass.ExportLastDocumentInDocSet(AsycudaDocumentSetId, directoryName, overWrite);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallExportLastDocumentInDocSetWithInvalidDirectoryName(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExportLastDocumentInDocSet(1216764995, value, true));
        }

        [Test]
        public async Task CanCallGetDocSetWithEntryDataDocs()
        {
            var AsycudaDocumentSetId = 431492027;
            var result = await _testClass.GetDocSetWithEntryDataDocs(AsycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task GetDocSetWithEntryDataDocsPerformsMapping()
        {
            var AsycudaDocumentSetId = 656064894;
            var result = await _testClass.GetDocSetWithEntryDataDocs(AsycudaDocumentSetId);
            Assert.That(result.AsycudaDocumentSetId, Is.EqualTo(AsycudaDocumentSetId));
        }

        [Test]
        public async Task CanCallGetAsycudaDocumentSet()
        {
            var asycudaDocumentSetId = 2049345452;
            var result = await _testClass.GetAsycudaDocumentSet(asycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task GetAsycudaDocumentSetPerformsMapping()
        {
            var asycudaDocumentSetId = 319111463;
            var result = await _testClass.GetAsycudaDocumentSet(asycudaDocumentSetId);
            Assert.That(result.AsycudaDocumentSetId, Is.EqualTo(asycudaDocumentSetId));
        }

        [Test]
        public void CanCallSaveAttachments()
        {
            var docSet = new AsycudaDocumentSet();
            var cdoc = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            BaseDataModel.SaveAttachments(docSet, cdoc);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveAttachmentsWithNullDocSet()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.SaveAttachments(default(AsycudaDocumentSet), new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() }));
        }

        [Test]
        public void CannotCallSaveAttachmentsWithNullCdoc()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.SaveAttachments(new AsycudaDocumentSet(), default(DocumentCT)));
        }

        [Test]
        public async Task CanCallGetSelectedPODetailsWithLstAndAsycudaDocumentSetId()
        {
            var lst = new List<int>();
            var asycudaDocumentSetId = 1633506911;
            var result = await _testClass.GetSelectedPODetails(lst, asycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetSelectedPODetailsWithLstAndAsycudaDocumentSetIdWithNullLst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetSelectedPODetails(default(List<int>), 218663843));
        }

        [Test]
        public async Task CanCallGetSelectedPODetailsWithElstAndAsycudaDocumentSetId()
        {
            var elst = new List<string>();
            var asycudaDocumentSetId = 1368130050;
            var result = await _testClass.GetSelectedPODetails(elst, asycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetSelectedPODetailsWithElstAndAsycudaDocumentSetIdWithNullElst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.GetSelectedPODetails(default(List<string>), 1553779736));
        }

        [Test]
        public async Task CanCallSaveAsycudaDocumentItem()
        {
            var asycudaDocumentItem = new AsycudaDocumentItem();
            await _testClass.SaveAsycudaDocumentItem(asycudaDocumentItem);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveAsycudaDocumentItemWithNullAsycudaDocumentItem()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveAsycudaDocumentItem(default(AsycudaDocumentItem)));
        }

        [Test]
        public async Task CanCallSaveAsycudaDocument()
        {
            var asycudaDocument = new AsycudaDocument();
            await _testClass.SaveAsycudaDocument(asycudaDocument);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveAsycudaDocumentWithNullAsycudaDocument()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveAsycudaDocument(default(AsycudaDocument)));
        }

        [Test]
        public async Task CanCallDeleteDocumentCt()
        {
            var da = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            await _testClass.DeleteDocumentCt(da);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallDeleteDocumentCtWithNullDa()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.DeleteDocumentCt(default(DocumentCT)));
        }

        [Test]
        public async Task CanCallLinkExistingPreviousItems()
        {
            var da = new DocumentCT { Document = new xcuda_ASYCUDA(), DocumentItems = new List<xcuda_Item>(), EntryDataDetails = new List<EntryDataDetails>(), EmailIds = new List<int?>() };
            await _testClass.LinkExistingPreviousItems(da);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallLinkExistingPreviousItemsWithNullDa()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.LinkExistingPreviousItems(default(DocumentCT)));
        }

        [Test]
        public async Task CanCallSaveEntryPreviousItems()
        {
            var epi = new List<EntryPreviousItems>();
            await _testClass.SaveEntryPreviousItems(epi);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveEntryPreviousItemsWithNullEpi()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveEntryPreviousItems(default(List<EntryPreviousItems>)));
        }

        [Test]
        public async Task CanCallDeleteAsycudaDocumentSet()
        {
            var docSetId = 694220224;
            await _testClass.DeleteAsycudaDocumentSet(docSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public async Task CanCallSaveAttachedDocuments()
        {
            var csvFiles = new[] { new FileInfo("TestValue2108615605"), new FileInfo("TestValue2039921208"), new FileInfo("TestValue822942152") };
            var fileType = new FileTypes();
            await _testClass.SaveAttachedDocuments(csvFiles, fileType);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveAttachedDocumentsWithNullCsvFiles()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveAttachedDocuments(default(FileInfo[]), new FileTypes()));
        }

        [Test]
        public void CannotCallSaveAttachedDocumentsWithNullFileType()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveAttachedDocuments(new[] { new FileInfo("TestValue1059979162"), new FileInfo("TestValue678454720"), new FileInfo("TestValue596971467") }, default(FileTypes)));
        }

        [Test]
        public void CanCallStripAttachments()
        {
            var doclst = new List<DocumentCT>();
            var emailId = "TestValue866969724";
            BaseDataModel.StripAttachments(doclst, emailId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallStripAttachmentsWithNullDoclst()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.StripAttachments(default(List<DocumentCT>), "TestValue1151681958"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallStripAttachmentsWithInvalidEmailId(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.StripAttachments(new List<DocumentCT>(), value));
        }

        [Test]
        public void CanCallAttachToExistingDocuments()
        {
            var asycudaDocumentSetId = 131349299;
            _testClass.AttachToExistingDocuments(asycudaDocumentSetId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallAttachEmailPDF()
        {
            var asycudaDocumentSetId = 962092764;
            var emailId = "TestValue2002957514";
            BaseDataModel.AttachEmailPDF(asycudaDocumentSetId, emailId);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallAttachEmailPDFWithInvalidEmailId(string value)
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.AttachEmailPDF(313616808, value));
        }

        [Test]
        public void CanCallAttachBlankC71()
        {
            var docList = new List<DocumentCT>();
            BaseDataModel.AttachBlankC71(docList);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAttachBlankC71WithNullDocList()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.AttachBlankC71(default(List<DocumentCT>)));
        }

        [Test]
        public void CanCallSetInvoicePerline()
        {
            var docList = new List<int>();
            BaseDataModel.SetInvoicePerline(docList);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSetInvoicePerlineWithNullDocList()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.SetInvoicePerline(default(List<int>)));
        }

        [Test]
        public void CanCallRenameDuplicateDocumentCodes()
        {
            var docList = new List<int>();
            BaseDataModel.RenameDuplicateDocumentCodes(docList);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallRenameDuplicateDocumentCodesWithNullDocList()
        {
            Assert.Throws<ArgumentNullException>(() => BaseDataModel.RenameDuplicateDocumentCodes(default(List<int>)));
        }

        [Test]
        public void CanCallRenameDuplicateDocuments()
        {
            var docKey = 345590223;
            BaseDataModel.RenameDuplicateDocuments(docKey);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanCallGetFileTypeWithOcrInvoicesFileTypeId()
        {
            var ocrInvoicesFileTypeId = 1743486990;
            var result = BaseDataModel.GetFileType(ocrInvoicesFileTypeId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(BaseDataModel.Instance, Is.InstanceOf<BaseDataModel>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInventoryCache()
        {
            Assert.That(_testClass.InventoryCache, Is.InstanceOf<DataCache<InventoryItem>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCustoms_ProcedureCache()
        {
            Assert.That(_testClass.Customs_ProcedureCache, Is.InstanceOf<DataCache<Customs_Procedure>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetDocument_TypeCache()
        {
            Assert.That(_testClass.Document_TypeCache, Is.InstanceOf<DataCache<Document_Type>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetCurrentApplicationSettings()
        {
            var testValue = new ApplicationSettings();
            _testClass.CurrentApplicationSettings = testValue;
            Assert.That(_testClass.CurrentApplicationSettings, Is.EqualTo(testValue));
        }

        [Test]
        public void CanGetExportTemplates()
        {
            Assert.That(_testClass.ExportTemplates, Is.InstanceOf<IEnumerable<ExportTemplate>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetCustoms_Procedures()
        {
            Assert.That(_testClass.Customs_Procedures, Is.InstanceOf<IEnumerable<Customs_Procedure>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetDocument_Types()
        {
            Assert.That(_testClass.Document_Types, Is.InstanceOf<IEnumerable<Document_Type>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetInitialization()
        {
            Assert.That(BaseDataModel.Initialization, Is.InstanceOf<Task>());
            Assert.Fail("Create or modify test");
        }
    }

    [TestFixture]
    public class LicenceDocItemTests
    {
        private LicenceDocItem _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new LicenceDocItem();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new LicenceDocItem();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetItem_Id()
        {
            var testValue = 476100188;
            _testClass.Item_Id = testValue;
            Assert.That(_testClass.Item_Id, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDetails()
        {
            var testValue = new List<string>();
            _testClass.Details = testValue;
            Assert.That(_testClass.Details, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTariffCode()
        {
            var testValue = "TestValue2060541811";
            _testClass.TariffCode = testValue;
            Assert.That(_testClass.TariffCode, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemQuantity()
        {
            var testValue = 283173993.63;
            _testClass.ItemQuantity = testValue;
            Assert.That(_testClass.ItemQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAsycudaDocumentId()
        {
            var testValue = 1710819622;
            _testClass.AsycudaDocumentId = testValue;
            Assert.That(_testClass.AsycudaDocumentId, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class SaleReportLineTests
    {
        private SaleReportLine _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SaleReportLine();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new SaleReportLine();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetLine()
        {
            var testValue = 1216852537;
            _testClass.Line = testValue;
            Assert.That(_testClass.Line, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDate()
        {
            var testValue = new DateTime(1181819821);
            _testClass.Date = testValue;
            Assert.That(_testClass.Date, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetInvoiceNo()
        {
            var testValue = "TestValue1646426972";
            _testClass.InvoiceNo = testValue;
            Assert.That(_testClass.InvoiceNo, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCustomerName()
        {
            var testValue = "TestValue498613081";
            _testClass.CustomerName = testValue;
            Assert.That(_testClass.CustomerName, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemNumber()
        {
            var testValue = "TestValue926927863";
            _testClass.ItemNumber = testValue;
            Assert.That(_testClass.ItemNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemDescription()
        {
            var testValue = "TestValue1526933858";
            _testClass.ItemDescription = testValue;
            Assert.That(_testClass.ItemDescription, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTariffCode()
        {
            var testValue = "TestValue1107620262";
            _testClass.TariffCode = testValue;
            Assert.That(_testClass.TariffCode, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetQuantity()
        {
            var testValue = 140051505.33;
            _testClass.Quantity = testValue;
            Assert.That(_testClass.Quantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPrice()
        {
            var testValue = 346787515.8;
            _testClass.Price = testValue;
            Assert.That(_testClass.Price, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSalesType()
        {
            var testValue = "TestValue256377835";
            _testClass.SalesType = testValue;
            Assert.That(_testClass.SalesType, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetGrossSales()
        {
            var testValue = 1723499909.01;
            _testClass.GrossSales = testValue;
            Assert.That(_testClass.GrossSales, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPreviousCNumber()
        {
            var testValue = "TestValue1228945090";
            _testClass.PreviousCNumber = testValue;
            Assert.That(_testClass.PreviousCNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPreviousLineNumber()
        {
            var testValue = "TestValue1768888057";
            _testClass.PreviousLineNumber = testValue;
            Assert.That(_testClass.PreviousLineNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPreviousRegDate()
        {
            var testValue = "TestValue1636445620";
            _testClass.PreviousRegDate = testValue;
            Assert.That(_testClass.PreviousRegDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCIFValue()
        {
            var testValue = 933587970.48;
            _testClass.CIFValue = testValue;
            Assert.That(_testClass.CIFValue, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDutyLiablity()
        {
            var testValue = 932646468.6;
            _testClass.DutyLiablity = testValue;
            Assert.That(_testClass.DutyLiablity, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class EntryDataDetailSummaryTests
    {
        private EntryDataDetailSummary _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EntryDataDetailSummary();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EntryDataDetailSummary();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetEntryData_Id()
        {
            var testValue = 1692755510;
            _testClass.EntryData_Id = testValue;
            Assert.That(_testClass.EntryData_Id, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEntryDataId()
        {
            var testValue = "TestValue473774722";
            _testClass.EntryDataId = testValue;
            Assert.That(_testClass.EntryDataId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEntryDataDetailsId()
        {
            var testValue = 1442063110;
            _testClass.EntryDataDetailsId = testValue;
            Assert.That(_testClass.EntryDataDetailsId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEntryDataDate()
        {
            var testValue = new DateTime(584001056);
            _testClass.EntryDataDate = testValue;
            Assert.That(_testClass.EntryDataDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetQtyAllocated()
        {
            var testValue = 153042525.9;
            _testClass.QtyAllocated = testValue;
            Assert.That(_testClass.QtyAllocated, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetEffectiveDate()
        {
            var testValue = new DateTime(971644732);
            _testClass.EffectiveDate = testValue;
            Assert.That(_testClass.EffectiveDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCurrency()
        {
            var testValue = "TestValue1478253558";
            _testClass.Currency = testValue;
            Assert.That(_testClass.Currency, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetLineNumber()
        {
            var testValue = 254458815;
            _testClass.LineNumber = testValue;
            Assert.That(_testClass.LineNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetComment()
        {
            var testValue = "TestValue958855026";
            _testClass.Comment = testValue;
            Assert.That(_testClass.Comment, Is.EqualTo(testValue));
        }
    }
}