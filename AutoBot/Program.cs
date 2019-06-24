using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AllocationQS.Business.Entities;
using AllocationQS.Client.Repositories;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using CoreEntities.Business.Services;
using DocumentDS.Business.Services;
using EntryDataQS.Business.Entities;
using ExcelDataReader;
using MoreLinq;
using SalesDataQS.Business.Services;
using SalesDataQS.Client.Repositories;
using TrackableEntities;
using WaterNut.DataSpace;


namespace AutoBot
{
    partial class Program
    {
        public class FileAction
        {

            public string Filetype { get; set; }

            public List<Action<FileTypes, FileInfo[]>> Actions { get; set; } =
                new List<Action<FileTypes, FileInfo[]>>();
        }

        private static Dictionary<string, Action<FileTypes, FileInfo[]>> fileActions =>
            new Dictionary<string, Action<FileTypes, FileInfo[]>>
            {
                {"ImportSalesEntries",(ft, fs) => ImportSalesEntries() },
                {"AllocateSales",(ft, fs) => AllocateSales() },
                {"CreateEx9",(ft, fs) => CreateEx9(ft) },
                {"ExportEx9Entries",(ft, fs) => ExportEx9Entries().Wait() },
                {"AssessEx9Entries",(ft, fs) => AssessEx9Entries(ft) },
                {"SaveCsv",(ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId) },
                {"CreatePOEntries",(ft, fs) => CreatePOEntries().Wait() },
                {"ExportPOEntries",(ft, fs) => ExportPOEntries().Wait() },
                {"AssessEntry",(ft, fs) => AssessEntry(ft)},
                {"EmailPOEntries",(ft, fs) => EmailPOEntries(ft.FileTypeContacts.Select(x => x.Contacts).ToList()) },
                {"DownloadSalesFiles",(ft, fs) => DownloadSalesFiles() },
                {"Xlsx2csv",(ft, fs) => Xlsx2csv(fs, ft) },
                {"SaveInfo",(ft, fs) => SaveInfo(fs, ft.AsycudaDocumentSetId) },
                {"CleanupEntries",(ft, fs) => CleanupEntries() },
                {"SaveAttachments",(ft, fs) => SaveAttachments(fs, ft) },
            };

        private static void CleanupEntries()
        {
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


        //private static List<FileAction> fileActions => new List<FileAction>
        //{
        //    new FileAction
        //    {
        //        Filetype = "XML",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {

        //            (ft, fs) => ImportSalesEntries(),
        //            (x,y) => AllocateSales(),
        //            (ft, fs) => CreateEx9(ft),
        //            (ft, fs) => ExportEx9Entries().Wait(),
        //            (x,y) => AssessEx9Entries(x),

        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "PO",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId),
        //            (ft, fs) => CreatePOEntries().Wait(),
        //             (ft, fs) => ExportPOEntries().Wait(),
        //            //(x,y) => RunSiKuLi(x.AsycudaDocumentSetId,"AssessIM7"),
        //            (ft, fs) => EmailPOEntries(ft.Contacts),
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "Sales",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {

        //            (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId),
        //            (x, y) => DownloadSalesFiles(),
        //            (ft, fs) => ImportSalesEntries(),
        //            (x,y) => AllocateSales(),
        //            (ft, fs) => CreateEx9(ft),
        //            (ft, fs) => ExportEx9Entries().Wait(),
        //            (x,y) => AssessEx9Entries(x),
        //            (ft, fs) => ImportSalesEntries(),
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "OPS",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "ADJ",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "DIS",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "RECON",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => SaveCsv(fs, ft.Type, ft.AsycudaDocumentSetId)
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "XLSX",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => Xlsx2csv(fs, ft.FileTypeMappings)
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "FIX",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //            (ft, fs) => FixCsv(fs, ft.FileTypeMappings)
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "Info",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //             (ft, fs) =>  CreatePOEntries().Wait(),
        //             (ft, fs) =>  ExportPOEntries().Wait(),
        //            //(x,y) => AssessEntry(x),
        //            (ft, fs) => EmailPOEntries(ft.Contacts),
        //        }
        //    },
        //    new FileAction
        //    {
        //        Filetype = "PDF",
        //        Actions = new List<Action<FileTypes, FileInfo[]>>
        //        {
        //             (ft, fs) =>  CreatePOEntries().Wait(),
        //             (ft, fs) =>  ExportPOEntries().Wait(),
        //            //(x,y) => AssessEntry(x),
        //            (ft, fs) => EmailPOEntries(ft.Contacts),
        //        }

        //    }
        //};

        private static void EmailSalesErrors()
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
                if(File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(client, directory, $"Sales Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile});
            }

        }
        private static void EmailPOEntries(List<Contacts> contacts)
        {
            if (!contacts.Any()) return;
            var lst = CurrentPOInfo();
            foreach (var poInfo in lst)
            {
                
                if (poInfo.Item1 == null) return;
                var reference = poInfo.Item1.Declarant_Reference_Number;
                var directory = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, reference);
                var files = Directory.GetFiles(directory, "*.xml");
                var emailres = new FileInfo(Path.Combine(directory, "EmailResults.txt"));
                var instructions = new FileInfo(Path.Combine(directory, "Instructions.txt"));
                if (!instructions.Exists) return;
                Console.WriteLine("Emailing Po Entries");
                if (emailres.Exists)
                {
                    var eRes = File.ReadAllLines(emailres.FullName);
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = files.ToList().Where(x => insts.Contains(x) && !eRes.Contains(x)).ToArray();
                }
                else
                {
                    var insts = File.ReadAllLines(instructions.FullName);
                    files = files.ToList().Where(x => insts.Any(z => z.Contains(x))).ToArray();
                }
                if (files.Length > 0)
                    EmailDownloader.EmailDownloader.SendEmail(client, directory, $"Entries for {reference}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", files);

            }
        }

        private static void DownloadSalesFiles()
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

        private static void AssessEx9Entries(FileTypes x)
        {
            Console.WriteLine("Assessing Ex9 Entries");
            var saleinfo = CurrentSalesInfo();
            x.DocReference = saleinfo.Item3.Declarant_Reference_Number;
            x.AsycudaDocumentSetId = saleinfo.Item3.AsycudaDocumentSetId;
            AssessEntry(x);
        }

        private static void AssessEntry(FileTypes x)
        {
            Console.WriteLine($"Assessing {x.Type} Entries");
            if (x.DocReference == null) return;
            var resultsFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                x.DocReference, "InstructionResults.txt");
            var instrFile = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                x.DocReference, "Instructions.txt");

            var lcont = 0;
            while (AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                RunSiKuLi(x.AsycudaDocumentSetId, "AssessIM7", lcont.ToString());
                RunSiKuLi(CurrentSalesInfo().Item3.AsycudaDocumentSetId, "IM7", lcont.ToString());
                ImportSalesEntries();
                CleanupEntries();
            }



        }

        private static bool ImportComplete(string directoryName, out int lcont)
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
        private static bool AssessComplete(string instrFile, string resultsFile, out int lcont)
        {
            lcont = 0;

            
            if (File.Exists(instrFile) )
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

        private static void ImportSalesEntries()
        {
            Console.WriteLine("Import Entries");
            using (var ctx = new CoreEntitiesContext())
            {
                var docSetId = CurrentSalesInfo().Item3.AsycudaDocumentSetId;
                var ft = ctx.FileTypes.FirstOrDefault(x => x.Type == "XML" && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if(ft == null) return;
                var desFolder = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                    ctx.AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == docSetId)
                        .Declarant_Reference_Number);
                var csvFiles = new DirectoryInfo(desFolder).GetFiles().Where(x => Regex.IsMatch(x.FullName, ft.FilePattern)).ToArray();
                if(csvFiles.Length > 0)
                    BaseDataModel.Instance.ImportDocuments(ft.AsycudaDocumentSetId,csvFiles.Select(x => x.FullName).ToList(), true, true, false, false, true).Wait();
            }
        }

        private static void CreateEx9(FileTypes ft)
        {
            Console.WriteLine("Create Ex9");
            if (ft.Type != "Sales") return;
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

        private static Tuple<DateTime, DateTime, AsycudaDocumentSetEx,string > CurrentSalesInfo()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23);
            var docRef = startDate.ToString("MMMM") + " " + startDate.Year.ToString();
            var docSet = new CoreEntitiesContext().AsycudaDocumentSetExs.FirstOrDefault(x =>
                x.Declarant_Reference_Number == docRef && x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
            var dirPath = Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, docRef);
            return new Tuple<DateTime, DateTime, AsycudaDocumentSetEx, string>(startDate,endDate, docSet, dirPath);
        }

        private static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo()
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

        private static void AllocateSales()
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

        private static async Task CreatePOEntries()
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
                            Entrylst = x.Select(z => new {z.EntryDataDetailsId, z.IsClassified}).ToList()
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

        private static async Task ExportPOEntries()
        {
            Console.WriteLine("Export PO Entries");
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    var res = ctx.AsycudaDocuments
                        .Where(x =>x.ApplicationSettingsId ==BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                   && x.ImportComplete == false 
                                   && x.DocumentType == "IM7")
                        .GroupBy(x => new {x.AsycudaDocumentSetId, ReferenceNumber = x.AsycudaDocumentSetEx.Declarant_Reference_Number})
                        .Select(x => new
                        {
                            DocSet =  x.Key,
                            Entrylst = x.Select(z => new { z.ASYCUDA_Id}).ToList()
                        })
                        .ToList();
                    foreach (var docSetId in res)
                    {
                        BaseDataModel.Instance.ExportDocSet(docSetId.DocSet.AsycudaDocumentSetId.Value,
                            Path.Combine(BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                docSetId.DocSet.ReferenceNumber), true).Wait();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static async Task ExportEx9Entries()
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

            alst = await IM9AdjustmentsReport(ASYCUDA_Id).ConfigureAwait(false);
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

        private static async Task<ObservableCollection<SaleReportLine>> IM9AdjustmentsReport(int ASYCUDA_Id)
        {
            try
            {
                using (var ctx = new AdjustmentShortAllocationRepository())
                {
                    var alst =
                        (await ctx.GetAdjustmentShortAllocationsByExpression(
                                $"xASYCUDA_Id == {ASYCUDA_Id} " + "&& EntryDataDetailsId != null " +
                                "&& PreviousItem_Id != null" + "&& pRegistrationDate != null")
                            .ConfigureAwait(false)).ToList();

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

        private static async Task<ObservableCollection<SaleReportLine>> Ex9SalesReport(int ASYCUDA_Id)
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

        static void Main(string[] args)
        {
            try
            {
                

                using (var ctx = new CoreEntitiesContext() { })
                {


                    foreach (var appSetting in ctx.ApplicationSettings.AsNoTracking()
                        .Include(x => x.FileTypes)
                        .Include("FileTypes.FileTypeContacts.Contacts")
                        .Include("FileTypes.FileTypeActions.Actions")
                        .Include(x => x.EmailMapping)

                        .Include("FileTypes.FileTypeMappings").ToList())
                    {
                        // set BaseDataModel CurrentAppSettings
                        BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                        //check emails

                        var salesInfo = CurrentSalesInfo();
                        var dref = salesInfo.Item1.ToString("MMMM yyyy");
                        if (salesInfo.Item3 == null)
                        {

                            var doctype = ctx.Document_Type.Include(x => x.Customs_Procedure).First(x =>
                                x.Type_of_declaration == "IM" && x.Declaration_gen_procedure_code == "7");
                            ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({appSetting.ApplicationSettingsId},'{dref}',{doctype.Document_TypeId},{
                                    doctype.Customs_Procedure.First(z => z.IsDefault == true).Customs_ProcedureId
                                },0)");
                            Directory.CreateDirectory(Path.Combine(appSetting.DataFolder, dref));
                        }

                        if (!Directory.Exists(Path.Combine(appSetting.DataFolder, dref)))
                            Directory.CreateDirectory(Path.Combine(appSetting.DataFolder, dref));
                        //
                        client = new EmailDownloader.Client
                        {
                            DataFolder = appSetting.DataFolder,
                            Password = appSetting.EmailPassword,
                            Email = appSetting.Email,
                            EmailMappings = appSetting.EmailMapping.Select(x => x.Pattern).ToList()
                        };
                        
                        var msgLst = EmailDownloader.EmailDownloader.CheckEmails(client);
                        // get downloads
                        foreach (var msg in msgLst)
                        {
                            var desFolder = Path.Combine(appSetting.DataFolder, msg.Key);
                            foreach (var fileType in appSetting.FileTypes)
                            {
                                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                                    .Where(x => msg.Value.Contains(x.FullName) && Regex.IsMatch(x.FullName, fileType.FilePattern) &&
                                                x.LastWriteTime.Date == DateTime.Now.Date).ToArray();

                                if (csvFiles.Length == 0) continue;

                                var oldDocSet =
                                    ctx.AsycudaDocumentSetExs
                                        .Include(x => x.AsycudaDocumentSet_Attachments)
                                        .Include("AsycudaDocumentSet_Attachments.Attachments")
                                        .FirstOrDefault(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                                if (fileType.CreateDocumentSet)
                                {
                                    var docSet =
                                        ctx.AsycudaDocumentSetExs
                                            .Include(x => x.AsycudaDocumentSet_Attachments)
                                            .Include("AsycudaDocumentSet_Attachments.Attachments")
                                            .FirstOrDefault(x => x.Declarant_Reference_Number == msg.Key);
                                    if (docSet == null)
                                    {
                                        ctx.Database.ExecuteSqlCommand($@"INSERT INTO AsycudaDocumentSet
                                        (ApplicationSettingsId, Declarant_Reference_Number, Document_TypeId, Customs_ProcedureId, Exchange_Rate)
                                    VALUES({oldDocSet.ApplicationSettingsId},'{msg.Key}',{oldDocSet.Document_TypeId},{
                                                oldDocSet.Customs_ProcedureId
                                            },0)");

                                    }
                                }

                            }
                        }

                        var docLst = new List<AsycudaDocumentSetEx>()
                        {
                            CurrentSalesInfo().Item3,
                        };
                        foreach (var i in CurrentPOInfo().Select(x => x.Item1))
                        {
                            if(docLst.FirstOrDefault(x => x.AsycudaDocumentSetId == i.AsycudaDocumentSetId) == null)
                                docLst.AddRange(CurrentPOInfo().Select(x => x.Item1));
                        }
                        
                            

                        foreach (var fileType in appSetting.FileTypes
                        ) //.Where(x => x.Type != "Sales" && x.Type != "PO")
                        {
                            foreach (var dSet in docLst)
                            {
                                var desFolder = Path.Combine(appSetting.DataFolder, dSet.Declarant_Reference_Number);
                                fileType.AsycudaDocumentSetId = dSet.AsycudaDocumentSetId;
                                fileType.DocReference = dSet.Declarant_Reference_Number;
                                var csvFiles = new DirectoryInfo(desFolder).GetFiles()
                                    .Where(x => Regex.IsMatch(x.FullName, fileType.FilePattern) &&
                                                x.LastWriteTime.Date == DateTime.Now.Date).ToArray();
                                if (!csvFiles.Any()) continue;
                                fileType.FileTypeActions.OrderBy(x => x.Priority)
                                    .Select(x => fileActions[x.Actions.Name]).ToList()
                                    .ForEach(x =>
                                        x.Invoke(fileType,
                                            csvFiles)); 
                            }
                        }

                    }
                }

               // Application.SetSuspendState(PowerState.Suspend, true, true);
            }
            catch (Exception e)
            {
                EmailDownloader.EmailDownloader.SendEmail(client, null, $"Bug Found",
                    new []{"Josephbartholomew@outlook.com"}, $"{e.Message}\r\n{e.StackTrace}", Array.Empty<string>());
            }
        }

        private static void SaveAttachments(FileInfo[] csvFiles, FileTypes fileType)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                
                foreach (var file in csvFiles)
                {
                    if (fileType.DocumentCode == "NA") continue;
                    var attachment =
                        ctx.AsycudaDocumentSet_Attachments.FirstOrDefault(x =>
                            x.Attachments.FilePath == file.FullName && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
                    if (attachment == null)
                    {
                        using (var ctx1 = new CoreEntitiesContext() {StartTracking = true})
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
                                    TrackingState = TrackingState.Added
                                });
                            ctx1.SaveChanges();
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }

        private static void SaveInfo(FileInfo[] csvFiles, int oldDocSet)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                var dbStatement = "";
                foreach (var file in csvFiles)
                {
                    var fileTxt = File.ReadAllLines(file.FullName);
                    var res = ctx.InfoMapping.Where(x => x.EntityType == "AsycudaDocumentSet")
                        .ToList();
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
                }

                if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
            }
        }

        public static EmailDownloader.Client client { get; set; }

        private static string ReplaceSpecialChar(string msgSubject, string rstring)
        {
            return Regex.Replace(msgSubject, @"[^0-9a-zA-Z.\s]+", rstring);
        }



        private static void RunSiKuLi(int docSetId, string scriptName, string lastCNumber = "")
        {
            if(docSetId == 0) return;
            
            Console.WriteLine($"Executing {scriptName}");
            var docRef = new AsycudaDocumentSetExService().GetAsycudaDocumentSetExByKey(docSetId.ToString()).Result.Declarant_Reference_Number;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "java.exe";
            startInfo.Arguments =
                $@"-jar C:\Users\josep\OneDrive\Clients\AutoBot\sikulix.jar -r C:\Users\josep\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin
                    } {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} {(string.IsNullOrEmpty(lastCNumber)? "" : lastCNumber + " ")}""{
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



        private static void SaveCsv(FileInfo[] csvFiles, string fileType, int asycudaDocumentSetId)
        {
            Console.WriteLine("Importing CSV " + fileType);
            foreach (var file in csvFiles)
            {
                SaveCSVModel.Instance.ProcessDroppedFile(file.FullName, fileType, asycudaDocumentSetId, false)
                    .Wait();

                if (VerifyCSVImport(file))
                    return;
                else
                    continue;
            }
        }

        private static bool VerifyCSVImport(FileInfo file)
        {
            var dt = CSV2DataTable(file);
            if (dt.Rows.Count == 0) return false;
            var fileRes = dt.AsEnumerable()
                .Where(x => x[0] != DBNull.Value)
                .Select(x => new
                {
                    Invoice = x["Invoice #"].ToString(),
                    Total = Convert.ToDouble(x["Quantity"]) * x.Field<double>("Cost")
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
                        (x, y) => new {file = x, db = y.SingleOrDefault()})
                    .Where(x => x.file.Total != x.db.Total)
                    .Select(x => new {x.file.Invoice, FileTotal = x.file.Total, dbTotal = x.db.Total})
                    .ToList();

                if (res.Any())
                {
                    //TODO: Log Message
                    return true;
                }
            }

            return false;
        }


        private static void Xlsx2csv(FileInfo[] files, FileTypes fileType)
        {
            var dic = new Dictionary<string, string>()
            {
                {"CurrentDate", DateTime.Now.Date.ToShortDateString() },
                { "DIS-Reference", $"DIS-{new CoreEntitiesContext().AsycudaDocumentSetExs.First(x => x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId).Declarant_Reference_Number}"}
            };

            foreach (var file in files)
            {
                if (File.Exists($@"{file.DirectoryName}\{file.Name.Replace(file.Extension, ".csv")}")) return;
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

                FixCsv(new FileInfo(output), fileType.FileTypeMappings);
                
            }
        }

        private static void FixCsv(FileInfo file, List<FileTypeMappings> mappings)
        {
            if (mappings.Count == 0) return;
            if (File.Exists($@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}")) return;
            // Reading from a binary Excel file (format; *.xlsx)
            var dt = CSV2DataTable(file);


            string table = "";
            int row_no = 0;

            while (row_no < dt.Rows.Count)
            {
                var row = new Dictionary<string, string>();
                foreach (var mapping in mappings)
                {
                    var maps = mapping.OriginalName.Split('+');
                    var val = "";
                    foreach (var map in maps)
                    {
                        if (!dt.Columns.Contains(map))
                        {
                            //TODO: log error
                            return;
                        }

                        if (row_no == 0)
                        {
                            val += mapping.DestinationName;
                            if (maps.Length > 1) break;
                        }
                        else
                        {
                            //if (string.IsNullOrEmpty(dt.Rows[row_no][map].ToString())) continue;

                            val += dt.Rows[row_no][map];
                            if (maps.Length > 1 && map != maps.Last()) val += " - ";
                        }


                    }
                    if (row_no > 0)
                        if (string.IsNullOrEmpty(val) && mapping.Required == true)
                            break;
                        else if (mapping.DataType == "Number")
                        {
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

                    row.Add(mapping.DestinationName, StringToCSVCell(val));

                }
                row_no++;

                if (row.Count > 0 && row.Count == mappings.Count) table += row.Select(x => x.Value).Aggregate((a, x) => a + "," + x) + "\n";
            }
            string output = $@"{file.DirectoryName}\{file.Name.Replace(".csv", "")}-Fixed{file.Extension}";
            StreamWriter csv = new StreamWriter(output, false);
            csv.Write(table);
            csv.Close();


        }

      

        private static DataTable CSV2DataTable(FileInfo file)
        {
            OleDbConnection conn = new OleDbConnection(string.Format(
                @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};" +
                "Extended Properties=\"Text;HDR=YES;FMT=Delimited\"",
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
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n") );
            if (mustQuote)
            {
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
            }

            return str;
        }
    }

}
