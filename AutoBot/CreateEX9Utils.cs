using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace AutoBot
{
    public class CreateEX9Utils
    {
        public static List<DocumentCT> CreateEx9(bool overwrite, int months)
        {
            try
            {
                SQLBlackBox.RunSqlBlackBox();

                Console.WriteLine("Create Ex9");

                var saleInfo = BaseDataModel.CurrentSalesInfo(months);


                if (saleInfo.Item3.AsycudaDocumentSetId == 0 || !HasData(saleInfo)) return new List<DocumentCT>();
               

                var docSet = GetDocset(saleInfo.DocSet.AsycudaDocumentSetId);

                if (overwrite) BaseDataModel.Instance.ClearAsycudaDocumentSet(docSet.AsycudaDocumentSetId).Wait();


                var filterExpression = CreateFilterExpression(saleInfo);


                return AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, true, docSet, "Sales", "Historic", BaseDataModel.Instance.CurrentApplicationSettings.GroupEX9.GetValueOrDefault(), true, true, true, true, false, true, true, true, true).Result;
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static AsycudaDocumentSet GetDocset(int asycudaDocumentSetId) => BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).Result;

        private static string CreateFilterExpression(
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) saleInfo) =>
                    $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                    $"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                    $" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                    //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                    "&& ( TaxAmount == 0 ||  TaxAmount != 0)" +
                    //"&& PreviousItem_Id != null" +
                    //"&& (xBond_Item_Id == 0 )" +
                    //"&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                    //"&& (PiQuantity < pQtyAllocated)" +
                    //"&& (Status == null || Status == \"\")" +
                    //(BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                    //    ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                    //    : "") +
                    ($" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\"");

        private static bool HasData((DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) saleInfo)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.Database.CommandTimeout = 0;

                return  ctx.Database.SqlQuery<string>(GetSqlStr(saleInfo)).Any();
            }
        }

        private static string GetSqlStr(
            (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) saleInfo) =>
            $@"SELECT EX9AsycudaSalesAllocations.ItemNumber
                    FROM    EX9AsycudaSalesAllocations INNER JOIN
                                     ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                                     EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                                     AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
                    WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                                     (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                                     EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                                     (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) AND (EX9AsycudaSalesAllocations.CustomsOperationId = {(int)CustomsOperations.Warehouse}) AND (AllocationErrors.ItemNumber IS NULL) 
                          AND (ApplicationSettings.ApplicationSettingsId = {
                              BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                          }) AND (EX9AsycudaSalesAllocations.InvoiceDate >= '{
                              saleInfo.Item1.ToShortDateString()
                          }') AND 
                                     (EX9AsycudaSalesAllocations.InvoiceDate <= '{saleInfo.EndDate.ToShortDateString()}')
                    GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId--, EX9AsycudaSalesAllocations.pQuantity, EX9AsycudaSalesAllocations.PreviousItem_Id
                    HAVING (SUM(EX9AsycudaSalesAllocations.PiQuantity) < SUM(EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL)";
    }
}