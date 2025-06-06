using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Converters; // For FormatedSpace
using CoreEntities.Business.Entities;
using EmailDownloader; // For EmailDownloader access
using WaterNut.DataSpace; // For BaseDataModel
using System.Data.Entity; // For Include extension method
using TrackableEntities; // For TrackingState
using Core.Common.Utils; // Added for FormatedSpace

namespace AutoBot.Services
{
    /// <summary>
    /// Handles reporting specific errors like missing invoices or incomplete data via email.
    /// Extracted from AutoBot.Utils class to adhere to SRP.
    /// </summary>
    public static class ErrorReportingService
    {
        public static void SubmitMissingInvoices(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Submit Missing Invoices");

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitDocSetWithIncompleteInvoices
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                        // Call public static method in this service
                        if (GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices").Any()) continue;

                        var firstItem = emailIds.FirstOrDefault();
                        if (firstItem == null) continue;

                        var body = $"The {firstItem.Declarant_Reference_Number} is missing Invoices. {firstItem.ImportedInvoices} were Imported out of {firstItem.TotalInvoices} . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Invoice Date".FormatedSpace(20)}{"Invoice Value".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.InvoiceDate.ToShortDateString().FormatedSpace(20)}{current.InvoiceTotal.Value.ToString("N2").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();

                        // Dependency on Utils.Client - Needs refactoring later
                        if (emailIds.Key.EmailId == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(AutoBot.Utils.Client, "", "Error:Missing Invoices",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key.EmailId, AutoBot.Utils.Client, "Error:Missing Invoices", body, contacts, attlst.ToArray());
                        }

                        // Call public static method
                        LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoices");
                    }
                     // Save changes once after processing all groups for this context
                     ctx.SaveChanges();
                }
                // SubmitMissingInvoicePDFs(ft); // This creates a tight coupling, consider removing or handling differently
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in SubmitMissingInvoices: {e.Message}");
                throw;
            }
        }

        public static void SubmitMissingInvoicePDFs(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Submit Missing Invoice PDFs");

                using (var ctx = new CoreEntitiesContext())
                {
                    ctx.Database.CommandTimeout = 10;
                    var contacts = ctx.Contacts.Where(x => x.Role == "PO Clerk" || x.Role == "Broker")
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        .Select(x => x.EmailAddress)
                        .Distinct()
                        .ToArray();
                    var lst = ctx.TODO_SubmitMissingInvoicePDFs
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => new { x.EmailId, x.AsycudaDocumentSetId });

                    foreach (var emailIds in lst)
                    {
                         // Call public static method in this service
                        if (GetDocSetActions(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs").Any()) continue;

                        var firstItem = emailIds.FirstOrDefault();
                         if (firstItem == null) continue;

                        var body = $"The {firstItem.Declarant_Reference_Number} is missing Invoice PDF Attachments. \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Source File".FormatedSpace(20)}\r\n" +
                                   $"{emailIds.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SourceFile.FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please email CSV with Coresponding PDF to prevent this error.\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();

                        // Dependency on Utils.Client - Needs refactoring later
                        if (emailIds.Key.EmailId == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(AutoBot.Utils.Client, "", "Error:Missing Invoices PDF Attachments",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailIds.Key.EmailId, AutoBot.Utils.Client, "Error:Missing Invoices PDF Attachments", body, contacts, attlst.ToArray());
                        }

                         // Call public static method
                         LogDocSetAction(emailIds.Key.AsycudaDocumentSetId, "SubmitMissingInvoicePDFs");
                    }
                     // Save changes once after processing all groups for this context
                     ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                 Console.WriteLine($"Error in SubmitMissingInvoicePDFs: {e.Message}");
                throw;
            }
        }

        public static void SubmitIncompleteEntryData(FileTypes ft)
        {
            try
            {
                Console.WriteLine("Submit Incomplete Entry Data");

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
                        .Where(x => x.AsycudaDocumentSetId == ft.AsycudaDocumentSetId)
                        .ToList()
                        .GroupBy(x => x.EmailId);

                    foreach (var emailGroup in lst)
                    {
                        var body = "The Following Invoices Total do not match Imported Totals . \r\n" +
                                   $"\t{"Invoice No.".FormatedSpace(20)}{"Supplier Code".FormatedSpace(20)}{"Invoice Total".FormatedSpace(20)}{"Imported Total".FormatedSpace(20)}\r\n" +
                                   $"{emailGroup.Select(current => $"\t{current.InvoiceNo.FormatedSpace(20)}{current.SupplierCode.FormatedSpace(20)}{current.InvoiceTotal.Value.ToString("C").FormatedSpace(20)}{current.ImportedTotal.Value.ToString("C").FormatedSpace(20)} \r\n").Aggregate((old, current) => old + current)}" +
                                   $"Please Check CSVs or Document Set Total Invoices\r\n" +
                                   $"Any questions or concerns please contact Joseph Bartholomew at Joseph@auto-brokerage.com.\r\n" +
                                   $"\r\n" +
                                   $"Regards,\r\n" +
                                   $"AutoBot";
                        List<string> attlst = new List<string>();

                        // Dependency on Utils.Client - Needs refactoring later
                        if (emailGroup.Key == null)
                        {
                            EmailDownloader.EmailDownloader.SendEmail(AutoBot.Utils.Client, "", "Error:Incomplete Invoice Data",
                                contacts, body, attlst.ToArray());
                        }
                        else
                        {
                            EmailDownloader.EmailDownloader.ForwardMsg(emailGroup.Key, AutoBot.Utils.Client, "Error:Incomplete Invoice Data", body, contacts, attlst.ToArray());
                        }
                        // No SaveChanges or LogDocSetAction was present in the original code for this loop
                    }
                }
            }
            catch (Exception e)
            {
                 Console.WriteLine($"Error in SubmitIncompleteEntryData: {e.Message}");
                throw;
            }
        }

        // Made public for external use
        public static List<ActionDocSetLogs> GetDocSetActions(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext())
            {
                return ctx.ActionDocSetLogs.Include(l => l.Actions)
                          .Where(x => x.Actions.Name == actionName && x.AsycudaDocumentSetId == docSetId)
                          .ToList();
            }
        }

         // Made public, renamed, and handles its own context
        public static void LogDocSetAction(int docSetId, string actionName)
        {
            using (var ctx = new CoreEntitiesContext()) // Create context internally
            {
                var action = ctx.Actions.FirstOrDefault(x => x.Name == actionName);
                if (action == null) throw new ApplicationException($"Action with name: {actionName} not found");
                ctx.ActionDocSetLogs.Add(new ActionDocSetLogs(true)
                {
                    ActonId = action.Id,
                    AsycudaDocumentSetId = docSetId,
                    TrackingState = TrackingState.Added
                });
                 ctx.SaveChanges(); // Save changes within this method now
            }
        }
    }
}