using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using Core.Common.Utils;
using MoreLinq;

namespace WaterNut.DataSpace
{
    public class CSVToShipmentInvoiceConverter
    {
        public List<dynamic> ConvertCSVToShipmentInvoice(List<dynamic> eslst)
        {
           

            var ed = (eslst.Select(x => (dynamic)x)
                .GroupBy(es => new { es.EntryDataId, es.EntryDataDate, es.CustomerName })
                .Select(g => new
                {
                    EntryData = new
                    {
                        g.Key.EntryDataId,
                        g.Key.EntryDataDate,
                        g.Key.CustomerName,
                        ((dynamic)g.FirstOrDefault())?.Tax,
                        Supplier =
                            string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                                ? null
                                : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))?.FinancialInformation,
                        ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                    },
                    EntryDataDetails = g.Where(x => !string.IsNullOrEmpty(x.ItemNumber))
                        .Select(x => new
                        {
                            x.EntryDataId,
                            //Can't set entrydata_id here cuz this is from data
                            ItemNumber = ((string)x.ItemNumber.ToUpper()).Truncate(20),
                            x.ItemDescription,
                            x.SupplierItemNumber,
                            x.SupplierItemDescription,
                            Cost = x.Cost ?? 0,
                            TotalCost = Convert.ToDouble((double)(x.TotalCost ?? 0.0)),
                            Quantity = Convert.ToDouble((double)(x.Quantity ?? 0.0)),
                            FileLineNumber = x.LineNumber,
                            x.Units,
                            Freight = Convert.ToDouble((double)(x.Freight ?? 0.0)),
                            Weight = Convert.ToDouble((double)(x.Weight ?? 0.0)),
                            InternalFreight = Convert.ToDouble((double)(x.InternalFreight ?? 0.0)),
                            InvoiceQty = Convert.ToDouble((double)(x.InvoiceQuantity ?? 0.0)),
                            ReceivedQty = Convert.ToDouble((double)(x.ReceivedQuantity ?? 0.0)),
                            TaxAmount = x.Tax ?? 0,
                            CNumber = x.PreviousCNumber,
                            CLineNumber = (int?)x.PreviousCLineNumber,
                            x.PreviousInvoiceNumber,
                            x.Comment,
                            InventoryItemId = x.ItemId ?? 0,
                            x.EffectiveDate,
                            VolumeLiters =
                                Convert.ToDouble(
                                    x.Gallons * DomainFactLibary.GalToLtrRate ?? Convert.ToDouble((double)(x.Liters ?? 0.0))),
                        }),
                    f = g.Select(x => new
                    {
                        TotalWeight = Convert.ToDouble((double)(x.TotalWeight ?? 0.0)),
                        TotalFreight = Convert.ToDouble((double)(x.TotalFreight ?? 0.0)),
                        TotalInternalFreight = Convert.ToDouble((double)(x.TotalInternalFreight ?? 0.0)),
                        TotalOtherCost = Convert.ToDouble((double)(x.TotalOtherCost ?? 0.0)),
                        TotalInsurance = Convert.ToDouble((double)(x.TotalInsurance ?? 0.0)),
                        TotalDeductions = Convert.ToDouble((double)(x.TotalDeductions ?? 0.0)),
                        InvoiceTotal = Convert.ToDouble((double)(x.InvoiceTotal ?? 0.0)),
                        Packages = Convert.ToInt32((int)(x.Packages ?? 0)),
                        x.WarehouseNo,
                    }),
                    InventoryItems = g.DistinctBy(x => new { x.ItemNumber, x.ItemAlias })
                        .Select(x => new { x.ItemNumber, x.ItemAlias })
                })).ToList();


            var res = new List<dynamic>();
            foreach (var itm in ed)
            {
                dynamic header = (IDictionary<string, object>)new BetterExpando();
                header.InvoiceNo = itm.EntryData.SupplierInvoiceNo;
                header.PONumber = itm.EntryData.EntryDataId;
                header.InvoiceDate = itm.EntryData.EntryDataDate;
                header.InvoiceTotal = itm.f.Sum(x => (double)x.InvoiceTotal);
                header.TotalInternalFreight = itm.f.Sum(x => (double)x.TotalInternalFreight);
                header.TotalOtherCost = itm.f.Sum(x => (double)x.TotalOtherCost);
                header.TotalDeduction = itm.f.Sum(x => (double)x.TotalDeductions);
                header.SupplierCode = itm.EntryData.Supplier;


                dynamic extraInfo = (IDictionary<string, object>)new BetterExpando();
                extraInfo.PONumber = itm.EntryData.PONumber;

                dynamic invoiceDetails = new List<IDictionary<string, object>>();

                foreach (var entryDataDetail in itm.EntryDataDetails)
                {
                    if (string.IsNullOrEmpty(entryDataDetail.SupplierItemNumber)) return null;
                    dynamic edd = (IDictionary<string, object>)new BetterExpando();
                    edd.ItemNumber = entryDataDetail.SupplierItemNumber;
                    edd.ItemDescription = entryDataDetail.SupplierItemDescription;
                    edd.Quantity = entryDataDetail.Quantity;
                    edd.Cost = entryDataDetail.Cost;
                    edd.TotalCost = entryDataDetail.TotalCost;
                    edd.FileLineNumber = entryDataDetail.FileLineNumber;

                    invoiceDetails.Add((IDictionary<string, object>)edd);
                }

                header.ExtraInfo = new List<IDictionary<string, object>>() { extraInfo };
                header.InvoiceDetails = invoiceDetails;

                res.Add(new List<IDictionary<string, object>>(){(IDictionary<string, object>)header });
            }


            return res;
        }
    }
}