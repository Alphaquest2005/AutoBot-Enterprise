namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using DocumentDS.Business.Entities;
    using System.Threading.Tasks;

    [TestFixture]
    public class CreateAsycudaDocumentModelTests
    {
        private CreateAsycudaDocumentModel _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CreateAsycudaDocumentModel();
        }

        [Test]
        public async Task CanCallSaveAsycudaDocument()
        {
            var doc = new xcuda_ASYCUDA();
            var docInfo = new DocInfo { Currency_Code = "TestValue655327868", DeclarantReferenceNumber = "TestValue151277513", Customs_ProcedureId = 371959915, Description = "TestValue1195276899", Document_TypeId = 1610002524, Exchange_Rate = 1101863411.55, Country_of_origin_code = "TestValue1443491941", ManifestNumber = "TestValue2099842547", BlNumber = "TestValue536988553", TotalGrossWeight = 244214236.53M, TotalFreight = 604485970.11M };
            await _testClass.SaveAsycudaDocument(doc, docInfo);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallSaveAsycudaDocumentWithNullDoc()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveAsycudaDocument(default(xcuda_ASYCUDA), new DocInfo { Currency_Code = "TestValue1558256606", DeclarantReferenceNumber = "TestValue1049887856", Customs_ProcedureId = 1340688354, Description = "TestValue366511894", Document_TypeId = 1518082303, Exchange_Rate = 1535496662.7, Country_of_origin_code = "TestValue50731480", ManifestNumber = "TestValue2325466", BlNumber = "TestValue618478701", TotalGrossWeight = 1831021758.72M, TotalFreight = 1540668899.88M }));
        }

        [Test]
        public void CannotCallSaveAsycudaDocumentWithNullDocInfo()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _testClass.SaveAsycudaDocument(new xcuda_ASYCUDA(), default(DocInfo)));
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CreateAsycudaDocumentModel.Instance, Is.InstanceOf<CreateAsycudaDocumentModel>());
            Assert.Fail("Create or modify test");
        }
    }

    [TestFixture]
    public class DocInfoTests
    {
        private DocInfo _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new DocInfo();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new DocInfo();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetCurrency_Code()
        {
            var testValue = "TestValue245207805";
            _testClass.Currency_Code = testValue;
            Assert.That(_testClass.Currency_Code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDeclarantReferenceNumber()
        {
            var testValue = "TestValue1851049910";
            _testClass.DeclarantReferenceNumber = testValue;
            Assert.That(_testClass.DeclarantReferenceNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCustoms_ProcedureId()
        {
            var testValue = 1391968157;
            _testClass.Customs_ProcedureId = testValue;
            Assert.That(_testClass.Customs_ProcedureId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDescription()
        {
            var testValue = "TestValue1687241702";
            _testClass.Description = testValue;
            Assert.That(_testClass.Description, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetDocument_TypeId()
        {
            var testValue = 2026205712;
            _testClass.Document_TypeId = testValue;
            Assert.That(_testClass.Document_TypeId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetExchange_Rate()
        {
            var testValue = 163986302.7;
            _testClass.Exchange_Rate = testValue;
            Assert.That(_testClass.Exchange_Rate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCountry_of_origin_code()
        {
            var testValue = "TestValue2118650234";
            _testClass.Country_of_origin_code = testValue;
            Assert.That(_testClass.Country_of_origin_code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetManifestNumber()
        {
            var testValue = "TestValue55564363";
            _testClass.ManifestNumber = testValue;
            Assert.That(_testClass.ManifestNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetBlNumber()
        {
            var testValue = "TestValue1437712516";
            _testClass.BlNumber = testValue;
            Assert.That(_testClass.BlNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTotalGrossWeight()
        {
            var testValue = 822310055.82M;
            _testClass.TotalGrossWeight = testValue;
            Assert.That(_testClass.TotalGrossWeight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTotalFreight()
        {
            var testValue = 1321507143M;
            _testClass.TotalFreight = testValue;
            Assert.That(_testClass.TotalFreight, Is.EqualTo(testValue));
        }
    }
}