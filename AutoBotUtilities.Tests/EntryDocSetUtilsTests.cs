using System.Threading.Tasks;
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
            Infrastructure.Utils.SetTestApplicationSettings(2);
            _testClass = new EntryDocSetUtils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new EntryDocSetUtils();
            Assert.That(instance, Is.Not.Null);
        }



        [Test]
        public async Task CanCallGetRelatedDocuments()
        {
            var docSets =await EntryDocSetUtils.GetLatestModifiedDocSet().ConfigureAwait(false);
            foreach (var set in docSets)
            {
                var res = await EntryDocSetUtils.GetRelatedDocuments(set).ConfigureAwait(false);
                Assert.That(res.First().First().xcuda_ASYCUDA_ExtendedProperties != null);
            }
        }


    }
}