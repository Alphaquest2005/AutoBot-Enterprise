

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Core.Common.Data;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Entities;
using EntryDataQS.Business.Entities;
using Omu.ValueInjecter;
using TrackableEntities;

using WaterNut.Asycuda;
using DocumentItemDS.Business.Entities;
using Core.Common.UI;
using InventoryDS.Business.Entities;
using DocumentDS.Business.Services;

using InventoryDS.Business.Services;
using WaterNut.Business.Entities;
using WaterNut.DataSpace.Asycuda;
using WaterNut.Interfaces;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using Customs_ProcedureService = DocumentDS.Business.Services.Customs_ProcedureService;
using Document_Type = DocumentDS.Business.Entities.Document_Type;
using Document_TypeService = DocumentDS.Business.Services.Document_TypeService;
using EntryData = EntryDataDS.Business.Entities.EntryData;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;
using EntryDataDetailsEx = EntryDataQS.Business.Entities.EntryDataDetailsEx;
using EntryDataDetailsService = EntryDataDS.Business.Services.EntryDataDetailsService;
using EntryDataService = EntryDataDS.Business.Services.EntryDataService;
using WaterNutDBEntities = WaterNut.DataLayer.WaterNutDBEntities;
using xcuda_Item = DocumentItemDS.Business.Entities.xcuda_Item;
using xcuda_ItemService = DocumentItemDS.Business.Services.xcuda_ItemService;
using xcuda_Item_Invoice = DocumentItemDS.Business.Entities.xcuda_Item_Invoice;
using xcuda_PreviousItem = DocumentItemDS.Business.Entities.xcuda_PreviousItem;
using xcuda_PreviousItemService = DocumentItemDS.Business.Services.xcuda_PreviousItemService;
using xcuda_Supplementary_unit = DocumentItemDS.Business.Entities.xcuda_Supplementary_unit;


namespace WaterNut.DataSpace
{
    public partial  class BaseDataModel
    {
        private static readonly BaseDataModel instance;
        static BaseDataModel()
        {
            instance = new BaseDataModel();
            Initialization = InitializationAsync();
        }

        public static  BaseDataModel Instance
        {
            get { return instance; }
        }

       
        private static async Task InitializationAsync()
        {
            StatusModel.Timer("Loading Data");
            var tasks = new List<Task>();
            
                _inventoryCache =
                    new DataCache<InventoryItem>(
                       await InventoryDS.DataModels.BaseDataModel.Instance.SearchInventoryItem(new List<string>() { "All" },
                            null).ConfigureAwait(false))
                    ;
           
             _tariffCodeCache = new DataCache<TariffCode>(await InventoryDS.DataModels.BaseDataModel.Instance.SearchTariffCode(new List<string>() { "All" }).ConfigureAwait(false)); 

            _document_TypeCache = new DataCache<Document_Type>(await DocumentDS.DataModels.BaseDataModel.Instance.SearchDocument_Type(new List<string>() { "All" }).ConfigureAwait(false)); 

            _customs_ProcedureCache = new DataCache<Customs_Procedure>(await DocumentDS.DataModels.BaseDataModel.Instance.SearchCustoms_Procedure(new List<string>() { "All" }).ConfigureAwait(false));

            _documentCache =
    new DataCache<xcuda_ASYCUDA>(

             DocumentDS.DataModels.BaseDataModel.Instance.Searchxcuda_ASYCUDA(new List<string>() { "All" },
                new List<string>()
                              {
                                  "xcuda_Identification.xcuda_Registration"
                              }).Result);



            _documentItemCache =
                new DataCache<AsycudaDocumentItem>(BaseDataModel.Instance.GetAllDocumentItems().Result);

            StatusModel.StopStatusUpdate();
        }

        public static DataCache<InventoryItem> _inventoryCache;
        public static DataCache<TariffCode> _tariffCodeCache;
        public static DataCache<Customs_Procedure> _customs_ProcedureCache;
        public static DataCache<Document_Type> _document_TypeCache;
        public static DataCache<xcuda_ASYCUDA> _documentCache;
        public static DataCache<AsycudaDocumentItem> _documentItemCache;


        public DataCache<InventoryItem> InventoryCache { get { return _inventoryCache; } }
        public DataCache<TariffCode> TariffCodeCache {  get { return _tariffCodeCache; } }
        public DataCache<Customs_Procedure> Customs_ProcedureCache {  get { return _customs_ProcedureCache; } }
        public DataCache<Document_Type> Document_TypeCache {  get { return _document_TypeCache; } }

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
            var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId, new List<string>()
            {
                 "xcuda_ASYCUDA_ExtendedProperties",
                 "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                 "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_Declarant"
            }).ConfigureAwait(false);
            await ClearAsycudaDocumentSet(docset).ConfigureAwait(false);
        }

        public async Task ClearAsycudaDocumentSet(AsycudaDocumentSet docset)
        {
            
            StatusModel.StartStatusUpdate("Deleting Documents from '' Document Set", docset.xcuda_ASYCUDA_ExtendedProperties.Count());

            var doclst = docset.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.xcuda_ASYCUDA != null).ToList();
            //foreach (var item in doclst)
            Parallel.ForEach(doclst, new ParallelOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, item =>
            {
                StatusModel.StatusUpdate();

                //_asycudaDocuments.Remove(item.xcuda_ASYCUDA);

                //await DeleteAsycudaDocument(item.xcuda_ASYCUDA).ConfigureAwait(false);
                 DeleteAsycudaDocument(item.xcuda_ASYCUDA).Wait();

            }
                );
          
            StatusModel.StopStatusUpdate();
            
        }

        public interface IEntryLineData
        {
            string ItemNumber { get; set; }
             string ItemDescription { get; set; }
             string TariffCode { get; set; }
             double Cost { get; set; }
             string PreviousDocumentItemId { get; set; }
             double Quantity { get; set; }
             List<IEntryDataDetail> EntryDataDetails { get; set; }
             IDocumentItem PreviousDocumentItem { get; set; }
             IInventoryItem InventoryItem { get; set; }
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
                            InventoryItem = ctx.GetInventoryItemByKey(_itemNumber, new List<string>()
                            {
                                "TariffCodes.TariffCategory.TariffSupUnitLkps"
                            } ).Result;
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
            
            string _previousDocumentItemId;
            public string PreviousDocumentItemId
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
                        if (_previousDocumentItemId != null)
                        {
                            PreviousDocumentItem = ctx.Getxcuda_ItemByKey(_previousDocumentItemId).Result;
                        }
                        else
                        {
                            PreviousDocumentItem = null;
                        }
                    }
                }
            }
            public double Quantity { get; set; }
            public List<IEntryDataDetail> EntryDataDetails { get; set; }
            public IDocumentItem PreviousDocumentItem { get; set; }
            public IInventoryItem InventoryItem { get; set; }

        }


        public  void IntCdoc(xcuda_ASYCUDA doc, Document_Type dt, AsycudaDocumentSet ads)
        {
            var cdoc = new DocumentCT {Document = doc};
            IntCdoc(cdoc, dt, ads);
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

        public xcuda_ASYCUDA CreateNewAsycudaDocument(AsycudaDocumentSet CurrentAsycudaDocumentSet)
        {
            var ndoc = new xcuda_ASYCUDA() { TrackingState = TrackingState.Added };// 
            ndoc.SetupProperties();

            if (CurrentAsycudaDocumentSet != null)
            {
                CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(ndoc.xcuda_ASYCUDA_ExtendedProperties);
                ndoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = CurrentAsycudaDocumentSet;
                ndoc.xcuda_ASYCUDA_ExtendedProperties.FileNumber = CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Count();
            }
            return ndoc;
        }

        public  void IntCdoc(DocumentCT cdoc, Document_Type dt, AsycudaDocumentSet ads)
        {

            cdoc.Document.xcuda_Declarant.Number = ads.Declarant_Reference_Number + "-F" + cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber.ToString();
            cdoc.Document.xcuda_Identification.Manifest_reference_number = ads.Manifest_Number;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = ads.AsycudaDocumentSetId;

            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_Type = dt;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Document_TypeId = dt.Document_TypeId;
            cdoc.Document.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code = dt.Declaration_gen_procedure_code;
            cdoc.Document.xcuda_Identification.xcuda_Type.Type_of_declaration = dt.Type_of_declaration;
            cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination = ads.Country_of_origin_code;
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

        public async Task AddToEntry(List<EntryDataEx> entryDatalst, AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            if (!IsValidDocument(currentAsycudaDocumentSet)) return;

            var slstSource = (from s in await GetSelectedPODetails(entryDatalst).ConfigureAwait(false)//.Where(p => p.Downloaded == false)
                              select s).ToList(); ;
            if (!IsValidEntryData(slstSource)) return;

            await CreateEntryItems(slstSource, currentAsycudaDocumentSet).ConfigureAwait(false);
        }

        public async Task AddToEntry(List<EntryDataDetailsEx> entryDataDetailslst, AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            if (!IsValidDocument(currentAsycudaDocumentSet)) return;

            var slstSource = (from s in await GetSelectedPODetails(entryDataDetailslst).ConfigureAwait(false)//.Where(p => p.Downloaded == false)
                              select s).ToList(); ;
            if (!IsValidEntryData(slstSource)) return;

            await CreateEntryItems(slstSource, currentAsycudaDocumentSet).ConfigureAwait(false);
        }

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

        private async Task CreateEntryItems(List<EntryDataDetails> slstSource, AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            var itmcount = 0;
            var slst = CreateEntryLineData(slstSource);

            var cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};

            //BaseDataModel.Instance.CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(cdoc.xcuda_ASYCUDA_ExtendedProperties);
            IntCdoc(cdoc, currentAsycudaDocumentSet.Document_Type,
                currentAsycudaDocumentSet);
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = true;

            var entryLineDatas = slst as IList<EntryLineData> ?? slst.ToList();
            StatusModel.StartStatusUpdate("Adding Entries to New Asycuda Document", entryLineDatas.Count());

            foreach (var pod in entryLineDatas.OrderBy(p => p.InventoryItem.TariffCode))
            {
                var itm = CreateItemFromEntryDataDetail(pod, cdoc);

                if (itm == null) continue;

                itmcount += 1;
                if (itmcount%CurrentApplicationSettings.MaxEntryLines == 0)
                {
                    await SaveDocumentCT(cdoc).ConfigureAwait(false);
                    cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};

                    //BaseDataModel.Instance.CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(cdoc.xcuda_ASYCUDA_ExtendedProperties);
                    IntCdoc(cdoc, currentAsycudaDocumentSet.Document_Type,
                        currentAsycudaDocumentSet);
                    cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = true;
                }


                StatusModel.StatusUpdate();
                //System.Windows.Forms.MessageBox.
            }
            StatusModel.Timer("Saving To Database");

            await SaveDocumentCT(cdoc).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();


          
        }

        private  IEnumerable<EntryLineData> CreateEntryLineData(IEnumerable<EntryDataDetails> slstSource)
        {
            var slst = from s in slstSource.AsEnumerable()//.Where(p => p.Downloaded == false)
                group s by new {s.ItemNumber, s.ItemDescription, s.TariffCode, s.Cost}
                into g
                select new EntryLineData
                {
                    ItemNumber = g.Key.ItemNumber,
                    ItemDescription = g.Key.ItemDescription,
                    TariffCode = g.Key.TariffCode,
                    Cost = g.Key.Cost,
                    Quantity = g.Sum(x => x.Quantity),
                    EntryDataDetails = g.Select(x => x as IEntryDataDetail).ToList()
                };
            return slst;
        }

        public async Task<xcuda_ASYCUDA> GetDocument(int ASYCUDA_Id, List<string> includeLst = null )
        {
            using (var ctx = new xcuda_ASYCUDAService())
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

                using (var ctx = new xcuda_ASYCUDAService())
                {
                    cdoc.Document = await ctx.Updatexcuda_ASYCUDA(cdoc.Document).ConfigureAwait(false);
                }
                if (cdoc.Document.ASYCUDA_Id == 0) return;
               
                    // prepare items for parrallel import
                    foreach (var item in cdoc.DocumentItems)
                    {
                        item.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                        item.LineNumber = cdoc.DocumentItems.IndexOf(item) + 1;
                        if (item.xcuda_PreviousItem != null)
                            item.xcuda_PreviousItem.ASYCUDA_Id = cdoc.Document.ASYCUDA_Id;
                    }

                    //Parallel.ForEach(cdoc.DocumentItems, new ParallelOptions(){MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, item =>
                    //{
                    
                        using (var ctx = new xcuda_ItemService())
                        {
                            //foreach (var item in cdoc.DocumentItems)
                            //{
                            cdoc.DocumentItems.AsParallel().ForAll((t) => ctx.Updatexcuda_Item(t).Wait());
                            //    await ctx.Updatexcuda_Item(cdoc.DocumentItems).ConfigureAwait(false);
                            //}
                        }
                    //});
                
            }
            catch (Exception)
            {
                throw;
            }

        }

        internal  xcuda_Item CreateItemFromEntryDataDetail(IEntryLineData pod, DocumentCT cdoc)
        {
           // if (pod.TariffCode != null)
           // {
            try
            {

               var itm = CreateNewDocumentItem();
                cdoc.DocumentItems.Add(itm);
                itm.SetupProperties();
               
                itm.xcuda_Goods_description.Commercial_Description = pod.InventoryItem.Description;
                if (cdoc.Document.xcuda_General_information != null)
                    itm.xcuda_Goods_description.Country_of_origin_code = cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination;
                itm.xcuda_Tarification.Item_price = Convert.ToSingle(pod.Cost * pod.Quantity);
                itm.xcuda_Tarification.National_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.National_customs_procedure; //cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Customs_Procedure.National_customs_procedure;
                itm.xcuda_Tarification.Extended_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Extended_customs_procedure;//cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Customs_Procedure.Extended_customs_procedure;

                itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pod.TariffCode ?? "NULL";
                itm.xcuda_Tarification.xcuda_HScode.Precision_4 = pod.PreviousDocumentItem == null ? pod.ItemNumber : pod.PreviousDocumentItem.ItemNumber;
                itm.xcuda_Tarification.xcuda_HScode.InventoryItems = pod.InventoryItem ;

                if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != null)
                    itm.xcuda_Previous_doc.Summary_declaration = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber;

                itm.xcuda_Valuation_item.Total_CIF_itm = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));
                itm.xcuda_Valuation_item.Statistical_value = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));



                var ivc = new xcuda_Item_Invoice(){TrackingState = TrackingState.Added};

                ivc.Amount_national_currency = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost) * Convert.ToDecimal(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Exchange_Rate), 4));
                ivc.Amount_foreign_currency = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));


                if (cdoc.Document.xcuda_Valuation != null && cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice != null)
                {
                    //;

                    ivc.Currency_code = cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;//xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Currency_Code;
                    ivc.Currency_rate = cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate;//Convert.ToSingle(cdoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Exchange_Rate);
                }

                itm.xcuda_Valuation_item.xcuda_Item_Invoice = ivc;

                //itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm();
                //itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = (Single)pod.Quantity * Convert.ToSingle(.1); //(Decimal)ops.Quantity;
                //itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = (Single)pod.Quantity * Convert.ToSingle(.1); //(Decimal)ops.Quantity;


                foreach (var ed in pod.EntryDataDetails)
                {
                    itm.EntryDataDetails.Add(new xcuda_ItemEntryDataDetails {Item_Id = itm.Item_Id , xcuda_Item = itm, EntryDataDetailsId = ed.EntryDataDetailsId, TrackingState = TrackingState.Added});
                }


                itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit() {Tarification_Id = itm.xcuda_Tarification.Item_Id, Suppplementary_unit_code = "NMB", Suppplementary_unit_quantity = pod.Quantity, TrackingState = TrackingState.Added});

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

        private  xcuda_Item CreateNewDocumentItem()
        {
            return new xcuda_Item(){TrackingState = TrackingState.Added};//
        }

        private  void ProcessItemTariff(IEntryLineData pod, xcuda_ASYCUDA cdoc, xcuda_Item itm)
        {

            if (pod.InventoryItem.TariffCode != null)
            {

                if ((pod.InventoryItem as ITariffUnit) != null)
                {
                    var tariffSupUnitLkps = (pod.InventoryItem as ITariffUnit).TariffSupUnitLkps;
                    if (tariffSupUnitLkps != null)
                        foreach (var item in tariffSupUnitLkps.ToList())
                        {
                            itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit() { Suppplementary_unit_code = item.SuppUnitCode2, Suppplementary_unit_quantity = pod.Quantity * item.SuppQty, TrackingState = TrackingState.Added });
                        }
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

        public async Task RemoveEntryData(EntryData po)
        {
            using (var ctx = new EntryDataService())
            {
                if (po != null) await ctx.DeleteEntryData(po.EntryDataId).ConfigureAwait(false);
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

        private  async Task ReDoDocumentLineNumbers(int ASYCUDA_Id)
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





        public  async Task<AsycudaDocumentSet> CreateAsycudaDocumentSet()
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                var doc = await ctx.CreateAsycudaDocumentSet(new AsycudaDocumentSet()).ConfigureAwait(false);
                return doc ;
            }
        }






        public async Task DeleteAsycudaDocument(int ASYCUDA_Id)
        {
            xcuda_ASYCUDA doc = null;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                doc = await ctx.Getxcuda_ASYCUDAByKey(ASYCUDA_Id.ToString()).ConfigureAwait(false);
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
                await ctx.Updatexcuda_ASYCUDA(doc).ConfigureAwait(false);
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

            await DeleteDocumentPreviousItems(doc).ConfigureAwait(false);
            await DeleteItem(doc).ConfigureAwait(false);
            await DeleteDocument(doc).ConfigureAwait(false);
        }

        private  async Task DeleteDocument(xcuda_ASYCUDA doc)
        {
            using (var ctx = new xcuda_ASYCUDAService())
            {
                await ctx.Deletexcuda_ASYCUDA(doc.ASYCUDA_Id.ToString()).ConfigureAwait(false);
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
            using (var ctx = new global::DocumentDS.Business.Services.xcuda_PreviousItemService())
            {
                //TODO: replace with deletebyAsycuda_id command
                foreach (
                    var item in await ctx.Getxcuda_PreviousItemByASYCUDA_Id(doc.ASYCUDA_Id.ToString()).ConfigureAwait(false))
                {
                    //item.xcuda_Items.Clear();
                    await ctx.Deletexcuda_PreviousItem(item.PreviousItem_Id.ToString());
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
                DocToXML(currentDocument, f);
            }
            else
            {
                throw new ApplicationException("Please Select Asycuda Document to Export");
            }
        }

        internal  void DocToXML(xcuda_ASYCUDA doc, string f)
        {

            var a = new ASYCUDA();
            a.LoadFromDataBase(doc.ASYCUDA_Id, a);
            a.SaveToFile(f);
        }

        public async Task ImportDocuments(AsycudaDocumentSet docSet, IEnumerable<string> fileNames, bool importOnlyRegisteredDocument = true, bool importTariffCodes = true, bool noMessages = false, bool overwriteExisting = true)
        {
                await Task.Run(() =>
                    ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages, overwriteExisting, fileNames))
                    .ConfigureAwait(false);
        }

        private  void ImportDocuments(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, IEnumerable<string> fileNames )
        {
            //Asycuda.ASYCUDA.NewAsycudaDocumentSet()
            //StatusModel.RefreshNow();
            Parallel.ForEach(fileNames, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 1 }, f => //
            {
                StatusModel.StatusUpdate();
                try
                {
                    var a = ASYCUDA.LoadFromFile(f);

                    if (a != null)
                    {
                        var importer = new AsycudaToDataBase();
                        importer.UpdateItemsTariffCode = importTariffCodes;
                        importer.ImportOnlyRegisteredDocuments = importOnlyRegisteredDocument;
                        importer.OverwriteExisting = overwriteExisting;
                        importer.NoMessages = noMessages;
                        importer.SaveToDatabase(a, docSet).Wait();
                    }
                    //await a.SaveToDatabase(a).ConfigureAwait(false);

                    Debug.WriteLine(f);
                }
                catch (AggregateException ae)
                {
                    foreach (var ex in ae.InnerExceptions)
                    {
                        throw ex;
                    }
                }
                catch (Exception Ex)
                {
                    if (!noMessages)
                        throw new ApplicationException(string.Format("Could not import file - '{0}. Error:{1} Stacktrace:{2}", f,
                            Ex.Message, Ex.StackTrace));
                }
            }
                );
        }

        public  void ExportDocument(string filename, xcuda_ASYCUDA doc )
        {
            Instance.ExporttoXML(filename, doc);
        }

        public  void IM72Ex9Document(string filename)
        {
            try
            {
                var zeroitems = "";
                // create blank asycuda document
                var olddoc = ASYCUDA.LoadFromFile(filename);
                var newdoc = ASYCUDA.LoadFromFile(filename);

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




                    i.Tarification.Extended_customs_procedure = ExportTemplates.Where(x => x.Description == "Ex9").FirstOrDefault().Customs_Procedure.Split('-')[0];
                    i.Tarification.National_customs_procedure = ExportTemplates.Where(x => x.Description == "Ex9").FirstOrDefault().Customs_Procedure.Split('-')[1];

                    i.Previous_doc.Summary_declaration.Text.Clear();
                    i.Previous_doc.Summary_declaration.Text.Add(String.Format("{0} {1} C {2} art. {3}", olddoc.Identification.Office_segment.Customs_clearance_office_code.Text.FirstOrDefault(),
                                                                                                  DateTime.Parse(olddoc.Identification.Registration.Date).Year.ToString(),
                                                                                                  olddoc.Identification.Registration.Number, linenumber));


                    // create previous item


                    var pitm = new ASYCUDAPreviousItem();
                    pitm.Hs_code = i.Tarification.HScode.Commodity_code;
                    pitm.Commodity_code = "00";
                    pitm.Current_item_number = linenumber.ToString(); // piggy back the previous item count
                    pitm.Previous_item_number = linenumber.ToString();

                    pitm.Net_weight = olditem.Valuation_item.Weight_itm.Net_weight_itm.ToString(); //System.Convert.ToDecimal(pline.Net_weight_itm) / System.Convert.ToDecimal(pline.ItemQuantity) * System.Convert.ToDecimal(fa.DutyFreeQuantity);
                    pitm.Prev_net_weight = olditem.Valuation_item.Weight_itm.Net_weight_itm.ToString();




                    pitm.Previous_Packages_number = Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();
                    pitm.Packages_number = Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();


                    pitm.Suplementary_Quantity = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();
                    pitm.Preveious_suplementary_quantity = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();


                    pitm.Goods_origin = olditem.Goods_description.Country_of_origin_code;

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


                    pitm.Previous_value = (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq)).ToString();
                    pitm.Current_value = (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq)).ToString();// * System.Convert.ToDecimal(fa.QUANTITY);
                    pitm.Prev_reg_ser = "C";
                    pitm.Prev_reg_nbr = olddoc.Identification.Registration.Number;
                    pitm.Prev_reg_dat = DateTime.Parse(olddoc.Identification.Registration.Date).Year.ToString();
                    pitm.Prev_reg_cuo = olddoc.Identification.Office_segment.Customs_clearance_office_code.Text.FirstOrDefault();

                    newdoc.PreviousItem.Add(pitm);



                    i.Valuation_item.Item_Invoice.Currency_code = "XCD";
                    i.Valuation_item.Item_Invoice.Amount_foreign_currency = olditem.Valuation_item.Total_CIF_itm;
                    i.Valuation_item.Item_Invoice.Amount_national_currency = olditem.Valuation_item.Total_CIF_itm;
                    i.Valuation_item.Statistical_value = olditem.Valuation_item.Total_CIF_itm;

                    newdoc.Item.Add(i);



                }

                newdoc.Identification.Manifest_reference_number = null;
                newdoc.Identification.Type.Type_of_declaration = "Ex";
                newdoc.Identification.Type.Declaration_gen_procedure_code = "9";
                newdoc.Declarant.Reference.Number = "Ex9For" + newdoc.Identification.Registration.Number;
                
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

        internal async Task ExportDocSet(int AsycudaDocumentSetId, string directoryName)
        {
           var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId,  new List<string>()
                   {
                       "xcuda_ASYCUDA_ExtendedProperties",
                       "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                       "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_Declarant"
                       }).ConfigureAwait(false);
           ExportDocSet(docset, directoryName);
        }

        public  async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int AsycudaDocumentSetId, List<string> includesLst )
        {
            
            using (var ctx = new AsycudaDocumentSetService())
            {
               return await ctx.GetAsycudaDocumentSetByKey(AsycudaDocumentSetId.ToString(),
                   new List<string>()
                   {
                       "xcuda_ASYCUDA_ExtendedProperties",
                       "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                       "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_Declarant",
                       
                        "Customs_Procedure",
                        "Document_Type"
                   }).ConfigureAwait(false);
            }
        }

        internal  void ExportDocSet(AsycudaDocumentSet docSet, string directoryName)
        {
            
                            StatusModel.StartStatusUpdate("Exporting Files", docSet.Documents.Count);
                            Parallel.ForEach(docSet.Documents, doc =>
                            {
                                //if (doc.xcuda_Item.Any() == true)
                                //{
                                Instance.DocToXML(doc, Path.Combine(directoryName, doc.ReferenceNumber + ".xml"));
                                StatusModel.StatusUpdate();
                                ////}
                            });
              
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

         public ApplicationSettings CurrentApplicationSettings { get; set; }
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
                       _customs_Procedures = ctx.GetCustoms_Procedure().Result;
                    }
                }
                return _customs_Procedures;
            }
        }

         IEnumerable<Document_Type> _document_Types = null;
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

        public async Task<List<EntryDataDetails>> GetSelectedPODetails(
            List<EntryDataDetailsEx> lst)
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
                                "EntryDataDetailsEx"
                               //, "InventoryItem.TariffCodes.TariffCategory.TariffSupUnitLkps"
                            }).ConfigureAwait(false));
                    }
                }
               
            }
             return res;
        }

        public async Task<List<EntryDataDetails>> GetSelectedPODetails(List<EntryDataEx> elst )
        {
                
                var res = new List<EntryDataDetails>();
               if (elst.Any())
                    {
                        using (var ctx = new EntryDataDetailsService())
                        {
                            foreach (var item in elst.Where(x => x != null))
                            {

                                res.AddRange(await ctx.GetEntryDataDetailsByEntryDataId(item.InvoiceNo, new List<string>()
                                {
                                    "EntryDataDetailsEx"
                                   // ,"InventoryItems.TariffCodes.TariffCategory.TariffSupUnitLkps"
                                }).ConfigureAwait(false));
                            }
                        }
                    }
            return res;
        }



        //public  string CurrentAsycudaItemEntryId
        //{
        //    get
        //    {
        //        return QuerySpace.CoreEntities.DataModels.BaseDataModel.Instance.CurrentAsycudaDocumentItemID ?? null;
        //    }

        //}

        internal async Task SaveAsycudaDocumentItem(AsycudaDocumentItem asycudaDocumentItem)
        {
            if (asycudaDocumentItem == null) return;
            //get the original item
          var i =  await GetDocumentItem(asycudaDocumentItem.Item_Id, new List<string>()
          {
              "xcuda_Tarification.xcuda_HScode",
              "xcuda_Valuation_item.xcuda_Weight_itm"
          }).ConfigureAwait(false);
            // null for now cuz there are no navigation properties involved.
            i.InjectFrom(asycudaDocumentItem);

            await Save_xcuda_Item(i).ConfigureAwait(false);

        }

        private async Task<xcuda_Item> GetDocumentItem(int item_Id, List<string> includeLst)
        {
            using (var ctx = new xcuda_ItemService())
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
            if (asycudaDocument == null) return;
            //get the original item
            var i = await GetDocument(asycudaDocument.ASYCUDA_Id, new List<string>()
          {
              "xcuda_ASYCUDA_ExtendedProperties"
          }).ConfigureAwait(false);
            // null for now cuz there are no navigation properties involved.
            i.InjectFrom(asycudaDocument);

            await Save_xcuda_ASYCUDA(i).ConfigureAwait(false);
        }

        private async Task Save_xcuda_ASYCUDA(xcuda_ASYCUDA i)
        {
            if (i == null) return;
            using (var ctx = new xcuda_ASYCUDAService())
            {
                await ctx.Updatexcuda_ASYCUDA(i).ConfigureAwait(false);
            }
        }

        internal async Task DeleteDocumentCt(DocumentCT da)
        {
            using (var ctx = new WaterNutDBEntities())
            {
               await ctx.ExecuteStoreCommandAsync(@"delete from xcuda_Item
                                               where ASYCUDA_Id = @ASYCUDA_Id

                                               delete from xcuda_ASYCUDA
                                               where ASYCUDA_Id = @ASYCUDA_Id", new SqlParameter("@ASYCUDA_Id", SqlDbType.Int)).ConfigureAwait(false);
            }
            
        }

        #region IAsyncInitialization Members

        public static Task Initialization { get; private set; }

        #endregion
    }
}
