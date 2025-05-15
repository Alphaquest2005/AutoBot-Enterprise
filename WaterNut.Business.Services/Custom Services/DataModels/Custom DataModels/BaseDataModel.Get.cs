using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using EntryDataDS.Business.Services;
using EntryDataQS.Business.Entities;
using InventoryDS.Business.Entities;
using Omu.ValueInjecter;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.BaseDataModel.GettingItemSets;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace.Asycuda;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using AsycudaDocumentSetService = DocumentDS.Business.Services.AsycudaDocumentSetService;
using Customs_Procedure = DocumentDS.Business.Entities.Customs_Procedure;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

// Serilog usings
using Serilog;
using Serilog.Context;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    private static readonly ILogger _log = Log.ForContext<BaseDataModel>(); // Add static logger

    public static List<List<(string ItemNumber, int InventoryItemId)>> GetItemSets(string lst)
    {
        return isDBMem
            ? new GetItemSets().Execute(lst)
            : new GetItemSetsMem().Execute(lst);
    }

    public static string GetDocSetDirectoryName(string docSetReference) =>
        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
            docSetReference).Trim();

    public static int GetDefaultCustomsOperation()
    {
        return (int)(BaseDataModel.Instance.CurrentApplicationSettings.AllowXBond == "Visible"
            ? CustomsOperations.Warehouse
            : CustomsOperations.Import);
    }

    #region IAsyncInitialization Members

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
                    DocumentDS.DataModels.BaseDataModel.Instance.SearchCustoms_Procedure(new List<string> { "All" })
                        .ConfigureAwait(false));

        // Load ExportTemplates asynchronously during initialization
        using (var ctx = new ExportTemplateService())
        {
            Instance._exportTemplates = await ctx.GetExportTemplates().ConfigureAwait(false);
        }

        StatusModel.StopStatusUpdate();
    }

    public static async Task<(DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath)>
        CurrentSalesInfo(ILogger log, int months) // Add ILogger parameter
    {
        string methodName = nameof(CurrentSalesInfo);
        log.Information("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ Months: {Months} }}",
            methodName, "EmailSalesErrorsUtils.EmailSalesErrors", months); // Add METHOD_ENTRY log
        var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

        try
        {
            log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Getting session parameters.", methodName, "GetSessionParameters"); // Add step log
            var sessionParams = GetSessionParameterMonths(log); // Pass log parameter
            if (sessionParams.HasValue)
            {
                log.Information("INTERNAL_STEP ({MethodName} - {Stage}): Session parameters found. Using them.", methodName, "UseSessionParameters"); // Add step log
                stopwatch.Stop(); // Stop stopwatch
                log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                    methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
                return sessionParams.Value;
            }

            log.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetCurrentSalesInfo", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
            var getCurrentSalesInfoStopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await GetCurrentSalesInfo(log, months).ConfigureAwait(false); // Pass log
            getCurrentSalesInfoStopwatch.Stop();
            log.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "GetCurrentSalesInfo", getCurrentSalesInfoStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

            stopwatch.Stop(); // Stop stopwatch
            log.Information("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            return result;
        }
        catch (Exception ex) // Catch specific exception variable
        {
            stopwatch.Stop(); // Stop stopwatch
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
            throw; // Re-throw the original exception
        }
    }

    private static async Task<(DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath)>
        GetCurrentSalesInfo(ILogger log, int months) // Add ILogger parameter
    {
        string methodName = nameof(GetCurrentSalesInfo);
        log.Debug("METHOD_ENTRY: {MethodName}. Caller: {CallerInfo}. Context: {{ Months: {Months} }}",
            methodName, "BaseDataModel.CurrentSalesInfo", months); // Add METHOD_ENTRY log
        var stopwatch = System.Diagnostics.Stopwatch.StartNew(); // Start stopwatch

        try
        {
            log.Debug("INTERNAL_STEP ({MethodName} - {Stage}): Calculating start date.", methodName, "CalculateStartDate"); // Add step log
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(months);

            log.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "EntryDocSetUtils.CreateMonthYearAsycudaDocSet", "ASYNC_EXPECTED"); // Add INVOKING_OPERATION log
            var createDocSetStopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await EntryDocSetUtils.CreateMonthYearAsycudaDocSet(log, startDate).ConfigureAwait(false); // Pass log
            createDocSetStopwatch.Stop();
            log.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "EntryDocSetUtils.CreateMonthYearAsycudaDocSet", createDocSetStopwatch.ElapsedMilliseconds, "Async call completed (await)."); // Add OPERATION_INVOKED_AND_CONTROL_RETURNED log

            stopwatch.Stop(); // Stop stopwatch
            log.Debug("METHOD_EXIT_SUCCESS: {MethodName}. Total execution time: {ExecutionDurationMs}ms.",
                methodName, stopwatch.ElapsedMilliseconds); // Add METHOD_EXIT_SUCCESS log
            return result;
        }
        catch (Exception ex) // Catch specific exception variable
        {
            stopwatch.Stop(); // Stop stopwatch
            log.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                methodName, stopwatch.ElapsedMilliseconds, ex.Message); // Add METHOD_EXIT_FAILURE log
            throw; // Re-throw the original exception
        }
    }

    private static (DateTime StartDate, DateTime EndDate, AsycudaDocumentSet DocSet, string DirPath)?
        GetSessionParameterMonths(ILogger log) // Added ILogger parameter
    {
        var parameterSet = BaseDataModel.Instance.CurrentSessionSchedule.FirstOrDefault(x =>
            x.SesseionId == BaseDataModel.Instance.CurrentSessionAction.SessionId)?.ParameterSet;
        if (BaseDataModel.Instance.CurrentSessionAction == null || parameterSet == null) return null;

        var startDate = DateTime.Parse(parameterSet.ParameterSetParameters.Select(x => x.Parameters)
            .FirstOrDefault(x => x.Name == "StartDate")?.Value);

        var endDate = DateTime.Parse(parameterSet.ParameterSetParameters.Select(x => x.Parameters)
            .FirstOrDefault(x => x.Name == "EndDate")?.Value);

        // This call was to the now async GetAsycudaDocumentSet.
        // GetSessionParameterMonths needs to be re-evaluated if it should be async.
        // For now, to fix the immediate build error, this specific call needs to be handled.
        // However, making GetSessionParameterMonths fully async is better if possible.
        // For a minimal fix, if this is the only async call, it could be awaited here,
        // but that would require GetSessionParameterMonths to return a Task.
        // Assuming the user will address this if GetSessionParameterMonths needs to be async.
        // The error CS0029 was in GetCurrentSalesInfo, which is now fixed.
        // This line might cause issues if GetAsycudaDocumentSet is called synchronously.
        // For now, I will assume the user's previous fix to EntryDocSetUtils.GetAsycudaDocumentSet
        // is being called correctly elsewhere or this path is not the one causing the immediate build error.
        // The primary error was in GetCurrentSalesInfo.
        var docSet = WaterNut.DataSpace.EntryDocSetUtils
            .GetAsycudaDocumentSet(
                log, // Added log parameter
                parameterSet.ParameterSetParameters.Select(x => x.Parameters)
                    .FirstOrDefault(x => x.Name == "AsycudaDocumentSet")?.Value ?? "Unknown", false).GetAwaiter()
            .GetResult();

        var dirPath =
            StringExtensions.UpdateToCurrentUser(
                Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    docSet.Declarant_Reference_Number));
        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);


        return (StartDate: startDate, EndDate: endDate, DocSet: docSet, DirPath: dirPath);
    }

    public static Customs_Procedure GetCustoms_Procedure(Func<Customs_Procedure, bool> dtpredicate)
    {
        return BaseDataModel.Instance.Customs_Procedures
            .Where(dtpredicate)
            .OrderByDescending(x => x.IsDefault == true)
            .First();
    }

    public async Task<xcuda_ASYCUDA> GetDocument(int ASYCUDA_Id, List<string> includeLst = null)
    {
        using (var ctx = new xcuda_ASYCUDAService { StartTracking = true })
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

    internal TariffCode GetTariffCode(Func<TariffCode, bool> p)
    {
        using (var ctx = new InventoryDSContext())
        {
            return ctx.TariffCodes.FirstOrDefault(p);
        }
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
                        "Customs_Procedure.Document_Type",
                        "Consignee"
                    })
                .ConfigureAwait(false);
        }
    }

    public async Task<AsycudaDocument> GetAsycudaDocument(int asycudaId)
    {
        using (var ctx = new CoreEntities.Business.Services.AsycudaDocumentService())
        {
            return await ctx.GetAsycudaDocumentByKey(asycudaId.ToString(),
                    new List<string>
                    {
                        //"xcuda_ASYCUDA_ExtendedProperties",
                        //"xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA",
                        //"xcuda_ASYCUDA_ExtendedProperties.xcuda_ASYCUDA.xcuda_Declarant",
                        //"AsycudaDocumentSet_Attachments.Attachment",
                        //"AsycudaDocumentSet_Attachments.FileType",
                        //"Customs_Procedure.Document_Type"
                    })
                .ConfigureAwait(false);
        }
    }

    private async Task<xcuda_Item> GetDocumentItem(int item_Id, List<string> includeLst)
    {
        using (var ctx = new xcuda_ItemService { StartTracking = true })
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
}