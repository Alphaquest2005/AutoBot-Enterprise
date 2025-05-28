using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;
using xlsxWriter;
// Added for AsycudaDocumentSet, xcuda_Declarant etc.
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet; // Keep alias for clarity if needed elsewhere
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using System.Text; // Added for StringBuilder
namespace AutoBot
{
    using Serilog;

    public class ShipmentUtils
    {
        public static async Task ImportUnAttachedSummary(FileTypes ft, FileInfo[] fs, ILogger log)
        {
            try
            {
                foreach (var file in fs)
                {
                    var reference = XlsxWriter.SaveUnAttachedSummary(file);
                    ft.EmailId = reference;
                    await CreateShipmentEmail(ft, fs, log).ConfigureAwait(false);
                    //}

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static async Task UpdateSupplierInfo(FileTypes ft, FileInfo[] fs)
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

                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }

                }
            }
        }

        public static async Task<bool> CreateShipmentEmail(FileTypes fileType, FileInfo[] files, ILogger log)
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

                ShipmentExtensions.shipmentInvoices = null;

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



                var sent = false;

                shipments.ForEach(async shipment => { sent = await EmailShipment(shipment, log).ConfigureAwait(false); });



                return sent;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    var entityName = validationError.Entry.Entity.GetType().Name;
                    foreach (var error in validationError.ValidationErrors)
                    {
                        Console.WriteLine($"Validation Error in Entity: {entityName}, Field: {error.PropertyName}, Error: {error.ErrorMessage}, Value: {validationError.Entry.CurrentValues[error.PropertyName]}");
                    }
                }

                await BaseDataModel.EmailExceptionHandlerAsync(ex, log).ConfigureAwait(false);
                throw; // Re-throw the exception to preserve the stack trace
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await BaseDataModel.EmailExceptionHandlerAsync(e, log).ConfigureAwait(false);
                throw;
            }
        }

        private static async Task<bool> EmailShipment(Shipment shipment, ILogger log)
        {
            bool sent;
            using (var ctx = new EntryDataDSContext())
            {
                // Check if correction is needed before routing email
                var totalZeroSum = shipment.Invoices?.Sum(x => x.TotalsZero) ?? 0;

                // If there are issues, attempt correction first
                if (Math.Abs(totalZeroSum) > 0.01)
                {
                    log.Information("Import issues detected for EmailId: {EmailId}, TotalZeroSum: {TotalZeroSum}. Attempting correction.",
                        shipment.EmailId, totalZeroSum);

                    var correctionSuccessful = await AttemptImportCorrection(shipment.EmailId, log).ConfigureAwait(false);

                    if (correctionSuccessful)
                    {
                        // Reload shipment data after correction
                        shipment = new Shipment(){ShipmentName = shipment.ShipmentName, EmailId = shipment.EmailId, TrackingState = TrackingState.Added}
                            .LoadEmailInvoices();
                        totalZeroSum = shipment.Invoices?.Sum(x => x.TotalsZero) ?? 0;

                        log.Information("Correction completed for EmailId: {EmailId}, New TotalZeroSum: {TotalZeroSum}",
                            shipment.EmailId, totalZeroSum);
                    }
                }

                var contacts = Math.Abs(totalZeroSum) < 0.01
                    ? new CoreEntitiesContext().Contacts.Where(x => x.Role == "Shipments").Select(x => x.EmailAddress).Distinct().ToArray()
                    : new CoreEntitiesContext().Contacts.Where(x => x.Role == "Developer" || x.Role == "PO Clerk").Select(x => x.EmailAddress).Distinct().ToArray();

                await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "",
                    $"Shipment: {shipment.ShipmentName}", contacts, shipment.ToString(),
                    shipment.ShipmentAttachments.Select(x => x.Attachments.FilePath).ToArray(), log).ConfigureAwait(false);

                sent = true;
                ctx.Attachments.AddRange(shipment.ShipmentAttachments.Select(x => x.Attachments).ToList());
                ctx.SaveChanges();
            }

            return sent;
        }

        public static Task CreateInstructions()
        {
            var dir = new DirectoryInfo(@"D:\OneDrive\Clients\Rouge\2019\October");
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
            return Task.CompletedTask;
        }

        public static async Task MapUnClassifiedItems(FileTypes ft, FileInfo[] fs, ILogger log)
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
                        var itms = await ctx.InventoryItems.Where(x => x.ItemNumber == itmNumber && x.ApplicationSettingsId ==
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId).ToListAsync().ConfigureAwait(false);
                        foreach (var itm in itms)
                        {
                            itm.TariffCode = row["TariffCode"].ToString();
                        }

                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                    }

                }
            }

            EntryDocSetUtils.SetFileTypeDocSetToLatest(log, ft);

        }




        public static async Task SubmitUnclassifiedItems(FileTypes ft, ILogger log)
        {


            try
            {
                var info = await BaseDataModel.CurrentSalesInfo(-1, log).ConfigureAwait(false);
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
                            await Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta).ConfigureAwait(false);
                        }

                        var contacts = ctx.Contacts
                            .Where(x => x.Role == "Broker")
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();
                        if (File.Exists(errorfile))
                           await EmailDownloader.EmailDownloader.ForwardMsgAsync(email.Key.EmailId, Utils.Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile }, log).ConfigureAwait(false);

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

        public static async Task SubmitDocSetUnclassifiedItems(FileTypes fileType, ILogger log)
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
                            await Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                                TaskCreationOptions.None, sta).ConfigureAwait(false);
                        }

                        var contacts = ctx.Contacts
                            .Where(x => x.Role == "Broker")
                            .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                            .ToList();
                        if (File.Exists(errorfile))
                            await EmailDownloader.EmailDownloader.ForwardMsgAsync(email.Key.EmailId, Utils.Client,
                                $"Error:UnClassified Items",
                                "Please Fill out the attached Tarrif Codes and resend CSV...",
                                contacts.Select(x => x.EmailAddress).ToArray(),
                                new string[] { errorfile }, log).ConfigureAwait(false);

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

        public static async Task SubmitIncompleteSuppliers(FileTypes ft, ILogger log)
        {


            try
            {


                var info = await BaseDataModel.CurrentSalesInfo(-1, log).ConfigureAwait(false);
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
                        await Task.Factory.StartNew(() => res.SaveReport(errorfile), CancellationToken.None,
                            TaskCreationOptions.None, sta).ConfigureAwait(false);
                    }

                    var contacts = new CoreEntitiesContext().Contacts
                        .Where(x => x.Role == "Broker" || x.Role == "PO Clerk").ToList();
                    if (File.Exists(errorfile))
                        await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, directory,
                            $"Error:InComplete Supplier Info", contacts.Select(x => x.EmailAddress).ToArray(),
                            "Please Fill out the attached Supplier Info and resend CSV...",
                            new string[] { errorfile }, log).ConfigureAwait(false);

                    //LogDocSetAction(sup.Key.AsycudaDocumentSetId, "SubmitIncompleteSuppliers");


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public static async Task SubmitInadequatePackages(FileTypes ft, ILogger log)
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
                                   $"Please note the System will automatically switch from \"Entry per Template\" to \"Group Invoices per Entry\", if there are not enough packages per invoice. \r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();
                       await EmailDownloader.EmailDownloader.SendEmailAsync(Utils.Client, "", $"Shipment: {docSet.Declarant_Reference_Number}",
                            contacts, body, attlst.ToArray(), log).ConfigureAwait(false);

                        //LogDocSetAction(docSet.AsycudaDocumentSetId, "SubmitInadequatePackages");



                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static async Task ClearShipmentData(FileTypes fileType, FileInfo[] fileInfos)
        {
            using (var ctx = new EntryDataDSContext())
            {
               await ctx.Database.ExecuteSqlCommandAsync($@"DELETE FROM ShipmentBL WHERE (EmailId = N'{fileType.EmailId}')
                                                    delete from ShipmentInvoice WHERE (EmailId = N'{fileType.EmailId}')
                                                    delete from entrydata WHERE (EmailId = N'{fileType.EmailId}')
                                                    delete from ShipmentManifest WHERE (EmailId = N'{fileType.EmailId}')").ConfigureAwait(false);
            }
        }
// --- Start of Added/Corrected Methods ---

        public static async Task ImportShipmentInfoFromTxt(FileTypes ft, FileInfo[] files, ILogger log)
        {
            Console.WriteLine($"Starting ImportShipmentInfoFromTxt for FileType: {ft.Id}");
            var infoFile = files.FirstOrDefault(f => f.Name.Equals("Info.txt", StringComparison.OrdinalIgnoreCase));

            if (infoFile == null || !infoFile.Exists)
            {
                Console.WriteLine("Error: Info.txt not found in the provided files.");
                throw new FileNotFoundException("Info.txt not found for ImportShipmentInfoFromTxt action.", "Info.txt");
            }

            Console.WriteLine($"Found Info.txt: {infoFile.FullName}");
            var infoData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var lines = File.ReadAllLines(infoFile.FullName);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        // Don't add empty values or known placeholders
                        if (!infoData.ContainsKey(key) && !string.IsNullOrWhiteSpace(value) && !value.Equals("Consignee Address Not Found", StringComparison.OrdinalIgnoreCase) && !value.Equals("Not Found", StringComparison.OrdinalIgnoreCase))
                        {
                            infoData.Add(key, value);
                        }
                    }
                }
                Console.WriteLine($"Parsed {infoData.Count} key-value pairs from Info.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing Info.txt: {ex.Message}");
                throw; // Rethrow as this is critical
            }

            // --- Extract Key Identifier (BL Number) ---
            if (!infoData.TryGetValue("BL", out var blNumber) || string.IsNullOrWhiteSpace(blNumber))
            {
                Console.WriteLine("Error: BL Number not found or empty in Info.txt.");
                throw new ApplicationException("BL Number not found or empty in Info.txt.");
            }
            Console.WriteLine($"Using BLNumber '{blNumber}' as key identifier (Declarant_Reference_Number).");
            string declarantRef = blNumber;

            try
            {
                using (var ctx = new DocumentDSContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    ctx.Configuration.AutoDetectChangesEnabled = false;

                    // --- Find or Create AsycudaDocumentSet ---
                    var docSet = ctx.AsycudaDocumentSets
                                    .FirstOrDefault(ds => ds.Declarant_Reference_Number == declarantRef &&
                                                           ds.ApplicationSettingsId == ft.ApplicationSettingsId);

                    if (docSet == null)
                    {
                        Console.WriteLine($"AsycudaDocumentSet not found for Ref '{declarantRef}'. Creating new one.");

                        // --- Parse required fields BEFORE creating ---
                        if (!infoData.TryGetValue("Currency", out var currencyCode) || string.IsNullOrWhiteSpace(currencyCode))
                        {
                            throw new ApplicationException("Required field 'Currency' not found or empty in Info.txt.");
                        }
                        // Assuming default exchange rate if not provided
                        double exchangeRate = 1.0;
                        if (infoData.TryGetValue("Exchange Rate", out var exRateStr) && double.TryParse(exRateStr, out var parsedRate))
                        {
                            exchangeRate = parsedRate;
                        }
                         // FreightCurrencyCode has a DB default 'USD', so we don't strictly need to set it unless different

                        docSet = new AsycudaDocumentSet(true)
                        {
                            TrackingState = TrackingState.Added,
                            ApplicationSettingsId = ft.ApplicationSettingsId,
                            Declarant_Reference_Number = declarantRef,
                            EntryTimeStamp = DateTime.Now,
                            // Set required fields found above
                            Currency_Code = currencyCode,
                            Exchange_Rate = exchangeRate,
                            FreightCurrencyCode = infoData.TryGetValue("Freight Currency", out var frCurr) ? frCurr : "USD" // Use info or default
                        };
                        ctx.AsycudaDocumentSets.Add(docSet);
                        ctx.SaveChanges(); // Save to get the ID
                        Console.WriteLine($"Created new AsycudaDocumentSet with ID: {docSet.AsycudaDocumentSetId}");
                    }
                    else
                    {
                        Console.WriteLine($"Found existing AsycudaDocumentSet with ID: {docSet.AsycudaDocumentSetId}");
                        ctx.Entry(docSet).State = EntityState.Modified;
                    }

                    // --- Update AsycudaDocumentSet fields ---
                    if (infoData.TryGetValue("Manifest", out var manifest)) docSet.Manifest_Number = manifest;
                    if (infoData.TryGetValue("Freight", out var freightStr) && double.TryParse(freightStr, out var freight)) docSet.TotalFreight = freight;
                    if (infoData.TryGetValue("Weight(kg)", out var weightStr) && double.TryParse(weightStr, out var weight)) docSet.TotalWeight = weight;
                    if (infoData.TryGetValue("Currency", out var currency)) docSet.Currency_Code = currency;
                    if (infoData.TryGetValue("Country of Origin", out var origin)) docSet.Country_of_origin_code = origin;
                    if (infoData.TryGetValue("Total Invoices", out var totalInvStr) && int.TryParse(totalInvStr, out var totalInv)) docSet.TotalInvoices = totalInv;
                    if (infoData.TryGetValue("Packages", out var packagesStr) && int.TryParse(packagesStr, out var packages)) docSet.TotalPackages = packages;
                    if (infoData.TryGetValue("Freight Currency", out var freightCurr)) docSet.FreightCurrencyCode = freightCurr;
                    if (infoData.TryGetValue("Office", out var office)) docSet.Office = office; // Corrected: Assign to docSet.Office

                    // --- Find/Create/Update xcuda_ASYCUDA & related ---
                    var extendedProps = ctx.xcuda_ASYCUDA_ExtendedProperties
                                           .Include(x => x.xcuda_ASYCUDA.xcuda_Declarant)
                                           .Include(x => x.xcuda_ASYCUDA.xcuda_Transport) // Include Transport
                                           .FirstOrDefault(x => x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId);

                    xcuda_ASYCUDA asycudaDoc = null;
                    if (extendedProps == null)
                    {
                        Console.WriteLine($"Creating xcuda_ASYCUDA and ExtendedProperties for DocSetId {docSet.AsycudaDocumentSetId}.");
                        asycudaDoc = new xcuda_ASYCUDA(true) { TrackingState = TrackingState.Added };
                        ctx.xcuda_ASYCUDA.Add(asycudaDoc);
                        ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges(); // Save ASYCUDA first

                        extendedProps = new xcuda_ASYCUDA_ExtendedProperties(true)
                        {
                            TrackingState = TrackingState.Added,
                            ASYCUDA_Id = asycudaDoc.ASYCUDA_Id,
                            AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                            ImportComplete = false
                        };
                        ctx.xcuda_ASYCUDA_ExtendedProperties.Add(extendedProps);
                    }
                    else
                    {
                        Console.WriteLine($"Found existing ExtendedProperties for DocSetId {docSet.AsycudaDocumentSetId}.");
                        asycudaDoc = extendedProps.xcuda_ASYCUDA;
                        ctx.Entry(extendedProps).State = EntityState.Modified;
                        if (asycudaDoc != null) ctx.Entry(asycudaDoc).State = EntityState.Modified;
                    }

                    // Update/Create Transport for Location_of_Goods
                    if (asycudaDoc != null && infoData.TryGetValue("Location of Goods", out var location))
                    {
                        var transport = asycudaDoc.xcuda_Transport.FirstOrDefault(); // Assuming one transport record per ASYCUDA doc
                        if (transport == null)
                        {
                            transport = new xcuda_Transport(true) { TrackingState = TrackingState.Added, ASYCUDA_Id = asycudaDoc.ASYCUDA_Id };
                            asycudaDoc.xcuda_Transport.Add(transport); // Add to collection
                            ctx.Entry(transport).State = EntityState.Added;
                        }
                        else
                        {
                            ctx.Entry(transport).State = EntityState.Modified;
                        }
                        transport.Location_of_goods = location; // Corrected: Assign to xcuda_Transport
                    }

                    // Update/Create Declarant (Consignee) linked to xcuda_ASYCUDA
                    if (asycudaDoc != null)
                    {
                        var declarant = asycudaDoc.xcuda_Declarant;
                        if (declarant == null)
                        {
                            declarant = new xcuda_Declarant(true) { TrackingState = TrackingState.Added, ASYCUDA_Id = asycudaDoc.ASYCUDA_Id };
                            asycudaDoc.xcuda_Declarant = declarant;
                            ctx.Entry(declarant).State = EntityState.Added;
                        }
                        else
                        {
                            ctx.Entry(declarant).State = EntityState.Modified;
                        }

                        if (infoData.TryGetValue("Consignee Code", out var consigneeCode)) declarant.Declarant_code = consigneeCode;
                        if (infoData.TryGetValue("Consignee Name", out var consigneeName)) declarant.Declarant_name = consigneeName;
                    }
                    else
                    {
                         Console.WriteLine($"Warning: Could not find or create xcuda_ASYCUDA for DocSetId {docSet.AsycudaDocumentSetId}. Transport and Consignee info not updated.");
                    }


                    // --- Save All Changes ---
                    ctx.ChangeTracker.DetectChanges();
                    ctx.SaveChanges();
                    Console.WriteLine($"Saved DB changes related to DocSetId: {docSet.AsycudaDocumentSetId}");

                    // --- Update FileType Context ---
                    ft.AsycudaDocumentSetId = docSet.AsycudaDocumentSetId;
                    ft.DocSetRefernece = docSet.Declarant_Reference_Number;

                    // Correctly handle ft.Data (List<KeyValuePair<string, string>>)
                    var dataList = ft.Data as List<KeyValuePair<string, string>>;
                    if (dataList != null)
                    {
                        var existingEntryIndex = dataList.FindIndex(kvp => kvp.Key.Equals("ShipmentKey", StringComparison.OrdinalIgnoreCase));
                        if (existingEntryIndex >= 0)
                        {
                             Console.WriteLine("ShipmentKey already exists in FileType.Data, not overwriting.");
                        }
                        else
                        {
                            dataList.Add(new KeyValuePair<string, string>("ShipmentKey", blNumber)); // Add new
                        }
                    }
                    else
                    {
                         Console.WriteLine("Warning: FileType.Data is not a List<KeyValuePair<string, string>>. Cannot store ShipmentKey.");
                    }

                    Console.WriteLine($"Updated FileType context: AsycudaDocumentSetId={ft.AsycudaDocumentSetId}, ShipmentKey='{blNumber}'");

                } // end using DocumentDSContext
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"Error processing Info.txt and updating database: {ex.Message}\n{ex.StackTrace}");
                 await BaseDataModel.EmailExceptionHandlerAsync(ex, log).ConfigureAwait(false);
                 throw;
            }
        } // End of ImportShipmentInfoFromTxt

        public static void SaveShipmentInfoToFile(Shipment shipment, string outputDirectory)
        {
             if (shipment == null || string.IsNullOrWhiteSpace(outputDirectory))
             {
                 Console.WriteLine("Error: Shipment object or output directory is null/empty in SaveShipmentInfoToFile.");
                 return;
             }

             try
             {
                 Directory.CreateDirectory(outputDirectory); // Ensure output directory exists

                 string blNumber = shipment.BLNumber;
                 if (string.IsNullOrWhiteSpace(blNumber))
                 {
                     Console.WriteLine("Warning: BLNumber is missing from Shipment object. Cannot generate ShipmentKey.");
                     blNumber = $"MISSINGBL-{Guid.NewGuid()}";
                 }

                 StringBuilder infoContent = new StringBuilder();
                 infoContent.AppendLine($"ShipmentKey: {blNumber}");
                 infoContent.AppendLine($"Expected Entries: {shipment.ExpectedEntries}");
                 infoContent.AppendLine($"Manifest: {shipment.ManifestNumber ?? "Not Found"}");
                 infoContent.AppendLine($"Consignee Code: {shipment.ConsigneeCode ?? "Not Found"}");
                 infoContent.AppendLine($"Consignee Name: {shipment.ConsigneeName ?? "Not Found"}");
                 infoContent.AppendLine($"Consignee Address: {shipment.ConsigneeAddress ?? "Not Found"}");
                 infoContent.AppendLine($"BL: {shipment.BLNumber ?? "Not Found"}");
                 infoContent.AppendLine($"Freight: {shipment.Freight?.ToString() ?? "Not Found"}");
                 infoContent.AppendLine($"Weight(kg): {shipment.WeightKG?.ToString() ?? "Not Found"}");
                 infoContent.AppendLine($"Currency: {shipment.Currency ?? "Not Found"}");
                 infoContent.AppendLine($"Country of Origin: {shipment.Origin ?? "Not Found"}");
                 infoContent.AppendLine($"Total Invoices: {shipment.TotalInvoices?.ToString() ?? "Not Found"}");
                 infoContent.AppendLine($"Packages: {shipment.Packages?.ToString() ?? "Not Found"}");
                 infoContent.AppendLine($"Freight Currency: {shipment.FreightCurrency ?? "Not Found"}");
                 infoContent.AppendLine($"Location of Goods: {shipment.Location ?? "Not Found"}");
                 infoContent.AppendLine($"Office: {shipment.Office ?? "Not Found"}");

                 string filePath = Path.Combine(outputDirectory, "Info.txt");
                 File.WriteAllText(filePath, infoContent.ToString());
                 Console.WriteLine($"Successfully saved shipment info to: {filePath}");
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Error saving shipment info to file: {ex.Message}");
             }
        } // End of SaveShipmentInfoToFile

        /// <summary>
        /// Attempts to correct import issues for a given email by re-processing with OCR corrections
        /// </summary>
        public static async Task<bool> AttemptImportCorrection(string emailId, ILogger log)
        {
            try
            {
                log.Information("Starting import correction for EmailId: {EmailId}", emailId);

                using (var ctx = new EntryDataDSContext())
                {
                    // Get all shipment invoices with issues for this email
                    var invoicesWithIssues = ctx.ShipmentInvoice
                        .Include(x => x.InvoiceDetails)
                        .Where(x => x.EmailId == emailId)
                        .ToList()
                        .Where(x => Math.Abs(x.TotalsZero) > 0.01)
                        .ToList();

                    if (!invoicesWithIssues.Any())
                    {
                        log.Information("No invoices with issues found for EmailId: {EmailId}", emailId);
                        return true;
                    }

                    log.Information("Found {Count} invoices with issues for EmailId: {EmailId}",
                        invoicesWithIssues.Count, emailId);

                    var correctionSuccessful = false;

                    foreach (var invoice in invoicesWithIssues)
                    {
                        var invoiceCorrected = await CorrectSingleInvoice(invoice, log).ConfigureAwait(false);
                        if (invoiceCorrected)
                        {
                            correctionSuccessful = true;
                        }
                    }

                    if (correctionSuccessful)
                    {
                        await ctx.SaveChangesAsync().ConfigureAwait(false);
                        log.Information("Import correction completed successfully for EmailId: {EmailId}", emailId);
                    }

                    return correctionSuccessful;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error during import correction for EmailId: {EmailId}", emailId);
                return false;
            }
        }

        /// <summary>
        /// Action method for FileUtils - corrects import issues for files
        /// </summary>
        public static async Task CorrectImportIssues(FileTypes fileType, FileInfo[] files, ILogger log)
        {
            if (string.IsNullOrEmpty(fileType.EmailId))
            {
                log.Warning("No EmailId provided for import correction");
                return;
            }

            await AttemptImportCorrection(fileType.EmailId, log).ConfigureAwait(false);
        }

        private static async Task<bool> CorrectSingleInvoice(ShipmentInvoice invoice, ILogger log)
        {
            try
            {
                log.Information("Attempting to correct invoice {InvoiceId} ({InvoiceNo}) with TotalsZero: {TotalsZero}",
                    invoice.Id, invoice.InvoiceNo, invoice.TotalsZero);

                // Apply common corrections based on invoice type and issue patterns
                var corrected = false;

                // Determine invoice type from invoice number pattern
                var invoiceType = DetermineInvoiceType(invoice);

                // Apply specific corrections based on the type of issue
                corrected = await ApplyCommonCorrections(invoice, invoiceType, log).ConfigureAwait(false);

                if (corrected)
                {
                    log.Information("Successfully corrected invoice {InvoiceId}", invoice.Id);
                }

                return corrected;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error correcting invoice {InvoiceId}", invoice.Id);
                return false;
            }
        }

        private static string DetermineInvoiceType(ShipmentInvoice invoice)
        {
            if (string.IsNullOrEmpty(invoice.InvoiceNo))
                return "General";

            var invoiceNo = invoice.InvoiceNo.ToUpperInvariant();

            if (invoiceNo.Contains("AMAZON") || invoiceNo.Contains("AMZN"))
                return "Amazon";
            if (invoiceNo.Contains("TEMU"))
                return "TEMU";
            if (invoiceNo.Contains("SHEIN"))
                return "Shein";
            if (invoiceNo.Contains("ALIBABA") || invoiceNo.Contains("ALIEXPRESS"))
                return "Alibaba";

            return "General";
        }

        private static async Task<bool> ApplyCommonCorrections(ShipmentInvoice invoice, string invoiceType, ILogger log)
        {
            var corrected = false;

            try
            {
                // Calculate the differences to understand the issue
                var detailLevelDiff = invoice.InvoiceDetails
                    .Sum(detail => (detail.TotalCost ?? 0.0) - ((detail.Cost) * (detail.Quantity)));

                var calculatedSubTotal = invoice.InvoiceDetails
                    .Sum(detail => detail.TotalCost ?? ((detail.Cost) * (detail.Quantity)));

                var headerLevelDiff = (calculatedSubTotal
                                     + (invoice.TotalInternalFreight ?? 0)
                                     + (invoice.TotalOtherCost ?? 0)
                                     + (invoice.TotalInsurance ?? 0)
                                     - (invoice.TotalDeduction ?? 0))
                                    - (invoice.InvoiceTotal ?? 0);

                log.Information("Invoice {InvoiceId} analysis - DetailDiff: {DetailDiff}, HeaderDiff: {HeaderDiff}",
                    invoice.Id, detailLevelDiff, headerLevelDiff);

                // Apply corrections based on common patterns
                if (Math.Abs(headerLevelDiff) > 0.01)
                {
                    corrected = await CorrectHeaderLevelIssues(invoice, invoiceType, headerLevelDiff, log).ConfigureAwait(false);
                }

                if (Math.Abs(detailLevelDiff) > 0.01)
                {
                    corrected = await CorrectDetailLevelIssues(invoice, invoiceType, detailLevelDiff, log).ConfigureAwait(false) || corrected;
                }

                return corrected;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error applying corrections to invoice {InvoiceId}", invoice.Id);
                return false;
            }
        }

        private static async Task<bool> CorrectHeaderLevelIssues(ShipmentInvoice invoice, string invoiceType, double headerDiff, ILogger log)
        {
            var corrected = false;

            try
            {
                // If header difference is negative, we're missing charges (shipping, tax, etc.)
                if (headerDiff < -0.01)
                {
                    var missingAmount = Math.Abs(headerDiff);

                    // Apply invoice-type specific corrections
                    switch (invoiceType)
                    {
                        case "Amazon":
                            // Amazon often has shipping and tax that might be missed
                            if ((invoice.TotalOtherCost ?? 0) == 0)
                            {
                                invoice.TotalOtherCost = missingAmount;
                                log.Information("Applied Amazon shipping/tax correction: {Amount} to invoice {InvoiceId}",
                                    missingAmount, invoice.Id);
                                corrected = true;
                            }
                            break;

                        case "TEMU":
                            // TEMU often has shipping fees
                            if ((invoice.TotalOtherCost ?? 0) == 0)
                            {
                                invoice.TotalOtherCost = missingAmount;
                                log.Information("Applied TEMU shipping correction: {Amount} to invoice {InvoiceId}",
                                    missingAmount, invoice.Id);
                                corrected = true;
                            }
                            break;

                        default:
                            // General case - add to other costs
                            if ((invoice.TotalOtherCost ?? 0) == 0)
                            {
                                invoice.TotalOtherCost = missingAmount;
                                log.Information("Applied general other cost correction: {Amount} to invoice {InvoiceId}",
                                    missingAmount, invoice.Id);
                                corrected = true;
                            }
                            break;
                    }
                }
                // If header difference is positive, we're missing deductions (coupons, discounts)
                else if (headerDiff > 0.01)
                {
                    var missingDeduction = headerDiff;

                    switch (invoiceType)
                    {
                        case "TEMU":
                            // TEMU often has coupon deductions
                            if ((invoice.TotalDeduction ?? 0) == 0)
                            {
                                invoice.TotalDeduction = missingDeduction;
                                log.Information("Applied TEMU coupon deduction: {Amount} to invoice {InvoiceId}",
                                    missingDeduction, invoice.Id);
                                corrected = true;
                            }
                            break;

                        case "Amazon":
                            // Amazon might have promotional credits
                            if ((invoice.TotalDeduction ?? 0) == 0)
                            {
                                invoice.TotalDeduction = missingDeduction;
                                log.Information("Applied Amazon promotional deduction: {Amount} to invoice {InvoiceId}",
                                    missingDeduction, invoice.Id);
                                corrected = true;
                            }
                            break;

                        default:
                            // General case - add deduction
                            if ((invoice.TotalDeduction ?? 0) == 0)
                            {
                                invoice.TotalDeduction = missingDeduction;
                                log.Information("Applied general deduction: {Amount} to invoice {InvoiceId}",
                                    missingDeduction, invoice.Id);
                                corrected = true;
                            }
                            break;
                    }
                }

                return corrected;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error correcting header level issues for invoice {InvoiceId}", invoice.Id);
                return false;
            }
        }

        private static async Task<bool> CorrectDetailLevelIssues(ShipmentInvoice invoice, string invoiceType, double detailDiff, ILogger log)
        {
            var corrected = false;

            try
            {
                // Detail level issues usually involve missing or incorrect TotalCost values
                foreach (var detail in invoice.InvoiceDetails.Where(d => d.TotalCost == null || d.TotalCost == 0))
                {
                    if (detail.Cost > 0 && detail.Quantity > 0)
                    {
                        detail.TotalCost = detail.Cost * detail.Quantity;
                        log.Information("Corrected missing TotalCost for detail {DetailId}: {Cost} x {Quantity} = {TotalCost}",
                            detail.Id, detail.Cost, detail.Quantity, detail.TotalCost);
                        corrected = true;
                    }
                }

                return corrected;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error correcting detail level issues for invoice {InvoiceId}", invoice.Id);
                return false;
            }
        }

    } // End of ShipmentUtils Class
} // End of namespace AutoBot