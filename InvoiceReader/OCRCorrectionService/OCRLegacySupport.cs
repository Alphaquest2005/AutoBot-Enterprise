// File: OCRCorrectionService/OCRLegacySupport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using Serilog;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Legacy Support)

        // Definition for _totalsZeroAmounts
        // This table allows associating a calculated imbalance amount with a ShipmentInvoice instance
        // without modifying the ShipmentInvoice class itself. It's primarily for the
        // .TotalsZeroAmount() extension method for debugging or specific scenarios.
        private static readonly ConditionalWeakTable<ShipmentInvoice, StrongBox<double>> _totalsZeroAmounts =
            new ConditionalWeakTable<ShipmentInvoice, StrongBox<double>>();

        public struct InvoiceBalanceStatus
        {
            public string InvoiceIdentifier { get; set; }
            public bool IsBalanced { get; set; }
            public double ImbalanceAmount { get; set; }
            public string ErrorMessage { get; set; }
        }

        public static bool TotalsZero(ShipmentInvoice invoice, out double differenceAmount, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            differenceAmount = double.MaxValue;

            if (invoice == null)
            {
                log.Warning("TotalsZero (ShipmentInvoice): Null invoice provided.");
                return false;
            }

            var baseTotal = (invoice.SubTotal ?? 0) +
                          (invoice.TotalInternalFreight ?? 0) +
                          (invoice.TotalOtherCost ?? 0) +
                          (invoice.TotalInsurance ?? 0);
            var deductionAmount = invoice.TotalDeduction ?? 0;
            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

            var calculatedFinalTotal = baseTotal - deductionAmount;
            differenceAmount = Math.Abs(calculatedFinalTotal - reportedInvoiceTotal);

            bool isZero = differenceAmount < 0.01;

            _totalsZeroAmounts.Remove(invoice);
            _totalsZeroAmounts.Add(invoice, new StrongBox<double>(differenceAmount));

            log.Debug("TotalsZero Check for ShipmentInvoice {InvoiceNo}: BaseCalc={BaseTotal:F2}, Deduction={Deduction:F2}, ExpectedFinal={ExpectedFinal:F2}, ReportedFinal={ReportedTotal:F2}, Diff={Difference:F4}, IsZero={IsZeroResult}",
                invoice.InvoiceNo, baseTotal, deductionAmount, calculatedFinalTotal, reportedInvoiceTotal, differenceAmount, isZero);

            return isZero;
        }

        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger)
        {
            return TotalsZero(invoice, out _, logger);
        }

        public static bool TotalsZero(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            List<InvoiceBalanceStatus> bals = TotalsZeroInternal(dynamicInvoiceResults,out totalImbalanceSum, logger);
            return bals.Any() && bals.All(s => s.IsBalanced);
        }

        /// <summary>
        /// Checks the balance for all invoices found within a List<dynamic> structure.
        /// Outputs the SUM of all individual imbalance amounts.
        /// </summary>
        /// <param name="dynamicInvoiceResults">The List<dynamic> containing invoice data.</param>
        /// <param name="totalImbalanceSum">Outputs the sum of absolute imbalance amounts for all processed invoices.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <returns>A list of InvoiceBalanceStatus objects, one for each processed invoice dictionary.</returns>
        private static List<InvoiceBalanceStatus> TotalsZeroInternal(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            var allStatuses = new List<InvoiceBalanceStatus>();
            totalImbalanceSum = 0.0; // Initialize sum

            if (dynamicInvoiceResults == null || !dynamicInvoiceResults.Any())
            {
                log.Information("TotalsZero (List<dynamic>): Input list is null or empty.");
                return allStatuses;
            }

            int dynamicItemIndex = 0;

            foreach (var dynamicItem in dynamicInvoiceResults)
            {
                List<IDictionary<string, object>> currentBatchOfDicts = new List<IDictionary<string, object>>();
                if (dynamicItem is List<IDictionary<string, object>> invoiceList)
                {
                    currentBatchOfDicts.AddRange(invoiceList);
                }
                else if (dynamicItem is IDictionary<string, object> singleInvoiceDict)
                {
                    currentBatchOfDicts.Add(singleInvoiceDict);
                }
                else
                {
                    log.Warning("TotalsZero (List<dynamic>): Encountered an item of unexpected type '{ItemType}' at main index {Index}.",
                        dynamicItem?.GetType().Name ?? "null", dynamicItemIndex);
                }

                int subIndex = 0;
                foreach (var invoiceDict in currentBatchOfDicts)
                {
                    string defaultIdentifier = (currentBatchOfDicts.Count > 1 || dynamicItem is List<IDictionary<string, object>>)
                        ? $"ListSection_{dynamicItemIndex}_Item_{subIndex++}"
                        : $"DirectItem_{dynamicItemIndex}";

                    var status = ProcessSingleDynamicInvoiceForListInternal(invoiceDict, defaultIdentifier, log);
                    allStatuses.Add(status);

                    // Add to the total sum of imbalances
                    if (status.ErrorMessage == null) // Only add if successfully processed
                    {
                        totalImbalanceSum += status.ImbalanceAmount;
                    }
                }
                dynamicItemIndex++;
            }

            if (!allStatuses.Any()) // No invoices were processed from the list
            {
                totalImbalanceSum = double.MaxValue; // Indicate no processable data
            }

            log.Debug("TotalsZero (List<dynamic>): Processed {Count} invoices. Total Imbalance Sum: {TotalImbalanceSum:F4}", allStatuses.Count, totalImbalanceSum);
            return allStatuses;
        }

        /// <summary>
        /// Internal helper to process a single IDictionary<string, object> for TotalsZero and return its balance status.
        /// </summary>
        private static InvoiceBalanceStatus ProcessSingleDynamicInvoiceForListInternal(IDictionary<string, object> invoiceDict, string defaultIdentifier, ILogger log)
        {
            if (invoiceDict == null)
            {
                return new InvoiceBalanceStatus { InvoiceIdentifier = $"{defaultIdentifier}_NullData", IsBalanced = false, ImbalanceAmount = double.MaxValue, ErrorMessage = "Invoice data dictionary was null." };
            }

            string invoiceNoForLog = invoiceDict.TryGetValue("InvoiceNo", out var invNoObj) && invNoObj != null ? invNoObj.ToString() : defaultIdentifier;
            ShipmentInvoice tempInvoice = CreateTempShipmentInvoice(invoiceDict, log);

            if (tempInvoice == null || tempInvoice.InvoiceNo == "ERROR_CREATION")
            {
                log.Warning("ProcessSingleDynamicInvoiceForListInternal: Failed to create a temporary ShipmentInvoice for identifier '{InvoiceIdentifier}'.", invoiceNoForLog);
                return new InvoiceBalanceStatus { InvoiceIdentifier = invoiceNoForLog, IsBalanced = false, ImbalanceAmount = double.MaxValue, ErrorMessage = "Failed to convert dynamic data to ShipmentInvoice." };
            }

            bool isBalanced = TotalsZero(tempInvoice, out double imbalance, log);

            return new InvoiceBalanceStatus
            {
                InvoiceIdentifier = tempInvoice.InvoiceNo,
                IsBalanced = isBalanced,
                ImbalanceAmount = imbalance
            };
        }

        /// <summary>
        /// Determines if correction processing should continue based on the balance status of invoices in 'res'.
        /// Continues if *any* invoice in the list is found to be unbalanced.
        /// Outputs the total sum of imbalances for the processed invoices.
        /// </summary>
        public static bool ShouldContinueCorrections(List<dynamic> res, out double totalImbalanceSum, ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));

            // Use the TotalsZero overload that outputs the sum
            List<InvoiceBalanceStatus> balanceStatuses = TotalsZeroInternal(res, out totalImbalanceSum, log);

            if (!balanceStatuses.Any())
            {
                log.Information("ShouldContinueCorrections: No processable invoice data found in 'res'. Assuming corrections should not continue.");
                // totalImbalanceSum would be MaxValue from TotalsZero if list is empty or no processable items
                return false;
            }

            bool anyUnbalanced = balanceStatuses.Any(s => !s.IsBalanced);

            if (anyUnbalanced)
            {
                log.Information("ShouldContinueCorrections: At least one invoice is NOT balanced (Total Imbalance Sum for all processed: {TotalImbalance:F4}). Corrections should continue.",
                    totalImbalanceSum);
                return true;
            }
            else
            {
                log.Information("ShouldContinueCorrections: All processed invoices are balanced. (Total Imbalance Sum for all processed: {TotalImbalance:F4}). Balance corrections might not be primary focus.",
                    totalImbalanceSum);
                return false;
            }
        }


        public static async Task<bool> CorrectInvoices(ShipmentInvoice invoice, string fileText, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            if (TotalsZero(invoice, out double diff, log))
            {
                log.Information("CorrectInvoices (single ShipmentInvoice): Invoice {InvNo} is already balanced. Diff: {Difference:F4}. Skipping correction.", invoice.InvoiceNo, diff);
                return true;
            }

            log.Information("CorrectInvoices (single ShipmentInvoice): Invoice {InvNo} is not balanced. Diff: {Difference:F4}. Proceeding with correction.", invoice.InvoiceNo, diff);
            using var service = new OCRCorrectionService(log);
            return await service.CorrectInvoiceAsync(invoice, fileText).ConfigureAwait(false);
        }

        public static async Task CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            if (res == null || !res.Any() || template == null)
            {
                log.Warning("CorrectInvoices (static batch): Input 'res' or 'template' is null/empty. Skipping.");
                return;
            }

            try
            {
                var templateLogId = template.OcrInvoices?.Id.ToString() ?? template.OcrInvoices.Id.ToString();
                log.Information("Starting static batch OCR correction for {DynamicResultCount} dynamic results against template {TemplateLogId} ({TemplateName})",
                    res.Count, templateLogId, template.OcrInvoices?.Name);

                string originalText = GetOriginalTextFromFile(template.FilePath, log);
                if (string.IsNullOrEmpty(originalText) && res.Any(item => (item is List<IDictionary<string, object>> l && l.Any()) || (item is IDictionary<string, object>)))
                {
                    log.Warning("Original text file not found or empty for template {TemplateLogId} at path '{FilePath}'. Corrections will be limited.",
                        templateLogId, template.FilePath + ".txt");
                }

                List<ShipmentInvoiceWithMetadata> allShipmentInvoicesWithMetadata;
                using (var tempServiceForMeta = new OCRCorrectionService(log))
                {
                    allShipmentInvoicesWithMetadata = ConvertDynamicToShipmentInvoicesWithMetadata(res, template, tempServiceForMeta, log);
                }

                if (!allShipmentInvoicesWithMetadata.Any())
                {
                    log.Information("No ShipmentInvoices could be converted from dynamic results for template {TemplateLogId}. Ending correction process.", templateLogId);
                    return;
                }

                log.Information("Converted {Count} ShipmentInvoices with metadata for processing against template {TemplateLogId}.", allShipmentInvoicesWithMetadata.Count, templateLogId);

                EnhancedCorrectionResult overallCorrectionResults = new EnhancedCorrectionResult();

                using var correctionServiceInstance = new OCRCorrectionService(log);
                foreach (var invoiceWithMeta in allShipmentInvoicesWithMetadata)
                {
                    var currentShipmentInvoice = invoiceWithMeta.Invoice;
                    log.Information("Processing corrections for invoice (derived) {InvoiceNo} from template {TemplateLogId}", currentShipmentInvoice.InvoiceNo, templateLogId);

                    if (TotalsZero(currentShipmentInvoice, log))
                    {
                        log.Information("Invoice {InvoiceNo} from batch is already balanced. Skipping intensive error detection/application for this item.", currentShipmentInvoice.InvoiceNo);
                        continue;
                    }

                    var errors = await correctionServiceInstance.DetectInvoiceErrorsAsync(currentShipmentInvoice, originalText, invoiceWithMeta.FieldMetadata).ConfigureAwait(false);
                    if (errors.Any())
                    {
                        var appliedCorrections = await correctionServiceInstance.ApplyCorrectionsAsync(currentShipmentInvoice, errors, originalText, invoiceWithMeta.FieldMetadata).ConfigureAwait(false);
                        var successfulValueApplications = appliedCorrections.Where(c => c.Success).ToList();

                        overallCorrectionResults.TotalCorrections += appliedCorrections.Count;
                        overallCorrectionResults.SuccessfulUpdates += successfulValueApplications.Count;

                        if (successfulValueApplications.Any())
                        {
                            await correctionServiceInstance.UpdateRegexPatternsAsync(successfulValueApplications, originalText, template.FilePath, invoiceWithMeta.FieldMetadata).ConfigureAwait(false);
                        }
                    }
                }

                UpdateDynamicResultsWithCorrections(res, allShipmentInvoicesWithMetadata.Select(x => x.Invoice).ToList(), log);
                UpdateTemplateLineValues(template, allShipmentInvoicesWithMetadata.Select(x => x.Invoice).ToList(), log);

                log.Information("Completed static batch OCR correction for template {TemplateLogId}. Applied {SuccessfulValueChanges} value changes to in-memory objects. Check logs for DB pattern update details.",
                    templateLogId, overallCorrectionResults.SuccessfulUpdates);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error in static CorrectInvoices (batch dynamic) method for template ID {TemplateId}.", template?.OcrInvoices?.Id ?? template.OcrInvoices.Id);
            }
        }

        public static async Task<List<dynamic>> ApplyDirectDataCorrectionFallbackAsync(
            List<dynamic> res,
            string originalText,
            ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            log.Warning("Applying direct data correction fallback as regex/pattern fixes might have failed or were insufficient.");

            if (res == null || !res.Any() || string.IsNullOrEmpty(originalText))
            {
                log.Warning("ApplyDirectDataCorrectionFallbackAsync: Insufficient data (res or originalText is empty).");
                return res;
            }

            try
            {
                using var correctionService = new OCRCorrectionService(log);
                List<dynamic> correctedRes = new List<dynamic>();

                foreach (var dynamicItem in res)
                {
                    if (dynamicItem is List<IDictionary<string, object>> invoiceList)
                    {
                        var tempList = new List<IDictionary<string, object>>();
                        foreach (var invoiceDict in invoiceList)
                        {
                            var tempInvoice = CreateTempShipmentInvoice(invoiceDict, log);
                            if (!TotalsZero(tempInvoice, out _, log))
                            {
                                var correctedDict = await correctionService.RequestAndApplyDirectCorrectionsAsync(tempInvoice, invoiceDict, originalText).ConfigureAwait(false);
                                tempList.Add(correctedDict ?? invoiceDict);
                            }
                            else
                            {
                                tempList.Add(invoiceDict);
                            }
                        }
                        correctedRes.Add(tempList);
                    }
                    else if (dynamicItem is IDictionary<string, object> singleInvoiceDict)
                    {
                        var tempInvoice = CreateTempShipmentInvoice(singleInvoiceDict, log);
                        if (!TotalsZero(tempInvoice, out _, log))
                        {
                            var correctedDict = await correctionService.RequestAndApplyDirectCorrectionsAsync(tempInvoice, singleInvoiceDict, originalText).ConfigureAwait(false);
                            correctedRes.Add(correctedDict ?? singleInvoiceDict);
                        }
                        else
                        {
                            correctedRes.Add(singleInvoiceDict);
                        }
                    }
                    else
                    {
                        correctedRes.Add(dynamicItem);
                    }
                }
                log.Information("Completed direct data correction fallback attempt.");
                return correctedRes;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error during direct data correction fallback process.");
                return res;
            }
        }
        #endregion

        #region Instance Helper for Direct Correction (called by static fallback)
        public async Task<IDictionary<string, object>> RequestAndApplyDirectCorrectionsAsync(ShipmentInvoice invoice, IDictionary<string, object> originalInvoiceDict, string originalDocumentText)
        {
            _logger.Information("Requesting direct data corrections from DeepSeek for invoice {InvoiceNo}", invoice.InvoiceNo);
            var prompt = this.CreateDirectDataCorrectionPrompt(new List<dynamic> { originalInvoiceDict }, originalDocumentText);
            var deepSeekResponseJson = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
            {
                _logger.Warning("No response from DeepSeek for direct data correction of {InvoiceNo}", invoice.InvoiceNo);
                return null;
            }

            IDictionary<string, object> workingInvoiceDict = originalInvoiceDict.ToDictionary(entry => entry.Key, entry => entry.Value, StringComparer.OrdinalIgnoreCase);
            bool changesApplied = false;

            try
            {
                using (var jsonDoc = JsonDocument.Parse(this.CleanJsonResponse(deepSeekResponseJson)))
                {
                    if (jsonDoc.RootElement.TryGetProperty("corrections", out var correctionsArray) && correctionsArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var corrElem in correctionsArray.EnumerateArray())
                        {
                            string fieldPath = this.GetStringValueWithLogging(corrElem, "field", 0);
                            string correctValueString = this.GetStringValueWithLogging(corrElem, "correct_value", 0);
                            string reasoning = this.GetStringValueWithLogging(corrElem, "reasoning", 0, isOptional: true);

                            if (!string.IsNullOrEmpty(fieldPath) && correctValueString != null)
                            {
                                _logger.Debug("DeepSeek direct correction proposal: Field='{FieldPath}', NewValue='{Value}', Reason='{Reason}'", fieldPath, correctValueString, reasoning);
                                object parsedValue = this.ParseCorrectedValue(correctValueString, fieldPath);

                                if (fieldPath.Contains("["))
                                {
                                    var match = Regex.Match(fieldPath, @"(?i)(?<listName>\w+)\[(?<index>\d+)\]\.(?<propName>\w+)");
                                    if (match.Success)
                                    {
                                        string listName = match.Groups["listName"].Value;
                                        int index = int.Parse(match.Groups["index"].Value);
                                        string propName = match.Groups["propName"].Value;

                                        if (workingInvoiceDict.TryGetValue(listName, out var detailsObj) &&
                                            detailsObj is List<IDictionary<string, object>> detailsList &&
                                            index >= 0 && index < detailsList.Count && detailsList[index] != null)
                                        {
                                            detailsList[index][propName] = parsedValue;
                                            changesApplied = true;
                                        }
                                        else
                                        {
                                            _logger.Warning("Direct correction: Could not apply to nested field path '{FieldPath}'. List/index issue or type mismatch.", fieldPath);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warning("Direct correction: Nested field path '{FieldPath}' format not recognized.", fieldPath);
                                    }
                                }
                                else if (workingInvoiceDict.ContainsKey(fieldPath))
                                {
                                    workingInvoiceDict[fieldPath] = parsedValue;
                                    changesApplied = true;
                                }
                                else
                                {
                                    _logger.Warning("Direct correction: Field '{FieldPath}' not found in dictionary for direct update.", fieldPath);
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Information("No 'corrections' array found in DeepSeek's direct correction response for {InvoiceNo}.", invoice.InvoiceNo);
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(jsonEx, "Failed to parse DeepSeek's direct corrections JSON response for {InvoiceNo}. Response: {ResponseSnippet}",
                    invoice.InvoiceNo, this.TruncateForLog(deepSeekResponseJson));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error applying direct corrections for {InvoiceNo}.", invoice.InvoiceNo);
                return null;
            }

            if (changesApplied)
            {
                var tempCorrectedInvoice = CreateTempShipmentInvoice(workingInvoiceDict, _logger);
                if (TotalsZero(tempCorrectedInvoice, out double finalImbalance, _logger))
                {
                    _logger.Information("Direct data correction successful and invoice {InvoiceNo} is now balanced.", invoice.InvoiceNo);
                    return workingInvoiceDict;
                }
                else
                {
                    _logger.Warning("Direct data corrections applied for {InvoiceNo}, but it is still not balanced. Final Imbalance: {ImbalanceAmount:F4}",
                       invoice.InvoiceNo, finalImbalance);
                    return workingInvoiceDict;
                }
            }
            _logger.Information("No direct data corrections were effectively applied or suggested by LLM for {InvoiceNo}", invoice.InvoiceNo);
            return null;
        }
        #endregion

        #region Static Private Helper Methods for Legacy Static Calls

        public static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<dynamic> res,
            Invoice template,
            OCRCorrectionService serviceInstance,
            ILogger logger)
        {
            var allInvoicesWithMetadata = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null)
            {
                logger.Error("ConvertDynamicToShipmentInvoicesWithMetadata: serviceInstance is null. Cannot proceed.");
                return allInvoicesWithMetadata;
            }
            try
            {
                var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);

                foreach (var item in res)
                {
                    IEnumerable<IDictionary<string, object>> currentInvoiceDicts = null;
                    if (item is List<IDictionary<string, object>> invoiceList)
                    {
                        currentInvoiceDicts = invoiceList;
                    }
                    else if (item is IDictionary<string, object> singleInvoiceDict)
                    {
                        currentInvoiceDicts = new[] { singleInvoiceDict };
                    }

                    if (currentInvoiceDicts != null)
                    {
                        foreach (var invoiceDict in currentInvoiceDicts)
                        {
                            if (invoiceDict == null) continue;
                            var shipmentInvoice = CreateTempShipmentInvoice(invoiceDict, logger);
                            var metadata = serviceInstance.ExtractEnhancedOCRMetadata(invoiceDict, template, fieldMappings);
                            allInvoicesWithMetadata.Add(new ShipmentInvoiceWithMetadata { Invoice = shipmentInvoice, FieldMetadata = metadata });
                        }
                    }
                }
                logger.Information("Successfully converted {Count} dynamic results to ShipmentInvoiceWithMetadata.", allInvoicesWithMetadata.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in ConvertDynamicToShipmentInvoicesWithMetadata.");
            }
            return allInvoicesWithMetadata;
        }

        private static string GetOriginalTextFromFile(string templateFilePath, ILogger logger)
        {
            if (string.IsNullOrEmpty(templateFilePath))
            {
                logger?.Debug("GetOriginalTextFromFile: templateFilePath is null or empty.");
                return "";
            }
            try
            {
                var txtFile = Path.ChangeExtension(templateFilePath, ".txt");
                if (File.Exists(txtFile))
                {
                    return File.ReadAllText(txtFile);
                }
                if (!txtFile.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    txtFile = templateFilePath + ".txt";
                    if (File.Exists(txtFile)) return File.ReadAllText(txtFile);
                }
                logger?.Warning("Text file not found for template path: {TemplatePath} (tried {TxtFilePath})", templateFilePath, Path.ChangeExtension(templateFilePath, ".txt"));
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error reading original text file for template path: {FilePath}", templateFilePath);
            }
            return "";
        }

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            try
            {
                var invoice = new ShipmentInvoice();
                T GetValue<T>(string key, T defaultValue = default)
                {
                    if (x.TryGetValue(key, out var val) && val != null)
                    {
                        try
                        {
                            if (typeof(T) == typeof(DateTime?) && val is string sVal && DateTime.TryParse(sVal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime dt))
                            {
                                return (T)(object)dt;
                            }
                            if (val is T alreadyTyped) return alreadyTyped;
                            return (T)Convert.ChangeType(val, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception convEx)
                        {
                            logger?.Verbose("CreateTempShipmentInvoice: Could not convert key '{Key}' value '{ValText}' (type: {ValType}) to target type {Type}. Error: {ExMsg}",
                                key, val.ToString(), val.GetType().Name, typeof(T).Name, convEx.Message);
                        }
                    }
                    return defaultValue;
                }
                double? GetNullableDouble(string key)
                {
                    if (x.TryGetValue(key, out var val) && val != null)
                    {
                        if (val is double d) return d;
                        if (val is decimal dec) return (double)dec;
                        if (val is int i) return (double)i;
                        if (val is long l) return (double)l;
                        if (double.TryParse(val.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                            return dbl;
                        logger?.Verbose("CreateTempShipmentInvoice: Could not parse key '{Key}' value '{ValText}' to double?", key, val.ToString());
                    }
                    return null;
                }

                invoice.InvoiceNo = GetValue<string>("InvoiceNo") ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceDate = GetValue<DateTime?>("InvoiceDate");
                invoice.InvoiceTotal = GetNullableDouble("InvoiceTotal");
                invoice.SubTotal = GetNullableDouble("SubTotal");
                invoice.TotalInternalFreight = GetNullableDouble("TotalInternalFreight");
                invoice.TotalOtherCost = GetNullableDouble("TotalOtherCost");
                invoice.TotalInsurance = GetNullableDouble("TotalInsurance");
                invoice.TotalDeduction = GetNullableDouble("TotalDeduction");
                invoice.Currency = GetValue<string>("Currency");
                invoice.SupplierName = GetValue<string>("SupplierName");
                invoice.SupplierAddress = GetValue<string>("SupplierAddress");


                if (x.TryGetValue("InvoiceDetails", out var detailsObj) && detailsObj is List<IDictionary<string, object>> detailsList)
                {
                    invoice.InvoiceDetails = new List<InvoiceDetails>();
                    int lineNum = 1;
                    foreach (var detailDict in detailsList.Where(d => d != null))
                    {
                        var detail = new InvoiceDetails();
                        detail.LineNumber = detailDict.TryGetValue("LineNumber", out var lnObj) && lnObj is int dictLn && dictLn > 0 ? dictLn : lineNum++;

                        detail.ItemDescription = detailDict.TryGetValue("ItemDescription", out var idVal) ? idVal?.ToString() : null;
                        detail.Quantity = detailDict.TryGetValue("Quantity", out var qtyVal) && qtyVal != null && double.TryParse(qtyVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblQty) ? dblQty : 0;
                        detail.Cost = detailDict.TryGetValue("Cost", out var costVal) && costVal != null && double.TryParse(costVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblCost) ? dblCost : 0;
                        detail.TotalCost = (detailDict.TryGetValue("TotalCost", out var tcVal) && tcVal != null && double.TryParse(tcVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblTc) ? dblTc : (double?)null);
                        detail.Discount = (detailDict.TryGetValue("Discount", out var discVal) && discVal != null && double.TryParse(discVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblDisc) ? dblDisc : (double?)null);
                        detail.Units = detailDict.TryGetValue("Units", out var unitsVal) && unitsVal != null ? unitsVal.ToString() : null;

                        if (detail.TotalCost == null)
                            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);

                        invoice.InvoiceDetails.Add(detail);
                    }
                }
                else
                {
                    invoice.InvoiceDetails = new List<InvoiceDetails>();
                }
                return invoice;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error creating temporary ShipmentInvoice from dictionary.");
                return new ShipmentInvoice { InvoiceNo = "ERROR_CREATION", InvoiceDetails = new List<InvoiceDetails>() };
            }
        }

        public static int CountTotalInvoices(List<dynamic> res)
        {
            if (res == null) return 0;
            int count = 0;
            foreach (var item in res)
            {
                if (item is List<IDictionary<string, object>> list) count += list.Count;
                else if (item is IDictionary<string, object>) count++;
            }
            return count;
        }

        private static void UpdateDynamicResultsWithCorrections(List<dynamic> res, List<ShipmentInvoice> correctedInvoices, ILogger logger)
        {
            try
            {
                int correctedInvoiceIdx = 0;
                for (int i = 0; i < res.Count; i++)
                {
                    if (correctedInvoiceIdx >= correctedInvoices.Count) break;

                    Action<IDictionary<string, object>, ShipmentInvoice> updateDict = (dict, correctedInv) => {
                        dict["InvoiceNo"] = correctedInv.InvoiceNo;
                        if (correctedInv.InvoiceDate.HasValue) dict["InvoiceDate"] = correctedInv.InvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss"); else dict.Remove("InvoiceDate");
                        if (correctedInv.SupplierName != null) dict["SupplierName"] = correctedInv.SupplierName; else dict.Remove("SupplierName");
                        if (correctedInv.SupplierAddress != null) dict["SupplierAddress"] = correctedInv.SupplierAddress; else dict.Remove("SupplierAddress");
                        if (correctedInv.Currency != null) dict["Currency"] = correctedInv.Currency; else dict.Remove("Currency");

                        dict["InvoiceTotal"] = correctedInv.InvoiceTotal;
                        dict["SubTotal"] = correctedInv.SubTotal;
                        dict["TotalInternalFreight"] = correctedInv.TotalInternalFreight;
                        dict["TotalOtherCost"] = correctedInv.TotalOtherCost;
                        dict["TotalInsurance"] = correctedInv.TotalInsurance;
                        dict["TotalDeduction"] = correctedInv.TotalDeduction;

                        if (correctedInv.InvoiceDetails != null && correctedInv.InvoiceDetails.Any())
                        {
                            var newDictDetailsList = correctedInv.InvoiceDetails.Select(cDetail =>
                                new Dictionary<string, object>{
                                    {"LineNumber", cDetail.LineNumber},
                                    {"ItemDescription", cDetail.ItemDescription},
                                    {"Quantity", cDetail.Quantity},
                                    {"Cost", cDetail.Cost},
                                    {"TotalCost", cDetail.TotalCost},
                                    {"Discount", cDetail.Discount},
                                    {"Units", cDetail.Units}
                                }).Cast<IDictionary<string, object>>().ToList();
                            dict["InvoiceDetails"] = newDictDetailsList;
                        }
                        else
                        {
                            dict.Remove("InvoiceDetails");
                        }
                    };

                    if (res[i] is List<IDictionary<string, object>> invoiceList)
                    {
                        for (int j = 0; j < invoiceList.Count; j++)
                        {
                            if (correctedInvoiceIdx >= correctedInvoices.Count) break;
                            updateDict(invoiceList[j], correctedInvoices[correctedInvoiceIdx++]);
                        }
                    }
                    else if (res[i] is IDictionary<string, object> singleInvoiceDict)
                    {
                        if (correctedInvoiceIdx < correctedInvoices.Count)
                            updateDict(singleInvoiceDict, correctedInvoices[correctedInvoiceIdx++]);
                    }
                }
                logger.Information("UpdateDynamicResults: Updated {Count} dynamic result entries with corrected invoice data.", correctedInvoiceIdx);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in UpdateDynamicResultsWithCorrections.");
            }
        }

        private static void UpdateTemplateLineValues(Invoice template, List<ShipmentInvoice> correctedInvoices, ILogger log)
        {
            if (template?.Lines == null || !correctedInvoices.Any()) return;
            var templateLogId = template.OcrInvoices?.Id.ToString() ?? template.OcrInvoices.Id.ToString();
            log.Information("Attempting to update template (LogID: {TemplateLogId}) LineValues based on {Count} corrected invoices.", templateLogId, correctedInvoices.Count);

            var fieldMappings = GetTemplateFieldMappings(template, log);
            var representativeCorrectedInvoice = correctedInvoices.First();

            Action<string, object> updateTemplateField = (fieldName, value) => {
                string stringValue = null;
                if (value is DateTime dt) stringValue = dt.ToString("yyyy-MM-dd");
                else if (value is double dbl || value is decimal dec) stringValue = Convert.ToDouble(value).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                else stringValue = value?.ToString();
                UpdateFieldInTemplate(template, fieldMappings, fieldName, stringValue, log);
            };

            updateTemplateField("InvoiceTotal", representativeCorrectedInvoice.InvoiceTotal);
            updateTemplateField("SubTotal", representativeCorrectedInvoice.SubTotal);
            updateTemplateField("TotalInternalFreight", representativeCorrectedInvoice.TotalInternalFreight);
            updateTemplateField("TotalOtherCost", representativeCorrectedInvoice.TotalOtherCost);
            updateTemplateField("TotalInsurance", representativeCorrectedInvoice.TotalInsurance);
            updateTemplateField("TotalDeduction", representativeCorrectedInvoice.TotalDeduction);
            updateTemplateField("InvoiceNo", representativeCorrectedInvoice.InvoiceNo);
            updateTemplateField("InvoiceDate", representativeCorrectedInvoice.InvoiceDate);
            updateTemplateField("SupplierName", representativeCorrectedInvoice.SupplierName);
            updateTemplateField("Currency", representativeCorrectedInvoice.Currency);

            log.Information("Template LineValues updated for header fields of template {TemplateLogId}.", templateLogId);
        }

        private static Dictionary<string, (int LineId, int FieldId)> GetTemplateFieldMappings(Invoice template, ILogger logger)
        {
            var mappings = new Dictionary<string, (int LineId, int FieldId)>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (template?.Lines == null) return mappings;
                foreach (var lineWrapper in template.Lines.Where(lw => lw?.OCR_Lines?.Fields != null))
                {
                    var ocrLineDef = lineWrapper.OCR_Lines;
                    foreach (var fieldDef in ocrLineDef.Fields)
                    {
                        var canonicalPropName = MapTemplateFieldToPropertyName(fieldDef.Field);
                        if (!string.IsNullOrEmpty(canonicalPropName) && !mappings.ContainsKey(canonicalPropName))
                        {
                            mappings[canonicalPropName] = (ocrLineDef.Id, fieldDef.Id);
                        }
                        var canonicalKeyName = MapTemplateFieldToPropertyName(fieldDef.Key);
                        if (!string.IsNullOrEmpty(canonicalKeyName) && !mappings.ContainsKey(canonicalKeyName) &&
                            (string.IsNullOrEmpty(canonicalPropName) || !canonicalKeyName.Equals(canonicalPropName, StringComparison.OrdinalIgnoreCase)))
                        {
                            mappings[canonicalKeyName] = (ocrLineDef.Id, fieldDef.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var templateLogId = template?.OcrInvoices?.Id.ToString() ?? template?.OcrInvoices.Id.ToString();
                logger.Error(ex, "Error in GetTemplateFieldMappings for template (LogID: {TemplateLogId})", templateLogId);
            }
            return mappings;
        }
        private static void UpdateFieldInTemplate(Invoice template, Dictionary<string, (int LineId, int FieldId)> fieldMappings,
            string canonicalPropertyName, string newStringValue, ILogger log)
        {
            if (newStringValue == null || !fieldMappings.TryGetValue(canonicalPropertyName, out var mappingInfo)) return;

            var invoiceLineWrapper = template.Lines?.FirstOrDefault(lWrapper => lWrapper.OCR_Lines?.Id == mappingInfo.LineId);
            var targetOcrLineDef = (Line: invoiceLineWrapper?.OCR_Lines, LineValue: invoiceLineWrapper.Values);

            if (!targetOcrLineDef.LineValue.Any())
            {
                log.Debug("UpdateFieldInTemplate: No LineValue (extracted instances) found for LineDefId {LineId} to update field {PropName}", mappingInfo.LineId, canonicalPropertyName);
                return;
            }

            bool wasUpdatedInAnyInstance = false;
            foreach (var lineInstanceEntry in targetOcrLineDef.LineValue.ToList())
            {
                var fieldValuesInThisInstance = lineInstanceEntry.Value;
                if (fieldValuesInThisInstance == null) continue;

                var fieldTupleKeyToUpdate = fieldValuesInThisInstance.Keys
                                            .FirstOrDefault(keyTuple => keyTuple.Item1?.Id == mappingInfo.FieldId);

                if (fieldTupleKeyToUpdate.Item1 != null)
                {
                    if (fieldValuesInThisInstance.TryGetValue(fieldTupleKeyToUpdate, out string oldValueInTemplate) &&
                        string.Equals(oldValueInTemplate, newStringValue))
                    {
                        wasUpdatedInAnyInstance = true;
                        continue;
                    }
                    fieldValuesInThisInstance[fieldTupleKeyToUpdate] = newStringValue ?? "";
                    wasUpdatedInAnyInstance = true;
                }
            }

            if (wasUpdatedInAnyInstance)
            {
                log.Information("Updated/Verified template LineValue for {PropName} (FieldId: {FieldId}, LineDefId: {LineId}) to '{NewVal}' across its instances.",
                   canonicalPropertyName, mappingInfo.FieldId, mappingInfo.LineId, newStringValue ?? "NULL");
            }
            else
            {
                log.Warning("Could not find target FieldId {FieldId} in any LineValue instances for LineDefId {LineId} to update {PropName}.",
                   mappingInfo.FieldId, mappingInfo.LineId, canonicalPropertyName);
            }
        }

        #endregion

        #region Data Models for Legacy Support (used by static methods in this file)
        public class EnhancedFieldMapping
        {
            public int LineId { get; set; }
            public int FieldId { get; set; }
            public int PartId { get; set; }
            public string RegexPattern { get; set; }
            public string Key { get; set; }
            public string FieldName { get; set; }
            public string EntityType { get; set; }
            public string DataType { get; set; }
        }

        public class DirectCorrection
        {
            public string FieldName { get; set; }
            public string CurrentValue { get; set; }
            public string CorrectedValue { get; set; }
            public string Reasoning { get; set; }
        }

        public static double TotalsZeroAmount(ShipmentInvoice invoice)
        {
            if (invoice != null && _totalsZeroAmounts.TryGetValue(invoice, out var box))
            {
                return box.Value;
            }
            return double.NaN;
        }
        #endregion
    }
}