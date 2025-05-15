using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoBotUtilities;
using AutoBotUtilities.CSV;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

// Serilog usings
using Serilog;
using Serilog.Context;
using ExcelDataReader.Log;

namespace AutoBot
{
    public class FileUtils
    {
       
        public static Dictionary<string, Func<ILogger, FileTypes, FileInfo[], Task>> FileActions =>
            new Dictionary<string, Func<ILogger, FileTypes, FileInfo[], Task>>(WaterNut.DataSpace.Utils.ignoreCase)
            {
                // Modify lambdas to accept ILogger and pass it down where applicable
                {"ImportSalesEntries",(log, ft, fs) => DocumentUtils.ImportSalesEntries(log, false) }, // Signature needs update in DocumentUtils
                {"AllocateSales",(log, ft, fs) => AllocateSalesUtils.AllocateSales(log) }, // Signature needs update in AllocateSalesUtils
                {"CreateEx9",(log, ft, fs) => CreateEX9Utils.CreateEx9(log,false, -1) }, // Signature needs update in CreateEX9Utils
                {"ExportEx9Entries",(log, ft, fs) => EX9Utils.ExportEx9Entries(log,-1) }, // Signature needs update in EX9Utils
                {"AssessEx9Entries",(log, ft, fs) => EX9Utils.AssessEx9Entries(log, -1) }, // Signature needs update in EX9Utils
                {"SaveCsv", (log, ft, fs) => CSVUtils.SaveCsv(fs, ft, log) }, // Signature needs update in CSVUtils
                {"ReplaceCSV",(log, ft, fs) => CSVUtils.ReplaceCSV(fs, ft, log) }, // Signature needs update in CSVUtils
                {"RecreatePOEntries",(log, ft, fs) => POUtils.RecreatePOEntries(ft.AsycudaDocumentSetId, log) }, // Signature needs update in POUtils
                {"ExportPOEntries",(log, ft, fs) => POUtils.ExportPOEntries(ft.AsycudaDocumentSetId, log) }, // Signature needs update in POUtils
                {"AssessPOEntry", (log, ft, fs) => POUtils.AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId, log)}, // Signature needs update in POUtils
                {"EmailPOEntries", (log, ft, fs) => POUtils.EmailPOEntries(ft.AsycudaDocumentSetId, log) }, // Signature needs update in POUtils
                {"DownloadSalesFiles",(log, ft, fs) => EX9Utils.DownloadSalesFiles(log, 10, "IM7History",false) }, // Signature needs update in EX9Utils
                {"Xlsx2csv",  (log, ft, fs) => XLSXProcessor.Xlsx2csv(fs, new List < FileTypes >() { ft }, log) }, // Signature needs update in XLSXProcessor
                {"SaveInfo", (log, ft, fs) => EmailTextProcessor.Execute(log, fs, ft) }, // Pass log to EmailTextProcessor.Execute (already updated)
                {"CleanupEntries",(log, ft, fs) => EntryDocSetUtils.CleanupEntries() }, // Signature needs update in EntryDocSetUtils
                {"SubmitToCustoms",(log, ft, fs) => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1, log) }, // Signature needs update in SubmitSalesXmlToCustomsUtils
                {"MapUnClassifiedItems", (log, ft, fs) => ShipmentUtils.MapUnClassifiedItems(ft,fs, log) }, // Signature needs update in ShipmentUtils
                {"UpdateSupplierInfo", (log, ft, fs) => ShipmentUtils.UpdateSupplierInfo(ft,fs) }, // Signature needs update in ShipmentUtils
                {"ImportPDF", (log, ft, fs) => InvoiceReader.InvoiceReader.ImportPDF(fs, ft, log) },//PDFUtils.ImportPDF(fs, ft).GetAwaiter().GetResult() }, // Signature needs update in InvoiceReader.InvoiceReader
                {"CreateShipmentEmail", (log, types, infos) => ShipmentUtils.CreateShipmentEmail(types, infos, log) }, // Signature needs update in ShipmentUtils
                //{"SaveAttachments",(log, ft, fs) => SaveAttachments(fs, ft) }, // Signature needs update if uncommented

                //{"AttachToDocSetByRef", (log, ft, fs) => AttachToDocSetByRef(ft.AsycudaDocumentSetId) }, // Signature needs update if uncommented


                {"SyncConsigneeInDB", (log, types, infos) => EntryDocSetUtils.SyncConsigneeInDB(types, infos, log) }, // Signature needs update in EntryDocSetUtils

                {"ClearDocSetEntries",(log, ft, fs) => EntryDocSetUtils.ClearDocSetEntries(log ,ft) }, // Signature needs update in EntryDocSetUtils

                {"SubmitDocSetUnclassifiedItems",(log, ft, fs) => ShipmentUtils.SubmitDocSetUnclassifiedItems(ft, log) }, // Signature needs update in ShipmentUtils
                {"AllocateDocSetDiscrepancies",(log, ft, fs) => DISUtils.AllocateDocSetDiscrepancies(ft, log) }, // Signature needs update in DISUtils
                {"CleanupDocSetDiscpancies",(log, ft, fs) => DISUtils.CleanupDocSetDiscpancies(ft, log) }, // Signature needs update in DISUtils
                {"RecreateDocSetDiscrepanciesEntries", (log, ft, fs) => DISUtils.RecreateDocSetDiscrepanciesEntries(ft, log ) }, // Signature needs update in DISUtils
                {"ExportDocSetDiscpancyEntries", (log, ft, fs) => DISUtils.ExportDocSetDiscpancyEntries("DIS",ft) }, // Signature needs update in DISUtils
                {"SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms", (log, ft, fs) => DISUtils.SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(ft, log) }, // Signature needs update in DISUtils
                {"AssessDiscrepancyExecutions", (log, ft, fs) => DISUtils.AssessDiscrepancyExecutions(ft, fs, log) }, // Signature needs update in DISUtils
                {"AttachEmailPDF", (log, ft, fs) => PDFUtils.AttachEmailPDF(ft, fs) }, // Signature needs update in PDFUtils
                {"ReSubmitDiscrepanciesToCustoms", (log, types, infos) => DISUtils.ReSubmitDiscrepanciesToCustoms(types, infos, log)
                }, // Signature needs update in DISUtils
                {"ReSubmitSalesToCustoms", (log, types, infos) => SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms(types, infos, log)
                }, // Signature needs update in SubmitSalesToCustomsUtils


                {"SubmitMissingInvoices",  (log, ft, fs) => Utils.SubmitMissingInvoices(ft, log) }, // Signature needs update in Utils
                {"SubmitIncompleteEntryData",(log, ft, fs) => Utils.SubmitIncompleteEntryData(ft, log) }, // Signature needs update in Utils
                {"SubmitUnclassifiedItems",(log, ft, fs) => ShipmentUtils.SubmitUnclassifiedItems(ft, log) }, // Signature needs update in ShipmentUtils
                {"SubmitInadequatePackages",(log, ft, fs) => ShipmentUtils.SubmitInadequatePackages(ft, log) }, // Signature needs update in ShipmentUtils
                {"SubmitIncompleteSuppliers",(log, ft, fs) => ShipmentUtils.SubmitIncompleteSuppliers(ft, log) }, // Signature needs update in ShipmentUtils
                {"CreateC71",(log, ft, fs) => C71Utils.CreateC71(ft) }, // Signature needs update in C71Utils
                {"CreateLicense", (log, ft, fs) =>  LICUtils.CreateLicence(ft)}, // Signature needs update in LICUtils
                { "AssessC71",(log, ft, fs) => C71Utils.AssessC71(ft) }, // Signature needs update in C71Utils
                {"AssessLicense",(log, ft, fs) => LICUtils.AssessLicense(ft) }, // Signature needs update in LICUtils
                {"DownLoadC71", (log, ft, fs) => C71Utils.DownLoadC71(ft) }, // Signature needs update in C71Utils
                {"DownLoadLicense", (log, ft, fs) => LICUtils.DownLoadLicence(false, ft) }, // Signature needs update in LICUtils
                { "ImportC71", (log, ft, fs) => C71Utils.ImportC71(ft, log) }, // Signature needs update in C72Utils
                {"ImportLicense", (log, ft, fs) => LICUtils.ImportLicense(ft) }, // Signature needs update in LICUtils

                { "AttachToDocSetByRef",(log, ft, fs) => EntryDocSetUtils.AttachToDocSetByRef(log, ft) }, // Signature needs update in EntryDocSetUtils

                {"AssessPOEntries", (log, ft, fs) => POUtils.AssessPOEntries(ft, log) }, // Signature needs update in POUtils
                {"AssessDiscpancyEntries", (log, ft, fs) => DISUtils.AssessDiscpancyEntries(ft, fs, log) }, // Signature needs update in DISUtils
                {"DeletePONumber", (log, ft, fs) => POUtils.DeletePONumber(ft, fs, log) }, // Signature needs update in POUtils
                { "SubmitPOs", (log, ft, fs) => POUtils.SubmitPOs(log) }, // Signature needs update in POUtils
                {"SubmitEntryCIF", (log, types, infos) => EntryDocSetUtils.SubmitEntryCIF(log,types, infos) }, // Signature needs update in EntryDocSetUtils
                {"SubmitBlankLicenses", (log, ft, fs) => LICUtils.SubmitBlankLicenses(ft, log) }, // Signature needs update in LICUtils
                {"ProcessUnknownCSVFileType", (log, ft,fs) => CSVUtils.ProcessUnknownCSVFileType(ft, fs) }, // Signature needs update in CSVUtils
               // {"ProcessUnknownPDFFileType", (log, ft,fs) => PDFUtils.ProcessUnknownPDFFileType(ft, fs) }, // Signature needs update if uncommented
                {"ImportUnAttachedSummary", (log, ft, fs) => ShipmentUtils.ImportUnAttachedSummary(ft, fs, log) }, // Signature needs update in ShipmentUtils

                {"RemoveDuplicateEntries", (log, ft,fs) => EntryDocSetUtils.RemoveDuplicateEntries(log) }, // Signature needs update in EntryDocSetUtils
                {"FixIncompleteEntries", (log, ft, fs) => EntryDocSetUtils.FixIncompleteEntries(log) }, // Signature needs update in EntryDocSetUtils
                {"EmailWarehouseErrors", (log, ft, fs) => EntryDocSetUtils.EmailWarehouseErrors(log, ft, fs) }, // Signature needs update in EntryDocSetUtils
                {"ImportExpiredEntires", (log, ft, fs) => EntryDocSetUtils.ImportExpiredEntires(log) }, // Signature needs update in EntryDocSetUtils
                {"ImportCancelledEntries", (log, ft, fs) => EntryDocSetUtils.ImportCancelledEntries(log) }, // Signature needs update in EntryDocSetUtils
                {"EmailEntriesExpiringNextMonth", (log, ft, fs) => EntryDocSetUtils.EmailEntriesExpiringNextMonth(log) }, // Signature needs update in EntryDocSetUtils
                {"RecreateEx9", (log, types, infos) => EX9Utils.RecreateEx9(log, types, infos) },// // Signature needs update in EX9Utils
                {"UpdateRegEx", (log, ft, fs) => UpdateInvoice.UpdateRegEx(ft,fs, log) }, // Signature needs update in UpdateInvoice
                {"ImportWarehouseErrors", (log, ft, fs) => ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1, log)}, // Signature needs update in ImportWarehouseErrorsUtils
                {"Kill", (log, ft, fs) => Utils.Kill(ft, fs) }, // Signature needs update in Utils
                {"Continue", (log, ft, fs) => { return Task.Run(() => { });}}, // No change needed in lambda body
                {"LinkPDFs", (log, ft,fs) => PDFUtils.LinkPDFs()}, // Signature needs update in PDFUtils
                {"DownloadPOFiles", (log, ft,fs) => EX9Utils.DownloadSalesFiles(log, 10, "IM7", false)}, // Signature needs update in EX9Utils
                {"ReDownloadPOFiles", (log, ft,fs) => EX9Utils.DownloadSalesFiles(log, 10, "IM7", true)}, // Signature needs update in EX9Utils
                {"SubmitDiscrepanciesToCustoms", (log, types, infos) => DISUtils.SubmitDiscrepanciesToCustoms(types, infos, log) }, // Signature needs update in DISUtils
                {"ClearShipmentData", (log, ft, fs) => ShipmentUtils.ClearShipmentData(ft, fs) }, // Signature needs update in ShipmentUtils
                {"ImportPOEntries", (log, ft, fs) => DocumentUtils.ImportPOEntries(log,false) }, // Signature needs update in DocumentUtils
                {"ImportAllAsycudaDocumentsInDataFolder", (log, ft,fs) => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false, log) }, // Signature needs update in ImportAllAsycudaDocumentsInDataFolderUtils
                {"ImportEntries",(log, ft, fs) => DocumentUtils.ImportEntries(false, ft.Data.ToString(), log) }, // Signature needs update in DocumentUtils
                {"ImportShipmentInfoFromTxt", (log, types, infos) => ShipmentUtils.ImportShipmentInfoFromTxt(types, infos, log) }, // Added mapping for new action, Signature needs update in ShipmentUtils


            };
            // Removed extra closing brace here
    }
}