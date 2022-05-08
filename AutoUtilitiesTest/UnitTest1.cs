using AutoBot;
using NUnit.Framework;

namespace AutoUtilitiesTest
{
    [TestFixture]
    public class Ex9UtilsTests
    {
        private EX9Utils _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EX9Utils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EX9Utils();
            Assert.That(instance, Is.Not.Null);
        }



        [Test]
        public void CanGetXSalesFileType()
        {
            var type = "XSales";
            var fileType = Utils.GetFileType(type);
            Assert.AreEqual(type, fileType.Type);
        }



    }
}