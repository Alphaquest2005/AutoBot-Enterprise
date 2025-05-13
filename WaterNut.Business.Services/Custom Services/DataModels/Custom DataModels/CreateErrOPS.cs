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
using java.lang.@ref;
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


        public async Task CreateErrorOPS(string filterExpression, AsycudaDocumentSet docSet,  bool perInvoice)
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


                var cslst = perInvoice ? AllocationEntryLinePerInvoice(slst) : AllocationEntryLine(slst);


                var monthyearLst = perInvoice
                    ? cslst
                        .GroupBy(x => (x.MonthYear, x.EntryDataId, x.SourceFile))
                        .OrderBy(x => x.Key.MonthYear)
                        .ThenBy(x => x.Key.EntryDataId)
                        .ToList()
                    : cslst
                        .GroupBy(x => (x.MonthYear, EntryDataId: x.MonthYear, x.SourceFile))
                        .OrderBy(x => x.Key.MonthYear)
                        .ToList();

                string prevEntryId = "";

                var cdoc = new DocumentCT
                {
                    Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet)
                };


                docSet.Customs_Procedure = cp;

                ErrOpsIntializeCdoc(cdoc, docSet);
                var itmcount = 0;

                List<Attachment> attachments = new List<Attachment>();



                foreach (var lst in monthyearLst)
                {


                    cdoc = CreateEntry(docSet);

                    var savedEntryData = CreateOPSEntryData(lst, perInvoice);

                    attachments = AddAttachments(cdoc, exportTemplate, lst);

                    foreach (var pod in savedEntryData)
                    {
                        StatusModel.StatusUpdate();

                        BaseDataModel.Instance.CreateItemFromEntryDataDetail(pod, cdoc);


                        itmcount += 1;

                        if ((BaseDataModel.Instance.MaxLineCount(itmcount)
                                // || (!string.IsNullOrEmpty(prevEntryId) && BaseDataModel.Instance.InvoicePerEntry(perInvoice, prevEntryId, pod.EntryDataId))
                            )

                           )
                        {
                            await SaveEntry(attachments, cdoc).ConfigureAwait(false);

                            cdoc = CreateEntry(docSet);
                        }





                        prevEntryId = pod.EntryDataDetails.Count() > 0
                            ? pod.EntryDataId
                            : "";

                    }

                    if (cdoc.DocumentItems.Count == 0)
                    {
                        cdoc = null;
                        return;
                    }

                    await SaveEntry(attachments, cdoc).ConfigureAwait(false);

                }

                //await SaveEntry(attachments, cdoc).ConfigureAwait(false);

                //BaseDataModel.SetEffectiveAssessmentDate(cdoc);
                //BaseDataModel.AttachToDocument(attachments,
                //    cdoc.Document, cdoc.DocumentItems);
                //await BaseDataModel.Instance.SaveDocumentCT(cdoc).ConfigureAwait(false);
                //StatusModel.StopStatusUpdate();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

     

        private DocumentCT CreateEntry(AsycudaDocumentSet docSet)
        {
            var cdoc = new DocumentCT
            {
                Document = BaseDataModel.Instance.CreateNewAsycudaDocument(docSet)
            };

            ErrOpsIntializeCdoc(cdoc, docSet);
            return cdoc;
        }

        private static async Task SaveEntry(List<Attachment> attachments, DocumentCT cdoc)
        {
            if (cdoc.DocumentItems.Any() && cdoc.Document != null)
            {
                BaseDataModel.SetEffectiveAssessmentDate(cdoc);
                //attachments = AddAttachments(savedEntryData, exportTemplate, lst);
                await BaseDataModel.AttachToDocument(attachments,
                    cdoc.Document, cdoc.DocumentItems).ConfigureAwait(false);
                await BaseDataModel.Instance.SaveDocumentCt.Execute(cdoc).ConfigureAwait(false);
                //dup new file
            }
        }

        private static List<Attachment> AddAttachments(DocumentCT cdoc, ExportTemplate exportTemplate, IGrouping<(string MonthYear, string EntryDataId, string SourceFile), AllocationsModel.AlloEntryLineData> lst)
        {
            var attachments = new List<Attachment>()
            {
                new Attachment()
                {
                    FilePath = $"{cdoc.Document.ReferenceNumber}.pdf",
                    DocumentCode = exportTemplate.AttachedDocumentCode,
                    Reference = cdoc.Document.ReferenceNumber,
                    TrackingState = TrackingState.Added,
                }
            };

            var sourceFile = new FileInfo(lst.Key.SourceFile);
            var sourceFilePdf = sourceFile.FullName.Replace(sourceFile.Extension, ".pdf");
            sourceFilePdf = sourceFilePdf.Replace("-Fixed", "");
            if (File.Exists(sourceFilePdf))
                attachments.Add(new Attachment(true)
                {
                    FilePath = sourceFilePdf,
                    TrackingState = TrackingState.Added,
                    DocumentCode = @"IV05",
                    //EmailId = lineData.EmailId?.ToString(),
                    Reference = lst.Key.EntryDataId + ".pdf",
                });

            attachments.Add(   new Attachment()
                {
                    FilePath = $"",
                    DocumentCode = "DC05",
                    Reference = "NA",
                    TrackingState = TrackingState.Added,
                 });


            return attachments;
        }

        private List<AllocationsModel.AlloEntryLineData> CreateOPSEntryData(
            IGrouping<(string MonthYear, string EntryDataId, string SourceFile), AllocationsModel.AlloEntryLineData>
                lst, bool perInvoice)
        {
            using (var ctx = new EntryDataDSContext())
            {
                var lines = lst.ToList();
                var opsNumber = $"ERROPS-{(perInvoice == true ? lst.Key.EntryDataId : lst.Key.MonthYear)}";

                var oops = ctx.EntryData.OfType<OpeningStock>().FirstOrDefault(x => x.OPSNumber == opsNumber);

                var ops = new OpeningStock();

                    if(oops != null)
                    {
                        ctx.EntryData.Remove(oops);
                    }
                 
                    ops = CreateOpeningStock(lst, opsNumber, ctx);
              
                
                

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
                        EntryDataDetailsKey = $"ERROPS-{lst.Key.EntryDataId}|{lines.IndexOf(line) + 1}",
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

        private static OpeningStock CreateOpeningStock(IGrouping<(string MonthYear, string EntryDataId, string SourceFile), AllocationsModel.AlloEntryLineData> lst, string opsNumber, EntryDataDSContext ctx)
        {
            OpeningStock ops;
            ops = new OpeningStock(true)
            {
                OPSNumber = opsNumber,
                EntryDataId = opsNumber,
                EntryDataDate = lst.Min(x => x.EntryDataDate),
                ApplicationSettingsId =
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                EntryType = "OPS",
                TrackingState = TrackingState.Added,
            };
            ctx.EntryData.Add(ops);
            return ops;
        }

        private IEnumerable<AllocationsModel.AlloEntryLineData> AllocationEntryLinePerInvoice(List<AsycudaSalesAllocations> slst)
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
                            s.EntryDataDetails.EntryData.EntryDataId,
                            s.EntryDataDetails.EntryData.SourceFile,
                            MonthYear = s.EntryDataDetails.EntryData.EntryDataDate.ToString("MM-yyyy")
                            //,
                            //  s.EntryDataDetails.InventoryItem
                        }
                into g
                        select new AllocationsModel.AlloEntryLineData
                        {
                            MonthYear = g.Key.MonthYear,
                            EntryDataId = g.Key.EntryDataId,
                            SourceFile = g.Key.SourceFile,
                            ItemNumber = g.Key.ItemNumber,
                            ItemDescription = g.Key.ItemDescription,
                            Cost = g.Key.Cost,
                            EntryDataDate = g.Min(x => x.EntryDataDetails.EntryData.EntryDataDate),
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
                                SourceFile =x.EntryDataDetails.EntryData.SourceFile,
                                ItemNumber = x.EntryDataDetails.ItemNumber,
                                ItemDescription = x.EntryDataDetails.ItemDescription,
                                Cost = x.EntryDataDetails.Cost,
                                Quantity = x.QtyAllocated,
                                EffectiveDate = x.EntryDataDetails.EffectiveDate.GetValueOrDefault(),
                                EntryDataDate = x.EntryDataDetails.EntryData.EntryDataDate,
                                LineNumber = x.EntryDataDetails.LineNumber,
                                InventoryItemId = x.EntryDataDetails.InventoryItemId,
                                Comment = $"{x.Status}-{x.xStatus}"

                            }).ToList()
                        };
            return cslst.Where(x => x.Quantity > 0);
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
                           // s.EntryDataDetails.EntryData.EntryDataId,
                           // s.EntryDataDetails.EntryData.SourceFile,
                            MonthYear = s.EntryDataDetails.EntryData.EntryDataDate.ToString("MM-yyyy")
                            //,
                            //  s.EntryDataDetails.InventoryItem
                        }
                into g
                        select new AllocationsModel.AlloEntryLineData
                        {
                            MonthYear = g.Key.MonthYear,
                            EntryDataId = g.First().EntryDataDetails.EntryData.EntryDataId,
                            SourceFile = g.First().EntryDataDetails.EntryData.SourceFile,
                            ItemNumber = g.Key.ItemNumber,
                            ItemDescription = g.Key.ItemDescription,
                            Cost = g.Key.Cost,
                            EntryDataDate = g.Min(x => x.EntryDataDetails.EntryData.EntryDataDate),
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
                                SourceFile = x.EntryDataDetails.EntryData.SourceFile,
                                ItemNumber = x.EntryDataDetails.ItemNumber,
                                ItemDescription = x.EntryDataDetails.ItemDescription,
                                Cost = x.EntryDataDetails.Cost,
                                Quantity = x.QtyAllocated,
                                EffectiveDate = x.EntryDataDetails.EffectiveDate.GetValueOrDefault(),
                                EntryDataDate = x.EntryDataDetails.EntryData.EntryDataDate,
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