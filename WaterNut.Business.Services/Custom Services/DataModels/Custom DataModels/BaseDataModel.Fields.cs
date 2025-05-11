using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.Data;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using DocumentDS.Business.Services;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Services;
using WaterNut.Interfaces;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using EntryData = EntryDataDS.Business.Entities.EntryData;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public static DataCache<Customs_Procedure> _customs_ProcedureCache;
    private readonly AsycudaDocumentSet _currentAsycudaDocumentSet = null;
    private IEnumerable<Customs_Procedure> _customs_Procedures;
    private IEnumerable<ExportTemplate> _exportTemplates;
    private static readonly decimal _minimumPossibleAsycudaWeight = 0.0m;
    private static decimal _runningMiniumWeight = 0.0m;
    private static readonly decimal WeightAsycudaNormallyOffBy = 0.5m;
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
                    _exportTemplates = ctx.GetExportTemplates().GetAwaiter().GetResult();
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
                        .Include("InCustomsProcedure.InCustomsProcedure.OutCustomsProcedure")
                        .Where(x => x.BondTypeId == CurrentApplicationSettings.BondTypeId
                                    && x.IsObsolete != true).ToList();
                }

            return _customs_Procedures;
        }
    }

    public List<CategoryTariffs> CategoryTariffs
    {
        get
        {
            if (_categoryTariffs == null)
                using (var ctx = new EntryDataDSContext())
                {
                    _categoryTariffs = ctx.CategoryTariffs.ToList();
                }

            return _categoryTariffs;
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

    private static readonly HashSet<string> EmailedMessagesList = new HashSet<string>();
    private static readonly bool isDBMem = false;
    private readonly ISaveDocumentCT _saveDocumentCt;
    private List<CategoryTariffs> _categoryTariffs = null;
}

public partial class BaseDataModel
{
    static BaseDataModel()
    {
        Instance = new BaseDataModel
        {
            CurrentApplicationSettings = new CoreEntitiesContext().ApplicationSettings.Include(x => x.Declarants)
                .First()
        };

        Initialization = InitializationAsync();
    }

    #region IAsyncInitialization Members

    public static Task Initialization { get; }
    public double ResourcePercentage { get; } = 0.8;
    public List<SessionSchedule> CurrentSessionSchedule { get; set; } = new List<SessionSchedule>();
    public SessionActions CurrentSessionAction { get; set; }

    public ISaveDocumentCT SaveDocumentCt
    {
        get { return _saveDocumentCt; }
    }

    #endregion

    public class MyPodData
    {
        public List<AsycudaSalesAllocations> Allocations { get; set; }
        public AlloEntryLineData EntlnData { get; set; }
        public List<string> AllNames { get; set; }
        public (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) Filter { get; set; }
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
                            }, false).GetAwaiter().GetResult().FirstOrDefault();
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
                // Avoid blocking calls in property setters. Load PreviousDocumentItem explicitly when needed.
                _previousDocumentItemId = value;
                PreviousDocumentItem = null; // Clear related property when Id changes
            }
        }

        public double Quantity { get; set; }
        public List<EntryDataDetailSummary> EntryDataDetails { get; set; }

        public double Freight { get; set; }
        public List<ITariffSupUnitLkp> TariffSupUnitLkps { get; set; }

        public double Weight { get; set; }

        public double InternalFreight { get; set; }
    }

    public BaseDataModel()
    {
        _saveDocumentCt = new SaveDocumentCTBulk();
    }
}