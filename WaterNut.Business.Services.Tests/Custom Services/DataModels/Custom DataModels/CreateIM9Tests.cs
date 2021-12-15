namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class CreateIM9Tests
    {
        private CreateIM9 _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new CreateIM9();
        }

        [Test]
        public void CanGetInstance()
        {
            Assert.That(CreateIM9.Instance, Is.InstanceOf<CreateIM9>());
            Assert.Fail("Create or modify test");
        }
    }

    [TestFixture]
    public class AsycudaDocumentItemIM9Tests
    {
        private AsycudaDocumentItemIM9 _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new AsycudaDocumentItemIM9();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new AsycudaDocumentItemIM9();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanSetAndGetLineNumber()
        {
            var testValue = 1453984865;
            _testClass.LineNumber = testValue;
            Assert.That(_testClass.LineNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetRegistrationDate()
        {
            var testValue = new DateTime(436211760);
            _testClass.RegistrationDate = testValue;
            Assert.That(_testClass.RegistrationDate, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetAsycudaDocumentId()
        {
            var testValue = 273844604;
            _testClass.AsycudaDocumentId = testValue;
            Assert.That(_testClass.AsycudaDocumentId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemNumber()
        {
            var testValue = "TestValue1834939009";
            _testClass.ItemNumber = testValue;
            Assert.That(_testClass.ItemNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemQuantity()
        {
            var testValue = 483205445.90999997;
            _testClass.ItemQuantity = testValue;
            Assert.That(_testClass.ItemQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPiQuantity()
        {
            var testValue = 933914975.4;
            _testClass.PiQuantity = testValue;
            Assert.That(_testClass.PiQuantity, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTariffCode()
        {
            var testValue = "TestValue1629254169";
            _testClass.TariffCode = testValue;
            Assert.That(_testClass.TariffCode, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNumber_of_packages()
        {
            var testValue = 325981005;
            _testClass.Number_of_packages = testValue;
            Assert.That(_testClass.Number_of_packages, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetNet_weight()
        {
            var testValue = 279375819.03;
            _testClass.Net_weight = testValue;
            Assert.That(_testClass.Net_weight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCNumber()
        {
            var testValue = "TestValue829850427";
            _testClass.CNumber = testValue;
            Assert.That(_testClass.CNumber, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCustoms_clearance_office_code()
        {
            var testValue = "TestValue1164864009";
            _testClass.Customs_clearance_office_code = testValue;
            Assert.That(_testClass.Customs_clearance_office_code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCommercial_Description()
        {
            var testValue = "TestValue183584746";
            _testClass.Commercial_Description = testValue;
            Assert.That(_testClass.Commercial_Description, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItem_price()
        {
            var testValue = 349946839.44;
            _testClass.Item_price = testValue;
            Assert.That(_testClass.Item_price, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSuppplementary_unit_code()
        {
            var testValue = "TestValue681649320";
            _testClass.Suppplementary_unit_code = testValue;
            Assert.That(_testClass.Suppplementary_unit_code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetItemId()
        {
            var testValue = 2057510122;
            _testClass.ItemId = testValue;
            Assert.That(_testClass.ItemId, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetPiWeight()
        {
            var testValue = 1248545108.25;
            _testClass.PiWeight = testValue;
            Assert.That(_testClass.PiWeight, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetCountry_of_origin_code()
        {
            var testValue = "TestValue288273677";
            _testClass.Country_of_origin_code = testValue;
            Assert.That(_testClass.Country_of_origin_code, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetTotal_CIF_itm()
        {
            var testValue = 1154564070.66;
            _testClass.Total_CIF_itm = testValue;
            Assert.That(_testClass.Total_CIF_itm, Is.EqualTo(testValue));
        }

        [Test]
        public void CanSetAndGetSalesFactor()
        {
            var testValue = 1869389441.37;
            _testClass.SalesFactor = testValue;
            Assert.That(_testClass.SalesFactor, Is.EqualTo(testValue));
        }
    }
}