namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using DocumentDS.Business.Entities;
    using CoreEntities.Business.Entities;
    using EmailDownloader;
    using System.Linq.Expressions;
    using OCR.Business.Entities;
    using System.Text;

    [TestFixture]
    public class InvoiceReaderTests
    {
        private InvoiceReader _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new InvoiceReader();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new InvoiceReader();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanCallImport()
        {
            var file = "TestValue372656811";
            var fileTypeId = 749838961;
            var emailId = 1993795316;
            var overWriteExisting = false;
            var docSet = new List<AsycudaDocumentSet>();
            var fileType = new FileTypes();
            var client = new Client { Email = "TestValue379903248", Password = "TestValue170130369", DataFolder = "TestValue1227406271", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue981876694" };
            var result = InvoiceReader.Import(file, fileTypeId, emailId, overWriteExisting, docSet, fileType, client);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallImportWithNullDocSet()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.Import("TestValue511298729", 2088727355, 1681766505, false, default(List<AsycudaDocumentSet>), new FileTypes(), new Client { Email = "TestValue3549110", Password = "TestValue1501012959", DataFolder = "TestValue231540783", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue66059323" }));
        }

        [Test]
        public void CannotCallImportWithNullFileType()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.Import("TestValue1685299064", 607500628, 1439762873, true, new List<AsycudaDocumentSet>(), default(FileTypes), new Client { Email = "TestValue578487776", Password = "TestValue1743637421", DataFolder = "TestValue1715641881", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue612963920" }));
        }

        [Test]
        public void CannotCallImportWithNullClient()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.Import("TestValue687511459", 1456942939, 875380847, true, new List<AsycudaDocumentSet>(), new FileTypes(), default(Client)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallImportWithInvalidFile(string value)
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.Import(value, 94858677, 203780906, false, new List<AsycudaDocumentSet>(), new FileTypes(), new Client { Email = "TestValue417570096", Password = "TestValue119547420", DataFolder = "TestValue270694103", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue792207198" }));
        }

        [Test]
        public void CanCallGetTemplates()
        {
            var filter = new Expression<Func<Invoices, bool>>();
            var result = InvoiceReader.GetTemplates(filter);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallGetTemplatesWithNullFilter()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.GetTemplates(default(Expression<Func<Invoices, bool>>)));
        }

        [Test]
        public void CanCallTryReadFile()
        {
            var file = "TestValue1581229432";
            var emailId = 507169987;
            var fileType = new FileTypes();
            var pdftxt = new StringBuilder();
            var client = new Client { Email = "TestValue369335130", Password = "TestValue1677606874", DataFolder = "TestValue1091026306", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue1208351053" };
            var overWriteExisting = false;
            var docSet = new List<AsycudaDocumentSet>();
            var tmp = new Invoice(new Invoices());
            var fileTypeId = 1376973434;
            var result = InvoiceReader.TryReadFile(file, emailId, fileType, pdftxt, client, overWriteExisting, docSet, tmp, fileTypeId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallTryReadFileWithNullFileType()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.TryReadFile("TestValue2117315717", 1986962028, default(FileTypes), new StringBuilder(), new Client { Email = "TestValue391195428", Password = "TestValue1123203358", DataFolder = "TestValue44934445", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue468020249" }, true, new List<AsycudaDocumentSet>(), new Invoice(new Invoices()), 1201752137));
        }

        [Test]
        public void CannotCallTryReadFileWithNullPdftxt()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.TryReadFile("TestValue1822652294", 616131683, new FileTypes(), default(StringBuilder), new Client { Email = "TestValue1647617083", Password = "TestValue2128152195", DataFolder = "TestValue776503037", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue1181680727" }, false, new List<AsycudaDocumentSet>(), new Invoice(new Invoices()), 1772286378));
        }

        [Test]
        public void CannotCallTryReadFileWithNullClient()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.TryReadFile("TestValue1070443956", 1442233169, new FileTypes(), new StringBuilder(), default(Client), false, new List<AsycudaDocumentSet>(), new Invoice(new Invoices()), 887018017));
        }

        [Test]
        public void CannotCallTryReadFileWithNullDocSet()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.TryReadFile("TestValue307517823", 742142461, new FileTypes(), new StringBuilder(), new Client { Email = "TestValue1264196409", Password = "TestValue255730558", DataFolder = "TestValue1880338654", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue708805761" }, false, default(List<AsycudaDocumentSet>), new Invoice(new Invoices()), 1706372983));
        }

        [Test]
        public void CannotCallTryReadFileWithNullTmp()
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.TryReadFile("TestValue1315963934", 1077377401, new FileTypes(), new StringBuilder(), new Client { Email = "TestValue348736567", Password = "TestValue1601649535", DataFolder = "TestValue394138136", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue366663987" }, true, new List<AsycudaDocumentSet>(), default(Invoice), 773485539));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallTryReadFileWithInvalidFile(string value)
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.TryReadFile(value, 296316031, new FileTypes(), new StringBuilder(), new Client { Email = "TestValue333739581", Password = "TestValue830446402", DataFolder = "TestValue667357728", EmailMappings = new List<EmailMapping>(), CompanyName = "TestValue1915174015" }, false, new List<AsycudaDocumentSet>(), new Invoice(new Invoices()), 1671416247));
        }

        [Test]
        public void CanCallGetPdftxt()
        {
            var file = "TestValue1948922060";
            var result = InvoiceReader.GetPdftxt(file);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetPdftxtWithInvalidFile(string value)
        {
            Assert.Throws<ArgumentNullException>(() => InvoiceReader.GetPdftxt(value));
        }
    }

    [TestFixture]
    public class InvoiceTests
    {
        private Invoice _testClass;
        private Invoices _ocrInvoices;

        [SetUp]
        public void SetUp()
        {
            _ocrInvoices = new Invoices();
            _testClass = new Invoice(_ocrInvoices);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new Invoice(_ocrInvoices);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullOcrInvoices()
        {
            Assert.Throws<ArgumentNullException>(() => new Invoice(default(Invoices)));
        }

        [Test]
        public void CanCallRead()
        {
            var text = new List<string>();
            var result = _testClass.Read(text);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallReadWithNullText()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Read(default(List<string>)));
        }

        [Test]
        public void ReadPerformsMapping()
        {
            var text = new List<string>();
            var result = _testClass.Read(text);
            Assert.That(result.Capacity, Is.EqualTo(text.Capacity));
            Assert.That(result.Count, Is.EqualTo(text.Count));
            Assert.That(result.System.Collections.IList.IsFixedSize, Is.EqualTo(text.System.Collections.IList.IsFixedSize));
            Assert.That(result.System.Collections.Generic.ICollection<T>.IsReadOnly, Is.EqualTo(text.System.Collections.Generic.ICollection<T>.IsReadOnly));
            Assert.That(result.System.Collections.IList.IsReadOnly, Is.EqualTo(text.System.Collections.IList.IsReadOnly));
            Assert.That(result.System.Collections.ICollection.IsSynchronized, Is.EqualTo(text.System.Collections.ICollection.IsSynchronized));
            Assert.That(result.System.Collections.ICollection.SyncRoot, Is.EqualTo(text.System.Collections.ICollection.SyncRoot));
            Assert.That(result.System.Collections.IList.Item, Is.EqualTo(text.System.Collections.IList.Item));
        }

        [Test]
        public void CanCallFormat()
        {
            var pdftxt = "TestValue619413984";
            var result = _testClass.Format(pdftxt);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallFormatWithInvalidPdftxt(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Format(value));
        }

        [Test]
        public void OcrInvoicesIsInitializedCorrectly()
        {
            Assert.That(_testClass.OcrInvoices, Is.EqualTo(_ocrInvoices));
        }

        [Test]
        public void CanSetAndGetParts()
        {
            var testValue = new List<Part>();
            _testClass.Parts = testValue;
            Assert.That(_testClass.Parts, Is.EqualTo(testValue));
        }

        [Test]
        public void CanGetSuccess()
        {
            Assert.That(_testClass.Success, Is.InstanceOf<bool>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetLines()
        {
            Assert.That(_testClass.Lines, Is.InstanceOf<List<Line>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanSetAndGetMaxLinesCheckedToStart()
        {
            var testValue = 287125299.45;
            _testClass.MaxLinesCheckedToStart = testValue;
            Assert.That(_testClass.MaxLinesCheckedToStart, Is.EqualTo(testValue));
        }
    }

    [TestFixture]
    public class InvoiceLineTests
    {
        private InvoiceLine _testClass;
        private string _line;
        private int _lineNumber;

        [SetUp]
        public void SetUp()
        {
            _line = "TestValue15704161";
            _lineNumber = 399548705;
            _testClass = new InvoiceLine(_line, _lineNumber);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new InvoiceLine(_line, _lineNumber);
            Assert.That(instance, Is.Not.Null);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotConstructWithInvalidLine(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new InvoiceLine(value, 1273822238));
        }

        [Test]
        public void LineIsInitializedCorrectly()
        {
            Assert.That(_testClass.Line, Is.EqualTo(_line));
        }

        [Test]
        public void LineNumberIsInitializedCorrectly()
        {
            Assert.That(_testClass.LineNumber, Is.EqualTo(_lineNumber));
        }
    }

    [TestFixture]
    public class PartTests
    {
        private Part _testClass;
        private Parts _part;

        [SetUp]
        public void SetUp()
        {
            _part = new Parts();
            _testClass = new Part(_part);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new Part(_part);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullPart()
        {
            Assert.Throws<ArgumentNullException>(() => new Part(default(Parts)));
        }

        [Test]
        public void CanCallRead()
        {
            var newlines = new List<InvoiceLine>();
            _testClass.Read(newlines);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallReadWithNullNewlines()
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Read(default(List<InvoiceLine>)));
        }

        [Test]
        public void CanGetChildParts()
        {
            Assert.That(_testClass.ChildParts, Is.InstanceOf<List<Part>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetLines()
        {
            Assert.That(_testClass.Lines, Is.InstanceOf<List<Line>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetSuccess()
        {
            Assert.That(_testClass.Success, Is.InstanceOf<bool>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetFailedLines()
        {
            Assert.That(_testClass.FailedLines, Is.InstanceOf<List<Line>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetFailedFields()
        {
            Assert.That(_testClass.FailedFields, Is.InstanceOf<List<Dictionary<string, List<KeyValuePair<Fields, string>>>>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetAllLines()
        {
            Assert.That(_testClass.AllLines, Is.InstanceOf<List<Line>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetWasStarted()
        {
            Assert.That(_testClass.WasStarted, Is.InstanceOf<bool>());
            Assert.Fail("Create or modify test");
        }
    }

    [TestFixture]
    public class LineTests
    {
        private Line _testClass;
        private Lines _lines;

        [SetUp]
        public void SetUp()
        {
            _lines = new Lines();
            _testClass = new Line(_lines);
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new Line(_lines);
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CannotConstructWithNullLines()
        {
            Assert.Throws<ArgumentNullException>(() => new Line(default(Lines)));
        }

        [Test]
        public void CanCallRead()
        {
            var line = "TestValue744532399";
            var lineNumber = 607162918;
            var result = _testClass.Read(line, lineNumber);
            Assert.Fail("Create or modify test");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallReadWithInvalidLine(string value)
        {
            Assert.Throws<ArgumentNullException>(() => _testClass.Read(value, 1796276784));
        }

        [Test]
        public void CanGetOCR_Lines()
        {
            Assert.That(_testClass.OCR_Lines, Is.InstanceOf<Lines>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetValues()
        {
            Assert.That(_testClass.Values, Is.InstanceOf<Dictionary<int, Dictionary<Fields, string>>>());
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CanGetFailedFields()
        {
            Assert.That(_testClass.FailedFields, Is.InstanceOf<List<Dictionary<string, List<KeyValuePair<Fields, string>>>>>());
            Assert.Fail("Create or modify test");
        }
    }
}