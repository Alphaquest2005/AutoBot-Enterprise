using AdjustmentQS.Business.Services;
using AllocationQS.Business.Entities;
using Core.Common.Converters;
using Core.Common.Data.Contracts;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using EntryDataQS.Business.Entities;
using ExcelDataReader;
using InventoryDS.Business.Entities;
using LicenseDS.Business.Entities;
using MoreLinq;
using SalesDataQS.Business.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using AdjustmentQS.Business.Entities;
using AllocationDS.Business.Entities;
using Asycuda421;
using AutoBotUtilities;
using DocumentItemDS.Business.Entities;
using EmailDownloader;
using OCR.Business.Entities;
using Org.BouncyCastle.Ocsp;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using SimpleMvvmToolkit.ModelExtensions;
using TrackableEntities;
using TrackableEntities.Client;
using ValuationDS.Business.Entities;
using WaterNut.Business.Entities;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using WaterNut.DataSpace.Asycuda;
using ApplicationException = System.ApplicationException;
using AsycudaDocument = CoreEntities.Business.Entities.AsycudaDocument;
using AsycudaDocument_Attachments = CoreEntities.Business.Entities.AsycudaDocument_Attachments;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using Contacts = CoreEntities.Business.Entities.Contacts;
using CoreEntitiesContext = CoreEntities.Business.Entities.CoreEntitiesContext;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;
using EntryDataDetails = EntryDataDS.Business.Entities.EntryDataDetails;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using InventoryItemAlias = EntryDataDS.Business.Entities.InventoryItemAlias;
using Invoices = OCR.Business.Entities.Invoices;
using xBondAllocations = AllocationDS.Business.Entities.xBondAllocations;
using xcuda_Tarification = DocumentItemDS.Business.Entities.xcuda_Tarification;

namespace AutoBot
{
   

    public partial class Utils
    {
       
        public static Client Client { get; set; } = new Client
        {
            CompanyName = BaseDataModel.Instance.CurrentApplicationSettings.CompanyName,
            DataFolder = BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
            Password = BaseDataModel.Instance.CurrentApplicationSettings.EmailPassword,
            Email = BaseDataModel.Instance.CurrentApplicationSettings.Email,
            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
            EmailMappings = BaseDataModel.Instance.CurrentApplicationSettings.EmailMapping.ToList(),
            DevMode = true
        };


    public static void Kill(FileTypes arg1, FileInfo[] arg2)
        {
            Application.Exit();
        }


    public static void SetCurrentApplicationSettings(int id)
        {
            using (var ctx = new CoreEntitiesContext() { })
            {


                var appSetting = ctx.ApplicationSettings.AsNoTracking()
                    .Include(x => x.FileTypes)
                    .Include(x => x.Declarants)
                    .Include("FileTypes.FileTypeContacts.Contacts")
                    .Include("FileTypes.FileTypeActions.Actions")
                    .Include(x => x.EmailMapping)
                    .Include("FileTypes.FileTypeMappings")
                    .Where(x => x.IsActive)
                    .First(x => x.ApplicationSettingsId == id);

                // set BaseDataModel CurrentAppSettings
                BaseDataModel.Instance.CurrentApplicationSettings = appSetting;
                //check emails

            }
        }



      


        public static void RunAllocationTestCases()
        {
            try
            {
                Console.WriteLine("Running Test Cases");
                List<KeyValuePair<int, string>> lst;
                using (var ctx = new AllocationDSContext())
                {
                    ctx.Database.CommandTimeout = 10;

                    lst = ctx
                        .AllocationsTestCases
                        .Select(x => new { x.EntryDataDetailsId, x.ItemNumber })
                        .Distinct()
                        .ToList()
                        .Select(x => new KeyValuePair<int, string>(x.EntryDataDetailsId, x.ItemNumber))
                        .ToList();
                }

                if (!lst.Any()) return;
                AllocationsModel.Instance.ClearDocSetAllocations(lst.Select(x => $"'{x.Value}'").Aggregate((o, n) => $"{o},{n}")).Wait();

                AllocationsBaseModel.PrepareDataForAllocation(BaseDataModel.Instance.CurrentApplicationSettings);



                var strLst = lst.Select(x => $"{x.Key.ToString()}-{x.Value}").Aggregate((o, n) => $"{o},{n}");


                new AdjustmentShortService().AutoMatchUtils.AutoMatchItems(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, strLst).Wait();


                new AdjustmentShortService().AutoMatchUtils.AutoMatchProcessor.ProcessDisErrorsForAllocation
                    .Execute(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        strLst).Wait();

                new OldSalesAllocator()
                    .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false, false, strLst).Wait();

                new MarkErrors()
                    .Execute(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        public static void SaveAttachments(FileInfo[] csvFiles, FileTypes fileType, Email email)
        {
            try
            {


                using (var ctx = new CoreEntitiesContext() { StartTracking = true })
                {
                    var oldemail = ctx.Emails.Include("EmailAttachments.Attachments").FirstOrDefault(x => x.EmailId == email.EmailId);
                    if (oldemail == null)
                    {
                        oldemail = ctx.Emails.Add(new Emails(true)
                        {
                            EmailUniqueId = email.EmailUniqueId,
                            EmailId = email.EmailId,
                            Subject = email.Subject,
                            EmailDate = email.EmailDate,
                            MachineName = Environment.MachineName,
                            ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                            TrackingState = TrackingState.Added
                        });
                        ctx.SaveChanges();
                    }
                    else
                    {
                        oldemail.MachineName = Environment.MachineName;
                        oldemail.EmailUniqueId = email.EmailUniqueId;
                    }

                    foreach (var file in csvFiles)
                    {

                        if (fileType.FileImporterInfos.EntryType != FileTypeManager.EntryTypes.Unknown)
                        {
                            FileTypeManager.SendBackTooBigEmail(file, fileType);
                        }

                        var reference = GetReference(file, ctx);

                        Attachments attachment = ctx.Attachments.FirstOrDefault(x => x.FilePath == file.FullName);
                        if(attachment == null)
                        attachment = new Attachments(true)
                        {
                            FilePath = file.FullName,
                            DocumentCode = fileType.DocumentCode,
                            Reference = reference,
                            EmailId = email.EmailId,
                            TrackingState = TrackingState.Added
                        };


                        AddUpdateEmailAttachments(fileType, email, oldemail, file, ctx, attachment, reference);

                        if (fileType.AsycudaDocumentSetId != 0) EntryDocSetUtils.AddUpdateDocSetAttachement(fileType, email, ctx, file, attachment, reference);

                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        private static void AddUpdateEmailAttachments(FileTypes fileType, Email email, Emails oldemail, FileInfo file,
            CoreEntitiesContext ctx, Attachments attachment, string reference)
        {
            var emailAttachement =
                oldemail.EmailAttachments.FirstOrDefault(x => x.Attachments.FilePath == file.FullName);


            if (emailAttachement == null)
            {
                ctx.EmailAttachments.Add(
                    new EmailAttachments(true)
                    {
                        Attachments = attachment,
                        DocumentSpecific = fileType.DocumentSpecific,
                        EmailId = email.EmailId,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added
                    });
            }
            else
            {
                emailAttachement.DocumentSpecific = fileType.DocumentSpecific;
                emailAttachement.EmailId = email.EmailId;
                emailAttachement.FileTypeId = fileType.Id;
                emailAttachement.Attachments.Reference = reference;
                emailAttachement.Attachments.DocumentCode = fileType.DocumentCode;
                emailAttachement.Attachments.EmailId = email.EmailId;
            }
            ctx.SaveChanges();
        }

        private static string GetReference(FileInfo file, CoreEntitiesContext ctx)
        {
            var newReference = file.Name.Replace(file.Extension, "");

            var existingRefCount = ctx.Attachments.Select(x => x.Reference)
                .Where(x => x.Contains(newReference)).Count();
            if (existingRefCount > 0) newReference = $"{existingRefCount + 1}-{newReference}";
            return newReference;
        }


        public static bool AssessComplete(string instrFile, string resultsFile, out int lcont)
        {
            try
            {


                lcont = 0;
                var rcount = 0;

                if (File.Exists(instrFile))
                {
                    if (!File.Exists(resultsFile)) return false;
                    var lines = File.ReadAllLines(instrFile).Where(x => x.StartsWith("File\t")).ToArray();
                    var res = File.ReadAllLines(resultsFile).Where(x => x.StartsWith("File\t")).ToArray();
                    if (res.Length == 0)
                    {

                        return false;
                    }


                    foreach (var line in lines)
                    {
                        var p = line.Split('\t');
                        if (lcont + 1 >= res.Length) return false;
                        if (string.IsNullOrEmpty(res[lcont])) return false;
                        rcount += 1;
                        var isSuccess = false;
                        foreach (var rline in res)
                        {
                            var r = rline.Split('\t');

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Success")
                            {
                                if (r[0] == "File") lcont = rcount - 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] == r[0] && r[2] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 4 && p[2] == r[2] && r[3] == "Success") // for file
                            {
                                lcont += 1;
                                isSuccess = true;
                                break;
                            }

                            if (r.Length == 3 && p[1] != r[1] && r[2] == "Error")
                            {

                                if (r[0] == "Screenshot")
                                {

                                    SubmitScriptErrors(r[1]);
                                    return true;
                                }
                                //isSuccess = true;
                                //break;
                            }

                            if (r.Length == 3 && p[1] == r[1] && r[2] == "Error")
                            {
                                // email error
                                //if (r[0] == "File") lcont = rcount - 1;
                                //isSuccess = true;
                                //break;
                            }

                        }

                        if (isSuccess == true) continue;
                        return false;
                    }

                    return true;
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void SubmitScriptErrors(string file)
        {
            try
            {


                Console.WriteLine("Submit Script Errors");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId
                                    && (x.Role == "Developer" || x.Role == "Broker")).Select(x => x.EmailAddress).ToArray();


                    var body = $"Please see attached.\r\n" +
                               $"Regards,\r\n" +
                               $"AutoBot";

                    var msg = EmailDownloader.EmailDownloader.CreateMessage(Client, "AutoBot Script Error", contacts, body, new string[]
                    {
                        file
                    });
                    EmailDownloader.EmailDownloader.SendEmail(Client, msg);
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public static void RetryImport(int trytimes, string script, bool redownload, string directoryName)
        {
            int lcont;
            for (int i = 0; i < trytimes; i++)
            {
                if (Utils.ImportComplete(directoryName, redownload, out lcont, DateTime.Now.Year))
                    break; //ImportComplete(directoryName,false, out lcont);
                Utils.RunSiKuLi(directoryName, script, lcont.ToString());
                if (Utils.ImportComplete(directoryName, redownload, out lcont, DateTime.Now.Year)) break;
            }
        }

        public static void RetryAssess(string instrFile, string resultsFile, string directoryName, int trytimes)
        {
            var lcont = 0;
            for (int i = 0; i < trytimes; i++)
            {
                if (Utils.AssessComplete(instrFile, resultsFile, out lcont) == true) break;
            
                // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                Utils.RunSiKuLi(directoryName, "AssessIM7", lcont.ToString()); //SaveIM7
                if(Utils.AssessComplete(instrFile, resultsFile, out lcont) == true) break;
            }
        }

        public static void Assess(string instrFile, string resultsFile, string directoryName)
        {
            var lcont = 0;
            while (Utils.AssessComplete(instrFile, resultsFile, out lcont) == false)
            {
                // RunSiKuLi(asycudaDocumentSetId, "AssessIM7", lcont.ToString());
                Utils.RunSiKuLi(directoryName, "AssessIM7", lcont.ToString()); //SaveIM7
            }
        }

        public static void RunSiKuLi(string directoryName, string scriptName, string lastCNumber = "", int sMonths = 0, int sYears = 0, int eMonths = 0, int eYears = 0)
        {
            try
            {

                if (string.IsNullOrEmpty(directoryName)) return;

                Console.WriteLine($"Executing {scriptName}");


                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "java.exe",
                    Arguments = $@"-jar C:\Users\{Environment.UserName}\OneDrive\Clients\AutoBot\sikulixide-2.0.5.jar -r C:\Users\{Environment.UserName
                    }\OneDrive\Clients\AutoBot\Scripts\{scriptName}.sikuli --args {
                        BaseDataModel.Instance.CurrentApplicationSettings.AsycudaLogin} {BaseDataModel.Instance.CurrentApplicationSettings.AsycudaPassword} ""{directoryName + "\\\\"}"" {
                        (string.IsNullOrEmpty(lastCNumber) ? "" : lastCNumber + "")
                    }{(sMonths + sYears + eMonths + eYears == 0 ? "" : $" {sMonths} {sYears} {eMonths} {eYears}")}",
                    UseShellExecute = false
                };
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;

                /// wait if instance already running 
                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA"))
                             .ToList())
                {
                    Thread.Sleep(1000 * 60);
                }

                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("SikulixIDE"))
                           .ToList())
                {
                    process1.Kill();
                }


                process.Start();
                var timeoutCycles = 0;
                while (!process.HasExited && process.Responding)
                {
                    var rmo = Process.GetProcesses().Select(x => x.MainModule.FileName).ToList();
                    if (timeoutCycles > 1 && !Process.GetProcesses().Where(x =>
                                x.MainWindowTitle.Contains("ASYCUDA") || x.MainWindowTitle.Contains("Acrobat Reader"))
                            .ToList().Any()) break;
                    if (timeoutCycles > WaterNut.DataSpace.Utils._noOfCyclesBeforeHardExit) break;
                    //Console.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Debug.WriteLine($"Waiting {timeoutCycles} Minutes");
                    Thread.Sleep(1000 * 60);
                    timeoutCycles += 1;
                }

                if (!process.HasExited) process.Kill();

                foreach (var process1 in Process.GetProcesses().Where(x => x.MainWindowTitle.Contains("ASYCUDA") || x.MainWindowTitle.Contains("Acrobat Reader"))
                             .ToList())
                {
                    process1.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool ImportComplete(string directoryName, bool redownload, out int lcont, int startYear, bool retryOnblankFile = false)
        {
            try
            {

            lcont = 0;

            var desFolder = directoryName + (directoryName.EndsWith(@"\") ? "" : "\\");
            var overviewFile = Path.Combine(desFolder, "OverView.txt");
            if (File.Exists(overviewFile) || File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-2))
            {


                if (File.GetLastWriteTime(overviewFile) <= DateTime.Now.AddHours(-1)) return false;
                var readAllText = File.ReadAllText(overviewFile);

                if (readAllText == "No Files Found") return !retryOnblankFile;

                var lines = readAllText
                    .Split(new[] { $"\r\n{startYear}\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {

                    return false;
                }



                var existingfiles = 0;

                foreach (var line in lines)
                {
                    lcont += 1;

                    //var p = line.StartsWith($"{DateTime.Now.Year}\t")
                    //    ? line.Replace($"{DateTime.Now.Year}\t", "").Split('\t')
                    //    : line.Split('\t');
                    var p = line.Split('\t').ToList();
                    if (p[0] == startYear.ToString())
                    {
                            p.RemoveAt(0);
                    }


                    if (p.Count < 8) continue;
                    if(!DateTime.TryParse(p[6], out var regDate)) continue;

                    var fileName = Path.Combine(desFolder, $"{p[7] + p[8]}-{p[0]}-{regDate.Year}-{p[5]}.xml");
                    if (File.Exists(fileName))
                    {
                        existingfiles += 1;
                        continue;
                    }
                    return false;
                }

                if (redownload && (DateTime.Now - new FileInfo(overviewFile).LastWriteTime).Minutes > 5) return false;
                return existingfiles != 0;
            }
            else
            {

                return false;
            }

            }
            catch (Exception)
            {

                throw;
            }
        }

     
        public static void SubmitMissingInvoices(FileTypes ft)
        {
            try
            {


                Console.WriteLine("Submit Missing Invoices");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitDocSetWithIncompleteInvoices
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList()
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        if (Utils.GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices").Any<ActionDocSetLogs>()) continue;


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Invoices. {emailIds.FirstOrDefault().ImportedInvoices} were Imported out of {emailIds.FirstOrDefault().TotalInvoices} . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Invoice Date".FormatedSpace(20)}{"Invoice Value".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.InvoiceDate.ToShortDateString().FormatedSpace(20)}{current.InvoiceTotal.Value.ToString().FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", "Error:Missing Invoices",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key.EmailId, Utils.Client, "Error:Missing Invoices", body, contacts, attlst.ToArray());
                        }


                        ctx.SaveChanges();


                        //LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices");


                    }

                }
                SubmitMissingInvoicePDFs(ft);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static List<ActionDocSetLogs> GetDocSetActions(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                return ctx.ActionDocSetLogs.Where(x => x.Actions.Name == actionName && x.AsycudaDocumentSetId == docSetId).ToList();

            }
        }

        public static void SubmitMissingInvoicePDFs(FileTypes ft)
        {
            try
            {


                Console.WriteLine("Submit Missing Invoice PDFs");


                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitMissingInvoicePDFs
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId) // use the more precise filter
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        if (Enumerable.Any<ActionDocSetLogs>(Utils.GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs"))) continue;


                        var body = $"The {emailIds.FirstOrDefault().Declarant_Reference_Number} is missing Invoice PDF Attachments. \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Source File".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SourceFile.FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please email CSV with Coresponding PDF to prevent this error.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", "Error:Missing Invoices PDF Attachments",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key.EmailId, Utils.Client, "Error:Missing Invoices PDF Attachments", body, contacts, attlst.ToArray());
                        }


                        ctx.SaveChanges();


                        //LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs");


                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void SubmitIncompleteEntryData(FileTypes ft)
        {
            try
            {


                Console.WriteLine("Submit Incomplete Entry Data");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts
                        .Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitIncompleteEntryData
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)// ft.AsycudaDocumentSetId == 0 ||
                        .ToList()
                        .GroupBy(x => x.EmailId);
                    foreach (var emailIds in lst)
                    {



                        var body = "The Following Invoices Total do not match Imported Totals . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Supplier Code".FormatedSpace(20)}{"Invoice Total".FormatedSpace(20)}{"Imported Total".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SupplierCode.FormatedSpace(20)}{current.InvoiceTotal.Value.ToString("C").FormatedSpace(20)}{current.ImportedTotal.Value.ToString("C").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();



                        if (emailIds.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", "Error:Incomplete Invoice Data",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key, Utils.Client, "Error:Incomplete Invoice Data", body, contacts, attlst.ToArray());
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
        public static List<FileTypes> GetFileType(string entryType, string format, string fileName)
        {
            return FileTypeManager.GetImportableFileType(entryType, format, fileName);
        }

        public static void RetryImport(int trytimes, string script, bool redownload, string directoryName, int sMonth,
            int sYear, int eMonth, int eYear, int startYear, bool retryOnblankFile = false)
        {
            int lcont;
            for (int i = 0; i < trytimes; i++)
            {
                if (Utils.ImportComplete(directoryName, redownload, out lcont, startYear, retryOnblankFile))
                    break; //ImportComplete(directoryName,false, out lcont);
                Utils.RunSiKuLi(directoryName, script, lcont.ToString(), sMonth, sYear, eMonth, eYear);
                if (Utils.ImportComplete(directoryName, redownload, out lcont, startYear, retryOnblankFile)) break;
            }
        }
    }
}
