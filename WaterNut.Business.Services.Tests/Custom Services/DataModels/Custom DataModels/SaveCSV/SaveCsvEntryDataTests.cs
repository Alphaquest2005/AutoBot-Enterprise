namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using NSubstitute;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using DocumentDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class SaveCsvEntryDataTests
    {
        private SaveCsvEntryData _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new SaveCsvEntryData();
        }

        [Test]
        public async Task CanCallExtractEntryData()
        {
            var fileType = new FileTypes();
            var lines = new[] { "TestValue1734014059", "TestValue409839050", "TestValue1583354563" };
            var headings = new[] { "TestValue607345857", "TestValue1475970347", "TestValue736704375" };
            var docSet = new List<AsycudaDocumentSet>();
            var overWriteExisting = true;
            var emailId = 1572154213;
            var droppedFilePath = "TestValue1119327649";
            var result = await _testClass.ExtractEntryData(fileType, lines, headings, docSet, overWriteExisting, emailId, droppedFilePath);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallExtractEntryDataWithNullFileType()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExtractEntryData(default(FileTypes), new[] { "TestValue1305489054", "TestValue1218209861", "TestValue1462757996" }, new[] { "TestValue1783800221", "TestValue1590313099", "TestValue693101600" }, new List<AsycudaDocumentSet>(), true, 432550745, "TestValue571431535"));
        }

        [Test]
        public void CannotCallExtractEntryDataWithNullLines()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExtractEntryData(new FileTypes(), default(string[]), new[] { "TestValue470214483", "TestValue699943430", "TestValue485326776" }, new List<AsycudaDocumentSet>(), true, 1444692023, "TestValue370636555"));
        }

        [Test]
        public void CannotCallExtractEntryDataWithNullHeadings()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExtractEntryData(new FileTypes(), new[] { "TestValue50719920", "TestValue160851161", "TestValue1090874877" }, default(string[]), new List<AsycudaDocumentSet>(), false, 997132477, "TestValue1146895042"));
        }

        [Test]
        public void CannotCallExtractEntryDataWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExtractEntryData(new FileTypes(), new[] { "TestValue1263724213", "TestValue926141354", "TestValue1681526944" }, new[] { "TestValue573446436", "TestValue703495143", "TestValue1753348758" }, default(List<AsycudaDocumentSet>), true, 666476381, "TestValue1416045699"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallExtractEntryDataWithInvalidDroppedFilePath(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ExtractEntryData(new FileTypes(), new[] { "TestValue2017942190", "TestValue1184490017", "TestValue1691652193" }, new[] { "TestValue325487621", "TestValue1911450204", "TestValue156963395" }, new List<AsycudaDocumentSet>(), false, 499148296, value));
        }

        [Test]
        public async Task CanCallProcessCsvSummaryData()
        {
            var fileType = new FileTypes();
            var docSet = new List<AsycudaDocumentSet>();
            var overWriteExisting = true;
            var emailId = 790510731;
            var droppedFilePath = "TestValue1419462605";
            var eslst = new List<dynamic>();
            var result = await _testClass.ProcessCsvSummaryData(fileType, docSet, overWriteExisting, emailId, droppedFilePath, eslst);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallProcessCsvSummaryDataWithNullFileType()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessCsvSummaryData(default(FileTypes), new List<AsycudaDocumentSet>(), false, 1461840315, "TestValue275060410", new List<dynamic>()));
        }

        [Test]
        public void CannotCallProcessCsvSummaryDataWithNullDocSet()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessCsvSummaryData(new FileTypes(), default(List<AsycudaDocumentSet>), true, 1708124419, "TestValue637207546", new List<dynamic>()));
        }

        [Test]
        public void CannotCallProcessCsvSummaryDataWithNullEslst()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessCsvSummaryData(new FileTypes(), new List<AsycudaDocumentSet>(), false, 1212718008, "TestValue556625937", default(List<dynamic>)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallProcessCsvSummaryDataWithInvalidDroppedFilePath(string value)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.ProcessCsvSummaryData(new FileTypes(), new List<AsycudaDocumentSet>(), false, 1793313145, value, new List<dynamic>()));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(SaveCsvEntryData.Instance, Is.InstanceOf<SaveCsvEntryData>());
            Assert.Fail("Create or modify test");
        }
    }

    [TestFixture]
    public class ImportDataTests
    {
        private ImportData _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new ImportData();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ImportData();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetres()
        {
            var testValue = Substitute.For<IDictionary<string, object>>();
            _testClass.res = testValue;
            Assert.That(_testClass.res, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetmapping()
        {
            var testValue = new Dictionary<string, int>();
            _testClass.mapping = testValue;
            Assert.That(_testClass.mapping, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetsplits()
        {
            var testValue = new[] { "TestValue1264952696", "TestValue575021736", "TestValue2119916254" };
            _testClass.splits = testValue;
            Assert.That(_testClass.splits, Is.EqualTo(testValue));
        }
    }
}