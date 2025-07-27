
﻿// File: OCRCorrectionService/OCRLegacySupport.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using Serilog;
using InvoiceReader.OCRCorrectionService;
using InvoiceReader.PipelineInfrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Core.Common.Extensions;
using WaterNut.DataSpace.PipelineInfrastructure;

namespace WaterNut.DataSpace
{
    using System.Linq;

    using global::EntryDataDS.Business.Entities;
    using InvoiceReader.OCRCorrectionService;
    using InvoiceReader.PipelineInfrastructure;
    using MoreLinq;

    using Invoices = global::EntryDataDS.Business.Entities.Invoices;

    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Now Multi-Invoice and Nested-List Aware)

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ShipmentInvoice, System.Runtime.CompilerServices.StrongBox<double>> _totalsZeroAmounts =
            new System.Runtime.CompilerServices.ConditionalWeakTable<ShipmentInvoice, System.Runtime.CompilerServices.StrongBox<double>>();

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Invoice financial balance validator with LLM diagnostic workflow enforcement
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
        /// **VALIDATION LOGIC**: Sums SubTotal+Freight+OtherCost+Insurance-Deduction, compares to InvoiceTotal with precision tolerance
        /// **RETURN BEHAVIOR**: Boolean balance status + precise difference amount for diagnostic correlation and pipeline decisions
        /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis without additional context requirements
        /// </summary>
        public static bool TotalsZero(ShipmentInvoice invoice, out double differenceAmount, ILogger logger)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Complete LLM diagnostic workflow implementation
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            
            // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
            log.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for invoice balance validation");
            log.Error("📋 **AVAILABLE_LOG_DATA**: Current context - TotalsZero validation with precision calculation");
            log.Error("🔍 **PATTERN_ANALYSIS**: Financial balance validation pattern with null-safe value extraction");
            log.Error("❓ **EVIDENCE_GAPS**: Need complete invoice component analysis and tolerance-based determination");
            log.Error("💡 **LOG_BASED_HYPOTHESIS**: Invoice balance validation requires step-by-step calculation logging");
            
            // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
            log.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for balance calculation");
            log.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed component extraction, calculation steps, tolerance validation");
            log.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Invoice components, balance calculation, difference measurement, business logic");
            
            // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
            log.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based invoice balance validation");
            log.Error("📚 **FIX_RATIONALE**: Based on financial integrity requirements, implementing comprehensive balance validation");
            log.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring balance calculation accuracy and tolerance determination");
            
            differenceAmount = double.MaxValue;
            
            if (invoice == null)
            {
                // **v4.1 NULL VALIDATION LOGGING**: LLM diagnostic evidence for null invoice scenarios
                log.Error("❌ **NULL_INVOICE_EVIDENCE**: TotalsZero validation attempted with null invoice reference");
                log.Error("📋 **AVAILABLE_LOG_DATA**: Null validation context - no invoice object available for analysis");
                log.Error("🔍 **PATTERN_ANALYSIS**: Null input pattern requires graceful degradation with diagnostic preservation");
                log.Error("💡 **LOG_BASED_HYPOTHESIS**: Null invoice indicates upstream data conversion failure or invalid call context");
                log.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced null handling with comprehensive error context preservation");
                log.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Validation state, return behavior, difference amount assignment");
                log.Error("📚 **FIX_RATIONALE**: Null invoice requires false return with MaxValue difference for pipeline safety");
                log.Error("🔍 **FIX_VALIDATION**: Monitoring null invoice frequency and upstream cause identification");
                
                return false;
            }

            // **v4.1 COMPONENT EXTRACTION LOGGING**: Enhanced financial component analysis
            log.Error("📊 **COMPONENT_EXTRACTION_START**: Financial component extraction with null-safe value assignment");
            var subTotal = invoice.SubTotal ?? 0;
            var freight = invoice.TotalInternalFreight ?? 0;
            var otherCost = invoice.TotalOtherCost ?? 0;
            var insurance = invoice.TotalInsurance ?? 0;
            var deductionAmount = invoice.TotalDeduction ?? 0;
            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;
            
            log.Error("📋 **AVAILABLE_LOG_DATA**: Invoice components extracted - InvoiceNo={InvoiceNo}", invoice.InvoiceNo ?? "Unknown");
            log.Error("🔍 **PATTERN_ANALYSIS**: Component values - SubTotal={SubTotal}, Freight={Freight}, OtherCost={OtherCost}, Insurance={Insurance}", 
                subTotal, freight, otherCost, insurance);
            log.Error("❓ **EVIDENCE_GAPS**: Adjustment analysis - Deduction={Deduction}, ReportedTotal={ReportedTotal}", deductionAmount, reportedInvoiceTotal);

            // **v4.1 CALCULATION LOGGING**: Step-by-step balance calculation with enhanced diagnostics
            log.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing balance calculation logging for precision analysis");
            var baseTotal = subTotal + freight + otherCost + insurance;
            var calculatedFinalTotal = baseTotal - deductionAmount;
            differenceAmount = Math.Abs(calculatedFinalTotal - reportedInvoiceTotal);
            
            log.Error("📊 **LOGGING_ENHANCEMENTS**: Balance calculation steps - BaseTotal={BaseTotal}, CalculatedFinal={CalculatedFinal}, Difference={Difference}", 
                baseTotal, calculatedFinalTotal, differenceAmount);

            // **v4.1 TOLERANCE DETERMINATION LOGGING**: Business logic validation with comprehensive reasoning
            bool isZero = differenceAmount < 0.001; // Precision tolerance for floating-point arithmetic
            
            log.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Balance determination - IsBalanced={IsBalanced}, Tolerance=0.001, DifferenceAmount={DifferenceAmount}", 
                isZero, differenceAmount);
            log.Error("📚 **FIX_RATIONALE**: Business logic - {BalanceStatus}", 
                isZero ? "Invoice totals balanced within acceptable tolerance" : "Invoice requires correction due to imbalance");

            // **v4.1 CACHING LOGGING**: Performance optimization with diagnostic preservation
            log.Error("🔍 **FIX_VALIDATION**: Difference amount caching for diagnostic correlation and performance optimization");
            _totalsZeroAmounts.Remove(invoice);
            _totalsZeroAmounts.Add(invoice, new System.Runtime.CompilerServices.StrongBox<double>(differenceAmount));
            
            log.Error("✅ **VALIDATION_COMPLETE**: Balance validation result={IsBalanced}, DifferenceAmount={DifferenceAmount}", 
                isZero, differenceAmount);
            log.Error("🎯 **SUCCESS_ASSERTION**: Invoice balance validation completed with cached difference amount for LLM analysis");
            
            return isZero;
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Invoice balance validator wrapper without difference output
        /// 
        /// **LOG_THE_WHAT**: Simplified invoice balance validation delegating to comprehensive validation method
        /// **LOG_THE_HOW**: Calls full TotalsZero method with discarded difference amount output parameter
        /// **LOG_THE_WHY**: Provides convenient API when precise difference amount is not needed by caller
        /// **LOG_THE_WHO**: Returns boolean balance status without exposing difference calculation details
        /// **LOG_THE_WHAT_IF**: Expects valid invoice and logger; inherits all validation logic from primary method
        /// </summary>
        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger) => TotalsZero(invoice, out _, logger);

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Dynamic invoice data balance validator with nested structure support
        /// 
        /// **LOG_THE_WHAT**: Dynamic invoice balance validation handling nested list structures and dictionary conversion
        /// **LOG_THE_HOW**: Extracts dictionaries from dynamic data, creates temporary invoice, validates balance using core logic
        /// **LOG_THE_WHY**: Enables balance validation on dynamic OCR results before ShipmentInvoice object creation
        /// **LOG_THE_WHO**: Returns boolean balance status and outputs total imbalance sum for pipeline decisions
        /// **LOG_THE_WHAT_IF**: Expects dynamic invoice data; handles nested lists and empty data gracefully; defaults to balanced
        /// </summary>
        public static bool TotalsZero(List<dynamic> dynamicInvoiceResults, out double totalImbalanceSum, ILogger logger = null)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Complete dynamic invoice balance validation narrative
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            log.Verbose("🦠 **DYNAMIC_TOTALS_VALIDATION_START**: Dynamic invoice data balance validation with structure analysis");
            log.Verbose("   - **ARCHITECTURAL_INTENT**: Validate dynamic OCR results balance before object conversion and processing");
            log.Verbose("   - **DATA_HANDLING**: Support both flat dynamic lists and nested list structures from OCR pipeline");
            
            totalImbalanceSum = 0.0;
            
            if (dynamicInvoiceResults == null || !dynamicInvoiceResults.Any())
            {
                // **LOG_THE_WHAT_IF**: Empty data handling with default balanced assumption
                log.Verbose("⚠️ **EMPTY_DYNAMIC_DATA**: No dynamic invoice data provided for balance validation");
                log.Verbose("   - **DATA_STATE**: DynamicResults={DataState}, Count={Count}", 
                    dynamicInvoiceResults == null ? "NULL" : "EMPTY", dynamicInvoiceResults?.Count ?? 0);
                log.Verbose("   - **DEFAULT_BEHAVIOR**: Returning true (balanced) for empty data sets");
                
                return true;
            }

            // **LOG_THE_HOW**: Dynamic data structure analysis and dictionary extraction
            log.Verbose("🔍 **STRUCTURE_ANALYSIS**: Analyzing dynamic data structure for dictionary extraction");
            log.Verbose("   - **INPUT_ANALYSIS**: Count={Count}, FirstItemType={FirstItemType}", 
                dynamicInvoiceResults.Count, dynamicInvoiceResults[0]?.GetType().Name ?? "NULL");
            
            var dictionaries = new List<IDictionary<string, object>>();
            
            if (dynamicInvoiceResults.Any() && dynamicInvoiceResults[0] is IList nestedList)
            {
                log.Verbose("🔄 **NESTED_LIST_DETECTED**: Extracting dictionaries from nested list structure");
                dictionaries = nestedList.OfType<IDictionary<string, object>>().ToList();
                log.Verbose("   - **NESTED_EXTRACTION**: Found {DictionaryCount} dictionaries in nested structure", dictionaries.Count);
            }
            else
            {
                log.Verbose("🔄 **FLAT_LIST_DETECTED**: Extracting dictionaries from flat dynamic structure");
                dictionaries = dynamicInvoiceResults.OfType<IDictionary<string, object>>().ToList();
                log.Verbose("   - **FLAT_EXTRACTION**: Found {DictionaryCount} dictionaries in flat structure", dictionaries.Count);
            }

            if (!dictionaries.Any())
            {
                // **LOG_THE_WHAT_IF**: No dictionaries found - default to balanced
                log.Verbose("⚠️ **NO_DICTIONARIES_FOUND**: No dictionary objects found in dynamic data for validation");
                log.Verbose("   - **EXTRACTION_RESULT**: No IDictionary<string, object> items available for invoice creation");
                log.Verbose("   - **DEFAULT_BEHAVIOR**: Returning true (balanced) when no processable data available");
                
                return true;
            }

            // **LOG_THE_WHO**: Temporary invoice creation and balance validation
            log.Verbose("🏗️ **TEMP_INVOICE_CREATION**: Creating temporary ShipmentInvoice for balance validation");
            var tempInvoice = CreateTempShipmentInvoice(dictionaries.First(), log);
            
            if (tempInvoice == null)
            {
                log.Warning("❌ **TEMP_INVOICE_CREATION_FAILED**: Could not create temporary invoice from dynamic data");
                return true; // Default to balanced if conversion fails
            }
            
            log.Verbose("🦠 **BALANCE_VALIDATION_DELEGATION**: Delegating to ShipmentInvoice balance validation logic");
            var isBalanced = TotalsZero(tempInvoice, out totalImbalanceSum, log);
            
            log.Verbose("✅ **DYNAMIC_TOTALS_VALIDATION_COMPLETE**: Dynamic balance validation result={IsBalanced}, Imbalance={Imbalance}", 
                isBalanced, totalImbalanceSum);
            log.Verbose("   - **SUCCESS_ASSERTION**: Dynamic invoice data balance validation completed with ShipmentInvoice logic");
            
            return isBalanced;
        }

        public static bool ShouldContinueCorrections(List<dynamic> res, out double totalImbalanceSum, ILogger logger = null)
        {
            return !TotalsZero(res, out totalImbalanceSum, logger);
        }

        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Template template, List<string> textLines, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));

            try
            {
                log.Error("🏁 **CORRECT_INVOICES_ENTRY**: Starting correction process with native data structure handling.");
                log.Error("   - **ARCHITECTURAL_INTENT**: Heal DB -> Read -> Detect -> Apply -> Learn -> Sync -> Re-wrap.");

                var actualInvoiceData = new List<IDictionary<string, object>>();
                if (res != null && res.Any() && res[0] is IList nestedList)
                {
                    actualInvoiceData = nestedList.OfType<IDictionary<string, object>>().ToList();
                }
                else
                {
                    actualInvoiceData = res?.OfType<IDictionary<string, object>>().ToList() ?? new List<IDictionary<string, object>>();
                }

                using (var dbContextForValidation = new OCRContext())
                {
                    var validator = new DatabaseValidator(dbContextForValidation, log);
                    validator.ValidateAndHealTemplate();
                }

                GetTemplatesStep.InvalidateTemplateCache();
                var freshTemplate = GetTemplatesStep.GetAllTemplates(
                    new InvoiceProcessingContext(logger) { FilePath = template.FilePath },
                    new OCRContext()
                ).FirstOrDefault(x => x.OcrTemplates.Id == template.OcrTemplates.Id);

                if (freshTemplate == null)
                {
                    log.Error("   - ❌ FATAL_ERROR: Could not reload template with ID {TemplateId}. Aborting correction.", template.OcrTemplates.Id);
                    return res;
                }

                if (!ShouldContinueCorrections(res, out var imbalance, log))
                {
                    log.Error("     - ✅ **INTENTION_MET**: Invoice is balanced by {Imbalance:F2}. No AI correction needed.", imbalance);
                    return res;
                }
                log.Error("     - ❌ **INTENTION_FAILED**: Invoice unbalanced by {Imbalance:F2}. Proceeding to AI learning.", imbalance);

                using (var correctionService = new OCRCorrectionService(log))
                {
                    var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(actualInvoiceData, freshTemplate, correctionService, log);


                    if (!shipmentInvoicesWithMeta.Any())
                    {
                        log.Error("     - ❌ ERROR: Could not convert dynamic data to ShipmentInvoice object for AI analysis. Aborting.");
                        return res;
                    }

                    // ======================================================================================
                    //                          *** DEFINITIVE FIX IS HERE ***
                    // The logic now iterates over all invoices instead of just processing the first one.
                    // This ensures all invoices are corrected and their data can be synced back.
                    // ======================================================================================

                    var allUpdateRequests = new List<RegexUpdateRequest>();
                    var fullDocumentText = template.FormattedPdfText;

                    foreach (var invoiceWrapper in shipmentInvoicesWithMeta)
                    {
                        log.Information("   - 🔎 Processing corrections for InvoiceNo: {InvoiceNo}", invoiceWrapper.Invoice.InvoiceNo);

                        // ====== INVOICE PRE-CORRECTION TOTALS VERIFICATION ======
                        if (TotalsZero(invoiceWrapper.Invoice, out double preCorrectionsImbalance, log))
                        {
                            log.Information("🧮 **PRE_CORRECTIONS_TOTALS**: Invoice {InvoiceNo} is balanced before corrections. TotalsZero=0, Imbalance={Imbalance}", 
                                invoiceWrapper.Invoice.InvoiceNo, preCorrectionsImbalance);
                        }
                        else
                        {
                            log.Information("🧮 **PRE_CORRECTIONS_TOTALS**: Invoice {InvoiceNo} is UNBALANCED before corrections. TotalsZero≠0, Imbalance={Imbalance}", 
                                invoiceWrapper.Invoice.InvoiceNo, preCorrectionsImbalance);
                            log.Information("   - 📊 Pre-Correction Financial State: SubTotal={SubTotal}, Freight={Freight}, OtherCost={OtherCost}, Insurance={Insurance}, Deduction={Deduction}, InvoiceTotal={InvoiceTotal}",
                                invoiceWrapper.Invoice.SubTotal ?? 0, invoiceWrapper.Invoice.TotalInternalFreight ?? 0, invoiceWrapper.Invoice.TotalOtherCost ?? 0,
                                invoiceWrapper.Invoice.TotalInsurance ?? 0, invoiceWrapper.Invoice.TotalDeduction ?? 0, invoiceWrapper.Invoice.InvoiceTotal ?? 0);
                        }

                        var allDetectedErrors = await correctionService.DetectInvoiceErrorsAsync(invoiceWrapper.Invoice, fullDocumentText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);

                        if (!allDetectedErrors.Any())
                        {
                            log.Error("     - ⚠️ WARNING for Invoice {InvoiceNo}: Correction pipeline triggered, but AI error detection found no omissions. The imbalance may remain.", invoiceWrapper.Invoice.InvoiceNo);
                            continue; // Skip to the next invoice
                        }
                        log.Information("     - ✅ AI detected {ErrorCount} potential errors/omissions for Invoice {InvoiceNo}.", allDetectedErrors.Count, invoiceWrapper.Invoice.InvoiceNo);

                        var appliedCorrections = await correctionService.ApplyCorrectionsAsync(invoiceWrapper.Invoice, allDetectedErrors, fullDocumentText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);
                        log.Information("     - ✅ Applied {AppliedCount} corrections directly to Invoice {InvoiceNo}.", appliedCorrections.Count(c => c.Success), invoiceWrapper.Invoice.InvoiceNo);

                        // ====== INVOICE POST-CORRECTION TOTALS VERIFICATION ======
                        if (TotalsZero(invoiceWrapper.Invoice, out double postCorrectionsImbalance, log))
                        {
                            log.Information("🧮 **POST_CORRECTIONS_TOTALS**: Invoice {InvoiceNo} is NOW BALANCED after corrections. TotalsZero=0, Imbalance={Imbalance}", 
                                invoiceWrapper.Invoice.InvoiceNo, postCorrectionsImbalance);
                        }
                        else
                        {
                            log.Warning("🧮 **POST_CORRECTIONS_TOTALS**: Invoice {InvoiceNo} is STILL UNBALANCED after corrections. TotalsZero≠0, Imbalance={Imbalance}", 
                                invoiceWrapper.Invoice.InvoiceNo, postCorrectionsImbalance);
                            log.Information("   - 📊 Post-Correction Financial State: SubTotal={SubTotal}, Freight={Freight}, OtherCost={OtherCost}, Insurance={Insurance}, Deduction={Deduction}, InvoiceTotal={InvoiceTotal}",
                                invoiceWrapper.Invoice.SubTotal ?? 0, invoiceWrapper.Invoice.TotalInternalFreight ?? 0, invoiceWrapper.Invoice.TotalOtherCost ?? 0,
                                invoiceWrapper.Invoice.TotalInsurance ?? 0, invoiceWrapper.Invoice.TotalDeduction ?? 0, invoiceWrapper.Invoice.InvoiceTotal ?? 0);
                            
                            var expectedTotal = (invoiceWrapper.Invoice.SubTotal ?? 0) + (invoiceWrapper.Invoice.TotalInternalFreight ?? 0) + (invoiceWrapper.Invoice.TotalOtherCost ?? 0) + (invoiceWrapper.Invoice.TotalInsurance ?? 0) - (invoiceWrapper.Invoice.TotalDeduction ?? 0);
                            log.Warning("   - 🧮 Expected Calculation: {SubTotal} + {Freight} + {OtherCost} + {Insurance} - {Deduction} = {Expected} (vs Actual: {Actual})",
                                invoiceWrapper.Invoice.SubTotal ?? 0, invoiceWrapper.Invoice.TotalInternalFreight ?? 0, invoiceWrapper.Invoice.TotalOtherCost ?? 0,
                                invoiceWrapper.Invoice.TotalInsurance ?? 0, invoiceWrapper.Invoice.TotalDeduction ?? 0, expectedTotal, invoiceWrapper.Invoice.InvoiceTotal ?? 0);
                        }

                        // Apply Caribbean customs rules for each invoice
                        var customsCorrections = correctionService.ApplyCaribbeanCustomsRules(
                            invoiceWrapper.Invoice,
                            appliedCorrections.Where(c => c.Success).ToList());
                        if (customsCorrections.Any())
                        {
                            correctionService.ApplyCaribbeanCustomsCorrectionsToInvoice(invoiceWrapper.Invoice, customsCorrections);
                            log.Information("     - ✅ Applied {CustomsCount} Caribbean customs rules to Invoice {InvoiceNo}.", customsCorrections.Count, invoiceWrapper.Invoice.InvoiceNo);
                        }

                        // Accumulate database update requests from all detected errors for this invoice
                        var updateRequestsForThisInvoice = allDetectedErrors.DistinctBy(x => new { x.Field, x.SuggestedRegex }).Select(e => new RegexUpdateRequest
                        {
                            FieldName = e.Field,
                            OldValue = e.ExtractedValue,
                            NewValue = e.CorrectValue,
                            CorrectionType = e.ErrorType,
                            Confidence = e.Confidence,
                            DeepSeekReasoning = e.Reasoning,
                            LineNumber = e.LineNumber,
                            LineText = e.LineText,
                            ContextLinesBefore = e.ContextLinesBefore,
                            ContextLinesAfter = e.ContextLinesAfter,
                            RequiresMultilineRegex = e.RequiresMultilineRegex,
                            SuggestedRegex = e.SuggestedRegex,
                            InvoiceId = template.OcrTemplates.Id,
                            FilePath = template.FilePath,
                           
                            // =================== THE FIX ===================
                            // Add the missing mappings here.
                            Pattern = e.Pattern,
                            Replacement = e.Replacement
                            // ===============================================
                        }).ToList();

                        allUpdateRequests.AddRange(updateRequestsForThisInvoice);
                    }

                    // Perform all database learning updates in one batch after processing all invoices
                    await correctionService.UpdateRegexPatternsAsync(allUpdateRequests).ConfigureAwait(false);

                    // Sync all corrected invoices back to the original dynamic data structure
                    var correctedInvoices = shipmentInvoicesWithMeta.Select(siwm => siwm.Invoice).ToList();
                    
                    // ====== FREE SHIPPING PRE-SYNC DIAGNOSTIC ======
                    var freeShippingInvoicesForSync = correctedInvoices.Where(inv => inv.TotalDeduction.HasValue && inv.TotalDeduction.Value > 0).ToList();
                    if (freeShippingInvoicesForSync.Any())
                    {
                        log.Information("🚢 **FREE_SHIPPING_PRE_SYNC**: About to sync {Count} corrected invoices with Free Shipping data back to dynamic items", freeShippingInvoicesForSync.Count);
                        foreach (var inv in freeShippingInvoicesForSync)
                        {
                            log.Information("   - 📊 Pre-Sync Invoice {InvoiceNo}: TotalDeduction={TotalDeduction}", inv.InvoiceNo, inv.TotalDeduction);
                        }
                    }
                    
                    UpdateDynamicResultsWithCorrections(actualInvoiceData, correctedInvoices, log);

                    // ====== CORRECTED INVOICES FINAL TOTALS VERIFICATION ======
                    foreach (var correctedInvoice in correctedInvoices)
                    {
                        if (TotalsZero(correctedInvoice, out double finalImbalance, log))
                        {
                            log.Information("🧮 **CORRECTED_INVOICE_FINAL_TOTALS**: Invoice {InvoiceNo} is BALANCED after full correction pipeline. TotalsZero=0, Imbalance={Imbalance}", 
                                correctedInvoice.InvoiceNo, finalImbalance);
                        }
                        else
                        {
                            log.Error("🧮 **CORRECTED_INVOICE_FINAL_TOTALS**: Invoice {InvoiceNo} is STILL UNBALANCED after full correction pipeline. TotalsZero≠0, Imbalance={Imbalance}", 
                                correctedInvoice.InvoiceNo, finalImbalance);
                            log.Error("   - ❌ CRITICAL: This means the correction pipeline failed to balance the invoice!");
                        }
                    }
                }

                // ====== DYNAMIC DATA SERIALIZATION AND VERIFICATION ======
                try
                {
                    var actualInvoiceDataJson = System.Text.Json.JsonSerializer.Serialize(actualInvoiceData, new System.Text.Json.JsonSerializerOptions 
                    { 
                        WriteIndented = true, 
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
                    });
                    log.Information("📄 **ACTUAL_INVOICE_DATA_SERIALIZED**: Complete dynamic data structure after corrections: {ActualInvoiceDataJson}", actualInvoiceDataJson);
                }
                catch (Exception jsonEx)
                {
                    log.Warning("📄 **ACTUAL_INVOICE_DATA_SERIALIZATION_FAILED**: Could not serialize actualInvoiceData: {Error}", jsonEx.Message);
                }

                // ====== DYNAMIC DATA TOTALS VERIFICATION ======
                foreach (var dynamicItem in actualInvoiceData)
                {
                    if (dynamicItem.ContainsKey("InvoiceNo"))
                    {
                        var invoiceNo = dynamicItem["InvoiceNo"]?.ToString() ?? "Unknown";
                        var subTotal = dynamicItem.ContainsKey("SubTotal") ? Convert.ToDouble(dynamicItem["SubTotal"] ?? 0) : 0;
                        var freight = dynamicItem.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(dynamicItem["TotalInternalFreight"] ?? 0) : 0;
                        var otherCost = dynamicItem.ContainsKey("TotalOtherCost") ? Convert.ToDouble(dynamicItem["TotalOtherCost"] ?? 0) : 0;
                        var insurance = dynamicItem.ContainsKey("TotalInsurance") ? Convert.ToDouble(dynamicItem["TotalInsurance"] ?? 0) : 0;
                        var deduction = dynamicItem.ContainsKey("TotalDeduction") ? Convert.ToDouble(dynamicItem["TotalDeduction"] ?? 0) : 0;
                        var invoiceTotal = dynamicItem.ContainsKey("InvoiceTotal") ? Convert.ToDouble(dynamicItem["InvoiceTotal"] ?? 0) : 0;
                        
                        var expectedTotal = subTotal + freight + otherCost + insurance - deduction;
                        var difference = invoiceTotal - expectedTotal;
                        
                        if (Math.Abs(difference) < 0.01)
                        {
                            log.Information("🧮 **DYNAMIC_DATA_TOTALS**: Dynamic item {InvoiceNo} is BALANCED. Expected={Expected}, Actual={Actual}, Difference={Difference}", 
                                invoiceNo, expectedTotal, invoiceTotal, difference);
                        }
                        else
                        {
                            log.Error("🧮 **DYNAMIC_DATA_TOTALS**: Dynamic item {InvoiceNo} is UNBALANCED. Expected={Expected}, Actual={Actual}, Difference={Difference}", 
                                invoiceNo, expectedTotal, invoiceTotal, difference);
                            log.Error("   - 📊 Dynamic Item Financial State: SubTotal={SubTotal}, Freight={Freight}, OtherCost={OtherCost}, Insurance={Insurance}, Deduction={Deduction}, InvoiceTotal={InvoiceTotal}",
                                subTotal, freight, otherCost, insurance, deduction, invoiceTotal);
                        }
                    }
                }

                // ====== FREE SHIPPING FINAL VERIFICATION BEFORE RETURN ======
                var finalFreeShippingVerification = actualInvoiceData
                    .Where(item => item.ContainsKey("TotalDeduction") && 
                                   item["TotalDeduction"] != null && 
                                   double.TryParse(item["TotalDeduction"].ToString(), out double val) && val > 0)
                    .ToList();
                    
                if (finalFreeShippingVerification.Any())
                {
                    log.Information("🚢 **FREE_SHIPPING_FINAL_VERIFICATION**: {Count} dynamic items have TotalDeduction values before returning to ReadPDFTextMethod", finalFreeShippingVerification.Count);
                    foreach (var item in finalFreeShippingVerification)
                    {
                        var invoiceNo = item.ContainsKey("InvoiceNo") ? item["InvoiceNo"]?.ToString() : "Unknown";
                        var totalDeduction = item["TotalDeduction"]?.ToString();
                        log.Information("   - 📊 Final Invoice {InvoiceNo}: TotalDeduction={TotalDeduction} (ready for ReadPDFTextMethod)", invoiceNo, totalDeduction);
                    }
                }
                else
                {
                    log.Warning("⚠️ **FREE_SHIPPING_FINAL_VERIFICATION_MISSING**: No dynamic items with TotalDeduction values found before returning to ReadPDFTextMethod");
                }

                var finalResult = new List<dynamic> { actualInvoiceData };
                log.Information("   - **STEP 6: RE-WRAP_DATA**: Correction process complete. Re-wrapping corrected data into nested list structure before returning.");

                log.Information("🏁 **CORRECT_INVOICES_EXIT**: OCR correction pipeline complete.");

                return finalResult;

            }
            catch (Exception ex)
            {
                log.Error(ex, "🚨 **CORRECT_INVOICES_CRASH**: An unhandled exception occurred in the main correction pipeline.");
                return res; // Return original data on crash
            }
        }

        public static void UpdateDynamicResultsWithCorrections(
            List<IDictionary<string, object>> dynamicItems,
            List<ShipmentInvoice> correctedInvoices,
            ILogger logger)
        {
            if (dynamicItems == null || !correctedInvoices.Any()) 
            {
                logger.Warning("🔄 **SYNC_DATA_SKIP**: UpdateDynamicResultsWithCorrections called with null or empty data. DynamicItems={DynamicItems}, CorrectedInvoices={CorrectedInvoices}", 
                    dynamicItems?.Count ?? 0, correctedInvoices?.Count ?? 0);
                return;
            }
            var correctedInvoiceMap = correctedInvoices.ToDictionary(inv => inv.InvoiceNo, inv => inv);

            logger.Information("🔄 **SYNC_DATA_START**: Synchronizing {CorrectedCount} corrected ShipmentInvoice objects back into a list of {DynamicCount} dynamic data items.", correctedInvoices.Count, dynamicItems.Count);

            // ====== FREE SHIPPING SYNC DIAGNOSTIC ENTRY ======
            var freeShippingInvoices = correctedInvoices.Where(inv => inv.TotalDeduction.HasValue && inv.TotalDeduction.Value > 0).ToList();
            if (freeShippingInvoices.Any())
            {
                logger.Information("🚢 **FREE_SHIPPING_SYNC_START**: Found {Count} corrected invoices with TotalDeduction values to sync", freeShippingInvoices.Count);
                foreach (var inv in freeShippingInvoices)
                {
                    logger.Information("   - 📊 Invoice {InvoiceNo}: TotalDeduction={TotalDeduction} (corrected value to sync)", 
                        inv.InvoiceNo, inv.TotalDeduction);
                }
            }

            // ====== GIFT CARD SYNC DIAGNOSTIC ENTRY ======
            var giftCardInvoices = correctedInvoices.Where(inv => inv.TotalInsurance.HasValue && inv.TotalInsurance.Value != 0).ToList();
            if (giftCardInvoices.Any())
            {
                logger.Information("💳 **GIFT_CARD_SYNC_START**: Found {Count} corrected invoices with TotalInsurance values to sync", giftCardInvoices.Count);
                foreach (var inv in giftCardInvoices)
                {
                    logger.Information("   - 📊 Invoice {InvoiceNo}: TotalInsurance={TotalInsurance} (corrected value to sync)", 
                        inv.InvoiceNo, inv.TotalInsurance);
                }
            }
            else
            {
                logger.Warning("💳 **GIFT_CARD_SYNC_MISSING**: NO corrected invoices with TotalInsurance values found");
                logger.Information("   - 🔍 All corrected invoice TotalInsurance values: {Values}", 
                    string.Join(", ", correctedInvoices.Select(inv => $"{inv.InvoiceNo}={inv.TotalInsurance?.ToString() ?? "null"}")));
            }

            foreach (var dynamicItem in dynamicItems)
            {
                if (dynamicItem.TryGetValue("InvoiceNo", out var invNoObj) && invNoObj != null)
                {
                    if (correctedInvoiceMap.TryGetValue(invNoObj.ToString(), out var correctedInv))
                    {
                        logger.Information("   -  syncing InvoiceNo: {InvoiceNo}", correctedInv.InvoiceNo);

                        // ====== FREE SHIPPING DYNAMIC SYNC DIAGNOSTIC ======
                        if (correctedInv.TotalDeduction.HasValue && correctedInv.TotalDeduction.Value > 0)
                        {
                            var currentDynamicDeduction = dynamicItem.ContainsKey("TotalDeduction") ? dynamicItem["TotalDeduction"] : null;
                            logger.Information("🚢 **FREE_SHIPPING_DYNAMIC_SYNC**: About to sync TotalDeduction. DynamicItem current='{Current}', CorrectedInvoice value='{Corrected}'",
                                currentDynamicDeduction?.ToString() ?? "null", correctedInv.TotalDeduction);
                        }

                        // ====== GIFT CARD DYNAMIC SYNC DIAGNOSTIC ======
                        if (correctedInv.TotalInsurance.HasValue && correctedInv.TotalInsurance.Value != 0)
                        {
                            var currentDynamicInsurance = dynamicItem.ContainsKey("TotalInsurance") ? dynamicItem["TotalInsurance"] : null;
                            logger.Information("💳 **GIFT_CARD_DYNAMIC_SYNC**: About to sync TotalInsurance. DynamicItem current='{Current}', CorrectedInvoice value='{Corrected}'",
                                currentDynamicInsurance?.ToString() ?? "null", correctedInv.TotalInsurance);
                        }
                        else
                        {
                            logger.Warning("💳 **GIFT_CARD_DYNAMIC_SYNC_MISSING**: CorrectedInvoice has no TotalInsurance value to sync. Value={Value}",
                                correctedInv.TotalInsurance?.ToString() ?? "null");
                        }

                        Action<string, object> logChange = (field, newVal) =>
                        {
                            object oldVal = dynamicItem.ContainsKey(field) ? dynamicItem[field] : null;

                            if (!object.Equals(oldVal, newVal))
                            {
                                logger.Information("     - 📝 Field '{Field}': OLD='{Old}' -> NEW='{New}'",
                                    field,
                                    oldVal?.ToString() ?? "null",
                                    newVal?.ToString() ?? "null");

                                // ====== FREE SHIPPING FIELD SYNC SPECIFIC LOGGING ======
                                if (field == "TotalDeduction")
                                {
                                    logger.Information("🚢 **FREE_SHIPPING_FIELD_SYNC**: TotalDeduction field updated in dynamic item. OLD='{Old}' -> NEW='{New}'",
                                        oldVal?.ToString() ?? "null", newVal?.ToString() ?? "null");
                                }

                                // ====== GIFT CARD FIELD SYNC SPECIFIC LOGGING ======
                                if (field == "TotalInsurance")
                                {
                                    logger.Information("💳 **GIFT_CARD_FIELD_SYNC**: TotalInsurance field updated in dynamic item. OLD='{Old}' -> NEW='{New}'",
                                        oldVal?.ToString() ?? "null", newVal?.ToString() ?? "null");
                                }
                            }
                            else if (field == "TotalDeduction")
                            {
                                logger.Information("🚢 **FREE_SHIPPING_FIELD_SYNC_UNCHANGED**: TotalDeduction field already matches. Value='{Value}'", newVal?.ToString() ?? "null");
                            }
                            else if (field == "TotalInsurance")
                            {
                                logger.Information("💳 **GIFT_CARD_FIELD_SYNC_UNCHANGED**: TotalInsurance field already matches. Value='{Value}'", newVal?.ToString() ?? "null");
                            }
                        };

                        logChange("InvoiceTotal", correctedInv.InvoiceTotal);
                        dynamicItem["InvoiceTotal"] = correctedInv.InvoiceTotal;
                        logChange("SubTotal", correctedInv.SubTotal);
                        dynamicItem["SubTotal"] = correctedInv.SubTotal;
                        logChange("TotalInternalFreight", correctedInv.TotalInternalFreight);
                        dynamicItem["TotalInternalFreight"] = correctedInv.TotalInternalFreight;
                        logChange("TotalOtherCost", correctedInv.TotalOtherCost);
                        dynamicItem["TotalOtherCost"] = correctedInv.TotalOtherCost;
                        logChange("TotalInsurance", correctedInv.TotalInsurance);
                        dynamicItem["TotalInsurance"] = correctedInv.TotalInsurance;
                        logChange("TotalDeduction", correctedInv.TotalDeduction);
                        dynamicItem["TotalDeduction"] = correctedInv.TotalDeduction;

                        // ====== FREE SHIPPING POST-SYNC VERIFICATION ======
                        if (correctedInv.TotalDeduction.HasValue && correctedInv.TotalDeduction.Value > 0)
                        {
                            var finalDynamicDeduction = dynamicItem.ContainsKey("TotalDeduction") ? dynamicItem["TotalDeduction"] : null;
                            logger.Information("🚢 **FREE_SHIPPING_POST_SYNC**: TotalDeduction sync complete. Final dynamic item value='{Final}'", 
                                finalDynamicDeduction?.ToString() ?? "null");
                        }

                        // ====== GIFT CARD POST-SYNC VERIFICATION ======
                        if (correctedInv.TotalInsurance.HasValue && correctedInv.TotalInsurance.Value != 0)
                        {
                            var finalDynamicInsurance = dynamicItem.ContainsKey("TotalInsurance") ? dynamicItem["TotalInsurance"] : null;
                            logger.Information("💳 **GIFT_CARD_POST_SYNC**: TotalInsurance sync complete. Final dynamic item value='{Final}'", 
                                finalDynamicInsurance?.ToString() ?? "null");
                        }
                        else
                        {
                            var finalDynamicInsurance = dynamicItem.ContainsKey("TotalInsurance") ? dynamicItem["TotalInsurance"] : null;
                            logger.Warning("💳 **GIFT_CARD_POST_SYNC_MISSING**: No TotalInsurance value to sync. CorrectedInvoice={Corrected}, FinalDynamic='{Final}'",
                                correctedInv.TotalInsurance?.ToString() ?? "null", finalDynamicInsurance?.ToString() ?? "null");
                        }
                    }
                }
            }

            // ====== FREE SHIPPING SYNC COMPLETION SUMMARY ======
            var syncedFreeShippingCount = dynamicItems.Count(item => 
                item.ContainsKey("TotalDeduction") && 
                item["TotalDeduction"] != null && 
                double.TryParse(item["TotalDeduction"].ToString(), out double val) && val > 0);

            // ====== GIFT CARD SYNC COMPLETION SUMMARY ======
            var syncedGiftCardCount = dynamicItems.Count(item => 
                item.ContainsKey("TotalInsurance") && 
                item["TotalInsurance"] != null && 
                double.TryParse(item["TotalInsurance"].ToString(), out double val) && val != 0);
                
            logger.Information("✅ **SYNC_DATA_COMPLETE**: Finished synchronizing corrected data. The dynamic data list is now updated.");
            if (syncedFreeShippingCount > 0)
            {
                logger.Information("🚢 **FREE_SHIPPING_SYNC_SUMMARY**: {Count} dynamic items now have TotalDeduction values > 0", syncedFreeShippingCount);
            }
            if (syncedGiftCardCount > 0)
            {
                logger.Information("💳 **GIFT_CARD_SYNC_SUMMARY**: {Count} dynamic items now have TotalInsurance values ≠ 0", syncedGiftCardCount);
            }
            else
            {
                logger.Warning("💳 **GIFT_CARD_SYNC_SUMMARY**: NO dynamic items have TotalInsurance values ≠ 0");
                logger.Information("   - 🔍 This may be the source of the 6.99 imbalance - Gift Card not synced to dynamic data");
            }
        }

        public static string GetOriginalTextFromFile(string templateFilePath, ILogger logger)
        {
            if (string.IsNullOrEmpty(templateFilePath)) return "";
            try
            {
                var txtFile = templateFilePath + ".txt";
                if (File.Exists(txtFile)) return File.ReadAllText(txtFile);
                var txtFileOld = Path.ChangeExtension(templateFilePath, ".txt");
                if (File.Exists(txtFileOld)) return File.ReadAllText(txtFileOld);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error reading original text file: {FilePath}", templateFilePath);
            }
            return "";
        }

        public static ShipmentInvoice ConvertDynamicToShipmentInvoice(IDictionary<string, object> dict, ILogger logger = null)
        {
            return CreateTempShipmentInvoice(dict, logger);
        }

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails>() };
            try
            {
                invoice.InvoiceNo = (x.TryGetValue("InvoiceNo", out var v) ? v?.ToString() : null) ?? (x.TryGetValue("Name", out var n) ? n?.ToString() : null) ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceTotal = GetNullableDouble(x, "InvoiceTotal", logger);
                invoice.SubTotal = GetNullableDouble(x, "SubTotal", logger);
                invoice.TotalInternalFreight = GetNullableDouble(x, "TotalInternalFreight", logger);
                invoice.TotalOtherCost = GetNullableDouble(x, "TotalOtherCost", logger);
                invoice.TotalInsurance = GetNullableDouble(x, "TotalInsurance", logger);
                invoice.TotalDeduction = GetNullableDouble(x, "TotalDeduction", logger);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "🚨 **CONVERSION_EXCEPTION**: Error creating temporary ShipmentInvoice.");
                return null;
            }
            return invoice;
        }

        private static double? GetNullableDouble(IDictionary<string, object> x, string key, ILogger logger)
        {
            if (!x.TryGetValue(key, out var val) || val == null) return null;
            var valStr = val.ToString();
            if (string.IsNullOrWhiteSpace(valStr)) return null;

            string cleanedValue = Regex.Replace(valStr.Trim(), @"[^\d.,-]", "").Trim();
            if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
            {
                cleanedValue = cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.')
                                    ? cleanedValue.Replace(",", "")
                                    : cleanedValue.Replace(".", "").Replace(",", ".");
            }
            else if (cleanedValue.Contains(',')) cleanedValue = cleanedValue.Replace(",", ".");

            if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
            {
                return dbl;
            }
            return null;
        }

        public static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<IDictionary<string, object>> res,
            Template template, OCRCorrectionService serviceInstance, ILogger logger)
        {
            var allInvoices = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null || res == null) return allInvoices;
            var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);

            foreach (var item in res)
            {
                var shipmentInvoice = CreateTempShipmentInvoice(item, logger);
                if (shipmentInvoice != null)
                {
                    var metadata = serviceInstance.ExtractEnhancedOCRMetadata(item, template, fieldMappings);
                    allInvoices.Add(new ShipmentInvoiceWithMetadata { Invoice = shipmentInvoice, FieldMetadata = metadata });
                }
            }
            return allInvoices;
        }

        public static void UpdateTemplateLineValues(
            Template template,
            List<ShipmentInvoice> correctedInvoices,
            ILogger log)
        {
            log.Information("UpdateTemplateLineValues: This method is a placeholder and has no effect in this version.");
        }

        #endregion
    }
}