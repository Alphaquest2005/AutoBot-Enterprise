using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using AutoBotUtilities;
using Core.Common.Converters;
using CoreEntities.Business.Entities;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;
using TrackableEntities;
using WaterNut.DataSpace;
using xlsxWriter;
using FileTypes = CoreEntities.Business.Entities.FileTypes;

namespace AutoBot
{
    public class ShipmentUtils
    {
        public static void ImportUnAttachedSummary(FileTypes ft, FileInfo[] fs)
        {
            foreach (var file in fs)
            {
                var reference = XlsxWriter.SaveUnAttachedSummary(file);
                var res = reference.Split('-');
                var riderId = int.Parse(res[1]);
                var rider = new EntryDataDSContext().ShipmentRider.FirstOrDefault(x => x.Id == riderId);
                if (rider != null)
                {
                    ft.EmailId = rider.EmailId;
                    CreateShipmentEmail(ft, fs);
                }

            }

        }

        public static void UpdateSupplierInfo(FileTypes ft, FileInfo[] fs)
        {
            using (var ctx = new EntryDataDSContext() { StartTracking = true })
            {
                ctx.Database.CommandTimeout = 10;
                foreach (var file in fs)
                {
                    var dt = CSVUtils.CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["SupplierCode"].ToString())) continue;
                        var supplierCode = row["SupplierCode"].ToString();
                        var itm = ctx.Suppliers.First(x => x.SupplierCode == supplierCode && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                        itm.SupplierName = row["SupplierName"].ToString();
                        itm.Street = row["SupplierAddress"].ToString();
                        itm.CountryCode = row["CountryCode"].ToString();

                        ctx.SaveChanges();
                    }

                }
            }
        }

        public static void CreateShipmentEmail(FileTypes fileType, FileInfo[] files)
        {
            try
            {

           
                var emailId = fileType.EmailId;
            
                // Todo quick paks etc put Man reg#, bl numer, TotalWeight & Totalfreight in spreadsheet. so invoice can generate the entry.
                // change the required documents to match too

                using (var ctx = new EntryDataDSContext())
                {
                    ctx.Database.ExecuteSqlCommand(@"EXEC [dbo].[PreProcessShipmentSP]");
                }



                var shipments = new Shipment(){ShipmentName = "Next Shipment",EmailId = emailId, TrackingState = TrackingState.Added}
                        .LoadEmailPOs()
                        .LoadEmailInvoices()
                        .LoadEmailRiders()
                        .LoadEmailBL()
                        .LoadEmailManifest()
                        .LoadEmailFreight()
                        .LoadDBBL()
                        .LoadDBManifest()
                        .LoadDBRiders()
                        .LoadDBBL()
                        .LoadDBManifest()
                        .LoadDBRiders()
                        .LoadDBFreight()
                        .LoadDBInvoices()
                        .AutoCorrect()
                        .ProcessShipment()
                    //.SaveShipment()
                    ;
               
                var contacts = new CoreEntitiesContext().Contacts.Where(x => x.Role == "PDF Entries" || x.Role == "Developer" || x.Role == "PO Clerk")
                    .Select(x => x.EmailAddress)
                    .Distinct()
                    .ToArray();


                using (var ctx = new EntryDataDSContext())
                {
                    shipments.ForEach(shipment =>
                    {
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "",
                            $"CSVs for {shipment.ShipmentName}", contacts, shipment.ToString(),
                            shipment.ShipmentAttachments.Select(x => x.Attachments.FilePath).ToArray());

                       
                        ctx.Attachments.AddRange(shipment.ShipmentAttachments.Select(x => x.Attachments).ToList());
                       
                    });
                    ctx.SaveChanges();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void CreateInstructions()
        {
            var dir = new DirectoryInfo(@"C:\Users\josep\OneDrive\Clients\Rouge\2019\October");
            var files = dir.GetFiles().Where(x => Regex.IsMatch(x.Name, @".*(P|F)\d+.xml"));
            if (File.Exists(Path.Combine(dir.FullName, "Instructions.txt")))
            {
                File.Delete(Path.Combine(dir.FullName, "Instructions.txt"));
                File.Delete(Path.Combine(dir.FullName, "InstructionResults.txt"));
            }
            foreach (var file in files)
            {
                File.AppendAllText(Path.Combine(dir.FullName, "Instructions.txt"), $"File\t{file.FullName}\r\n");
                if (File.Exists(file.FullName.Replace("xml", "csv")) && !File.Exists(file.FullName.Replace("xml", "csv.pdf")))
                    File.Copy(file.FullName.Replace("xml", "csv"), file.FullName.Replace("xml", "csv.pdf"));
                File.AppendAllText(Path.Combine(dir.FullName, "Instructions.txt"), $"Attachment\t{file.FullName.Replace("xml", "csv.pdf")}\r\n");
            }
        }

        public static void MapUnClassifiedItems(FileTypes ft, FileInfo[] fs)
        {
            Console.WriteLine("Mapping unclassified items");
            using (var ctx = new InventoryDSContext() { StartTracking = true })
            {
                foreach (var file in fs)
                {
                    var dt = CSVUtils.CSV2DataTable(file, "YES");
                    if (dt.Rows.Count == 0) continue;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.IsNullOrEmpty(row["TariffCode"].ToString())) continue;
                        var itmNumber = row["ItemNumber"].ToString();
                        var itms = ctx.InventoryItems.Where(x => x.ItemNumber == itmNumber && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToList();
                        foreach (var itm in itms)
                        {
                            itm.TariffCode = row["TariffCode"].ToString();
                        }

                        ctx.SaveChanges();
                    }

                }
            }

            EntryDocSetUtils.SetFileTypeDocSetToLatest(ft);

        }

        public static void ReadMISMatches(DataTable misMatches, DataTable poTemplate)
        {
            try
            {

                var misHeaderRow = misMatches.Rows[0].ItemArray.ToList();
                var poHeaderRow = poTemplate.Rows[0].ItemArray.ToList();
                foreach (DataRow misMatch in misMatches.Rows)
                {
                    if (misMatch == misMatches.Rows[0]) continue;
                    var InvoiceNo = misMatch[misHeaderRow.IndexOf("InvoiceNo")].ToString();
                    var invItemCode = misMatch[misHeaderRow.IndexOf("INVItemCode")].ToString();
                    var poItemCode = misMatch[misHeaderRow.IndexOf("POItemCode")].ToString();
                    var poNumber = misMatch[misHeaderRow.IndexOf("PONumber")].ToString();
                    var invDetailId = misMatch[misHeaderRow.IndexOf("INVDetailsId")].ToString();
                    //var poDetailId = misMatch[misHeaderRow.IndexOf("PODetailsId")].ToString();
                    if (!string.IsNullOrEmpty(poNumber) &&
                        !string.IsNullOrEmpty(InvoiceNo) &&
                        !string.IsNullOrEmpty(poItemCode) &&
                        !string.IsNullOrEmpty(invItemCode))
                    {

                        DataRow row;
                        var addrow = true;// changed to false because when importing in portage it doubling the errors because they get imported in importData function
                        if (string.IsNullOrEmpty(poTemplate.Rows[1][poHeaderRow.IndexOf("PO Number")].ToString()))
                        {
                            row = poTemplate.Rows[1];
                            addrow = false;
                        }
                        else
                        {
                            row = poTemplate.NewRow();
                        }

                        
                        


                        row[poHeaderRow.IndexOf("PO Number")] = misMatch[misHeaderRow.IndexOf("PONumber")];
                        row[poHeaderRow.IndexOf("Date")] = poTemplate.Rows[1][poHeaderRow.IndexOf("Date")];
                        row[poHeaderRow.IndexOf("PO Item Number")] = poItemCode;
                        row[poHeaderRow.IndexOf("Supplier Item Number")] = invItemCode;
                        row[poHeaderRow.IndexOf("PO Item Description")] =
                            misMatch[misHeaderRow.IndexOf("PODescription")];
                        row[poHeaderRow.IndexOf("Supplier Item Description")] =
                            misMatch[misHeaderRow.IndexOf("INVDescription")];
                        row[poHeaderRow.IndexOf("Cost")] =
                            ((double) misMatch[misHeaderRow.IndexOf("INVCost")] /
                             ((misHeaderRow.IndexOf("INVSalesFactor") > -1
                               && !string.IsNullOrEmpty(misMatch[misHeaderRow.IndexOf("INVSalesFactor")].ToString()))
                                 ? Convert.ToInt32(misMatch[misHeaderRow.IndexOf("INVSalesFactor")])
                                 : 1));
                        row[poHeaderRow.IndexOf("Quantity")] = misMatch[misHeaderRow.IndexOf("POQuantity")];
                        row[poHeaderRow.IndexOf("Total Cost")] = misMatch[misHeaderRow.IndexOf("INVTotalCost")];

                        //if (!string.IsNullOrEmpty(invDetailId) && int.TryParse(invDetailId, out int invId))
                        //{
                        //    InvoiceDetails invDetail;
                        //    invDetail = new EntryDataDSContext().ShipmentInvoiceDetails.FirstOrDefault(x => x.Id == invId);
                        //    if (invDetail != null)
                        //    {
                        //        //row[poHeaderRow.IndexOf("FileLineNumber")] = invDetail.FileLineNumber;
                        //    }
                        //}

                        if (addrow) poTemplate.Rows.Add(row);

                        ImportInventoryMapping(invItemCode, misMatch, misHeaderRow, poItemCode);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void ImportInventoryMapping(string invItemCode, DataRow misMatch, List<object> misHeaderRow, string poItemCode)
        {
            using (var ctx = new EntryDataDSContext())
            {
                InvoiceDetails invRow;
                EntryDataDetails poRow;

                var invItm = ctx.InventoryItems.Include(x => x.AliasItems).FirstOrDefault(x =>
                    x.ItemNumber == invItemCode
                    && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (invItm == null)
                {
                    invItm = new InventoryItems()
                    {
                        ApplicationSettingsId =
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        Description = misMatch[misHeaderRow.IndexOf("INVDescription")].ToString(),
                        ItemNumber = invItemCode,
                        TrackingState = TrackingState.Added
                    };
                    ctx.InventoryItems.Add(invItm);
                }

                var poItm = ctx.InventoryItems.Include(x => x.AliasItems).FirstOrDefault(x =>
                    x.ItemNumber == poItemCode && x.ApplicationSettingsId ==
                    BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId);
                if (poItm == null)
                {
                    poItm = new InventoryItems()
                    {
                        ApplicationSettingsId =
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId,
                        Description = misMatch[misHeaderRow.IndexOf("PODescription")].ToString(),
                        ItemNumber = poItemCode,
                        TrackingState = TrackingState.Added
                    };
                    ctx.InventoryItems.Add(poItm);
                }

                if (!poItm.AliasItems.Any(x => x.AliasItemId == invItm.Id) &&
                    !invItm.AliasItems.Any(x => x.InventoryItemId == poItm.Id))
                {
                    ctx.InventoryItemAlias.Add(new InventoryItemAlias(true)
                    {
                        InventoryItems = poItm,
                        AliasItem = invItm,
                        AliasName = invItm.ItemNumber,
                        TrackingState = TrackingState.Added
                    });
                }

                //var itmAlias = ctx.InventoryItemAlias
                ctx.SaveChanges();
            }
        }

        public static void SubmitUnclassifiedItems(FileTypes ft)
        {


            try
            {
                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;




                using (var ctx = new CoreEntitiesContext())
                {

                    var emails = ctx.TODO_SubmitUnclassifiedItems
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId && x.EmailId != null)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {

                        //if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;


                        var errorfile = Path.Combine(directory, $"UnclassifiedItems-{email.Key.AsycudaDocumentSetId}.csv");
                        var errors = email.Select(x => new Utils.UnClassifiedItem()
                        {
                            InvoiceNo = x.InvoiceNo,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode
                        }).ToList();


                        var res =
                            new ExportToCSV<Utils.UnClassifiedItem,
                                List<Utils.UnClassifiedItem>>()
                            {
                                dataToPrint = errors
                            };
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = ctx.Contacts
                            .Where(x => x.Role == "Broker")
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();
                        if (File.Exists(errorfile))
                            EmailDownloader.EmailDownloader.ForwardMsg(email.Key.EmailId, Utils.Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

                        // LogDocSetAction(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems");


                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SubmitDocSetUnclassifiedItems(FileTypes fileType)
        {

            try
            {

                using (var ctx = new CoreEntitiesContext())
                {

                    var emails = ctx.TODO_SubmitUnclassifiedItems
                        .Where(x => //x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId &&
                            x.EmailId != null
                            && x.AsycudaDocumentSetId == fileType.AsycudaDocumentSetId)
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId }).ToList();
                    foreach (var email in emails)
                    {

                        // if (GetDocSetActions(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems").Any()) continue;
                        var info = Enumerable.FirstOrDefault<Tuple<AsycudaDocumentSet, string>>(POUtils.CurrentPOInfo(email.Key.AsycudaDocumentSetId));
                        var directory = info.Item2;

                        var errorfile = Path.Combine(directory, $"UnclassifiedItems-{email.Key.AsycudaDocumentSetId}.csv");
                        var errors = email.Select(x => new Utils.UnClassifiedItem()
                        {
                            InvoiceNo = x.InvoiceNo,
                            ItemNumber = x.ItemNumber,
                            LineNumber = x.LineNumber.GetValueOrDefault(),
                            ItemDescription = x.ItemDescription,
                            TariffCode = x.TariffCode
                        }).ToList();


                        var res =
                            new ExportToCSV<Utils.UnClassifiedItem,
                                List<Utils.UnClassifiedItem>>()
                            {
                                dataToPrint = errors
                            };
                        using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                        {
                            Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta);
                        }

                        var contacts = ctx.Contacts
                            .Where(x => x.Role == "Broker")
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();
                        if (File.Exists(errorfile))
                            EmailDownloader.EmailDownloader.ForwardMsg(email.Key.EmailId, Utils.Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile });

                        // LogDocSetAction(email.Key.AsycudaDocumentSetId, "SubmitUnclassifiedItems");


                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void SubmitIncompleteSuppliers(FileTypes ft)
        {


            try
            {


                var info = BaseDataModel.CurrentSalesInfo();
                var directory = info.Item4;


                using (var ctx = new CoreEntitiesContext())
                {

                    var suppliers = ctx.TODO_SubmitIncompleteSupplierInfo
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .GroupBy(x => new { x.SupplierCode }).ToList();

                    if (!suppliers.Any()) return;

                    if (Enumerable.Any<ActionDocSetLogs>(Utils.GetDocSetActions(ft.AsycudaDocumentSetId, "SubmitIncompleteSuppliers"))) return;

                    var errorfile = Path.Combine(directory, $"IncompleteSuppliers.csv");
                    var errors = suppliers.SelectMany(x => x.Select(z => new IncompleteSupplier()
                    {
                        SupplierName = z.SupplierName,
                        SupplierCode = z.SupplierCode,
                        SupplierAddress = z.Street,
                        CountryCode = z.CountryCode
                    }).ToList()).ToList();


                    var res =
                        new ExportToCSV<IncompleteSupplier,
                            List<IncompleteSupplier>>()
                        {
                            dataToPrint = errors
                        };
                    using (var sta = new StaTaskScheduler(numberOfThreads: 1))
                    {
                        Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                            TaskCreationOptions.None, sta);
                    }

                    var contacts = new CoreEntitiesContext().Contacts
                        .Where(x => x.Role == "Broker" || x.Role == "PO Clerk").ToList();
                    if (File.Exists(errorfile))
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, directory,
                            $"Error:InComplete Supplier Info", contacts.Select(x => x.EmailAddress).ToArray(),
                            "Please Fill out the attached Supplier Info and resend CSV...",
                            new string[] { errorfile });

                    //LogDocSetAction(sup.Key.AsycudaDocumentSetId, "SubmitIncompleteSuppliers");


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public static void SubmitInadequatePackages(FileTypes ft)
        {


            try
            {
                Console.WriteLine("Submit Inadequate Packages");

                // var saleInfo = CurrentSalesInfo();


                using (var ctx = new CoreEntitiesContext())
                {
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress).ToArray();
                    var lst = ctx.TODO_SubmitInadequatePackages
                        //.Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)//ft.AsycudaDocumentSetId == 0 ||
                        .ToList();

                    foreach (var docSet in lst)
                    {

                        if (Enumerable.Any<ActionDocSetLogs>(Utils.GetDocSetActions(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages"))) continue;

                        var body = $"Error: Inadequate Lines\r\n" +
                                   $"\r\n" +
                                   $"The {docSet.Declarant_Reference_Number} has {docSet.TotalPackages} Packages with {docSet.TotalLines} Lines.\r\n" +
                                   $"Your Maximum Lines per Entry is {docSet.MaxEntryLines}.\r\n" +
                                   $"The minium required packages is {docSet.RequiredPackages}\r\n" +
                                   $"Please increase the Maxlines using \"MaxLines:\"\r\n" +
                                   $"Please note the System will automatically switch from \"Entry per Invoice\" to \"Group Invoices per Entry\", if there are not enough packages per invoice. \r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                        EmailDownloader.EmailDownloader.SendEmail(Utils.Client, "", $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray());

                        //LogDocSetAction(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages");



                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}