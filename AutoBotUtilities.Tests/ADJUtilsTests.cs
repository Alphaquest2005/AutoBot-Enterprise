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
            _testClass = new ADJUtils();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new ADJUtils();
            Assert.That(instance, Is.Not.Null);
        }

        

        [Test]
        public void CanCallCreateAdjustmentEntriesWithOverwriteAndAdjustmentType()
        {
            var overwrite = true;
            var adjustmentType = "DIS";
            ADJUtils.CreateAdjustmentEntries(overwrite, adjustmentType);
            Assert.Fail("Create or modify test");
        }

        
    }
}