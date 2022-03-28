using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using Core.Common.Data;
using Core.Common.UI;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Enums;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.DataSpace;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using MoreLinq;
using WaterNut.Business.Entities;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;


namespace AdjustmentQS.Business.Services
{
    public partial class AdjustmentShortService
    {
        private List<AsycudaDocumentItem> _itemCache;
        private List<InventoryItemAliasX> _itemAliaCache;

        public async Task AutoMatch(int applicationSettingsId, bool overwriteExisting)
        {
            try
            {
                _itemCache = null;

                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 10;



                    var lst = ctx.AdjustmentDetails
                        .Include(x => x.AdjustmentEx)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => x.SystemDocumentSet != null)
                        .Where(x => x.Type == "DIS")
                        //.Where(x => x.ItemNumber == "SH01053")
                        //.Where(x => x.ItemNumber == "BS01016")
                       // .Where(x => x.EntryDataId == "Asycuda-C#5062")
                        //.Where( x => x.EntryDataDetailsId == 16569)
                        .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                        .Where(x => overwriteExisting
                            ? x != null
                            : x.EffectiveDate == null) // take out other check cuz of existing entries 
                        .Where(x => !x.ShortAllocations.Any())
                        .OrderBy(x => x.EntryDataDetailsId)
                        .DistinctBy(x => x.EntryDataDetailsId)
                        .ToList();

                    //var ids = lst.Select(x => x.EntryDataDetailsId).ToList();

                    //var itemEntryDataDetails = ctx.AsycudaDocumentItemEntryDataDetails
                    //    .Where(x => ids.Contains(x.EntryDataDetailsId))
                    //    .ToList()
                    //    .Join(lst, x => x.EntryDataDetailsId, z => z.EntryDataDetailsId, (x, z) => new { key = z, doc = x })
                    //    .ToList();


                    //var alreadyExecuted = lst.Where(x => itemEntryDataDetails.Any(z => z.key.EntryDataDetailsId == x.EntryDataDetailsId)).ToList();
                    //foreach (var itm in alreadyExecuted)
                    //{
                    //    //lst.Remove(itm);
                    //}
                    if (!lst.Any()) return;
                    AllocationsModel.Instance
                        .ClearDocSetAllocations(lst.Select(x => $"'{x.ItemNumber}'").Aggregate((o, n) => $"{o},{n}"))
                        .Wait();

                    AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);



                    await DoAutoMatch(applicationSettingsId, lst, ctx);



                    new AdjustmentShortService()
                        .ProcessDISErrorsForAllocation(
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            lst.Select(x => $"{x.EntryDataDetailsId.ToString()}-{x.ItemNumber}")
                                .Aggregate((o, n) => $"{o},{n}")).Wait();

                    new AllocationsBaseModel()
                        .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false,
                            lst.Select(x => $"{x.EntryDataDetailsId.ToString()}-{x.ItemNumber}")
                                .Aggregate((o, n) => $"{o},{n}")).Wait();

                    new AllocationsBaseModel()
                        .MarkErrors(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();


                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        public async Task AutoMatchDocSet(int applicationSettingsId, int docSetId)
        {
            try
            {

                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 10;



                    var lst = ctx.AdjustmentDetails
                        //.Include(x => x.AdjustmentEx)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == docSetId)
                        //.Where(x => x.SystemDocumentSet != null)
                        // .Where(x => x.ItemNumber == "HEL/003361001")
                        //.Where(x => x.EntryDataId == "120902")
                        //.Where( x => x.EntryDataDetailsId == 545303)
                        .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                        .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 

                        .DistinctBy(x => x.EntryDataDetailsId)
                        .OrderBy(x => x.EntryDataDetailsId)
                        .ToList();

                    await DoAutoMatch(applicationSettingsId, lst, ctx);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task AutoMatchItems(int applicationSettingsId, int docSetId)
        {
            try
            {

                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 10;



                    var lst = ctx.AdjustmentDetails
                        //.Include(x => x.AdjustmentEx)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == docSetId)
                        //.Where(x => x.SystemDocumentSet != null)
                        // .Where(x => x.ItemNumber == "HEL/003361001")
                        //.Where(x => x.EntryDataId == "120902")
                        //.Where( x => x.EntryDataDetailsId == 545303)
                        .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                        .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 

                        .DistinctBy(x => x.EntryDataDetailsId)
                        .OrderBy(x => x.EntryDataDetailsId)
                        .ToList();

                    await DoAutoMatch(applicationSettingsId, lst, ctx);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task DoAutoMatch(int applicationSettingsId, List<AdjustmentDetail> lst, AdjustmentQSContext ctx)
        {
            try
            {

                StatusModel.StartStatusUpdate("Matching Shorts To Asycuda Entries", lst.Count());
                var edLst = new List<EntryDataDetail>();
                DateTime? minEffectiveDate = null;
                foreach (var s in lst)
                {
                    //StatusModel.StatusUpdate("Matching Shorts To Asycuda Entries");
                    var tryCNumber = false;
                    try
                    {
                        ctx.SaveChanges();
                        
                        if (string.IsNullOrEmpty(s.ItemNumber)) continue;
                        var ed = ctx.EntryDataDetails.Include(x => x.AdjustmentEx).First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                        edLst.Add(ed);
                        ed.Comment = null;
                        if (!string.IsNullOrEmpty(s.PreviousInvoiceNumber))
                        {
                            List<AsycudaDocumentItem> aItem;
                            if (s.InvoiceQty > 0)
                            {
                                aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                                        s.PreviousInvoiceNumber, s.EntryDataId, s.ItemNumber)
                                    .ConfigureAwait(false);
                                if (aItem.Any()) MatchToAsycudaItem(s, aItem, ed, ctx);
                            }
                            else
                            {
                                aItem = await GetAsycudaEntriesWithInvoiceNumber(applicationSettingsId,
                                        s.PreviousInvoiceNumber, s.EntryDataId)
                                    .ConfigureAwait(false);
                                if (aItem.Any()) MatchToAsycudaDocument(aItem.First().AsycudaDocument, ed);
                            }

                            if (!aItem.Any())
                            {
                                tryCNumber = true;
                            }
                        }

                        if ((tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber)) &&
                            s.InvoiceQty.GetValueOrDefault() > 0 && !string.IsNullOrEmpty(s.PreviousCNumber))
                        {
                            var aItem = await GetAsycudaEntriesInCNumber(s.PreviousCNumber, s.PreviousCLineNumber, s.ItemNumber)
                                .ConfigureAwait(false);

                            if (!aItem.Any())
                                aItem = await GetAsycudaEntriesInCNumberReference(applicationSettingsId,
                                        s.PreviousCNumber, s.ItemNumber)
                                    .ConfigureAwait(false);
                            MatchToAsycudaItem(s, aItem.OrderBy(x => x.AsycudaDocument.AssessmentDate).ToList(), ed, ctx);
                            // continue;
                        }

                        else if ((tryCNumber || string.IsNullOrEmpty(s.PreviousInvoiceNumber)) &&
                                 s.InvoiceQty.GetValueOrDefault() <= 0 && !string.IsNullOrEmpty(s.PreviousCNumber))
                        {
                            var asycudaDocument = GetAsycudaDocumentInCNumber(applicationSettingsId, s.PreviousCNumber);
                            MatchToAsycudaDocument(asycudaDocument, ed);
                        }

                        if (ed.EffectiveDate == null)
                        {
                            //Set Overs 1st and Shorts to Last of Month
                            //Try match effective date if i find the invoice if not leave it blank

                            // if (ed.EffectiveDate == null) ed.EffectiveDate = s.AdjustmentEx.InvoiceDate;
                            if (s.Type == "DIS")
                            {
                                var po = new EntryDataDSContext().EntryData.OfType<PurchaseOrders>()
                                    .FirstOrDefault(x => x.EntryDataId == ed.EntryDataId);// || ed.PreviousInvoiceNumber.EndsWith(x.EntryDataId) Contains too random
                                ed.EffectiveDate = po?.EntryDataDate;
                            }
                            else
                            {
                                ed.EffectiveDate = s.InvoiceDate;
                            }
                        }

                        if (ed.Cost != 0) continue;
                        if (s.InvoiceQty.GetValueOrDefault() > 0)
                            continue; // if invoice > 0 it should have been imported

                        //////////// only apply to Adjustments because they are after shipment... discrepancies have to be provided.
                        if (s.Type != "ADJ") continue;
                        var lastItemCost = ctx.AsycudaDocumentItemLastItemCosts
                            .Where(x => x.assessmentdate <= ed.EffectiveDate)
                            .OrderByDescending(x => x.assessmentdate)
                            .FirstOrDefault(x =>
                                x.ItemNumber == ed.ItemNumber &&
                                x.applicationsettingsid == applicationSettingsId);
                        if (lastItemCost != null)
                            ed.LastCost = (double)lastItemCost.LocalItemCost.GetValueOrDefault();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }


                minEffectiveDate = edLst.Min(x => x.EffectiveDate) 
                                   ?? edLst.Where(x => x.AdjustmentEx != null).Min(x => x.AdjustmentEx.InvoiceDate);

                foreach (var ed in edLst.Where(x => x.EffectiveDate == null))
                {
                    ed.EffectiveDate = minEffectiveDate;
                    ed.Status = null;
                }

                ctx.SaveChanges();
                StatusModel.StopStatusUpdate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static void MatchToAsycudaDocument(AsycudaDocument asycudaDocument, EntryDataDetail ed)
        {
            if (asycudaDocument != null)
            {
                ed.EffectiveDate = asycudaDocument.AssessmentDate;
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    //PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = asycudaDocument.ASYCUDA_Id,
                    TrackingState = TrackingState.Added
                });
            }
            else
            {
                ed.Status = "No Asycuda Entry Found";
            }
        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
    string sPreviousInvoiceNumber, string sEntryDataId)
        {
            try
            {

                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.ApplicationSettingsId == applicationSettingsId &&
                                (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                || sEntryDataId.ToUpper().Trim() == x.PreviousInvoiceNumber.ToUpper().Trim()
                                ));// contains is too vague
                var res = aItem.ToList();


                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesWithInvoiceNumber(int applicationSettingsId,
            string sPreviousInvoiceNumber, string sEntryDataId, string sItemNumber)
        {
            try
            {

                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim() &&
                                (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                 || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())));
                var res = aItem.ToList();
                var alias = ItemAliasCache
                    .Where(x => x.InventoryItemsEx.ApplicationSettingsId == applicationSettingsId &&
                                x.ItemNumber.ToUpper().Trim() == sItemNumber.ToUpper().Trim())
                    .Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => x.PreviousInvoiceNumber != null
                                && alias.Contains(x.ItemNumber)
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && (x.PreviousInvoiceNumber.ToUpper().Trim() == sPreviousInvoiceNumber.ToUpper().Trim()
                                 || x.PreviousInvoiceNumber.ToUpper().Trim().Contains(sEntryDataId.ToUpper().Trim())))
                    .ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// /////////////////// Create one entry fro kim

        public async Task CreateIM9(string filterExpression, bool perInvoice,
            int asycudaDocumentSetId, string dutyFreePaid, string adjustmentType, List<int> entryDataDetailsIds, string emailId)
        {
            try
            {
                CreateEx9Class.DocSetPi.Clear();
                var docSet =
                    await BaseDataModel.Instance.GetAsycudaDocumentSet(asycudaDocumentSetId)
                        .ConfigureAwait(false);

                var cp =
                    BaseDataModel.Instance.Customs_Procedures
                        .Single(x => x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Discrepancy == true);

                docSet.Customs_Procedure = cp;


                var exPro =
                    $" && PreviousDocumentItem.AsycudaDocument.ApplicationSettingsId = {docSet.ApplicationSettingsId}" +
                    $" && EntryDataDetails.EntryDataDetailsEx.AsycudaDocumentSetId == {docSet.AsycudaDocumentSetId}" +
                    $" && (PreviousDocumentItem.AsycudaDocument.CustomsOperationId == {(int)CustomsOperations.Warehouse})";
                var slst =
                    (await CreateAllocationDataBlocks(perInvoice, filterExpression + exPro, entryDataDetailsIds, adjustmentType).ConfigureAwait(false))
                    .Where(
                        x => x.Allocations.Count > 0).ToList();
                if (slst.Any())
                {
                    // must use the month cuz assessment date could be 1 or 30
                    var startDate =
                        new DateTime(slst.SelectMany(x => x.Allocations).Min(x => x.EffectiveDate).GetValueOrDefault().Year,
                            slst.SelectMany(x => x.Allocations).Min(x => x.EffectiveDate).GetValueOrDefault().Month, 1);
                    var endDate = new DateTime(slst.SelectMany(x => x.Allocations).Max(x => x.EffectiveDate).GetValueOrDefault().Year,
                        slst.SelectMany(x => x.Allocations).Max(x => x.EffectiveDate).GetValueOrDefault().Month, 1).AddMonths(1).AddDays(-1);


                    var itemPiSummarylst =
                        CreateEx9Class.Instance.GetItemSalesPiSummary(docSet.ApplicationSettingsId, startDate,
                            endDate, dutyFreePaid, adjustmentType);
                    List<DocumentCT> doclst;
                    if (adjustmentType == "DIS")
                    {

                        doclst = await CreateEx9Class.Instance.CreateDutyFreePaidDocument(dutyFreePaid,
                                slst, docSet, adjustmentType, false, itemPiSummarylst, false,
                                false, false, "Current", false, false, true, perInvoice, false, false, false, "S") //ex9bucket = false because sales affect current the piquantity
                            .ConfigureAwait(
                                false);
                    }
                    else
                    {
                        doclst = await CreateEx9Class.Instance.CreateDutyFreePaidDocument(dutyFreePaid,
                                 slst, docSet, adjustmentType, false, itemPiSummarylst, true,
                                 false, false, "Historic", true, true, true, perInvoice, false, true, true, "S")
                             .ConfigureAwait(
                                 false);
                    }

                    BaseDataModel.StripAttachments(doclst, emailId);
                    BaseDataModel.AttachEmailPDF(asycudaDocumentSetId, emailId);
                    BaseDataModel.SetInvoicePerline(doclst.Select(x => x.Document.ASYCUDA_Id).ToList());
                    BaseDataModel.RenameDuplicateDocumentCodes(doclst.Select(x => x.Document.ASYCUDA_Id).ToList());

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }




        }




        private async Task<IEnumerable<AllocationDataBlock>> CreateAllocationDataBlocks(bool perInvoice, string filterExpression, List<int> entryDataDetailsIds, string adjustmentType)
        {
            try
            {
                StatusModel.Timer("Getting IM9 Data");
                var slstSource = await GetIM9Data(filterExpression, entryDataDetailsIds, adjustmentType).ConfigureAwait(false);
                StatusModel.StartStatusUpdate("Creating IM9 Entries", slstSource.Count());
                IEnumerable<AllocationDataBlock> slst;
                slst = CreateWholeAllocationDataBlocks(perInvoice, slstSource);
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(bool perInvoice,
            IEnumerable<EX9Allocations> slstSource)
        {
            IEnumerable<AllocationDataBlock> slst;
            if (perInvoice == true)
            {
                slst = CreatePerInvoiceAllocationDataBlocks(slstSource);
            }
            else
            {
                slst = CreateNonPerInvoiceAllocationDataBlocks(slstSource);
            }
            return slst;
        }

        private IEnumerable<AllocationDataBlock> CreateNonPerInvoiceAllocationDataBlocks(IEnumerable<EX9Allocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                       group s by
                           new
                           {
                               s.DutyFreePaid,
                               MonthYear = s.EffectiveDate.GetValueOrDefault().ToString("MMM-yy")
                           }
                    into g
                       select new AllocationDataBlock
                       {
                           MonthYear = g.Key.MonthYear,
                           DutyFreePaid = g.Key.DutyFreePaid,
                           Allocations = g.ToList()
                       };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreatePerInvoiceAllocationDataBlocks(IEnumerable<EX9Allocations> slstSource)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                       group s by
                           new
                           {
                               s.DutyFreePaid,
                               MonthYear = s.EffectiveDate.GetValueOrDefault().ToString("MMM-yy"),
                               s.InvoiceNo
                           }
                    into g
                       select new AllocationDataBlock
                       {
                           MonthYear = g.Key.MonthYear,
                           DutyFreePaid = g.Key.DutyFreePaid,
                           Allocations = g.ToList(),
                           CNumber = g.Key.InvoiceNo
                       };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<EX9Allocations>> GetIM9Data(string FilterExpression, List<int> entryDataDetailsIds, string adjustmentType)
        {
            FilterExpression =
                FilterExpression.Replace("&& (pExpiryDate >= \"" + DateTime.Now.Date.ToShortDateString() + "\")", "");
            var docsetid = new CoreEntitiesContext().FileTypes.First(x => x.Type == adjustmentType).AsycudaDocumentSetId;
            FilterExpression =
                Regex.Replace(FilterExpression, "AsycudaDocumentSetId == \"\\d+\"", $"AsycudaDocumentSetId == \"{docsetid}\"");

            FilterExpression += "&& (PreviousDocumentItem.DoNotAllocate != true && PreviousDocumentItem.DoNotEX != true && PreviousDocumentItem.WarehouseError == null)";



            var exp1 = AllocationsModel.Instance.TranslateAllocationWhereExpression(FilterExpression);
            var map = new Dictionary<string, string>()
            {
                {"pIsAssessed", "PreviousDocumentItem.IsAssessed"},
                {"pRegistrationDate", "PreviousDocumentItem.AsycudaDocument.RegistrationDate"},
                { "EntryDataId", "InvoiceNo" },
                {"pExpiryDate", "(DbFunctions.AddDays(PreviousDocumentItem.AsycudaDocument.RegistrationDate.GetValueOrDefault(),730))"},
                {"Invalid", "EntryDataDetails.InventoryItem.TariffCodes.Invalid"},
               // {"xBond_Item_Id == 0", "(xBond_Item_Id == null || xBond_Item_Id == 0)"}//xBondAllocations != null  && xBondAllocations.Any() == false

            };
            var exp = map.Aggregate(exp1, (current, m) => current.Replace(m.Key, m.Value));
            var res = new List<EX9Allocations>();
            using (var ctx = new AllocationDSContext())
            {
                ctx.Database.CommandTimeout = (60 * 30);
                try
                {
                    IQueryable<AdjustmentShortAllocations> pres;
                    if (FilterExpression.Contains("xBond_Item_Id == 0"))
                    {
                        //TODO: use expirydate in asycuda document
                        pres = ctx.AdjustmentShortAllocations
                            .Include(x => x.AsycudaSalesAllocationsPIData)
                            .OrderBy(x => x.AllocationId)
                            .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null)
                            .Where(x => x.xBond_Item_Id == 0);
                    }
                    else
                    {
                        pres = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                            .Include(x => x.AsycudaSalesAllocationsPIData)
                            .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                            .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null);

                        //var pres1 = ctx.AdjustmentShortAllocations.OrderBy(x => x.AllocationId)
                        //     .Where(x => x.pRegistrationDate == null || (DbFunctions.AddDays(((DateTime)x.pRegistrationDate), 730)) > DateTime.Now)
                        //     .Where(x => x.EntryDataDetails.EntryDataDetailsEx.SystemDocumentSets != null).ToList();

                    }

                    // entryDataDetailsIds = entryDataDetailsIds ?? new List<int>();

                    res = pres
                        .Where(exp)
                        .Where(x => x.Status == null)//|| x.Status == "Short Shipped"
                        .Where(x => x.xBond_Item_Id == 0 //&& x.pQuantity != 0
                            )
                        // .Where(x => !entryDataDetailsIds.Any()|| entryDataDetailsIds.Contains(x.EntryDataDetailsId))//entryDataDetailsIds.Any(z => z == x.EntryDataDetailsId)

                        .GroupJoin(ctx.xcuda_Weight_itm, x => x.PreviousItem_Id, q => q.Valuation_item_Id,
                            (x, w) => new { x, w })
                        // took this out to allow creating entries on is manually assessed entries 
                        //.Where(g => g.x.pCNumber != null) 
                        .Where(g => g.w.Any())
                        .Select(c => new EX9Allocations
                        {
                            AllocationId = c.x.AllocationId,
                            EntryData_Id = c.x.EntryDataDetails.EntryData_Id,
                            Commercial_Description =
                                c.x.PreviousDocumentItem == null
                                    ? null
                                    : c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            DutyFreePaid = c.x.DutyFreePaid,
                            EntryDataDetailsId = (int)c.x.EntryDataDetailsId,
                            InvoiceDate = c.x.InvoiceDate,
                            EffectiveDate = (DateTime)c.x.EffectiveDate,
                            InvoiceNo = c.x.InvoiceNo,
                            ItemDescription = c.x.ItemDescription,
                            ItemNumber = c.x.ItemNumber,
                            pCNumber = c.x.pCNumber,
                            pItemCost = (double)
                                        (c.x.PreviousDocumentItem.xcuda_Tarification.Item_price /
                                         c.x.PreviousDocumentItem.xcuda_Tarification.xcuda_Supplementary_unit
                                             .FirstOrDefault(z => z.IsFirstRow == true).Suppplementary_unit_quantity),
                            Status = c.x.Status,
                            PreviousItem_Id = c.x.PreviousItem_Id,
                            QtyAllocated = (double)c.x.QtyAllocated,
                            SalesFactor = c.x.PreviousDocumentItem.SalesFactor,
                            SalesQtyAllocated = (double)c.x.SalesQtyAllocated,
                            SalesQuantity = (int)c.x.SalesQuantity,
                            pItemNumber = c.x.pItemNumber,
                            pItemDescription = c.x.PreviousDocumentItem.xcuda_Goods_description.Commercial_Description,
                            pTariffCode = c.x.pTariffCode,
                            pPrecision1 = c.x.pPrecision1,
                            DFQtyAllocated = c.x.PreviousDocumentItem.DFQtyAllocated,
                            DPQtyAllocated = c.x.PreviousDocumentItem.DPQtyAllocated,
                            LineNumber = (int)c.x.EntryDataDetails.LineNumber,
                            Comment = c.x.EntryDataDetails.Comment,
                            pLineNumber = c.x.PreviousDocumentItem.LineNumber,
                            // Currency don't matter for im9 or exwarehouse
                            Customs_clearance_office_code = c.x.PreviousDocumentItem.AsycudaDocument.Customs_clearance_office_code,
                            pQuantity = (double)c.x.pQuantity,
                            pRegistrationDate = (DateTime)(c.x.pRegistrationDate ?? c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate),
                            pAssessmentDate = (DateTime)c.x.PreviousDocumentItem.AsycudaDocument.AssessmentDate,
                            pExpiryDate = (DateTime)c.x.PreviousDocumentItem.AsycudaDocument.ExpiryDate,
                            Country_of_origin_code =
                                c.x.PreviousDocumentItem.xcuda_Goods_description.Country_of_origin_code,
                            Total_CIF_itm = c.x.PreviousDocumentItem.xcuda_Valuation_item.Total_CIF_itm,
                            Net_weight_itm = c.w.FirstOrDefault().Net_weight_itm,
                            InventoryItemId = c.x.EntryDataDetails.InventoryItemId,
                            // Net_weight_itm = c.x.PreviousDocumentItem != null ? ctx.xcuda_Weight_itm.FirstOrDefault(q => q.Valuation_item_Id == x.PreviousItem_Id).Net_weight_itm: 0,
                            PIData = c.x.AsycudaSalesAllocationsPIData,
                            previousItems = c.x.PreviousDocumentItem.EntryPreviousItems
                                    .Select(y => y.xcuda_PreviousItem)
                                    .Where(y => (y.xcuda_Item.AsycudaDocument.CNumber != null || y.xcuda_Item.AsycudaDocument.IsManuallyAssessed == true) && y.xcuda_Item.AsycudaDocument.Cancelled != true)
                                    .Select(z => new previousItems()
                                    {
                                        PreviousItem_Id = z.PreviousItem_Id,
                                        DutyFreePaid =
                                            z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4074" || z.xcuda_Item.AsycudaDocument.Extended_customs_procedure == "4070"
                                                ? "Duty Paid"
                                                : "Duty Free",
                                        Net_weight = (double)z.Net_weight,
                                        Suplementary_Quantity = (double)z.Suplementary_Quantity
                                    }).ToList(),
                            TariffSupUnitLkps =
                                c.x.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.Select(x => x.TariffSupUnitLkps).ToList(),
                            FileTypeId = c.x.FileTypeId,
                            EmailId = c.x.EmailId,
                            //.Select(x => (ITariffSupUnitLkp)x)

                        }
                        )
                        //////////// prevent exwarehouse of item whos piQuantity > than AllocatedQuantity//////////

                        .ToList();

                    return res;

                }
                catch (Exception)
                {
                    throw;
                }
            }

        }


        private AsycudaDocument GetAsycudaDocumentInCNumber(int applicationSettingsId, string cNumber)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                try
                {


                    // get document item in cnumber
                    var clst = GetCNumbersFromString(cNumber);
                    var res = ctx.AsycudaDocuments
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                        .Where(x => (x.CNumber != null || x.IsManuallyAssessed == true) &&
                                    (x.Customs_Procedure.CustomsOperationId == (int)CustomsOperations.Warehouse) &&
                                    // x.WarehouseError == null && 
                                    (x.Cancelled == null || x.Cancelled == false) &&
                                    x.DoNotAllocate != true)
                        .OrderByDescending(x => x.AssessmentDate)
                        .FirstOrDefault(x => clst.Contains(x.CNumber));
                    return res;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task MatchToAsycudaItem(int entryDataDetailId, int itemId)
        {
            try
            {
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    var os = ctx.AdjustmentDetails.First(x => x.EntryDataDetailsId == entryDataDetailId);
                    var ed = ctx.EntryDataDetails.First(x => x.EntryDataDetailsId == entryDataDetailId);
                    var item = AsycudaDocumentItemCache.FirstOrDefault(x => x.Item_Id == itemId);
                    MatchToAsycudaItem(os, new List<AsycudaDocumentItem>() { item }, ed, ctx);
                    ed.Status = null;
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task CreateIM9(string filterExpression, bool perInvoice, bool process7100, int asycudaDocumentSetId, string ex9Type, string dutyFreePaid)
        {
            await CreateIM9(filterExpression, perInvoice, asycudaDocumentSetId, dutyFreePaid,
                ex9Type, null, null).ConfigureAwait(false);
        }

        private DateTime? MatchToAsycudaItem(AdjustmentDetail os, List<AsycudaDocumentItem> alst, EntryDataDetail ed, AdjustmentQSContext ctx)
        {
            //ed.Status = null;
            //ed.QtyAllocated = 0;
            DateTime? minEffectiveDate = null;
            var remainingShortQty = os.InvoiceQty.GetValueOrDefault() - os.ReceivedQty.GetValueOrDefault();

            if (!alst.Any())
            {
                //Mark here cuz Allocations don't do Discrepancies only Adjustments
                ed.Status = "No Asycuda Item Found";
                return minEffectiveDate;
            }

            /// took PiQuantity out because then entries can exist already and this just preventing it from relinking
            /// 

            ed.EffectiveDate = alst.FirstOrDefault().AsycudaDocument.AssessmentDate;
            minEffectiveDate = ed.EffectiveDate;
            if (alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.DFQtyAllocated.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error
                ed.Status = "Insufficent Quantities";
                return minEffectiveDate;
            }

            if ((ed.IsReconciled??false) == false && alst.Select(x => x.ItemQuantity.GetValueOrDefault() - x.PiQuantity.GetValueOrDefault()).Sum() <
                remainingShortQty)
            {

                var existingExecution = ctx.AsycudaDocumentItemEntryDataDetails
                    .FirstOrDefault(x => x.EntryDataDetailsId == ed.EntryDataDetailsId);

                if (existingExecution == null)
                {
                    ed.Status = "PiQuantity Already eXed-Out";
                    return minEffectiveDate;
                }

                ////Adding back Status because something is definitly wrong if qty reported is more than what entry states... 
                /// going and allocat the rest will cause allocations to an error

            }


            if (remainingShortQty < 0)
            {
                ed.AdjustmentOversAllocations.Add(new AdjustmentOversAllocation(true)
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    PreviousItem_Id = alst.First().Item_Id,
                    Asycuda_Id = alst.First().AsycudaDocumentId.GetValueOrDefault(),
                    TrackingState = TrackingState.Added

                });
                return minEffectiveDate;
            } // if overs just mark and return

            foreach (var aItem in alst)
            {
                var pitm = ctx.xcuda_Item
                    //   .Include("AsycudaSalesAllocations.EntryDataDetail.AdjustmentEx")
                    .First(x => x.Item_Id == aItem.Item_Id);
                //pitm.DFQtyAllocated = 0;

                //if (Math.Abs(pitm.AsycudaSalesAllocations.Where(x => x.EntryDataDetailsId == os.EntryDataDetailsId)
                //    .Select(x => x.QtyAllocated).Sum() - os.Quantity) < 0.001)
                //    return;

                var asycudaQty =
                    Convert.ToDouble(aItem.ItemQuantity.GetValueOrDefault() - pitm.DFQtyAllocated);

                if (asycudaQty <= 0) continue;

                var osa = new AsycudaSalesAllocation(true)
                {
                    PreviousItem_Id = aItem.Item_Id,
                    EntryDataDetailsId = os.EntryDataDetailsId,
                    //Status = "Short Shipped",
                    TrackingState = TrackingState.Added,

                };
                ctx.AsycudaSalesAllocations.Add(osa);
                if (asycudaQty >= remainingShortQty)
                {

                    osa.QtyAllocated = remainingShortQty;
                    ed.QtyAllocated += remainingShortQty;
                    pitm.DFQtyAllocated += remainingShortQty;
                    aItem.DFQtyAllocated += remainingShortQty;
                    break;
                }
                else
                {

                    osa.QtyAllocated = asycudaQty;
                    ed.QtyAllocated += asycudaQty;
                    pitm.DFQtyAllocated += asycudaQty;
                    aItem.DFQtyAllocated += asycudaQty;
                    remainingShortQty -= asycudaQty;
                }

            }


            return minEffectiveDate;

        }


        //private void SetPreviousItemXbond(AsycudaSalesAllocations ssa, AsycudaDocumentItem cAsycudaItm, string dfp, double amt)
        //{
        //    try
        //    {
        //        if (BaseDataModel.Instance.CurrentApplicationSettings.AllowEntryDoNotAllocate != "Visible") return;


        //        var alst = cAsycudaItm.EntryPreviousItems.Select(p => p.xcuda_PreviousItem)
        //            .Where(x => x.DutyFreePaid == dfp && x.QtyAllocated <= (double)x.Suplementary_Quantity)
        //            .Where(x => x.xcuda_Item != null && x.xcuda_Item.AsycudaDocument != null && x.xcuda_Item.AsycudaDocument.Cancelled != true)
        //            .OrderBy(
        //                x =>
        //                    x.xcuda_Item.AsycudaDocument.EffectiveRegistrationDate ?? Convert.ToDateTime(x.xcuda_Item.AsycudaDocument.RegistrationDate)).ToList();
        //        foreach (var pitm in alst)
        //        {

        //            var atot = (double)pitm.Suplementary_Quantity - Convert.ToDouble(pitm.QtyAllocated);
        //            if (atot == 0) continue;
        //            if (amt <= atot)
        //            {
        //                pitm.QtyAllocated += amt;
        //                var xbond = new xBondAllocations(true)
        //                {
        //                    AllocationId = ssa.AllocationId,
        //                    xEntryItem_Id = pitm.xcuda_Item.Item_Id,
        //                    TrackingState = TrackingState.Added
        //                };

        //                ssa.xBondAllocations.Add(xbond);
        //                pitm.xcuda_Item.xBondAllocations.Add(xbond);
        //                break;
        //            }
        //            else
        //            {
        //                pitm.QtyAllocated += atot;
        //                var xbond = new xBondAllocations(true)
        //                {
        //                    AllocationId = ssa.AllocationId,
        //                    xEntryItem_Id = pitm.xcuda_Item.Item_Id,
        //                    TrackingState = TrackingState.Added
        //                };
        //                ssa.xBondAllocations.Add(xbond);
        //                pitm.xcuda_Item.xBondAllocations.Add(xbond);
        //                amt -= atot;
        //            }

        //        }

        //    }
        //    catch (Exception Ex)
        //    {
        //        throw;
        //    }
        //}

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumber(string cNumber,
            int? previousCLineNumber, string itemNumber)
        {

            try
            {

                var clst = GetCNumbersFromString(cNumber);
                // get document item in cnumber
                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                && x.AsycudaDocument.CNumber != null
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.ItemNumber == itemNumber && clst.Any(z => z == x.AsycudaDocument.CNumber)
                                && x.LineNumber == (previousCLineNumber == null ? x.LineNumber : previousCLineNumber.Value.ToString()));
                var res = aItem.ToList();
                var alias = ItemAliasCache.Where(x => x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => x.AsycudaDocument.CNumber != null
                                && x.AsycudaDocument.DocumentType == "IM7"
                                && x.AsycudaDocument.ImportComplete == true
                                && alias.Contains(x.ItemNumber) && cNumber.Contains(x.AsycudaDocument.CNumber)).ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public List<InventoryItemAliasX> ItemAliasCache
        {
            get
            {
                if (_itemAliaCache != null) return _itemAliaCache;
                using (var ctx = new CoreEntitiesContext())
                {
                    _itemAliaCache = ctx.InventoryItemAliasX
                        .Include(x => x.InventoryItemsEx)
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                }

                return _itemAliaCache;
            }
        }

        private List<AsycudaDocumentItem> AsycudaDocumentItemCache
        {
            get
            {
                try
                {
                    if (_itemCache != null) return _itemCache;
                    using (var ctx = new CoreEntitiesContext())
                    {
                        ctx.Database.CommandTimeout = 0;
                        _itemCache = ctx.AsycudaDocumentItems
                            .Include(x => x.AsycudaDocument)
                            .Include(x => x.AsycudaDocumentItemEntryDataDetails)
                            .Where(x => x.AsycudaDocument.ApplicationSettingsId == BaseDataModel.Instance
                                            .CurrentApplicationSettings.ApplicationSettingsId)
                            .Where(x => (x.AsycudaDocument.CNumber != null ||
                                         x.AsycudaDocument.IsManuallyAssessed == true) &&
                                        (x.AsycudaDocument.Customs_Procedure.CustomsOperations.Name == "Warehouse") &&
                                        // x.WarehouseError == null && 
                                        (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
                                        x.AsycudaDocument.DoNotAllocate != true).ToList();
                    }

                    return _itemCache;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

        }

        private async Task<List<AsycudaDocumentItem>> GetAsycudaEntriesInCNumberReference(int applicationSettingsId,
              string cNumber, string itemNumber)
        {

            try
            {
                List<string> cnumberlst = GetCNumbersFromString(cNumber);
                var doc = AsycudaDocumentItemCache.FirstOrDefault(x => x.CNumber != null && cnumberlst.Contains(x.CNumber));
                
                if (doc == null) return new List<AsycudaDocumentItem>();
                var docref = doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal) > 0
                    ? doc.ReferenceNumber.Substring(0, doc.ReferenceNumber.LastIndexOf("-", StringComparison.Ordinal))
                    : doc.ReferenceNumber;


                var aItem = AsycudaDocumentItemCache
                    .Where(x => x.ItemNumber == itemNumber
                                && x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref));
                var res = aItem.ToList();
                var alias = ItemAliasCache.Where(x => x.ApplicationSettingsId == applicationSettingsId && x.ItemNumber.ToUpper().Trim() == itemNumber).Select(y => y.AliasName.ToUpper().Trim()).ToList();

                if (!alias.Any()) return res;

                var ae = AsycudaDocumentItemCache
                    .Where(x => alias.Contains(x.ItemNumber)
                                && x.AsycudaDocument.DocumentType == "IM7"
                                && x.AsycudaDocument.ImportComplete == true
                                && x.AsycudaDocument.ReferenceNumber.Contains(docref)).ToList();
                if (ae.Any()) res.AddRange(ae);



                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private List<string> GetCNumbersFromString(string cNumber)
        {
            var str = cNumber.ToUpper().Replace("C", "");
            var res = str.Split(',', ' ').ToList();
            return res;
        }

        public async Task ProcessDISErrorsForAllocation(int applicationSettingsId)
        {
            // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
            using (var ctx = new AdjustmentQSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;



                var lst = ctx.TODO_PreDiscrepancyErrors
                    .Where(x => x.ApplicationSettingsId == applicationSettingsId)
                    //.Where(x => x.ItemNumber == "HEL/003361001")
                    .OrderBy(x => x.EntryDataDetailsId)
                    .DistinctBy(x => x.EntryDataDetailsId)
                    .ToList();
                // looking for 'INT/YBA473/3GL'

                ProcessDISErrorsForAllocation(lst, ctx);
            }
        }

        public async Task ProcessDISErrorsForAllocation(int applicationSettingsId, string res)
        {
            try
            {
                // get Discrepancy Allocation Errors && Discrepancies that can't be Exwarehoused
                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 10;



                    var lst = ctx.TODO_PreDiscrepancyErrors.ToList()
                         .OrderBy(x => x.EntryDataDetailsId)
                        .DistinctBy(x => x.EntryDataDetailsId)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                    && res.Contains(x.EntryDataDetailsId.ToString()))
                        //.Where(x => x.ItemNumber == "HEL/003361001")

                        .ToList();
                    // looking for 'INT/YBA473/3GL'

                    ProcessDISErrorsForAllocation(lst, ctx);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static void ProcessDISErrorsForAllocation(List<TODO_PreDiscrepancyErrors> lst, AdjustmentQSContext ctx)
        {
            StatusModel.StartStatusUpdate("Preparing Discrepancy Errors for Re-Allocation", lst.Count());
            foreach (var s in lst.Where(x =>  x.IsReconciled != true))//x.ReceivedQty > x.InvoiceQty &&
            {
                var ed = ctx.EntryDataDetails.Include(x => x.AdjustmentEx).First(x => x.EntryDataDetailsId == s.EntryDataDetailsId);
                ed.EffectiveDate = ed.AdjustmentEx.InvoiceDate;// BaseDataModel.CurrentSalesInfo().Item2; reset to invoicedate because this just wrong duh make sense
                ed.QtyAllocated = 0;
                ed.Comment = $@"DISERROR:{s.Status}";
                ed.Status = null;
                ctx.Database.ExecuteSqlCommand(
                    $"delete from AsycudaSalesAllocations where EntryDataDetailsId = {ed.EntryDataDetailsId}");
            }

            ctx.SaveChanges();
        }

        public async Task AutoMatchItems(int applicationSettingsId, string strLst)
        {
            try
            {

                using (var ctx = new AdjustmentQSContext() { StartTracking = true })
                {
                    ctx.Database.CommandTimeout = 10;



                    var lst = ctx.AdjustmentDetails
                        //.Include(x => x.AdjustmentEx)
                        .Where(x => x.ApplicationSettingsId == applicationSettingsId
                                    && strLst.Contains(x.EntryDataDetailsId.ToString()))
                        //.Where(x => x.SystemDocumentSet != null)
                        // .Where(x => x.ItemNumber == "HEL/003361001")
                        //.Where(x => x.EntryDataId == "120902")
                        //.Where( x => x.EntryDataDetailsId == 545303)
                        .Where(x => x.DoNotAllocate == null || x.DoNotAllocate != true)
                        .Where(x => x.EffectiveDate == null) // take out other check cuz of existing entries 

                        .DistinctBy(x => x.EntryDataDetailsId)
                        .OrderBy(x => x.EntryDataDetailsId)
                        .ToList();

                    await DoAutoMatch(applicationSettingsId, lst, ctx);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

