using System.Threading.Tasks;
using AdjustmentQS.Business.Services;
using AllocationDS.Business.Entities;
using AllocationQS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.AdjustmentQS.GettingEx9AllocationsList;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingEntryDataDetails;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems;
using WaterNut.Business.Services.Utils.AutoMatching;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class AllocationsTests
    {
        private AllocationsBaseModel _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
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
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                Assert.That(new GetXcudaItemsMem(), Is.Not.Null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        public void CanGetEntryDataDetailsMem()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                Assert.That(new GetEntryDataDetailsMem(), Is.Not.Null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        public void CreateItemSets()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var itemSets = BaseDataModel.GetItemSets(null);
                var list = itemSets.Where(x => x.Any(z => z.ItemNumber == "MMM/62556752301")).ToList();
                Assert.Multiple(() =>
                {
                    Assert.That(itemSets.Any(), Is.True);
                    
                    Assert.Equals(1, list.Count());


                });
               
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [Test]
       [Timeout(3*1000*60)]
        public void AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                 new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(2, false, false, "TOH/MTSX018S").Wait();
                timer.Stop();
                Console.Write("AllocatSales1 in seconds: " + timer.Elapsed.Seconds);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)] 
        [TestCase("MTV/44311005", "2022-12-19", 62)]
        [TestCase("SUA/408-1-1/2", "2022-12-19", 60)] // looking for negative qty allocated
        [TestCase("OUR/RASH-GRE", "2022-12-19", 60)] // non stock entrydatadetails
        [TestCase("TOH/MTS009_8L", "2022-12-19", 60)] // Overage Adjustment not allocating - decided to leave it so
        [TestCase("CRB/SSMX-240/0", "2022-12-19", 60)] // discrepancy with overage not allocating 
        [TestCase("INT/YBC106/3GL", "2022-12-19", 60)] // discrepancy with overage not allocating 
        [TestCase("SE H/R1-S1", "2022-12-19", 60)] // DIS with alias name not allocating 
        [TestCase("FSP/906020", "2022-12-19", 60)] // null ex9asycudasales allocations 
        [TestCase("ECL/80040", "3/1/2023", 60)] // cancelled document item
        [TestCase("CHAIN/10G-28", "3/1/2023", 60)] // early sales error that don't make sense in adjustments
        [TestCase("INT/YBC106GL", "3/1/2023", 60)] // don't mark early sales if returns in future
        [TestCase(null, "2023-12-19", 101)]
        [TestCase("SFL/SFCPA1-G500-01", "3/1/2023", 60)]// effective assessment date = '01/01/0001' not allocating
        [TestCase("TOH/369-60111-0", "3/1/2023", 60)]// 7500 entry in 2 '01/01/0001' not allocating
        [TestCase("CRB/ED-BTE18AURL-E_W", "3/1/2023", 60)]//  not allocating
        [TestCase("MAG/CO10-106", "3/1/2023", 60)]//1 marked as under allocated - left as is because discrepancy executed after cut off date
        [TestCase("DAI-JB4U15-3000DG-", "3/1/2023", 60)] // overallocated don't make sense
        [TestCase("ANR/ROC020", "3/1/2023", 60)] // overallocated don't make sense
        [TestCase("PVH/2322410", "3/1/2023", 60)] //"MJM/RM454-04T"
        [TestCase("MJM/RM454-04T", "3/1/2023", 60)]//not allocating
        [TestCase("GAR/010-02093-03", "3/1/2023", 60)]//not allocating audit
        [TestCase("AB/320000600065-BOND", "3/1/2023", 60)]//not allocating audit
        [TestCase("CRB/HIF-SR270BL", "3/1/2023", 60)]
        [TestCase("WAL/5002010004", "3/1/2023", 60)]
        [TestCase("CRB/HIF-SR290BL", "3/1/2023", 60)] 
        [TestCase("POL/3248AG", "3/1/2023", 60)]
        [TestCase("XMN/LNK-PL-RGW", "3/1/2023", 60)] // infinite loop 
        [TestCase("21SHTZZV971ZZZZ", "3/1/2023", 60)] //qty allocated under reported 
        




        /////////////////////// sandals
        [TestCase("0756-1474", "3/1/2023", 60)]
        [TestCase("14001-0214SMNVY", "3/1/2023", 60)]  // return not allocated
        [TestCase("18130-05GTUR", "3/1/2023", 60)] //infinite loop
        [TestCase("0532-1168", "3/1/2023", 60)]
        [TestCase("14002-85LGMUL", "3/1/2023", 60)]
        [TestCase("0424-09-16KHT", "3/1/2023", 60)]

        [TestCase("0101-0898", "3/1/2023", 60)]// checking mapping
        [TestCase("0101-0898XLCRL", "3/1/2023", 60)]// checking mapping

        [TestCase("0645-7733", "3/1/2023", 60)]// Allocate to Manually Assessed OPS



        ////////// IWW

        [TestCase("MM07722", "3/1/2023", 60)]
        [TestCase("ML33451", "3/1/2023", 60)] // not marking overallocations
        [TestCase("SA00363", "3/1/2023", 60)] // not marking overallocations
        [TestCase("FJ41051431", "3/1/2023", 60)] // not allocating all im7 7400-WSC
        [TestCase("E7763322", "3/1/2023", 60)]  // return marked as early sales
        [TestCase("HF01360", "3/1/2023", 60)]  //return and 2 sales not labeled properly
        [TestCase("RK00650", "3/1/2023", 60)] // not allocating existing ex9 properly need to set the piqty for each allocation
        [TestCase("DD43011", "3/1/2023", 60)]  // missing allocations for entrydatadeatilsid = 1279189

        ////////////Rouge
        [TestCase("82453688", "3/1/2023", 60)] // not allocating
        [TestCase("F050101", "3/1/2023", 60)] // not allocating 
        [TestCase("68400", "3/1/2023", 60)] 
        [TestCase("004211", "3/1/2023", 60)]
        [TestCase("65100000000", "3/1/2023", 60)]// over allocation issue
        [TestCase("LC9935007", "3/1/2023", 60)] // incomplete allocations
        [TestCase("L5366300", "3/1/2023", 60)] // Make sure IM9 come out from im7 


        public void AllocatSales(string itemNumber, string LastInvoiceDate, int NoOfAllocations )
        {
            try
            {
                Z.EntityFramework.Extensions.LicenseManager.AddLicense("7242;101-JosephBartholomew", "2080412a-8e17-8a71-cb4a-8e12f684d4da");
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if(itemSets == null || !itemSets.Any()) throw new ApplicationException("No Item Found");
                if(string.IsNullOrEmpty(itemNumber) )
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();

                

                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();
                
                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
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
                //        Assert.Equals(NoOfAllocations, allocations);
                //        Assert.Equals(0, overallocatedSales);
                //        Assert.Equals(0, unallocatedSales);
                //    });
                //}
                


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [TestCase("ANR/ROC020", "3/1/2023", 60)] //P2O Shorts allocating to already exwarehoused
        [TestCase( null, "3/1/2023", 60)] //P2O Shorts allocating to already exwarehoused
        [TestCase("BLS/9004E", "3/1/2023", 60)] //P2O Shorts allocating to already exwarehoused
        
        public void AllocatSalesOnlyNewAllocations(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if (string.IsNullOrEmpty(itemNumber))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, true, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
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
                //        Assert.Equals(NoOfAllocations, allocations);
                //        Assert.Equals(0, overallocatedSales);
                //        Assert.Equals(0, unallocatedSales);
                //    });
                //}



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("Audit", "2023-12-19", 101)]
        public void AllocatSalesByInvoiceNo(string invoiceNo, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemNumbers = GetItemNumbers(invoiceNo);
                

                var itemSets = BaseDataModel.GetItemSets(itemNumbers);
                if (string.IsNullOrEmpty(itemNumbers))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        private string GetItemNumbers(string invoiceNo)
        {
            using (var ctx = new AllocationDSContext())
            {
                var itemNumbersList = ctx.EntryDataDetails
                    .AsNoTracking()
                    .Where(x => x.EntryDataId.StartsWith(invoiceNo))
                    .Select(x => x.ItemNumber).ToList();
               return string.Join(",", itemNumbersList);
            }
        }


        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("MMM/62556752301, 7000046623, 7100060280", "TR/026176")] // null ex9asycudasales allocations 

        public void doubleAllocation (string itemNumber, string invoiceNo)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if (string.IsNullOrEmpty(itemNumber))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
                //var lastInvoiceDate = DateTime.Parse(LastInvoiceDate)+TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count(x => x.InvoiceNo == invoiceNo && itemNumber.Contains(x.ItemNumber));


                    Assert.Multiple(() =>
                    {
                        Assert.Equals(2, allocations);
                        

                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("CRB/SF-MD3/120-370", "50797")] // null ex9asycudasales allocations 

        public void ReturnNotBringingDownQtyAllocated(string itemNumber, string pCnumber)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if (string.IsNullOrEmpty(itemNumber))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
                //var lastInvoiceDate = DateTime.Parse(LastInvoiceDate)+TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count(x => x.pCNumber == pCnumber && x.ItemNumber == itemNumber);
                    var isCancelled =
                        ctx.AsycudaDocument.FirstOrDefault(x => x.CNumber == pCnumber && x.Cancelled == true);





                    Assert.Multiple(() =>
                    {
                        Assert.Equals(0, allocations);
                        Assert.That(isCancelled, Is.Not.Null);

                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("ECL/80040", "40967")] // null ex9asycudasales allocations 
       
        public void AllocateSaleToCancelledItem(string itemNumber, string pCnumber)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if (string.IsNullOrEmpty(itemNumber))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
                //var lastInvoiceDate = DateTime.Parse(LastInvoiceDate)+TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count(x => x.pCNumber == pCnumber && x.ItemNumber == itemNumber);
                    var isCancelled =
                        ctx.AsycudaDocument.FirstOrDefault(x => x.CNumber == pCnumber && x.Cancelled == true);





                    Assert.Multiple(() =>
                    {
                        Assert.Equals(0, allocations);
                        Assert.That(isCancelled, Is.Not.Null); ;
                        
                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        [TestCase("TOH/MTS009_8L", "2023-2-11", 7)] // Overage Adjustment not allocating - decided to leave it so
      public void AllocatSalesTOH_MTS009_8L(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if (string.IsNullOrEmpty(itemNumber))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
                var lastInvoiceDate = DateTime.Parse(LastInvoiceDate) + TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber);

                    var errallocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Count(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber && x.Status != null);

                    //var duplicateAllocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                    //    .Where(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber)
                    //    .GroupBy(x => new{ x.EntryDataDetailsId, x.SalesQuantity})
                    //    .Where(x => x.Sum(z => z.QtyAllocated) > x.Key.SalesQuantity)

                    var overallocatedSales = ctx.EntryDataDetails
                        .AsNoTracking()
                        .Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated > x.Quantity);
                    var unallocatedSales = ctx.EntryDataDetails.AsNoTracking()
                        .Where(x => x.EntryData.EntryType != "PO" && x.EntryData.EntryType != "OPS")
                        .Where(x => x.ItemNumber == itemNumber)
                        .Count(x => x.QtyAllocated == 0);


                    Assert.Multiple(() =>
                    {
                        Assert.Equals(NoOfAllocations, allocations);
                        Assert.Equals(0, overallocatedSales);
                        Assert.Equals(0, unallocatedSales);
                        Assert.Equals(1, errallocations);
                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

      [Test]
        [TestCase("CAM/CMMP83RZ", "2023-2-28", 67)] // Overage Adjustment not allocating - decided to leave it so
        public void MarkNegetiveAllocations(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();

                var itemSets = BaseDataModel.GetItemSets(itemNumber);
                if (string.IsNullOrEmpty(itemNumber))
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
                else
                    AllocationsModel.Instance.ClearItemSetAllocations(itemSets).Wait();



                timer.Start();
                new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false, false, itemSets);
                timer.Stop();

                Console.Write("AllocatSales in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(true);
                var lastInvoiceDate = DateTime.Parse(LastInvoiceDate) + TimeSpan.FromHours(12);
                using (var ctx = new AllocationDSContext())
                {
                    var allocations = new AllocationQSContext().AsycudaSalesAndAdjustmentAllocationsExes.AsNoTracking()
                        .Where(x => x.InvoiceDate <= lastInvoiceDate && x.ItemNumber == itemNumber).ToList();

                    var qtyallocated = allocations.Sum(x => x.SalesQtyAllocated);
                    var salesQty = allocations.Sum(x => x.SalesQuantity);


                    Assert.Multiple(() =>
                    {
                        Assert.Equals(NoOfAllocations, allocations.Count);
                        Assert.Equals(qtyallocated, salesQty);
                       
                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        //[TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public void AllocatExistingEx9s(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
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
                        Assert.Equals(NoOfAllocations, allocations);
                        Assert.Equals(0, overallocatedSales);
                        Assert.Equals(0, unallocatedSales);
                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public void Allocat1000Sales(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
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
                    .ForAll(async x => await new OldSalesAllocator().AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(false, false, x).ConfigureAwait(false)); // to prevent changing allocations when im7 info changes
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
                        Assert.Equals(NoOfAllocations, allocations);
                        Assert.Equals(0, overallocatedSales);
                        Assert.Equals(0, unallocatedSales);
                    });
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        [TestCase("INT/YBA470GL", "3/1/2023", 60)]
        public void MarkErrors(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
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
               Assert.That(true);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
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
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
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
                Assert.That(true);



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        [Timeout(60 * 1000 * 60)]
        [TestCase("TOH/MTSX018S", "2022-12-19", 62)]
        [TestCase(null, "2023-12-19", 101)]
        public void ClearDocSetAllocations(string itemNumber, string LastInvoiceDate, int NoOfAllocations)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
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
                Assert.That(true);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }


        [Test]
        public void GetEntryDataDetailsContainsSales()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
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
                Assert.That(false);
            }
        }

        [Test]
        [Timeout(3 * 1000 * 60)]
        public void GetItemSets()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var lst = BaseDataModel.GetItemSets("CAM/CMMP83RZ");
                timer.Stop();
                Console.Write("GetItemSets in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(lst.Any(), Is.True);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

        [Test]
        //[Timeout(3 * 1000 * 60)]
        public void getEx9AllocationsRefactored()
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.That(true);
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var lst = new getEx9AllocationsRefactored().Execute("TOH/MTSX018S");
                timer.Stop();
                Console.Write("getEx9AllocationsRefactored in seconds: " + timer.Elapsed.TotalSeconds);
                Assert.That(lst.Any(), Is.True);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.That(false);
            }
        }

     

    }
}