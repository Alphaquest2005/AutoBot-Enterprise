using System.IO;
using System.Threading.Tasks;
using AdjustmentQS.Business.Services;
using AllocationDS.Business.Entities;
using AllocationQS.Business.Entities;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingEntryDataDetails;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;
using WaterNut.Business.Services.Utils;
using WaterNut.Business.Services.Utils.AutoMatching;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    using AutoBot;
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using Org.BouncyCastle.Asn1.Pkcs;

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
        public void CanGetXcudaItemsMem()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                Assert.That(new GetXcudaItemsMem(), Is.Not.Null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void CanGetEntryDataDetailsMem()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                Assert.That(new GetEntryDataDetailsMem(), Is.Not.Null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        public void CreateItemSets()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                Assert.That(BaseDataModel.GetItemSets(null).Any(), Is.True);

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
                 new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(2, false, "TOH/MTSX018S").Wait();
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
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)] 
        [TestCase("MTV/44311005", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public void AllocatSales(string itemNumber, string LastInvoiceDate, int NoOfAllocations )
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();
                
                if(Infrastructure.Utils.IsDevSqlServer()) AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

                timer.Start();
                new AllocateSalesChain().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, itemNumber).Wait();
                timer.Stop();
                
                Console.Write("AllocatSalesChained in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.IsTrue(true);
                //var lastInvoiceDate = DateTime.Parse(LastInvoiceDate)+TimeSpan.FromHours(12);
                //using (var ctx = new AllocationDSContext())
                //{
                //    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                //        .Count(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber);

                //    //var duplicateAllocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                //    //    .Where(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber)
                //    //    .GroupBy(x => new{ x.EntryDataDetailsId, x.SalesQuantity})
                //    //    .Where(x => x.Sum(z => z.QtyAllocated) > x.Key.SalesQuantity)

                //    var overallocatedSales = ctx.EntryDataDetails
                //        .AsNoTracking()
                //        .Where(x => x.ItemNumber == itemNumber)
                //        .Count(x => x.QtyAllocated > x.Quantity);
                //    var unallocatedSales = ctx.EntryDataDetails.AsNoTracking()
                //        .Where(x => x.ItemNumber == itemNumber)
                //        .Count(x => x.QtyAllocated == 0);
                   

                //    Assert.Multiple(() =>
                //    {
                //        Assert.AreEqual(NoOfAllocations, allocations);
                //        Assert.AreEqual(0, overallocatedSales);
                //        Assert.AreEqual(0, unallocatedSales);
                //    });
                //}
                


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }


        [Test]
        [Timeout(60 * 1000 * 60)]
        //[TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public async Task AllocatExistingEx9s(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();

                if (Infrastructure.Utils.IsDevSqlServer()) AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

                timer.Start();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                timer.Stop();

                Console.Write($"GetItemSets in seconds: {timer.Elapsed.TotalSeconds} \r\n");

                timer.Restart();
                
                itemSets.Take(1000)
                    .AsParallel()
                    .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage))
                    //.WithDegreeOfParallelism(1)
                    .ForAll(async x => await new ReallocateExistingEx9().Execute(x).ConfigureAwait(false)); // to prevent changing allocations when im7 info changes

                
                timer.Stop();

                Console.Write($"AllocatExistingEx9s in seconds: {timer.Elapsed.TotalSeconds} \r\n" );
                var lastInvoiceDate = DateTime.Parse(LastInvoiceDate) + TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count();

                    //var duplicateAllocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                    //    .Where(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber)
                    //    .GroupBy(x => new{ x.EntryDataDetailsId, x.SalesQuantity})
                    //    .Where(x => x.Sum(z => z.QtyAllocated) > x.Key.SalesQuantity)

                    var overallocatedSales = ctx.EntryDataDetails
                        .AsNoTracking()
                        //.Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated > x.Quantity);
                    var unallocatedSales = ctx.EntryDataDetails.AsNoTracking()
                        //.Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated == 0);


                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(NoOfAllocations, allocations);
                        Assert.AreEqual(0, overallocatedSales);
                        Assert.AreEqual(0, unallocatedSales);
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
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public async Task Allocat1000Sales(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();

                if (Infrastructure.Utils.IsDevSqlServer()) AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

                timer.Start();

                var itemSets = BaseDataModel.GetItemSets(itemNumber).Take(1000).ToList();
                timer.Stop();

                Console.Write($"GetItemSets in seconds: {timer.Elapsed.TotalSeconds} \r\n");

                timer.Restart();

                itemSets
                    .AsParallel()
                    .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage))
                    .ForAll(async x => await new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(false, x).ConfigureAwait(false)); // to prevent changing allocations when im7 info changes
                     // .ForEach(async x => await new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(false, x).ConfigureAwait(false)); // to prevent changing allocations when im7 info changes

                timer.Stop();

                Console.Write($"AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber in seconds: {timer.Elapsed.TotalSeconds} \r\n");
                var lastInvoiceDate = DateTime.Parse(LastInvoiceDate) + TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count();

                    //var duplicateAllocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                    //    .Where(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber)
                    //    .GroupBy(x => new{ x.EntryDataDetailsId, x.SalesQuantity})
                    //    .Where(x => x.Sum(z => z.QtyAllocated) > x.Key.SalesQuantity)

                    var overallocatedSales = ctx.EntryDataDetails
                        .AsNoTracking()
                        //.Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated > x.Quantity);
                    var unallocatedSales = ctx.EntryDataDetails.AsNoTracking()
                        //.Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated == 0);


                    Assert.Multiple(() =>
                    {
                        Assert.AreEqual(NoOfAllocations, allocations);
                        Assert.AreEqual(0, overallocatedSales);
                        Assert.AreEqual(0, unallocatedSales);
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
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public async Task MarkErrors(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();

                
                timer.Start();

                var itemSets = BaseDataModel.GetItemSets(itemNumber).ToList();
                timer.Stop();

                Console.Write($"GetItemSets in seconds: {timer.Elapsed.TotalSeconds} \r\n");

                timer.Restart();

                itemSets
                    .AsParallel()
                    .WithDegreeOfParallelism(Convert.ToInt32(Environment.ProcessorCount * BaseDataModel.Instance.ResourcePercentage))
                    .ForAll(async x => await new MarkErrors().Execute(x).ConfigureAwait(false)); // to prevent changing allocations when im7 info changes
                                                                                                                                                                // .ForEach(async x => await new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(false, x).ConfigureAwait(false)); // to prevent changing allocations when im7 info changes

                timer.Stop();

                Console.Write($"MarkErrors in seconds: {timer.Elapsed.TotalSeconds} \r\n");
               Assert.IsTrue(true);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public async Task AutoMatch(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();


                timer.Start();

                var itemSets = BaseDataModel.GetItemSets(itemNumber).ToList();
                timer.Stop();

                Console.Write($"GetItemSets in seconds: {timer.Elapsed.TotalSeconds} \r\n");

                timer.Restart();

                await new AdjustmentShortService().AutoMatch(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, true, itemSets)
                    .ConfigureAwait(false);
                timer.Stop();

                Console.Write($"AutoMatch in seconds: {timer.Elapsed.TotalSeconds} \r\n");
                Assert.IsTrue(true);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public async Task ClearDocSetAllocations(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);
                var timer = new System.Diagnostics.Stopwatch();


                timer.Start();

                var itemSets = BaseDataModel.GetItemSets(itemNumber).ToList();
                timer.Stop();

                Console.Write($"GetItemSets in seconds: {timer.Elapsed.TotalSeconds} \r\n");
                timer.Restart();

                var rawData = ParallelEnumerable.Select(itemSets
                        .AsParallel(), x => new AutoMatchSetBasedProcessor().GetAllDiscrepancyDetails(true, x))
                    .SelectMany(x => x.ToList())
                    .ToList(); //.Where(x => x.EntryDataId == "Asycuda-C#33687-24").ToList();

                timer.Stop();

                Console.Write($"Get RawData in seconds: {timer.Elapsed.TotalSeconds} \r\n");

                timer.Restart();

                new AutoMatchSetBasedProcessor().ClearDocSetAllocations(rawData, itemSets);

                timer.Stop();

                Console.Write($"ClearDocSetAllocations in seconds: {timer.Elapsed.TotalSeconds} \r\n");
                Assert.IsTrue(true);

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
                var res = OldSalesAllocator
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
                var lst = BaseDataModel.GetItemSets("TOH/MTSX018S");
                timer.Stop();
                Console.Write("GetItemSets in seconds: " + timer.Elapsed.TotalSeconds);
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