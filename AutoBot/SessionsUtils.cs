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
    using Serilog;

    public class SessionsUtils
    {
        public static Dictionary<string, Func<ILogger, Task>> SessionActions =>
            new Dictionary<string, Func<ILogger, Task>>(WaterNut.DataSpace.Utils.ignoreCase)
            {

                {"CreateDiscpancyEntries", (log) => ADJUtils.CreateAdjustmentEntries(false, "DIS", log) },
                {"RecreateDiscpancyEntries",(log) => ADJUtils.CreateAdjustmentEntries(true, "DIS", log) },
                {"CreateAdjustmentEntries",(log) => ADJUtils.CreateAdjustmentEntries(false, "ADJ", log) },
                {"RecreateAdjustmentEntries",(log) => ADJUtils.CreateAdjustmentEntries(true, "ADJ", log) },
                {"AutoMatch", (log) => DISUtils.AutoMatch() },
                {"AssessDiscpancyEntries", (log) => DISUtils.AssessDiscpancyEntries(log) },
                {"ExportDiscpancyEntries", (log) => DISUtils.ExportDiscpancyEntries("DIS") },
                {"ExportAdjustmentEntries", (log) => DISUtils.ExportDiscpancyEntries("ADJ") },
                //{"SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", (log) => AllocateSalesUtils.AllocateSales(log) },
                {"CreateEx9",(log) => CreateEX9Utils.CreateEx9(log, false, -1) },
                {"ExportEx9Entries", (log) => EX9Utils.ExportEx9Entries(log, -1) },
                {"AssessEx9Entries", (log) => EX9Utils.AssessEx9Entries(log, -1) },
                {"SubmitToCustoms", (log) => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1, log) },
                {"CleanupEntries", (log) => EntryDocSetUtils.CleanupEntries() },
                {"ClearAllocations", (log) => SalesUtils.ClearAllocations() },
                {"AssessDISEntries", (log) => DISUtils.AssessDISEntries("DIS", log) },
                {"AssessADJEntries", (log) => DISUtils.AssessDISEntries("ADJ", log) },
                {"DownloadSalesFiles",(log) => EX9Utils.DownloadSalesFiles(log, 20,"IM7History", false) },
                {"ImportSalesEntries", (log) => DocumentUtils.ImportSalesEntries(log, true) },
                {"ImportPOEntries", (log) => DocumentUtils.ImportPOEntries(log, false) },

                {"SubmitDiscrepanciesToCustoms", (log) => DISUtils.SubmitDiscrepanciesToCustoms(log) },
                {"DownloadPDFs", (log) => PDFUtils.DownloadPDFs() },
                {"LinkPDFs", (log) => PDFUtils.LinkPDFs() },
                {"RemoveDuplicateEntries", (log) => EntryDocSetUtils.RemoveDuplicateEntries(log) },
                {"FixIncompleteEntries", (log) => EntryDocSetUtils.FixIncompleteEntries(log) },
                {"EmailEntriesExpiringNextMonth", (log) => EX9Utils.EmailEntriesExpiringNextMonth(log) },
                {"EmailWarehouseErrors", (log) => EX9Utils.EmailWarehouseErrors(null) },
                {"RecreateLatestPOEntries",(log) => POUtils.RecreateLatestPOEntries(log) },
                {"ReImportC71", (log) => C71Utils.ReImportC71(log) },
                {"ReImportLIC", (log) => LICUtils.ReImportLIC() },
               
                {"DownLoadLicense", (log) => LICUtils.DownLoadLicence(false,new FileTypes()) },
                {"ReDownLoadLicence", (log) => LICUtils.DownLoadLicence(true, new FileTypes()) },
                {"CreateC71", (log) => C71Utils.CreateC71(new FileTypes()) },
                {"CreateLicense",(log) => LICUtils.CreateLicence(new FileTypes()) },
                { "ImportC71", (log) => C71Utils.ImportC71(new FileTypes(), log) },

                {"ImportLicense", (log) => LICUtils.ImportLicense(new FileTypes()) },
                {"ImportAllLicense", (log) => LICUtils.ImportAllLicense() },

                {"DownLoadC71", (log) => C71Utils.DownLoadC71(new FileTypes()) },
                {"SubmitMissingInvoices", (log) => Utils.SubmitMissingInvoices(new FileTypes(), null) },
                {"SubmitIncompleteEntryData",(log) => Utils.SubmitIncompleteEntryData(new FileTypes(), null) },
                {"SubmitUnclassifiedItems",(log) => ShipmentUtils.SubmitUnclassifiedItems(new FileTypes(), log) },
                {"SubmitInadequatePackages",(log) => ShipmentUtils.SubmitInadequatePackages(new FileTypes(), log) },
                {"SubmitIncompleteSuppliers",(log) => ShipmentUtils.SubmitIncompleteSuppliers(new FileTypes(), log) },
                {"AssessC71",(log) => C71Utils.AssessC71(new FileTypes()) },
                {"AssessLicense",(log) => LICUtils.AssessLicense(new FileTypes()) },
                {"RecreatePOEntries", (log) => POUtils.RecreatePOEntries(log) },
               
                {"ExportPOEntries", (log) => POUtils.ExportPOEntries(log) },
                {"AssessPOEntries", (log) => POUtils.AssessPOEntries(new FileTypes(), log) },
                { "AttachToDocSetByRef", (log) => EntryDocSetUtils.AttachToDocSetByRef(log) },
                {"DownloadPOFiles",(log) => EX9Utils.DownloadSalesFiles(log, 10, "IM7", false) },
                {"SubmitPOs", (log) => POUtils.SubmitPOs(log) },
                {"RecreateEx9", (log) => EX9Utils.RecreateEx9(-1, log) },


                
                


                {"ReDownloadSalesFiles", (log) => SalesUtils.ReDownloadSalesFiles(log) },
                {"CleanupDiscpancies", (log) => DISUtils.CleanupDiscpancies() },
                {"SubmitDiscrepanciesPreAssessmentReportToCustoms", (log) => DISUtils.SubmitDiscrepanciesPreAssessmentReportToCustoms(log)
                },
                {"ClearAllDiscpancyEntries", (log) => ADJUtils.ClearAllAdjustmentEntries("DIS", log) },
                {"ClearAllAdjustmentEntries", (log) => ADJUtils.ClearAllAdjustmentEntries("ADJ", log) },
                {"ImportPDF", (log) => PDFUtils.ImportPDF(null, new FileTypes(), log) },
                {"CreateInstructions", (log) => ShipmentUtils.CreateInstructions() },
                {"SubmitUnknownDFPComments", (log) => SalesUtils.SubmitUnknownDFPComments(log) },
                {"ClearPOEntries", (log) => POUtils.ClearPOEntries(log) },
               
                {"ExportLatestPOEntries", (log) => POUtils.ExportLatestPOEntries(log) },
                {"EmailLatestPOEntries", (log) => POUtils.EmailLatestPOEntries(log) },
                {"LinkEmail", (log) => EntryDocSetUtils.LinkEmail(log) },
                {"RenameDuplicateDocuments", (log) => EntryDocSetUtils.RenameDuplicateDocuments(log) },
                {"RenameDuplicateDocumentCodes", (log) => EntryDocSetUtils.RenameDuplicateDocumentCodes(log) },
                {"ReLinkPDFs", (log) => PDFUtils.ReLinkPDFs() },
                {"ImportAllSalesEntries", (log) => DocumentUtils.ImportAllSalesEntries(log, false) },
                {"RebuildSalesReport", (log) => SalesUtils.RebuildSalesReport() },
                {"Ex9AllAllocatedSales",(log) => EX9Utils.Ex9AllAllocatedSales(true, log) },
                {"SubmitSalesToCustoms", (log) => SubmitSalesToCustomsUtils.SubmitSalesToCustoms(log) },
                {"ImportExpiredEntires", (log) => EntryDocSetUtils.ImportExpiredEntires(log) },
                {"ImportCancelledEntries", (log) => EntryDocSetUtils.ImportCancelledEntries(log) },
                {"ImportAllFilesInDataFolder", (log) => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false, log)},
                {"relinkAllPreviousItems", (log) => EX9Utils.relinkAllPreviousItems()},
                {"ImportWarehouseErrors", (log) => ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1, log)},
                {"RunSQLBlackBox", (log) => Task.Run(SQLBlackBox.RunSqlBlackBox)},

                {"RecreateCurrentEx9", (log) => EX9Utils.RecreateEx9(0, log) },
                {"ExportCurrentEx9Entries", (log) => EX9Utils.ExportEx9Entries(log, 0) },
                {"AssessCurrentEx9Entries", (log) => EX9Utils.AssessEx9Entries(log, 0) },
                {"ImportCurrentWarehouseErrors", (log) => ImportWarehouseErrorsUtils.ImportWarehouseErrors(0, log)},
                {"SubmitToCustomsCurrent", (log) => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(0, log) },
                {"ImportAllZeroItemsInDataFolder", (log) => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllZeroItemsInDataFolder(false, log)},

                {"DownloadHistoryWithInvoices", (log) => EX9Utils.DownloadHistoryWithInvoices(10, "IM7History-Invoices", false)},
                //{"ConvertPNG2PDF", (log) => PDFUtils.ConvertPNGPNG2PDF(log) },

                 {"ReSubmitSalesToCustoms",(log) => SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms(log) },
                 {"ImportEntries", (log) => DocumentUtils.ImportEntries(false, "152, 365, 391, 556, 558, 596, 973, 2714, 2906, 2969, 2985, 2986, 2988, 2989, 2991, 2992, 2993, 2994, 2995, 2996, 2998, 2999, 3000, 3002, 3003, 3006, 3007, 3009, 3010, 3011, 3013, 3014, 3016, 3017, 4381, 4509, 4514, 4524, 4545, 4551, 4561, 4567, 4568, 4570, 4576, 4577, 4579, 4642, 4646, 4652, 4665, 4703, 4704, 4705, 4709, 4719, 5169, 6646, 6661, 6665, 6668, 6685, 6686, 6687, 9670, 10473, 10475, 11057, 11058, 11059, 11060, 11061, 11062, 11063, 11280, 11286, 11287, 11290, 11292, 11293, 11294, 11296, 11297, 11298, 11299, 11311, 11312, 11313, 11314, 11316, 11317, 11318, 11319, 11320, 11321, 12089, 14607, 15368, 15370, 15371, 15373, 15374, 15375, 15376, 15377, 15380, 15381, 15382, 15383, 15385, 15394, 15396, 15399, 15401, 15402, 15404, 15406, 15408, 21802, 25394, 25400, 25406, 25417, 25420, 25485, 25488, 25489, 25499, 25503, 25505, 25506, 25508, 25554, 25558, 26865, 28929, 28933, 28934, 28951, 28955, 28959, 28964, 29070, 40113, 40122, 40124, 40170, 40176, 40184, 40221, 40222, 40223, 40225, 40226, 40228, 40229, 40462, 40469, 40477, 40479, 40491, 40492, 40495, 40497, 40499, 40500, 40598, 40599, 40636, 40643, 40668, 40832, 40842, 40843, 40844, 40847, 40850, 40949, 40965, 40967, 40984, 40987, 41026, 41036, 41065, 41066, 41078, 41167, 41174, 41179, 41268, 44723, 46576, 50259", log) },
            };




        

        


    }
}