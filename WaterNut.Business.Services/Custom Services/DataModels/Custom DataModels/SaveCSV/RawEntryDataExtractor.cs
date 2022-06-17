using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.DataSpace
{
    public class RawEntryDataExtractor
    {
        
        public List<((dynamic EntryDataId, dynamic EntryDataDate, int AsycudaDocumentSetId, int ApplicationSettingsId,
                dynamic CustomerName, dynamic Tax, dynamic Supplier, dynamic Currency, string EmailId, int FileTypeId,
                dynamic DocumentType, dynamic SupplierInvoiceNo, dynamic PreviousCNumber, dynamic FinancialInformation,
                dynamic Vendor, dynamic PONumber, string SourceFile) EntryData, IEnumerable<EntryDataDetails>
                EntryDataDetails,
                IEnumerable<(double TotalWeight, double TotalFreight, double TotalInternalFreight, double TotalOtherCost
                    ,
                    double TotalInsurance, double TotalDeductions, double InvoiceTotal, double TotalTax, int Packages,
                    dynamic WarehouseNo)> f, IEnumerable<(dynamic ItemNumber, dynamic ItemAlias)> InventoryItems)>
            GetRawEntryData(DataFile dataFile)
        {
            var ed = dataFile.Data.Select(x => (dynamic)x)
                .GroupBy(es => (es.EntryDataId, es.EntryDataDate, es.CustomerName))
                .Select(g => (
                    EntryData: (
                        g.Key.EntryDataId,
                        g.Key.EntryDataDate,
                        AsycudaDocumentSetId:
                        dataFile.DocSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.AsycudaDocumentSetId ??
                        dataFile.DocSet.First().AsycudaDocumentSetId,
                        ApplicationSettingsId:
                        dataFile.DocSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.ApplicationSettingsId ??
                        dataFile.DocSet.First().ApplicationSettingsId,
                        g.Key.CustomerName,
                        ((dynamic)g.FirstOrDefault())?.Tax,
                        Supplier:
                        string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                            ? null
                            : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                        EmailId: dataFile.EmailId,
                        FileTypeId: dataFile.FileType.Id,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))
                        ?.FinancialInformation,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Vendor != ""))?.Vendor,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                        SourceFile: dataFile.DroppedFilePath
                    ),
                    EntryDataDetails: g.Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .Select(x => new EntryDataDetails()
                        {
                            EntryDataId = x.EntryDataId,
                            //Can't set entrydata_id here cuz this is from data
                            ItemNumber = ((string)x.ItemNumber.ToUpper()).Truncate(20),
                            ItemDescription = x.ItemDescription,
                            Cost = x.Cost ?? 0,
                            TotalCost = Convert.ToDouble((double)(x.TotalCost ?? 0.0)),
                            Quantity = Convert.ToDouble((double)(x.Quantity ?? 0.0)),
                            FileLineNumber = x.LineNumber,
                            Units = x.Units,
                            Freight = Convert.ToDouble((double)(x.Freight ?? 0.0)),
                            Weight = Convert.ToDouble((double)(x.Weight ?? 0.0)),
                            InternalFreight = Convert.ToDouble((double)(x.InternalFreight ?? 0.0)),
                            InvoiceQty = Convert.ToDouble((double)(x.InvoiceQuantity ?? 0.0)),
                            ReceivedQty = Convert.ToDouble((double)(x.ReceivedQuantity ?? 0.0)),
                            TaxAmount = x.Tax ?? 0,
                            CNumber = x.PreviousCNumber,
                            CLineNumber = (int?)x.PreviousCLineNumber,
                            PreviousInvoiceNumber = x.PreviousInvoiceNumber,
                            Comment = x.Comment,
                            InventoryItemId = x.InventoryItemId ?? 0,
                            EffectiveDate = x.EffectiveDate,
                            VolumeLiters =
                                Convert.ToDouble((double)(x.Gallons * DomainFactLibary.GalToLtrRate ??
                                                          Convert.ToDouble((double)(x.Liters ?? 0.0)))),
                        }),
                    f: g.Select(x => (
                        TotalWeight: Convert.ToDouble((double)(x.TotalWeight ?? 0.0)),
                        TotalFreight: Convert.ToDouble((double)(x.TotalFreight ?? 0.0)),
                        TotalInternalFreight: Convert.ToDouble((double)(x.TotalInternalFreight ?? 0.0)),
                        TotalOtherCost: Convert.ToDouble((double)(x.TotalOtherCost ?? 0.0)),
                        TotalInsurance: Convert.ToDouble((double)(x.TotalInsurance ?? 0.0)),
                        TotalDeductions: Convert.ToDouble((double)(x.TotalDeductions ?? 0.0)),
                        InvoiceTotal: Convert.ToDouble((double)(x.InvoiceTotal ?? 0.0)),
                        TotalTax: Convert.ToDouble((double)(x.TotalTax ?? 0.0)),
                        Packages: Convert.ToInt32((int)(x.Packages ?? 0)),
                        x.WarehouseNo
                    )),
                    InventoryItems: g.DistinctBy(x => (x.ItemNumber, x.ItemAlias))
                        .Select(x => (x.ItemNumber, x.ItemAlias))
                )).ToList();


            return ed;
        }
    }
}