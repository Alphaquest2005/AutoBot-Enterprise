using System;
using System.Collections.Generic;
using System.IO;
using AutoBotUtilities;
using AutoBotUtilities.CSV;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class FileUtils
    {
        public static Dictionary<string, Action<FileTypes, FileInfo[]>> FileActions =>
            new Dictionary<string, Action<FileTypes, FileInfo[]>>(WaterNut.DataSpace.Utils.ignoreCase)
            {
                {"ImportSalesEntries",async (ft, fs) => await DocumentUtils.ImportSalesEntries(false).ConfigureAwait(false) },
                {"AllocateSales",async (ft, fs) => await AllocateSalesUtils.AllocateSales().ConfigureAwait(false) },
                {"CreateEx9",async (ft, fs) => await CreateEX9Utils.CreateEx9(false, -1).ConfigureAwait(false) },
                {"ExportEx9Entries",async (ft, fs) => await EX9Utils.ExportEx9Entries(-1).ConfigureAwait(false) },
                {"AssessEx9Entries",async (ft, fs) => await EX9Utils.AssessEx9Entries(-1).ConfigureAwait(false) },
                {"SaveCsv", async (ft, fs) => await CSVUtils.SaveCsv(fs, ft).ConfigureAwait(false) }, // Corrected namespace qualification
                {"ReplaceCSV",async (ft, fs) => await CSVUtils.ReplaceCSV(fs, ft).ConfigureAwait(false) },
                {"RecreatePOEntries",async (ft, fs) => await POUtils.RecreatePOEntries(ft.AsycudaDocumentSetId).ConfigureAwait(false) },
                {"ExportPOEntries",async (ft, fs) => await POUtils.ExportPOEntries(ft.AsycudaDocumentSetId).ConfigureAwait(false) },
                {"AssessPOEntry",async (ft, fs) => await POUtils.AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId).ConfigureAwait(false)},
                {"EmailPOEntries",async (ft, fs) => await POUtils.EmailPOEntries(ft.AsycudaDocumentSetId).ConfigureAwait(false) },
                {"DownloadSalesFiles",(ft, fs) => EX9Utils.DownloadSalesFiles(10, "IM7History",false) },
                {"Xlsx2csv",  async (ft, fs) =>  await XLSXProcessor.Xlsx2csv(fs, new List<FileTypes>(){ft}).ConfigureAwait(false) },
                {"SaveInfo", async (ft, fs) => await EmailTextProcessor.Execute(fs, ft).ConfigureAwait(false) },
                {"CleanupEntries",async (ft, fs) => await EntryDocSetUtils.CleanupEntries().ConfigureAwait(false) },
                {"SubmitToCustoms",async (ft, fs) => await SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1).ConfigureAwait(false) },
                {"MapUnClassifiedItems", (ft, fs) => ShipmentUtils.MapUnClassifiedItems(ft,fs) },
                {"UpdateSupplierInfo", (ft, fs) => ShipmentUtils.UpdateSupplierInfo(ft,fs) },
                {"ImportPDF", async (ft, fs) => await InvoiceReader.InvoiceReader.ImportPDF(fs, ft).ConfigureAwait(false) },//PDFUtils.ImportPDF(fs, ft).GetAwaiter().GetResult() },
                {"CreateShipmentEmail", async (types, infos) => await ShipmentUtils.CreateShipmentEmail(types, infos).ConfigureAwait(false) },
                //{"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
                
                //{"AttachToDocSetByRef", (ft, fs) => AttachToDocSetByRef(ft.AsycudaDocumentSetId) },


                {"SyncConsigneeInDB", async(types, infos) => await EntryDocSetUtils.SyncConsigneeInDB(types, infos).ConfigureAwait(false) },

                {"ClearDocSetEntries",async (ft, fs) => await EntryDocSetUtils.ClearDocSetEntries(ft).ConfigureAwait(false) },
               
                {"SubmitDocSetUnclassifiedItems",async (ft, fs) => await ShipmentUtils.SubmitDocSetUnclassifiedItems(ft).ConfigureAwait(false) },
                {"AllocateDocSetDiscrepancies",async (ft, fs) => await DISUtils.AllocateDocSetDiscrepancies(ft).ConfigureAwait(false) },
                {"CleanupDocSetDiscpancies",async (ft, fs) => await DISUtils.CleanupDocSetDiscpancies(ft).ConfigureAwait(false) },
                {"RecreateDocSetDiscrepanciesEntries", (ft, fs) => DISUtils.RecreateDocSetDiscrepanciesEntries(ft) },
                {"ExportDocSetDiscpancyEntries", (ft, fs) => DISUtils.ExportDocSetDiscpancyEntries("DIS",ft) },
                {"SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms", async (ft, fs) => await DISUtils.SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(ft).ConfigureAwait(false) },
                {"AssessDiscrepancyExecutions", async (ft, fs) => await DISUtils.AssessDiscrepancyExecutions(ft, fs).ConfigureAwait(false) },
                {"AttachEmailPDF", PDFUtils.AttachEmailPDF },
                {"ReSubmitDiscrepanciesToCustoms", async (types, infos) => await DISUtils.ReSubmitDiscrepanciesToCustoms(types, infos).ConfigureAwait(false)
                },
                {"ReSubmitSalesToCustoms", async (types, infos) => await SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms(types, infos).ConfigureAwait(false)
                },


                {"SubmitMissingInvoices",  async (ft, fs) => await Utils.SubmitMissingInvoices(ft).ConfigureAwait(false) },
                {"SubmitIncompleteEntryData",async (ft, fs) => await Utils.SubmitIncompleteEntryData(ft).ConfigureAwait(false) },
                {"SubmitUnclassifiedItems",async (ft, fs) =>  await ShipmentUtils.SubmitUnclassifiedItems(ft).ConfigureAwait(false) },
                {"SubmitInadequatePackages",async (ft, fs) => await ShipmentUtils.SubmitInadequatePackages(ft).ConfigureAwait(false) },
                {"SubmitIncompleteSuppliers",async (ft, fs) => await ShipmentUtils.SubmitIncompleteSuppliers(ft).ConfigureAwait(false) },
                {"CreateC71",async (ft, fs) => await C71Utils.CreateC71(ft).ConfigureAwait(false) },
                {"CreateLicense",async (ft, fs) =>  await LICUtils.CreateLicence(ft).ConfigureAwait(false)},
                { "AssessC71",(ft, fs) => C71Utils.AssessC71(ft) },
                {"AssessLicense",(ft, fs) => LICUtils.AssessLicense(ft) },
                {"DownLoadC71", (ft, fs) => C71Utils.DownLoadC71(ft) },
                {"DownLoadLicense", (ft, fs) => LICUtils.DownLoadLicence(false, ft) },
                { "ImportC71", async (ft, fs) => await C71Utils.ImportC71(ft).ConfigureAwait(false) },
                {"ImportLicense", async (ft, fs) => await LICUtils.ImportLicense(ft).ConfigureAwait(false) },
               
                { "AttachToDocSetByRef",(ft, fs) => EntryDocSetUtils.AttachToDocSetByRef(ft) },
                
                {"AssessPOEntries", async (ft, fs) => await POUtils.AssessPOEntries(ft).ConfigureAwait(false) },
                {"AssessDiscpancyEntries", async (ft, fs) => await DISUtils.AssessDiscpancyEntries(ft, fs).ConfigureAwait(false) },
                {"DeletePONumber", POUtils.DeletePONumber },
                { "SubmitPOs", async (ft, fs) => await POUtils.SubmitPOs().ConfigureAwait(false) },
                {"SubmitEntryCIF", async (types, infos) => await EntryDocSetUtils.SubmitEntryCIF(types, infos).ConfigureAwait(false) },
                {"SubmitBlankLicenses", async (ft,fs) => await LICUtils.SubmitBlankLicenses(ft).ConfigureAwait(false) },
                {"ProcessUnknownCSVFileType", (ft,fs) => CSVUtils.ProcessUnknownCSVFileType(ft, fs) },
                {"ProcessUnknownPDFFileType", (ft,fs) => PDFUtils.ProcessUnknownPDFFileType(ft, fs) },
                {"ImportUnAttachedSummary", async (ft,fs) => await ShipmentUtils.ImportUnAttachedSummary(ft, fs).ConfigureAwait(false) },

                {"RemoveDuplicateEntries", async (ft,fs) => await EntryDocSetUtils.RemoveDuplicateEntries().ConfigureAwait(false) },
                {"FixIncompleteEntries", async (ft,fs) => await EntryDocSetUtils.FixIncompleteEntries().ConfigureAwait(false) },
                {"EmailWarehouseErrors", async (ft,fs) => await EntryDocSetUtils.EmailWarehouseErrors(ft,fs).ConfigureAwait(false) },
                {"ImportExpiredEntires", async (ft,fs) => await EntryDocSetUtils.ImportExpiredEntires().ConfigureAwait(false) },
                {"ImportCancelledEntries", async (ft,fs) => await EntryDocSetUtils.ImportCancelledEntries().ConfigureAwait(false) },
                {"EmailEntriesExpiringNextMonth", async (ft,fs) => await EntryDocSetUtils.EmailEntriesExpiringNextMonth().ConfigureAwait(false) },
                {"RecreateEx9", async (types, infos) => await EX9Utils.RecreateEx9(types, infos).ConfigureAwait(false) },//
                {"UpdateRegEx", UpdateInvoice.UpdateRegEx},
                {"ImportWarehouseErrors", async (ft,fs) => await ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1).ConfigureAwait(false)},
                {"Kill", Utils.Kill},
                {"Continue", (ft, fs) => { }},
                {"LinkPDFs", (ft,fs) => PDFUtils.LinkPDFs()},
                {"DownloadPOFiles", (ft,fs) => EX9Utils.DownloadSalesFiles(10, "IM7", false)},
                {"ReDownloadPOFiles", (ft,fs) => EX9Utils.DownloadSalesFiles(10, "IM7", true)},
                {"SubmitDiscrepanciesToCustoms", async (types, infos) => await DISUtils.SubmitDiscrepanciesToCustoms(types, infos).ConfigureAwait(false) },
                {"ClearShipmentData", ShipmentUtils.ClearShipmentData},
                {"ImportPOEntries", async (ft,fs) => await DocumentUtils.ImportPOEntries(false).ConfigureAwait(false) },
                {"ImportAllAsycudaDocumentsInDataFolder", (ft,fs) => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false) },
                {"ImportEntries",async (ft, fs) => await DocumentUtils.ImportEntries(false, ft.Data.ToString()).ConfigureAwait(false) },
                {"ImportShipmentInfoFromTxt", async (types, infos) => await ShipmentUtils.ImportShipmentInfoFromTxt(types, infos).ConfigureAwait(false) }, // Added mapping for new action


            };
            // Removed extra closing brace here
    }
}