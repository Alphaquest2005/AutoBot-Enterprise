using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoBotUtilities;
using AutoBotUtilities.CSV;
using WaterNut.Business.Services.Importers;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class FileUtils
    {
        public static Dictionary<string, Func<FileTypes, FileInfo[], Task>> FileActions =>
            new Dictionary<string, Func<FileTypes, FileInfo[], Task>>(WaterNut.DataSpace.Utils.ignoreCase)
            {
                {"ImportSalesEntries",(ft, fs) => DocumentUtils.ImportSalesEntries(false) },
                {"AllocateSales",(ft, fs) => AllocateSalesUtils.AllocateSales() },
                {"CreateEx9",(ft, fs) => CreateEX9Utils.CreateEx9(false, -1) },
                {"ExportEx9Entries",(ft, fs) => EX9Utils.ExportEx9Entries(-1) },
                {"AssessEx9Entries",(ft, fs) => EX9Utils.AssessEx9Entries(-1) },
                {"SaveCsv", (ft, fs) => CSVUtils.SaveCsv(fs, ft) }, // Corrected namespace qualification
                {"ReplaceCSV",(ft, fs) => CSVUtils.ReplaceCSV(fs, ft) },
                {"RecreatePOEntries",(ft, fs) => POUtils.RecreatePOEntries(ft.AsycudaDocumentSetId) },
                {"ExportPOEntries",(ft, fs) => POUtils.ExportPOEntries(ft.AsycudaDocumentSetId) },
                {"AssessPOEntry", (ft, fs) => POUtils.AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId)},
                {"EmailPOEntries", (ft, fs) => POUtils.EmailPOEntries(ft.AsycudaDocumentSetId) },
                {"DownloadSalesFiles",(ft, fs) => EX9Utils.DownloadSalesFiles(10, "IM7History",false) },
                {"Xlsx2csv",  (ft, fs) => XLSXProcessor.Xlsx2csv(fs, new List < FileTypes >() { ft }) },
                {"SaveInfo", (ft, fs) => EmailTextProcessor.Execute(fs, ft) },
                {"CleanupEntries",(ft, fs) => EntryDocSetUtils.CleanupEntries() },
                {"SubmitToCustoms",(ft, fs) => SubmitSalesXmlToCustomsUtils.SubmitSalesXMLToCustoms(-1) },
                {"MapUnClassifiedItems", (ft, fs) => ShipmentUtils.MapUnClassifiedItems(ft,fs) },
                {"UpdateSupplierInfo", (ft, fs) => ShipmentUtils.UpdateSupplierInfo(ft,fs) },
                {"ImportPDF", (ft, fs) => InvoiceReader.InvoiceReader.ImportPDF(fs, ft) },//PDFUtils.ImportPDF(fs, ft).GetAwaiter().GetResult() },
                {"CreateShipmentEmail", (types, infos) => ShipmentUtils.CreateShipmentEmail(types, infos) },
                //{"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
                
                //{"AttachToDocSetByRef", (ft, fs) => AttachToDocSetByRef(ft.AsycudaDocumentSetId) },


                {"SyncConsigneeInDB", (types, infos) => EntryDocSetUtils.SyncConsigneeInDB(types, infos) },

                {"ClearDocSetEntries",(ft, fs) => EntryDocSetUtils.ClearDocSetEntries(ft) },
               
                {"SubmitDocSetUnclassifiedItems",(ft, fs) => ShipmentUtils.SubmitDocSetUnclassifiedItems(ft) },
                {"AllocateDocSetDiscrepancies",(ft, fs) => DISUtils.AllocateDocSetDiscrepancies(ft) },
                {"CleanupDocSetDiscpancies",(ft, fs) => DISUtils.CleanupDocSetDiscpancies(ft) },
                {"RecreateDocSetDiscrepanciesEntries", (ft, fs) => DISUtils.RecreateDocSetDiscrepanciesEntries(ft) },
                {"ExportDocSetDiscpancyEntries", (ft, fs) => DISUtils.ExportDocSetDiscpancyEntries("DIS",ft) },
                {"SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms", (ft, fs) => DISUtils.SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(ft) },
                {"AssessDiscrepancyExecutions", (ft, fs) => DISUtils.AssessDiscrepancyExecutions(ft, fs) },
                {"AttachEmailPDF", PDFUtils.AttachEmailPDF },
                {"ReSubmitDiscrepanciesToCustoms", (types, infos) => DISUtils.ReSubmitDiscrepanciesToCustoms(types, infos)
                },
                {"ReSubmitSalesToCustoms", (types, infos) => SubmitSalesToCustomsUtils.ReSubmitSalesToCustoms(types, infos)
                },


                {"SubmitMissingInvoices",  (ft, fs) => Utils.SubmitMissingInvoices(ft) },
                {"SubmitIncompleteEntryData",(ft, fs) => Utils.SubmitIncompleteEntryData(ft) },
                {"SubmitUnclassifiedItems",(ft, fs) => ShipmentUtils.SubmitUnclassifiedItems(ft) },
                {"SubmitInadequatePackages",(ft, fs) => ShipmentUtils.SubmitInadequatePackages(ft) },
                {"SubmitIncompleteSuppliers",(ft, fs) => ShipmentUtils.SubmitIncompleteSuppliers(ft) },
                {"CreateC71",(ft, fs) => C71Utils.CreateC71(ft) },
                {"CreateLicense", (ft, fs) =>  LICUtils.CreateLicence(ft)},
                { "AssessC71",(ft, fs) => C71Utils.AssessC71(ft) },
                {"AssessLicense",(ft, fs) => LICUtils.AssessLicense(ft) },
                {"DownLoadC71", (ft, fs) => C71Utils.DownLoadC71(ft) },
                {"DownLoadLicense", (ft, fs) => LICUtils.DownLoadLicence(false, ft) },
                { "ImportC71", (ft, fs) => C71Utils.ImportC71(ft) },
                {"ImportLicense", (ft, fs) => LICUtils.ImportLicense(ft) },
               
                { "AttachToDocSetByRef",(ft, fs) => EntryDocSetUtils.AttachToDocSetByRef(ft) },
                
                {"AssessPOEntries", (ft, fs) => POUtils.AssessPOEntries(ft) },
                {"AssessDiscpancyEntries", (ft, fs) => DISUtils.AssessDiscpancyEntries(ft, fs) },
                {"DeletePONumber", POUtils.DeletePONumber },
                { "SubmitPOs", (ft, fs) => POUtils.SubmitPOs() },
                {"SubmitEntryCIF", (types, infos) => EntryDocSetUtils.SubmitEntryCIF(types, infos) },
                {"SubmitBlankLicenses", (ft, fs) => LICUtils.SubmitBlankLicenses(ft) },
                {"ProcessUnknownCSVFileType", (ft,fs) => CSVUtils.ProcessUnknownCSVFileType(ft, fs) },
               // {"ProcessUnknownPDFFileType", (ft,fs) => PDFUtils.ProcessUnknownPDFFileType(ft, fs) },
                {"ImportUnAttachedSummary", (ft, fs) => ShipmentUtils.ImportUnAttachedSummary(ft, fs) },

                {"RemoveDuplicateEntries", (ft,fs) => EntryDocSetUtils.RemoveDuplicateEntries() },
                {"FixIncompleteEntries", (ft, fs) => EntryDocSetUtils.FixIncompleteEntries() },
                {"EmailWarehouseErrors", (ft, fs) => EntryDocSetUtils.EmailWarehouseErrors(ft, fs) },
                {"ImportExpiredEntires", (ft, fs) => EntryDocSetUtils.ImportExpiredEntires() },
                {"ImportCancelledEntries", (ft, fs) => EntryDocSetUtils.ImportCancelledEntries() },
                {"EmailEntriesExpiringNextMonth", (ft, fs) => EntryDocSetUtils.EmailEntriesExpiringNextMonth() },
                {"RecreateEx9", (types, infos) => EX9Utils.RecreateEx9(types, infos) },//
                {"UpdateRegEx", UpdateInvoice.UpdateRegEx},
                {"ImportWarehouseErrors", (ft, fs) => ImportWarehouseErrorsUtils.ImportWarehouseErrors(-1)},
                {"Kill", Utils.Kill},
                {"Continue", (ft, fs) => { return Task.Run(() => { });}},
                {"LinkPDFs", (ft,fs) => PDFUtils.LinkPDFs()},
                {"DownloadPOFiles", (ft,fs) => EX9Utils.DownloadSalesFiles(10, "IM7", false)},
                {"ReDownloadPOFiles", (ft,fs) => EX9Utils.DownloadSalesFiles(10, "IM7", true)},
                {"SubmitDiscrepanciesToCustoms", (types, infos) => DISUtils.SubmitDiscrepanciesToCustoms(types, infos) },
                {"ClearShipmentData", ShipmentUtils.ClearShipmentData},
                {"ImportPOEntries", (ft, fs) => DocumentUtils.ImportPOEntries(false) },
                {"ImportAllAsycudaDocumentsInDataFolder", (ft,fs) => ImportAllAsycudaDocumentsInDataFolderUtils.ImportAllAsycudaDocumentsInDataFolder(false) },
                {"ImportEntries",(ft, fs) => DocumentUtils.ImportEntries(false, ft.Data.ToString()) },
                {"ImportShipmentInfoFromTxt", (types, infos) => ShipmentUtils.ImportShipmentInfoFromTxt(types, infos) }, // Added mapping for new action


            };
            // Removed extra closing brace here
    }
}