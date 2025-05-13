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
        public static Dictionary<string, Func< Task>> SessionActions =>
            new Dictionary<string, Func<Task>>(WaterNut.DataSpace.Utils.ignoreCase)
            {

                {"CreateDiscpancyEntries", () => ADJUtils.CreateAdjustmentEntries(false, "DIS") },
                {"RecreateDiscpancyEntries",() => ADJUtils.CreateAdjustmentEntries(true, "DIS") },
                {"CreateAdjustmentEntries",() => ADJUtils.CreateAdjustmentEntries(false, "ADJ") },
                {"RecreateAdjustmentEntries",() => ADJUtils.CreateAdjustmentEntries(true, "ADJ") },
                {"AutoMatch", () => DISUtils.AutoMatch() },
                {"AssessDiscpancyEntries", () => DISUtils.AssessDiscpancyEntries() },
                {"ExportDiscpancyEntries", () => DISUtils.ExportDiscpancyEntries("DIS") },
                {"ExportAdjustmentEntries", () => DISUtils.ExportDiscpancyEntries("ADJ") },
                //{"SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", () => AllocateSalesUtils.AllocateSales() },
                {"CreateEx9",() => CreateEX9Utils.CreateEx9(false, -1) },
                {"ExportEx9Entries", () => EX9Utils.ExportEx9Entries(-1) },
                {"AssessEx9Entries", () => EX9Utils.AssessEx9Entries(-1) },
                {"SubmitToCustoms", () => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1) },
                {"CleanupEntries", () => EntryDocSetUtils.CleanupEntries() },
                {"ClearAllocations", SalesUtils.ClearAllocations },
                {"AssessDISEntries", () => DISUtils.AssessDISEntries("DIS") },
                {"AssessADJEntries", () => DISUtils.AssessDISEntries("ADJ") },
                {"DownloadSalesFiles",() => EX9Utils.DownloadSalesFiles(20,"IM7History") },
                {"ImportSalesEntries", () => DocumentUtils.ImportSalesEntries(true) },
                {"ImportPOEntries", () => DocumentUtils.ImportPOEntries(false) },

                {"SubmitDiscrepanciesToCustoms", () => DISUtils.SubmitDiscrepanciesToCustoms() },
                {"DownloadPDFs", PDFUtils.DownloadPDFs },
                {"LinkPDFs", PDFUtils.LinkPDFs },
                {"RemoveDuplicateEntries", () => EntryDocSetUtils.RemoveDuplicateEntries() },
                {"FixIncompleteEntries", () => EntryDocSetUtils.FixIncompleteEntries() },
                {"EmailEntriesExpiringNextMonth", () => EX9Utils.EmailEntriesExpiringNextMonth() },
                {"EmailWarehouseErrors", () => EX9Utils.EmailWarehouseErrors() },
                {"RecreateLatestPOEntries",() => POUtils.RecreateLatestPOEntries() },
                {"ReImportC71", () => C71Utils.ReImportC71() },
                {"ReImportLIC", () => LICUtils.ReImportLIC() },
               
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
                {"RecreatePOEntries", () => POUtils.RecreatePOEntries() },
               
                {"ExportPOEntries", () => POUtils.ExportPOEntries() },
                {"AssessPOEntries", () => POUtils.AssessPOEntries(new FileTypes()) },
                { "AttachToDocSetByRef", () => EntryDocSetUtils.AttachToDocSetByRef() },
                {"DownloadPOFiles",() => EX9Utils.DownloadSalesFiles(10, "IM7", false) },
                {"SubmitPOs", () => POUtils.SubmitPOs() },
                {"RecreateEx9", () => EX9Utils.RecreateEx9(-1) },


                


                
                {"ReDownloadSalesFiles", SalesUtils.ReDownloadSalesFiles },
                {"CleanupDiscpancies", () => DISUtils.CleanupDiscpancies() },
                {"SubmitDiscrepanciesPreAssessmentReportToCustoms", () => DISUtils.SubmitDiscrepanciesPreAssessmentReportToCustoms()
                },
                {"ClearAllDiscpancyEntries", () => ADJUtils.ClearAllAdjustmentEntries("DIS") },
                {"ClearAllAdjustmentEntries", () => ADJUtils.ClearAllAdjustmentEntries("ADJ") },
                {"ImportPDF", () => PDFUtils.ImportPDF() },
                {"CreateInstructions", ShipmentUtils.CreateInstructions },
                {"SubmitUnknownDFPComments", () => SalesUtils.SubmitUnknownDFPComments() },
                {"ClearPOEntries", () => POUtils.ClearPOEntries() },
               
                {"ExportLatestPOEntries", () => POUtils.ExportLatestPOEntries() },
                {"EmailLatestPOEntries", () => POUtils.EmailLatestPOEntries() },
                {"LinkEmail", EntryDocSetUtils.LinkEmail },
                {"RenameDuplicateDocuments", () => EntryDocSetUtils.RenameDuplicateDocuments() },
                {"RenameDuplicateDocumentCodes", EntryDocSetUtils.RenameDuplicateDocumentCodes },
                {"ReLinkPDFs", PDFUtils.ReLinkPDFs },
                {"ImportAllSalesEntries", () => DocumentUtils.ImportAllSalesEntries(false) },
                {"RebuildSalesReport", SalesUtils.RebuildSalesReport },
                {"Ex9AllAllocatedSales",() => EX9Utils.Ex9AllAllocatedSales(true) },
                {"SubmitSalesToCustoms", () => SubmitSalesToCustomsUtils.SubmitSalesToCustoms() },
                {"ImportExpiredEntires", () => EntryDocSetUtils.ImportExpiredEntires() },
                {"ImportCancelledEntries", () => EntryDocSetUtils.ImportCancelledEntries() },
                {"ImportAllFilesInDataFolder", () => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false)},
                {"relinkAllPreviousItems", () => EX9Utils.relinkAllPreviousItems()},
                {"ImportWarehouseErrors", () => ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1)},
                {"RunSQLBlackBox", () => Task.Run(SQLBlackBox.RunSqlBlackBox)},

                {"RecreateCurrentEx9", () => EX9Utils.RecreateEx9(0) },
                {"ExportCurrentEx9Entries", () => EX9Utils.ExportEx9Entries(0) },
                {"AssessCurrentEx9Entries", () => EX9Utils.AssessEx9Entries(0) },
                {"ImportCurrentWarehouseErrors", () => ImportWarehouseErrorsUtils.ImportWarehouseErrors(0)},
                {"SubmitToCustomsCurrent", () => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(0) },
                {"ImportAllZeroItemsInDataFolder", () => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllZeroItemsInDataFolder(false)},

                {"DownloadHistoryWithInvoices", () => EX9Utils.DownloadHistoryWithInvoices(10, "IM7History-Invoices", false)},
                //{"ConvertPNG2PDF", () => PDFUtils.ConvertPNG2PDF() },

                 {"ReSubmitSalesToCustoms",() => SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms() },
                 {"ImportEntries", () => DocumentUtils.ImportEntries(false, "152, 365, 391, 556, 558, 596, 973, 2714, 2906, 2969, 2985, 2986, 2988, 2989, 2991, 2992, 2993, 2994, 2995, 2996, 2998, 2999, 3000, 3002, 3003, 3006, 3007, 3009, 3010, 3011, 3013, 3014, 3016, 3017, 4381, 4509, 4514, 4524, 4545, 4551, 4561, 4567, 4568, 4570, 4576, 4577, 4579, 4642, 4646, 4652, 4665, 4703, 4704, 4705, 4709, 4719, 5169, 6646, 6661, 6665, 6668, 6685, 6686, 6687, 9670, 10473, 10475, 11057, 11058, 11059, 11060, 11061, 11062, 11063, 11280, 11286, 11287, 11290, 11292, 11293, 11294, 11296, 11297, 11298, 11299, 11311, 11312, 11313, 11314, 11316, 11317, 11318, 11319, 11320, 11321, 12089, 14607, 15368, 15370, 15371, 15373, 15374, 15375, 15376, 15377, 15380, 15381, 15382, 15383, 15385, 15394, 15396, 15399, 15401, 15402, 15404, 15406, 15408, 21802, 25394, 25400, 25406, 25417, 25420, 25485, 25488, 25489, 25499, 25503, 25505, 25506, 25508, 25554, 25558, 26865, 28929, 28933, 28934, 28951, 28955, 28959, 28964, 29070, 40113, 40122, 40124, 40170, 40176, 40184, 40221, 40222, 40223, 40225, 40226, 40228, 40229, 40462, 40469, 40477, 40479, 40491, 40492, 40495, 40497, 40499, 40500, 40598, 40599, 40636, 40643, 40668, 40832, 40842, 40843, 40844, 40847, 40850, 40949, 40965, 40967, 40984, 40987, 41026, 41036, 41065, 41066, 41078, 41167, 41174, 41179, 41268, 44723, 46576, 50259") },
            };









        

        


    }
}