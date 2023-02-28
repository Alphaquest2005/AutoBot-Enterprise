using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AllocationDS.Business.Entities;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using TrackableEntities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using InventoryItemAlias = AllocationDS.Business.Entities.InventoryItemAlias;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace WaterNut.DataSpace
{
    public static class  Utils
    {
        public const int maxRowsToFindHeader = 10;
        public const int _noOfCyclesBeforeHardExit = 60;
        public static int _oneMegaByte = 1000000;
        public static StringComparer ignoreCase = StringComparer.OrdinalIgnoreCase;



        public static List<AsycudaDocumentSet> GetDocSets(FileTypes fileType)
        {
            HashSet<AsycudaDocumentSet> docSet = new HashSet<AsycudaDocumentSet>();
            var sysDocSet = EntryDocSetUtils.GetAsycudaDocumentSet(fileType.DocSetRefernece, true);
            
            GetAsycudaDocumentSets(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId && fileType.AsycudaDocumentSetId != 0).ForEach(x => docSet.Add(x));

            var originaldocSetRefernece = GetFileTypeOriginalReference(fileType);
            if (fileType.CopyEntryData)
            {
                GetAsycudaDocumentSets( x => x.Declarant_Reference_Number == originaldocSetRefernece).ForEach(x => docSet.Add(x));
            }
            else
            {
                if (IsSystemDocSet(sysDocSet))
                {
                    docSet.Clear();
                    GetAsycudaDocumentSets(x => x.Declarant_Reference_Number == originaldocSetRefernece).ForEach(x => docSet.Add(x));

                }

            }

            if (!docSet.Any()) throw new ApplicationException("Document Set with reference not found");


            return docSet.DistinctBy(x => x.AsycudaDocumentSetId).ToList();
        }

        private static bool IsSystemDocSet(AsycudaDocumentSet asycudaDocumentSet) =>
            new DocumentDSContext().SystemDocumentSets.FirstOrDefault(x => x.Id == asycudaDocumentSet.AsycudaDocumentSetId) != null;

        private static string GetFileTypeOriginalReference(FileTypes fileType)
        {
            using (var ctx = new DocumentDSContext())
            {
                return ctx.FileTypes.First(x => x.Id == fileType.Id).DocSetRefernece;
            }
        }

        private static List<AsycudaDocumentSet> GetAsycudaDocumentSets( Expression<Func<AsycudaDocumentSet, bool>> predicate)
        {
            using (var ctx = new DocumentDSContext())
            {
                return ctx.AsycudaDocumentSets
                    .Include(x => x.SystemDocumentSet)
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(predicate).ToList();
            }
        }

        public static string GetExistingEmailId(string droppedFilePath, FileTypes fileType)
        {
            string emailId = null;

            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.AsycudaDocumentSet_Attachments.Where(x =>
                        x.Attachments.FilePath.Replace(".xlsx", "-Fixed.csv") == droppedFilePath
                        || x.Attachments.FilePath == droppedFilePath)
                    .Select(x => new { x.EmailId, x.FileTypeId }).FirstOrDefault();
                emailId = res?.EmailId ?? (fileType.EmailId == "0" || fileType.EmailId == null ? null : fileType.EmailId);
            }

            return emailId;
        }


        public static Dictionary<int, List<((string ItemNumber, int InventoryItemId) Key, List<T> Value)>> CreateItemSet<T>(List<((string ItemNumber, int InventoryItemId) Key, List<T> Value)> items)
        {


            var res = new Dictionary<int, List<((string ItemNumber, int InventoryItemId) Key, List<T> Value)>>();
            var set = 0;


            var itemsList = items.ToList();
             var inventoryItemAliases = new AllocationDSContext().InventoryItemAlias.AsNoTracking().ToList();

                while (itemsList.Any())
                {
                    set += 1;
                    var lst = new List<((string ItemNumber, int InventoryItemId) Key, List<T> Value)>();
                    var inventoryItem = itemsList.First();
                    lst.Add(inventoryItem);


                    
                    var aliaslst = inventoryItemAliases.Where(x => x.InventoryItemId == inventoryItem.Key.InventoryItemId || x.AliasItemId == inventoryItem.Key.InventoryItemId)
                        .ToList()
                        .Where(x => lst.All(z => z.Key.InventoryItemId != x.InventoryItemId))
                        .Where(x => itemsList.Any(z => z.Key.InventoryItemId == x.InventoryItemId))
                        .Select(x => (Key: (x.ItemNumber, x.InventoryItemId),
                            Value: itemsList.Where(i => i.Key.InventoryItemId == x.InventoryItemId).SelectMany(v => v.Value)
                                .ToList()))
                        .ToList();
                    if (aliaslst.Any())
                    {
                        // var AliasSetChildren = CreateItemSet(aliaslst);
                        // aliaslst.AddRange(AliasSetChildren.SelectMany(x => x.Value).ToList());
                        lst.AddRange(aliaslst);
                    }

                    
                    itemsList = MoreEnumerable.ExceptBy(itemsList, lst, x => x.Key).ToList();
                    res.Add(set, lst);
                }

                
                return res ;
            
        }

        public static List<ShipmentRiderDetails> CreatePackingList(ShipmentRider rawRider)
        {
            var invoiceLst = rawRider.ShipmentRiderDetails.Select(x => 
                (
                    x.WarehouseCode,
                    Totalpkgs : x.Pieces,
                    totalKgs : x.GrossWeightKg,
                    totalCF : x.CubicFeet,
                    Number : (x.InvoiceNumber ?? "").Contains(',')
                        ? x.InvoiceNumber.Split(',').Where(z => !string.IsNullOrEmpty(z)).ToArray()
                        : (x.InvoiceNumber ?? "").Split('/').Where(z => !string.IsNullOrEmpty(z))
                        .ToArray(),
                    Total : new string[] { }
                ))
                .ToList();
            var riderDetails = new List<ShipmentRiderDetails>();
            foreach (var i in invoiceLst)
            {
                var totalst = Enumerable.Select<string, double>(i.Total, x =>
                        Convert.ToDouble(string.IsNullOrEmpty(x) ? "1" : x.Replace("$", "").Trim()))
                    .ToList();
                var totalSum = totalst.Sum();
                var usedPieces = 0;
                var usedKgs = 0.0;
                var usedCF = 0.0;
                for (int j = 0; j < i.Number.Length; j++)
                {
                    var rate = (totalSum == 0 || j >= totalst.Count || totalst[j] == 0)
                        ? (1 / Convert.ToDouble((int)i.Number.Length))
                        : totalst[j] / totalSum;
                    var pkgs = j == i.Number.Length - 1
                        ? i.Totalpkgs - usedPieces
                        : Convert.ToInt32(i.Totalpkgs * rate);
                    if (pkgs == 0) pkgs = 1;
                    var kgs = j == i.Number.Length - 1
                        ? i.totalKgs - usedKgs
                        : Convert.ToDouble(i.totalKgs * rate);
                    var cf = j == i.Number.Length - 1
                        ? i.totalCF - usedCF
                        : Convert.ToDouble(i.totalCF * rate);

                    List<ShipmentRiderDetails> res = new List<ShipmentRiderDetails>();
                    if (usedPieces >= i.Totalpkgs)
                    {
                        var riderDetail = riderDetails.FirstOrDefault(x => x.Pieces > 1);
                        if (riderDetail != null)
                        {
                            riderDetail.Pieces -= 1;
                            res.Add(new ShipmentRiderDetails()
                            {
                                Consignee = riderDetail.Consignee,
                                Code = riderDetail.Code,
                                Shipper = riderDetail.Shipper,
                                TrackingNumber = riderDetail.TrackingNumber,
                                Pieces = 1,
                                WarehouseCode = i.WarehouseCode.Trim(), //riderDetail.WarehouseCode,
                                InvoiceNumber = i.Number[j].Trim(),
                                InvoiceTotal = j >= totalst.Count
                                    ? 0
                                    : totalst[j],
                                GrossWeightKg = kgs,
                                CubicFeet = cf,
                                TrackingState = TrackingState.Added
                            });
                        }
                        else
                        {
                            res = Enumerable.ToList<ShipmentRiderDetails>(rawRider.ShipmentRiderDetails
                                .Where(x => x.WarehouseCode == i.WarehouseCode &&
                                            (x.InvoiceNumber ?? "").Contains(i.Number[j])).Select(
                                    z =>
                                        new ShipmentRiderDetails()
                                        {
                                            Consignee = z.Consignee,
                                            Code = z.Code,
                                            Shipper = z.Shipper,
                                            TrackingNumber = z.TrackingNumber,
                                            Pieces = 0, //pkgs,
                                            WarehouseCode = z.WarehouseCode.Trim(),
                                            InvoiceNumber = i.Number[j].Trim(),
                                            InvoiceTotal = j >= totalst.Count ? 0 : totalst[j],
                                            GrossWeightKg = kgs,
                                            CubicFeet = cf,
                                            TrackingState = TrackingState.Added
                                        }));
                        }
                    }
                    else
                    {
                        res = Enumerable.ToList<ShipmentRiderDetails>(rawRider.ShipmentRiderDetails
                            .Where(x => x.WarehouseCode == i.WarehouseCode &&
                                        (x.InvoiceNumber ?? "").Contains(i.Number[j])).Select(
                                z =>
                                    new ShipmentRiderDetails()
                                    {
                                        Consignee = z.Consignee,
                                        Code = z.Code,
                                        Shipper = z.Shipper,
                                        TrackingNumber = z.TrackingNumber,
                                        Pieces = pkgs,
                                        WarehouseCode = z.WarehouseCode.Trim(),
                                        InvoiceNumber = i.Number[j].Trim(),
                                        InvoiceTotal = j >= totalst.Count ? 0 : totalst[j],
                                        GrossWeightKg = kgs,
                                        CubicFeet = cf,
                                        TrackingState = TrackingState.Added
                                    }));
                    }


                    usedCF += cf;
                    usedKgs += kgs;
                    usedPieces += pkgs;
                    riderDetails.AddRange(res);
                    if (totalst.Sum() != 0 && i.Number.Length != i.Total.Length) break;
                }
            }

            return riderDetails;
        }

        public static List<ShipmentBLDetails> CreatePackingList(ShipmentBL bl)
        {
            var blDetails = new List<ShipmentBLDetails>();
            var invoiceLst = bl.ShipmentBLDetails.Select(x =>
               (
                   WarehouseCode: x.Marks,
                   Totalpkgs: x.Quantity,
                   totalKgs: x.GrossWeightKg,
                   totalCF: x.CubicFeet,
                   Number: x.ShipmentBLInvoice.Select(z => z.InvoiceNo).ToArray(),
                   Total: new string[] { }
               ))
               .ToList();
            
            foreach (var i in invoiceLst)
            {
                var totalst = Enumerable.Select<string, double>(i.Total, x =>
                        Convert.ToDouble(string.IsNullOrEmpty(x) ? "1" : x.Replace("$", "").Trim()))
                    .ToList();
                var totalSum = totalst.Sum();
                var usedPieces = 0;
                var usedKgs = 0.0;
                var usedCF = 0.0;
                for (int j = 0; j < i.Number.Length; j++)
                {
                    var rate = (totalSum == 0 || j >= totalst.Count || totalst[j] == 0)
                        ? (1 / Convert.ToDouble((int)i.Number.Length))
                        : totalst[j] / totalSum;
                    var pkgs = j == i.Number.Length - 1
                        ? i.Totalpkgs - usedPieces
                        : Convert.ToInt32(i.Totalpkgs * rate);
                    if (pkgs == 0) pkgs = 1;
                    var kgs = j == i.Number.Length - 1
                        ? Convert.ToDouble( i.totalKgs - usedKgs)
                        : Convert.ToDouble(i.totalKgs * rate);
                    var cf = j == i.Number.Length - 1
                        ? Convert.ToDouble(i.totalCF - usedCF)
                        : Convert.ToDouble(i.totalCF * rate);

                    var res = new List<ShipmentBLDetails>();
                    if (usedPieces >= i.Totalpkgs)
                    {
                        var blDetail = blDetails.FirstOrDefault(x => x.Quantity > 1);
                        if (blDetail != null)
                        {
                            blDetail.Quantity -= 1;
                            res.Add(new ShipmentBLDetails()
                            {
                                Comments = blDetail.Comments,
                                Quantity = 1,
                                Marks = i.WarehouseCode.Trim(), //riderDetail.WarehouseCode,
                                InvoiceNumber = i.Number[j].Trim(),
                                GrossWeightKg = kgs,
                                CubicFeet = cf,
                                ShipmentBL = bl,
                                BLId = bl.Id,
                                ShipmentBLInvoice = blDetail.ShipmentBLInvoice,
                                TrackingState = TrackingState.Added
                            });
                        }
                        else
                        {
                            res = Enumerable.ToList<ShipmentBLDetails>(bl.ShipmentBLDetails
                                .Where(x => x.Marks == i.WarehouseCode &&
                                            (x.InvoiceNumber ?? "").Contains(i.Number[j])).Select(
                                    z =>
                                        new ShipmentBLDetails()
                                        {
                                            Comments = z.Comments,
                                            Quantity = 0, //pkgs,
                                            Marks = z.Marks.Trim(),
                                            InvoiceNumber = i.Number[j].Trim(),
                                            GrossWeightKg = kgs,
                                            CubicFeet = cf,
                                            ShipmentBL = bl,
                                            BLId = bl.Id,
                                            ShipmentBLInvoice = z.ShipmentBLInvoice,
                                            TrackingState = TrackingState.Added
                                        }));
                        }
                    }
                    else
                    {
                        res = Enumerable.ToList<ShipmentBLDetails>(bl.ShipmentBLDetails
                            .Where(x => x.Marks == i.WarehouseCode &&
                                        (x.InvoiceNumber ?? "").Contains(i.Number[j])).Select(
                                z =>
                                    new ShipmentBLDetails()
                                    {
                                        Comments = z.Comments,
                                        Quantity = pkgs,
                                        Marks = z.Marks.Trim(),
                                        InvoiceNumber = i.Number[j].Trim(),
                                        GrossWeightKg = kgs,
                                        CubicFeet = cf,
                                        ShipmentBL = bl,
                                        BLId = bl.Id,
                                        ShipmentBLInvoice = z.ShipmentBLInvoice,
                                        TrackingState = TrackingState.Added
                                    }));
                    }


                    usedCF += cf;
                    usedKgs += kgs;
                    usedPieces += pkgs;
                    blDetails.AddRange(res);
                    if (totalst.Sum() != 0 && i.Number.Length != i.Total.Length) break;
                }
            }

            
            return blDetails;
        }
    }
}