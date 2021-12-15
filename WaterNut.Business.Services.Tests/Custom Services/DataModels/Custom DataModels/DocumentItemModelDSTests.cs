namespace WaterNut.Business.Services.Tests
{
    using WaterNut.DataSpace;
    using System;
    using NUnit.Framework;
    using DocumentItemDS.Business.Entities;

    [TestFixture]
    public class DocumentItemModelDSTests
    {
        private DocumentItemModelDS _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new DocumentItemModelDS();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new DocumentItemModelDS();
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void CanCallAddFreeText()
        {
            var itmcnt = 766473406;
            var itm = new xcuda_Item();
            var entryDataId = "TestValue1126752468";
            var result = DocumentItemModelDS.AddFreeText(itmcnt, itm, entryDataId);
            Assert.Fail("Create or modify test");
        }

        [Test]
        public void CannotCallAddFreeTextWithNullItm()
        {
            Assert.Throws<ArgumentNullException>(() => DocumentItemModelDS.AddFreeText(1545947404, default(xcuda_Item), "TestValue699561032"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallAddFreeTextWithInvalidEntryDataId(string value)
        {
            Assert.Throws<ArgumentNullException>(() => DocumentItemModelDS.AddFreeText(393227632, new xcuda_Item(), value));
        }
    }
}