using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AdjustmentQS.Business.Services;
using Core.Common.Converters;
using EntryDataDS.Business.Entities;
using SalesDataQS.Business.Services;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class SessionsUtils
    {
        public static Dictionary<string, Action> SessionActions =>
            new Dictionary<string, Action>(WaterNut.DataSpace.Utils.ignoreCase)
            {

                {"CreateDiscpancyEntries",() => ADJUtils.CreateAdjustmentEntries(false, "DIS") },
                {"RecreateDiscpancyEntries",() => ADJUtils.CreateAdjustmentEntries(true, "DIS") },
                {"CreateAdjustmentEntries",() => ADJUtils.CreateAdjustmentEntries(false, "ADJ") },
                {"RecreateAdjustmentEntries",() => ADJUtils.CreateAdjustmentEntries(true, "ADJ") },
                {"AutoMatch", DISUtils.AutoMatch },
                {"AssessDiscpancyEntries", DISUtils.AssessDiscpancyEntries },
                {"ExportDiscpancyEntries", () => DISUtils.ExportDiscpancyEntries("DIS") },
                {"ExportAdjustmentEntries", () => DISUtils.ExportDiscpancyEntries("ADJ") },
                //{"SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", SalesUtils.AllocateSales },
                {"CreateEx9",() => EX9Utils.CreateEx9(false, -1) },
                {"ExportEx9Entries", () => EX9Utils.ExportEx9Entries(-1) },
                {"AssessEx9Entries", () => EX9Utils.AssessEx9Entries(-1) },
                {"SubmitToCustoms", SalesUtils.SubmitSalesXMLToCustoms },
                {"CleanupEntries", EntryDocSetUtils.CleanupEntries },
                {"ClearAllocations", SalesUtils.ClearAllocations },
                {"AssessDISEntries",() => DISUtils.AssessDISEntries("DIS") },
                {"AssessADJEntries",() => DISUtils.AssessDISEntries("ADJ") },
                {"DownloadSalesFiles",() => EX9Utils.DownloadSalesFiles(20,"IM7History") },
                {"ImportSalesEntries", () => SalesUtils.ImportSalesEntries(true) },
                {"ImportPOEntries", () => SalesUtils.ImportPOEntries(false) },

                {"SubmitDiscrepanciesToCustoms", DISUtils.SubmitDiscrepanciesToCustoms },
                {"DownloadPDFs", PDFUtils.DownloadPDFs },
                {"LinkPDFs", PDFUtils.LinkPDFs },
                {"RemoveDuplicateEntries", EntryDocSetUtils.RemoveDuplicateEntries },
                {"FixIncompleteEntries", EntryDocSetUtils.FixIncompleteEntries },
                {"EmailEntriesExpiringNextMonth", EX9Utils.EmailEntriesExpiringNextMonth },
                {"EmailWarehouseErrors", EX9Utils.EmailWarehouseErrors },
                {"RecreateLatestPOEntries",() =>  POUtils.RecreateLatestPOEntries() },
                {"ReImportC71", C71Utils.ReImportC71 },
                {"ReImportLIC", LICUtils.ReImportLIC },
               
                {"DownLoadLicense", () => LICUtils.DownLoadLicence(false,new FileTypes()) },
                {"ReDownLoadLicence", () => LICUtils.DownLoadLicence(true, new FileTypes()) },
                {"CreateC71", () => C71Utils.CreateC71(new FileTypes()) },
                {"CreateLicense",() => LICUtils.CreateLicence(new FileTypes()) },
                { "ImportC71", () => C71Utils.ImportC71(new FileTypes()) },

                {"ImportLicense", () => LICUtils.ImportLicense(new FileTypes()) },
                {"ImportAllLicense", () => LICUtils.ImportAllLicense() },

                {"DownLoadC71", () => C71Utils.DownLoadC71(new FileTypes()) },
                {"SubmitMissingInvoices", () => Utils.SubmitMissingInvoices(new FileTypes()) },
                {"SubmitIncompleteEntryData",() => Utils.SubmitIncompleteEntryData(new FileTypes()) },
                {"SubmitUnclassifiedItems",() => ShipmentUtils.SubmitUnclassifiedItems(new FileTypes()) },
                {"SubmitInadequatePackages",() => ShipmentUtils.SubmitInadequatePackages(new FileTypes()) },
                {"SubmitIncompleteSuppliers",() => ShipmentUtils.SubmitIncompleteSuppliers(new FileTypes()) },
                {"AssessC71",() => C71Utils.AssessC71(new FileTypes()) },
                {"AssessLicense",() => LICUtils.AssessLicense(new FileTypes()) },
                {"RecreatePOEntries", POUtils.RecreatePOEntries },
               
                {"ExportPOEntries", POUtils.ExportPOEntries },
                {"AssessPOEntries",() => POUtils.AssessPOEntries(new FileTypes()) },
                { "AttachToDocSetByRef", EntryDocSetUtils.AttachToDocSetByRef },
                {"DownloadPOFiles",() => EX9Utils.DownloadSalesFiles(10, "IM7", false) },
                {"SubmitPOs", POUtils.SubmitPOs },
                {"RecreateEx9", () => EX9Utils.RecreateEx9(-1) },
                
                {"ReDownloadSalesFiles", SalesUtils.ReDownloadSalesFiles },
                {"CleanupDiscpancies", DISUtils.CleanupDiscpancies },
                {"SubmitDiscrepanciesPreAssessmentReportToCustoms", DISUtils.SubmitDiscrepanciesPreAssessmentReportToCustoms },
                {"ClearAllDiscpancyEntries", () => ADJUtils.ClearAllAdjustmentEntries("DIS") },
                {"ClearAllAdjustmentEntries", () => ADJUtils.ClearAllAdjustmentEntries("ADJ") },
                {"ImportPDF", PDFUtils.ImportPDF },
                {"CreateInstructions", ShipmentUtils.CreateInstructions },
                {"SubmitUnknownDFPComments", SalesUtils.SubmitUnknownDFPComments },
                {"ClearPOEntries", POUtils.ClearPOEntries },
               
                {"ExportLatestPOEntries", POUtils.ExportLatestPOEntries },
                {"EmailLatestPOEntries", POUtils.EmailLatestPOEntries },
                {"LinkEmail", EntryDocSetUtils.LinkEmail },
                {"RenameDuplicateDocuments", EntryDocSetUtils.RenameDuplicateDocuments },
                {"RenameDuplicateDocumentCodes", EntryDocSetUtils.RenameDuplicateDocumentCodes },
                {"ReLinkPDFs", PDFUtils.ReLinkPDFs },
                {"ImportAllSalesEntries", SalesUtils.ImportAllSalesEntries },
                {"RebuildSalesReport", SalesUtils.RebuildSalesReport },
                {"Ex9AllAllocatedSales",() => EX9Utils.Ex9AllAllocatedSales(true) },
                {"SubmitSalesToCustoms", SalesUtils.SubmitSalesToCustoms },
                {"ImportExpiredEntires", EntryDocSetUtils.ImportExpiredEntires },
                {"ImportCancelledEntries", EntryDocSetUtils.ImportCancelledEntries },
                {"ImportAllFilesInDataFolder", Utils.ImportAllAsycudaDocumentsInDataFolder},
                {"relinkAllPreviousItems", EX9Utils.relinkAllPreviousItems},
                {"ImportWarehouseErrors", () => ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1)},
                {"RunSQLBlackBox", SQLBlackBox.RunSqlBlackBox},

                {"RecreateCurrentEx9", () => EX9Utils.RecreateEx9(0) },
                {"ExportCurrentEx9Entries", () => EX9Utils.ExportEx9Entries(0) },
                {"AssessCurrentEx9Entries", () => EX9Utils.AssessEx9Entries(0) },
                {"ImportCurrentWarehouseErrors", () => ImportWarehouseErrorsUtils.ImportWarehouseErrors(0)},


            };









        

        


    }
}