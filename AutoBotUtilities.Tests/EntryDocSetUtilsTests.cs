using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class EntryDocSetUtilsTests
    {
        private EntryDocSetUtils _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new EntryDocSetUtils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EntryDocSetUtils();
            Assert.That(instance, Is.Not.Null);
        }



        [Test]
        public void CanCallGetRelatedDocuments()
        {
            var docSet = EntryDocSetUtils.GetDocSet("xSales");
            var res = EntryDocSetUtils.GetRelatedDocuments(docSet);
            Assert.IsTrue(res.First().First().xcuda_ASYCUDA_ExtendedProperties != null);
            

        }


    }
}