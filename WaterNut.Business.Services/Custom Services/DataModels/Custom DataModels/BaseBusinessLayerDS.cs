

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AllocationQS.Business.Entities;
using AllocationQS.Business.Services;
using Core.Common.Data;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using Omu.ValueInjecter;
using TrackableEntities;
using TrackableEntities.EF6;
using System.Data.Entity;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks.Schedulers;
using AllocationDS.Business.Entities;
using Asycuda421;
using Core.Common.Business.Services;
using Core.Common.Converters;
using DocumentItemDS.Business.Entities;
using Core.Common.UI;
using InventoryDS.Business.Entities;
using DocumentDS.Business.Services;

using InventoryDS.Business.Services;
using WaterNut.Business.Entities;
using WaterNut.DataSpace.Asycuda;
using WaterNut.Interfaces;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using AsycudaDocumentEntryData = DocumentDS.Business.Entities.AsycudaDocumentEntryData;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using Customs_ProcedureService = DocumentDS.Business.Services.Customs_ProcedureService;
using Document_Type = DocumentDS.Business.Entities.Document_Type;
using Document_TypeService = DocumentDS.Business.Services.Document_TypeService;
using EntryData = EntryDataDS.Business.Entities.EntryData;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;
using EntryDataDetailsEx = EntryDataQS.Business.Entities.EntryDataDetailsEx;
using EntryDataDetailsService = EntryDataDS.Business.Services.EntryDataDetailsService;
using EntryDataService = EntryDataDS.Business.Services.EntryDataService;
using EntryPreviousItems = DocumentItemDS.Business.Entities.EntryPreviousItems;
using InventoryItem = InventoryDS.Business.Entities.InventoryItem;
using WaterNutDBEntities = WaterNut.DataLayer.WaterNutDBEntities;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_ItemService = DocumentItemDS.Business.Services.xcuda_ItemService;
using xcuda_Item_Invoice = DocumentItemDS.Business.Entities.xcuda_Item_Invoice;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using xcuda_PreviousItemService = DocumentItemDS.Business.Services.xcuda_PreviousItemService;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;
using xcuda_Weight_itm = DocumentItemDS.Business.Entities.xcuda_Weight_itm;
using xcuda_Weight = DocumentDS.Business.Entities.xcuda_Weight;


namespace WaterNut.DataSpace
{
    public partial  class BaseDataModel
    {
        private static readonly BaseDataModel instance;

        static BaseDataModel()
        {


            instance = new BaseDataModel();
           
            instance.CurrentApplicationSettings = null;

            Initialization = InitializationAsync();
        }

        public static  BaseDataModel Instance
        {
            get { return BaseDataModel.instance; }
        }


        private static async Task InitializationAsync()
        {
            StatusModel.Timer("Loading Data");
            var tasks = new List<Task>();



           // _inventoryCache =
           //     new DataCache<InventoryItem>(
           //         await InventoryDS.DataModels.BaseDataModel.Instance.SearchInventoryItem(new List<string>() {"All"},
           //             new List<string>()
           //             {
           //                 "InventoryItemAlias",
           //                 "TariffCodes.TariffCategory.TariffSupUnitLkps"
           //             }).ConfigureAwait(false));

           //_tariffCodeCache =
           //     new DataCache<TariffCode>(
           //         await
           //             InventoryDS.DataModels.BaseDataModel.Instance.SearchTariffCode(new List<string>() {"All"})
           //                 .ConfigureAwait(false));

            _document_TypeCache =
                new DataCache<Document_Type>(
                    await
                        DocumentDS.DataModels.BaseDataModel.Instance.SearchDocument_Type(new List<string>() {"All"})
                            .ConfigureAwait(false));

            _customs_ProcedureCache =
                new DataCache<Customs_Procedure>(
                    await
                        DocumentDS.DataModels.BaseDataModel.Instance.SearchCustoms_Procedure(new List<string>() {"All"})
                            .ConfigureAwait(false));

           
            StatusModel.StopStatusUpdate();
        }

        //public static DataCache<InventoryItem> _inventoryCache;
        //public static DataCache<TariffCode> _tariffCodeCache;
        public static DataCache<Customs_Procedure> _customs_ProcedureCache;
        public static DataCache<Document_Type> _document_TypeCache;
     


        //public DataCache<InventoryItem> InventoryCache { get { return BaseDataModel._inventoryCache; } }
        //public DataCache<TariffCode> TariffCodeCache {  get { return BaseDataModel._tariffCodeCache; } }
        public DataCache<Customs_Procedure> Customs_ProcedureCache {  get { return BaseDataModel._customs_ProcedureCache; } }
        public DataCache<Document_Type> Document_TypeCache {  get { return BaseDataModel._document_TypeCache; } }
       

        public bool ValidateInstallation()
        {
            try
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
            catch (Exception)
            {

                throw;
            }
        }

        internal async Task Clear(int AsycudaDocumentSetId)
        {
            AsycudaDocumentSet docset = null;
            using (var ctx = new AsycudaDocumentSetService())
            {
                docset = await ctx.GetAsycudaDocumentSetByKey(AsycudaDocumentSetId.ToString(), 
                    new List<string>()
                    {
                        "xcuda_ASYCUDA_ExtendedProperties",
                        "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                        "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_PreviousItem"
                    }).ConfigureAwait(false);
            }
            Clear(docset);
        }
        internal async  void Clear(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
                await ClearAsycudaDocumentSet(currentAsycudaDocumentSet).ConfigureAwait(false);
        }

        public  async Task ClearAsycudaDocumentSet(int AsycudaDocumentSetId)
        {
            var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);
            await ClearAsycudaDocumentSet(docset).ConfigureAwait(false);
        }

        public void AttachCustomProcedure(DocumentCT cdoc, Customs_Procedure cp)
        {
            if (cp == null)
            {
                throw new ApplicationException("Default Export Template not configured properly!");
            }
            else
            {
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = cp.Customs_ProcedureId;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = cp;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = cp.Document_TypeId;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type = cp.Document_Type;
                cdoc.Document.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code = cp.Document_Type.Declaration_gen_procedure_code;
                cdoc.Document.xcuda_Identification.xcuda_Type.Type_of_declaration = cp.Document_Type.Type_of_declaration;
            }
        }

        public async Task ClearAsycudaDocumentSet(AsycudaDocumentSet docset)
        {
            
            StatusModel.StartStatusUpdate("Deleting Documents from '' Document Set", docset.xcuda_ASYCUDA_ExtendedProperties.Count());

            var doclst = docset.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.xcuda_ASYCUDA != null).ToList();
            //foreach (var item in doclst)
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(doclst, new ParallelOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, item =>
            {
                StatusModel.StatusUpdate();
                try
                {
                    DeleteAsycudaDocument(item.xcuda_ASYCUDA).Wait();
                }
                catch (Exception ex)
                {

                    exceptions.Enqueue(
                                new ApplicationException(
                                    $"Could not import file - '{item.xcuda_ASYCUDA.CNumber + item.xcuda_ASYCUDA.RegistrationDate.ToShortDateString()}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
                }
                 

            }
                );
          if(exceptions.Count > 0) throw new AggregateException(exceptions);
            await CalculateDocumentSetFreight(docset.AsycudaDocumentSetId).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();
            
        }

        public interface IEntryLineData
        {
            string ItemNumber { get; set; }
             string ItemDescription { get; set; }
             string TariffCode { get; set; }
             double Cost { get; set; }
             int PreviousDocumentItemId { get; set; }
             double Quantity { get; set; }
            
            List<EntryDataDetailSummary> EntryDataDetails { get; set; }
           //  IDocumentItem PreviousDocumentItem { get; set; }
            // IInventoryItem InventoryItem { get; set; }
         

            double Weight { get; set; }

             double InternalFreight { get; set; }

             double Freight { get; set; }
            List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }
        }

        public class EntryLineData : IEntryLineData
        {
            string _itemNumber;
            public string ItemNumber 
            {
                get 
                {
                    return _itemNumber;
                }
                set
                {
                    _itemNumber = value;

                    using (var ctx = new InventoryItemService())
                    {
                        if (_itemNumber != null)
                        {
                            InventoryItem = ctx.GetInventoryItemsByExpression(
                                $"ItemNumber == \"{_itemNumber}\" && ApplicationSettingsId == {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}"
                                , new List<string>()
                            {
                                "TariffCodes.TariffCategory.TariffCategoryCodeSuppUnits.TariffSupUnitLkp"
                            }, false ).Result.FirstOrDefault();
                        }
                        else
                        {
                            InventoryItem = null;
                        }
                    }
                }
            }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double Cost { get; set; }
            
            int _previousDocumentItemId;
            public int PreviousDocumentItemId
            {
                get
                {
                    return _previousDocumentItemId;
                }
                set
                {
                    _previousDocumentItemId = value;

                    using (var ctx = new xcuda_ItemService())
                    {
                        if (_previousDocumentItemId != 0)
                        {
                            PreviousDocumentItem = ctx.Getxcuda_ItemByKey(_previousDocumentItemId.ToString()).Result;
                        }
                        else
                        {
                            PreviousDocumentItem = null;
                        }
                    }
                }
            }
            public double Quantity { get; set; }
            public List<EntryDataDetailSummary> EntryDataDetails { get; set; }
            public IDocumentItem PreviousDocumentItem { get; set; }
            public IInventoryItem InventoryItem { get; set; }
            public EntryData EntryData { get; set; }

            public double Freight { get; set; }
            public List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }

            public double Weight { get; set; }

            public double InternalFreight { get; set; }
            public global::EntryDataDS.Business.Entities.InventoryItemsEx InventoryItemEx { get; set; }
        }


        public  void IntCdoc(xcuda_ASYCUDA doc, AsycudaDocumentSet ads)
        {
            var cdoc = new DocumentCT {Document = doc};
            IntCdoc(cdoc, ads);
        }

        public async Task<DocumentCT> CreateDocumentCt(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            try
            {
                var cdoc = new DocumentCT();
                //using (var ctx = new xcuda_ASYCUDAService())
                //{
                    cdoc.Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet);
                    //d.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = null;// this is to save with out 2 extended properties with new entityid = 0
                    //cdoc.Document = await ctx.Createxcuda_ASYCUDA(d).ConfigureAwait(false);
                //}
                cdoc.DocumentItems = new List<xcuda_Item>();
                return cdoc;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal IInventoryItem GetInventoryItem(Func<IInventoryItem, bool> p)
        {
            using (var ctx = new InventoryDSContext())
            {
                return ctx.InventoryItems.FirstOrDefault(p);
            }
        }

        public xcuda_ASYCUDA CreateNewAsycudaDocument(AsycudaDocumentSet CurrentAsycudaDocumentSet)
        {
            var ndoc = new xcuda_ASYCUDA(true) { TrackingState = TrackingState.Added };// 
            //ndoc.SetupProperties();

            if (CurrentAsycudaDocumentSet != null)
            {
                CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(ndoc.xcuda_ASYCUDA_ExtendedProperties);
                ndoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = CurrentAsycudaDocumentSet;
                ndoc.xcuda_ASYCUDA_ExtendedProperties.FileNumber = CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Count();
            }
            return ndoc;
        }

        public  void IntCdoc(DocumentCT cdoc, AsycudaDocumentSet ads)
        {

            

            cdoc.Document.xcuda_Declarant.Number = ads.Declarant_Reference_Number + "-F" + cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString();
            cdoc.Document.xcuda_Declarant.Declarant_code = CurrentApplicationSettings.DeclarantCode;
            cdoc.Document.xcuda_Identification.Manifest_reference_number = ads.Manifest_Number;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = ads.AsycudaDocumentSetId;

            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type = ads.Customs_Procedure.Document_Type;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = ads.Customs_Procedure.Document_Type.Document_TypeId;
            cdoc.Document.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code = ads.Customs_Procedure.Document_Type.Declaration_gen_procedure_code;
            cdoc.Document.xcuda_Identification.xcuda_Type.Type_of_declaration = ads.Customs_Procedure.Document_Type.Type_of_declaration;
            cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination = ads.Country_of_origin_code;
            cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = ads.Country_of_origin_code;
            cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code = ads.Country_of_origin_code;
           //cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Destination.Destination_country_code = "GD";
            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = Convert.ToSingle(ads.Exchange_Rate);
            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = ads.Currency_Code;

            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId != ads.Customs_Procedure.Customs_ProcedureId)
            {
                if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId  != 0)
                {
                    var c = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure;
                    foreach (var item in cdoc.DocumentItems.Where(x => x.xcuda_Tarification.Extended_customs_procedure == c.Extended_customs_procedure && x.xcuda_Tarification.National_customs_procedure == c.National_customs_procedure).ToList())
                    {
                        item.xcuda_Tarification.Extended_customs_procedure = ads.Customs_Procedure.Extended_customs_procedure;
                        item.xcuda_Tarification.National_customs_procedure = ads.Customs_Procedure.National_customs_procedure;
                    }
                }
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = ads.Customs_ProcedureId;
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = ads.Customs_Procedure;
            }


            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != ads.BLNumber)
            {
                if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != null)
                {
                    var b = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber;
                    foreach (var item in cdoc.DocumentItems.Where(x => x.xcuda_Previous_doc.Summary_declaration == b).ToList())
                    {
                        item.xcuda_Previous_doc.Summary_declaration = ads.BLNumber;
                    }
                }
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber = ads.BLNumber;
            }



            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = ads.Description;

            //    cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = true;


        }

        
        public async Task AddToEntry(IEnumerable<string> entryDatalst, AsycudaDocumentSet currentAsycudaDocumentSet, bool perInvoice)
        {
            try
            {
                if (!IsValidDocument(currentAsycudaDocumentSet)) return;

                var slstSource =
                    (from s in await GetSelectedPODetails(entryDatalst).ConfigureAwait(false)
                        //.Where(p => p.Downloaded == false)
                        select s).ToList();
                ;
                if (!IsValidEntryData(slstSource)) return;

                await CreateEntryItems(slstSource, currentAsycudaDocumentSet, perInvoice, true, false).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddToEntry(IEnumerable<int> entryDatalst, int docSetId, bool perInvoice)
        {
            try
            {

                var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId).ConfigureAwait(false);
                if (!IsValidDocument(docSet)) return;

                var slstSource =
                    (from s in await GetSelectedPODetails(entryDatalst).ConfigureAwait(false)
                        //.Where(p => p.Downloaded == false)
                        select s).ToList();
                ;
                if (!IsValidEntryData(slstSource)) return;

                await CreateEntryItems(slstSource, docSet, perInvoice, true, false).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ValidateExistingTariffCodes(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
           
        }



        //public async Task AddToEntry(IEnumerable<EntryDataDetailsEx> entryDataDetailslst,
        //    AsycudaDocumentSet currentAsycudaDocumentSet)
        //{
        //    try
        //    {
        //        if (!IsValidDocument(currentAsycudaDocumentSet)) return;

        //        var slstSource =
        //            (from s in await GetSelectedPODetails(entryDataDetailslst).ConfigureAwait(false)
        //                //.Where(p => p.Downloaded == false)
        //                select s).ToList();
                
        //        if (!IsValidEntryData(slstSource)) return;

        //        await CreateEntryItems(slstSource, currentAsycudaDocumentSet).ConfigureAwait(false);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private bool IsValidEntryData(List<EntryDataDetails> slstSource)
        {
            
            if (!slstSource.Any())
            {
               throw new ApplicationException("Please Select Entry Data before proceeding");
                
            }
            return true;
        }

        private bool IsValidDocument(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            if (currentAsycudaDocumentSet == null)
            {
                throw new ApplicationException("Please Select a Asycuda Document Set before proceeding");
            }

            if (currentAsycudaDocumentSet.Document_Type == null ||
                currentAsycudaDocumentSet.Customs_Procedure == null)
            {
                throw new ApplicationException(
                    "Please Select Document Type & Customs Procedure for selected Asycuda Document Set before proceeding");
                
            }
            return true;
        }

        public async Task CreateEntryItems(List<EntryDataDetails> slstSource,
            AsycudaDocumentSet currentAsycudaDocumentSet, bool perInvoice, bool autoUpdate, bool autoAssess)
        {
            var itmcount = 0;
            var slst = CreateEntryLineData(slstSource);

            var cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};

            //BaseDataModel.Instance.CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(cdoc.xcuda_ASYCUDA_ExtendedProperties);
            IntCdoc(cdoc, currentAsycudaDocumentSet);
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
            if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
            AttachCustomProcedure(cdoc, currentAsycudaDocumentSet.Customs_Procedure);
            var entryLineDatas = slst as IList<BaseDataModel.EntryLineData> ?? slst.ToList();
            StatusModel.StartStatusUpdate("Adding Entries to New Asycuda Document", entryLineDatas.Count());
            EntryData entryData = null;


            var curLst = slstSource.Select(x => x.EntryData.Currency).Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
            if (curLst.Any() && perInvoice == false)
            {
                if (curLst.Count() > 1)
                    throw new ApplicationException("EntryDataDetails Contains More than 1 Currencies");
            }


            //////////////////////////////////////////////////////
            /// 
            /// If per invoice is choosen and sorted by tariff code it will alternate between invoice no and create too much entries
            /// left as is because this is not a problem
            /// 
            switch (CurrentApplicationSettings.OrderEntriesBy)
                {
                    case "TariffCode":
                        entryLineDatas = entryLineDatas.OrderBy(p => p.InventoryItem.TariffCode).ToList();
                        break;
                    case "Invoice":
                        entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.EntryDataId).ToList();
                        break;
                    default:
                        break;
                }
            


            var oldentryData = "";
            foreach (var pod in entryLineDatas)//
            {

                if (oldentryData != pod.EntryData.EntryDataId)
                {
                    if (perInvoice)
                    {
                        if (cdoc.DocumentItems.Any() && oldentryData != pod.EntryData.EntryDataId)
                        {
                            SetEffectiveAssessmentDate(cdoc);

                            await SaveDocumentCT(cdoc).ConfigureAwait(false);
                            cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};

                            //BaseDataModel.Instance.CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(cdoc.xcuda_ASYCUDA_ExtendedProperties);
                            IntCdoc(cdoc, currentAsycudaDocumentSet);
                            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
                            if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                            AttachCustomProcedure(cdoc, currentAsycudaDocumentSet.Customs_Procedure);
                            
                            itmcount = 0;
                        }
                    }
                    
                }

                //invoice dictates currency to prevent currencies mismatch
                 if (curLst.Any() && cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code != curLst.First())
                        {
                            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = curLst.First();
                        }

                if (entryData == null || entryData.EntryDataId != pod.EntryData.EntryDataId)
                {
                    // do container
                    if (entryData == null || !pod.EntryData.ContainerEntryData.OrderBy(x => x.Container_Id).SequenceEqual(entryData.ContainerEntryData.OrderBy(z => z.Container_Id)))
                    {
                        foreach (var c in pod.EntryData.ContainerEntryData)
                        {
                            var cnt =(await EntryDataDS.DataModels.BaseDataModel.Instance.SearchContainer(new List<string>()
                            {
                                $"Container_Id == {c.Container_Id}"
                            }).ConfigureAwait(false)).FirstOrDefault();
                            //c.Container = cnt;
                                var xcnt =
                                    cdoc.Document.xcuda_Container.FirstOrDefault(
                                        x => x.Container_identity == cnt.Container_identity);
                            if (xcnt == null)
                            {
                                xcnt = new xcuda_Container(true)
                                {
                                    ASYCUDA_Id = cdoc.Document.ASYCUDA_Id,
                                    Container_identity =  cnt.Container_identity,
                                    Container_type = cnt.Container_type,
                                    Packages_number = cnt.Packages_number,
                                    Packages_type = cnt.Packages_type,
                                    Packages_weight = cnt.Packages_weight,
                                    Item_Number = (cdoc.DocumentItems.Count + 1).ToString(),
                                    TrackingState = TrackingState.Added
                                };
                                cdoc.Document.xcuda_Container.Add(xcnt);
                            }
                            
                            
                        }
                    }
                    entryData = pod.EntryData;
                }

                

                var itm = CreateItemFromEntryDataDetail(pod, cdoc);

             
                if (itm == null) continue;
                itmcount += 1;

                if (oldentryData != pod.EntryData.EntryDataId)
                {
                    if(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Document_Type.DisplayName != "IM7")
                    itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                    {
                        Attached_document_code = BaseDataModel.Instance.ExportTemplates.FirstOrDefault(x => x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.DisplayName)?.AttachedDocumentCode ?? "IV05",
                        Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                        Attached_document_reference = pod.EntryData.EntryDataId,
                        TrackingState = TrackingState.Added
                    });

                    var alst = currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments
                        .Where(x => x.Attachment.FilePath.Contains(pod.EntryData.EntryDataId) || (x.DocumentSpecific == false && !cdoc.Document.AsycudaDocument_Attachments.Any(z => z.AttachmentId == x.AttachmentId) ))
                        .Select(x => x.Attachment);
                    foreach (var att in alst)
                    {
                        cdoc.Document.AsycudaDocument_Attachments.Add(new AsycudaDocument_Attachments(true)
                        {
                            AttachmentId = att.Id, AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                            //Attachment = att,
                            //xcuda_ASYCUDA = cdoc.Document,
                            TrackingState = TrackingState.Added
                        });

                        var f = new FileInfo(att.FilePath);
                        itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                        {
                            Attached_document_code = att.DocumentCode,
                            Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                            Attached_document_reference = f.Name.Replace(f.Extension, ""),//pod.EntryData.EntryDataId,
                            xcuda_Attachments = new List<xcuda_Attachments>()
                            {
                                new xcuda_Attachments(true)
                                {
                                    AttachmentId = att.Id,
                                    TrackingState = TrackingState.Added
                                }
                            },
                            TrackingState = TrackingState.Added
                        });
                    }
                    

                    oldentryData = pod.EntryData.EntryDataId;
                }



                if (itmcount%CurrentApplicationSettings.MaxEntryLines == 0)
                {
                    if (cdoc.DocumentItems.Any())
                    {
                        SetEffectiveAssessmentDate(cdoc);
                        await SaveDocumentCT(cdoc).ConfigureAwait(false);
                        cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};

                        //BaseDataModel.Instance.CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(cdoc.xcuda_ASYCUDA_ExtendedProperties);
                        IntCdoc(cdoc, currentAsycudaDocumentSet);
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = true;
                        if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                        AttachCustomProcedure(cdoc, currentAsycudaDocumentSet.Customs_Procedure);
                        itmcount = 0;
                    }
                }


                StatusModel.StatusUpdate();
                //System.Windows.Forms.MessageBox.
            }
            StatusModel.Timer("Saving To Database");
            if (cdoc.DocumentItems.Any())
            {
                SetEffectiveAssessmentDate(cdoc);
                await SaveDocumentCT(cdoc).ConfigureAwait(false);
            }

            await CalculateDocumentSetFreight(currentAsycudaDocumentSet.AsycudaDocumentSetId).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();


          
        }

        private static void SetEffectiveAssessmentDate(DocumentCT cdoc)
        {
            var effectiveAssessmentDate = (DateTime)
                cdoc.EntryDataDetails.Select(x => x.EffectiveDate).Min();
            cdoc.Document.xcuda_General_information.Comments_free_text =
                $"EffectiveAssessmentDate:{effectiveAssessmentDate:MMM-dd-yyyy}";
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
                effectiveAssessmentDate;
        }


        private  IEnumerable<BaseDataModel.EntryLineData> CreateEntryLineData(IEnumerable<EntryDataDetails> slstSource)
        {
            var slst = from s in slstSource.AsEnumerable()//.Where(p => p.Downloaded == false)
                group s by new {s.ItemNumber, s.ItemDescription, s.TariffCode, s.Cost, s.EntryData, s.InventoryItems}
                into g
                select new BaseDataModel.EntryLineData
                {
                    ItemNumber = g.Key.ItemNumber.Trim(),
                    ItemDescription = g.Key.ItemDescription.Trim(),
                    TariffCode = g.Key.TariffCode,
                    Cost = g.Key.Cost,
                    Quantity = g.Sum(x => x.Quantity),
                    EntryDataDetails = g.Select(x => new EntryDataDetailSummary()
                    {
                        EntryDataDetailsId = x.EntryDataDetailsId,
                        EntryDataId = x.EntryDataId,
                        EffectiveDate = x.EffectiveDate.GetValueOrDefault(),
                        EntryDataDate = x.EntryData.EntryDataDate,
                        QtyAllocated = x.QtyAllocated,
                        Currency = x.EntryData.Currency,
                        LineNumber = x.LineNumber,
                    }).ToList(),
                    EntryData = g.Key.EntryData,
                    Freight = Convert.ToDouble(g.Sum(x => x.Freight)),
                    Weight = Convert.ToDouble(g.Sum(x => x.Weight)),
                    InternalFreight = Convert.ToDouble(g.Sum(x => x.InternalFreight)),
                    TariffSupUnitLkps = g.Key.InventoryItems.SuppUnitCode2 != null ? new List<ITariffSupUnitLkp>() { new TariffSupUnitLkps() { SuppUnitCode2 = g.Key.InventoryItems.SuppUnitCode2, SuppQty = g.Key.InventoryItems.SuppQty.GetValueOrDefault()} }: null, 
                    InventoryItemEx = g.Key.InventoryItems,
                    
                };
            return slst;
        }

        public async Task<xcuda_ASYCUDA> GetDocument(int ASYCUDA_Id, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_ASYCUDAService(){StartTracking = true})
            {
                return await ctx.Getxcuda_ASYCUDAByKey(ASYCUDA_Id.ToString(), includeLst).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<AsycudaDocumentItem>> GetAllDocumentItems(List<string> includeLst = null)
        {
            using (var ctx = new AsycudaDocumentItemService())
            {
                return await ctx.GetAsycudaDocumentItems(includeLst).ConfigureAwait(false);
            }
        }

        public  async Task<AsycudaDocumentItem> Getxcuda_Item(int p)
        {
            using (var ctx = new AsycudaDocumentItemService())
            {
                return await ctx.GetAsycudaDocumentItemByKey(p.ToString()).ConfigureAwait(false);
            }
        }

        public async Task SaveDocumentCT(DocumentCT cdoc)
        {
            try
            {
                
                if (cdoc == null) return;
                //var docset = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet;
                 using (var ctx = new xcuda_ASYCUDAService())
                {
                    cdoc.Document = await ctx.CleanAndUpdateXcuda_ASYCUDA(cdoc.Document).ConfigureAwait(false);
                }

                //cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet =
                //    await GetAsycudaDocumentSet(docset.AsycudaDocumentSetId, new List<string>()
                //    {
                //        "xcuda_ASYCUDA_ExtendedProperties"
                //    }).ConfigureAwait(false);
               
               
               // ------------------- took it out cuz i want to import generated documents
               // if (cdoc.Document.ASYCUDA_Id == 0) return;
               
                    // prepare items for parrallel import
                    foreach (var item in cdoc.DocumentItems)
                    {
                        item.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                        item.LineNumber = cdoc.DocumentItems.IndexOf(item) + 1;
                        if (item.xcuda_PreviousItem != null)
                        {
                            item.xcuda_PreviousItem.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                            item.xcuda_PreviousItem.Current_item_number = item.LineNumber.ToString();
                            
                        }
                    }

                    //Parallel.ForEach(cdoc.DocumentItems, new ParallelOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, item =>
                    //{
                    
                        using (var ctx = new xcuda_ItemService())
                        {
                            foreach (var t in cdoc.DocumentItems)
                            {
                                var exceptions = new ConcurrentQueue<Exception>();
                                //cdoc.DocumentItems.AsParallel(new ParallelLinqOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }).ForAll((t) =>
                                //{
                                try
                                {
                                    if (t.ChangeTracker != null)
                                        ctx.Updatexcuda_Item(t).Wait(); //.ChangeTracker.GetChanges().FirstOrDefault()
                                }
                                catch (Exception ex)
                                {

                                    exceptions.Enqueue(ex);
                                }

                                //});
                                if (exceptions.Count > 0) throw new AggregateException(exceptions);
                                //    await ctx.Updatexcuda_Item(cdoc.DocumentItems).ConfigureAwait(false);
                            }
                        }
                //});
              //  await CalculateDocumentSetFreight(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId.GetValueOrDefault()).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task CalculateDocumentSetFreight(int asycudaDocumentSetId)
        {
            try
            {

                string currency = "";
                double totalfob = 0;
                double totalFreight = 0;
                double totalWeight = 0;
                List<int> doclst = null;
                Dictionary<int, double> docValues = new Dictionary<int, double>();
                AsycudaDocumentSet asycudaDocumentSet;
                using (var ctx = new DocumentDSContext())
                {
                    
                    asycudaDocumentSet = ctx.AsycudaDocumentSets.FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
                    if (asycudaDocumentSet != null)
                    {
                        if (asycudaDocumentSet.TotalFreight != null) totalFreight =asycudaDocumentSet.TotalFreight.Value;
                        if (asycudaDocumentSet.TotalWeight != null) totalWeight = asycudaDocumentSet.TotalWeight.Value;

                        currency = asycudaDocumentSet.Currency_Code;
                    }
                    
                    doclst =
                        ctx.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.AsycudaDocumentSetId == asycudaDocumentSetId)
                            .Where(x => (x.IsManuallyAssessed != true && x.ImportComplete == false)) // prevent recalculating weights of assessed entries
                            .Select(x => x.ASYCUDA_Id)
                            .ToList();
                    if (!doclst.Any()) return;
                }
                using (var ctx = new CoreEntitiesContext())
                {
                    
                    foreach (var doc in doclst)
                    {
                        var t = ctx.AsycudaDocuments.Where(x => x.ASYCUDA_Id == doc)
                            .Select(y => y.TotalCIF).DefaultIfEmpty(0).Sum();
                       
                        docValues.Add(doc, t.GetValueOrDefault());
                        totalfob += t.GetValueOrDefault();
                    }
                    
                }
                double freightRate = totalFreight != 0? totalFreight /totalfob: 0;
                double weightRate = totalWeight != 0 ?  totalWeight / totalfob: 0;
                using (var ctx = new DocumentDSContext() {StartTracking = true})
                {
                    foreach (var doc in docValues.Where(x => x.Value != 0))
                    {
                        if (asycudaDocumentSet.ApportionMethod == "Equal")
                        {
                            UpdateFreight(ctx, doc, totalFreight / doclst.Count(), currency);
                            UpdateWeight(ctx, doc, totalWeight/doclst.Count(), currency);
                        }
                        else
                        {
                            UpdateFreight(ctx, doc, doc.Value * freightRate, currency);
                            UpdateWeight(ctx, doc, doc.Value * weightRate, currency);
                        }
                        

                    }
                    ctx.SaveChanges();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private static void UpdateFreight(DocumentDSContext ctx, KeyValuePair<int, double> doc, double freightRate, string currency)
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
            if (xcuda_Gs_external_freight.TrackingState != TrackingState.Added)
                xcuda_Gs_external_freight.TrackingState = TrackingState.Modified;
            ctx.ApplyChanges(xcuda_Gs_external_freight);
        }

        private static void UpdateWeight(DocumentDSContext ctx, KeyValuePair<int, double> doc, double weightRate, string currency)
        {
            try
            {

            
            if (instance.CurrentApplicationSettings.WeightCalculationMethod.ToLower() != "Value".ToLower()) return;
            var val = ctx.xcuda_Valuation.Include(x => x.xcuda_Weight).FirstOrDefault(x => x.ASYCUDA_Id == doc.Key);
            if (val == null) return;
            var xcuda_Weight = val.xcuda_Weight;
            if (xcuda_Weight == null)
            {
                xcuda_Weight = new xcuda_Weight(true)
                {
                    Valuation_Id = doc.Key,
                    xcuda_Valuation = val,
                    TrackingState = TrackingState.Added
                };
                val.xcuda_Weight = xcuda_Weight;
            }

            xcuda_Weight.Gross_weight =  weightRate < 0.01 ?0.01 : weightRate;
            
            if (xcuda_Weight.TrackingState != TrackingState.Added)
                xcuda_Weight.TrackingState = TrackingState.Modified;
            ctx.ApplyChanges(xcuda_Weight);

            using (var ictx = new DocumentItemDSContext(){StartTracking = true})
            {
                var lst = ictx.xcuda_Weight_itm
                    .Include(x => x.xcuda_Valuation_item)
                   .Where(x => x.xcuda_Valuation_item.xcuda_Item.ASYCUDA_Id == doc.Key).ToList();
                foreach (var itm in lst)
                {


                    var calWgt = weightRate * (itm.xcuda_Valuation_item.Total_CIF_itm / doc.Value);
                    var minWgt = ictx.xcuda_Tarification.Include(x => x.Unordered_xcuda_Supplementary_unit)
                                     .First(z => z.Item_Id == itm.Valuation_item_Id).Unordered_xcuda_Supplementary_unit.First(x => x.IsFirstRow == true).Suppplementary_unit_quantity.GetValueOrDefault() * .01;
                    
                    itm.Gross_weight_itm = calWgt < minWgt? minWgt:calWgt;
                    itm.Net_weight_itm =
                        itm.Gross_weight_itm;
                }

                ictx.SaveChanges();
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal TariffCode GetTariffCode(Func<TariffCode, bool> p)
        {
            using (var ctx = new InventoryDSContext())
            {
                return ctx.TariffCodes.FirstOrDefault(p);
            }
        }

        internal  xcuda_Item CreateItemFromEntryDataDetail(BaseDataModel.IEntryLineData pod, DocumentCT cdoc)
        {
           // if (pod.TariffCode != null)
           // {
            try
            {
               

               var itm = CreateNewDocumentItem();
                cdoc.DocumentItems.Add(itm);
                //itm.SetupProperties();
                

                itm.xcuda_Goods_description.Commercial_Description = CleanText(pod.ItemDescription);
                if (cdoc.Document.xcuda_General_information != null)
                    itm.xcuda_Goods_description.Country_of_origin_code = cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination;
                itm.xcuda_Tarification.Item_price = Convert.ToSingle(pod.Cost * pod.Quantity);
                itm.xcuda_Tarification.National_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.National_customs_procedure; //cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Customs_Procedure.National_customs_procedure;
                itm.xcuda_Tarification.Extended_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure;//cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Customs_Procedure.Extended_customs_procedure;

                itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pod.TariffCode ?? "NULL";
                itm.xcuda_Tarification.xcuda_HScode.Precision_4 = pod.ItemNumber; //pod.PreviousDocumentItem == null ? pod.ItemNumber : pod.PreviousDocumentItem.ItemNumber;
                //itm.xcuda_Tarification.xcuda_HScode.InventoryItems = pod.InventoryItem ;

                if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != null)
                    itm.xcuda_Previous_doc.Summary_declaration = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber;

                itm.xcuda_Valuation_item.Total_CIF_itm = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));
                itm.xcuda_Valuation_item.Statistical_value = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));



                var ivc = new xcuda_Item_Invoice(true) { TrackingState = TrackingState.Added };

                ivc.Amount_national_currency = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost) * Convert.ToDecimal(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Exchange_Rate), 4));
                ivc.Amount_foreign_currency = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));


                if (cdoc.Document.xcuda_Valuation != null && cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice != null)
                {
                    //;
                    ivc.Currency_code = cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;//xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Currency_Code;
                    ivc.Currency_rate = cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate;//Convert.ToSingle(cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Exchange_Rate);
                }

                itm.xcuda_Valuation_item.xcuda_Item_Invoice = ivc;

                switch (CurrentApplicationSettings.WeightCalculationMethod)
                {
                    case "WeightEqualQuantity":
                        if (itm.xcuda_Valuation_item.xcuda_Weight_itm != null)
                        {
                            itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm =
                                Convert.ToSingle(
                                    Math.Round(pod.Quantity, 4));

                            itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm =
                                Convert.ToSingle(
                                    Math.Round(pod.Quantity, 4));
                        }

                        break;
                    case "OneOrMore":
                        if ((Single)pod.Quantity > 99)
                        {
                            itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
                            {
                                TrackingState = TrackingState.Added
                            };
                            itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = (Single)pod.Quantity *
                                                                                         Convert.ToSingle(.1);
                            //(Decimal)ops.Quantity;
                            itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = (Single)pod.Quantity *
                                                                                       Convert.ToSingle(.1);
                            //(Decimal)ops.Quantity;
                        }

                        if (pod.Weight != 0)
                            if (itm.xcuda_Valuation_item.xcuda_Weight_itm != null)
                                itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm =
                                    Convert.ToSingle(
                                        Math.Round(pod.Weight, 4));
                        if (pod.Weight != 0)
                            if (itm.xcuda_Valuation_item.xcuda_Weight_itm != null)
                                itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm =
                                    Convert.ToSingle(
                                        Math.Round(pod.Weight, 4));
                        break;
                    case "MinimumWeight" :
                        SetMinWeight(pod, itm);
                        break;
                    case "Value":
                        //Still set minium weight in event no weight is set via docset
                        SetMinWeight(pod, itm);
                        break;
                    default:
                        throw new ApplicationException("Please Configure WeightCalculationMethod");
                        break;


                }

                

                if (pod.InternalFreight != 0) itm.xcuda_Valuation_item.xcuda_item_internal_freight.Amount_foreign_currency =
                    Convert.ToSingle(pod.InternalFreight);
               

                if (pod.Freight != 0) itm.xcuda_Valuation_item.xcuda_item_external_freight.Amount_foreign_currency =
                    Convert.ToSingle(pod.Freight);
                // set on each line in event of grouped invoices per IM7 etc.
                if (pod.EntryDataDetails.Count() == 1)//if (cdoc.DocumentItems.Count() == 1)
                {
                    var fr = pod.EntryDataDetails.FirstOrDefault();
                    if (fr != null)
                    {
                        itm.Free_text_1 = $"{fr.EntryDataId}|{fr.LineNumber}";
                        itm.Free_text_2 = pod.ItemNumber;
                    }

                }
                foreach (var ed in pod.EntryDataDetails)
                {
                    if (itm.EntryDataDetails.All(x => x.EntryDataDetailsId != ed.EntryDataDetailsId))
                        itm.EntryDataDetails.Add(new xcuda_ItemEntryDataDetails(true) { Item_Id = itm.Item_Id, EntryDataDetailsId = ed.EntryDataDetailsId, TrackingState = TrackingState.Added });

                    if (cdoc.Document.AsycudaDocumentEntryDatas.All(x => x.EntryDataId != ed.EntryDataId))
                    {
                        cdoc.Document.AsycudaDocumentEntryDatas.Add(new AsycudaDocumentEntryData(true)
                        {
                            AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                            EntryDataId = ed.EntryDataId,
                            TrackingState = TrackingState.Added
                        });
                    }

                    cdoc.EntryDataDetails.Add(new EntryDataDetails(){EntryDataDetailsId = ed.EntryDataDetailsId, EntryDataId = ed.EntryDataId, EffectiveDate = ed.EffectiveDate == DateTime.MinValue ? ed.EntryDataDate: ed.EffectiveDate});

                }


                itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit(true) { Tarification_Id = itm.xcuda_Tarification.Item_Id, Suppplementary_unit_code = "NMB", Suppplementary_unit_quantity = pod.Quantity, IsFirstRow = true, TrackingState = TrackingState.Added });

                ProcessItemTariff(pod, cdoc.Document, itm);

                return itm;
            }
            catch (Exception)
            {

                throw;
            }
                // }
           // return null;
        }

        private static void SetMinWeight(IEntryLineData pod, xcuda_Item itm)
        {
            itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
            {
                TrackingState = TrackingState.Added
            };
            itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = (Single) pod.Quantity *
                                                                         Convert.ToSingle(.01);
            //(Decimal)ops.Quantity;
            itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = (Single) pod.Quantity *
                                                                       Convert.ToSingle(.01);
        }

        public string CleanText(string p)
        {
            return p?.Replace(",", "");
        }

        private  xcuda_Item CreateNewDocumentItem()
        {
            return new xcuda_Item(true) { TrackingState = TrackingState.Added };//
        }

        private  void ProcessItemTariff(BaseDataModel.IEntryLineData pod, xcuda_ASYCUDA cdoc, xcuda_Item itm)
        {

            if (pod.TariffCode != null)
            {

               
                    var tariffSupUnitLkps = pod.TariffSupUnitLkps;
                    if (tariffSupUnitLkps != null)
                        foreach (var item in tariffSupUnitLkps.ToList())
                        {
                            itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit(true) { Suppplementary_unit_code = item.SuppUnitCode2, Suppplementary_unit_quantity = pod.Quantity /* * item.SuppQty*/, TrackingState = TrackingState.Added });
                        }
               
                    

                //if (pod.InventoryItem.TariffCodes.TariffCategory != null && pod.InventoryItem.TariffCodes.TariffCategory.LicenseRequired == true)
                //{
                //    Licences lic = cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Licences.Where(l => l.TariffCateoryCode == pod.InventoryItem.TariffCodes.TariffCategoryCode).FirstOrDefault();
                //    if (lic != null)
                //    {
                        
                //        itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents() { Attached_document_code = "LC02", Attached_document_from_rule = 1, Attached_document_name = "IMPORT LICENCE", Attached_document_reference = lic.LicenceNumber + " " + lic.Year, Attached_document_date = DateTime.Now.Date.ToShortDateString() });
                //    }
                //}
            }
            
        }

        public async Task RemoveEntryData(string po)
        {
            using (var ctx = new EntryDataService())
            {
                if (po != null) await ctx.DeleteEntryData(po).ConfigureAwait(false);
            }
            
        }


        public  async Task RemoveItem(int id)
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

        public  async Task ReDoDocumentLineNumbers(int ASYCUDA_Id)
        {
            using (var ctx = new xcuda_ItemService())
            {
                var lst = (await ctx.Getxcuda_ItemByASYCUDA_Id(ASYCUDA_Id.ToString()).ConfigureAwait(false)).OrderBy(x => x.LineNumber);

                for (var i = 0; i < lst.Count(); i++)
                {
                    var itm = lst.ElementAt(i);
                    itm.LineNumber = i + 1;
                    await ctx.Updatexcuda_Item(itm).ConfigureAwait(false);
                }                
            }
        }


        internal  async Task RemoveSelectedItems(List<xcuda_Item> lst)
        {
          
               
                StatusModel.StartStatusUpdate("Removing selected items", lst.Count());

                var docs = lst.Select(x => x.ASYCUDA_Id).ToList();

                foreach (var item in lst.ToList())
                {
                    await DeleteItem(item.Item_Id).ConfigureAwait(false);
                    StatusModel.StatusUpdate();
                }
                foreach (var docId in docs)
                {
                    await ReDoDocumentLineNumbers(docId).ConfigureAwait(false);
                }
                
                StatusModel.StopStatusUpdate();


           
        }

        public async Task DeleteItem(int p)
        {
            //xcuda_Item res;
            using (var ctx = new xcuda_ItemService())
            {
                //res = await ctx.Getxcuda_Item(p.ToString()).ConfigureAwait(false);
                await ctx.Deletexcuda_Item(p.ToString()).ConfigureAwait(false);
            }
           // await DeleteItem(res).ConfigureAwait(false);
        }





        public  async Task<AsycudaDocumentSet> CreateAsycudaDocumentSet(int applicationSettingsId)
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                var doc = await ctx.CreateAsycudaDocumentSet(new AsycudaDocumentSet(){ApplicationSettingsId = applicationSettingsId}).ConfigureAwait(false);
                return doc ;
            }
        }






        public async Task DeleteAsycudaDocument(int ASYCUDA_Id)
        {
            xcuda_ASYCUDA doc = null;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                doc = await ctx.Getxcuda_ASYCUDAByKey(ASYCUDA_Id.ToString(),new List<string>(){ "xcuda_ASYCUDA_ExtendedProperties" }).ConfigureAwait(false);
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

            if(doc.TrackingState == TrackingState.Added) return;
            using (var ctx = new WaterNutDBEntities())
            {
                await ctx.ExecuteStoreCommandAsync($@"DELETE FROM AsycudaSalesAllocations
                                                FROM    AsycudaSalesAllocations INNER JOIN
                                                xcuda_Item ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Item.Item_Id
                                                WHERE(xcuda_Item.ASYCUDA_Id = {doc.ASYCUDA_Id})").ConfigureAwait(false);
            }
            //var lst = new List<AsycudaSalesAllocationsEx>();
            //using (var ctx = new AsycudaSalesAllocationsExService())
            //{
            //    //lst =
            //    //    (await
            //    //        ctx.GetAsycudaSalesAllocationsExBypASYCUDA_Id(doc.ASYCUDA_Id.ToString()).ConfigureAwait(false))
            //    //        .ToList();
            //}

            //if(lst.Count > 0) throw new ApplicationException("Please Remove Sales Allocations before Deleting this IM7");
        }

        private async Task DeleteDocument(xcuda_ASYCUDA doc)
        {
            var docid = doc.ASYCUDA_Id;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                await ctx.Deletexcuda_ASYCUDA(docid.ToString()).ConfigureAwait(false);
            }
            
        }

        private  async Task DeleteItem(xcuda_ASYCUDA doc)
        {
            using (var ctx = new xcuda_ItemService())
            {
                foreach (var item in await ctx.Getxcuda_ItemByASYCUDA_Id(doc.ASYCUDA_Id.ToString()).ConfigureAwait(false))
                {
                    await ctx.Deletexcuda_Item(item.Item_Id.ToString()).ConfigureAwait(false);
                }
            }
        }
        //private async Task DeleteItem(xcuda_Item item)
        //{
        //    using (var ctx = new xcuda_ItemService())
        //    {
        //        item.xBondAllocations.Clear();
        //        item.xcuda_PreviousItems.Clear();
        //        await ctx.Updatexcuda_Item(item).ConfigureAwait(false);
        //        await ctx.Deletexcuda_Item(item.Item_Id.ToString()).ConfigureAwait(false);
        //        MessageBus.Default.BeginNotify(DocumentItemDS.MessageToken.xcuda_ItemDeleted, null,
        //                                              new NotificationEventArgs<xcuda_Item>(DocumentItemDS.MessageToken.xcuda_ItemDeleted, item));
        //    }
        //}

        private  async Task DeleteDocumentPreviousItems(xcuda_ASYCUDA doc)
        {
            using (var ctx = new global::DocumentItemDS.Business.Services.xcuda_PreviousItemService())
            {
                //TODO: replace with deletebyAsycuda_id command
                foreach (
                    var item in await ctx.Getxcuda_PreviousItemByASYCUDA_Id(doc.ASYCUDA_Id.ToString()).ConfigureAwait(false))
                {
                    //item.xcuda_Items.Clear();
                    await ctx.Deletexcuda_PreviousItem(item.PreviousItem_Id.ToString()).ConfigureAwait(false);
                    //MessageBus.Default.BeginNotify(DocumentDS.MessageToken.xcuda_PreviousItemDeleted, null,
                    //    new NotificationEventArgs<global::DocumentDS.Business.Entities.xcuda_PreviousItem>(
                    //        DocumentDS.MessageToken.xcuda_PreviousItemDeleted, item));
                }
            }
        }



        //internal  void ExportDocSet(string dir)
        //{
        //    if (dir == null) return;
        //    var d = new DirectoryInfo(Path.GetDirectoryName(dir));
        //    if (d.Exists)
        //    {
        //        foreach (var doc in Instance.CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Select(c => c.xcuda_ASYCUDA).ToList())
        //        {
        //            var a = new ASYCUDA();
        //            a.LoadFromDataBase(doc.ASYCUDA_Id, a);
        //            a.SaveToFile(Path.Combine(d.FullName, doc.xcuda_Declarant.Number + ".xml"));
        //        }

        //    }
        //}

        internal  void ExporttoXML(string f, xcuda_ASYCUDA currentDocument)
        {

            if (currentDocument != null)
            {
                DocToXML(currentDocument, new FileInfo(f));
               // ExportDocumentToExcel(currentDocument, f);
            }
            else
            {
                throw new ApplicationException("Please Select Asycuda Document to Export");
            }
        }

        //private void ExportDocumentToExcel(xcuda_ASYCUDA doc, string folder)
        //{
        //    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
        //    {
        //         Task.Factory.StartNew(() =>
        //            {
        //                var s = new ExportToExcel<SaleReportLine, List<SaleReportLine>>();
        //                s.StartUp();
                      
        //                    try
        //                    {
        //                        var data =GetDocumentSalesReport(doc.ASYCUDA_Id);
        //                        if (data != null)
        //                        {
        //                            string path = Path.Combine(folder,
        //                                !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".xls");
        //                            s.dataToPrint = data.ToList();
        //                            s.SaveReport(path);
        //                        }
        //                        else
        //                        {
        //                            File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".xls"));
        //                        }
        //                        StatusModel.StatusUpdate();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        throw ex;
        //                    }
                       
        //                s.ShutDown();
        //            },
        //            CancellationToken.None, TaskCreationOptions.None, sta);
        //    }
        //}

        //public List<SaleReportLine> GetDocumentSalesReport(int ASYCUDA_Id)
        //{
        //    try
        //    {


        //        using (var ctx = new AsycudaSalesAllocationsExService())
        //        {
        //            var alst =
        //            ctx.GetAsycudaSalesAllocationsExsByExpression(string.Format("xASYCUDA_Id == {0} " +
        //                                                                               "&& EntryDataDetailsId != null " +
        //                                                                               "&& PreviousItem_Id != null" +
        //                                                                               "&& pRegistrationDate != null",
        //                    ASYCUDA_Id)).Result.ToList();
        //            if (alst.Count <= 0) return null;
        //            var d =
        //                alst.Where(x => x.xLineNumber != null)
        //                    .OrderBy(s => s.xLineNumber)
        //                    .ThenBy(s => s.InvoiceNo)
        //                    .Select(s => new SaleReportLine
        //                    {
        //                        Line = Convert.ToInt32(s.xLineNumber),
        //                        Date = Convert.ToDateTime(s.InvoiceDate),
        //                        InvoiceNo = s.InvoiceNo,
        //                        CustomerName = s.CustomerName,
        //                        ItemNumber = s.ItemNumber,
        //                        ItemDescription = s.ItemDescription,
        //                        TariffCode = s.TariffCode,
        //                        Quantity = Convert.ToDouble(s.QtyAllocated),
        //                        Price = Convert.ToDouble(s.Cost),
        //                        SalesType = s.DutyFreePaid,
        //                        GrossSales = Convert.ToDouble(s.TotalValue),
        //                        PreviousCNumber = s.pCNumber,
        //                        PreviousLineNumber = s.pLineNumber.ToString(),
        //                        PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
        //                        CIFValue =
        //                            (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
        //                            Convert.ToDouble(s.QtyAllocated),
        //                        DutyLiablity =
        //                            (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
        //                            Convert.ToDouble(s.QtyAllocated)
        //                    }).Distinct();



        //            return new List<SaleReportLine>(d);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return null;
        //}


        internal  void DocToXML(xcuda_ASYCUDA doc, FileInfo f)
        {
            File.AppendAllText(Path.Combine(f.DirectoryName, "Instructions.txt"), $"File\t{f.FullName}\r\n");
            var a = new Asycuda421.ASYCUDA();
            a.LoadFromDataBase(doc.ASYCUDA_Id, a, f);
            a.SaveToFile(f.FullName);
        }

        public async Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames, bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages, bool overwriteExisting, bool linkPi)
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet = ctx.AsycudaDocumentSets.FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (docSet == null) throw new ApplicationException("Document Set with reference not found");
                await Task.Run(() =>
                        ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages, overwriteExisting, linkPi, fileNames))
                    .ConfigureAwait(false);
            }
        }

        public async Task ImportDocuments(AsycudaDocumentSet docSet, IEnumerable<string> fileNames, bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages, bool overwriteExisting ,bool linkPi)
        {
                await Task.Run(() =>
                    ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages, overwriteExisting,linkPi, fileNames))
                    .ConfigureAwait(false);
        }

        private void ImportDocuments(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument,
            bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi,  IEnumerable<string> fileNames)
        {
            //Asycuda.ASYCUDA.NewAsycudaDocumentSet()
            //StatusModel.RefreshNow();
            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(fileNames, new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount*1},//
                f => //
                {
                    try
                    {
                        

                        if (ASYCUDA.CanLoadFromFile(f))
                        {
                            LoadAsycuda421(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                                overwriteExisting, linkPi, f,ref exceptions);
                        }
                        else
                        {
                            if (!noMessages)
                                throw new ApplicationException($"Can not Load file '{f}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions.Enqueue(ex);
                    }

                }
                );

            if (exceptions.Count > 0) throw new ApplicationException(exceptions.FirstOrDefault().Message + "|" + exceptions.FirstOrDefault().StackTrace);
        }



        private void LoadAsycuda421(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument, bool importTariffCodes,
           bool noMessages, bool overwriteExisting, bool linkPi, string f,ref ConcurrentQueue<Exception> exceptions)
        {
            StatusModel.StatusUpdate();
            try
            {

                var a = Asycuda421.ASYCUDA.LoadFromFile(f);
                using (var ctx = new CoreEntitiesContext())
                {
                    var declarantCode = ctx.ApplicationSettings.First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).DeclarantCode;
                    var fileCode = a.Warehouse.Identification.Text.FirstOrDefault()?? a.Declarant.Declarant_code.Text.FirstOrDefault();
                    if (!fileCode.Contains(declarantCode))
                    {
                        throw new ApplicationException($"Could not import file - '{f} - The file is for another warehouse{fileCode}. While this Warehouse is {declarantCode}");
                    }
                }
                    

                if (a != null)
                {
                    var importer = new AsycudaToDataBase421();
                    importer.UpdateItemsTariffCode = importTariffCodes;
                    importer.ImportOnlyRegisteredDocuments = importOnlyRegisteredDocument;
                    importer.OverwriteExisting = overwriteExisting;
                    importer.NoMessages = noMessages;
                    importer.LinkPi = linkPi;
                    importer.SaveToDatabase(a, docSet).Wait();
                }
                //await a.SaveToDatabase(a).ConfigureAwait(false);

                Debug.WriteLine(f);
            }

            catch (Exception Ex)
            {
                if (!noMessages)
                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not import file - '{f}. Error:{(Ex.InnerException ?? Ex).Message} Stacktrace:{(Ex.InnerException ?? Ex).StackTrace}"));
            }
        }

        public async Task ExportDocument(string filename, xcuda_ASYCUDA doc )
        {
            Instance.ExporttoXML(filename, doc);
           
        }

        public  void IM72Ex9Document(string filename)
        {
            try
            {
                var zeroitems = "";
                // create blank asycuda document
                dynamic olddoc;
                if (ASYCUDA.CanLoadFromFile(filename))
                {
                    olddoc = ASYCUDA.LoadFromFile(filename);

                }
                else if (Asycuda421.ASYCUDA.CanLoadFromFile(filename))
                {
                    olddoc = Asycuda421.ASYCUDA.LoadFromFile(filename);
                }
                else
                {
                    
                        throw new ApplicationException($"Can not Load file '{filename}'");
                }


                
                var newdoc = Asycuda421.ASYCUDA.LoadFromFile(filename);

                newdoc.Container = null;

                if (olddoc.Identification.Registration.Date == null)
                {
                    throw new ApplicationException("Document is not Assesed! Convert Assessed Documents only");
                }


                newdoc.Item.Clear();




                var linenumber = 0;
                foreach (var olditem in olddoc.Item)
                {

                    linenumber += 1;


                    // create new entry
                    var i = olditem.Clone();


                    var extemp = ExportTemplates.Where(x => x.Description.ToUpper() == "IM9").FirstOrDefault();
                    if (extemp != null)
                    {
                        var customsProcedure = extemp.Customs_Procedure;
                        if (customsProcedure != null)
                        {
                            i.Tarification.Extended_customs_procedure = customsProcedure.Split('-')[0];
                            i.Tarification.National_customs_procedure = customsProcedure.Split('-')[1];
                        }
                    }

                    i.Previous_doc.Summary_declaration.Text.Clear();
                    i.Previous_doc.Summary_declaration.Text.Add(
                        $"{olddoc.Identification.Office_segment.Customs_clearance_office_code.Text[0]} {DateTime.Parse(olddoc.Identification.Registration.Date).Year.ToString()} C {olddoc.Identification.Registration.Number} art. {linenumber}");


                    // create previous item


                    var pitm = new ASYCUDAPrev_decl();
                    pitm.Prev_decl_HS_code = i.Tarification.HScode.Commodity_code;
                    pitm.Prev_decl_HS_prec = "00";
                    pitm.Prev_decl_current_item = linenumber.ToString(); // piggy back the previous item count
                    pitm.Prev_decl_item_number = linenumber.ToString();

                    pitm.Prev_decl_weight = olditem.Valuation_item.Weight_itm.Net_weight_itm.ToString(); //System.Convert.ToDecimal(pline.Net_weight_itm) / System.Convert.ToDecimal(pline.ItemQuantity) * System.Convert.ToDecimal(fa.DutyFreeQuantity);
                    pitm.Prev_decl_weight_written_off = olditem.Valuation_item.Weight_itm.Net_weight_itm.ToString();




                    pitm.Prev_decl_number_packages_written_off = Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();
                    pitm.Prev_decl_number_packages = Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();


                    pitm.Prev_decl_supp_quantity = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();
                    pitm.Prev_decl_supp_quantity_written_off = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();


                    pitm.Prev_decl_country_origin = olditem.Goods_description.Country_of_origin_code;

                    var oq = "";

                    if (string.IsNullOrEmpty(olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity) || olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity == "0")
                    {
                        oq = "1";
                        zeroitems = "ZeroItems";
                    }
                    else
                    {
                        oq = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();
                    }


                    pitm.Prev_decl_ref_value_written_off = (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq)).ToString();
                    pitm.Prev_decl_ref_value = (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq)).ToString();// * System.Convert.ToDecimal(fa.QUANTITY);
                    pitm.Prev_decl_reg_serial = "C";
                    pitm.Prev_decl_reg_number = olddoc.Identification.Registration.Number;
                    pitm.Prev_decl_reg_year = DateTime.Parse(olddoc.Identification.Registration.Date).Year.ToString();
                    pitm.Prev_decl_office_code = olddoc.Identification.Office_segment.Customs_clearance_office_code.Text[0];

                    newdoc.Prev_decl.Add(pitm);



                    i.Valuation_item.Item_Invoice.Currency_code = "XCD";
                    i.Valuation_item.Item_Invoice.Amount_foreign_currency = olditem.Valuation_item.Total_CIF_itm;
                    i.Valuation_item.Item_Invoice.Amount_national_currency = olditem.Valuation_item.Total_CIF_itm;
                    i.Valuation_item.Statistical_value = olditem.Valuation_item.Total_CIF_itm;

                    newdoc.Item.Add(i);



                }

                newdoc.Identification.Manifest_reference_number = null;
                newdoc.Identification.Type.Type_of_declaration = "Ex";
                newdoc.Identification.Type.Declaration_gen_procedure_code = "9";
                newdoc.Declarant.Reference.Number.Text.Add("Ex9For" + newdoc.Identification.Registration.Number); 
                
                newdoc.Valuation.Gs_Invoice.Currency_code.Text.Add("XCD");
                newdoc.Valuation.Gs_Invoice.Amount_foreign_currency = Math.Round(newdoc.Item.Sum(i => Convert.ToDouble(i.Valuation_item.Total_CIF_itm)), 2).ToString();
                newdoc.Valuation.Gs_Invoice.Amount_national_currency = Math.Round(newdoc.Item.Sum(i => Convert.ToDouble(i.Valuation_item.Total_CIF_itm)), 2).ToString();

                var oldfile = new FileInfo(filename);
                newdoc.SaveToFile(Path.Combine(oldfile.DirectoryName, oldfile.Name.Replace(oldfile.Extension, "") + "-Ex9" + zeroitems + oldfile.Extension));
            }
            catch (Exception Ex)
            {
                throw;
            }

        }

        public async Task ExportDocSet(int AsycudaDocumentSetId, string directoryName, bool overWrite)
        {
           var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);
           ExportDocSet(docset, directoryName, overWrite);
        }

        public  async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int asycudaDocumentSetId )
        {
            
            using (var ctx = new AsycudaDocumentSetService())
            {
               return await ctx.GetAsycudaDocumentSetByKey(asycudaDocumentSetId.ToString(),
                   new List<string>()
                   {
                       "xcuda_ASYCUDA_ExtendedProperties",
                       "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                       "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_Declarant",
                       "AsycudaDocumentSet_Attachments.Attachment",
                        "Customs_Procedure.Document_Type",
                        "Document_Type"
                   }).ConfigureAwait(false);
            }
        }

        internal void ExportDocSet(AsycudaDocumentSet docSet, string directoryName, bool overWrite)
        {
            
                            StatusModel.StartStatusUpdate("Exporting Files", docSet.Documents.Count());
            var exceptions = new ConcurrentQueue<Exception>();
            foreach (var doc in docSet.Documents)
            {
                //if (doc.xcuda_Item.Any() == true)
                //{
                try
                {
                    if (overWrite == true || !File.Exists(Path.Combine(directoryName, doc.ReferenceNumber + ".xml")))
                        Instance.DocToXML(doc, new FileInfo(Path.Combine(directoryName, doc.ReferenceNumber + ".xml")));
                    StatusModel.StatusUpdate();
                    // ExportDocumentToExcel(doc, directoryName);
                }
                catch (Exception ex)
                {

                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not import file - '{doc.ReferenceNumber}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
                }

                ////}
            }

            if (exceptions.Count <= 0) return;
            var fault = new ValidationFault
            {
                Result = false,
                Message = exceptions.First().Message,
                Description = exceptions.First().StackTrace
            };
            throw new FaultException<ValidationFault>(fault, new FaultReason(fault.Message));

        }

         AsycudaDocumentSet _currentAsycudaDocumentSet = null;
        //public  AsycudaDocumentSet CurrentAsycudaDocumentSet
        //{
        //    get
        //    {
        //        if (
        //            QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocumentSetEx != null &&
        //            (_currentAsycudaDocumentSet == null ||
        //                _currentAsycudaDocumentSet.AsycudaDocumentSetId != QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId))
        //        {
        //            _currentAsycudaDocumentSet = GetAsycudaDocumentSet(QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocumentSetEx.AsycudaDocumentSetId, new List<string>()
        //            {
        //                "xcuda_ASYCUDA_ExtendedProperties",
        //                "Customs_Procedure",
        //                "Document_Type"
        //            }).Result;
        //            return _currentAsycudaDocumentSet;
        //        }
        //        else
        //        {
        //            return _currentAsycudaDocumentSet;
        //        }
        //    }
        //    set
        //    {
        //       _currentAsycudaDocumentSet = value;
               
        //       MessageBus.Default.BeginNotify<string>(MessageToken.CurrentAsycudaDocumentSetExIDChanged, null,
        //           new NotificationEventArgs<string>(MessageToken.CurrentAsycudaDocumentSetExIDChanged,
        //                                _currentAsycudaDocumentSet != null ? _currentAsycudaDocumentSet.AsycudaDocumentSetId.ToString() : "0"));

        //       MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentsChanged, null,
        //               new NotificationEventArgs(MessageToken.AsycudaDocumentsChanged));

        //       MessageBus.Default.BeginNotify(MessageToken.AsycudaDocumentSetExsChanged, null,
        //               new NotificationEventArgs(MessageToken.AsycudaDocumentSetExsChanged));
              
        //    }
        //}




        //public  xcuda_ASYCUDA CurrentAsycudaDocument
        //{
        //    get
        //    {
        //        if (QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocument != null)
        //        {
        //            using (var ctx = new xcuda_ASYCUDAService())
        //            {
        //                return ctx.Getxcuda_ASYCUDA(QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocument.ASYCUDA_Id.ToString(),
        //                    new List<string>()
        //                    {
        //                        "xcuda_Declarant"
        //                    }).Result;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        //public  InventoryItem CurrentInventoryItem
        //{
        //    get
        //    {
        //        if (QuerySpace.InventoryQS.DataModels.BaseDataModel.Instance.CurrentInventoryItemsEx != null)
        //        {
        //            using (var ctx = new InventoryItemService())
        //            {
        //                return ctx.GetInventoryItem(QuerySpace.InventoryQS.DataModels.BaseDataModel.Instance.CurrentInventoryItemsEx.ItemNumber.ToString()).Result;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        //public  EntryDataDetails CurrentEntryDataDetail
        //{
        //    get
        //    {
        //        using (var ctx = new EntryDataDetailsService())
        //        {
        //            return ctx.GetEntryDataDetails(QuerySpace.EntryDataQS.DataModels.BaseDataModel.Instance.CurrentEntryDataDetailsEx.EntryDataDetailsId.ToString()).Result;
        //        }
        //    }            
        //}

        //public  AsycudaSalesAllocations CurrentAsycudaSalesAllocation
        //{
        //    get
        //    {
        //        if (QuerySpace.AllocationQS.DataModels.BaseDataModel.Instance.CurrentAsycudaSalesAllocationsEx != null)
        //        {
        //            using (var ctx = new AsycudaSalesAllocationsService())
        //            {
        //                return ctx.GetAsycudaSalesAllocations(QuerySpace.AllocationQS.DataModels.BaseDataModel.Instance.CurrentAsycudaSalesAllocationsEx.AllocationId.ToString()).Result;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        //public  TariffCode CurrentTariffCode
        //{
        //    get
        //    {
        //        if (QuerySpace.InventoryQS.DataModels.BaseDataModel.Instance.CurrentTariffCodes != null)
        //        {
        //            using (var ctx = new TariffCodeService())
        //            {
        //                return ctx.GetTariffCode(QuerySpace.InventoryQS.DataModels.BaseDataModel.Instance.CurrentTariffCodes.TariffCode.ToString()).Result;
        //            }
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        
         public  ApplicationSettings CurrentApplicationSettings { get; set; }
        //{
        //    get
        //    {
        //        return QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentApplicationSettings;
        //    }
        //}
         IEnumerable<ExportTemplate> _exportTemplates = null;
        public  IEnumerable<ExportTemplate> ExportTemplates
        {
            get
            {
                if (_exportTemplates == null)
                {
                    using (var ctx = new ExportTemplateService())
                    {
                        _exportTemplates =  ctx.GetExportTemplates().Result;
                    }
                }
                return _exportTemplates;
            }
        }
         IEnumerable<Customs_Procedure> _customs_Procedures = null;
        public  IEnumerable<Customs_Procedure> Customs_Procedures
        {
            get
            {
                if (_customs_Procedures == null)
                {
                    using (var ctx = new Customs_ProcedureService())
                    {
                       _customs_Procedures = ctx.GetCustoms_Procedure(new List<string>(){ "Document_Type" }).Result;
                    }
                }
                return _customs_Procedures;
            }
        }

         IEnumerable<Document_Type> _document_Types = null;
        private readonly CreateIM9 _createIm9;

        public  IEnumerable<Document_Type> Document_Types
        {
            get
            {
                if (_document_Types == null)
                {
                    using (var ctx = new Document_TypeService())
                    {
                        _document_Types = ctx.GetDocument_Type().Result;
                    }
                }
                return _document_Types;
            }
        }

        public async Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(
            IEnumerable<EntryDataDetailsEx> lst)
        {
            var res = new List<EntryDataDetails>();
            if (lst.Any())
            {
                using (var ctx = new EntryDataDetailsService())
                {
                    foreach (var item in lst.Where(x => x != null))
                    {

                        res.Add(await ctx.GetEntryDataDetailsByKey(item.EntryDataDetailsId.ToString(),
                            new List<string>()
                            {
                                "EntryDataDetailsEx",
                                "InventoryItems",
                                "EntryData.ContainerEntryData"
                              // , "InventoryItem.TariffCodes.TariffCategory.TariffSupUnitLkps"
                            }).ConfigureAwait(false));
                    }
                }
               
            }
             return res.OrderBy(x => x.EntryDataDetailsId);
        }

        public async Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(IEnumerable<int> lst)
        {
            var res = new List<EntryDataDetails>();
            if (lst.Any())
            {
                using (var ctx = new EntryDataDetailsService())
                {
                    foreach (var item in lst.Where(x => x != 0))
                    {

                        res.Add(await ctx.GetEntryDataDetailsByKey(item.ToString(),
                            new List<string>()
                            {
                                "EntryDataDetailsEx",
                                "InventoryItems",
                                "EntryData.ContainerEntryData"
                                // , "InventoryItem.TariffCodes.TariffCategory.TariffSupUnitLkps"
                            }).ConfigureAwait(false));
                    }
                }

            }
            return res.OrderBy(x => x.EntryDataDetailsId);
        }

        public async Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(IEnumerable<string> elst )
        {
                
                var res = new List<EntryDataDetails>();
               if (elst.Any())
                    {
                        using (var ctx = new EntryDataDetailsService())
                        {
                            foreach (var item in elst.Where(x => x != null))
                            {

                                res.AddRange(await ctx.GetEntryDataDetailsByEntryDataId(item, new List<string>()
                                {
                                    "EntryDataDetailsEx",
                                    "InventoryItems",
                                    "EntryData.ContainerEntryData"
                                   //,"InventoryItems.TariffCodes.TariffCategory.TariffSupUnitLkps"
                                }).ConfigureAwait(false));
                            }
                        }
                    }
               return res.OrderBy(x => x.EntryDataDetailsId);
        }



        //public  string CurrentAsycudaItemEntryId
        //{
        //    get
        //    {
        //        return QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocumentItemID ?? null;
        //    }

        //}

        public async Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem)
        {
            if (asycudaDocumentItem == null) return;
            //get the original item
          var i =  await GetDocumentItem(asycudaDocumentItem.Item_Id, new List<string>()
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
                i.xcuda_PreviousItem.Net_weight = asycudaDocumentItem.Net_weight_itm;
                i.Net_weight = asycudaDocumentItem.Net_weight_itm;
                i.Gross_weight = asycudaDocumentItem.Net_weight_itm;
            }
       
            await Save_xcuda_Item(i).ConfigureAwait(false);

        }

        private async Task<xcuda_Item> GetDocumentItem(int item_Id, List<string> includeLst)
        {
            using (var ctx = new xcuda_ItemService(){StartTracking = true})
            {
                xcuda_Item i = null;
                i = await ctx.Getxcuda_ItemByKey(item_Id.ToString(), includeLst).ConfigureAwait(false);
                return i;
            }
        }

        internal async Task SaveEntryDataDetailsEx(EntryDataDetailsEx entryDataDetailsEx)
        {
            using (var ctx = new EntryDataDetailsService())
            {
                var i =
                   await ctx.GetEntryDataDetailsByKey(entryDataDetailsEx.EntryDataDetailsId.ToString()).ConfigureAwait(false);
                i.InjectFrom(entryDataDetailsEx);
                await ctx.UpdateEntryDataDetails(i).ConfigureAwait(false);

               
                //MessageBus.Default.BeginNotify(QuerySpace.EntryDataQS.MessageToken.CurrentEntryDataDetailsExChanged, this, new NotificationEventArgs<EntryDataDetailsEx>(QuerySpace.EntryDataQS.MessageToken.CurrentEntryDataDetailsExChanged, null));
            } 
        }

        public async Task SaveAsycudaDocument(AsycudaDocument asycudaDocument)
        {
            asycudaDocument.ModifiedProperties = null;
            if (asycudaDocument == null) return;
            //get the original item
            var i = await GetDocument(asycudaDocument.ASYCUDA_Id, new List<string>()
          {
              "xcuda_ASYCUDA_ExtendedProperties",
                "xcuda_Identification",
                "xcuda_Valuation.xcuda_Gs_Invoice",
                "xcuda_Declarant",
                "xcuda_General_information.xcuda_Country",
                "xcuda_Property"
          }).ConfigureAwait(false);
            i.StartTracking();
            // null for now cuz there are no navigation properties involved.
            i.InjectFrom(asycudaDocument);
            i.xcuda_ASYCUDA_ExtendedProperties.StartTracking();
            i.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = asycudaDocument.EffectiveRegistrationDate;
            i.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = asycudaDocument.DoNotAllocate;
            await Save_xcuda_ASYCUDA(i).ConfigureAwait(false);
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
                                               where ASYCUDA_Id = @ASYCUDA_Id", new SqlParameter("@ASYCUDA_Id", SqlDbType.Int).Value = da.Document.ASYCUDA_Id).ConfigureAwait(false);
                


            }
            
        }

        public async Task LinkExistingPreviousItems(DocumentCT da)
        {
            //get all previous items for this document
            try
            {

                IEnumerable<xcuda_PreviousItem> plst;

                plst = await DocumentItemDS.DataModels.BaseDataModel.Instance.Searchxcuda_PreviousItem(
                    new List<string>()
                    {
                        $"Prev_reg_nbr == \"{da.Document.xcuda_Identification.xcuda_Registration.Number}\"",
                    }).ConfigureAwait(false);


                if (plst.Any() == false) return;
                foreach (var itm in da.DocumentItems)
                {
                    var pplst = plst.Where(x => x.Previous_item_number == itm.LineNumber.ToString() &&
                                                x.Prev_decl_HS_spec == itm.ItemNumber);
                    //if(pplst.Any == false) MessageBox.Show("Please Import 
                    foreach (var p in pplst)
                    {
                        var ep = new EntryPreviousItems(true)
                        {
                            Item_Id = itm.Item_Id,
                            PreviousItem_Id = p.PreviousItem_Id,
                            TrackingState = TrackingState.Added
                        };
                        itm.xcuda_PreviousItems.Add(ep);
                        //await DIBaseDataModel.Instance.SaveEntryPreviousItems(ep).ConfigureAwait(false);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }



        public async Task SaveEntryPreviousItems(List<CoreEntities.Business.Entities.EntryPreviousItems> epi)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.ApplyChanges(epi);
                ctx.SaveChanges();
            }
        }

        #region IAsyncInitialization Members

        public static Task Initialization { get; private set; }

     
        #endregion


        
    }

    public class SaleReportLine
    {
        public int Line { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public string TariffCode { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string SalesType { get; set; }
        public double GrossSales { get; set; }
        public string PreviousCNumber { get; set; }
        public string PreviousLineNumber { get; set; }
        public string PreviousRegDate { get; set; }
        public double CIFValue { get; set; }
        public double DutyLiablity { get; set; }
    }

    public class EntryDataDetailSummary
    {
        public string EntryDataId { get; set; }
        public int EntryDataDetailsId { get; set; }
        public DateTime EntryDataDate { get; set; }

        public double QtyAllocated { get; set; }
        public DateTime EffectiveDate
        { get; set; }

        public string Currency { get; set; }
        public int? LineNumber { get; set; }
    }
}
