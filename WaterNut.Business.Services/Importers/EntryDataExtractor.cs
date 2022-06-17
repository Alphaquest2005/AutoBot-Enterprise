using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Extensions;
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq.Extensions;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace WaterNut.Business.Services.Importers
{
    public class EntryDataExtractor : IDataExtractor
    {
        private readonly FileTypes _fileType;
        private readonly List<AsycudaDocumentSet> _docSet;
        private readonly string _emailId;
        private readonly string _droppedFilePath;
        

        public EntryDataExtractor(FileTypes fileType, List<AsycudaDocumentSet> docSet,
            string emailId, string droppedFilePath)
        {
            _fileType = fileType;
            _docSet = docSet;
            _emailId = emailId;
            _droppedFilePath = droppedFilePath;
        }
        public List<dynamic> Execute()
        {
            return new List<dynamic>() { new BetterExpando() };
        }

        public List<dynamic> Execute(List<dynamic> list)
        {
            return ConvertCSVToEntryData(list);
        }


        private List<dynamic> ConvertCSVToEntryData(List<dynamic> eslst)
        {


            var ed = eslst.Select(x => (dynamic)x)
                  .GroupBy(es => (es.EntryDataId, es.EntryDataDate, es.CustomerName))
                  .Select(g => (
                      EntryData: (
                          g.Key.EntryDataId,
                          g.Key.EntryDataDate,
                          AsycudaDocumentSetId:
                          _docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.AsycudaDocumentSetId ??
                          _docSet.First().AsycudaDocumentSetId,
                          ApplicationSettingsId:
                          _docSet.FirstOrDefault(x => x.SystemDocumentSet == null)?.ApplicationSettingsId ??
                          _docSet.First().ApplicationSettingsId,
                          g.Key.CustomerName,
                          ((dynamic)g.FirstOrDefault())?.Tax,
                          Supplier:
                              string.IsNullOrEmpty(g.Max(x => ((dynamic)x).SupplierCode))
                                  ? null
                                  : g.Max(x => ((dynamic)x).SupplierCode?.ToUpper()),
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Currency != ""))?.Currency,
                          EmailId: _emailId,
                          FileTypeId: _fileType.Id,
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).DocumentType != ""))?.DocumentType,
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).SupplierInvoiceNo != ""))?.SupplierInvoiceNo,
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PreviousCNumber != ""))?.PreviousCNumber,
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).FinancialInformation != ""))
                              ?.FinancialInformation,
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).Vendor != ""))?.Vendor,
                          ((dynamic)g.FirstOrDefault(x => ((dynamic)x).PONumber != ""))?.PONumber,
                          SourceFile: _droppedFilePath
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
                                  Convert.ToDouble(x.Gallons * DomainFactLibary.GalToLtrRate ??
                                                   Convert.ToDouble((double)(x.Liters ?? 0.0))),
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


            


            var res = new List<dynamic>();
            //TODO: was feeling sick so didn't bother to fixe this code

            foreach (var itm in ed)
            {
                dynamic header = (IDictionary<string, object>)new BetterExpando();
                header.EntryDataId = itm.EntryData.EntryDataId;
                header.EntryDataDate = itm.EntryData.EntryDataDate;
                header.AsycudaDocumentSetId = itm.EntryData.AsycudaDocumentSetId;
                header.ApplicationSettingsId = itm.EntryData.ApplicationSettingsId;
                header.CustomerName = itm.EntryData.CustomerName;
                header.Tax = itm.EntryData.Tax;
                header.Supplier = itm.EntryData.Supplier;
                header.Currency = itm.EntryData.Currency;
                header.EmailId = itm.EntryData.EmailId;
                header.FileTypeId = itm.EntryData.FileTypeId;
                header.DocumentType = itm.EntryData.DocumentType;
                header.SupplierInvoiceNo = itm.EntryData.SupplierInvoiceNo;
                header.PreviousCNumber = itm.EntryData.PreviousCNumber;
                header.FinancialInformation = itm.EntryData.FinancialInformation;
                header.Vendor = itm.EntryData.Vendor;
                header.PONumber = itm.EntryData.PONumber;
                header.SourceFile = itm.EntryData.SourceFile;
                header.SourceFile = itm.EntryData.SourceFile;


                header.TotalWeight = itm.f.Sum(x => (double)x.TotalWeight);
                header.TotalFreight = itm.f.Sum(x => (double)x.TotalFreight);
                header.TotalInternalFreight = itm.f.Sum(x => (double)x.TotalInternalFreight);
                header.TotalOtherCost = itm.f.Sum(x => (double)x.TotalOtherCost);
                header.TotalInsurance = itm.f.Sum(x => (double)x.TotalInsurance);
                header.TotalDeductions = itm.f.Sum(x => (double)x.TotalDeductions);
                header.InvoiceTotal = itm.f.Sum(x => (double)x.InvoiceTotal);
                header.TotalTax = itm.f.Sum(x => (double)x.TotalTax);

                header.Packages = itm.f.Sum(x => (double)x.Packages);
                header.WarehouseNo = itm.f.Select(x => x.WarehouseNo).Aggregate((o,n) => $"{o}, {n}");

                header.SupplierCode = itm.EntryData.Supplier;



                dynamic entryDataDetails = new List<IDictionary<string, object>>();

                foreach (var entryDataDetail in itm.EntryDataDetails)
                {
                    if (string.IsNullOrEmpty(entryDataDetail.ItemNumber)) return null;
                    dynamic edd = (IDictionary<string, object>)new BetterExpando();
                    edd.EntryDataId = entryDataDetail.EntryDataId;
                    edd.ItemNumber = entryDataDetail.ItemNumber;
                    edd.ItemDescription = entryDataDetail.ItemDescription;
                    edd.Quantity = entryDataDetail.Quantity;
                    edd.Cost = entryDataDetail.Cost;
                    edd.TotalCost = entryDataDetail.TotalCost;
                    edd.FileLineNumber = entryDataDetail.FileLineNumber;

                    edd.Units = entryDataDetail.Units;
                    edd.Freight = entryDataDetail.Freight;
                    edd.Weight = entryDataDetail.Weight;
                    edd.InternalFreight = entryDataDetail.InternalFreight;
                    edd.InvoiceQty = entryDataDetail.InvoiceQty;
                    edd.ReceivedQty = entryDataDetail.ReceivedQty;
                    edd.TaxAmount = entryDataDetail.TaxAmount;
                    edd.CNumber = entryDataDetail.CNumber;
                    edd.CLineNumber = entryDataDetail.CLineNumber;

                    edd.PreviousInvoiceNumber = entryDataDetail.PreviousInvoiceNumber;
                    edd.Comment = entryDataDetail.Comment;
                    edd.InventoryItemId = entryDataDetail.InventoryItemId;
                    edd.EffectiveDate = entryDataDetail.EffectiveDate;
                    edd.VolumeLiters = entryDataDetail.VolumeLiters;


                    entryDataDetails.Add((IDictionary<string, object>)edd);
                }

                
                header.EntryDataDetails = entryDataDetails;

                res.Add(new List<IDictionary<string, object>>() { (IDictionary<string, object>)header });
            }


            return res;
        }
    }
}