using System.IO;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils;
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
    public class AllocationsTests
    {
        private AllocationsBaseModel _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(2);
            Infrastructure.Utils.ClearDataBase();
            _testClass = new AllocationsBaseModel();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new AllocationsBaseModel();
            Assert.That(instance, Is.Not.Null);
        }



       
        [Test]
        public void CreateItemSets()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                Assert.That(_testClass.CreateItemSets(2, null).Any(), Is.True);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

       [Test]
       [Timeout(3*1000*60)]
        public void AllocatSales1()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                 _testClass.AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(2, false, "TOH/MTSX018S").Wait();
                timer.Stop();
                Console.Write("AllocatSales1 in seconds: " + timer.Elapsed.Seconds);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }
      
    }
}