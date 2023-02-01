using System.IO;
using AllocationDS.Business.Entities;
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
        public void AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber()
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


        [Test]
        [Timeout(3 * 1000 * 60)]
        [TestCase("TOH/MTSX018S")]
        public void AllocatSales(string itemNumber)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                _testClass.AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false, itemNumber).Wait();
                timer.Stop();
                Console.Write("AllocatSales1 in seconds: " + timer.Elapsed.Seconds);

                using (var ctx = new AllocationDSContext())
                {
                    var overallocatedSales = ctx.EntryDataDetails
                        .AsNoTracking()
                        .Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated > x.Quantity);
                    var unallocatedSales = ctx.EntryDataDetails.AsNoTracking()
                        .Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated == 0);

                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(overallocatedSales, 0);
                        Assert.AreEqual(unallocatedSales, 0);
                    });
                }
                


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void GetEntryDataDetailsContainsSales()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var res = AllocationsBaseModel
                    .GetEntryDataDetails(
                        new List<(string ItemNumber, int InventoryItemId)>() { ("TOH/MTSX018S", 8116) }, true)
                    //.First(x => x.EntryDataId.Trim() == "TR/021252".Trim() && x.LineNumber == 1);
                    //.First(x => x.EntryDataDetailsId == 1986030);
                    .All(x => x.Sales == null);
                Assert.That(res, Is.Not.True);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        [Timeout(3 * 1000 * 60)]
        public void GetItemSets()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var lst = BaseDataModel.GetItemSets(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,"TOH/MTSX018S");
                timer.Stop();
                Console.Write("AllocatSales1 in seconds: " + timer.Elapsed.Seconds);
                Assert.That(lst.Any(), Is.True);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

    }
}