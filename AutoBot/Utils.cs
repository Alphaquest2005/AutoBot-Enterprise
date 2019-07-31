using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using AllocationQS.Client.Repositories;
using Asycuda421;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Services;
using EmailDownloader;
using EntryDataQS.Business.Entities;
using ExcelDataReader;
using MoreLinq;
using SalesDataQS.Business.Services;
using SalesDataQS.Client.Repositories;
using TrackableEntities;
using WaterNut.DataSpace;

namespace AutoBot
{
    public class Utils
    {
        public class FileAction
        {

            public string Filetype { get; set; }

            public List<Action<FileTypes, FileInfo[]>> Actions { get; set; } =
                new List<Action<FileTypes, FileInfo[]>>();
        }

        public static Dictionary<string, Action<FileTypes, FileInfo[]>> FileActions =>
            new Dictionary<string, Action<FileTypes, FileInfo[]>>
            {
                {"ImportSalesEntries",(ft, fs) => ImportSalesEntries() },
                {"AllocateSales",(ft, fs) => AllocateSales() },
                {"CreateEx9",(ft, fs) => CreateEx9() },
                {"ExportEx9Entries",(ft, fs) => ExportEx9Entries() },
                {"AssessEx9Entries",(ft, fs) => AssessEx9Entries() },
                {"SaveCsv",(ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId) },
                {"CreatePOEntries",(ft, fs) => CreatePOEntries().Wait() },
                {"ExportPOEntries",(ft, fs) => ExportPOEntries().Wait() },
                {"AssessEntry",(ft, fs) => AssessEntry(ft.DocReference, ft.AsycudaDocumentSetId)},
                {"EmailPOEntries",(ft, fs) => EmailPOEntries(ft.FileTypeContacts.Select(x => x.Contacts).ToList()) },
                {"DownloadSalesFiles",(ft, fs) => DownloadSalesFiles() },
                {"Xlsx2csv",(ft, fs) => Xlsx2csv(fs, ft) },
                {"SaveInfo",(ft, fs) => SaveInfo(fs, ft.AsycudaDocumentSetId) },
                {"CleanupEntries",(ft, fs) => CleanupEntries() },
                {"SubmitToCustoms",(ft, fs) => SubmitSalesXMLToCustoms() },
                //{"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
                


            };

        public static Dictionary<string, Action> SessionActions =>
            new Dictionary<string, Action>
            {

                {"CreateDiscpancyEntries", CreateDiscpancyEntries },
                {"AutoMatch", AutoMatch },
                {"AssessDiscpancyEntries", AssessDiscpancyEntries },
                {"ExportDiscpancyEntries", ExportDiscpancyEntries },
                { "SubmitDiscrepancyErrors", SubmitDiscrepancyErrors },
                {"AllocateSales", AllocateSales },
                {"CreateEx9", CreateEx9 },
                {"ExportEx9Entries", ExportEx9Entries },
                {"AssessEx9Entries", AssessEx9Entries },
                {"SubmitToCustoms", SubmitSalesXMLToCustoms },
                {"CleanupEntries", CleanupEntries },
            };

        private static void SubmitSalesXMLToCustoms()
        {
            try
            {


                Console.WriteLine("Submit XML To Customs");

                // var saleInfo = CurrentSalesInfo();
                var salesinfo = CurrentSalesInfo();

                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "Customs").Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitXMLToCustoms.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.AssessmentDate >= salesinfo.Item1 && x.AssessmentDate <= salesinfo.Item2).ToList()
                        .GroupBy(x =>  x.EmailId );
                    foreach (var emailIds in lst)
                    {
                       
                        

                            var body = "The Following Ex-Warehouse Entries were Assessed. \r\n" +
                                       $"Start Date: {salesinfo.Item1} End Date {salesinfo.Item2}: \r\n" +
                                       "\tCNumber\t\tReference\t\tAssessmentDate\r\n" +
                                       $"{emailIds.Select(current => $"\t{current.CNumber}\t\t{current.ReferenceNumber}\t\t{current.RegistrationDate.Value:yyyy-MM-dd} \r\n").Aggregate((old, current) => old + current)}" +
                                       $"\r\n" +
                                       $"Any questions or concerns please contact Joseph Bartholomew at josephBartholomew@outlook.com.\r\n" +
                                       $"\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";

                            
                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Client, "", "Assessed Ex-Warehouse Entries",
                                contacts, body, new string[0]);
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(Convert.ToInt32(emailIds.Key), Client, "Assessed Ex-Warehouse Entries", body, contacts);
                        }

                        foreach (var item in emailIds)
                        {
                            var sfile = ctx.AsycudaDocuments.FirstOrDefault(x =>
                                x.ASYCUDA_Id == item.ASYCUDA_Id &&
                                x.ApplicationSettingsId == item.ApplicationSettingsId);
                            var eAtt = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                                x.Attachments.FilePath == sfile.SourceFileName);
                            if (eAtt != null)
                            {
                                ctx.AttachmentLog.Add(new AttachmentLog(true)
                                {
                                    DocSetAttachment = eAtt.Id,
                                    Status = "Submit XML To Customs",
                                    TrackingState = TrackingState.Added
                                });
                            }
                        }
                        
                        ctx.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void AutoMatch()
        {
            Console.WriteLine("AutoMatch ...");
            new AdjustmentShortService()
                .AutoMatch(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();
        }

        public static void CleanupEntries()
        {
            Console.WriteLine("AutoMatch ...");
            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_DocumentsToDelete
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.ASYCUDA_Id).ToList();
                foreach (var itm in lst)
                {
                    BaseDataModel.Instance.DeleteAsycudaDocument(itm).Wait();
                }

                var doclst = ctx.TODO_DeleteDocumentSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Select(x => x.AsycudaDocumentSetId).ToList();


                foreach (var itm in doclst)
                {
                    BaseDataModel.Instance.DeleteAsycudaDocumentSet(itm).Wait();
                }

                ctx.Database.ExecuteSqlCommand(@"delete from entrydata where entrydataid in 
                    (SELECT EntryData.EntryDataId
                    FROM    EntryData LEFT OUTER JOIN
                AsycudaDocumentSetEntryData ON EntryData.EntryDataId = AsycudaDocumentSetEntryData.EntryDataId
                WHERE(AsycudaDocumentSetEntryData.AsycudaDocumentSetId IS NULL))");

            }
        }


        public static void EmailSalesErrors()
        {

            var info = CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "SalesErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                .Where(x => x.Status != null)
                                .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>();
                res.dataToPrint = errors;
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Sales").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Sales Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile });
            }

        }
        public static void EmailAdjustmentErrors()
        {

            var info = CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "AdjustmentErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                    .Where(x => x.Type == "ADJ")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Status != null)
                    .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>();
                res.dataToPrint = errors;
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.FileTypes.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Where(x => x.Type == "Sales").SelectMany(x => x.FileTypeContacts.Select(z => z.Contacts)).ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Adjustment Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile });
            }

        }
        public static void EmailPOEntries(List<Contacts> contacts)
        {
            if (!contacts.Any()) return;
            var lst = CurrentPOInfo();
            foreach (var poInfo in lst)
            {

                if (poInfo.Item1 == null) return;
                var reference = poInfo.Item1.Declarant_Reference_Number;
                var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, reference);
                if (!Directory.Exists(directory)) continue;
                var sourcefiles = Directory.GetFiles(directory, "*.xml");

                var emailres = new FileInfo(Path.Combine(directory, "EmailResults.txt"));
                var instructions = new FileInfo(Path.Combine(directory, "Instructions.txt"));
                if (!instructions.Exists) return;
                Console.WriteLine("Emailing Po Entries");
                string[] files;
                if (emailres.Exists)
                {
                    var eRes = File.ReadAllLines(emailres.FullName);
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = sourcefiles.Where(x => insts.Any(z => z.Contains(x)) && !eRes.Any(z => z.Contains(x))).ToArray();
                }
                else
                {
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = sourcefiles.ToList().Where(x => insts.Any(z => z.Contains(x))).ToArray();
                }
                if (files.Length > 0)
                    EmailDownloader.EmailDownloader.SendEmail(Client, directory, $"Entries for {reference}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", files);

            }
        }

        public static void DownloadSalesFiles()
        {
            try

            {
                var directoryName = CurrentSalesInfo().Item4;
                Console.WriteLine("Download Entries");
                var lcont = 0;
                while (ImportComplete(directoryName, out lcont) == false)
                {
                    RunSiKuLi(CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void DownloadSalesFiles(int trytimes)
        {
            try

            {
                var directoryName = CurrentSalesInfo().Item4;
                Console.WriteLine("Download Entries");
                var lcont = 0;
                
                for (int i = 0; i < trytimes; i++)
                {
                    ImportComplete(directoryName, out lcont);
                    RunSiKuLi(CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AssessEx9Entries()
        {
            Console.WriteLine("Assessing Ex9 Entries");
            var saleinfo = CurrentSalesInfo();
            AssessEntry(saleinfo.Item3.Declarant_Reference_Number, saleinfo.Item3.AsycudaDocumentSetId);
        }

        public static void AssessEntry(string docReference, int asycudaDocumentSetId)
        {

            if (docReference == null) return;
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                docReference, "Instructions.txt");

            var lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
            }

            lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                RunSiKuLi(CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
            }

            ImportSalesEntries();
            CleanupEntries();

        }

        public static bool ImportComplete(string directoryName, out int lcont)
        {
            lcont = 0;

            var desFolder = directoryName + "\\";
            if (File.Exists(Path.Combine(desFolder, "OverView.txt")))
            {
                var lines = File.ReadAllText(Path.Combine(directoryName, "OverView.txt"))
                    .Split(new[] { $"\r\n{DateTime.Now.Year}\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }


                foreach (var line in lines)
                {
                    lcont += 1;

                    var p = line.StartsWith($"{DateTime.Now.Year}\t")
                        ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                        : line.Split('\t');
                    if (File.Exists(Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{p[5]}.xml"))) continue;
                    return false;
                }

                return true;
            }
            else
            {

                return false;
            }
        }
        public static bool AssessComplete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;


            if (File.Exists(instrFile))
            {
                if (!File.Exists(resultsFile)) return false;
                var lines = File.ReadAllLines(instrFile);
                var res = File.ReadAllLines(resultsFile);
                if (res.Length == 0)
                {

                    return false;
                }


                foreach (var line in lines)
                {
                    var p = line.Split('\t');
                    if (lcont >= res.Length) return false;
                    var r = res[lcont].Split('\t');
                    lcont += 1;
                    if (p[1] == r[1] && r.Length == 3 && r[2] == "Success") continue;
                    return false;
                }

                return true;
            }
            else
            {

                return true;
            }
        }

        public static void ImportSalesEntries()
        {
            Console.WriteLine("Import Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docSetId = CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                var ft = ctx.FileTypes.FirstOrDefault(x => x.Type == "XML" && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (ft == null) return;
                var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                        .Declarant_Reference_Number);
                var csvFiles = new DirectoryInfo(desFolder).GetFiles().Where(x => Regex.IsMatch(x.FullName, ft.FilePattern, RegexOptions.IgnoreCase)).ToArray();
                if (csvFiles.Length > 0)
                    BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId, csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();
            }
        }

        public static void CreateEx9()
        {
            Console.WriteLine("Create Ex9");
           
            var saleInfo = CurrentSalesInfo();

            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.Database.SqlQuery<string>(
                    $@"SELECT EX9AsycudaSalesAllocations.ItemNumber
                    FROM    EX9AsycudaSalesAllocations INNER JOIN
                                     ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                                     EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                                     AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
                    WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                                     (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                                     EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                                     (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) AND (EX9AsycudaSalesAllocations.DocumentType = 'IM7' OR
                                     EX9AsycudaSalesAllocations.DocumentType = 'OS7') AND (AllocationErrors.ItemNumber IS NULL) AND (ApplicationSettings.ApplicationSettingsId = {BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}) AND 
                                     (EX9AsycudaSalesAllocations.InvoiceDate >= '{saleInfo.Item1.ToShortDateString()}' and EX9AsycudaSalesAllocations.InvoiceDate <= '{saleInfo.Item2.ToShortDateString()}')
                    GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId, EX9AsycudaSalesAllocations.pQuantity, EX9AsycudaSalesAllocations.PreviousItem_Id
                    HAVING (SUM(EX9AsycudaSalesAllocations.PiQuantity) < SUM(EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0)");
                if (!res.Any()) return;
            }


            var filterExpression =
                $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                $"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                $" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                //  $"&& (AllocationErrors == null)" +// || (AllocationErrors.EntryDataDate  >= \"{saleInfo.Item1:MM/01/yyyy}\" &&  AllocationErrors.EntryDataDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\"))" +
                "&& (TaxAmount == 0 || TaxAmount != 0)" +
                "&& PreviousItem_Id != null" +
                "&& (xBond_Item_Id == 0)" +
                "&& (QtyAllocated != null && EntryDataDetailsId != null)" +
                "&& (PiQuantity < pQtyAllocated)" +
                "&& (Status == null || Status == \"\")" +
                (BaseDataModel.Instance.CurrentApplicationSettings.AllowNonXEntries == "Visible"
                    ? $"&& (Invalid != true && (pExpiryDate >= \"{DateTime.Now.ToShortDateString()}\" || pExpiryDate == null) && (Status == null || Status == \"\"))"
                    : "") +
                (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate.HasValue
                    ? $" && pRegistrationDate >= \"{BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate}\""
                    : "");

            if (saleInfo.Item3.AsycudaDocumentSetId == 0) return;
            var docset = BaseDataModel.Instance.GetAsycudaDocumentSet(saleInfo.Item3.AsycudaDocumentSetId).Result;

            AllocationsModel.Instance.CreateEX9Class.CreateEx9(filterExpression, false, false, false, docset).Wait();
        }

        public static void CreateDiscpancyEntries()
        {
            Console.WriteLine("Create DIS Short Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_AdjustmentsToXML.Where(x =>
                    x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                    && x.AdjustmentType == "DIS"
                        //    && x.InvoiceNo == "118965"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.InvoiceNo }).ToList();
                foreach (var doc in lst)
                {
                    var filterExpression =
                        $"(ApplicationSettingsId == \"{BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId}\")" +
                        //$"&& (InvoiceDate >= \"{saleInfo.Item1:MM/01/yyyy}\" " +
                        //$" && InvoiceDate <= \"{saleInfo.Item2:MM/dd/yyyy HH:mm:ss}\")" +
                        $" && EntryDataId == \"{doc.Key.InvoiceNo}\"";
                    new AdjustmentShortService().CreateIM9(filterExpression, true, false, doc.Key.AsycudaDocumentSetId, "IM4-801", "Duty Paid").Wait();
                    new AdjustmentOverService().CreateOPS(filterExpression, true, doc.Key.AsycudaDocumentSetId).Wait();

                }

            }

        }

        public static void ExportDiscpancyEntries()
        {
            Console.WriteLine("Export Discrepancy Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_CreateDiscrepancyEntries.Where(x =>
                    x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                    && x.AdjustmentType == "DIS"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                foreach (var doc in lst)
                {

                    BaseDataModel.Instance.ExportDocSet(doc.Key.AsycudaDocumentSetId,
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Key.Declarant_Reference_Number), false).Wait();
                    ExportDocSetSalesReport(doc.Key.AsycudaDocumentSetId,
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            doc.Key.Declarant_Reference_Number)).Wait();

                }

            }

        }

        public static void AssessDiscpancyEntries()
        {
            Console.WriteLine("Assess Discrepancy Entries");

            // var saleInfo = CurrentSalesInfo();


            using (var ctx = new CoreEntitiesContext())
            {
                var lst = ctx.TODO_CreateDiscrepancyEntries.Where(x =>
                    x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                    && x.AdjustmentType == "DIS"
                        //&& x.InvoiceDate >= saleInfo.Item1
                        //    &&  x.InvoiceDate <= saleInfo.Item2
                        )
                    .GroupBy(x => new { x.AsycudaDocumentSetId, x.Declarant_Reference_Number }).ToList();
                foreach (var doc in lst)
                {

                    AssessEntry(doc.Key.Declarant_Reference_Number, doc.Key.AsycudaDocumentSetId);
                }

            }

        }

        public static void SubmitDiscrepancyErrors()
        {
            try
            {


                Console.WriteLine("Submit Discrepancy Entries");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var lst = ctx.TODO_DiscrepanciesToSubmit.Where(x =>
                                x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings
                                    .ApplicationSettingsId
                                && x.AdjustmentType == "DIS").ToList()
                        .GroupBy(x => new { x.AsycudaDocumentSetId, x.EmailId });
                    foreach (var doc in lst)
                    {
                        var emailIds = ctx.AsycudaDocumentSet_Attachments
                            .Where(x => x.AsycudaDocumentSetId == doc.Key.AsycudaDocumentSetId &&
                                        x.Attachments.FilePath.EndsWith(".xlsx"))
                            .Where(x => x.AttachmentLog.All(z => z.Status != "Submit Discrepancy Errors"))
                            .ToList();
                        foreach (var eId in emailIds)
                        {

                            var body = "Discrepancies Found: \r\n" +
                                       "System could not find the following items on the CNumbers Stated: \r\n" +
                                       "\tItem Number\t\tInvoiceQty\t\tRecieved Qty\t\tCNumber\r\n" +
                                       $"{doc.Select(current => $"\t{current.ItemNumber}\t\t{current.InvoiceQty}\t\t\t\t{current.ReceivedQty}\t\t{current.CNumber}\r\n").Aggregate((old, current) => old + current)}" +
                                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                            if (!EmailDownloader.EmailDownloader.SendBackMsg(Convert.ToInt32(eId.EmailUniqueId), Client,
                                body)) continue;
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = eId.Id,
                                Status = "Submit Discrepancy Errors",
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();


                        }


                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public static void SetCurrentApplicationSettings(int id)
        {
            using (var ctx = new CoreEntitiesContext() { })
            {


                var appSetting = ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes)
                    .Include("FileTypes.FileTypeContacts.Contacts")
                    .Include("FileTypes.FileTypeActions.Actions")
                    .Include(x => x.EmailMapping)

                    .Include("FileTypes.FileTypeMappings").First(x => x.ApplicationSettingsId == id);
                
                    // set BaseDataModel CurrentAppSettings
                    BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                    //check emails
                
            }
        }

        public static Tuple<DateTime, DateTime, AsycudaDocumentSetEx, string> CurrentSalesInfo()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23);
            var docRef = startDate.ToString("MMMM") + " " + startDate.Year.ToString();
            var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
            
            var dirPath = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docRef);
            return new Tuple<DateTime, DateTime, AsycudaDocumentSetEx, string>(startDate, endDate, docSet, dirPath);
        }

        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo()
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var poDocSet = ctx.TODO_PODocSet.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (poDocSet != null)
                {
                    var docSet = ctx.AsycudaDocumentSetExs.Where(x =>
                         poDocSet.Any(z => z.AsycudaDocumentSetId == x.AsycudaDocumentSetId));
                    var lst = new List<Tuple<AsycudaDocumentSetEx, string>>();
                    foreach (var item in docSet)
                    {

                        var dirPath = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            item.Declarant_Reference_Number);
                        lst.Add(new Tuple<AsycudaDocumentSetEx, string>(item, dirPath));
                    }
                    return lst;
                }
                return new List<Tuple<AsycudaDocumentSetEx, string>>();
            }
        }

        public static void AllocateSales()
        {
            Console.WriteLine("Allocations Started");
            using (var ctx = new CoreEntitiesContext())
            {
                if (!ctx.TODO_UnallocatedSales.Any(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)) return;
                AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings
                    .ApplicationSettingsId).Wait();
                AllocationsBaseModel.Instance
                    .AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false)
                    .Wait();
                EmailSalesErrors();
            }
        }

        public static void AllocateShorts()
        {
            try
            {
                return;
                Console.WriteLine("Short Allocations Started");
                using (var ctx = new CoreEntitiesContext())
                {
                    if (!ctx.TODO_UnallocatedShorts.Any(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)) return;
                    AllocationsModel.Instance.ClearAllAllocations(BaseDataModel.Instance.CurrentApplicationSettings
                        .ApplicationSettingsId).Wait();
                    AllocationsBaseModel.Instance
                        .AllocateSales(BaseDataModel.Instance.CurrentApplicationSettings, false)
                        .Wait();
                    EmailAdjustmentErrors();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task CreatePOEntries()
        {
            Console.WriteLine("CreatePOEntries");
            try
            {
                using (var ctx = new EntryDataQSContext())
                {
                    var res = ctx.ToDo_POToXML.Where(x =>
                            x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .Select(x => new
                        {
                            DocSetId = x.Key,
                            Entrylst = x.Select(z => new { z.EntryDataDetailsId, z.IsClassified }).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        //BaseDataModel.Instance.AddToEntry(
                        //        docSetId.Entrylst.Where(x => x.IsClassified == true).Select(x => x.EntryDataDetailsId),
                        //        docSetId.DocSetId,
                        //        BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true).Wait();
                        //BaseDataModel.Instance.AddToEntry(
                        //        docSetId.Entrylst.Where(x => x.IsClassified == false).Select(x => x.EntryDataDetailsId),
                        //        docSetId.DocSetId,
                        //        BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true)
                        //    .Wait();

                        BaseDataModel.Instance.AddToEntry(
                               docSetId.Entrylst.Select(x => x.EntryDataDetailsId),
                               docSetId.DocSetId,
                               BaseDataModel.Instance.CurrentApplicationSettings.InvoicePerEntry ?? true).Wait();

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task ExportPOEntries()
        {
            Console.WriteLine("Export PO Entries");
            try
            {
                using (var ctx = new DocumentDS.Business.Entities.DocumentDSContext())
                {
                    var res = ctx.xcuda_ASYCUDA
                        .Include(x => x.xcuda_Declarant)
                        .Where(x => x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                   && x.xcuda_ASYCUDA_ExtendedProperties.ImportComplete == false
                                   && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type.Type_of_declaration == "IM" 
                                        && x.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.Document_Type.Declaration_gen_procedure_code == "7")
                        .GroupBy(x => new { x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.AsycudaDocumentSetId, ReferenceNumber = x.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Declarant_Reference_Number })
                        .Select(x => new
                        {
                            DocSet = x.Key,
                            Entrylst = x.Select(z => new { z, ReferenceNumber = z.xcuda_Declarant.Number }).ToList()
                        })
                        .ToList();


                    foreach (var docSetId in res)
                    {
                        var directoryName = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                            docSetId.DocSet.ReferenceNumber);
                        if (!Directory.Exists(directoryName)) continue;
                        foreach (var item in docSetId.Entrylst)
                        {
                            var expectedfileName = Path.Combine(directoryName, item.ReferenceNumber + ".xml");
                            if (File.Exists(expectedfileName)) continue;
                            BaseDataModel.Instance.ExportDocument(expectedfileName, item.z).Wait();

                        }                    
                        
                        
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void ExportEx9Entries()
        {
            Console.WriteLine("Export EX9 Entries");
            try
            {
                var i = CurrentSalesInfo();
                BaseDataModel.Instance.ExportDocSet(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number), false).Wait();
                ExportDocSetSalesReport(i.Item3.AsycudaDocumentSetId,
                    Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                        i.Item3.Declarant_Reference_Number)).Wait();

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task ExportDocSetSalesReport(int asycudaDocumentSetId, string folder)
        {
            var doclst =
                await
                    new SalesDataService().GetSalesDocuments(
                        asycudaDocumentSetId)
                        .ConfigureAwait(false);
            if (doclst == null || !doclst.ToList().Any()) return;


            var exceptions = new ConcurrentQueue<Exception>();

            using (var sta = new StaTaskScheduler(numberOfThreads: 1))
            {

                await Task.Factory.StartNew(() =>
                {
                    var s = new ExportToCSV<SaleReportLine, List<SaleReportLine>>();
                    s.StartUp();
                    foreach (var doc in doclst)
                    {
                        try
                        {
                            var data = GetDocumentSalesReport(doc.ASYCUDA_Id).Result;
                            if (data != null)
                            {
                                string path = Path.Combine(folder,
                                    !string.IsNullOrEmpty(doc.CNumber) ? doc.CNumber : doc.ReferenceNumber + ".csv.pdf");
                                s.dataToPrint = data.ToList();
                                s.SaveReport(path);
                            }
                            else
                            {
                                File.Create(Path.Combine(folder, doc.CNumber ?? doc.ReferenceNumber + ".csv.pdf"));
                            }

                        }
                        catch (Exception ex)
                        {
                            exceptions.Enqueue(ex);
                        }
                    }
                    s.ShutDown();
                },
                    CancellationToken.None, TaskCreationOptions.None, sta).ConfigureAwait(false);
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);
        }

        public static async Task<IEnumerable<SaleReportLine>> GetDocumentSalesReport(int ASYCUDA_Id)
        {
            var alst = await Ex9SalesReport(ASYCUDA_Id).ConfigureAwait(false);
            if (alst.Any()) return alst;

            alst = IM9AdjustmentsReport(ASYCUDA_Id);
            return alst;
        }
        public class SaleReportLine
        {
            public int Line { get; set; }
            public DateTime Date { get; set; }
            public string InvoiceNo { get; set; }
            public string CustomerName { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public double SalesQuantity { get; set; }

            public double SalesFactor { get; set; }
            public double xQuantity { get; set; }
            public double Price { get; set; }
            public string SalesType { get; set; }
            public double GrossSales { get; set; }
            public string PreviousCNumber { get; set; }
            public string PreviousLineNumber { get; set; }
            public string PreviousRegDate { get; set; }
            public double CIFValue { get; set; }
            public double DutyLiablity { get; set; }
            public string Comment { get; set; }
        }

        public static ObservableCollection<SaleReportLine> IM9AdjustmentsReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AllocationQSContext())
                {
                    var alst =
                         ctx.AdjustmentShortAllocations.Where(x => x.xASYCUDA_Id == ASYCUDA_Id
                                                                         && x.EntryDataDetailsId != 0
                                                                         && x.PreviousItem_Id != 0
                                                                         && x.pRegistrationDate != null).ToList();

                    //    $"xASYCUDA_Id == {} " + "&& EntryDataDetailsId != null " +
                    //    "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                    //.ConfigureAwait(false)).ToList();

                    var d =
                        alst.Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 17) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                Comment = s.Comment,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = "Duty Free",
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated)
                            }).Distinct();



                    return new ObservableCollection<SaleReportLine>(d);


                }
            }
            catch (Exception Ex)
            {
            }

            return null;
        }

        public static async Task<ObservableCollection<SaleReportLine>> Ex9SalesReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AllocationQSContext())
                {
                    var alst =
                        ctx.AsycudaSalesAllocationsExs.Where(
                                $"xASYCUDA_Id == {ASYCUDA_Id} " + "&& EntryDataDetailsId != null " +
                                "&& PreviousItem_Id != null" + "&& pRegistrationDate != null").ToList();

                    var d =
                        alst.Where(x => x.xLineNumber != null)
                            .Where(x => !string.IsNullOrEmpty(x.pCNumber))// prevent pre assessed entries
                            .Where(x => x.pItemNumber.Length <= 17) // to match the entry
                            .OrderBy(s => s.xLineNumber)
                            .ThenBy(s => s.InvoiceNo)
                            .Select(s => new SaleReportLine
                            {
                                Line = Convert.ToInt32(s.xLineNumber),
                                Date = Convert.ToDateTime(s.InvoiceDate),
                                InvoiceNo = s.InvoiceNo,
                                CustomerName = s.CustomerName,
                                ItemNumber = s.ItemNumber,
                                ItemDescription = s.ItemDescription,
                                TariffCode = s.TariffCode,
                                SalesFactor = Convert.ToDouble(s.SalesFactor),
                                SalesQuantity = Convert.ToDouble(s.QtyAllocated),

                                xQuantity = Convert.ToDouble(s.xQuantity), // Convert.ToDouble(s.QtyAllocated),
                                Price = Convert.ToDouble(s.Cost),
                                SalesType = s.DutyFreePaid,
                                GrossSales = Convert.ToDouble(s.TotalValue),
                                PreviousCNumber = s.pCNumber,
                                PreviousLineNumber = s.pLineNumber.ToString(),
                                PreviousRegDate = Convert.ToDateTime(s.pRegistrationDate).ToShortDateString(),
                                CIFValue =
                                    (Convert.ToDouble(s.Total_CIF_itm) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated),
                                DutyLiablity =
                                    (Convert.ToDouble(s.DutyLiability) / Convert.ToDouble(s.pQuantity)) *
                                    Convert.ToDouble(s.QtyAllocated)
                            }).Distinct();



                    return new ObservableCollection<SaleReportLine>(d);


                }
            }
            catch (Exception Ex)
            {

            }

            return null;

        }

        

        public static void SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email)
        {
            using (var ctx = new CoreEntitiesContext())
            {

                foreach (var file in csvFiles)
                {

                    var attachment =
                        ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                            x.Attachments.FilePath == file.FullName && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);

                    using (var ctx1 = new CoreEntitiesContext() { StartTracking = true })
                    {
                        var oldemail = ctx1.Emails.FirstOrDefault(x => x.EmailUniqueId == email.EmailId);
                        if (oldemail == null)
                        {
                            oldemail = ctx.Emails.Add(new Emails(true)
                            {
                                EmailUniqueId = email.EmailId,
                                Subject = email.Subject,
                                EmailDate = email.EmailDate,
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();
                        }
                        if (attachment == null)
                        {
                            ctx1.AsycudaDocumentSet_Attachments.Add(
                                new AsycudaDocumentSet_Attachments(true)
                                {
                                    AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                                    Attachments = new Attachments(true)
                                    {
                                        FilePath = file.FullName,
                                        DocumentCode = fileType.DocumentCode
                                    },
                                    DocumentSpecific = fileType.DocumentSpecific,
                                    FileDate = file.LastWriteTime,
                                    EmailUniqueId = email.EmailId,
                                    FileTypeId = fileType.Id,
                                    TrackingState = TrackingState.Added
                                });

                        }
                        else
                        {
                            attachment.DocumentSpecific = fileType.DocumentSpecific;
                            attachment.FileDate = file.LastWriteTime;
                            attachment.EmailUniqueId = email.EmailId;
                            attachment.FileTypeId = fileType.Id;
                        }
                        ctx1.SaveChanges();
                    }
                }

                ctx.SaveChanges();
            }
        }

        public static void SaveInfo(FileInfo[] csvFiles, int oldDocSet)
        {
            foreach (var file in csvFiles.Where(x => x.Name == "Info.txt"))
            {
                var fileTxt = File.ReadAllLines(file.FullName);
                SaveAsycudaDocumentSet(oldDocSet, fileTxt);
                // SaveEntryData(oldDocSet, fileTxt);
            }
        }

        public static void SaveAsycudaDocumentSet(int oldDocSet, string[] fileTxt)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var res = ctx.InfoMapping.Where(x => x.EntityType == "AsycudaDocumentSet")
                    .ToList();
                string dbStatement = "";
                foreach (var line in fileTxt)
                {
                    var match = Regex.Match(line,
                        @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.]*))", RegexOptions.IgnoreCase);
                    if (match.Success)
                        foreach (var infoMapping in res.Where(x =>
                            x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower()))
                        {
                            dbStatement +=
                                $@" Update AsycudaDocumentSet Set {infoMapping.Field} = '{
                                        ReplaceSpecialChar(match.Groups["Value"].Value.Trim(),
                                            "")
                                    }' Where AsycudaDocumentSetId = '{oldDocSet}';";
                        }
                }

                if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
            }
        }

        //public static void SaveEntryData(int oldDocSet, string[] fileTxt)
        //{
        //    using (var ctx = new CoreEntitiesContext())
        //    {
        //        var res = ctx.InfoMapping.Where(x => x.EntityType == "Discrepancy")
        //            .ToList();
        //        string dbStatement = "";
        //        foreach (var line in fileTxt)
        //        {
        //            var match = Regex.Match(line,
        //                @"((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.]*))", RegexOptions.IgnoreCase);
        //            if (match.Success)
        //                foreach (var infoMapping in res.Where(x =>
        //                    x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower()))
        //                {
        //                    dbStatement +=
        //                        $@" Update AsycudaDocumentSet Set {infoMapping.Field} = '{
        //                                ReplaceSpecialChar(match.Groups["Value"].Value.Trim(),
        //                                    "")
        //                            }' Where AsycudaDocumentSetId = '{oldDocSet}';";
        //                }
        //        }

        //        if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
        //    }
        //}

        public static EmailDownloader.Client Client { get; set; }

        public static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s]+", rstring);
        }



        public static void RunSiKuLi(int docSetId, string scriptName, string lastCNumber = "")
        {
            if (docSetId == 0) return;

            Console.WriteLine($"Executing {scriptName}");
            var docRef = new AsycudaDocumentSetExService().GetAsycudaDocumentSetExByKey(docSetId.ToString()).Result.Declarant_Reference_Number;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "java.exe";
            startInfo.Arguments =
                $@"-jar C:\Users\josep\OneDrive\Clients\AutoBot\sikulix.jar -r C:\Users\josep\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin
                    } {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} {(string.IsNullOrEmpty(lastCNumber) ? "" : lastCNumber + " ")}""{
                        Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docRef) + "\\"
                    }";
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();
            var timeoutCycles = 0;
            while (!process.HasExited && process.Responding)
            {
                if (timeoutCycles > 60) break;
                Thread.Sleep(1000 * 60);
                timeoutCycles += 1;
            }

            if (!process.HasExited) process.Kill();

            foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA")).ToList())
            {
                process1.Kill();
            }
        }



        public static void SaveCsv(FileInfo[] csvFiles, string fileType, int asycudaDocumentSetId)
        {
            Console.WriteLine("Importing CSV " + fileType);
            foreach (var file in csvFiles)
            {

                try
                {
                    SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, asycudaDocumentSetId, false)
                        .Wait();

                    //if (VerifyCSVImport(file))
                    //    return;
                    //else
                    //    continue;

                }
                catch (Exception e)
                {

                    using (var ctx = new CoreEntitiesContext())
                    {
                        if (ctx.AttachmentLog.FirstOrDefault(x =>
                                x.AsycudaDocumentSet_Attachments.Attachments.FilePath == file.FullName
                                && x.Status == "Sender Informed of Error") == null)
                        {
                            var att = ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x => x.Attachments.FilePath == file.FullName);
                            var body = "Error While Importing: \r\n" +
                                       $"File: {file}\r\n" +
                                       $"Error: {(e.InnerException ?? e).Message.Replace(file.FullName, file.Name)} \r\n" +
                                       $"Please Check the spreadsheet or inform Joseph Bartholomew if this is an Error.\r\n" +
                                       $"Regards,\r\n" +
                                       $"AutoBot";
                            var emailId = ctx.AsycudaDocumentSet_Attachments
                                .FirstOrDefault(x => x.Attachments.FilePath.Contains(file.FullName.Replace(file.Extension, "")))?.EmailUniqueId;
                            EmailDownloader.EmailDownloader.SendBackMsg(Convert.ToInt32(emailId), Client, body);
                            ctx.AttachmentLog.Add(new AttachmentLog(true)
                            {
                                DocSetAttachment = att.Id,
                                Status = "Sender Informed of Error",
                                TrackingState = TrackingState.Added
                            });
                            ctx.SaveChanges();
                        }
                    }


                }
            }
        }

        public static bool VerifyCSVImport(FileInfo file)
        {
            var dt = CSV2DataTable(file);
            if (dt.Rows.Count == 0) return false;
            var fileRes = dt.AsEnumerable()
                .Where(x => x[0] != DBNull.Value)
                .Select(x => new
                {
                    Invoice = x.Table.Columns.Contains("Invoice #") ? x["Invoice #"].ToString() : x["Invoice#"].ToString(),
                    Total = Convert.ToDouble(x["Quantity"]) * Convert.ToDouble(x["Cost"])
                })
                .GroupBy(x => x.Invoice)
                .Select(x => new
                {
                    Invoice = x.Key,
                    Total = Math.Round(x.Sum(z => z.Total), 2)
                }).ToList();

            using (var ctx = new EntryDataQSContext())
            {
                var dbres = ctx.EntryDataDetailsExes
                    .Select(x => new
                    {
                        Invoice = x.EntryDataId,
                        Total = x.Quantity * x.Cost
                    })
                    .GroupBy(x => x.Invoice)
                    .Select(x => new
                    {
                        Invoice = x.Key,
                        Total = Math.Round(x.Sum(z => z.Total), 2)
                    }).ToList();
                var res = fileRes.GroupJoin(dbres, x => x.Invoice, y => y.Invoice,
                        (x, y) => new { file = x, db = y.SingleOrDefault() })
                    .Where(x => x.file.Total != x.db.Total)
                    .Select(x => new { x.file.Invoice, FileTotal = x.file.Total, dbTotal = x.db.Total })
                    .ToList();

                if (!res.Any())
                {
                    //TODO: Log Message
                    return true;
                }
            }

            return false;
        }


        public static void Xlsx2csv(FileInfo[] files, FileTypes fileType)
        {
            var dic = new Dictionary<string, Func<Dictionary<string, string>, string>>()
            {
                {"CurrentDate", (dt)=> DateTime.Now.Date.ToShortDateString() },
                { "DIS-Reference", (dt) => $"DIS-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"},
                { "ADJ-Reference", (dt) => $"ADJ-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"},
                {"Quantity", (dt) => Convert.ToString(Math.Abs(Convert.ToDouble(dt["Received Quantity"].Replace("\"","")) - Convert.ToDouble(dt["Invoice Quantity"].Replace("\"",""))), CultureInfo.CurrentCulture) },
                {"ZeroCost", (x) => "0" },
                {"ABS-Added", (dt) => Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                {"ABS-Removed", (dt) => Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))).ToString(CultureInfo.CurrentCulture) },
                {"ADJ-Quantity", (dt) => Convert.ToString(Math.Abs((Math.Abs(Convert.ToDouble(dt["{Added}"].Replace("\"",""))) - Math.Abs(Convert.ToDouble(dt["{Removed}"].Replace("\"",""))))), CultureInfo.CurrentCulture) },
                {"Cost2USD", (dt) => (Convert.ToDouble(dt["{XCDCost}"].Replace("\"",""))/2.7169).ToString(CultureInfo.CurrentCulture) },
            };

            foreach (var file in files)
            {
                var dfile = new FileInfo($@"{file.DirectoryName}\{file.Name.Replace(file.Extension, ".csv")}");
                if (dfile.Exists && dfile.LastWriteTime.Date == DateTime.Now.Date) return;
                // Reading from a binary Excel file (format; *.xlsx)
                FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
                var excelReader = ExcelReaderFactory.CreateReader(stream);
                var result = excelReader.AsDataSet();
                excelReader.Close();

                string a = "";
                int row_no = 0;

                while (row_no < result.Tables[0].Rows.Count)
                {
                    if (fileType.FileTypeMappings.Any() && fileType.FileTypeMappings.Select(x => x.OriginalName)
                            .All(x => result.Tables[0].Rows[row_no].ItemArray.Contains(x)))
                    {
                        //if(dic.ContainsKey())
                        a = "";
                    }

                    for (int i = 0; i < result.Tables[0].Columns.Count; i++)
                    {

                        a += StringToCSVCell(result.Tables[0].Rows[row_no][i].ToString()) + ",";
                    }
                    row_no++;
                    a += "\n";


                }
                string output = Path.ChangeExtension(file.FullName, ".csv");
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(a);
                csv.Close();

                FixCsv(new FileInfo(output), fileType, dic);

            }
        }

        public static void FixCsv(FileInfo file, FileTypes fileType,
            Dictionary<string, Func<Dictionary<string, string>, string>> dic)
        {
            try
            {


                if (fileType.FileTypeMappings.Count == 0) return;
                var dfile = new FileInfo(
                    $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}");
                if (dfile.Exists && dfile.LastWriteTime.Date == DateTime.Now.Date) return;
                // Reading from a binary Excel file (format; *.xlsx)
                var dt = CSV2DataTable(file);


                string table = "";
                int row_no = 0;
                var dRows = new List<DataRow>();


                //delete rows till header
                while (row_no < dt.Rows.Count)
                {
                    if (dt.Rows[row_no].ItemArray.Count(x => !string.IsNullOrEmpty(x.ToString())) >=
                        fileType.FileTypeMappings.Count(x => x.Required))
                        dRows.Add(dt.Rows[row_no]);

                    row_no++;

                }
                // delete duplicate headers

                var dupheaders = dRows.Where(x =>
                        x.ItemArray.Contains(fileType.FileTypeMappings.OrderBy(z => z.Id).First(z => !z.OriginalName.Contains("{")).OriginalName)).Skip(1)
                    .ToList();
                foreach (var row in dupheaders)
                {
                    dRows.Remove(row);
                }

                row_no = 0;
                var header = dRows[0];
                for (int i = 0; i < header.ItemArray.Length; i++)
                {
                    header[i] = header[i].ToString().ToUpper();
                }
                while (row_no < dRows.Count)
                {
                    var row = new Dictionary<string, string>();
                    foreach (var mapping in fileType.FileTypeMappings.OrderBy(x => x.Id))
                    {
                        var maps = mapping.OriginalName.Split('+');
                        var val = "";
                        foreach (var map in maps)
                        {
                            if (!header.ItemArray.Contains(map.ToUpper()) &&
                                !dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
                            {
                                //TODO: log error
                                continue;
                            }

                            if (row_no == 0)
                            {
                                val += mapping.DestinationName;
                                if (maps.Length > 1) break;
                            }
                            else
                            {
                                //if (string.IsNullOrEmpty(dt.Rows[row_no][map].ToString())) continue;
                                if (dic.ContainsKey(map.Replace("{", "").Replace("}", "")))
                                {

                                    val += dic[map.Replace("{", "").Replace("}", "")].Invoke(row);
                                }
                                else
                                {
                                    var index = Array.LastIndexOf(header.ItemArray, map.ToUpper()); //last index of because of Cost USD file has two columns
                                    val += dRows[row_no][index];
                                }

                                if (maps.Length > 1 && map != maps.Last()) val += " - ";
                            }


                        }

                        if (val == "" && row_no == 0) continue;
                        if (row_no > 0)
                            if (string.IsNullOrEmpty(val) && mapping.Required == true)
                            {
                                if (mapping.DestinationName == "Invoice #")
                                {
                                    val += dic["DIS-Reference"].Invoke(row);

                                }
                                else
                                {
                                    break;
                                }

                            }

                            else if (mapping.DataType == "Number")
                            {
                                if (string.IsNullOrEmpty(val)) val = "0";
                                if (val.ToCharArray().All(x => !char.IsDigit(x)))
                                {
                                    //Log Error
                                    break;
                                    //val = "";
                                }
                            }
                            else if (mapping.DataType == "Date")
                            {
                                DateTime tmp;
                                if (DateTime.TryParse(val, out tmp) == false)
                                {
                                    //Log Error
                                    break;
                                    //  val = "";
                                }
                            }

                        if (row.ContainsKey(mapping.DestinationName))
                        {
                            row[mapping.DestinationName] = StringToCSVCell(val);
                        }
                        else
                        {
                            row.Add(mapping.DestinationName, StringToCSVCell(val));
                        }





                    }

                    row_no++;

                    if (row.Count > 0 && row.Count >= fileType.FileTypeMappings.DistinctBy(x => x.DestinationName).Count(x => x.Required == true))
                        table += row.Where(x => x.Key.StartsWith("{") == false).Select(x => x.Value).Aggregate((a, x) => a + "," + x) + "\n";
                }

                string output = $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}";
                StreamWriter csv = new StreamWriter(output, false);
                csv.Write(table);
                csv.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }



        public static DataTable CSV2DataTable(FileInfo file)
        {
            OleDbConnection conn = new OleDbConnection(string.Format(
                @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};" +
                "Extended Properties=\"Text;HDR=NO;FMT=Delimited;CharacterSet=65001\"",
                file.DirectoryName
            ));
            conn.Open();

            string sql = string.Format("select * from [{0}]", Path.GetFileName(file.Name));
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();

            return dt;
        }

        /// <summary>
        /// Turn a string into a CSV cell output
        /// </summary>
        /// <param name="str">String to output</param>
        /// <returns>The CSV cell formatted string</returns>
        public static string StringToCSVCell(string str)
        {
            //bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") 
            //                  || str.Contains("\n") || str.Contains(".") || str.Contains("'") || str.Contains("#"));
            //if (mustQuote)
            //{
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char nextChar in str)
            {
                sb.Append(nextChar);
                if (nextChar == '"')
                    sb.Append("\"");
            }
            sb.Append("\"");
            return sb.ToString();
            //}

            //return str;
        }
    }
}
