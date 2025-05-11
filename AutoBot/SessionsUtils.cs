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

                {"CreateDiscpancyEntries",async () => await ADJUtils.CreateAdjustmentEntries(false, "DIS").ConfigureAwait(false) },
                {"RecreateDiscpancyEntries",async () => await ADJUtils.CreateAdjustmentEntries(true, "DIS").ConfigureAwait(false) },
                {"CreateAdjustmentEntries",async () => await ADJUtils.CreateAdjustmentEntries(false, "ADJ").ConfigureAwait(false) },
                {"RecreateAdjustmentEntries",async () => await ADJUtils.CreateAdjustmentEntries(true, "ADJ").ConfigureAwait(false) },
                {"AutoMatch", async () => await DISUtils.AutoMatch().ConfigureAwait(false) },
                {"AssessDiscpancyEntries", async () => await DISUtils.AssessDiscpancyEntries().ConfigureAwait(false) },
                {"ExportDiscpancyEntries", () => DISUtils.ExportDiscpancyEntries("DIS") },
                {"ExportAdjustmentEntries", () => DISUtils.ExportDiscpancyEntries("ADJ") },
                //{"SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", async () => await AllocateSalesUtils.AllocateSales().ConfigureAwait(false) },
                {"CreateEx9",async () => await CreateEX9Utils.CreateEx9(false, -1).ConfigureAwait(false) },
                {"ExportEx9Entries", async () => await EX9Utils.ExportEx9Entries(-1).ConfigureAwait(false) },
                {"AssessEx9Entries", async () => await EX9Utils.AssessEx9Entries(-1).ConfigureAwait(false) },
                {"SubmitToCustoms", async () => await SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1).ConfigureAwait(false) },
                {"CleanupEntries", async () => await EntryDocSetUtils.CleanupEntries().ConfigureAwait(false) },
                {"ClearAllocations", SalesUtils.ClearAllocations },
                {"AssessDISEntries", async () => await DISUtils.AssessDISEntries("DIS").ConfigureAwait(false) },
                {"AssessADJEntries", async () => await DISUtils.AssessDISEntries("ADJ").ConfigureAwait(false) },
                {"DownloadSalesFiles",() => EX9Utils.DownloadSalesFiles(20,"IM7History") },
                {"ImportSalesEntries", async () => await DocumentUtils.ImportSalesEntries(true).ConfigureAwait(false) },
                {"ImportPOEntries", async () => await DocumentUtils.ImportPOEntries(false).ConfigureAwait(false) },

                {"SubmitDiscrepanciesToCustoms", async () => await DISUtils.SubmitDiscrepanciesToCustoms().ConfigureAwait(false) },
                {"DownloadPDFs", PDFUtils.DownloadPDFs },
                {"LinkPDFs", PDFUtils.LinkPDFs },
                {"RemoveDuplicateEntries", async () => await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false) },
                {"FixIncompleteEntries", async () => await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false) },
                {"EmailEntriesExpiringNextMonth", async () => await EX9Utils.EmailEntriesExpiringNextMonth().ConfigureAwait(false) },
                {"EmailWarehouseErrors", async () =>await EX9Utils.EmailWarehouseErrors().ConfigureAwait(false) },
                {"RecreateLatestPOEntries",async () =>  await POUtils.RecreateLatestPOEntries().ConfigureAwait(false) },
                {"ReImportC71", async () => await C71Utils.ReImportC71().ConfigureAwait(false) },
                {"ReImportLIC", async () => await LICUtils.ReImportLIC().ConfigureAwait(false) },
               
                {"DownLoadLicense", () => LICUtils.DownLoadLicence(false,new FileTypes()) },
                {"ReDownLoadLicence", () => LICUtils.DownLoadLicence(true, new FileTypes()) },
                {"CreateC71", async () => await C71Utils.CreateC71(new FileTypes()).ConfigureAwait(false) },
                {"CreateLicense",async () => await LICUtils.CreateLicence(new FileTypes()).ConfigureAwait(false) },
                { "ImportC71", async () => await C71Utils.ImportC71(new FileTypes()).ConfigureAwait(false) },

                {"ImportLicense", async () => await LICUtils.ImportLicense(new FileTypes()).ConfigureAwait(false) },
                {"ImportAllLicense", async () => await LICUtils.ImportAllLicense().ConfigureAwait(false) },

                {"DownLoadC71", () => C71Utils.DownLoadC71(new FileTypes()) },
                {"SubmitMissingInvoices", async () => await Utils.SubmitMissingInvoices(new FileTypes()).ConfigureAwait(false) },
                {"SubmitIncompleteEntryData",async () => await Utils.SubmitIncompleteEntryData(new FileTypes()).ConfigureAwait(false) },
                {"SubmitUnclassifiedItems",async () => await ShipmentUtils.SubmitUnclassifiedItems(new FileTypes()).ConfigureAwait(false) },
                {"SubmitInadequatePackages",async () => await ShipmentUtils.SubmitInadequatePackages(new FileTypes()).ConfigureAwait(false) },
                {"SubmitIncompleteSuppliers",async () => await ShipmentUtils.SubmitIncompleteSuppliers(new FileTypes()).ConfigureAwait(false) },
                {"AssessC71",() => C71Utils.AssessC71(new FileTypes()) },
                {"AssessLicense",() => LICUtils.AssessLicense(new FileTypes()) },
                {"RecreatePOEntries", async () => await POUtils.RecreatePOEntries().ConfigureAwait(false) },
               
                {"ExportPOEntries", async () => await POUtils.ExportPOEntries().ConfigureAwait(false) },
                {"AssessPOEntries", async () => await POUtils.AssessPOEntries(new FileTypes()).ConfigureAwait(false) },
                { "AttachToDocSetByRef", async () => await EntryDocSetUtils.AttachToDocSetByRef().ConfigureAwait(false) },
                {"DownloadPOFiles",() => EX9Utils.DownloadSalesFiles(10, "IM7", false) },
                {"SubmitPOs", async () => await POUtils.SubmitPOs().ConfigureAwait(false) },
                {"RecreateEx9", async () => await EX9Utils.RecreateEx9(-1).ConfigureAwait(false) },


                


                
                {"ReDownloadSalesFiles", SalesUtils.ReDownloadSalesFiles },
                {"CleanupDiscpancies", async () => await DISUtils.CleanupDiscpancies().ConfigureAwait(false) },
                {"SubmitDiscrepanciesPreAssessmentReportToCustoms", async () => await DISUtils.SubmitDiscrepanciesPreAssessmentReportToCustoms().ConfigureAwait(false)
                },
                {"ClearAllDiscpancyEntries", () => ADJUtils.ClearAllAdjustmentEntries("DIS") },
                {"ClearAllAdjustmentEntries", () => ADJUtils.ClearAllAdjustmentEntries("ADJ") },
                {"ImportPDF", async () => await PDFUtils.ImportPDF().ConfigureAwait(false) },
                {"CreateInstructions", ShipmentUtils.CreateInstructions },
                {"SubmitUnknownDFPComments", async () => await SalesUtils.SubmitUnknownDFPComments().ConfigureAwait(false) },
                {"ClearPOEntries", async () =>await POUtils.ClearPOEntries().ConfigureAwait(false) },
               
                {"ExportLatestPOEntries", async () => await POUtils.ExportLatestPOEntries().ConfigureAwait(false) },
                {"EmailLatestPOEntries", async () => await POUtils.EmailLatestPOEntries().ConfigureAwait(false) },
                {"LinkEmail", EntryDocSetUtils.LinkEmail },
                {"RenameDuplicateDocuments", async () => await EntryDocSetUtils.RenameDuplicateDocuments().ConfigureAwait(false) },
                {"RenameDuplicateDocumentCodes", EntryDocSetUtils.RenameDuplicateDocumentCodes },
                {"ReLinkPDFs", PDFUtils.ReLinkPDFs },
                {"ImportAllSalesEntries", async () => await DocumentUtils.ImportAllSalesEntries(false).ConfigureAwait(false) },
                {"RebuildSalesReport", SalesUtils.RebuildSalesReport },
                {"Ex9AllAllocatedSales",async () => await EX9Utils.Ex9AllAllocatedSales(true).ConfigureAwait(false) },
                {"SubmitSalesToCustoms", async () => await SubmitSalesToCustomsUtils.SubmitSalesToCustoms().ConfigureAwait(false) },
                {"ImportExpiredEntires", async() => await EntryDocSetUtils.ImportExpiredEntires().ConfigureAwait(false) },
                {"ImportCancelledEntries", async() => await EntryDocSetUtils.ImportCancelledEntries().ConfigureAwait(false) },
                {"ImportAllFilesInDataFolder", () => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false)},
                {"relinkAllPreviousItems", async () => await EX9Utils.relinkAllPreviousItems().ConfigureAwait(false)},
                {"ImportWarehouseErrors", async () => await ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1).ConfigureAwait(false)},
                {"RunSQLBlackBox", SQLBlackBox.RunSqlBlackBox},

                {"RecreateCurrentEx9", async () => await EX9Utils.RecreateEx9(0).ConfigureAwait(false) },
                {"ExportCurrentEx9Entries", async () => await EX9Utils.ExportEx9Entries(0).ConfigureAwait(false) },
                {"AssessCurrentEx9Entries", async () => await EX9Utils.AssessEx9Entries(0).ConfigureAwait(false) },
                {"ImportCurrentWarehouseErrors", async () => await ImportWarehouseErrorsUtils.ImportWarehouseErrors(0).ConfigureAwait(false)},
                {"SubmitToCustomsCurrent", async () => await SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(0).ConfigureAwait(false) },
                {"ImportAllZeroItemsInDataFolder", () => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllZeroItemsInDataFolder(false)},

                {"DownloadHistoryWithInvoices", () => EX9Utils.DownloadHistoryWithInvoices(10, "IM7History-Invoices", false)},
                {"ConvertPNG2PDF", () => PDFUtils.ConvertPNG2PDF() },

                 {"ReSubmitSalesToCustoms",async () => await SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms().ConfigureAwait(false) },
                 {"ImportEntries", async () => await DocumentUtils.ImportEntries(false, "152, 365, 391, 556, 558, 596, 973, 2714, 2906, 2969, 2985, 2986, 2988, 2989, 2991, 2992, 2993, 2994, 2995, 2996, 2998, 2999, 3000, 3002, 3003, 3006, 3007, 3009, 3010, 3011, 3013, 3014, 3016, 3017, 4381, 4509, 4514, 4524, 4545, 4551, 4561, 4567, 4568, 4570, 4576, 4577, 4579, 4642, 4646, 4652, 4665, 4703, 4704, 4705, 4709, 4719, 5169, 6646, 6661, 6665, 6668, 6685, 6686, 6687, 9670, 10473, 10475, 11057, 11058, 11059, 11060, 11061, 11062, 11063, 11280, 11286, 11287, 11290, 11292, 11293, 11294, 11296, 11297, 11298, 11299, 11311, 11312, 11313, 11314, 11316, 11317, 11318, 11319, 11320, 11321, 12089, 14607, 15368, 15370, 15371, 15373, 15374, 15375, 15376, 15377, 15380, 15381, 15382, 15383, 15385, 15394, 15396, 15399, 15401, 15402, 15404, 15406, 15408, 21802, 25394, 25400, 25406, 25417, 25420, 25485, 25488, 25489, 25499, 25503, 25505, 25506, 25508, 25554, 25558, 26865, 28929, 28933, 28934, 28951, 28955, 28959, 28964, 29070, 40113, 40122, 40124, 40170, 40176, 40184, 40221, 40222, 40223, 40225, 40226, 40228, 40229, 40462, 40469, 40477, 40479, 40491, 40492, 40495, 40497, 40499, 40500, 40598, 40599, 40636, 40643, 40668, 40832, 40842, 40843, 40844, 40847, 40850, 40949, 40965, 40967, 40984, 40987, 41026, 41036, 41065, 41066, 41078, 41167, 41174, 41179, 41268, 44723, 46576, 50259").ConfigureAwait(false) },
            };









        

        


    }
}