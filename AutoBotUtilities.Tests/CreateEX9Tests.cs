using System.IO;
using Core.Common.Utils;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.Business.Services.Utils;

namespace AutoBotUtilities.Tests
{
  
    using System;
    using NUnit.Framework;
    using CoreEntities.Business.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using DocumentDS.Business.Entities;

    [TestFixture]
    public class CreateEX9Tests
    {
        private CreateEX9Tests _testClass;

        [SetUp]
        public void SetUp()
        {
            Infrastructure.Utils.SetTestApplicationSettings(2);
            Infrastructure.Utils.ClearDataBase();
            _testClass = new CreateEX9Tests();
        }

        [Test]
        public void CanConstruct()
        {
            var instance = new CreateEX9Tests();
            Assert.That(instance, Is.Not.Null);
        }



       
        [Test]
        [TestCase( "2/1/2023", "SEH/1277G")]
        public void CanCreateEx9(DateTime startDate, string itemNumber)
        {
            try
            {
                if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);

                var saleInfo =  EntryDocSetUtils.CreateMonthYearAsycudaDocSet(startDate);

                var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;
                BaseDataModel.Instance.ClearAsycudaDocumentSet(docset.AsycudaDocumentSetId).Wait();
                BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(docset.AsycudaDocumentSetId, 0);

                var filterExpression =
                    $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                    $"&& ({(string.IsNullOrEmpty(itemNumber) ? "ItemNumber != null" : $"ItemNumber.Contains(\"{itemNumber}\")")})" +
                    $"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                    $" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
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


                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                var res = AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, true, docset, "Sales", "Historic", true, true, true, true, true, false, true, true, true, true).Result;
                timer.Stop();
                Console.Write($"CreateEx9 for {startDate} in seconds: {timer.Elapsed.TotalSeconds}");

                using (var ctx = new EntryDataDSContext())
                {
                    //Assert.Multiple(() =>
                    //{
                    //    Assert.AreEqual(ctx.xSalesFiles.Count(), 1);
                    //    Assert.AreEqual(ctx.xSalesDetails.Count(), 1);
                    //});
                }
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.IsTrue(false);
            }
        }





      
    }
}