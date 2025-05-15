using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.UI;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentItemDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using MoreLinq.Extensions;
using TrackableEntities;
using WaterNut.Business.Entities;
using WaterNut.Interfaces;
using AsycudaDocument_Attachments = DocumentDS.Business.Entities.AsycudaDocument_Attachments;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public bool ValidateInstallation()
    {
        // if (DateTime.Now >= DateTime.Parse("4/11/2019")) return false;

        return true;

        //return true;
        //using (var ctx = new CoreEntitiesContext())
        //{

        //    //if (Environment.MachineName.ToLower() == "alphaquest-PC".ToLower())return true;
        //    //if (Environment.MachineName.ToLower() == "Joseph-PC".ToLower()) return true;

        //    //    if (Environment.ProcessorCount == 4 && Environment.MachineName.ToLower() == "Alister-PC".ToLower()
        //    //    && ctx.Database.Connection.ConnectionString.ToLower().Contains(@"Alister-PC\SQLEXPRESS2017;Initial Catalog=IWWDB-Enterprise".ToLower()))
        //    //{
        //    //    return true;
        //    //}

        //    //if (Environment.MachineName.ToLower() == "DESKTOP-JP7GRGD".ToLower())return true;


        //    //if (Environment.MachineName.ToLower() == "DESKTOP-VIS2G9B".ToLower())return true;

        //    return false;

        //}
    }

    public void AttachCustomProcedure(DocumentCT cdoc, Customs_Procedure cp)
    {
        if (cp == null) throw new ApplicationException("Default Export Template not configured properly!");

        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = cp.Customs_ProcedureId;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = cp;
        cdoc.Document.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code =
            cp.Document_Type.Declaration_gen_procedure_code;
        cdoc.Document.xcuda_Identification.xcuda_Type.Type_of_declaration =
            cp.Document_Type.Type_of_declaration;
    }

    private static void PreventDeletingFromSystemDocSet(AsycudaDocumentSet docset)
    {
        var sysDocSet =
            new DocumentDSContext().SystemDocumentSets.FirstOrDefault(x => x.Id == docset.AsycudaDocumentSetId);
        if (sysDocSet != null)
        {
            throw new ApplicationException(
                "Trying to delete from System DocumentSet! General Policy this Cannot happen.");
        }
    }

    private void ParalellDeleteDocSetDocuments(AsycudaDocumentSet docset)
    {
        var doclst = docset.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.xcuda_ASYCUDA != null).ToList();
        var exceptions = new ConcurrentQueue<Exception>();
        Parallel.ForEach(doclst, new ParallelOptions { MaxDegreeOfParallelism = 1 }, //Environment.ProcessorCount * 2
            async item =>
            {
                StatusModel.StatusUpdate();
                try
                {
                    await DeleteAsycudaDocument(item.xcuda_ASYCUDA).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not import file - '{item.xcuda_ASYCUDA.CNumber + item.xcuda_ASYCUDA.RegistrationDate.ToShortDateString()}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
                }
            }
        );
        if (exceptions.Count > 0) throw new AggregateException(exceptions);
    }

    public int UpdateAsycudaDocumentSetLastNumber(int docSetId, int num)
    {
        using (var ctx = new DocumentDSContext())
        {
            var docSetRef = ctx.AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == docSetId)
                .Declarant_Reference_Number;
            while (ctx.xcuda_ASYCUDA.FirstOrDefault(x =>
                       x.xcuda_Declarant.Number.Contains(docSetRef) &&
                       x.xcuda_Declarant.Number.EndsWith((num + 1).ToString())) !=
                   null)
                num += 1;

            var sql = $@"UPDATE AsycudaDocumentSet
                                SET         LastFileNumber = {num + 1}
                                FROM    AsycudaDocumentSet 
                                where AsycudaDocumentSet.AsycudaDocumentSetId = {docSetId}";
            ctx.Database.ExecuteSqlCommand(sql);
            return num + 1;
        }
    }

    public async Task AddToEntry(IEnumerable<string> entryDatalst, AsycudaDocumentSet currentAsycudaDocumentSet,
        bool perInvoice, bool combineEntryDataInSameFile, bool groupItems, bool checkPackages, Serilog.ILogger log)
    {
        try
        {
            if (!IsValidDocument(currentAsycudaDocumentSet)) return;

            var slstSource = new List<EntryDataDetails>();
            foreach (var s in (await GetSelectedPODetails(entryDatalst.Distinct().ToList(),
                         currentAsycudaDocumentSet.AsycudaDocumentSetId).ConfigureAwait(false))) slstSource.Add(s);

            ;
            if (!IsValidEntryData(slstSource)) return;

            await CreateEntryItems(slstSource, currentAsycudaDocumentSet, perInvoice, true, false,
                    combineEntryDataInSameFile, groupItems, checkPackages, log)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<DocumentCT>> AddToEntry(IEnumerable<int> entryDatalst, int docSetId, bool perInvoice,
        bool combineEntryDataInSameFile, bool groupItems, Serilog.ILogger log)
    {
        try
        {
            var docSet = await Instance.GetAsycudaDocumentSet(docSetId)
                .ConfigureAwait(false);
            if (!IsValidDocument(docSet)) return new List<DocumentCT>();
            if (perInvoice && combineEntryDataInSameFile == false)
                using (var ctx = new CoreEntitiesContext())
                {
                    var ds = ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId);
                    if (ds.TotalInvoices.GetValueOrDefault() > ds.TotalPackages.GetValueOrDefault())
                        perInvoice = false;
                }

            var cp = GetCustomsProcedure("Duty Paid", "PO");
            docSet.Customs_Procedure = cp;
            docSet.Customs_ProcedureId = cp.Customs_ProcedureId;

            var slstSource =
                (from s in await GetSelectedPODetails(entryDatalst.Distinct().ToList())
                        .ConfigureAwait(false)
                    select s).ToList();

            if (!IsValidEntryData(slstSource)) return new List<DocumentCT>();

            return await CreateEntryItems(slstSource, docSet, perInvoice, true, false, combineEntryDataInSameFile,
                groupItems, true, log).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public  Task ValidateExistingTariffCodes(AsycudaDocumentSet currentAsycudaDocumentSet)
    {
        throw new NotImplementedException("ValidateExistingTariff");
    }

    public static string SetFilename(string droppedFilePath, string targetFileName, string nameExtension)
    {
        try
        {
            string filename;

            var file = new FileInfo(droppedFilePath);
            filename = $"{Path.Combine(file.DirectoryName)}\\{targetFileName}{nameExtension}";
            if (!File.Exists(filename)) File.Copy(droppedFilePath, filename);

            return filename;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    private bool IsValidEntryData(List<EntryDataDetails> slstSource)
    {
        if (!slstSource.Any()) throw new ApplicationException("Please Select Entry Data before proceeding");

        return true;
    }

    private bool IsValidDocument(AsycudaDocumentSet currentAsycudaDocumentSet)
    {
        if (currentAsycudaDocumentSet == null)
            throw new ApplicationException("Please Select a Asycuda Document Set before proceeding");

        if (currentAsycudaDocumentSet.Customs_Procedure == null)
            throw new ApplicationException(
                "Please Select Customs Procedure for selected Asycuda Document Set before proceeding");

        return true;
    }

    private void LinkPreviousDocuments(BaseDataModel.EntryLineData pod, DocumentCT cdoc)
    {
        if (pod.EntryData is PurchaseOrders p)
            if (p.PreviousCNumber != null)
            {
                var pitms = new CoreEntitiesContext()
                    .AsycudaItemBasicInfo.Where(x =>
                        x.CNumber == p.PreviousCNumber).OrderByDescending(x => x.ASYCUDA_Id).ToList();
                foreach (var itm in cdoc.DocumentItems)
                {
                    var pitm = pitms.FirstOrDefault(x => x.ItemNumber == itm.PreviousInvoiceItemNumber);
                    if (pitm != null)
                        itm.xcuda_Previous_doc.Previous_document_reference =
                            $@"C# {p.PreviousCNumber} - Line:{pitm.LineNumber}";
                    else
                        itm.xcuda_Previous_doc.Previous_document_reference = $@"C# {p.PreviousCNumber}";
                }
            }
    }

    private static void SetPackages(ref int remainingPackages, ref double possibleEntries,
        BaseDataModel.EntryLineData pod,
        DocumentCT cdoc)
    {
        if (remainingPackages > 0)
        {
            var itm = cdoc.DocumentItems.First();
            var pkg = cdoc.DocumentItems.First().xcuda_Packages.FirstOrDefault();
            if (pkg == null)
            {
                pkg = new xcuda_Packages(true)
                {
                    Item_Id = itm.Item_Id,
                    xcuda_Item = itm,
                    Kind_of_packages_code = "PK",
                    Marks1_of_packages = "Marks",
                    TrackingState = TrackingState.Added
                };
                if (pod.EntryData is PurchaseOrders p)
                    if (p.WarehouseInfo.Any())
                        pkg.Marks2_of_packages = p.WarehouseInfo.Select(z => z.WarehouseNo).DefaultIfEmpty("")
                            .Aggregate((o, n) => $"{o},{n}").Truncate(39);
                itm.xcuda_Packages.Add(pkg);
            }

            if (possibleEntries == 1)
            {
                pkg.Number_of_packages += remainingPackages;
                remainingPackages = 0;
            }
            else
            {
                pkg.Number_of_packages = 1;
                remainingPackages -= 1;
                possibleEntries -= 1;
            }
        }
    }

    public async static void LinkPDFs(List<string> cNumbers, string docCode = "NA")
    {
        try
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var res = new List<int>();
                foreach (var entryId in cNumbers)
                    res.Add(ctx.AsycudaDocuments.Where(x => x.CNumber == entryId).OrderByDescending(x => x.ASYCUDA_Id)
                        .Select(x => x.ASYCUDA_Id).FirstOrDefault());
                await LinkPDFs(res.Where(x => x != 0).ToList(), docCode).ConfigureAwait(false);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private Customs_Procedure AttachEntryDataDocumentType(DocumentCT cdoc, EDDocumentTypes documentType)
    {
        using (var ctx = new DocumentDSContext())
        {
            var cp = Instance.Customs_Procedures.FirstOrDefault(x =>
                x.Extended_customs_procedure + "-" + x.National_customs_procedure == documentType.DocumentType);
            if (cp == null)
                throw new ApplicationException(
                    $"Entry Data DocumentType not Found - '{documentType.DocumentType}'");
            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId !=
                cp.Customs_ProcedureId)
            {
                if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId != 0)
                {
                    var c = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure;
                    foreach (var item in cdoc.DocumentItems.Where(x =>
                                     x.xcuda_Tarification.Extended_customs_procedure == c.Extended_customs_procedure &&
                                     x.xcuda_Tarification.National_customs_procedure == c.National_customs_procedure)
                                 .ToList())
                    {
                        item.xcuda_Tarification.Extended_customs_procedure =
                            cp.Extended_customs_procedure;
                        item.xcuda_Tarification.National_customs_procedure =
                            cp.National_customs_procedure;
                    }
                }

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = cp.Customs_ProcedureId;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = cp;
                AttachCustomProcedure(cdoc, cp);
            }

            return cp;
        }
    }

    public static void ConfigureDocSet(AsycudaDocumentSet docSet, ExportTemplate exportTemplate)
    {
        docSet.Customs_Procedure = Instance.Customs_Procedures.First(x =>
            x.DisplayName == exportTemplate.Customs_Procedure);
        docSet.Customs_ProcedureId = docSet.Customs_Procedure.Customs_ProcedureId;
        docSet.BLNumber = exportTemplate.BL;
        docSet.Manifest_Number = exportTemplate.Manifest;
        docSet.Currency_Code = exportTemplate.Gs_Invoice_Currency_code;
        docSet.LocationOfGoods = exportTemplate.Location_of_goods;
    }

    public static void SetEffectiveAssessmentDate(DocumentCT cdoc)
    {
        var effectiveAssessmentDate = (DateTime)
            cdoc.EntryDataDetails.Select(x => x.EffectiveDate).Min();
        cdoc.Document.xcuda_General_information.Comments_free_text =
            $"EffectiveAssessmentDate:{effectiveAssessmentDate:MMM-dd-yyyy}";
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
            effectiveAssessmentDate;
    }

    private IEnumerable<BaseDataModel.EntryLineData> CreateSingleEntryLineData(
        IEnumerable<EntryDataDetails> slstSource)
    {
        var slst = slstSource
            .Select(g =>
                CreateEntryLineData(g));
        return slst;
    }

    public static BaseDataModel.EntryLineData CreateEntryLineData(EntryDataDetails g)
    {
        return new BaseDataModel.EntryLineData
        {
            ItemNumber = g.ItemNumber.Trim(),
            ItemDescription = g.ItemDescription.Trim(),
            TariffCode = g.TariffCode,
            Cost = g.Cost,
            Quantity = g.Quantity,
            EntryDataDetails = new List<EntryDataDetailSummary>
            {
                new EntryDataDetailSummary
                {
                    EntryDataDetailsId = g.EntryDataDetailsId,
                    EntryData_Id = g.EntryData_Id,
                    EntryDataId = g.EntryDataId,
                    EffectiveDate = g.EffectiveDate.GetValueOrDefault(),
                    EntryDataDate = g.EntryData.EntryDataDate,
                    QtyAllocated = g.QtyAllocated,
                    Currency = g.EntryData.Currency,
                    LineNumber = g.LineNumber,
                    Comment = g.Comment
                }
            },
            EntryData = g.EntryData,

            Freight = Convert.ToDouble(g.Freight),
            Weight = Convert.ToDouble(g.Weight),
            InternalFreight = Convert.ToDouble(g.InternalFreight),
            TariffSupUnitLkps = g.InventoryItemEx.SuppUnitCode2 != null
                ? new List<ITariffSupUnitLkp>
                {
                    new TariffSupUnitLkps
                    {
                        SuppUnitCode2 = g.InventoryItemEx.SuppUnitCode2,
                        SuppQty = g.InventoryItemEx.SuppQty.GetValueOrDefault()
                    }
                }
                : null,
            InventoryItemEx = g.InventoryItemEx
        };
    }

    public IEnumerable<BaseDataModel.EntryLineData> CreateGroupEntryLineData(
        IEnumerable<EntryDataDetails> slstSource)
    {
        return Instance.CurrentApplicationSettings.GroupIM4ByCategory == true
            ? CategoryGroupEntryLineData.Execute(slstSource)
            : SimpleGroupEntryLineData.Execute(slstSource);
    }

    private static void UpdateFreight(DocumentDSContext ctx, KeyValuePair<int, double> doc, double freightRate,
        string currency)
    {
        var val = ctx.xcuda_Valuation.Include(x => x.xcuda_Gs_external_freight).First(x => x.ASYCUDA_Id == doc.Key);
        if (val == null) return;
        var xcuda_Gs_external_freight = val.xcuda_Gs_external_freight;
        if (xcuda_Gs_external_freight == null)
        {
            xcuda_Gs_external_freight = new xcuda_Gs_external_freight(true)
            {
                Valuation_Id = doc.Key,
                xcuda_Valuation = val,
                TrackingState = TrackingState.Added
            };
            val.xcuda_Gs_external_freight = xcuda_Gs_external_freight;
        }

        xcuda_Gs_external_freight.Amount_foreign_currency = freightRate;
        xcuda_Gs_external_freight.Currency_code = currency;
    }

    public static void LimitFreeText(xcuda_Item itm)
    {
        if (itm.Free_text_1 != null && itm.Free_text_1.Length > 1)
            itm.Free_text_1 = itm.Free_text_1.Length < 36
                ? itm.Free_text_1.Substring(0)
                : itm.Free_text_1.Substring(0, 35);


        if (itm.Free_text_2 != null && itm.Free_text_2.Length > 1)
            itm.Free_text_2 = itm.Free_text_2.Length < 21
                ? itm.Free_text_2.Substring(0)
                : itm.Free_text_2.Substring(0, 20);
    }

    private static void SetMinWeight(BaseDataModel.IEntryLineData pod, xcuda_Item itm)
    {
        itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
        {
            TrackingState = TrackingState.Added,
            Gross_weight_itm = (decimal)(pod.Quantity * (double)_minimumPossibleAsycudaWeight),
            Net_weight_itm = (decimal)(pod.Quantity * (double)_minimumPossibleAsycudaWeight),
            xcuda_Valuation_item = itm.xcuda_Valuation_item
        };
    }

    public void ProcessItemTariff(BaseDataModel.IEntryLineData pod, xcuda_ASYCUDA cdoc, xcuda_Item itm)
    {
        if (pod.TariffCode != null)
        {
            if (pod.TariffSupUnitLkps == null || !pod.TariffSupUnitLkps.Any()) return;

            var tariffSupUnitLkps = pod.TariffSupUnitLkps.Where(x => x != null).DistinctBy(s => s.SuppUnitCode2)
                .ToList();
            if (tariffSupUnitLkps.Any())
                foreach (var item in tariffSupUnitLkps.ToList())
                    itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(
                        new xcuda_Supplementary_unit(true)
                        {
                            Suppplementary_unit_code = item.SuppUnitCode2,
                            Suppplementary_unit_quantity = pod.Quantity,
                            TrackingState = TrackingState.Added,
                            xcuda_Tarification = itm.xcuda_Tarification
                        });
        }
    }

    public async Task RemoveEntryData(string po)
    {
        using (var ctx = new EntryDataService())
        {
            if (po != null) await ctx.DeleteEntryData(po).ConfigureAwait(false);
        }
    }

    public static void SaveAttachments(AsycudaDocumentSet docSet, DocumentCT cdoc)
    {
        try
        {
            if (!cdoc.DocumentItems.Any()) return;
            var alst = docSet.AsycudaDocumentSet_Attachments
                .Where(x => x.DocumentSpecific == false
                            && cdoc.EmailIds.Contains(x.EmailId)
                            && cdoc.Document.AsycudaDocument_Attachments.All(z =>
                                z.AttachmentId != x.AttachmentId)).ToList();

            foreach (var att in alst)
            foreach (var itm in cdoc.DocumentItems)
            {
                if (itm.EmailId != att.EmailId) continue;
                //.Select(x => x.InvoiceNo).Distinct().Aggregate((old, current) => old + "," + current)
                cdoc.Document.AsycudaDocument_Attachments.Add(new AsycudaDocument_Attachments(true)
                {
                    AttachmentId = att.AttachmentId,
                    AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                    //Attachment = att,
                    //xcuda_ASYCUDA = cdoc.Document,
                    TrackingState = TrackingState.Added
                });

                var f = new FileInfo(att.Attachment.FilePath);
                itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                {
                    Attached_document_code = att.Attachment.DocumentCode,
                    Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                    Attached_document_reference =
                        f.Name.Replace(f.Extension, ""), //pod.EntryData.EntryDataId,
                    xcuda_Attachments = new List<xcuda_Attachments>
                    {
                        new xcuda_Attachments(true)
                        {
                            AttachmentId = att.AttachmentId,
                            TrackingState = TrackingState.Added
                        }
                    },
                    TrackingState = TrackingState.Added
                });
                break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}