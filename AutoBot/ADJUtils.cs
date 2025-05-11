using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;
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


        public static async Task CreateAdjustmentEntries(bool overwrite, string adjustmentType)
        {
            Console.WriteLine($"Create {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {
                var lst = GetADJtoXMLForType(adjustmentType);

                await CreateAdjustmentEntries(overwrite, adjustmentType, lst, null).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static List<IGrouping<int, TODO_AdjustmentsToXML>> GetADJtoXMLForType(string adjustmentType)
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

                return lst;
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

        public static async Task EmailAdjustmentErrors()
        {

            var info = await BaseDataModel.CurrentSalesInfo(-1).ConfigureAwait(false);
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "AdjustmentErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                    .Where(x => x.Type == "ADJ")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Status != null)
                    .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>
                {
                    dataToPrint = errors
                };
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    await Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                    await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory, $"Adjustment Errors for {info.Item1:yyyy-MM-dd} - {info.Item2:yyyy-MM-dd}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[]
                    {
                        errorfile
                    }).ConfigureAwait(false);
            }

        }

        public static async Task CreateAdjustmentEntries(bool overwrite, string adjustmentType, FileTypes fileType)
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
                            $"select * from [TODO-AdjustmentsToXML]  where AsycudaDocumentSetId = {fileType.AsycudaDocumentSetId}" +
                            $"and AdjustmentType = '{adjustmentType}'")//because emails combined// and EmailId = '{fileType.EmailId}'
                        .ToList()
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .ToList();

                    await CreateAdjustmentEntries(overwrite, adjustmentType, lst, fileType.EmailId).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task CreateAdjustmentEntries(bool overwrite, string adjustmentType,
            List<IGrouping<int, TODO_AdjustmentsToXML>> lst, string emailId)
        {
            await ClearExistingAdjustments(overwrite, lst).ConfigureAwait(false);

            foreach (var doc in lst)
            {
                try
                {
                    //AllocationsModel.Instance.CreateEx9.SetfreashStart(true);
                    // do duty Paid
                    var itemFilter =
                        $" && ({Enumerable.Select<TODO_AdjustmentsToXML, string>(doc, x => $"EntryDataDetailsId == {x.EntryDataDetailsId}").Distinct().Aggregate((old, current) => old + " || " + current)})";
                    var entryDataDetailsIds = doc.Select(x => x.EntryDataDetailsId).ToList();
                    var t1 = CreateDutyPaidADJEntries(adjustmentType, emailId, itemFilter, doc, entryDataDetailsIds);
                    
                    var t2 = CreateDutyFreeADJEntries(adjustmentType, emailId, itemFilter, doc, entryDataDetailsIds);
                    var t3 = CreateDutyFreeOPSEntries(adjustmentType, emailId, itemFilter, doc, entryDataDetailsIds);

                    if (entryDataDetailsIds.Count > 7)
                    {
                        await Task.WhenAll(t1, t3).ConfigureAwait(false);
                        // t1.Wait();
                        await t2.ConfigureAwait(false);
                        // t3.Wait();
                    }
                    else
                    {
                        await Task.WhenAll(t1, t2, t3).ConfigureAwait(false);
                    }


                    await BaseDataModel.RenameDuplicateDocuments(doc.Key).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static Task CreateDutyFreeOPSEntries(string adjustmentType, string emailId, string itemFilter, IGrouping<int, TODO_AdjustmentsToXML> doc,
            List<int> entryDataDetailsIds)
        {
            var t3 = Task.Run(async () =>
            {
                var filterExpressionf = GetDutyFreeFilterExp(itemFilter);
                await new AdjustmentOverService()
                    .CreateOPS(filterExpressionf, false, doc.Key, adjustmentType, entryDataDetailsIds, emailId).ConfigureAwait(false);
            });
            return t3;
        }

        private static Task CreateDutyFreeADJEntries(string adjustmentType, string emailId, string itemFilter, IGrouping<int, TODO_AdjustmentsToXML> doc,
            List<int> entryDataDetailsIds)
        {
            var t2 = Task.Run(async () =>
            {
                var filterExpressionf = GetDutyFreeFilterExp(itemFilter);

                await new AdjustmentShortService().CreateIM9(filterExpressionf, false,
                    doc.Key, "Duty Free", adjustmentType, emailId).ConfigureAwait(false);
            });
            return t2;
        }

        private static string GetDutyFreeFilterExp(string itemFilter)
        {
            var filterExpressionf =
                    $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")"

                    //$"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                    //$" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                    // + $" && EntryDataId == \"ADJ-June 2019\""
                    //+ $" && ItemNumber == \"7IT/PR-LBL-RBN\""
                    + $" && DutyFreePaid == \"Duty Free\""
                    + itemFilter
                // + $" && \"{Enumerable.Select<TODO_AdjustmentsToXML, string>(doc, x => x.InvoiceNo).Distinct().Aggregate((old, current) => old + "," + current)}\".Contains(EntryDataId)"
                ;
            return filterExpressionf;
        }

        private static Task CreateDutyPaidADJEntries(string adjustmentType, string emailId, string itemFilter, IGrouping<int, TODO_AdjustmentsToXML> doc,
            List<int> entryDataDetailsIds)
        {
            var t1 = Task.Run(async () =>
            {
                var filterExpressionp =
                        $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")"

                        //$"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                        //$" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                        //+ $" && EntryDataId == \"{doc.InvoiceNo}\""
                        + $" && DutyFreePaid == \"Duty Paid\""
                        + itemFilter
                    ;


                await new AdjustmentShortService().CreateIM9(filterExpressionp, false,
                    doc.Key, "Duty Paid", adjustmentType, emailId).ConfigureAwait(false);
            });
            return t1;
        }

        private static async Task ClearExistingAdjustments(bool overwrite, List<IGrouping<int, TODO_AdjustmentsToXML>> lst)
        {
            if (overwrite)
            {
                foreach (var doc in lst.Select(x => x.Key).Distinct<int>())
                {
                    using (var ctx = new CoreEntitiesContext())
                    {
                        ctx.Database.CommandTimeout = 20;
                        var keys = lst.SelectMany(x => x.Select(z => z.EntryDataDetailsKey)).ToList();

                        if (ctx.AsycudaDocumentItemEntryDataDetails.Any(x => keys.Contains(x.key)))
                            BaseDataModel.Instance.UpdateAsycudaDocumentSetLastNumber(doc, 0);
                    }

                    await BaseDataModel.Instance.ClearAsycudaDocumentSet(doc).ConfigureAwait(false);
                    //; // took it of so it would keep counting up
                }
            }
        }
    }
}