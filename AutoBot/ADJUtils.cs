using System;
using System.Collections.ObjectModel;
using System.Linq;
using AllocationQS.Business.Entities;
using CoreEntities.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class ADJUtils
    {
        public static void ClearAllAdjustmentEntries(string adjustmentType)
        {
            Console.WriteLine($"Clear {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_TotalAdjustmentsToProcess.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                                                        && x.Type == adjustmentType)
                    .GroupBy(x => x.AsycudaDocumentSetId)
                    //.Where(x => x.Key != null)
                    .Select(x => x.Key)
                    .Distinct()
                    .ToList();

                foreach (var doc in lst)
                {
                    BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).Wait();
                    BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                }

            }
        }


        public static void CreateAdjustmentEntries(bool overwrite, string adjustmentType)
        {
            Console.WriteLine($"Create {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var lst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_AdjustmentsToXML>(
                            $"select * from [TODO-AdjustmentsToXML]  where ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}" +
                            $"and AdjustmentType = '{adjustmentType}'")
                        .ToList()
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .ToList();

                    Utils.CreateAdjustmentEntries(overwrite, adjustmentType, lst, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static ObservableCollection<EX9Utils.SaleReportLine> IM9AdjustmentsReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AllocationQSContext())
                {
                    //ctx.Database.CommandTimeout = 10;
                    //var alst =
                    //     ctx.AdjustmentShortAllocations.Where(x => x.xASYCUDA_Id == ASYCUDA_Id
                    //                                                     && x.EntryDataDetailsId != 0
                    //                                                     && x.PreviousItem_Id != 0
                    //                                                     && x.pRegistrationDate != null)
                    //     //.Select(x => new Allo()
                    //     //{
                    //     //    xLineNumber = x.xLineNumber,
                    //     //    pCNumber = x.pCNumber,
                    //     //    pItemNumber = x.pItemNumber,
                    //     //    InvoiceNo = x.InvoiceNo,
                    //     //    InvoiceDate = x.InvoiceDate,
                    //     //    Comment = x.Comment


                    //     //} )
                    //     .ToList();

                    ////    $"xASYCUDA_Id == {} " + "&& EntryDataDetailsId != null " +
                    ////    "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                    ////.ConfigureAwait(false)).ToList();

                    //var d =
                    //    alst.Where(x => x.xLineNumber != null)
                    //        .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                    //        .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                    //        .OrderBy(s => s.xLineNumber)
                    //        .ThenBy(s => s.InvoiceNo)
                    //        .Select(s => new SaleReportLine
                    //        {
                    //            Line = Convert.ToInt32(s.xLineNumber),
                    //            Date = Convert.ToDateTime(s.InvoiceDate),
                    //            InvoiceNo = s.InvoiceNo,
                    //            Comment = s.Comment,
                    //            ItemNumber = s.ItemNumber,
                    //            ItemDescription = s.ItemDescription,
                    //            TariffCode = s.TariffCode,
                    //            SalesFactor = Convert.ToDouble(s.SalesFactor),
                    //            SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                    //            xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                    //            Price = Convert.ToDouble(s.Cost),
                    //            SalesType = s.DutyFreePaid,
                    //            GrossSales = Convert.ToDouble(s.TotalValue),
                    //            PreviousCNumber = s.pCNumber,
                    //            PreviousLineNumber = s.pLineNumber.ToString(),
                    //            PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                    //            CIFValue =
                    //                (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                    //                Convert.ToDouble(s.QtyAllocated),
                    //            DutyLiablity =
                    //                (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                    //                Convert.ToDouble(s.QtyAllocated)
                    //        }).Distinct();

                    ctx.Database.CommandTimeout = 100;


                    //    $"xASYCUDA_Id == {} " + "&& EntryDataDetailsId != null " +
                    //    "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                    //.ConfigureAwait(false)).ToList();

                    var d =
                        ctx.AdjustmentShortAllocations.Where(x => x.xASYCUDA_Id == ASYCUDA_Id
                                                                  && x.EntryDataDetailsId != 0
                                                                  && x.PreviousItem_Id != 0
                                                                  && x.pRegistrationDate != null)
                            .Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 20) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)

                            .ToList()

                            .Select(s => new EX9Utils.SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                Comment = s.Comment,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = s.DutyFreePaid,
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated)
                            }).Distinct();


                    return new ObservableCollection<EX9Utils.SaleReportLine>(d);


                }
            }
            catch (Exception Ex)
            {
            }

            return null;
        }
    }
}