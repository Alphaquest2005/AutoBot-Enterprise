﻿using System;
using System.Collections.Generic;
using System.IO;
using AutoBotUtilities.CSV;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class FileUtils
    {
        public static Dictionary<string, Action<FileTypes, FileInfo[]>> FileActions =>
            new Dictionary<string, Action<FileTypes, FileInfo[]>>(Utils.ignoreCase)
            {
                {"ImportSalesEntries",(ft, fs) => SalesUtils.ImportSalesEntries() },
                {"AllocateSales",(ft, fs) => SalesUtils.AllocateSales() },
                {"CreateEx9",(ft, fs) => EX9Utils.CreateEx9(false) },
                {"ExportEx9Entries",(ft, fs) => EX9Utils.ExportEx9Entries() },
                {"AssessEx9Entries",(ft, fs) => EX9Utils.AssessEx9Entries() },
                {"SaveCsv",(ft, fs) => CSVUtils.SaveCsv(fs, ft) },
                {"ReplaceCSV",(ft, fs) => CSVUtils.ReplaceCSV(fs, ft) },
                {"RecreatePOEntries",(ft, fs) => POUtils.RecreatePOEntries(ft.AsycudaDocumentSetId) },
                {"ExportPOEntries",(ft, fs) => POUtils.ExportPOEntries(ft.AsycudaDocumentSetId) },
                {"AssessPOEntry",(ft, fs) => POUtils.AssessPOEntry(ft.DocReference, ft.AsycudaDocumentSetId)},
                {"EmailPOEntries",(ft, fs) => POUtils.EmailPOEntries(ft.AsycudaDocumentSetId) },
                {"DownloadSalesFiles",(ft, fs) => EX9Utils.DownloadSalesFiles(10, "IM7History",false) },
                {"Xlsx2csv",(ft, fs) => XLSXImporter.Xlsx2csv(fs, ft) },
                {"SaveInfo",(ft, fs) => ImportUtils.TrySaveFileInfo(fs, ft) },
                {"CleanupEntries",(ft, fs) => EntryDocSetUtils.CleanupEntries() },
                {"SubmitToCustoms",(ft, fs) => SalesUtils.SubmitSalesXMLToCustoms() },
                {"MapUnClassifiedItems", (ft, fs) => ShipmentUtils.MapUnClassifiedItems(ft,fs) },
                {"UpdateSupplierInfo", (ft, fs) => ShipmentUtils.UpdateSupplierInfo(ft,fs) },
                {"ImportPDF", (ft, fs) => PDFUtils.ImportPDF(fs,ft) },
                {"CreateShipmentEmail", ShipmentUtils.CreateShipmentEmail },
                //{"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
                
                //{"AttachToDocSetByRef", (ft, fs) => AttachToDocSetByRef(ft.AsycudaDocumentSetId) },

                {"ClearDocSetEntries",(ft, fs) => EntryDocSetUtils.ClearDocSetEntries(ft) },
               
                {"SubmitDocSetUnclassifiedItems",(ft, fs) => ShipmentUtils.SubmitDocSetUnclassifiedItems(ft) },
                {"AllocateDocSetDiscrepancies",(ft, fs) => DISUtils.AllocateDocSetDiscrepancies(ft) },
                {"CleanupDocSetDiscpancies",(ft, fs) => DISUtils.CleanupDocSetDiscpancies(ft) },
                {"RecreateDocSetDiscrepanciesEntries",(ft, fs) => DISUtils.RecreateDocSetDiscrepanciesEntries(ft) },
                {"ExportDocSetDiscpancyEntries",(ft, fs) => DISUtils.ExportDocSetDiscpancyEntries("DIS",ft) },
                {"SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms",(ft, fs) => DISUtils.SubmitDocSetDiscrepanciesPreAssessmentReportToCustoms(ft) },
                {"AssessDiscrepancyExecutions", DISUtils.AssessDiscrepancyExecutions },
                {"AttachEmailPDF", PDFUtils.AttachEmailPDF },
                {"ReSubmitDiscrepanciesToCustoms", DISUtils.ReSubmitDiscrepanciesToCustoms },
                {"ReSubmitSalesToCustoms", SalesUtils.ReSubmitSalesToCustoms },


                {"SubmitMissingInvoices",  (ft, fs) => Utils.SubmitMissingInvoices(ft) },
                {"SubmitIncompleteEntryData",(ft, fs) => Utils.SubmitIncompleteEntryData(ft) },
                {"SubmitUnclassifiedItems",(ft, fs) =>  ShipmentUtils.SubmitUnclassifiedItems(ft) },
                {"SubmitInadequatePackages",(ft, fs) => ShipmentUtils.SubmitInadequatePackages(ft) },
                {"SubmitIncompleteSuppliers",(ft, fs) => ShipmentUtils.SubmitIncompleteSuppliers(ft) },
                {"CreateC71",(ft, fs) => C71Utils.CreateC71(ft) },
                {"CreateLicense",(ft, fs) =>  LICUtils.CreateLicence(ft)},
                { "AssessC71",(ft, fs) => C71Utils.AssessC71(ft) },
                {"AssessLicense",(ft, fs) => LICUtils.AssessLicense(ft) },
                {"DownLoadC71", (ft, fs) => C71Utils.DownLoadC71(ft) },
                {"DownLoadLicense", (ft, fs) => LICUtils.DownLoadLicence(false, ft) },
                { "ImportC71", (ft, fs) => C71Utils.ImportC71(ft) },
                {"ImportLicense", (ft, fs) => LICUtils.ImportLicense(ft) },
               
                { "AttachToDocSetByRef",(ft, fs) => EntryDocSetUtils.AttachToDocSetByRef(ft) },
                
                {"AssessPOEntries",(ft, fs) => POUtils.AssessPOEntries(ft) },
                {"AssessDiscpancyEntries", DISUtils.AssessDiscpancyEntries },
                {"DeletePONumber", POUtils.DeletePONumber },
                { "SubmitPOs", POUtils.SubmitPOs },
                {"SubmitEntryCIF", EntryDocSetUtils.SubmitEntryCIF },
                {"SubmitBlankLicenses", (ft,fs) => LICUtils.SubmitBlankLicenses(ft) },
                {"ProcessUnknownCSVFileType", (ft,fs) => CSVUtils.ProcessUnknownCSVFileType(ft, fs) },
                {"ProcessUnknownPDFFileType", (ft,fs) => PDFUtils.ProcessUnknownPDFFileType(ft, fs) },
                {"ImportUnAttachedSummary", (ft,fs) => ShipmentUtils.ImportUnAttachedSummary(ft, fs) },

                {"RemoveDuplicateEntries", EntryDocSetUtils.RemoveDuplicateEntries },
                {"FixIncompleteEntries", EntryDocSetUtils.FixIncompleteEntries },
                {"EmailWarehouseErrors", EntryDocSetUtils.EmailWarehouseErrors },
                {"ImportExpiredEntires", EntryDocSetUtils.ImportExpiredEntires },
                {"ImportCancelledEntries", EntryDocSetUtils.ImportCancelledEntries },
                {"EmailEntriesExpiringNextMonth", EntryDocSetUtils.EmailEntriesExpiringNextMonth },
                {"RecreateEx9", EX9Utils.RecreateEx9 },//
                {"UpdateRegEx", UpdateInvoice.UpdateRegEx},
                {"ImportWarehouseErrors", (ft,fs) => ImportWarehouseErrorsUtils.ImportWarehouseErrors()},
                {"Kill", Utils.Kill},
                {"LinkPDFs", (ft,fs) => PDFUtils.LinkPDFs()},
                {"DownloadPOFiles", (ft,fs) => EX9Utils.DownloadSalesFiles(10, "IM7", false)},
                {"SubmitDiscrepanciesToCustoms", DISUtils.SubmitDiscrepanciesToCustoms}
                


            };
    }
}