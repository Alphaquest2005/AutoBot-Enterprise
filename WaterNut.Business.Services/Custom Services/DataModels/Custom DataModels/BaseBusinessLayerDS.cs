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
    public class BaseDataModel
    {
        public static DataCache<Customs_Procedure> _customs_ProcedureCache;
        

        
        private readonly AsycudaDocumentSet _currentAsycudaDocumentSet = null;

        private IEnumerable<Customs_Procedure> _customs_Procedures;

        

        private IEnumerable<ExportTemplate> _exportTemplates;
        private static readonly double _minimumPossibleAsycudaWeight = .01;
        private static double _runningMiniumWeight = 0.0;
        private static readonly double WeightAsycudaNormallyOffBy = 0.5;

        static BaseDataModel()
        {
            Instance = new BaseDataModel
            {
                CurrentApplicationSettings = new CoreEntitiesContext().ApplicationSettings.First()
            };

            Initialization = InitializationAsync();
        }

        public static BaseDataModel Instance { get; }


        


        public DataCache<Customs_Procedure> Customs_ProcedureCache => _customs_ProcedureCache;

       


        public ApplicationSettings CurrentApplicationSettings { get; set; }

        public IEnumerable<ExportTemplate> ExportTemplates
        {
            get
            {
                if (_exportTemplates == null)
                    using (var ctx = new ExportTemplateService())
                    {
                        _exportTemplates = ctx.GetExportTemplates().Result;
                    }

                return _exportTemplates;
            }
        }

        public IEnumerable<Customs_Procedure> Customs_Procedures
        {
            get
            {
                if (_customs_Procedures == null)
                    using (var ctx = new DocumentDSContext())
                    {
                        _customs_Procedures = ctx.Customs_Procedure
                            .Include(x => x.CustomsOperation)
                            .Include(x => x.Document_Type)
                            .Where(x => x.BondTypeId == CurrentApplicationSettings.BondTypeId
                                        && x.IsObsolete != true).ToList();
                    }

                return _customs_Procedures;
            }
        }

       

        #region IAsyncInitialization Members

        public static Task Initialization { get; }
        public double ResourcePercentage { get; } = 0.8;
        public List<SessionSchedule> CurrentSessionSchedule { get; set; } = new List<SessionSchedule>();
        public SessionActions CurrentSessionAction { get; set; }

        public static Client GetClient()
        {
            return new Client
            {
                CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
                DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
                Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
                EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList()
            };
        }

        #endregion


        private static async Task InitializationAsync()
        {
            StatusModel.Timer("Loading Data");
            


            SQLBlackBox.RunSqlBlackBox();

            
           
            _customs_ProcedureCache =
                new DataCache<Customs_Procedure>(
                    await
                        DocumentDS.DataModels.BaseDataModel.Instance.SearchCustoms_Procedure(new List<string> {"All"})
                            .ConfigureAwait(false));


            StatusModel.StopStatusUpdate();
        }

        public static (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) CurrentSalesInfo(int months)
        {
            return  GetSessionParameterMonths() ?? GetCurrentSalesInfo(months);
        }

        private static (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath) GetCurrentSalesInfo(int months)
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(months);
            return EntryDocSetUtils.CreateMonthYearAsycudaDocSet(startDate);
        }


        private static (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath)? GetSessionParameterMonths()
        {

            var parameterSet = BaseDataModel.Instance.CurrentSessionSchedule.FirstOrDefault(x =>
                x.SesseionId == BaseDataModel.Instance.CurrentSessionAction.SessionId)?.ParameterSet;
            if (BaseDataModel.Instance.CurrentSessionAction == null || parameterSet == null) return null;

            var startDate = DateTime.Parse(parameterSet.ParameterSetParameters.Select(x => x.Parameters).FirstOrDefault(x => x.Name == "StartDate")?.Value);

            var endDate = DateTime.Parse(parameterSet.ParameterSetParameters.Select(x => x.Parameters).FirstOrDefault(x => x.Name == "EndDate")?.Value);

            var docSet = WaterNut.DataSpace.EntryDocSetUtils.GetAsycudaDocumentSet(parameterSet.ParameterSetParameters.Select(x => x.Parameters).FirstOrDefault(x => x.Name == "AsycudaDocumentSet")?.Value ?? "Unknown", false);

            var dirPath =
                StringExtensions.UpdateToCurrentUser(
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docSet.Declarant_Reference_Number));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);


            return (StartDate: startDate, EndDate: endDate, DocSet: docSet, DirPath: dirPath);
        }

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

        internal async Task Clear(int AsycudaDocumentSetId)
        {
            AsycudaDocumentSet docset = null;
            using (var ctx = new AsycudaDocumentSetService())
            {
                docset = await ctx.GetAsycudaDocumentSetByKey(AsycudaDocumentSetId.ToString(),
                    new List<string>
                    {
                        "xcuda_ASYCUDA_ExtendedProperties",
                        "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                        "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_PreviousItem"
                    }).ConfigureAwait(false);
            }

            Clear(docset);
        }

        internal async void Clear(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            await ClearAsycudaDocumentSet(currentAsycudaDocumentSet).ConfigureAwait(false);
        }

        public async Task ClearAsycudaDocumentSet(int AsycudaDocumentSetId)
        {
            var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);
            await ClearAsycudaDocumentSet(docset).ConfigureAwait(false);
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

        public async Task ClearAsycudaDocumentSet(AsycudaDocumentSet docset)
        {
            PreventDeletingFromSystemDocSet(docset);

            StatusModel.StartStatusUpdate($"Deleting Documents from '{docset.Declarant_Reference_Number}' Document Set",
                docset.xcuda_ASYCUDA_ExtendedProperties.Count());

            ParalellDeleteDocSetDocuments(docset);

            await CalculateDocumentSetFreight(docset.AsycudaDocumentSetId).ConfigureAwait(false);

            StatusModel.StopStatusUpdate();
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
                item =>
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
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }


        public async Task<DocumentCT> CreateDocumentCt(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
            var cdoc = new DocumentCT
            {
                Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet),
                DocumentItems = new List<xcuda_Item>()
            };

            return cdoc;
        }

       

        public xcuda_ASYCUDA CreateNewAsycudaDocument(AsycudaDocumentSet CurrentAsycudaDocumentSet)
        {
            var ndoc = new xcuda_ASYCUDA(true) {TrackingState = TrackingState.Added};
            ndoc.SetupProperties();

            if (CurrentAsycudaDocumentSet != null)
            {
                CurrentAsycudaDocumentSet.xcuda_ASYCUDA_ExtendedProperties.Add(ndoc.xcuda_ASYCUDA_ExtendedProperties);
                ndoc.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet = CurrentAsycudaDocumentSet;
                ndoc.xcuda_ASYCUDA_ExtendedProperties.FileNumber =
                    CurrentAsycudaDocumentSet.LastFileNumber
                        .GetValueOrDefault(); // the number is forward looking
                CurrentAsycudaDocumentSet.LastFileNumber = UpdateAsycudaDocumentSetLastNumber(
                    CurrentAsycudaDocumentSet.AsycudaDocumentSetId,
                    CurrentAsycudaDocumentSet.LastFileNumber.GetValueOrDefault());
            }

            return ndoc;
        }


        public int UpdateAsycudaDocumentSetLastNumber(int docSetId, int num)
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSetRef = ctx.AsycudaDocumentSets.First(x => x.AsycudaDocumentSetId == docSetId)
                    .Declarant_Reference_Number;
                while (ctx.xcuda_ASYCUDA.FirstOrDefault(x =>
                           x.xcuda_Declarant.Number.Contains(docSetRef) && x.xcuda_Declarant.Number.EndsWith((num + 1).ToString())) !=
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

        public void IntCdoc(DocumentCT cdoc, AsycudaDocumentSet ads, string prefix = "F")
        {
            cdoc.Document.xcuda_Declarant.Number = ads.Declarant_Reference_Number + $"-{prefix}" +
                                                   cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber;
            cdoc.Document.xcuda_Declarant.Declarant_code =
                CurrentApplicationSettings.Declarants.First(x => x.IsDefault).DeclarantCode;
            cdoc.Document.xcuda_Identification.Manifest_reference_number = ads.Manifest_Number;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = ads.AsycudaDocumentSetId;

            
            cdoc.Document.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code =
                ads.Customs_Procedure.Document_Type.Declaration_gen_procedure_code;
            cdoc.Document.xcuda_Identification.xcuda_Type.Type_of_declaration =
                ads.Customs_Procedure.Document_Type.Type_of_declaration;
            cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination =
                ads.Country_of_origin_code;
            cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = ads.Country_of_origin_code;
            cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code =
                ads.Country_of_origin_code;

            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = Convert.ToSingle(ads.Exchange_Rate);
            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = ads.Currency_Code;

            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId !=
                ads.Customs_Procedure.Customs_ProcedureId)
            {
                if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId != 0)
                {
                    var c = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure;
                    foreach (var item in cdoc.DocumentItems.Where(x =>
                        x.xcuda_Tarification.Extended_customs_procedure == c.Extended_customs_procedure &&
                        x.xcuda_Tarification.National_customs_procedure == c.National_customs_procedure).ToList())
                    {
                        item.xcuda_Tarification.Extended_customs_procedure =
                            ads.Customs_Procedure.Extended_customs_procedure;
                        item.xcuda_Tarification.National_customs_procedure =
                            ads.Customs_Procedure.National_customs_procedure;
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
                    foreach (var item in cdoc.DocumentItems
                        .Where(x => x.xcuda_Previous_doc.Summary_declaration == b).ToList())
                        item.xcuda_Previous_doc.Summary_declaration = ads.BLNumber;
                }

                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber = ads.BLNumber;
            }


            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = ads.Description;


            using (var ctx = new DocumentDSContext())
            {
                var Exp = ctx.ExportTemplates
                    .Where(x => x.ApplicationSettingsId ==
                                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                                    .ApplicationSettingsId)
                    .First(x =>
                        x.Customs_Procedure == cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsProcedure);
                
                cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Destination.Destination_country_code =
                    Exp.Destination_country_code;
                cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_region =
                    Exp.Trading_country;
                if (string.IsNullOrEmpty(ads.Currency_Code))
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = Exp.Gs_Invoice_Currency_code;
                if (string.IsNullOrEmpty(ads.Country_of_origin_code))
                {
                    cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = Exp.Trading_country;
                    cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination =
                        Exp.Country_first_destination;

                    cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code =
                        Exp.Export_country_code;
                }

                if (Exp.Delivery_terms_Code != null)
                {
                    if (cdoc.Document.xcuda_Transport == null)
                        cdoc.Document.xcuda_Transport = new List<xcuda_Transport>();
                    var deliveryTerms = cdoc.Document.xcuda_Transport.FirstOrDefault()?.xcuda_Delivery_terms
                        .FirstOrDefault();
                    if (deliveryTerms == null)
                        cdoc.Document.xcuda_Transport.Add(new xcuda_Transport(true)
                        {
                            xcuda_Delivery_terms = new List<xcuda_Delivery_terms>
                            {
                                new xcuda_Delivery_terms(true)
                                {
                                    Code = Exp.Delivery_terms_Code,
                                    TrackingState = TrackingState.Added
                                }
                            },
                            TrackingState = TrackingState.Added
                        });
                    else
                        deliveryTerms.Code = Exp.Delivery_terms_Code;
                }

                cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_name = Exp.Consignee_name;
                cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_code = Exp.Consignee_code;
            }
        }


        public async Task AddToEntry(IEnumerable<string> entryDatalst, AsycudaDocumentSet currentAsycudaDocumentSet,
            bool perInvoice, bool combineEntryDataInSameFile, bool groupItems, bool checkPackages)
        {
            try
            {

                if (!IsValidDocument(currentAsycudaDocumentSet)) return;

                var slstSource =
                    (from s in await GetSelectedPODetails(entryDatalst.Distinct().ToList(),
                            currentAsycudaDocumentSet.AsycudaDocumentSetId).ConfigureAwait(false)
                        select s).ToList();
                ;
                if (!IsValidEntryData(slstSource)) return;

                await CreateEntryItems(slstSource, currentAsycudaDocumentSet, perInvoice, true, false,
                        combineEntryDataInSameFile, groupItems, checkPackages)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<DocumentCT>> AddToEntry(IEnumerable<int> entryDatalst, int docSetId, bool perInvoice,
            bool combineEntryDataInSameFile, bool groupItems)
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
                    (from s in await GetSelectedPODetails(entryDatalst.Distinct().ToList(), docSetId)
                            .ConfigureAwait(false)
                        select s).ToList();

                if (!IsValidEntryData(slstSource)) return new List<DocumentCT>();

                return await CreateEntryItems(slstSource, docSet, perInvoice, true, false, combineEntryDataInSameFile,
                    groupItems, true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task ValidateExistingTariffCodes(AsycudaDocumentSet currentAsycudaDocumentSet)
        {
        }

        public static Customs_Procedure GetCustomsProcedure(string dfp, string DocumentType)
        {
            Customs_Procedure customsProcedure;
            var isPaid = dfp == "Duty Paid";
            Func<Customs_Procedure, bool> dtpredicate = x => false;
            switch (DocumentType)
            {
                case "PO":
                    dtpredicate = x =>
                        x.CustomsOperationId == (int)CustomsOperations.Warehouse
                        && x.Sales == true && x.Stock != true;

                    break;
                case "Sales":
                    dtpredicate = x =>
                        x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Sales == true && x.IsPaid == isPaid;
                    break;
                case "DIS":
                    dtpredicate = x =>
                        x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Discrepancy == true &&
                        x.IsPaid == isPaid;
                    break;
                case "ADJ":
                    dtpredicate = x =>
                        x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Adjustment == true &&
                        x.IsPaid == isPaid;
                    break;
                case "IM9":
                        dtpredicate = x =>
                                        x.CustomsOperationId == (int)CustomsOperations.Exwarehouse && x.Stock == true;
                    break;
                default:
                    throw new ApplicationException("Document Type");
            }
            
            
            return GetCustoms_Procedure(dtpredicate);
        }

        public static Customs_Procedure GetCustoms_Procedure(Func<Customs_Procedure, bool> dtpredicate)
        {
            return BaseDataModel.Instance.Customs_Procedures
                .Where(dtpredicate)
                .OrderByDescending(x => x.IsDefault == true)
                .First();
        }

        public static string SetFilename(string droppedFilePath, string targetFileName, string nameExtension)
        {
            string filename;

            var file = new FileInfo(droppedFilePath);
            filename = $"{Path.Combine(file.DirectoryName)}\\{targetFileName}{nameExtension}";
            if (!File.Exists(filename)) File.Copy(droppedFilePath, filename);

            return filename;
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

        public async Task<List<DocumentCT>> CreateEntryItems(List<EntryDataDetails> slstSource,
            AsycudaDocumentSet currentAsycudaDocumentSet, bool perInvoice, bool autoUpdate, bool autoAssess,
            bool combineEntryDataInSameFile, bool groupItems, bool checkPackages, string prefix = null)
        {
            var docList = new List<DocumentCT>();
            var itmcount = 0;
            var slst = groupItems ? CreateGroupEntryLineData(slstSource) : CreateSingleEntryLineData(slstSource);

            var cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};


            IntCdoc(cdoc, currentAsycudaDocumentSet, prefix);

            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
            if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
            AttachCustomProcedure(cdoc, currentAsycudaDocumentSet.Customs_Procedure);
            var entryLineDatas = slst as IList<EntryLineData> ?? slst.ToList();
            StatusModel.StartStatusUpdate("Adding Entries to New Asycuda Document", entryLineDatas.Count());


            //////////////////////////////////////////////////////
            /// 
            /// If per invoice is choosen and sorted by tariff code it will alternate between invoice no and create too much entries
            /// left as is because this is not a problem
            /// 
            switch (CurrentApplicationSettings.OrderEntriesBy)
            {
                case "TariffCode":
                    if (combineEntryDataInSameFile)
                        entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.SourceFile)
                            .ThenBy(p => p.EntryData.EntryDataId)
                            .ThenBy(p => p.InventoryItem?.TariffCode ?? p.TariffCode).ToList();
                    else
                        entryLineDatas = entryLineDatas.OrderBy(p => p.InventoryItem?.TariffCode ?? p.TariffCode)
                            .ToList();

                    break;
                case "Invoice":
                    if (combineEntryDataInSameFile)
                        entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.SourceFile)
                            .ThenBy(p => p.EntryData.EntryDataId).ToList();
                    else
                        entryLineDatas = entryLineDatas.OrderBy(p => p.EntryData.EntryDataId).ToList();

                    break;
            }


            var oldentryData = new EntryData();
            foreach (var pod in entryLineDatas) //
            {
                var remainingPackages = pod.EntryData.Packages.GetValueOrDefault();
                var possibleEntries =
                    Math.Ceiling(pod.EntryDataDetails.Count /
                                 (double) (currentAsycudaDocumentSet.MaxLines ??
                                           CurrentApplicationSettings.MaxEntryLines));


                if (checkPackages)
                    if (combineEntryDataInSameFile == false && remainingPackages < possibleEntries)
                        throw new ApplicationException("Entry data lines need more packages");

                if (pod.EntryData.DocumentType?.DocumentType != null && cdoc.DocumentItems.Count == 0 &&
                    string.IsNullOrEmpty(oldentryData.EntryDataId))
                {
                    var cp = AttachEntryDataDocumentType(cdoc, pod.EntryData.DocumentType);
                }

                if (oldentryData.EntryDataId != pod.EntryData.EntryDataId)
                    if (perInvoice)
                        if (cdoc.DocumentItems.Any() && oldentryData.EntryDataId != pod.EntryData.EntryDataId)
                            if (combineEntryDataInSameFile && pod.EntryData.SourceFile != oldentryData.SourceFile ||
                                perInvoice && combineEntryDataInSameFile == false)
                            {
                                SetEffectiveAssessmentDate(cdoc);


                                LinkPreviousDocuments(pod, cdoc);

                                await SaveDocumentCT(cdoc).ConfigureAwait(false);
                                docList.Add(cdoc);
                                cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};


                                var cp = currentAsycudaDocumentSet.Customs_Procedure;
                                IntCdoc(cdoc, currentAsycudaDocumentSet);
                                if (pod.EntryData.DocumentType?.DocumentType != null)
                                    cp = AttachEntryDataDocumentType(cdoc, pod.EntryData.DocumentType);
                                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
                                if (autoAssess)
                                    cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                                AttachCustomProcedure(cdoc, cp);
                                itmcount = 0;
                            }
                            else
                            {
                                SetPackages(ref remainingPackages, ref possibleEntries, pod, cdoc);
                            }


                cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code =
                    pod.EntryData.Currency ?? currentAsycudaDocumentSet.Currency_Code;


                var itm = CreateItemFromEntryDataDetail(pod, cdoc);

                if (pod.EntryData is PurchaseOrders p)
                {
                    if (itmcount == 0 && p.FinancialInformation != null)
                        cdoc.Document.xcuda_Traders.xcuda_Traders_Financial = new xcuda_Traders_Financial
                        {
                            Financial_name = p.FinancialInformation,
                            TrackingState = TrackingState.Added,
                            xcuda_Traders = cdoc.Document.xcuda_Traders
                        };

                    cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name =
                        $"INV# {p.SupplierInvoiceNo}\r\n" +
                        $"{pod.EntryData.Suppliers?.SupplierName}\r\n" +
                        $"{pod.EntryData.Suppliers?.Street}\r\n";
                }

                if (pod.EntryData is Adjustments a)
                {
                  
                    cdoc.Document.xcuda_Traders.xcuda_Exporter.Exporter_name =
                        $"{pod.EntryData.Suppliers?.SupplierName ?? a.Vendor}\r\n" +
                        $"{pod.EntryData.Suppliers?.Street}\r\n";
                }


                if (itm == null) continue;
                itmcount += 1;

                if (itmcount == 1 && cdoc.DocumentItems.Any() && !cdoc.DocumentItems.First().xcuda_Packages.Any())
                    SetPackages(ref remainingPackages, ref possibleEntries, pod, cdoc);

                if (oldentryData.EntryDataId != pod.EntryData.EntryDataId)
                {
                    AttachDocSetDocumentsToDocuments(currentAsycudaDocumentSet, pod, cdoc);
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_internal_freight.Amount_foreign_currency +=
                        pod.EntryData.TotalInternalFreight.GetValueOrDefault();
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_internal_freight.Currency_code =
                        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_deduction.Amount_foreign_currency +=
                        pod.EntryData.TotalDeduction.GetValueOrDefault();
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_deduction.Currency_code =
                        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_insurance.Amount_foreign_currency +=
                        pod.EntryData.TotalInsurance.GetValueOrDefault();
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_insurance.Currency_code =
                        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_other_cost.Amount_foreign_currency +=
                        pod.EntryData.TotalOtherCost.GetValueOrDefault();
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_other_cost.Currency_code =
                        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code;

                    if (Instance.CurrentApplicationSettings.AssessIM7 == true &&
                        pod.EntryData.Suppliers == null)
                        throw new ApplicationException($"Supplier not found for InvoiceNo {pod.EntryData.EntryDataId}");


                    oldentryData = pod.EntryData;
                }


                if (itmcount % (currentAsycudaDocumentSet.MaxLines ?? CurrentApplicationSettings.MaxEntryLines) == 0)
                    if (cdoc.DocumentItems.Any())
                    {
                        SetEffectiveAssessmentDate(cdoc);
                        LinkPreviousDocuments(pod, cdoc);
                        await SaveDocumentCT(cdoc).ConfigureAwait(false);
                        docList.Add(cdoc);
                        cdoc = new DocumentCT {Document = CreateNewAsycudaDocument(currentAsycudaDocumentSet)};
                        var cp = currentAsycudaDocumentSet.Customs_Procedure;
                        IntCdoc(cdoc, currentAsycudaDocumentSet);
                        if (pod.EntryData.DocumentType?.DocumentType != null)
                            cp = AttachEntryDataDocumentType(cdoc, pod.EntryData.DocumentType);
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = autoUpdate;
                        if (autoAssess) cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = true;
                        AttachCustomProcedure(cdoc, cp);
                        itmcount = 0;
                    }

                LinkPreviousDocuments(pod, cdoc);

                StatusModel.StatusUpdate();
            }

            StatusModel.Timer("Saving To Database");
            if (cdoc.DocumentItems.Any())
            {
                SetEffectiveAssessmentDate(cdoc);
                await SaveDocumentCT(cdoc).ConfigureAwait(false);
                docList.Add(cdoc);
            }

            await CalculateDocumentSetFreight(currentAsycudaDocumentSet.AsycudaDocumentSetId).ConfigureAwait(false);
            StatusModel.StopStatusUpdate();

            AttachToExistingDocuments(currentAsycudaDocumentSet.AsycudaDocumentSetId);
            SetInvoicePerline(docList.Select(x => x.Document.ASYCUDA_Id).ToList());
            RenameDuplicateDocumentCodes(docList.Select(x => x.Document.ASYCUDA_Id).ToList());

            return docList;
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

        private static void SetPackages(ref int remainingPackages, ref double possibleEntries, BaseDataModel.EntryLineData pod,
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

        private void AttachDocSetDocumentsToDocuments(AsycudaDocumentSet currentAsycudaDocumentSet, BaseDataModel.EntryLineData pod,
            DocumentCT cdoc)
        {
            var alst = currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments
                .Where(x => x.Attachment.FilePath.Contains(pod.EntryData.EntryDataId) &&
                            (x.FileType.DocumentCode == "IV05" || x.FileType.DocumentCode == "DO02")
                )
                .Select(x => x.Attachment)
                .DistinctBy(x => x.Id)
                .ToList();
            if (!(pod.EntryData is PurchaseOrders p)) return;
            if (p.PreviousCNumber != null)
            {
                AddPreviousDocument(currentAsycudaDocumentSet, cdoc, p, alst);
            }

            AttachToDocument(alst.GroupBy(x => new FileInfo(x.FilePath).Name).Select(x => x.Last()).ToList(),
                cdoc.Document, cdoc.DocumentItems);
        }

        private static void AddPreviousDocument(AsycudaDocumentSet currentAsycudaDocumentSet, DocumentCT cdoc, PurchaseOrders p,
            List<Attachment> alst)
        {
            var pCnumber = new Regex(@"[C\#]+").Replace(p.PreviousCNumber, "");


            LinkPDFs(new List<string> {pCnumber}, "DO02");
            var pdf = $"{pCnumber}.pdf";
            List<Attachment> previousDocuments;

            previousDocuments = currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments
                .Where(x => x.Attachment.FilePath.Contains(pdf) && x.Attachment.DocumentCode == "DO02")
                .Select(x => x.Attachment).ToList();
            if (!previousDocuments.Any())
                using (var ctx = new DocumentDSContext())
                {
                    previousDocuments = ctx.Attachments
                        .Where(x => x.FilePath.Contains(pdf) &&
                                    x.DocumentCode == "NA")
                        .ToList();
                    foreach (var itm in previousDocuments.ToList())
                    {
                        previousDocuments.Remove(itm);

                        var att = new Attachment
                        {
                            TrackingState = TrackingState.Added,
                            FilePath = itm.FilePath,
                            Reference = pCnumber,
                            DocumentCode = "DO02"
                        };
                        ctx.Attachments.Add(att);

                        ctx.SaveChanges();
                        cdoc.Document.AsycudaDocument_Attachments.Add(
                            new AsycudaDocument_Attachments(true)
                            {
                                AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                                Attachment = att,

                                TrackingState = TrackingState.Added
                            });

                        currentAsycudaDocumentSet.AsycudaDocumentSet_Attachments.Add(
                            new AsycudaDocumentSet_Attachments(true)
                            {
                                AsycudaDocumentSetId = currentAsycudaDocumentSet.AsycudaDocumentSetId,
                                Attachment = att,

                                TrackingState = TrackingState.Added
                            });

                        previousDocuments.Add(att);
                        ctx.SaveChanges();
                    }
                }

            alst.AddRange(previousDocuments);
        }

        public static void LinkPDFs(List<string> cNumbers, string docCode = "NA")
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = new List<int>();
                    foreach (var entryId in cNumbers)
                        res.Add(ctx.AsycudaDocuments.Where(x => x.CNumber == entryId).OrderByDescending(x => x.ASYCUDA_Id)
                            .Select(x => x.ASYCUDA_Id).FirstOrDefault());
                    LinkPDFs(res.Where(x => x != 0).ToList(), docCode);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void LinkPDFs(List<int> entries, string docCode = "NA")
        {
            Console.WriteLine("Link PDF Files");
            var directoryName = StringExtensions.UpdateToCurrentUser(
                    Path.Combine(Instance.CurrentApplicationSettings.DataFolder,
                        "Imports")) //doc.z.Declarant_Reference_Number));
                ;

            using (var ctx = new CoreEntitiesContext())
            {
                foreach (var entryId in entries)
                {
                    var doc = ctx.AsycudaDocuments.FirstOrDefault(x => x.ASYCUDA_Id == entryId);
                    if (doc == null) continue;
                    var fileInfos = new DirectoryInfo(directoryName).GetFiles($"*-{doc.CNumber}.pdf").ToList();
                    fileInfos.AddRange(new DirectoryInfo(directoryName).GetFiles($"*-{doc.CNumber}-*.pdf").ToList());
                    var csvFiles = fileInfos
                        .Where(x => Regex.IsMatch(x.FullName,
                            @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",
                            RegexOptions.IgnoreCase)).ToArray();
                    foreach (var file in csvFiles)
                    {
                        var dfile = ctx.Attachments.Include(x => x.AsycudaDocument_Attachments)
                            .FirstOrDefault(x => x.FilePath == file.FullName);

                        if (dfile != null &&
                            dfile.AsycudaDocument_Attachments.Any(x => x.AsycudaDocumentId == doc.ASYCUDA_Id)) continue;
                        var mat = Regex.Match(file.FullName,
                            @".*(?<=\\)([A-Z,0-9]{3}\-[A-Z]{5}\-)(?<pCNumber>\d+).*.pdf",
                            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                        if (!mat.Success) continue;

                        var attachment = new Attachments(true)
                        {
                            FilePath = file.FullName,
                            DocumentCode = docCode,
                            Reference = file.Name.Replace(file.Extension, ""),
                            TrackingState = TrackingState.Added
                        };
                        ctx.AsycudaDocument_Attachments.Add(
                            new CoreEntities.Business.Entities.AsycudaDocument_Attachments(true)
                            {
                                AsycudaDocumentId = entryId,
                                Attachments = attachment,

                                TrackingState = TrackingState.Added
                            });

                        ctx.AsycudaDocumentSet_Attachments.Add(
                            new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                            {
                                AsycudaDocumentSetId = doc.AsycudaDocumentSetId.GetValueOrDefault(),
                                Attachments = attachment,

                                TrackingState = TrackingState.Added
                            });

                        ctx.SaveChanges();
                    }
                }
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
                            x.xcuda_Tarification.National_customs_procedure == c.National_customs_procedure).ToList())
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

        private static void AttachToDocument(IEnumerable<int> alst, int docId, int itmId)
        {
            try
            {
                xcuda_ASYCUDA doc;
                List<Attachment> attlst;

                List<xcuda_Item> itms;
                using (var docCtx = new DocumentDSContext())
                {
                    doc = docCtx.xcuda_ASYCUDA.First(x => x.ASYCUDA_Id == docId);

                    attlst = docCtx.Attachments.Where(x => alst.Contains(x.Id)).ToList();
                }

                using (var docItemCtx = new DocumentItemDSContext())
                {
                    itms = docItemCtx.xcuda_Item.Include("xcuda_Attached_documents.xcuda_Attachments")
                        .Where(x => x.Item_Id >= itmId).ToList();
                }

                AttachToDocument(attlst, doc, itms);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //public static void AttachToDocument(List<string> attachments, xcuda_ASYCUDA doc, List<xcuda_Item> itms)
        //{

        //    var alst = new List<Attachment>();
        //    using(var ctx = new CoreEntitiesContext())
        //    foreach (var astr in attachments)
        //    {
        //        var att = ctx.Attachments.FirstOrDefault(x => x.FilePath)
        //    }
        //}

        public static void AttachToDocument(List<Attachment> alst, xcuda_ASYCUDA doc, List<xcuda_Item> itms)
        {
            try
            {
                foreach (var att in alst.OrderBy(x => x.Id))

                {
                    if (att.Reference == "Info") continue;
                    if (doc.AsycudaDocument_Attachments.FirstOrDefault(x =>
                            x.AsycudaDocumentId == doc.ASYCUDA_Id && x.AttachmentId == att.Id) == null)
                        doc.AsycudaDocument_Attachments.Add(new AsycudaDocument_Attachments(true)
                        {
                            AttachmentId = att.Id,
                            AsycudaDocumentId = doc.ASYCUDA_Id,
                            TrackingState = TrackingState.Added,
                            Attachment = att
                        });


                    foreach (var itm in itms)
                    {
                        if (itm.xcuda_Attached_documents.Any(x => x.Attached_document_reference == att.Reference))
                            break;
                        itm.xcuda_Attached_documents.Add(new xcuda_Attached_documents(true)
                        {
                            Attached_document_code = att.DocumentCode,
                            Attached_document_date = DateTime.Today.Date.ToShortDateString(),
                            Attached_document_reference = att.Reference,
                            xcuda_Attachments = new List<xcuda_Attachments>
                            {
                                new xcuda_Attachments(true)
                                {
                                    AttachmentId = att.Id,
                                    TrackingState = TrackingState.Added,
                                    Attachments = new global::DocumentItemDS.Business.Entities.Attachments()
                                    {
                                        TrackingState = (att.Id == 0 ? TrackingState.Added : att.TrackingState),
                                        Id = att.Id,
                                        EmailId = att.EmailId,
                                        FilePath = att.FilePath,
                                        Reference = att.Reference,
                                        DocumentCode = att.DocumentCode

                                    }
                                }
                            },
                            TrackingState = TrackingState.Added
                        });
                        break;
                    }
                }

                if (doc.ASYCUDA_Id == 0) return;

                foreach (var itm in itms)
                    using (var docItemCtx = new DocumentItemDSContext {StartTracking = true})
                    {
                        foreach (var ad in itm.xcuda_Attached_documents)
                        {
                            if (docItemCtx.xcuda_Attached_documents.FirstOrDefault(x =>
                                    x.Item_Id == itm.Item_Id &&
                                    x.Attached_document_reference == ad.Attached_document_reference) !=
                                null) continue;
                            ad.Item_Id = itm.Item_Id;
                            docItemCtx.xcuda_Attached_documents.Add(ad);
                        }

                        docItemCtx.SaveChanges();
                    }

                using (var ctx = new DocumentDSContext {StartTracking = true})
                {
                    foreach (var at in doc.AsycudaDocument_Attachments)
                    {
                        if (ctx.AsycudaDocument_Attachments.FirstOrDefault(x =>
                                x.AsycudaDocumentId == doc.ASYCUDA_Id && x.AttachmentId == at.AttachmentId) !=
                            null) continue;
                        at.AsycudaDocumentId = doc.ASYCUDA_Id;
                        ctx.AsycudaDocument_Attachments.Add(at);
                    }

                    ctx.SaveChanges();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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


        private IEnumerable<EntryLineData> CreateSingleEntryLineData(
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


        public IEnumerable<EntryLineData> CreateGroupEntryLineData(
            IEnumerable<EntryDataDetails> slstSource)
        {
            var slst = from s in slstSource.AsEnumerable()
                group s by new {s.ItemNumber, s.ItemDescription, s.TariffCode, s.Cost, s.EntryData, s.InventoryItemEx}
                into g
                select new BaseDataModel.EntryLineData
                {
                    ItemNumber = g.Key.ItemNumber.Trim(),
                    ItemDescription = g.Key.ItemDescription.Trim(),
                    TariffCode = g.Key.TariffCode,
                    Cost = g.Key.Cost,
                    Quantity = g.Sum(x => x.Quantity),
                    EntryDataDetails = g.Select(x => new EntryDataDetailSummary
                    {
                        EntryDataDetailsId = x.EntryDataDetailsId,
                        EntryData_Id = x.EntryData_Id,
                        EntryDataId = x.EntryDataId,
                        EffectiveDate = x.EffectiveDate.GetValueOrDefault(),
                        EntryDataDate = x.EntryData.EntryDataDate,
                        QtyAllocated = x.QtyAllocated,
                        Currency = x.EntryData.Currency,
                        LineNumber = x.LineNumber
                    }).ToList(),
                    EntryData = g.Key.EntryData,

                    Freight = Convert.ToDouble(g.Sum(x => x.Freight)),
                    Weight = Convert.ToDouble(g.Sum(x => x.Weight)),
                    InternalFreight = Convert.ToDouble(g.Sum(x => x.InternalFreight)),
                    TariffSupUnitLkps = g.Key.InventoryItemEx.SuppUnitCode2 != null
                        ? new List<ITariffSupUnitLkp>
                        {
                            new TariffSupUnitLkps
                            {
                                SuppUnitCode2 = g.Key.InventoryItemEx.SuppUnitCode2,
                                SuppQty = g.Key.InventoryItemEx.SuppQty.GetValueOrDefault()
                            }
                        }
                        : null,
                    InventoryItemEx = g.Key.InventoryItemEx
                };
            return slst;
        }

        public async Task<xcuda_ASYCUDA> GetDocument(int ASYCUDA_Id, List<string> includeLst = null)
        {
            using (var ctx = new xcuda_ASYCUDAService {StartTracking = true})
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

        public async Task<AsycudaDocumentItem> Getxcuda_Item(int p)
        {
            using (var ctx = new AsycudaDocumentItemService())
            {
                return await ctx.GetAsycudaDocumentItemByKey(p.ToString()).ConfigureAwait(false);
            }
        }

        public async Task SaveDocumentCT(DocumentCT cdoc)
        {
            if (cdoc == null) return;

            using (var ctx = new xcuda_ASYCUDAService())
            {
                cdoc.Document = await ctx.CleanAndUpdateXcuda_ASYCUDA(cdoc.Document).ConfigureAwait(false);
            }


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
                            ctx.Updatexcuda_Item(t.ChangeTracker.FirstOrDefault()).Wait(); //.ChangeTracker.GetChanges().FirstOrDefault()
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
        }

        public async Task CalculateDocumentSetFreight(int asycudaDocumentSetId)
        {
            var currency = "";
            double totalfob = 0;
            double totalItemQuantity = 0;
            double totalFreight = 0;
            double totalWeight = 0;
            List<int> doclst = null;
            var CIFValues = new Dictionary<int, double>();
            var ItemQuantities = new Dictionary<int, double>();
            AsycudaDocumentSet asycudaDocumentSet;
            using (var ctx = new DocumentDSContext())
            {
                asycudaDocumentSet =
                    ctx.AsycudaDocumentSets.Include(x => x.xcuda_ASYCUDA_ExtendedProperties)
                        .FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (asycudaDocumentSet != null)
                {
                    if (asycudaDocumentSet.TotalFreight != null)
                        totalFreight = asycudaDocumentSet.TotalFreight.Value;
                    if (asycudaDocumentSet.TotalWeight != null) totalWeight = asycudaDocumentSet.TotalWeight.Value;
                    if (totalWeight <= 0 && asycudaDocumentSet.Documents.Count() > 0)
                        throw new ApplicationException(
                            $"DocSet:{asycudaDocumentSet.Declarant_Reference_Number} Weight is Zero");

                    currency = asycudaDocumentSet.FreightCurrencyCode ?? asycudaDocumentSet.Currency_Code;
                }

                doclst =
                    ctx.xcuda_ASYCUDA_ExtendedProperties.Where(x => x.AsycudaDocumentSetId == asycudaDocumentSetId)
                        .Where(x => x.IsManuallyAssessed != true &&
                                    x.ImportComplete ==
                                    false) // prevent recalculating weights of assessed entries
                        .Select(x => x.ASYCUDA_Id)
                        .ToList();
                if (!doclst.Any()) return;
            }

            using (var ctx = new CoreEntitiesContext())
            {
                foreach (var doc in doclst)
                {
                    var t = ctx.AsycudaDocuments.Where(x => x.ASYCUDA_Id == doc)
                        .Select(y =>
                            y.TotalCIF + y.TotalInternalFreight + y.TotalInsurance + y.TotalOtherCost -
                            y.TotalDeduction).DefaultIfEmpty(0).Sum();
                    var f = ctx.AsycudaDocuments.Where(x => x.ASYCUDA_Id == doc)
                        .Select(y => y.TotalFreight).DefaultIfEmpty(0)
                        .Sum(); // should be zero if new existing has value take away existing value
                    var totalItems = ctx.AsycudaItemBasicInfo.Where(x => x.ASYCUDA_Id == doc)
                        .Select(x => x.ItemQuantity).DefaultIfEmpty(0).Sum(); //* 0.01
                    ////////// added total items to prevent over weight due to minimum 0.01 requirement
                    var val = t.GetValueOrDefault() - f.GetValueOrDefault(); // + ; 
                    CIFValues.Add(doc, val);
                    ItemQuantities.Add(doc, totalItems.GetValueOrDefault());
                    totalfob += val;
                    totalItemQuantity += totalItems.GetValueOrDefault();
                }
            }

            totalWeight -= WeightAsycudaNormallyOffBy;

            var freightRate = totalFreight != 0 ? totalFreight / totalfob : 0;
            var weightRate = totalWeight != 0 ? totalWeight / totalItemQuantity : 0;
            double weightUsed = 0;

            using (var ctx = new DocumentDSContext {StartTracking = true})
            {
                var weightmsgSent = false;
                foreach (var doc in doclst)
                {
                    //calulate frieght based on value, calculate weight based on quantity to prevent the minimm weight per value issue
                    var cif = CIFValues.FirstOrDefault(x => x.Value > 0 && x.Key == doc);
                    var totalItems = ItemQuantities.FirstOrDefault(x => x.Value > 0 && x.Key == doc);
                    // refactor this in to a sub
                    if (weightUsed > totalWeight && !weightmsgSent)
                    {
                        //throw new ApplicationException("Weight Used Exceed Total Weight!");
                        if(!string.IsNullOrEmpty(BaseDataModel.GetClient().Email))
                            EmailDownloader.EmailDownloader.SendEmail(BaseDataModel.GetClient(), null, $"Bug Found",
                            new[] { "Joseph@auto-brokerage.com" }, $"Weight Used Exceed Total Weight! - DocSet:{asycudaDocumentSet?.Declarant_Reference_Number} TotalWeight:{totalWeight}",
                            Array.Empty<string>());
                        weightmsgSent = true;
                    }
                        

                    if (asycudaDocumentSet.ApportionMethod == "Equal")
                    {
                        if (cif.Value != 0) UpdateFreight(ctx, cif, totalFreight / doclst.Count(), currency);
                        if (totalItems.Value != 0)
                            weightUsed += UpdateWeight(ctx, totalItems, totalWeight / doclst.Count());
                    }
                    else
                    {
                        if (cif.Value != 0) UpdateFreight(ctx, cif, cif.Value * freightRate, currency);
                        if (totalItems.Value != 0)
                            weightUsed += UpdateWeight(ctx, totalItems, totalItems.Value * weightRate);
                    }
                }

                ctx.SaveChanges();
            }
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

        private static double UpdateWeight(DocumentDSContext ctx, KeyValuePair<int, double> doc, double weightRate)
        {
            try
            {
                if (Instance.CurrentApplicationSettings.WeightCalculationMethod.ToLower() !=
                    "Value".ToLower()) return 0;
                var val = ctx.xcuda_Valuation.Include(x => x.xcuda_Weight).FirstOrDefault(x => x.ASYCUDA_Id == doc.Key);
                if (val == null) return 0;
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

                xcuda_Weight.Gross_weight = weightRate < _minimumPossibleAsycudaWeight ? _minimumPossibleAsycudaWeight : weightRate;

                double weightUsed = 0;
                using (var ictx = new DocumentItemDSContext {StartTracking = true})
                {
                    var lst = ictx.xcuda_Weight_itm
                        .Include(x => x.xcuda_Valuation_item)
                        .Where(x => x.xcuda_Valuation_item.xcuda_Item.ASYCUDA_Id == doc.Key).ToList();


                    foreach (var itm in lst)
                    {
                        var itmQuantity = ictx.xcuda_Tarification.Include(x => x.Unordered_xcuda_Supplementary_unit)
                            .First(z => z.Item_Id == itm.Valuation_item_Id)
                            .Unordered_xcuda_Supplementary_unit.First(x => x.IsFirstRow == true)
                            .Suppplementary_unit_quantity.GetValueOrDefault();

                        var calWgt = weightRate * (itmQuantity / doc.Value);
                        var minWgt = itmQuantity * _minimumPossibleAsycudaWeight;

                        if (calWgt - _runningMiniumWeight < minWgt)
                        {
                            itm.Gross_weight_itm = minWgt;
                            _runningMiniumWeight += minWgt;
                        }
                        else
                        {
                            itm.Gross_weight_itm = calWgt - _runningMiniumWeight;
                            _runningMiniumWeight = 0;
                        }

                       // itm.Gross_weight_itm -= (itm == lst.First() ? _weightAsycudaNormallyOffBy : 0);

                        itm.Net_weight_itm =
                            itm.Gross_weight_itm;
                        weightUsed += itm.Gross_weight_itm;
                    }

                    ictx.SaveChanges();
                }

                return weightUsed;
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

        internal xcuda_Item CreateItemFromEntryDataDetail(BaseDataModel.IEntryLineData pod, DocumentCT cdoc)
        {
            var itm = CreateNewDocumentItem();
            cdoc.DocumentItems.Add(itm);


            itm.xcuda_Goods_description.Commercial_Description = CleanText(pod.ItemDescription).Truncate(159);
            if (cdoc.Document.xcuda_General_information != null)
                itm.xcuda_Goods_description.Country_of_origin_code = cdoc.Document.xcuda_General_information
                    .xcuda_Country.Country_first_destination;
            itm.xcuda_Tarification.Item_price = Convert.ToSingle(pod.Cost * pod.Quantity);
            itm.xcuda_Tarification.National_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                .Customs_Procedure
                .National_customs_procedure;
            itm.xcuda_Tarification.Extended_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
                .Customs_Procedure
                .Extended_customs_procedure;

            itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pod.TariffCode.Trim() ?? "NULL";
            itm.xcuda_Tarification.xcuda_HScode.Precision_4 =
                pod.ItemNumber;

            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != null)
                itm.xcuda_Previous_doc.Summary_declaration =
                    cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber;

            itm.xcuda_Valuation_item.Total_CIF_itm =
                Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));
            itm.xcuda_Valuation_item.Statistical_value =
                Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));


            var ivc = new xcuda_Item_Invoice(true)
            {
                TrackingState = TrackingState.Added,
                Amount_national_currency = Convert.ToSingle(Math.Round(
                    Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost) *
                    Convert.ToDecimal(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Exchange_Rate),
                    4)),
                Amount_foreign_currency = Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4)),
                xcuda_Valuation_item = itm.xcuda_Valuation_item
            };


            if (cdoc.Document.xcuda_Valuation != null && cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice != null)
            {
                //;
                ivc.Currency_code =
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice
                        .Currency_code;
                ivc.Currency_rate =
                    cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice
                        .Currency_rate;
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
                    if ((float) pod.Quantity > 99)
                    {
                        itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
                        {
                            TrackingState = TrackingState.Added,
                            Gross_weight_itm = (float) pod.Quantity *
                                               Convert.ToSingle(.1),
                            Net_weight_itm = (float) pod.Quantity *
                                             Convert.ToSingle(.1),
                            xcuda_Valuation_item = itm.xcuda_Valuation_item
                        };
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
                case "MinimumWeight":
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


            if (pod.InternalFreight != 0)
                itm.xcuda_Valuation_item.xcuda_item_internal_freight.Amount_foreign_currency =
                    Convert.ToSingle(pod.InternalFreight);


            if (pod.Freight != 0)
                itm.xcuda_Valuation_item.xcuda_item_external_freight.Amount_foreign_currency =
                    Convert.ToSingle(pod.Freight);
            // set on each line in event of grouped invoices per IM7 etc.
            if (pod.EntryDataDetails.Count() == 1) 
            {
                var fr = pod.EntryDataDetails.FirstOrDefault();
                if (fr != null)
                {
                    if (fr.Comment == null)
                    {
                        itm.Free_text_1 = $"{fr.EntryDataId}|{fr.LineNumber}";
                        itm.Free_text_2 = pod.ItemNumber;
                    }
                    else
                    {
                        itm.Free_text_1 = $"{fr.EntryDataId}|{fr.LineNumber}|{pod.ItemNumber}";
                        itm.Free_text_2 = fr.Comment;
                    }

                    itm.PreviousInvoiceItemNumber = pod.ItemNumber;
                    itm.PreviousInvoiceLineNumber = fr.LineNumber.ToString();
                    itm.PreviousInvoiceNumber = fr.EntryDataId;


                    LimitFreeText(itm);
                }
            }

            foreach (var ed in pod.EntryDataDetails)
            {
                if (itm.EntryDataDetails.All(x => x.EntryDataDetailsId != ed.EntryDataDetailsId))
                    itm.EntryDataDetails.Add(new xcuda_ItemEntryDataDetails(true)
                    {
                        Item_Id = itm.Item_Id,
                        EntryDataDetailsId = ed.EntryDataDetailsId,
                        TrackingState = TrackingState.Added,
                        xcuda_Item = itm
                    });

                if (cdoc.Document.AsycudaDocumentEntryDatas.All(x => x.EntryData_Id != ed.EntryData_Id))
                    cdoc.Document.AsycudaDocumentEntryDatas.Add(new AsycudaDocumentEntryData(true)
                    {
                        AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                        EntryData_Id = ed.EntryData_Id,
                        TrackingState = TrackingState.Added,
                        xcuda_ASYCUDA = cdoc.Document
                    });

                cdoc.EntryDataDetails.Add(new EntryDataDetails
                {
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    EntryDataId = ed.EntryDataId,
                    EntryData_Id = ed.EntryData_Id,
                    EffectiveDate = ed.EffectiveDate == DateTime.MinValue ? ed.EntryDataDate : ed.EffectiveDate,

                    
                });
            }


            itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit(true)
            {
                Tarification_Id = itm.xcuda_Tarification.Item_Id,
                Suppplementary_unit_code = "NMB",
                Suppplementary_unit_quantity = pod.Quantity,
                IsFirstRow = true,
                TrackingState = TrackingState.Added,
                xcuda_Tarification = itm.xcuda_Tarification
            });

            ProcessItemTariff(pod, cdoc.Document, itm);

            return itm;

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
                Gross_weight_itm = (float) pod.Quantity *
                                   Convert.ToSingle(_minimumPossibleAsycudaWeight),
                Net_weight_itm = (float) pod.Quantity *
                                 Convert.ToSingle(_minimumPossibleAsycudaWeight),
                xcuda_Valuation_item = itm.xcuda_Valuation_item

            };
        }

        public string CleanText(string p)
        {
            return p?.Replace(",", "");
        }

        private xcuda_Item CreateNewDocumentItem()
        {
            var item = new xcuda_Item(true) {TrackingState = TrackingState.Added};
            item.SetupProperties();
            return item; //
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

        public class MyPodData
        {
            public List<AsycudaSalesAllocations> Allocations { get; set; }
            public CreateEx9Class.AlloEntryLineData EntlnData { get; set; }
            public List<string> AllNames { get; set; }
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


        public async Task<AsycudaDocumentSet> CreateAsycudaDocumentSet(int applicationSettingsId)
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                var doc = await ctx
                    .CreateAsycudaDocumentSet(new AsycudaDocumentSet
                    {
                        ApplicationSettingsId = applicationSettingsId,
                        Currency_Code = "USD",
                        FreightCurrencyCode = "USD"
                    })
                    .ConfigureAwait(false);
                return doc;
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

        public async Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames,
            bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages, bool overwriteExisting,
            bool linkPi)
        {
            using (var ctx = new DocumentDSContext())
            {
                var docSet =
                    ctx.AsycudaDocumentSets.FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (docSet == null) throw new ApplicationException("Document Set with reference not found");
                await Task.Run(() =>
                        ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                            overwriteExisting, linkPi, fileNames))
                    .ConfigureAwait(false);
            }
        }


        public async Task ImportDocuments(AsycudaDocumentSet docSet, IEnumerable<string> fileNames,
            bool importOnlyRegisteredDocument, bool importTariffCodes, bool noMessages, bool overwriteExisting,
            bool linkPi)
        {
            await Task.Run(() =>
                    ImportDocuments(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                        overwriteExisting, linkPi, fileNames))
                .ConfigureAwait(false);
        }

        public void ImportC71(int asycudaDocumentSetId, List<string> files)
        {
            var docSet = Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).Result;
            var exceptions = new ConcurrentQueue<Exception>();
            foreach (var file in files)
                try
                {
                    if (Value_declaration_form.CanLoadFromFile(file))
                        LoadC71(docSet, file, ref exceptions);
                    else
                        throw new ApplicationException($"Can not Load file '{file}'");


                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }

            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public void ImportLicense(int asycudaDocumentSetId, List<string> files)
        {
            var docSet = Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).Result;
            var exceptions = new ConcurrentQueue<Exception>();
            foreach (var file in files)
                try
                {
                    if (Licence.CanLoadFromFile(file))
                        LoadLicence(docSet, file, ref exceptions);
                    else
                        throw new ApplicationException($"Can not Load file '{file}'");
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
        }

        private void LoadLicence(AsycudaDocumentSet docSet, string file, ref ConcurrentQueue<Exception> exceptions)
        {
            try
            {
                var a = Licence.LoadFromFile(file);
                var fileinfo = new FileInfo(file);
                if (a.General_segment.Exporter_address.Text.Any(x => x.Contains(fileinfo.Name.Replace("-LIC.xml", "")))
                ) return;
                using (var ctx = new CoreEntitiesContext())
                {
                    var declarants = ctx.ApplicationSettings
                        .Include(x => x.Declarants)
                        .First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).Declarants;
                    var fileCode = a.General_segment.Importer_code.Text.FirstOrDefault();
                    if (fileCode == null) return;
                    if (!declarants.Any(x => fileCode.Contains(x.DeclarantCode)))
                        throw new ApplicationException(
                            $"Could not import file - '{file} - The file is for another warehouse{fileCode}. While this Warehouse is {declarants.First().DeclarantCode}");
                }


                if (a != null)
                {
                    var importer = new LicenseToDataBase();
                    importer.SaveToDatabase(a, docSet, new FileInfo(file)).Wait();
                }

                Debug.WriteLine(file);
            }

            catch (Exception Ex)
            {
                exceptions.Enqueue(
                    new ApplicationException(
                        $"Could not import file - '{file}. Error:{(Ex.InnerException ?? Ex).Message} Stacktrace:{(Ex.InnerException ?? Ex).StackTrace}"));
            }
        }

        private void LoadC71(AsycudaDocumentSet docSet, string file, ref ConcurrentQueue<Exception> exceptions)
        {
            try
            {
                var a = Value_declaration_form.LoadFromFile(file);
                using (var ctx = new CoreEntitiesContext())
                {
                    var declarants = ctx.ApplicationSettings
                        .Include(x => x.Declarants)
                        .First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).Declarants;
                    var fileCode = a.Identification_segment.Declarant_segment.Code.Text.FirstOrDefault();
                    if (!declarants.Any(x => fileCode.Contains(x.DeclarantCode)))
                    {


                        BaseDataModel.EmailExceptionHandler(new ApplicationException(
                            $"Could not import file - '{file} - The file is for another warehouse{fileCode}. While this Warehouse is {declarants.First().DeclarantCode}"), true);
                        return;
                    }
                }


                if (a != null)
                {
                    var importer = new C71ToDataBase();
                    importer.SaveToDatabase(a, docSet, new FileInfo(file));
                }

                Debug.WriteLine(file);
            }

            catch (Exception Ex)
            {
                exceptions.Enqueue(
                    new ApplicationException(
                        $"Could not import file - '{file}. Error:{(Ex.InnerException ?? Ex).Message} Stacktrace:{(Ex.InnerException ?? Ex).StackTrace}"));
            }
        }

        private void ImportDocuments(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument,
            bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi, IEnumerable<string> fileNames)
        {
            var exceptions = new ConcurrentQueue<Exception>();
            //Parallel.ForEach(fileNames,
            //    new ParallelOptions
            //        {MaxDegreeOfParallelism = 1}, // Environment.ProcessorCount * // have to fix deadlock issue first
            //    f => //
            foreach (var f in fileNames)
           {
                    try
                    {
                        if (ASYCUDA.CanLoadFromFile(f))
                        {
                            LoadAsycuda421(docSet, importOnlyRegisteredDocument, importTariffCodes, noMessages,
                                overwriteExisting, linkPi, f, ref exceptions);
                        }
                        else if (Value_declaration_form.CanLoadFromFile(f))
                        {
                            LoadC71(docSet, f, ref exceptions);
                        }
                        else if (Licence.CanLoadFromFile(f))
                        {
                            LoadLicence(docSet, f, ref exceptions);
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
            // );

            if (exceptions.Count > 0)
                throw new ApplicationException(exceptions.FirstOrDefault().Message + "|" +
                                               exceptions.FirstOrDefault().StackTrace);
        }


        private void LoadAsycuda421(AsycudaDocumentSet docSet, bool importOnlyRegisteredDocument,
            bool importTariffCodes,
            bool noMessages, bool overwriteExisting, bool linkPi, string f, ref ConcurrentQueue<Exception> exceptions)
        {
            StatusModel.StatusUpdate();
            try
            {
                var a = ASYCUDA.LoadFromFile(f);
                using (var ctx = new CoreEntitiesContext())
                {
                    var declarants = ctx.ApplicationSettings
                        .Include(x => x.Declarants)
                        .First(x => x.ApplicationSettingsId == docSet.ApplicationSettingsId).Declarants;
                    var fileCode = a.Warehouse.Identification.Text.FirstOrDefault() ??
                                   a.Declarant.Declarant_code.Text.FirstOrDefault();
                    if (!declarants.Any(x => fileCode.Contains(x.DeclarantCode)))
                        //throw new ApplicationException(
                        //    $"Could not import file - '{f} - The file is for another warehouse{fileCode}. While this Warehouse is {declarants.First().DeclarantCode}");
                        return;
                }


                if (a != null)
                {
                    var importer = new AsycudaToDataBase421
                    {
                        UpdateItemsTariffCode = importTariffCodes,
                        ImportOnlyRegisteredDocuments = importOnlyRegisteredDocument,
                        OverwriteExisting = overwriteExisting,
                        NoMessages = noMessages,
                        LinkPi = linkPi
                    };
                    importer.SaveToDatabase(a, docSet, new FileInfo(f)).Wait();
                }

                Debug.WriteLine(f);
            }

            catch (Exception Ex)
            {
                if (!noMessages && (bool) !Ex?.InnerException?.Message.StartsWith("Please Import pCNumber"))
                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not import file - '{f}. Error:{(Ex.InnerException ?? Ex).Message} Stacktrace:{(Ex.InnerException ?? Ex).StackTrace}"));
            }
        }

        public async Task ExportDocument(string filename, xcuda_ASYCUDA doc)
        {
            Instance.ExporttoXML(filename, doc);
        }

        public void IM72Ex9Document(string filename)
        {
            var zeroitems = "";
            // create blank asycuda document
            ASYCUDA olddoc;
            if (ASYCUDA.CanLoadFromFile(filename))
                olddoc = ASYCUDA.LoadFromFile(filename);
            else if (ASYCUDA.CanLoadFromFile(filename))
                olddoc = ASYCUDA.LoadFromFile(filename);
            else
                throw new ApplicationException($"Can not Load file '{filename}'");


            var newdoc = ASYCUDA.LoadFromFile(filename);

            newdoc.Container = null;

            if (olddoc.Identification.Registration.Date == null)
                throw new ApplicationException("Document is not Assesed! Convert Assessed Documents only");


            newdoc.Item.Clear();

            var cp = GetCustomsProcedure("Duty Free", "IM9");

            var exp = Instance.ExportTemplates
                .Single(x =>x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&  x.Customs_Procedure == cp.CustomsProcedure);

            var linenumber = 0;
            foreach (var olditem in olddoc.Item)
            {
                linenumber += 1;


                // create new entry
                var i = olditem.Clone();

                i.Tarification.Extended_customs_procedure.Text.Clear();
                i.Tarification.Extended_customs_procedure.Text.Add(cp.Extended_customs_procedure);
                i.Tarification.National_customs_procedure.Text.Clear();
                i.Tarification.National_customs_procedure.Text.Add(cp.National_customs_procedure);


                i.Previous_doc.Summary_declaration.Text.Clear();
                i.Previous_doc.Summary_declaration.Text.Add(
                    $"{olddoc.Identification.Office_segment.Customs_clearance_office_code.Text[0]} {DateTime.Parse(olddoc.Identification.Registration.Date).Year} C {olddoc.Identification.Registration.Number} art. {linenumber}");


                // create previous item


                var pitm = new ASYCUDAPrev_decl
                {
                    Prev_decl_HS_code = new ASYCUDAPrev_declPrev_decl_HS_code(){Text = new ObservableCollection<string>(){i.Tarification.HScode.Commodity_code.Text.FirstOrDefault()}} ,
                    Prev_decl_HS_prec = new ASYCUDAPrev_declPrev_decl_HS_prec(){Text = new ObservableCollection<string>(){i.Tarification.HScode.Precision_1.Text.FirstOrDefault()}} ,
                    Prev_decl_current_item = linenumber.ToString(), // piggy back the previous item count
                    Prev_decl_item_number = linenumber.ToString(),
                    Prev_decl_weight = olditem.Valuation_item.Weight_itm.Net_weight_itm
                        .ToString(),
                    Prev_decl_weight_written_off = olditem.Valuation_item.Weight_itm.Net_weight_itm.ToString()
                };

                if (!string.IsNullOrEmpty(olditem.Packages.Number_of_packages))
                {
                    pitm.Prev_decl_number_packages_written_off =
                        Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();


                    pitm.Prev_decl_number_packages =
                        Math.Round(Convert.ToDouble(olditem.Packages.Number_of_packages), 0).ToString();
                }

                pitm.Prev_decl_supp_quantity = olditem.Tarification.Supplementary_unit[0]
                    .Suppplementary_unit_quantity.ToString();
                pitm.Prev_decl_supp_quantity_written_off = olditem.Tarification.Supplementary_unit[0]
                    .Suppplementary_unit_quantity.ToString();


                pitm.Prev_decl_country_origin = new ASYCUDAPrev_declPrev_decl_country_origin()
                {
                    Text = new ObservableCollection<string>()
                        { olditem.Goods_description.Country_of_origin_code.Text.FirstOrDefault() }
                };

                var oq = "";

                if (string.IsNullOrEmpty(olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity) ||
                    olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity == "0")
                {
                    oq = "1";
                    zeroitems = "ZeroItems";
                }
                else
                {
                    oq = olditem.Tarification.Supplementary_unit[0].Suppplementary_unit_quantity.ToString();
                }


                if (!string.IsNullOrEmpty(olditem.Valuation_item.Total_CIF_itm))
                {
                    pitm.Prev_decl_ref_value_written_off =
                        (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq)).ToString();
                    pitm.Prev_decl_ref_value =
                        (Convert.ToDecimal(olditem.Valuation_item.Total_CIF_itm) / Convert.ToDecimal(oq))
                        .ToString(); // * System.Convert.ToDecimal(fa.QUANTITY);
                }

                pitm.Prev_decl_reg_serial = new ASYCUDAPrev_declPrev_decl_reg_serial()
                    { Text = new ObservableCollection<string>() { "C" } };
                pitm.Prev_decl_reg_number = olddoc.Identification.Registration.Number;
                pitm.Prev_decl_reg_year = DateTime.Parse(olddoc.Identification.Registration.Date).Year.ToString();
                pitm.Prev_decl_office_code =
                    olddoc.Identification.Office_segment.Customs_clearance_office_code.Text[0];

                newdoc.Prev_decl.Add(pitm);

                i.Valuation_item.Item_Invoice.Currency_code.Text.Clear();
                i.Valuation_item.Item_Invoice.Currency_code.Text.Add(exp.Gs_Invoice_Currency_code); 
                i.Valuation_item.Item_Invoice.Amount_foreign_currency = olditem.Valuation_item.Total_CIF_itm;
                i.Valuation_item.Item_Invoice.Amount_national_currency = olditem.Valuation_item.Total_CIF_itm;
                i.Valuation_item.Statistical_value = olditem.Valuation_item.Total_CIF_itm;

                newdoc.Item.Add(i);
            }

            newdoc.Identification.Manifest_reference_number = null;
            newdoc.Identification.Type.Type_of_declaration = cp.Document_Type.Type_of_declaration;
            newdoc.Identification.Type.Declaration_gen_procedure_code =
                cp.Document_Type.Declaration_gen_procedure_code;
            newdoc.Declarant.Reference.Number.Text.Add("Ex9For" + newdoc.Identification.Registration.Number);

            newdoc.Valuation.Gs_Invoice.Currency_code.Text.Add(exp.Gs_Invoice_Currency_code);
            newdoc.Valuation.Gs_Invoice.Amount_foreign_currency = Math
                .Round(newdoc.Item.Where(i => !string.IsNullOrEmpty(i.Valuation_item.Total_CIF_itm)).Sum(i => Convert.ToDouble(i.Valuation_item.Total_CIF_itm)), 2).ToString();
            newdoc.Valuation.Gs_Invoice.Amount_national_currency = Math
                .Round(newdoc.Item.Where(i => !string.IsNullOrEmpty(i.Valuation_item.Total_CIF_itm)).Sum(i => Convert.ToDouble(i.Valuation_item.Total_CIF_itm)), 2).ToString();

            var oldfile = new FileInfo(filename);
            newdoc.SaveToFile(Path.Combine(oldfile.DirectoryName,
                olddoc.Identification.Registration.Number + "-Ex9" + zeroitems + oldfile.Extension));
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

        public async Task<AsycudaDocumentSet> GetDocSetWithEntryDataDocs(int AsycudaDocumentSetId)
        {
            var docset = await GetAsycudaDocumentSet(AsycudaDocumentSetId).ConfigureAwait(false);
            var docs = new List<xcuda_ASYCUDA>();
            foreach (var doc in docset.Documents)
                if (new DocumentDSContext().AsycudaDocumentEntryDatas.Any(x => x.AsycudaDocumentId == doc.ASYCUDA_Id))
                    docs.Add(doc);
                else
                    break;
            docset.xcuda_ASYCUDA_ExtendedProperties.ForEach(x =>
            {
                if (docs.FirstOrDefault(z => z.ASYCUDA_Id == x.ASYCUDA_Id) == null) x.xcuda_ASYCUDA = null;
            });
            return docset;
        }

        public async Task<AsycudaDocumentSet> GetAsycudaDocumentSet(int asycudaDocumentSetId)
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                return await ctx.GetAsycudaDocumentSetByKey(asycudaDocumentSetId.ToString(),
                    new List<string>
                    {
                        "xcuda_ASYCUDA_ExtendedProperties",
                        "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                        "xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_Declarant",
                        "AsycudaDocumentSet_Attachments.Attachment",
                        "AsycudaDocumentSet_Attachments.FileType",
                        "Customs_Procedure.Document_Type"
                    })
                    .ConfigureAwait(false);
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


        internal void ExportDocSet(AsycudaDocumentSet docSet, string directoryName, bool overWrite)
        {
            StatusModel.StartStatusUpdate("Exporting Files", docSet.Documents.Count());
            var exceptions = new ConcurrentQueue<Exception>();
            if (!Directory.Exists(directoryName)) return;
            if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
                File.Delete(Path.Combine(directoryName, "Instructions.txt"));
            if (File.Exists(Path.Combine(directoryName, "InstructionResults.txt")))
                File.Delete(Path.Combine(directoryName, "InstructionResults.txt"));

            foreach (var doc in docSet.Documents)
                //if (doc.xcuda_Item.Any() == true)
                //{
                try
                {
                    var fileInfo = new FileInfo(Path.Combine(directoryName, doc.ReferenceNumber + ".xml"));
                    if (overWrite || !File.Exists(fileInfo.FullName))
                        Instance.DocToXML(
                            Instance.CurrentApplicationSettings.DataFolder == null
                                ? fileInfo.DirectoryName
                                : Path.Combine(Instance.CurrentApplicationSettings.DataFolder,
                                    docSet.Declarant_Reference_Number), doc, fileInfo);


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

            if (exceptions.Count <= 0) return;
            var fault = new ValidationFault
            {
                Result = false,
                Message = exceptions.First().Message,
                Description = exceptions.First().StackTrace
            };
            throw new FaultException<ValidationFault>(fault, new FaultReason(fault.Message));
        }

        internal void ExportLastDocumentInDocSet(AsycudaDocumentSet docSet, string directoryName, bool overWrite)
        {
            StatusModel.StartStatusUpdate("Exporting Files", docSet.Documents.Count());
            var exceptions = new ConcurrentQueue<Exception>();
            if (!Directory.Exists(directoryName)) return;
            if (File.Exists(Path.Combine(directoryName, "Instructions.txt")))
                File.Delete(Path.Combine(directoryName, "Instructions.txt"));
            if (File.Exists(Path.Combine(directoryName, "InstructionResults.txt")))
                File.Delete(Path.Combine(directoryName, "InstructionResults.txt"));

            foreach (var doc in docSet.Documents.OrderByDescending(x => x.ASYCUDA_Id))

                try
                {
                    var fileInfo = new FileInfo(Path.Combine(directoryName, doc.ReferenceNumber + ".xml"));
                    if (overWrite || !File.Exists(fileInfo.FullName))
                        Instance.DocToXML(
                            Instance.CurrentApplicationSettings.DataFolder == null
                                ? fileInfo.DirectoryName
                                : Path.Combine(Instance.CurrentApplicationSettings.DataFolder,
                                    docSet.Declarant_Reference_Number), doc, fileInfo);


                    StatusModel.StatusUpdate();
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(
                        new ApplicationException(
                            $"Could not import file - '{doc.ReferenceNumber}. Error:{ex.Message} Stacktrace:{ex.StackTrace}"));
                }

            ////}

            if (exceptions.Count <= 0) return;
            var fault = new ValidationFault
            {
                Result = false,
                Message = exceptions.First().Message,
                Description = exceptions.First().StackTrace
            };
            throw new FaultException<ValidationFault>(fault, new FaultReason(fault.Message));
        }


        public async Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(List<int> lst, int asycudaDocumentSetId)
        {
            try
            {
                var res = new ConcurrentDictionary<int, EntryDataDetails>();
                if (lst.Any())
                    Parallel.ForEach(lst.Where(x => x != 0),
                        new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount}, item => //
                        {
                            using (var ctx = new EntryDataDSContext())
                            {
                                ctx.Database.CommandTimeout = 10;
                                var entryDataDetailses = ctx.EntryDataDetails
                                    .Include(x => x.EntryDataDetailsEx)
                                    .Include(x => x.InventoryItems)
                                    .Include(x => x.InventoryItemEx)
                                    .Where(x => x.EntryDataDetailsId == item
                                        //   && x.EntryData.EntryDataEx != null
                                    )
                                    .Where(x => Math.Abs((double) (x.EntryData.EntryDataEx.ExpectedTotal -
                                                                   (x.EntryData.InvoiceTotal == null ||
                                                                    x.EntryData.InvoiceTotal == 0
                                                                       ? x.EntryData.EntryDataEx.ExpectedTotal
                                                                       : x.EntryData.InvoiceTotal))) <
                                                0.01)
                                    .First();

                                // had to do all this stupidity because ef not loading the warehouse info because its derived...smh

                                entryDataDetailses.EntryData = ctx.EntryData.OfType<PurchaseOrders>()
                                    .Include(x => x.Suppliers)
                                    .Include(x => x.EntryDataEx)
                                    .Include(x => x.DocumentType)
                                    .Include(x => x.EntryDataTotals)
                                    .Include(x => x.WarehouseInfo)
                                    .First(x => x.EntryData_Id == entryDataDetailses.EntryData_Id);


                                res.AddOrUpdate(item, entryDataDetailses, (i, details) => details);
                            }
                        });


                return
                    res.Values.OrderBy(x =>
                        x.EntryDataDetailsId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<EntryDataDetails>> GetSelectedPODetails(List<string> elst,
            int asycudaDocumentSetId)
        {
            try
            {
                var res = new List<EntryDataDetails>();
                if (!elst.Any()) return res;
                {
                    using (var ctx = new EntryDataDSContext())
                    {
                        foreach (var item in elst.Where(x => x != null))
                        {
                            var entryDataDetailses = ctx.EntryDataDetails
                                .Include(x => x.EntryDataDetailsEx)
                                .Include(x => x.InventoryItems)
                                .Include(x => x.InventoryItemEx)
                                .Include(x => x.EntryData.EntryDataTotals)
                                .Include(x => x.EntryData.EntryDataEx)
                                .Include(x => x.EntryData.DocumentType)
                                .Include(x => x.EntryData.Suppliers)
                                .Where(x => x.EntryDataId == item
                                            && x.EntryDataDetailsEx.AsycudaDocumentSetId == asycudaDocumentSetId
                                            && x.EntryData.EntryDataEx != null
                                )
                                .ToList().DistinctBy(x => x.EntryDataDetailsId);

                            var res1 = entryDataDetailses.Where(x =>
                                Instance.CurrentApplicationSettings.AssessIM7 != true || Math.Abs(x.EntryData.EntryDataEx.ExpectedTotal -
                                    (x.EntryData.InvoiceTotal ?? x.EntryData.EntryDataEx
                                        .ExpectedTotal)) < 0.01);


                            res.AddRange(res1);
                        }
                    }
                }

                return res.OrderBy(x => x.EntryDataDetailsId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

        private async Task<xcuda_Item> GetDocumentItem(int item_Id, List<string> includeLst)
        {
            using (var ctx = new xcuda_ItemService {StartTracking = true})
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
                    await ctx.GetEntryDataDetailsByKey(entryDataDetailsEx.EntryDataDetailsId.ToString())
                        .ConfigureAwait(false);
                i.InjectFrom(entryDataDetailsEx);
                await ctx.UpdateEntryDataDetails(i).ConfigureAwait(false);
            }
        }

        public async Task SaveAsycudaDocument(AsycudaDocument asycudaDocument)
        {
            asycudaDocument.ModifiedProperties = null;
            if (asycudaDocument == null) return;
            //get the original item
            var i = await GetDocument(asycudaDocument.ASYCUDA_Id, new List<string>
            {
                "xcuda_ASYCUDA_ExtendedProperties"
            }).ConfigureAwait(false);
            i.StartTracking();
            //null for now cuz there are no navigation properties involved.
           i.InjectFrom(asycudaDocument);

            //var i = new xcuda_ASYCUDA()
            //{
            //    ASYCUDA_Id = asycudaDocument.ASYCUDA_Id,
            //    xcuda_ASYCUDA_ExtendedProperties = new xcuda_ASYCUDA_ExtendedProperties()
            //    {
            //        ASYCUDA_Id = asycudaDocument.ASYCUDA_Id,
            //        TrackingState = TrackingState.Unchanged
            //    },
            //    TrackingState = TrackingState.Unchanged
            //};

            i.xcuda_ASYCUDA_ExtendedProperties.StartTracking();
            if (i.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate !=
                asycudaDocument.EffectiveRegistrationDate)
                i.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate =
                    asycudaDocument.EffectiveRegistrationDate;
            if (i.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate != asycudaDocument.DoNotAllocate)
                i.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = asycudaDocument.DoNotAllocate;
            if (i.xcuda_ASYCUDA_ExtendedProperties.ModifiedProperties != null)
            {
               await Save_xcuda_ASYCUDA(i).ConfigureAwait(false);
            }
                
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

        public async Task LinkExistingPreviousItems(xcuda_ASYCUDA da, List<xcuda_Item> documentItems, bool update)
        {
            //get all previous items for this document
            try
            {
                IEnumerable<xcuda_PreviousItem> plst;

                plst = await DocumentItemDS.DataModels.BaseDataModel.Instance.Searchxcuda_PreviousItem(
                    new List<string>
                    {
                        $"Prev_reg_nbr == \"{da.xcuda_Identification.xcuda_Registration.Number}\""
                    }).ConfigureAwait(false);


                if (plst.Any() == false ) return; // || da.xcuda_Identification.xcuda_Type.DisplayName == "IM7"// im7s created from ex9 document can have previousitems... have to remove these
                foreach (var itm in documentItems)
                {
                    var pplst = plst.Where(x => x.Previous_item_number == itm.LineNumber.ToString() &&
                                                x.Prev_decl_HS_spec == itm.ItemNumber);

                    foreach (var p in pplst)
                    {
                        var ep = new global::DocumentItemDS.Business.Entities.EntryPreviousItems(true)
                        {
                            Item_Id = itm.Item_Id,
                            xcuda_Item = itm,
                            PreviousItem_Id = p.PreviousItem_Id,
                            xcuda_PreviousItem = p,
                            TrackingState = TrackingState.Added
                        };
                        itm.xcuda_PreviousItems.Add(ep);
                        if (!update) continue;
                        using (var ctx = new DocumentItemDSContext())
                        {
                            await ctx.Database.ExecuteSqlCommandAsync($@"INSERT INTO EntryPreviousItems
                                                                                (PreviousItem_Id, Item_Id)
                                                                                VALUES ({ep.PreviousItem_Id}, {ep.Item_Id})").ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public async Task SaveEntryPreviousItems(List<EntryPreviousItems> epi)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                ctx.ApplyChanges(epi);
                ctx.SaveChanges();
            }
        }


        public async Task DeleteAsycudaDocumentSet(int docSetId)
        {
            using (var ctx = new AsycudaDocumentSetService())
            {
                await ctx.DeleteAsycudaDocumentSet(docSetId.ToString()).ConfigureAwait(false);
            }
        }


        public async Task SaveAttachedDocuments(FileInfo[] csvFiles, FileTypes fileType)
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    foreach (var file in csvFiles)
                    {
                        var attachment =
                            ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).FirstOrDefault(x =>
                                x.Attachments.FilePath == file.FullName &&
                                x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);


                        using (var ctx1 = new CoreEntitiesContext {StartTracking = true})
                        {
                            var reference = GetReference(file, fileType);
                            if (reference == null) continue;
                            if (attachment == null)
                            {
                                if (string.IsNullOrEmpty(reference)) continue;
                                ctx1.AsycudaDocumentSet_Attachments.Add(
                                    new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                                    {
                                        AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                                        Attachments = new Attachments(true)
                                        {
                                            FilePath = file.FullName,
                                            DocumentCode = fileType.DocumentCode,
                                            Reference = reference,
                                            EmailId = fileType.EmailId
                                        },
                                        DocumentSpecific = fileType.DocumentSpecific,
                                        FileDate = file.LastWriteTime,

                                        FileTypeId = fileType.Id,
                                        TrackingState = TrackingState.Added
                                    });
                            }
                            else
                            {
                                attachment.DocumentSpecific = fileType.DocumentSpecific;
                                attachment.FileDate = file.LastWriteTime;
                                attachment.Attachments.Reference = reference;
                                attachment.Attachments.DocumentCode = fileType.DocumentCode;
                                attachment.FileTypeId = fileType.Id;
                            }

                            ctx1.SaveChanges();
                        }
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static void StripAttachments(List<DocumentCT> doclst, string emailId)
        {
            try
            {
                using (var ctx = new DocumentItemDSContext())
                {
                    foreach (var doc in doclst)
                    {
                        var res = ctx.xcuda_Attachments
                            .Include(x => x.Attachments)
                            .Include(x => x.xcuda_Attached_documents.xcuda_Attachments)
                            .Where(x => x.xcuda_Attached_documents != null)
                            .Where(x => x.xcuda_Attached_documents.xcuda_Item.ASYCUDA_Id == doc.Document.ASYCUDA_Id &&
                                        (x.Attachments.EmailId == null || x.Attachments.EmailId != emailId))
                            .ToList();
                        foreach (var itm in res.ToList())
                            ctx.xcuda_Attached_documents.Remove(itm.xcuda_Attached_documents);

                        ctx.SaveChanges();
                    }
                }

                using (var ctx = new DocumentDSContext())
                {
                    foreach (var doc in doclst)
                    {
                        var res = ctx.AsycudaDocument_Attachments
                            .Include(x => x.Attachment)
                            .Where(x => x.AsycudaDocumentId == doc.Document.ASYCUDA_Id &&
                                        (x.Attachment.EmailId == null || x.Attachment.EmailId != emailId)).ToList();
                        foreach (var itm in res.ToList())
                        {
                            ctx.AsycudaDocument_Attachments.Remove(itm);
                            itm.TrackingState = TrackingState.Deleted;
                        }

                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string GetReference(FileInfo file, FileTypes fileType)
        {
            switch (fileType.FileImporterInfos.EntryType)
            {
                case FileTypeManager.EntryTypes.C71:
                {
                    C71ToDataBase.GetRegNumber(file, out var regNumber);
                    return regNumber;
                }
                case FileTypeManager.EntryTypes.Lic:
                {
                    LicenseToDataBase.GetLicenceRegNumber(file, out var regNumber);
                    return regNumber;
                }
                default:
                    return file.Name.Replace(file.Extension, "");
            }
        }

        public void AttachToExistingDocuments(int asycudaDocumentSetId)
        {
            try
            {
                AttachC71(asycudaDocumentSetId);

                AttachPDF(asycudaDocumentSetId);
                AttachLicense(asycudaDocumentSetId);
                AttachContainer(asycudaDocumentSetId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void AttachContainer(int asycudaDocumentSetId)
        {
            List<xcuda_ASYCUDA> docs;
            AsycudaDocumentSet container;
            using (var ctx = new DocumentDSContext {StartTracking = true})
            {
                container = ctx.AsycudaDocumentSets.Include(x => x.Container)
                    .FirstOrDefault(x => x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (container?.Container == null) return;


                docs = ctx.xcuda_ASYCUDA
                    .Include(x => x.xcuda_Container)
                    .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .ToList();
            }

            var res = new List<xcuda_Container>();
            foreach (var xCon in docs)
            {
                var firstItem =
                    new CoreEntitiesContext().AsycudaDocumentItems
                        .First(x => x.AsycudaDocumentId == xCon.ASYCUDA_Id && x.LineNumber == "1");

                var c = xCon.xcuda_Container.FirstOrDefault();
                if (c == null)
                    c = new xcuda_Container(true)
                    {
                        ASYCUDA_Id = xCon.ASYCUDA_Id,
                        TrackingState = TrackingState.Added
                    };

                c.Container_identity = container.Container.Container_identity;
                c.Container_type = container.Container.Container_type;
                c.Empty_full_indicator = container.Container.Empty_full_indicator;
                c.Goods_description = firstItem.Commercial_Description;
                c.Packages_number = firstItem.Number_of_packages?.ToString(CultureInfo.InvariantCulture);
                c.Packages_type = "PK";
                c.Item_Number = firstItem.LineNumber;
                c.Packages_weight = firstItem.Gross_weight_itm;
                res.Add(c);
            }

            using (var ctx = new DocumentDSContext())
            {
                foreach (var itm in res)
                {
                    ctx.xcuda_Container.Add(itm);
                    ctx.SaveChanges();
                }
            }
        }


        private static void AttachPDF(int asycudaDocumentSetId)
        {
            List<xcuda_ASYCUDA> docs;
            List<xcuda_Item> itms;
            List<Attachment> pdfs;
            using (var ctx = new DocumentDSContext())
            {
                docs = ctx.xcuda_ASYCUDA.Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ==
                                                    asycudaDocumentSetId).ToList();
                pdfs = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachment).Where(x =>
                        x.Attachment.FilePath.ToLower().EndsWith("pdf") &&
                        x.FileType.DocumentSpecific != true &&
                        x.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .Select(x => x.Attachment).AsEnumerable().OrderByDescending(x => x.Id)
                    .Where(x => File.Exists(x.FilePath)).DistinctBy(x => new FileInfo(x.FilePath).Name).ToList();

                var nonpdfs = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachment).Where(x =>
                        !x.Attachment.FilePath.ToLower().EndsWith("pdf")
                        && !x.Attachment.FilePath.ToLower().Contains("xml")
                        && !x.Attachment.FilePath.ToLower().Contains("Info.txt".ToLower())
                        && x.FileType.DocumentSpecific != true
                        && x.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .Select(x => x.Attachment).AsEnumerable().OrderByDescending(x => x.Id)
                    .Where(x => File.Exists(x.FilePath)).DistinctBy(x => new FileInfo(x.FilePath).Name).ToList();

                pdfs.AddRange(nonpdfs);
            }

            using (var ctx = new DocumentItemDSContext())
            {
                var list = docs.Select(z => z.ASYCUDA_Id).ToList();
                itms = ctx.xcuda_Item.Where(x => list.Contains(x.ASYCUDA_Id)).ToList();
            }


            foreach (var doc in docs)
                AttachToDocument(pdfs, doc, itms.Where(x => x.ASYCUDA_Id == doc.ASYCUDA_Id).ToList());
        }


        public static void AttachEmailPDF(int asycudaDocumentSetId, string emailId)
        {
            try
            {
                if (emailId == null) return;
                var email = emailId;

                List<xcuda_ASYCUDA> docs;
                List<xcuda_Item> itms;
                List<Attachment> pdfs;
                using (var ctx = new DocumentDSContext())
                {
                    var pdfFileTypeInfo = new CoreEntitiesContext().FileImporterInfos.First(x =>
                        x.EntryType == FileTypeManager.EntryTypes.Inv && x.Format == FileTypeManager.FileFormats.PDF);
                    docs = ctx.xcuda_ASYCUDA
                        .Include(x => x.AsycudaDocument_Attachments)
                        .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ==
                                                        asycudaDocumentSetId).ToList();
                    pdfs = ctx.AsycudaDocumentSet_Attachments
                        .Include(x => x.Attachment)
                        .Where(x => x.Attachment.FilePath.EndsWith("pdf")
                                    && x.FileType.FileInfoId  != pdfFileTypeInfo.Id
                                    && x.AsycudaDocumentSetId == asycudaDocumentSetId
                                    && x.EmailId == email)
                        .Select(x => x.Attachment).AsEnumerable().DistinctBy(x => x.FilePath).ToList();

                    var nonpdfs = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachment).Where(x =>
                            !x.Attachment.FilePath.EndsWith("pdf") && x.FileType.FileInfoId != pdfFileTypeInfo.Id
                                                                   && x.AsycudaDocumentSetId == asycudaDocumentSetId
                                                                   && x.EmailId == email)
                        .Select(x => x.Attachment).AsEnumerable().DistinctBy(x => x.FilePath).ToList();

                    pdfs.AddRange(nonpdfs);
                }

                using (var ctx = new DocumentItemDSContext())
                {
                    var list = docs.Select(z => z.ASYCUDA_Id).ToList();
                    itms = ctx.xcuda_Item.Where(x => list.Contains(x.ASYCUDA_Id)).ToList();
                }


                foreach (var doc in docs)
                    AttachToDocument(pdfs, doc, itms.Where(x => x.ASYCUDA_Id == doc.ASYCUDA_Id).ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AttachLicense(int asycudaDocumentSetId)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext())
                {
                    var allocatedItms = new List<LicenceDocItem>();
                    var lst = ctx.AsycudaDocumentItems
                        .Include(x => x.AsycudaDocumentItemEntryDataDetails)
                        .Where(x => x.TariffCodeLicenseRequired == true
                                    && x.AsycudaDocument.AsycudaDocumentSetId ==
                                    asycudaDocumentSetId)
                        .Select(x => new LicenceDocItem
                        {
                            Item_Id = x.Item_Id,
                            Details = x.AsycudaDocumentItemEntryDataDetails.Select(z => z.key).ToList(),
                            TariffCode = x.TariffCode,
                            ItemQuantity = x.ItemQuantity,
                            AsycudaDocumentId = x.AsycudaDocumentId
                        })
                        .ToList();
                    if (!lst.Any()) return;
                    var docSet = Instance.GetAsycudaDocumentSet(asycudaDocumentSetId).Result;

                    ///// Scape any remaining license because i preapply and will still apply for new license when creating docset

                    var licAtt = new List<Attachments>();

                    //Add available license
                    var availableLic = ctx.TODO_LicenceAvailableQty.Where(x =>
                        x.ApplicationSettingsId == Instance.CurrentApplicationSettings.ApplicationSettingsId
                        && x.Origin == docSet.Country_of_origin_code).OrderBy(x => x.Application_date).ToList();
                    foreach (var lic in availableLic)
                    {
                        var attlst = ctx.Attachments.Where(x =>
                                x.DocumentCode == "LC02"
                                && x.FilePath == lic.SourceFile)
                            .DistinctBy(x => x.FilePath)
                            .Where(x => x.Reference != "LIC").ToList();
                        licAtt.AddRange(attlst);
                        foreach (var att in attlst)
                            ctx.AsycudaDocumentSet_Attachments.Add(
                                new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                                {
                                    AsycudaDocumentSetId = asycudaDocumentSetId,
                                    AttachmentId = att.Id
                                });
                        ctx.SaveChanges();
                    }


                    var res = new Dictionary<Attachments, Registered>();
                    foreach (var i in licAtt.DistinctBy(x => x.Reference))
                    {
                        var xLicLicense = new LicenseDSContext().xLIC_License.OfType<Registered>()
                            .Include("xLIC_Lic_item_segment.TODO_LicenceAvailableQty")
                            .Include(x => x.xLIC_General_segment)
                            .FirstOrDefault(x => x.SourceFile == i.FilePath);
                        if (xLicLicense != null) res.Add(i, xLicLicense);
                    }

                    foreach (var al in res)
                    {
                        EntryData entryDataId = null;


                        foreach (var lic in al.Value.xLIC_Lic_item_segment)
                        {
                            var truncate = lic.Commodity_code.Truncate(8);
                            var itms = entryDataId == null
                                ? lst.Where(x => x.TariffCode == truncate).ToList()
                                : lst.Where(x =>
                                        x.TariffCode == truncate &&
                                        x.Details.Any(z =>
                                            z.Contains(entryDataId.EntryDataId)))
                                    .ToList();

                            double rtotal = lic.TODO_LicenceAvailableQty == null
                                ? lic.Quantity_to_approve // assume all so it wont pass later 
                                : lic.Quantity_to_approve - lic.TODO_LicenceAvailableQty.Balance;


                            foreach (var itm in itms)
                            {
                                if (allocatedItms.Any(z => z.Item_Id == itm.Item_Id)) continue;
                                if (itm.ItemQuantity <= lic.Quantity_to_approve - rtotal &&
                                    itm.TariffCode == lic.Commodity_code.Truncate(8))
                                {
                                    rtotal += itm.ItemQuantity.GetValueOrDefault();
                                    AttachToDocument(new List<int> {al.Key.Id},
                                        itm.AsycudaDocumentId.GetValueOrDefault(), itm.Item_Id);
                                    allocatedItms.Add(itm);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AttachC71(int asycudaDocumentSetId)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.AsycudaItemBasicInfo
                    .Where(x => x.LineNumber == 1
                                && x.AsycudaDocumentSetId ==
                                asycudaDocumentSetId).ToList();
                var c71Att = ctx.AsycudaDocumentSet_Attachments.Include(x => x.Attachments).Where(x =>
                        x.FileTypes.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.C71 && x.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .Select(x => x.Attachments).AsEnumerable().DistinctBy(x => x.FilePath).Where(x =>
                        new FileInfo(x.FilePath).Name != $"{FileTypeManager.EntryTypes.C71}.xml" && x.Reference != FileTypeManager.EntryTypes.C71).ToList();

                if (!c71Att.Any())
                {
                    var eC71 = ctx.AsycudaDocumentSetC71.FirstOrDefault(x =>
                        x.AsycudaDocumentSetId == asycudaDocumentSetId);
                    if (eC71 != null)
                    {
                        var att = ctx.Attachments.FirstOrDefault(x => x.Id == eC71.AttachmentId);
                        if (att == null)
                        {
                            att = new Attachments
                            {
                                TrackingState = TrackingState.Added,
                                FilePath = eC71.FilePath,
                                DocumentCode = "DC05",
                                Reference = eC71.RegNumber
                            };
                            ctx.Attachments.Add(att);
                        }

                        c71Att = new List<Attachments> {att};

                        ctx.AsycudaDocumentSet_Attachments.Add(
                            new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                            {
                                AsycudaDocumentSetId = asycudaDocumentSetId,
                                AttachmentId = att.Id
                            });
                        ctx.SaveChanges();
                    }
                }


                var res = new Dictionary<Attachments, ValuationDS.Business.Entities.Registered>(); 
                var registeredC71s = new ValuationDSContext().xC71_Value_declaration_form
                        .OfType<ValuationDS.Business.Entities.Registered>()
                        .Include(x => x.xC71_Item)
                        .Include(x => x.xC71_Identification_segment).ToList();
                var asycudaDocumentItemEntryDataDetails = lst.GroupJoin(ctx.AsycudaDocumentItemEntryDataDetails,
                    x => x.Item_Id, e => e.Item_Id,
                    (x, e) => new {x.Item_Id, x.LineNumber, x.ASYCUDA_Id, data = e.Select(z => z.key)}).ToList();
                foreach (var i in c71Att)
                {
                   
                    var c71 = registeredC71s
                        .FirstOrDefault(x => x.SourceFile == i.FilePath);
                    if (c71 != null) res.Add(i, c71);
                }

                foreach (var al in res)
                {
                    foreach (var c71Item in al.Value.xC71_Item)
                    {
                       
                        var itms = asycudaDocumentItemEntryDataDetails
                            .Where(x =>
                                x.data.Any(z =>
                                    z.Contains(c71Item.Invoice_Number)) &&
                                x.LineNumber == 1).ToList();

                        foreach (var itm in itms)
                            AttachToDocument(new List<int> {al.Key.Id},
                                itm.ASYCUDA_Id, itm.Item_Id);
                    }
                }
            }
        }

        public static void AttachBlankC71(List<DocumentCT> docList)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var c71 = ctx.Attachments.First(x => x.DocumentCode == "DC05" && x.Reference == "NA");
                foreach (var doc in docList)
                    AttachToDocument(new List<int> {c71.Id},
                        doc.Document.ASYCUDA_Id, doc.DocumentItems.First().Item_Id);
            }
        }


        public static void SetInvoicePerline(List<int> docList)
        {
            try
            {
                using (var ctx = new DocumentItemDSContext {StartTracking = true})
                {
                    foreach (var doc in docList)
                    {
                        var mItms = ctx.xcuda_Item
                            .Include(x => x.xcuda_Attached_documents)
                            .Where(x => x.ASYCUDA_Id == doc &&
                                        x.xcuda_Attached_documents.Count(z => z.Attached_document_code == "IV05") > 1)
                            .SelectMany(x => x.xcuda_Attached_documents)
                            .Where(x => x.Attached_document_code == "IV05")
                            .ToList()
                            .GroupBy(x => x.Item_Id);

                        var sItms = ctx.xcuda_Item
                            .Include(x => x.xcuda_Attached_documents)
                            .Where(x => x.ASYCUDA_Id == doc
                            )
                            .ToList();


                        var i = 0;
                        foreach (var item in mItms)
                        foreach (var att in item)
                        {
                            if (i >= sItms.Count()) break;
                            var sitm = sItms[i];
                            att.Item_Id = sitm.Item_Id;

                            ctx.Database.ExecuteSqlCommand($@"update xcuda_Attached_documents
                                                                    set Item_Id = {sitm.Item_Id}
                                                                    where Attached_documents_Id = {
                                                                        att.Attached_documents_Id
                                                                    } ");
                            i += 1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void RenameDuplicateDocumentCodes(List<int> docList)
        {
            try
            {
                var codeLst = new List<string> {"IV05", "IV02", "IV03", "IV06", "IV07", "IV08"};
                using (var ctx = new DocumentItemDSContext {StartTracking = true})
                {
                    foreach (var doc in docList)
                    {
                        var itms = ctx.xcuda_Item
                            .Include(x => x.xcuda_Attached_documents)
                            .Where(x => x.ASYCUDA_Id == doc &&
                                        x.xcuda_Attached_documents.Count(z => z.Attached_document_code == "IV05") > 1)
                            .SelectMany(x => x.xcuda_Attached_documents)
                            .Where(x => x.Attached_document_code == "IV05")
                            .ToList()
                            .GroupBy(x => x.Item_Id);
                        foreach (var item in itms)
                        {
                            var i = 0;

                            foreach (var att in item)
                            {
                                if (i == codeLst.Count() - 1) break;
                                att.Attached_document_code = codeLst[i];
                                i += 1;
                            }
                        }


                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void RenameDuplicateDocuments(int docKey)
        {
            try
            {
                var (docSet, lst) = EntryDocSetUtils.GetDuplicateDocuments(docKey);
                if (!lst.Any() || !docSet.Documents.Any()) return;

                EntryDocSetUtils.RenameDuplicateDocuments(lst, ref  docSet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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


            double Weight { get; set; }

            double InternalFreight { get; set; }

            double Freight { get; set; }
            List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }
        }

        public class EntryLineData : BaseDataModel.IEntryLineData
        {
            private string _itemNumber;

            private int _previousDocumentItemId;
            public IDocumentItem PreviousDocumentItem { get; set; }
            public IInventoryItem InventoryItem { get; set; }
            public EntryData EntryData { get; set; }
            public InventoryItemsEx InventoryItemEx { get; set; }

            public string ItemNumber
            {
                get => _itemNumber;
                set
                {
                    _itemNumber = value;

                    using (var ctx = new InventoryItemService())
                    {
                        if (_itemNumber != null)
                            InventoryItem = ctx.GetInventoryItemsByExpression(
                                $"ItemNumber == \"{_itemNumber}\" && ApplicationSettingsId == {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}"
                                , new List<string>
                                {
                                    "TariffCodes.TariffCategory.TariffCategoryCodeSuppUnits.TariffSupUnitLkp"
                                }, false).Result.FirstOrDefault();
                        else
                            InventoryItem = null;
                    }
                }
            }

            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double Cost { get; set; }

            public int PreviousDocumentItemId
            {
                get => _previousDocumentItemId;
                set
                {
                    _previousDocumentItemId = value;

                    using (var ctx = new xcuda_ItemService())
                    {
                        if (_previousDocumentItemId != 0)
                            PreviousDocumentItem = ctx.Getxcuda_ItemByKey(_previousDocumentItemId.ToString()).Result;
                        else
                            PreviousDocumentItem = null;
                    }
                }
            }

            public double Quantity { get; set; }
            public List<EntryDataDetailSummary> EntryDataDetails { get; set; }

            public double Freight { get; set; }
            public List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }

            public double Weight { get; set; }

            public double InternalFreight { get; set; }
        }

        private static HashSet<string> emailedMessagesList = new HashSet<string>();

        public static void EmailExceptionHandler(Exception e, bool sendOnlyOnce = true )
        {
            var lastexception = false;
            var errorMessage = "Loading components";
            Exception exp = e;
            while (lastexception == false)
            {
                if (exp.InnerException == null)
                {
                    lastexception = true;


                    if (sendOnlyOnce == false || !emailedMessagesList.Contains(exp.Message))
                    {
                        EmailDownloader.EmailDownloader.SendEmail(BaseDataModel.GetClient(), null, $"Bug Found",
                            new[] {"Joseph@auto-brokerage.com"}, $"{exp.Message}\r\n{exp.StackTrace}",
                            Array.Empty<string>());
                        emailedMessagesList.Add(exp.Message);
                    }

                }

                errorMessage += $"An unhandled Exception occurred!: {exp.Message}"; //---- {1}
                exp = exp.InnerException;
            }
        }
    }

    internal class LicenceDocItem
    {
        public int Item_Id { get; set; }
        public List<string> Details { get; set; }
        public string TariffCode { get; set; }
        public double? ItemQuantity { get; set; }
        public int? AsycudaDocumentId { get; set; }
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
        public int EntryData_Id { get; set; }
        public string EntryDataId { get; set; }
        public int EntryDataDetailsId { get; set; }
        public DateTime EntryDataDate { get; set; }

        public double QtyAllocated { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Currency { get; set; }
        public int? LineNumber { get; set; }
        public string Comment { get; set; }
        public int AllocationId { get; set; }
        public int InventoryItemId { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public double Cost { get; set; }
        public double Quantity { get; set; }
        public string SourceFile { get; set; }
    }
}