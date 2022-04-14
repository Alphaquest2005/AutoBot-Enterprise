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
    public class Utils
    {
        public const int _noOfCyclesBeforeHardExit = 60;
        public static int maxRowsToFindHeader = 10;
        private static int _oneMegaByte = 1000000;


        public static void RecreateEx9()
        {
            var genDocs = FileUtils.CreateEx9(true);

            if (genDocs.Any()) //reexwarehouse process
            {
                FileUtils.ExportEx9Entries();
                FileUtils.AssessEx9Entries();
                FileUtils.DownloadSalesFiles(10, "IM7", false);
                FileUtils.ImportSalesEntries();
                FileUtils.ImportWarehouseErrors();
                RecreateEx9();
                Application.Exit();
            }
            else // reimport and submit to customs
            {
                FileUtils.LinkPDFs();
                FileUtils.SubmitSalesXMLToCustoms();
                FileUtils.CleanupEntries();
                Application.Exit();
            }
        }


        public static void EmailSalesErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "SalesErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                                .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                                .Where(x => x.Status != null)
                                .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>
                    {
                        dataToPrint = errors
                    };
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(FileUtils.Client, directory, $"Sales Errors for {info.Item1.ToString("yyyy-MM-dd")} - {info.Item2.ToString("yyyy-MM-dd")}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile });
            }

        }
        public static void EmailAdjustmentErrors()
        {

            var info = BaseDataModel.CurrentSalesInfo();
            var directory = info.Item4;
            var errorfile = Path.Combine(directory, "AdjustmentErrors.csv");

            using (var ctx = new AllocationQSContext())
            {
                var errors = ctx.AsycudaSalesAndAdjustmentAllocationsExes
                    .Where(x => x.Type == "ADJ")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(x => x.Status != null)
                    .Where(x => x.InvoiceDate >= info.Item1.Date && x.InvoiceDate <= info.Item2.Date).ToList();

                var res = new ExportToCSV<AsycudaSalesAndAdjustmentAllocationsEx, List<AsycudaSalesAndAdjustmentAllocationsEx>>
                    {
                        dataToPrint = errors
                    };
                using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                {
                    Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None, TaskCreationOptions.None, sta);
                }

            }

            using (var ctx = new CoreEntitiesContext())
            {
                var contacts = ctx.Contacts
                    .Where(x => x.Role == "Broker" || x.Role == "Customs" || x.Role == "Clerk")
                    .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .ToList();
                if (File.Exists(errorfile))
                    EmailDownloader.EmailDownloader.SendEmail(FileUtils.Client, directory, $"Adjustment Errors for {info.Item1:yyyy-MM-dd} - {info.Item2:yyyy-MM-dd}", contacts.Select(x => x.EmailAddress).ToArray(), "Please see attached...", new string[] { errorfile });
            }

        }


        public static void ClearDocSetEntryData(int asycudaDocumentSetId)
        {
            using (var dtx = new DocumentDSContext())
            {
                var res = dtx.AsycudaDocumentSetEntryDatas.Where(x =>
                    x.AsycudaDocumentSet.SystemDocumentSet == null
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId);
                if (res.Any())
                {
                    dtx.AsycudaDocumentSetEntryDatas.RemoveRange(res);
                    dtx.SaveChanges();
                }

            }
        }

        public static void ClearDocSetAttachments(int asycudaDocumentSetId, string emailId = null)
        {
            using (var dtx = new DocumentDSContext())
            {
                var res = dtx.AsycudaDocumentSet_Attachments.Where(x =>
                    x.AsycudaDocumentSet.SystemDocumentSet == null
                    && x.AsycudaDocumentSetId == asycudaDocumentSetId && emailId == null || x.EmailId != emailId);
                if (res.Any())
                {
                    dtx.AsycudaDocumentSet_Attachments.RemoveRange(res);
                    dtx.SaveChanges();
                }

            }
        }


        public static void CreateAdjustmentEntries(bool overwrite, string adjustmentType, FileTypes fileType)
        {
            Console.WriteLine($"Create {adjustmentType} Entries");

            // var saleInfo = CurrentSalesInfo();
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var lst = new CoreEntitiesContext().Database
                        .SqlQuery<TODO_AdjustmentsToXML>(
                            $"select * from [TODO-AdjustmentsToXML]  where AsycudaDocumentSetId = {fileType.AsycudaDocumentSetId}" +
                            $"and AdjustmentType = '{adjustmentType}'")//because emails combined// and EmailId = '{fileType.EmailId}'
                        .ToList()
                        .GroupBy(x => x.AsycudaDocumentSetId)
                        .ToList();

                    FileUtils.CreateAdjustmentEntries(overwrite, adjustmentType, lst, fileType.EmailId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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



        public static List<Tuple<AsycudaDocumentSetEx, string>> CurrentPOInfo()
        {
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var poDocSet = ctx.TODO_PODocSet.Where(x =>
                        x.ApplicationSettingsId ==
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                    if (poDocSet.Any())
                    {
                        var itmlst = poDocSet.Select(x => x.AsycudaDocumentSetId).ToList();
                        var docSet = ctx.AsycudaDocumentSetExs.Where(x =>
                            itmlst.Any(z => z == x.AsycudaDocumentSetId)).ToList();
                        var lst = new List<Tuple<AsycudaDocumentSetEx, string>>();
                        foreach (var item in docSet)
                        {

                            var dirPath = StringExtensions.UpdateToCurrentUser(Path.Combine(
                                BaseDataModel.Instance.CurrentApplicationSettings.DataFolder,
                                item.Declarant_Reference_Number));
                            lst.Add(new Tuple<AsycudaDocumentSetEx, string>(item, dirPath));
                        }

                        return lst;
                    }

                    return new List<Tuple<AsycudaDocumentSetEx, string>>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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


                new AdjustmentShortService().AutoMatchItems(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, strLst).Wait();


                new AdjustmentShortService()
                    .ProcessDISErrorsForAllocation(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        strLst).Wait();

                new AllocationsBaseModel()
                    .AllocateSalesByMatchingSalestoAsycudaEntriesOnItemNumber(
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId, false,
                        strLst).Wait();

                new AllocationsBaseModel()
                    .MarkErrors(BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).Wait();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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

        public class UnClassifiedItem
        {
            public string InvoiceNo { get; set; }
            public int LineNumber { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }

        }

        public class BlankLicenseDescription
        {
            public string InvoiceNo { get; set; }
            public int LineNumber { get; set; }
            public string ItemNumber { get; set; }
            public string ItemDescription { get; set; }
            public string TariffCode { get; set; }
            public string LicenseDescription { get; set; }

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

                        if (fileType.Type != "Unknown")
                        {
                            SendBackTooBigEmail(file, fileType);
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

                        if (fileType.AsycudaDocumentSetId != 0)
                            AddUpdateDocSetAttachement(fileType, email, ctx, file, attachment, reference);

                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SendBackTooBigEmail(FileInfo file, FileTypes fileType)
        {
            if (fileType.MaxFileSizeInMB != null && (file.Length / _oneMegaByte) > fileType.MaxFileSizeInMB)
            {
                var errTxt =
                    "Hey,\r\n\r\n" +
                    $@"Attachment: '{file.Name}' is too large to upload into Asycuda ({Math.Round((double)(file.Length / _oneMegaByte), 2)}). Please remove Formatting or Cut into Smaller chuncks and Resend!" +
                    "Thanks\r\n" +
                    "AutoBot";
                EmailDownloader.EmailDownloader.SendBackMsg(fileType.EmailId, FileUtils.Client, errTxt);
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

        private static void AddUpdateDocSetAttachement(FileTypes fileType, Email email, CoreEntitiesContext ctx, FileInfo file,
            Attachments attachment, string newReference)
        {
            var docSetAttachment =
                ctx.AsycudaDocumentSet_Attachments
                    .Include(x => x.Attachments)
                    .FirstOrDefault(x => x.Attachments.FilePath == file.FullName
                                         && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId);
            if (docSetAttachment == null)
            {
                ctx.AsycudaDocumentSet_Attachments.Add(
                    new AsycudaDocumentSet_Attachments(true)
                    {
                        AsycudaDocumentSetId = fileType.AsycudaDocumentSetId,
                        Attachments = attachment,
                        DocumentSpecific = fileType.DocumentSpecific,
                        FileDate = file.LastWriteTime,
                        EmailId = email.EmailId,
                        FileTypeId = fileType.Id,
                        TrackingState = TrackingState.Added
                    });
            }
            else
            {
                docSetAttachment.DocumentSpecific = fileType.DocumentSpecific;
                docSetAttachment.FileDate = file.LastWriteTime;
                docSetAttachment.EmailId = email.EmailId;
                docSetAttachment.FileTypeId = fileType.Id;
                docSetAttachment.Attachments.Reference = newReference;
                docSetAttachment.Attachments.DocumentCode = fileType.DocumentCode;
                docSetAttachment.Attachments.EmailId = email.EmailId;
            }

            ctx.SaveChanges();
        }

        //public static void SaveInfo(FileInfo[] csvFiles, FileTypes fileType, List<EmailInfoMappings> emailMappings)
        //{
        //    foreach (var file in csvFiles.Where(x => x.Name == "Info.txt"))
        //    {
        //        var fileTxt = File.ReadAllLines(file.FullName);
        //        SaveAsycudaDocumentSet(fileTxt, fileType,emailMappings);
        //        // SaveEntryData(oldDocSet, fileTxt);
        //    }
        //}

        //public static void SaveAsycudaDocumentSet(string[] fileTxt, FileTypes fileType, List<EmailInfoMappings> emailMappings)
        //{
        //    try
        //    {


        //        using (var ctx = new CoreEntitiesContext())
        //        {
        //            var res = emailMappings.Where(x => x.UpdateDatabase == true).ToList();
        //            string dbStatement = "";
        //            foreach (var line in fileTxt)
        //            {

        //                var im = res.SelectMany(x => x.InfoMapping.InfoMappingRegEx.Select(z => new
        //                {
        //                    Rx = z,
        //                    Key = Regex.Match(line, z.KeyRegX, RegexOptions.IgnoreCase),
        //                    Field = Regex.Match(line, z.FieldRx, RegexOptions.IgnoreCase)
        //                }))
        //                    .Where(z => z.Key.Success && z.Field.Success).ToList();

        //                //match = Regex.Match(line,
        //                //    @"[.]*((?<Key>[a-zA-Z\(\)]+)[:\s\#]+(?<Value>[a-zA-Z0-9\- :$,/\.]*))", RegexOptions.IgnoreCase);
        //                //res.Where(x =>
        //                //                            x.Key.ToLower() == match.Groups["Key"].Value.Trim().ToLower())

        //                if (im.Any())
        //                    foreach (var infoMapping in im)
        //                    {
        //                        var val = infoMapping.Rx.FieldReplaceRx == null
        //                            ? infoMapping.Field.Groups["Value"].Value.Trim()
        //                            : Regex.Match(
        //                                    Regex.Replace(line, infoMapping.Rx.FieldRx, infoMapping.Rx.FieldReplaceRx,
        //                                        RegexOptions.IgnoreCase), infoMapping.Rx.FieldRx,
        //                                    RegexOptions.IgnoreCase)
        //                                .Value.Trim();

        //                        dbStatement +=
        //                            $@" Update AsycudaDocumentSet Set {infoMapping.Rx.InfoMapping.Field} = '{
        //                                    ReplaceSpecialChar(val,
        //                                        "")
        //                                }' Where AsycudaDocumentSetId = '{fileType.Data.First(z => z.Key == )}';";
        //                    }
        //            }

        //            if (!string.IsNullOrEmpty(dbStatement)) ctx.Database.ExecuteSqlCommand(dbStatement);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

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


        public static bool VerifyCSVImport(FileInfo file)
        {
            var dt = FileUtils.CSV2DataTable(file, "NO");
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
    }
}
