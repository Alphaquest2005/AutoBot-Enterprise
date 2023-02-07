using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AdjustmentQS.Business.Services;
using NUnit.Framework;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DoubleExwarehouseIssue
    {

        [SetUp]
        public void SetUp()
        {
           



        }

        [Test]
        public async Task Exwarehouse()
        {


 if (!Infrastructure.Utils.IsTestApplicationSettings()) return;
            Infrastructure.Utils.ClearDataBase();
            Infrastructure.Utils.SetTestApplicationSettings(6);
            ImportTestFile();
            ImportTestIM7();
            new AllocateSales().Execute(BaseDataModel.Instance.CurrentApplicationSettings, false)
                .Wait();


            if (!Infrastructure.Utils.IsTestApplicationSettings()) Assert.IsTrue(true);

            var startDate = DateTime.Parse("5/1/2022");
          
            var saleInfo = EntryDocSetUtils.CreateMonthYearAsycudaDocSet(startDate);

            var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;

            var filterExpression =
                $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
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



            var res =  AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, true, docset, "Sales", "Historic", true, true, true, true, true, false, true, true, true, true).Result;

            Assert.IsEmpty(res);


        }

        private static void ImportTestIM7()
        {
            if (!Infrastructure.Utils.IsTestApplicationSettings()) return;

            var im7File = Infrastructure.Utils.GetTestSalesFile(new List<string>()
                { "DuplicateExwarehousingIssue", "IM7-GDFDX-8170.xml" });

            var ex9File = Infrastructure.Utils.GetTestSalesFile(new List<string>()
                { "DuplicateExwarehousingIssue", "EX3-GDSGO-24037.xml" });
            

            var docSet = WaterNut.DataSpace.EntryDocSetUtils.GetDocSet("Imports");
            Infrastructure.Utils.ImportDocuments(docSet, new List<string>() { im7File, ex9File }, true);
        }

        private static void ImportTestFile()
        {
            if (!Infrastructure.Utils.IsTestApplicationSettings()) return;

            var disFile = Infrastructure.Utils.GetTestSalesFile(new List<string>()
                { "DuplicateExwarehousingIssue", "29118-8026.csv" });

            Infrastructure.Utils.ImportEntryDataOldWay(new List<string>() { disFile }, FileTypeManager.EntryTypes.Sales,
                FileTypeManager.FileFormats.Csv);
        }
    }
}
