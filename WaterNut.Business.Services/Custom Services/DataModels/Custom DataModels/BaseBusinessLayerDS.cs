using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Asycuda421;
using Core.Common.Business.Services;
using Core.Common.Data;
using Core.Common.UI;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using DocumentItemDS.Business.Entities;
using DocumentItemDS.Business.Services;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using EntryDataDS.Business.Services;
using InventoryDS.Business.Entities;
using InventoryDS.Business.Services;
using LicenseDS.Business.Entities;
using MoreLinq.Extensions;
using Omu.ValueInjecter;
using TrackableEntities;
using TrackableEntities.EF6;
using ValuationDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;
using WaterNut.Business.Services.Utils;
using WaterNut.DataLayer;
using WaterNut.DataSpace.Asycuda;
using WaterNut.Interfaces;
using Adjustments = EntryDataDS.Business.Entities.Adjustments;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using AsycudaDocument_Attachments = DocumentDS.Business.Entities.AsycudaDocument_Attachments;
using AsycudaDocumentEntryData = DocumentDS.Business.Entities.AsycudaDocumentEntryData;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSet_Attachments = DocumentDS.Business.Entities.AsycudaDocumentSet_Attachments;
using AsycudaDocumentSetService = DocumentDS.Business.Services.AsycudaDocumentSetService;
using Attachments = CoreEntities.Business.Entities.Attachments;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using Document_Type = DocumentDS.Business.Entities.Document_Type;
using Document_TypeService = DocumentDS.Business.Services.Document_TypeService;
using EntryData = EntryDataDS.Business.Entities.EntryData;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;
using EntryDataDetailsEx = EntryDataQS.Business.Entities.EntryDataDetailsEx;
using EntryPreviousItems = CoreEntities.Business.Entities.EntryPreviousItems;
using ExportTemplate = DocumentDS.Business.Entities.ExportTemplate;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using InventoryItem = InventoryDS.Business.Entities.InventoryItem;
using InventoryItemsEx = EntryDataDS.Business.Entities.InventoryItemsEx;
using Registered = LicenseDS.Business.Entities.Registered;
using TariffSupUnitLkps = AllocationDS.Business.Entities.TariffSupUnitLkps;
using xcuda_ASYCUDA = DocumentDS.Business.Entities.xcuda_ASYCUDA;
using xcuda_ASYCUDA_ExtendedProperties = DocumentDS.Business.Entities.xcuda_ASYCUDA_ExtendedProperties;
using xcuda_Attached_documents = DocumentItemDS.Business.Entities.xcuda_Attached_documents;
using xcuda_Attachments = DocumentItemDS.Business.Entities.xcuda_Attachments;
using xcuda_Container = DocumentDS.Business.Entities.xcuda_Container;
using xcuda_Delivery_terms = DocumentDS.Business.Entities.xcuda_Delivery_terms;
using xcuda_Gs_external_freight = DocumentDS.Business.Entities.xcuda_Gs_external_freight;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_Item_Invoice = DocumentItemDS.Business.Entities.xcuda_Item_Invoice;
using xcuda_Packages = DocumentItemDS.Business.Entities.xcuda_Packages;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;
using xcuda_Traders_Financial = DocumentDS.Business.Entities.xcuda_Traders_Financial;
using xcuda_Transport = DocumentDS.Business.Entities.xcuda_Transport;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;

namespace WaterNut.DataSpace
{
    using Serilog;

    public partial class BaseDataModel
    {
        //public static void AttachToDocument(List<string> attachments, xcuda_ASYCUDA doc, List<xcuda_Item> itms)
        //{

        //    var alst = new List<Attachment>();
        //    using(var ctx = new CoreEntitiesContext())
        //    foreach (var astr in attachments)
        //    {
        //        var att = ctx.Attachments.FirstOrDefault(x => x.FilePath)
        //    }
        //}


        public bool IsPerIM7( bool PerIM7, string prevIM7, string CNumber)
        {
            return (PerIM7 == true &&
                    (string.IsNullOrEmpty(prevIM7) ||
                     (!string.IsNullOrEmpty(prevIM7) && prevIM7 != CNumber)));
        }

        public bool InvoicePerEntry(bool perInvoice, string prevEntryId, string entryDataId)
        {
            return (//BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry == true &&
                perInvoice == true &&
                //prevEntryId != "" &&
                prevEntryId != entryDataId);
        }

        public bool MaxLineCount(int itmcount)
        {
            return (itmcount != 0 &&
                    itmcount %
                    BaseDataModel.Instance.CurrentApplicationSettings.MaxEntryLines ==
                    0);
        }

        public async Task RemoveItem(int id)
        {
            //if (CurrentAsycudaItemEntryId == null) return;
            xcuda_Item r;
            using (var ctx = new xcuda_ItemService())
            {
                r = await ctx.Getxcuda_ItemByKey(id.ToString()).ConfigureAwait(false);
                await ctx.Deletexcuda_Item(id.ToString()).ConfigureAwait(false);
            }

            await ReDoDocumentLineNumbers(r.ASYCUDA_Id).ConfigureAwait(false);
        }

        public async Task ReDoDocumentLineNumbers(int ASYCUDA_Id)
        {
            using (var ctx = new xcuda_ItemService())
            {
                var lst =
                    (await ctx.Getxcuda_ItemByASYCUDA_Id(ASYCUDA_Id.ToString()).ConfigureAwait(false)).OrderBy(x =>
                        x.LineNumber);

                for (var i = 0; i < lst.Count(); i++)
                {
                    var itm = lst.ElementAt(i);
                    itm.LineNumber = i + 1;
                    await ctx.Updatexcuda_Item(itm).ConfigureAwait(false);
                }
            }
        }


        internal async Task RemoveSelectedItems(List<xcuda_Item> lst)
        {
            StatusModel.StartStatusUpdate("Removing selected items", lst.Count());

            var docs = lst.Select(x => x.ASYCUDA_Id).ToList();

            foreach (var item in lst.ToList())
            {
                await DeleteItem(item.Item_Id).ConfigureAwait(false);
                StatusModel.StatusUpdate();
            }

            foreach (var docId in docs) await ReDoDocumentLineNumbers(docId).ConfigureAwait(false);

            StatusModel.StopStatusUpdate();
        }

        public async Task DeleteItem(int p)
        {
            using (var ctx = new xcuda_ItemService())
            {
                await ctx.Deletexcuda_Item(p.ToString()).ConfigureAwait(false);
            }
        }


        public async Task DeleteAsycudaDocument(int ASYCUDA_Id)
        {
            xcuda_ASYCUDA doc = null;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                doc = await ctx
                    .Getxcuda_ASYCUDAByKey(ASYCUDA_Id.ToString(),
                        new List<string> {"xcuda_ASYCUDA_ExtendedProperties"}).ConfigureAwait(false);
            }

            await DeleteAsycudaDocument(doc).ConfigureAwait(false);
        }

        public async Task Save_xcuda_PreviousItem(xcuda_PreviousItem pi)
        {
            if (pi == null) return;
            using (var ctx = new xcuda_PreviousItemService())
            {
                await ctx.Updatexcuda_PreviousItem(pi).ConfigureAwait(false);
            }
        }

        public async Task Save_xcuda_Item(xcuda_Item Originalitm)
        {
            if (Originalitm == null) return;
            using (var ctx = new xcuda_ItemService())
            {
                await ctx.Updatexcuda_Item(Originalitm).ConfigureAwait(false);
            }
        }

        public async Task SaveDocument(xcuda_ASYCUDA doc)
        {
            if (doc == null) return;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                await ctx.CleanAndUpdateXcuda_ASYCUDA(doc).ConfigureAwait(false);
            }
        }

        public async Task SaveInventoryItem(InventoryItem item)
        {
            if (item == null) return;
            using (var ctx = new InventoryItemService())
            {
                await ctx.UpdateInventoryItem(item).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsycudaDocument(xcuda_ASYCUDA doc)
        {
            if (doc == null) return;
            await DeleteDocumentSalesAllocations(doc).ConfigureAwait(false);
            await DeleteDocument(doc).ConfigureAwait(false);
        }

        private async Task DeleteDocumentSalesAllocations(xcuda_ASYCUDA doc)
        {
            if (doc.TrackingState == TrackingState.Added) return;
            using (var ctx = new WaterNutDBEntities())
            {
                await ctx.ExecuteStoreCommandAsync($@"DELETE FROM AsycudaSalesAllocations
                                                FROM    AsycudaSalesAllocations INNER JOIN
                                                xcuda_Item ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Item.Item_Id
                                                WHERE(xcuda_Item.ASYCUDA_Id = {doc.ASYCUDA_Id})")
                    .ConfigureAwait(false);
            }
        }

        private async Task DeleteDocument(xcuda_ASYCUDA doc)
        {
            var docid = doc.ASYCUDA_Id;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                await ctx.Deletexcuda_ASYCUDA(docid.ToString()).ConfigureAwait(false);
            }
        }
        
        internal void ExporttoXML(string f, xcuda_ASYCUDA currentDocument)
        {
            if (currentDocument != null)
            {
                var docSetPath = Instance._currentAsycudaDocumentSet == null
                    ? null
                    : Path.Combine(Instance.CurrentApplicationSettings.DataFolder,
                        Instance._currentAsycudaDocumentSet.Declarant_Reference_Number);
                DocToXML(docSetPath, currentDocument, new FileInfo(f));
            }
            else
            {
                throw new ApplicationException("Please Select Asycuda Document to Export");
            }
        }


        internal void DocToXML(string docSetPath, xcuda_ASYCUDA doc, FileInfo f)
        {
            File.AppendAllText(Path.Combine(f.DirectoryName, "Instructions.txt"), $"File\t{f.FullName}\r\n");
            var a = new ASYCUDA();
            a.LoadFromDataBase(docSetPath, doc.ASYCUDA_Id, a, f);
            a.SaveToFile(f.FullName);
            File.AppendAllText(Path.Combine(f.DirectoryName, "Instructions.txt"), $"File\t{f.FullName}\r\n");
        }


        public async Task ImportC71(int asycudaDocumentSetId, List<string> files, ILogger log)
        {
            var docSet = await Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).ConfigureAwait(false);
            var exceptions = new ConcurrentQueue<Exception>();
            foreach (var file in files)
                try
                {
                    if (Value_declaration_form.CanLoadFromFile(file))
                        LoadC71(docSet, file, ref exceptions, log);
                    else
                        throw new ApplicationException($"Can not Load file '{file}'");


                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }

            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public async Task ImportLicense(int asycudaDocumentSetId, List<string> files)
        {
            var docSet = await Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).ConfigureAwait(false);
            var exceptions = new ConcurrentQueue<Exception>();
            foreach (var file in files)
                try
                {
                    if (Licence.CanLoadFromFile(file))
                        await LoadLicence(docSet, file, exceptions).ConfigureAwait(false);
                    else
                        throw new ApplicationException($"Can not Load file '{file}'");
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
        }


        public Task ExportDocument(string filename, xcuda_ASYCUDA doc)
        {
            Instance.ExporttoXML(filename, doc);
            return Task.CompletedTask;
        }

        public async Task ExportDocSet(int AsycudaDocumentSetId, string directoryName, bool overWrite)
        {
            var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);
            ExportDocSet(docset, directoryName, overWrite);
        }

        public async Task ExportLastDocumentInDocSet(int AsycudaDocumentSetId, string directoryName, bool overWrite)
        {
            var docset = await GetDocSetWithEntryDataDocs(AsycudaDocumentSetId).ConfigureAwait(false);
            ExportLastDocumentInDocSet(docset, directoryName, overWrite);
        }


        public async Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem)
        {
            if (asycudaDocumentItem == null) return;
            //get the original item
            var i = await GetDocumentItem(asycudaDocumentItem.Item_Id, new List<string>
            {
                "xcuda_Tarification.xcuda_HScode",
                "xcuda_Valuation_item.xcuda_Weight_itm",
                "xcuda_PreviousItem"
            }).ConfigureAwait(false);
            if (i.xcuda_Goods_description.TrackingState == TrackingState.Added) i.xcuda_Goods_description = null;
            if (i.xcuda_Previous_doc.TrackingState == TrackingState.Added) i.xcuda_Previous_doc = null;
            i.StartTracking();

            asycudaDocumentItem.ModifiedProperties = null;
            // null for now cuz there are no navigation properties involved.

            i.InjectFrom(asycudaDocumentItem);


            if (i.xcuda_PreviousItem != null)
            {
                i.xcuda_PreviousItem.Net_weight = (decimal) asycudaDocumentItem.Net_weight_itm;
                i.Net_weight = asycudaDocumentItem.Net_weight_itm;
                i.Gross_weight = asycudaDocumentItem.Net_weight_itm;
            }

            await Save_xcuda_Item(i).ConfigureAwait(false);
        }

        private async Task Save_xcuda_ASYCUDA(xcuda_ASYCUDA i)
        {
            if (i == null) return;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                await ctx.CleanAndUpdateXcuda_ASYCUDA(i).ConfigureAwait(false);
            }
        }


        public async Task DeleteDocumentCt(DocumentCT da)
        {
            if (da.Document.TrackingState == TrackingState.Added) return;
            using (var ctx = new WaterNutDBEntities())
            {
                await ctx.ExecuteStoreCommandAsync(@"delete from xcuda_Item
                                               where ASYCUDA_Id = @ASYCUDA_Id

                                               delete from xcuda_ASYCUDA
                                               where ASYCUDA_Id = @ASYCUDA_Id",
                        new SqlParameter("@ASYCUDA_Id", SqlDbType.Int).Value = da.Document.ASYCUDA_Id)
                    .ConfigureAwait(false);
            }
        }


        public async Task DeleteAsycudaDocumentSet(int docSetId)
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                await ctx.DeleteAsycudaDocumentSet(docSetId.ToString()).ConfigureAwait(false);
            }
        }


        public async Task AttachToExistingDocuments(int asycudaDocumentSetId)
        {
            try
            {
                await AttachC71(asycudaDocumentSetId).ConfigureAwait(false);

                    await AttachPDF(asycudaDocumentSetId).ConfigureAwait(false);
                await AttachLicense(asycudaDocumentSetId).ConfigureAwait(false);
                AttachContainer(asycudaDocumentSetId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static async Task AttachBlankC71(List<DocumentCT> docList)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var c71 = ctx.Attachments.First(x => x.DocumentCode == "DC05" && x.Reference == "NA");
                foreach (var doc in docList)
                    await AttachToDocument(new List<int> {c71.Id},
                        doc.Document.ASYCUDA_Id, doc.DocumentItems.First().Item_Id).ConfigureAwait(false);
            }
        }


        public static async Task RenameDuplicateDocuments(int docKey)
        {
            try
            {
                var (docSet, lst) = await EntryDocSetUtils.GetDuplicateDocuments(docKey).ConfigureAwait(false);
                if (!lst.Any() || !docSet.Documents.Any()) return;

                await EntryDocSetUtils.RenameDuplicateDocuments(lst, docSet).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static async Task EmailExceptionHandlerAsync(Exception e, ILogger log, bool sendOnlyOnce = true)
        {
            var lastexception = false;
            var errorMessage = "Loading components";
            Exception exp = e;
            while (lastexception == false)
            {
                if (exp.InnerException == null)
                {
                    lastexception = true;


                    if (sendOnlyOnce == false || !EmailedMessagesList.Contains(exp.Message))
                    {
                        await EmailDownloader.EmailDownloader.SendEmailAsync(BaseDataModel.GetClient(), null, $"Bug Found",
                            EmailDownloader.EmailDownloader.GetContacts("Developer", log), $"{exp.Message}\r\n{exp.StackTrace}",
                            Array.Empty<string>(), log).ConfigureAwait(false);
                        EmailedMessagesList.Add(exp.Message);
                    }

                }

                errorMessage += $"An unhandled Exception occurred!: {exp.Message}"; //---- {1}
                exp = exp.InnerException;
            }
        }

        
    }
}