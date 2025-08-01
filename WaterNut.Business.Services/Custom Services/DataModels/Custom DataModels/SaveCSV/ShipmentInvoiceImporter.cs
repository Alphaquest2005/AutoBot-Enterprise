﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Added for ILogger
using Core.Common.Utils;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using InventoryDS.Business.Entities;
using InventoryQS.Business.Services;
using TrackableEntities;
using WaterNut.Business.Services.Utils;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using static java.util.Locale;

namespace WaterNut.DataSpace
{
    using System.Data.Entity.Infrastructure.Design;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;

    using MoreLinq;

    public class ShipmentInvoiceImporter
    {
        static ShipmentInvoiceImporter()
        {
        }

        public ShipmentInvoiceImporter()
        {
        }

        public async Task<bool> ProcessShipmentInvoice(FileTypes fileType, List<AsycudaDocumentSet> docSet, bool overWriteExisting, string emailId, string droppedFilePath, List<object> eslst, Dictionary<string, string> invoicePOs, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ProcessShipmentInvoice), "Process shipment invoice data from email or file", $"FileType: {fileType?.Description}, EmailId: {emailId}, FilePath: {droppedFilePath}, DocSetCount: {docSet?.Count ?? 0}");

            using (var context = new EntryDataDSContext())
            {
                try
                {
                    logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                        nameof(ProcessShipmentInvoice), $"Processing shipment invoice from File: {droppedFilePath ?? emailId}");

                    var invoiceData = eslst.Cast<List<IDictionary<string, object>>>().SelectMany(x => x.ToList()).ToList();
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(ProcessShipmentInvoice), "DataExtraction", "Extracted invoice data from input list.", $"RawItemCount: {invoiceData.Count}");

                    logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ExtractShipmentInvoices", "ASYNC_EXPECTED");
                    var extractStopwatch = Stopwatch.StartNew();
                    var shipmentInvoices = await ExtractShipmentInvoices(fileType, emailId, droppedFilePath, invoiceData, logger).ConfigureAwait(false); // Pass logger




                    extractStopwatch.Stop();
                    logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ExtractShipmentInvoices", extractStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                    var invoices = ReduceLstByInvoiceNo(shipmentInvoices);
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(ProcessShipmentInvoice), "DataReduction", "Reduced list by invoice number.", $"UniqueInvoiceCount: {invoices.Count}");

                    var goodInvoices = invoices.Where(x => x.InvoiceDetails.All(z => !string.IsNullOrEmpty(z.ItemDescription)))
                                                .Where(x => x.InvoiceDetails.Any())
                                                .ToList();
                    logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(ProcessShipmentInvoice), "DataFiltering", "Filtered for good invoices.", $"GoodInvoiceCount: {goodInvoices.Count}");



                    logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "SaveInvoicePOsAsync", "ASYNC_EXPECTED");
                    var saveStopwatch = Stopwatch.StartNew();
                    await SaveInvoicePOsAsync(context, invoicePOs, goodInvoices).ConfigureAwait(false);
                    saveStopwatch.Stop();
                    logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "SaveInvoicePOsAsync", saveStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                    methodStopwatch.Stop(); // Stop stopwatch on success
                    logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(ProcessShipmentInvoice), "Shipment invoice processed successfully", $"ProcessedGoodInvoiceCount: {goodInvoices.Count}", methodStopwatch.ElapsedMilliseconds);
                    logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ProcessShipmentInvoice), $"Successfully processed {goodInvoices.Count} good invoices", methodStopwatch.ElapsedMilliseconds);

                    return true;
                }
                catch (Exception e)
                {
                    methodStopwatch.Stop(); // Stop stopwatch on failure
                    logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessShipmentInvoice), "Process shipment invoice data from email or file", methodStopwatch.ElapsedMilliseconds, $"Error processing shipment invoice: {e.Message}");
                    logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessShipmentInvoice), "Overall shipment invoice processing", methodStopwatch.ElapsedMilliseconds, $"Error processing shipment invoice: {e.Message}");
                    Console.WriteLine(e); // Keep Console.WriteLine for now
                    return false;
                }
            }
        }



        private static async Task<List<ShipmentInvoice>> ExtractShipmentInvoices(FileTypes fileType, string emailId, string droppedFilePath, List<IDictionary<string, object>> itms, ILogger logger) // Added ILogger parameter
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
            logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(ExtractShipmentInvoices), "Extract shipment invoices from raw data", $"FileType: {fileType?.Description}, EmailId: {emailId}, FilePath: {droppedFilePath}, RawItemCount: {itms?.Count ?? 0}");

            try
            {
                logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ExtractShipmentInvoices), $"Extracting shipment invoices from {itms?.Count ?? 0} raw items");

                var tasks = itms.Select(x => (IDictionary<string, object>)x)
                    .Where(x => x != null && x.Any())
                    .Select(x => ProcessInvoiceItem(x, logger)) // Pass logger to ProcessInvoiceItem
                    .Where(x => x != null)
                    .ToList();

                if (!tasks.Any())
                {
                    logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                       nameof(ExtractShipmentInvoices), "TaskCreation", "InvoiceItemTasks", "No valid invoice items found to process.");
                }

                logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "Task.WhenAll(ProcessInvoiceItem tasks)", "ASYNC_EXPECTED");
                var whenAllStopwatch = Stopwatch.StartNew();
                var results = await Task.WhenAll(tasks).ConfigureAwait(false);
                whenAllStopwatch.Stop();
                logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "Task.WhenAll(ProcessInvoiceItem tasks)", whenAllStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                var validResults = results.Where(x => x != null).ToList();
                logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(ExtractShipmentInvoices), "TaskCompletion", "Completed processing invoice item tasks.", $"ValidInvoiceCount: {validResults.Count}");

                methodStopwatch.Stop(); // Stop stopwatch on success
                logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(ExtractShipmentInvoices), "Shipment invoices extracted successfully", $"ExtractedInvoiceCount: {validResults.Count}", methodStopwatch.ElapsedMilliseconds);
                logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(ExtractShipmentInvoices), $"Successfully extracted {validResults.Count} shipment invoices", methodStopwatch.ElapsedMilliseconds);

                return validResults;
            }
            catch (Exception e)
            {
                methodStopwatch.Stop(); // Stop stopwatch on failure
                logger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ExtractShipmentInvoices), "Extract shipment invoices from raw data", methodStopwatch.ElapsedMilliseconds, $"Error extracting shipment invoices: {e.Message}");
                logger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ExtractShipmentInvoices), "Shipment invoice extraction process", methodStopwatch.ElapsedMilliseconds, $"Error extracting shipment invoices: {e.Message}");
                Console.WriteLine(e); // Keep Console.WriteLine for now
                throw;
            }

            async Task<ShipmentInvoice> ProcessInvoiceItem(IDictionary<string, object> x, ILogger itemLogger) // Added ILogger parameter
            {
                var itemMethodStopwatch = Stopwatch.StartNew(); // Start stopwatch for item method
                itemLogger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                    nameof(ProcessInvoiceItem), "Process a single invoice item dictionary", $"Keys: {string.Join(",", x?.Keys ?? Enumerable.Empty<string>())}");

                try
                {
                    itemLogger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                        nameof(ProcessInvoiceItem), $"Processing invoice item with keys: {string.Join(",", x?.Keys ?? Enumerable.Empty<string>())}");

                    var invoice = new ShipmentInvoice();

                    if (x["ShipmentInvoiceDetails"] == null)
                    {
                        itemLogger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(ProcessInvoiceItem), "Validation", "InvoiceDetails is null, skipping item.", "");
                        itemMethodStopwatch.Stop(); // Stop stopwatch
                        itemLogger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                            nameof(ProcessInvoiceItem), "Skipped item due to null InvoiceDetails", "Returned null", itemMethodStopwatch.ElapsedMilliseconds);
                        itemLogger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                            nameof(ProcessInvoiceItem), "Skipped item due to null InvoiceDetails", "Returned null", itemMethodStopwatch.ElapsedMilliseconds);
                        return null; // throw new ApplicationException("Template Details is null");
                    }

                    var items = ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                        .Where(z => z != null)
                        .Where(z => z.ContainsKey("ItemDescription"))
                        .Select(z => (
                            ItemNumber: z["ItemNumber"]?.ToString(),
                            ItemDescription: z["ItemDescription"].ToString(),
                            TariffCode: x.ContainsKey("TariffCode") ? x["TariffCode"]?.ToString() : ""
                        )).ToList();

                    if (!items.Any())
                    {
                        itemLogger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): Processing collection '{CollectionName}'. Item count: 0. {EmptyCollectionExpectation}",
                           nameof(ProcessInvoiceItem), "ItemExtraction", "ExtractedItems", "No valid items found within InvoiceDetails.");
                    }

                    itemLogger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "InventoryItemsExService.ClassifiedItms", "ASYNC_EXPECTED");
                    var classifiedStopwatch = Stopwatch.StartNew();
                    var classifiedItms = await InventoryItemsExService.ClassifiedItms(items, itemLogger).ConfigureAwait(false); // Pass logger
                    classifiedStopwatch.Stop();
                    itemLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "InventoryItemsExService.ClassifiedItms", classifiedStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

                    invoice.ApplicationSettingsId = BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId;
                    invoice.InvoiceNo = x.ContainsKey("InvoiceNo") && x["InvoiceNo"] != null ? x["InvoiceNo"].ToString().Truncate(50) : "Unknown";
                    invoice.PONumber = x.ContainsKey("PONumber") && x["PONumber"] != null ? x["PONumber"].ToString() : null;
                    invoice.InvoiceDate = x.ContainsKey("InvoiceDate") ? DateTime.Parse(x["InvoiceDate"].ToString()) : DateTime.MinValue;
                    invoice.InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null;
                    invoice.SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null;
                    invoice.ImportedLines = !x.ContainsKey("InvoiceDetails") ? 0 : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).Count;
                    invoice.SupplierCode = x.ContainsKey("SupplierCode") ? x["SupplierCode"]?.ToString() : null;
                    invoice.SupplierName = (x.ContainsKey("SupplierName") ? x["SupplierName"]?.ToString() : null) ?? (x.ContainsKey("SupplierCode") ? x["SupplierCode"]?.ToString() : null);
                    invoice.SupplierAddress = x.ContainsKey("SupplierAddress") ? x["SupplierAddress"]?.ToString() : null;
                    invoice.SupplierCountry = x.ContainsKey("SupplierCountryCode") ? x["SupplierCountryCode"]?.ToString() : null;
                    invoice.FileLineNumber = itms.IndexOf(x) + 1;
                    invoice.Currency = x.ContainsKey("Currency") ? x["Currency"].ToString() : null;
                    invoice.TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null;
                    invoice.TotalOtherCost = x.ContainsKey("TotalOtherCost") ? Convert.ToDouble(x["TotalOtherCost"].ToString()) : (double?)null;
                    invoice.TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null;
                    invoice.TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null;

                    itemLogger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ProcessInvoiceDetails", "SYNC_EXPECTED");
                    var processDetailsStopwatch = Stopwatch.StartNew();
                    invoice.InvoiceDetails = ProcessInvoiceDetails(x, classifiedItms);
                    processDetailsStopwatch.Stop();
                    itemLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ProcessInvoiceDetails", processDetailsStopwatch.ElapsedMilliseconds, "Sync call returned");

                    itemLogger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "ProcessExtraInfo", "SYNC_EXPECTED");
                    var processExtraInfoStopwatch = Stopwatch.StartNew();
                    invoice.InvoiceExtraInfo = ProcessExtraInfo(x);
                    processExtraInfoStopwatch.Stop();
                    itemLogger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        "ProcessExtraInfo", processExtraInfoStopwatch.ElapsedMilliseconds, "Sync call returned");

                    invoice.EmailId = emailId;
                    invoice.SourceFile = droppedFilePath;
                    invoice.FileTypeId = fileType.Id;
                    invoice.TrackingState = TrackingState.Added;

                    itemMethodStopwatch.Stop(); // Stop stopwatch on success
                    itemLogger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(ProcessInvoiceItem), "Invoice item processed successfully", $"InvoiceNo: {invoice.InvoiceNo}, ItemCount: {invoice.InvoiceDetails.Count}", itemMethodStopwatch.ElapsedMilliseconds);
                    itemLogger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ProcessInvoiceItem), $"Successfully processed invoice item for InvoiceNo: {invoice.InvoiceNo}", itemMethodStopwatch.ElapsedMilliseconds);

                    return invoice;
                }
                catch (Exception e)
                {
                    itemMethodStopwatch.Stop(); // Stop stopwatch on failure
                    itemLogger.Error(e, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessInvoiceItem), "Process a single invoice item dictionary", itemMethodStopwatch.ElapsedMilliseconds, $"Error processing invoice item: {e.Message}");
                    itemLogger.Error(e, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessInvoiceItem), "Invoice item processing", itemMethodStopwatch.ElapsedMilliseconds, $"Error processing invoice item: {e.Message}");
                    Console.WriteLine(e); // Keep Console.WriteLine for now
                    throw;
                }
            }

            List<InvoiceDetails> ProcessInvoiceDetails(IDictionary<string, object> x, Dictionary<string, (string ItemNumber, string ItemDescription, string TariffCode, string Category, string CategoryTariffCode)> classifiedItms)
            {
                if (!x.ContainsKey("InvoiceDetails"))
                    return new List<InvoiceDetails>();

                return ((List<IDictionary<string, object>>)x["InvoiceDetails"])
                    .Where(z => z != null)
                    .Where(z => z.ContainsKey("ItemDescription") && z["ItemDescription"] != null)
                    .Select(z =>
                    {
                        var details = new InvoiceDetails();
                        var qty = z.ContainsKey("Quantity") ? Convert.ToDouble(z["Quantity"].ToString()) : 1;
                        details.Quantity = qty;

                        var classifiedItm = classifiedItms.ContainsKey(z["ItemDescription"].ToString())
                            ? classifiedItms[z["ItemDescription"].ToString()]
                            : (
                                ItemNumber: z.ContainsKey("ItemNumber") ? z["ItemNumber"].ToString().ToUpper().Truncate(20) : null,
                                ItemDescription: z["ItemDescription"].ToString().Truncate(255),
                                TariffCode: z.ContainsKey("TariffCode") ? z["TariffCode"].ToString().ToUpper().Truncate(20) : null,
                                Category: string.Empty,
                                CategoryTariffCode: string.Empty
                            );

                        var dbCategoryTariffs = BaseDataModel.Instance.CategoryTariffs.FirstOrDefault(x => x.Category == classifiedItm.Category);

                        details.ItemNumber = (classifiedItm.ItemNumber ?? classifiedItm.ItemDescription).ToUpper().Truncate(20);
                        details.ItemDescription = classifiedItm.ItemDescription.Truncate(255);
                        details.TariffCode = classifiedItm.TariffCode;
                        details.Category = dbCategoryTariffs?.Category ?? classifiedItm.Category;
                        details.CategoryTariffCode = dbCategoryTariffs?.TariffCode ?? classifiedItm.CategoryTariffCode;
                        details.Units = z.ContainsKey("Units") ? z["Units"].ToString() : null;
                        details.Cost = z.ContainsKey("Cost")
                            ? Convert.ToDouble(z["Cost"].ToString())
                            : Convert.ToDouble(z["TotalCost"].ToString()) / (qty == 0 ? 1 : qty);
                        details.TotalCost = z.ContainsKey("TotalCost")
                            ? Convert.ToDouble(z["TotalCost"].ToString())
                            : Convert.ToDouble(z["Cost"].ToString()) * qty;
                        details.Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0;
                        details.Volume = z.ContainsKey("Gallons")
                            ? new InvoiceDetailsVolume()
                            {
                                Quantity = Convert.ToDouble(z["Gallons"].ToString()),
                                Units = "Gallons",
                                TrackingState = TrackingState.Added
                            }
                            : null;
                        details.SalesFactor = (z.ContainsKey("SalesFactor") && z.ContainsKey("Units") && z["Units"].ToString() != "EA")
                            || (z.ContainsKey("SalesFactor") && !z.ContainsKey("Units"))
                            ? Convert.ToInt32(z["SalesFactor"].ToString())
                            : 1;
                        details.LineNumber = z.ContainsKey("Instance")
                            ? Convert.ToInt32(
                                z["Instance"].ToString().Contains("-")
                                ? z["Instance"].ToString().Split('-')[1]
                                : z["Instance"].ToString())
                            : ((List<IDictionary<string, object>>)x["InvoiceDetails"]).IndexOf(z) + 1;
                        details.FileLineNumber = z.ContainsKey("FileLineNumber") ? Convert.ToInt32(z["FileLineNumber"].ToString()) : -1;
                        details.Section = z.ContainsKey("Section") ? z["Section"].ToString() : null;
                        details.InventoryItemId = z.ContainsKey("InventoryItemId") ? (int)z["InventoryItemId"] : (int?)null;
                        details.TrackingState = TrackingState.Added;
                        return details;
                    }).ToList();
            }

            List<InvoiceExtraInfo> ProcessExtraInfo(IDictionary<string, object> x)
            {
                if (!x.ContainsKey("ExtraInfo"))
                    return new List<InvoiceExtraInfo>();

                return ((List<IDictionary<string, object>>)x["ExtraInfo"])
                    .Where(z => z.Keys.Any())
                    .SelectMany(z => z)
                    .Where(z => z.Value != null)
                    .Select(z => new InvoiceExtraInfo
                    {
                        Info = z.Key.ToString(),
                        Value = z.Value.ToString(),
                        TrackingState = TrackingState.Added
                    }).ToList();
            }
        }




        private static async Task SaveInvoicePOsAsync(EntryDataDSContext ctx, Dictionary<string, string> invoicePOs, List<ShipmentInvoice> lst)
        {
            // 1. Collect all details and ensure categories exist
            var allDetails = lst.SelectMany(inv => inv.InvoiceDetails).ToList();

            if (allDetails.Any())
            {
                await EnsureCategoriesExistAsync(ctx, allDetails).ConfigureAwait(false);
            }

            // 2. Process invoices - much simpler now!
            foreach (var invoice in lst)
            {
                var existingManifest = ctx.ShipmentInvoice.FirstOrDefault(x => x.InvoiceNo == invoice.InvoiceNo);
                if (existingManifest != null)
                {
                    ctx.ShipmentInvoice.Remove(existingManifest);
                }

                invoice.InvoiceDetails = AutoFixImportErrors(invoice);
                invoice.ImportedLines = invoice.InvoiceDetails.Count;

                if (Math.Abs(invoice.SubTotal.GetValueOrDefault()) < 0.01)
                {
                    invoice.SubTotal = invoice.ImportedSubTotal;
                }

                if (!invoice.InvoiceDetails.Any())
                    continue;

                if (invoicePOs != null && lst.Count > 1 && invoice != lst.First())
                {
                    invoice.SourceFile = invoice.SourceFile.Replace($"{invoicePOs[lst.First().InvoiceNo]}",
                        invoicePOs[invoice.InvoiceNo]);
                }

                // 3. Simply normalize the category data - no foreign key complexity!
                foreach (var detail in invoice.InvoiceDetails)
                {
                    if (!string.IsNullOrEmpty(detail.Category))
                    {
                        detail.Category = detail.Category.ToUpperInvariant().Trim();
                    }
                    if (!string.IsNullOrEmpty(detail.CategoryTariffCode))
                    {
                        detail.CategoryTariffCode = detail.CategoryTariffCode.ToUpperInvariant().Trim();
                    }
                }

                ctx.ShipmentInvoice.Add(invoice);
            }

            // 4. Save everything - fast and simple!
            await ctx.SaveChangesAsync().ConfigureAwait(false);
        }

        private static async Task EnsureCategoriesExistAsync(EntryDataDSContext ctx, ICollection<InvoiceDetails> details)
        {
            if (details == null || !details.Any()) return;

            // Extract unique (Category, TariffCode) combinations
            var categoryTariffCombinations = details
                .Where(d => !string.IsNullOrEmpty(d.Category) && !string.IsNullOrEmpty(d.CategoryTariffCode))
                .Select(d => new {
                    Category = d.Category.ToUpperInvariant().Trim(),
                    TariffCode = d.CategoryTariffCode.ToUpperInvariant().Trim()
                })
                .GroupBy(x => new { x.Category, x.TariffCode })
                .Select(g => g.Key)
                .ToList();

            if (!categoryTariffCombinations.Any()) return;

            // Use MERGE statement to handle upserts at the database level
            foreach (var combo in categoryTariffCombinations)
            {
                try
                {
                    var sql = @"
                MERGE CategoryTariffs AS target
                USING (SELECT @Category AS Category, @TariffCode AS TariffCode) AS source
                ON target.Category = source.Category AND target.TariffCode = source.TariffCode
                WHEN NOT MATCHED THEN
                    INSERT (Category, TariffCode)
                    VALUES (source.Category, source.TariffCode);";

                    await ctx.Database.ExecuteSqlCommandAsync(sql,
                        new System.Data.SqlClient.SqlParameter("@Category", combo.Category),
                        new System.Data.SqlClient.SqlParameter("@TariffCode", combo.TariffCode))
                        .ConfigureAwait(false);
                }
                catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                {
                    // Duplicate key - record already exists, continue
                    Console.WriteLine($"CategoryTariff already exists: ({combo.Category}, {combo.TariffCode})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error ensuring CategoryTariff exists for ({combo.Category}, {combo.TariffCode}): {ex.Message}");
                    // Continue processing other combinations
                }
            }
        }


        private List<ShipmentInvoice> ReduceLstByInvoiceNo(List<ShipmentInvoice> lstData)
        {
            var res = new List<ShipmentInvoice>();

            foreach (var grp in lstData.GroupBy(x => x.InvoiceNo))
            {
                var rinvoice = grp.FirstOrDefault(x => grp.Count() <= 1 || (string.IsNullOrEmpty(x.PONumber) && x.InvoiceNo == x.PONumber) || x.InvoiceNo != x.PONumber);
                if (rinvoice == null) continue;
                foreach (var invoice in grp.Where(x => grp.Count() > 1 && x.InvoiceNo != x.PONumber && x.InvoiceNo != rinvoice.InvoiceNo))
                {
                    rinvoice.InvoiceDetails.AddRange(invoice.InvoiceDetails);
                    rinvoice.PONumber = invoice.PONumber; // don't think this makes a difference
                }
                res.Add(rinvoice);
            }

            return res;
        }

        private static List<InvoiceDetails> AutoFixImportErrors(ShipmentInvoice invoice)
        {
            var details = invoice.InvoiceDetails;

            foreach (var err in details.Where(x => x.TotalCost.GetValueOrDefault() > 0 && Math.Abs(((x.Quantity * x.Cost) - x.Discount.GetValueOrDefault()) - x.TotalCost.GetValueOrDefault()) > 0.01))
            {

            }

            //if (invoice.SubTotal > 0
            //    && Math.Abs((double) (invoice.SubTotal - details.Sum(x => x.Cost * x.Quantity))) > 0.01)
            var secList = details.GroupBy(x => x.Section).ToList();
            var lst = new List<InvoiceDetails>();
            foreach (var section in secList)
            {
                foreach (var detail in section)
                {
                    if (lst.Any(x =>
                            x.TotalCost == detail.TotalCost && x.ItemNumber == detail.ItemNumber &&
                            x.Quantity == detail.Quantity && x.Section != detail.Section)) continue;
                    lst.Add(detail);
                }
            }

            //details = details.DistinctBy(x => new { ItemNumber = x.ItemNumber.ToUpper(), x.Quantity, x.TotalCost})
            //    .ToList();


            //  var invoiceInvoiceDetails = lst ;
            return lst;
        }


    }
}