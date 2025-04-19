using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AdjustmentQS.Business.Services;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    using AutoBot;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class ADJUtilsTests
    {
        private ADJUtils _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(2);
            _testClass = new ADJUtils();

        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ADJUtils();
            Assert.That(instance, Is.Not.Null);
        }

        

        //[Test]
        //public void CanCallCreateAdjustmentEntriesWithOverwriteAndAdjustmentType()
        //{
        //    var overwrite = true;
        //    var adjustmentType = "DIS";
        //    ADJUtils.CreateAdjustmentEntries(overwrite, adjustmentType);
        //    Assert.Fail("Create or modify test");
        //}


     
        
    }
}