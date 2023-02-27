using System.Data.Entity;
using System.IO;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;
using WaterNut.Business.Services.Utils;

namespace AutoBotUtilities.Tests
{
  
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using DocumentDS.Business.Entities;
    using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations;

    [TestFixture]
    public class CreateEX9Tests
    {
       

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(2);
            Infrastructure.Utils.ClearDataBase();
            
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new CreateEX9Tests();
            Assert.That(instance, Is.Not.Null);
        }



       
        [Test]
        [TestCase("2/1/2023","2/28/2023", "SEH/1277G", 1, 2, 5)]
        [TestCase("9/1/2018", "2/28/2023", "SEH/1277G", 1, 2, 5)]
        [TestCase("7/1/2020", "7/31/2020", "MRL/JB0057F", 1, 2, 5)]// overexwarehousing
        public void CanCreateEx9(DateTime startDate, DateTime endDate, string itemNumber, int docCount, int lineCount,int totalQuantiy)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);

                var testData = SetupTest(startDate,endDate, itemNumber);

                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var res = new CreateEx9().Execute(testData.filterExpression, false, false, true, testData.docset, "Sales", "Historic", true, true, true, true, true, false, true, true, true, true).Result;
                timer.Stop();
                Console.Write($"CreateEx9 for {startDate} in seconds: {timer.Elapsed.TotalSeconds}");

                AssertTest(docCount, lineCount, totalQuantiy, testData.docset);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }


        [Test]
        [TestCase("2/1/2023", "2/28/2023", "SEH/1277G", 1, 2, 5)]
        [TestCase("9/1/2018", "2/28/2023", "SEH/1277G", 3, 4, 37)]
        [TestCase("9/1/2018", "2/28/2023", "", 3, 4, 37)] // over exwarehousing
        [TestCase("1/1/2022", "1/31/2022", "HCLAMP/HD20-4043", 0, 0, 0)] // over exwarehousing
        [TestCase("1/1/2022", "1/31/2022", "", 3, 4, 37)] // over exwarehousing
        [TestCase("7/1/2020", "7/31/2020", "MRL/JB0057F", 1, 1, 2)]// overexwarehousing
        public void CanCreateEx9Mem(DateTime startDate, DateTime endDate, string itemNumber, int docCount, int lineCount,
            int totalQuantiy)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);

                var testData = SetupTest(startDate,endDate, itemNumber);


                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var res = new CreateEx9Mem().Execute(testData.filterExpression, false, false, true, testData.docset, "Sales", "Historic", true, true, true, true, true, false, true, true, true, true).Result;
                timer.Stop();
                Console.Write($"CreateEx9 for {startDate} in seconds: {timer.Elapsed.TotalSeconds}");

                AssertTest(docCount, lineCount, totalQuantiy, testData.docset);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        [TestCase("2/1/2023", "2/28/2023", "SEH/1277G", 1, 2, 5)]
        [TestCase("9/1/2018", "2/28/2023", "SEH/1277G", 1, 2, 5)]


        public void CanGetEx9AsycudaSalesAllocationsMem(DateTime startDate, DateTime endDate, string itemNumber, int docCount, int lineCount,
            int totalQuantiy)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);

                var testData = SetupTest(startDate, endDate, itemNumber);


                var timer = new System.Diagnostics.Stopwatch();
                var realStartDate = new CreateEx9Mem().GetWholeRangeFilter(testData.filterExpression, out var realEndDate,  out var exPro, out var rdateFilter, out var realFilterExp);
                timer.Start();
                var res = new GetEx9AsycudaSalesAllocationsMem(realFilterExp, rdateFilter);
                timer.Stop();
                Console.Write($"CreateEx9 for {startDate} in seconds: {timer.Elapsed.TotalSeconds}");

                Assert.IsTrue(true);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }

        [Test]
        [TestCase("9/1/2018", "2/28/2023", "HCLAMP/HD20-4043", 11)]

        public void CanGetEx9AsycudaSalesAllocationsMem(DateTime startDate, DateTime endDate, string itemNumber, int previousItemsCount)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);

                var testData = SetupTest(startDate, endDate, itemNumber);


                var timer = new System.Diagnostics.Stopwatch();
                var realStartDate = new CreateEx9Mem().GetWholeRangeFilter(testData.filterExpression, out var realEndDate, out var exPro, out var rdateFilter, out var realFilterExp);
                timer.Start();
                var res = new GetEx9DataMem(realFilterExp, rdateFilter);
                var sDate = DateTime.Parse("1/1/2022");
                var eDate = DateTime.Parse("1/31/2022").AddHours(23);
                var currentFilterExpression = GetFilterExpression(itemNumber, sDate, eDate);
                var lst = new CreateAllocationDataBlocks().Execute(currentFilterExpression,new List<string>(),(currentFilterExpression,rdateFilter,sDate,eDate), res, false ).Result;

                timer.Stop();
                Console.Write($"CreateEx9 for {startDate} in seconds: {timer.Elapsed.TotalSeconds}");

                Assert.Multiple(() =>
                {
                    Assert.AreEqual(previousItemsCount, lst.Sum(x => x.Allocations.Sum(z => z.previousItems.Count)));
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }



        private static (AsycudaDocumentSet docset, string filterExpression) SetupTest(DateTime startDate, DateTime endDate, string itemNumber)
        {
            var saleInfo = EntryDocSetUtils.CreateMonthYearAsycudaDocSet(startDate);

            var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;
            BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
            BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0);

            var filterExpression = GetFilterExpression(itemNumber, startDate, endDate);
            return (docset, filterExpression);
        }

        [Test]
        [TestCase("MRL/JB0057F")]
        public void ItemSetTest(string itemNumber)
        {
           
            var itemSets = BaseDataModel.GetItemSets(itemNumber);
            var itemNumbers = itemSets.SelectMany(x => x.Select(z => z.ItemNumber)).DefaultIfEmpty("")
                .Aggregate((o, n) => $"{o},{n}");
            Assert.True(true);
        }

            private static (AsycudaDocumentSet docset, string filterExpression) SetupItemSetTest(DateTime startDate, DateTime endDate, string itemNumber)
        {
            var saleInfo = EntryDocSetUtils.CreateMonthYearAsycudaDocSet(startDate);
            var itemSets = BaseDataModel.GetItemSets(itemNumber);
            var itemNumbers = itemSets.SelectMany(x => x.Select(z => z.ItemNumber)).DefaultIfEmpty("")
                .Aggregate((o, n) => $"{o},{n}");
            var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;
            BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
            BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0);

            var filterExpression = GetFilterExpression(itemNumbers, startDate, endDate);
            return (docset, filterExpression);
        }

        private static void AssertTest(int docCount, int lineCount, int totalQuantiy, AsycudaDocumentSet docset)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var docs = ctx.AsycudaDocuments.Include(x => x.AsycudaDocumentItems)
                    .Where(x => x.AsycudaDocumentSetId == docset.AsycudaDocumentSetId)
                    .ToList();
                Assert.Multiple(() =>
                {
                    Assert.AreEqual(docCount, docs.Count);
                    Assert.AreEqual(lineCount, docs.Sum(x => x.Lines));
                    Assert.AreEqual(totalQuantiy, docs.Sum(x => x.AsycudaDocumentItems.Sum(z => z.ItemQuantity)));
                });
            }
        }
        private static string GetFilterExpression(string itemNumbers, DateTime startDate, DateTime endDate)
        {
            var filterExpression =
                $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                $"&& ({(string.IsNullOrEmpty(itemNumbers) ? "ItemNumber != null" : $"\"{itemNumbers}\".Contains(ItemNumber)")})" +
                $"&& (InvoiceDate >= \"{startDate:MM/01/yyyy}\" " +
                $" && InvoiceDate <= \"{endDate:MM/dd/yyyy HH:mm:ss}\")" +
                //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                "&& ( TaxAmount == 0 ||  TaxAmount != 0)" +
                "&& PreviousItem_Id != null" +
                "&& (xBond_Item_Id == 0 )" +
                "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                "&& (PiQuantity < pQtyAllocated)" +
                "&& (Status == null || Status == \"\")" +
                (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                    ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                    : "") +
                ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");
            return filterExpression;
        }
    }
}