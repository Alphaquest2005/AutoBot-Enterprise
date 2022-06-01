using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using AllocationQS.Business.Entities;
using Core.Common.UI;
using CoreEntities.Business.Enums;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using EntryData = EntryDataDS.Business.Entities.EntryData;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;


namespace WaterNut.DataSpace
{
    public class CreateErrOPS
    {
        private static readonly CreateErrOPS _instance;
        static CreateErrOPS()
        {
            _instance = new CreateErrOPS();
        }

        public static CreateErrOPS Instance
        {
            get { return _instance; }
        }


        public async Task CreateErrorOPS(string filterExpression, AsycudaDocumentSet docSet)
        {
            try
            {

                StatusModel.Timer("Getting Data ...");



                var cp =
                    BaseDataModel.Instance.Customs_Procedures
                        .Single(x =>
                            x.CustomsOperationId == (int)CustomsOperations.Warehouse && x.Stock == true &&
                            x.IsDefault == true);

               

                var exportTemplate =
                    BaseDataModel.Instance.ExportTemplates.First(z => z.Customs_Procedure == cp.CustomsProcedure);

                docSet.Customs_Procedure = cp;

                // inject custom procedure in docset
                BaseDataModel.ConfigureDocSet(docSet, exportTemplate);

                var slst = await GetErrOPSData(filterExpression).ConfigureAwait(false);


                StatusModel.StartStatusUpdate("Creating Error OPS entries", slst.Count());


                var cslst = AllocationEntryLine(slst);

                var monthyearLst = cslst
                    .GroupBy(x => x.MonthYear).ToList();

                foreach (var lst in monthyearLst)
                {
                    var cdoc = new DocumentCT
                    {
                        Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet)
                    };


                    docSet.Customs_Procedure = cp;

                    ErrOpsIntializeCdoc(cdoc, docSet);
                    var itmcount = 0;


                    var savedEntryData = CreateOPSEntryData(lst);

                    var attachments = new List<Attachment>() { new Attachment()
                    {
                        FilePath = $"{savedEntryData.First().EntryDataDetails.First().EntryDataId}.csv.pdf",
                        DocumentCode = exportTemplate.AttachedDocumentCode,
                        Reference = savedEntryData.First().EntryDataDetails.First().EntryDataId,
                        TrackingState = TrackingState.Added,

                    }};


                    foreach (var pod in savedEntryData)
                    {
                        StatusModel.StatusUpdate();

                        BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod, cdoc);


                        itmcount += 1;
                        if (itmcount % BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines == 0)
                        {
                            BaseDataModel.SetEffectiveAssessmentDate(cdoc);
                            
                            BaseDataModel.AttachToDocument(attachments,
                                cdoc.Document, cdoc.DocumentItems);
                            await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                            //dup new file
                            cdoc = new DocumentCT
                            {
                                Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet)
                            };

                            ErrOpsIntializeCdoc(cdoc, docSet);

                        }

                    }

                    if (cdoc.DocumentItems.Count == 0)
                    {
                        cdoc = null;
                        return;
                    }

                    BaseDataModel.SetEffectiveAssessmentDate(cdoc);
                    BaseDataModel.AttachToDocument(attachments,
                        cdoc.Document, cdoc.DocumentItems);
                    await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                    StatusModel.StopStatusUpdate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private List<AllocationsModel.AlloEntryLineData> CreateOPSEntryData(
            IGrouping<string, AllocationsModel.AlloEntryLineData> lst)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var lines = lst.ToList();
                var opsNumber = $"ERROPS-{lst.Key}";

                var oops = ctx.EntryData.OfType<OpeningStock>().FirstOrDefault(x => x.OPSNumber == opsNumber);

                if (oops != null)
                {
                    ctx.EntryData.Remove(oops);
                }

                var ops = new OpeningStock(true)
                    {
                        OPSNumber = opsNumber,
                        EntryDataId = opsNumber,
                        EntryDataDate = lst.Min(x => x.EntryDataDate),
                        ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        EntryType = "OPS",
                        TrackingState = TrackingState.Added,

                    };
                    ctx.EntryData.Add(ops);
                

                foreach (var line in lines)
                {
                    var eParent = new EntryDataDetails()
                    {
                        EntryData_Id = ops.EntryData_Id,
                        TrackingState = TrackingState.Added,
                        EntryDataId = ops.EntryDataId,
                        ItemNumber = line.ItemNumber,
                        ItemDescription = line.ItemDescription,
                        Cost = line.Cost,
                        Quantity = line.Quantity,
                        LineNumber = lines.IndexOf(line) + 1,
                        InventoryItemId = line.InventoryItemId,
                        EntryDataDetailsKey = $"ERROPS-{lst.Key}|{lines.IndexOf(line) + 1}",
                        EntryData = ops


                    };
                    ctx.EntryDataDetails.Add(eParent);
                    foreach (var itm in line.EntryDataDetails)
                    {
                        var eDetail = new SupportingDetail()
                        {
                            EntryDataId = itm.EntryDataId,
                            TrackingState = TrackingState.Added,
                            EntryData_Id = itm.EntryData_Id,
                            
                            
                            Cost = itm.Cost,
                            Quantity = itm.Quantity,
                            LineNumber = itm.LineNumber,
                            EntryDataDetailsKey = $"{itm.EntryDataId}|{itm.LineNumber}",
                            Comment = itm.Comment,
                            PreviousInvoiceNumber = itm.EntryDataId,
                            PreviousEntryDataDetailsId = itm.EntryDataDetailsId,
                            EntryDataDetails = eParent,

                        };
                        eParent.SupportingDetails.Add(eDetail);

                    }

                    ctx.SaveChanges();
                    line.EntryDataDetails.ForEach(x =>
                    {
                        x.EntryDataDetailsId = eParent.EntryDataDetailsId;
                        x.EntryData_Id = eParent.EntryData_Id;
                        x.EntryDataId = opsNumber;
                    });
                    

                }





                return lines;
            }
        }

        private IEnumerable<AllocationsModel.AlloEntryLineData> AllocationEntryLine(List<AsycudaSalesAllocations> slst)
        {
            var cslst = from s in slst
                        where
                            s.EntryDataDetails.EntryDataId != null 
                            //||
                            //s.EntryDataDetails.Quantity != s.EntryDataDetails.QtyAllocated
                        group s by new
                        {
                            
                            s.EntryDataDetails.ItemNumber,
                            s.EntryDataDetails.ItemDescription,
                            s.EntryDataDetails.Cost,
                            s.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCode,
                            s.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.InventoryItemId,
                            MonthYear = s.EntryDataDetails.Sales.EntryDataDate.ToString("MM-yyyy")
                            //,
                            //  s.EntryDataDetails.InventoryItem
                        }
                into g
                        select new AllocationsModel.AlloEntryLineData
                        {
                            MonthYear = g.Key.MonthYear,
                            ItemNumber = g.Key.ItemNumber,
                            ItemDescription = g.Key.ItemDescription,
                            Cost = g.Key.Cost,
                            EntryDataDate = g.Min(x => x.EntryDataDetails.Sales.EntryDataDate),
                            TariffCode = g.Key.TariffCode,
                            InventoryItem = g.First().EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx as IInventoryItem,
                            InventoryItemId = g.First().EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.InventoryItemId,


                            Quantity = g.Sum(x => x.QtyAllocated),
                            EntryDataDetails = g.Select(x => new EntryDataDetailSummary()
                            {
                                AllocationId = x.AllocationId,
                                EntryDataDetailsId = x.EntryDataDetailsId.GetValueOrDefault(),
                                EntryDataId = x.EntryDataDetails.EntryDataId,
                                EntryData_Id = x.EntryDataDetails.EntryData_Id,
                                ItemNumber = x.EntryDataDetails.ItemNumber,
                                ItemDescription = x.EntryDataDetails.ItemDescription,
                                Cost = x.EntryDataDetails.Cost,
                                Quantity = x.QtyAllocated,
                                EffectiveDate = x.EntryDataDetails.EffectiveDate.GetValueOrDefault(),
                                EntryDataDate = x.EntryDataDetails.Sales.EntryDataDate,
                                LineNumber = x.EntryDataDetails.LineNumber,
                                InventoryItemId = x.EntryDataDetails.InventoryItemId,
                                Comment = $"{x.Status}-{x.xStatus}"

                            }).ToList()
                        };
            return cslst.Where(x => x.Quantity > 0);
        }


       

        private async Task<List<AsycudaSalesAllocations>> GetErrOPSData(string filterExpression)
        {
            var lst = await AllocationsModel.Instance.GetAsycudaSalesAllocations(filterExpression).ConfigureAwait(false);


            return BaseDataModel.Instance.CurrentApplicationSettings.ExportNullTariffCodes ?? true
                ?
                lst.Where(x => (!string.IsNullOrEmpty(x.Status)
                                || x.xStatus != "Net Weight < 0.01" && !string.IsNullOrEmpty(x.xStatus)))
                    .ToList()
                : lst.Where(x => (!string.IsNullOrEmpty(x.Status)
                                  || x.xStatus != "Net Weight < 0.01" && !string.IsNullOrEmpty(x.xStatus)))
                    .Where(x => !string.IsNullOrEmpty(x.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCode) && Regex.Match(x.EntryDataDetails.EntryDataDetailsEx.InventoryItemsEx.TariffCode, "\\d{8}")
                        .Success)
                    .ToList();
        }

        public void ErrOpsIntializeCdoc(DocumentCT cdoc, AsycudaDocumentSet ads)
        {
          
            BaseDataModel.Instance.IntCdoc(cdoc, ads);
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = "Error Opening Stock Entries";
            cdoc.Document.xcuda_Declarant.Number = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number + "-ERROPS" + "-F" + cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString();

           

        }


        

    }
}