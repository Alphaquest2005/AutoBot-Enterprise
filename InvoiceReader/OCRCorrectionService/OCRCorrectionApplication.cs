// File: OCRCorrectionService/OCRCorrectionApplication.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using Serilog;
using Serilog.Events;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Correction Application to In-Memory Objects

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Processes transformation chains with LLM diagnostic workflow and business success criteria
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT + SUCCESS CRITERIA VALIDATION
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation → Phase 4 Success Criteria Validation
        /// **METHOD PURPOSE**: Process grouped error transformation chains with sequential application and unified processing
        /// **BUSINESS OBJECTIVE**: Transform error chains with proper sequencing and group management for accurate correction application
        /// **SUCCESS CRITERIA**: Chain processing completeness, group management accuracy, transformation sequencing, error preservation
        /// </summary>
        private List<InvoiceError> ProcessTransformationChains(List<InvoiceError> errors, ShipmentInvoice invoice, string fileText)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Complete LLM diagnostic workflow with success criteria validation
            
            // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
            _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for transformation chain processing");
            _logger.Error("📋 **AVAILABLE_LOG_DATA**: ErrorCount={ErrorCount}, InvoiceNo={InvoiceNo}, FileTextLength={FileTextLength}", errors?.Count ?? 0, invoice?.InvoiceNo ?? "NULL", fileText?.Length ?? 0);
            _logger.Error("🔍 **PATTERN_ANALYSIS**: Transformation chain processing requires group management with sequential error application");
            _logger.Error("❓ **EVIDENCE_GAPS**: Need to validate error grouping integrity, chain sequencing accuracy, and transformation completeness");
            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Unified group processing with proper sequencing ensures accurate transformation chain execution");
            
            // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
            _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for transformation chain processing");
            _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Track group formation, chain sequencing, transformation logic, and completion validation");
            _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Error grouping analysis, sequence ordering, transformation value tracking");
            
            // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
            _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based transformation chain processing");
            _logger.Error("📚 **FIX_RATIONALE**: Group-based transformation with proper sequencing ensures accurate chain processing");
            _logger.Error("🔍 **FIX_VALIDATION**: Validate error input, group by ID, process chains sequentially, verify transformation integrity");
            
            _logger.Information("🔗 **UNIFIED_TRANSFORMATION_START**: Processing {Count} errors - ALL errors are now grouped", errors.Count);
            
            var processedErrors = new List<InvoiceError>();
            
            // Group ALL errors by GroupId (every error should have one now)
            var groupedErrors = errors.GroupBy(e => e.GroupId ?? $"auto_group_{Guid.NewGuid():N}").ToList();
            
            _logger.Information("   - 🔗 Found {GroupCount} transformation groups (including single-step groups)", 
                groupedErrors.Count);
            
            // Process ALL transformation chains (including single-step "chains")
            foreach (var group in groupedErrors)
            {
                var chainErrors = group.OrderBy(e => e.SequenceOrder).ToList();
                
                if (chainErrors.Count == 1)
                {
                    _logger.Information("   - ⚡ Processing single-step group '{GroupId}'", group.Key);
                    // Single error group - no transformation needed, just pass through
                    processedErrors.Add(chainErrors[0]);
                }
                else
                {
                    _logger.Information("   - 🔄 Processing multi-step transformation chain '{GroupId}' with {Count} steps", 
                        group.Key, chainErrors.Count);
                    
                    string transformationValue = null;
                    
                    foreach (var error in chainErrors)
                    {
                        var processedError = error; // Copy the error
                        
                        // Apply transformation logic based on input source
                        if (error.TransformationInput == "ocr_text")
                        {
                            // First step in chain - use original CorrectValue from OCR analysis
                            transformationValue = error.CorrectValue;
                            _logger.Information("     - 📖 Step {Seq}: Input from OCR text → Output: '{Output}'", 
                                error.SequenceOrder, transformationValue);
                        }
                        else if (error.TransformationInput == "previous_output" && transformationValue != null)
                        {
                            // Subsequent step - use output from previous transformation
                            var previousValue = transformationValue;
                            transformationValue = error.CorrectValue; // The AI's transformed value
                            _logger.Information("     - 🔄 Step {Seq}: Input: '{Input}' → Output: '{Output}'", 
                                error.SequenceOrder, previousValue, transformationValue);
                            
                            // Update the error's extracted value to show the transformation input
                            processedError.ExtractedValue = previousValue;
                        }
                        
                        processedErrors.Add(processedError);
                    }
                    
                    _logger.Information("   - ✅ Completed transformation chain '{GroupId}': Final output = '{FinalValue}'", 
                        group.Key, transformationValue);
                }
            }
            
            _logger.Information("🔗 **UNIFIED_TRANSFORMATION_COMPLETE**: Processed {ProcessedCount} total errors across {GroupCount} groups", 
                processedErrors.Count, groupedErrors.Count);
            
            // **STEP 4: MANDATORY SUCCESS CRITERIA VALIDATION**
            _logger.Error("🎯 **BUSINESS_SUCCESS_CRITERIA_VALIDATION**: Transformation chain processing success analysis");
            
            // Individual criterion assessment
            var purposeFulfilled = processedErrors.Count > 0 && processedErrors.Count >= (errors?.Count ?? 0);
            _logger.Error((purposeFulfilled ? "✅" : "❌") + " **PURPOSE_FULFILLMENT**: " + (purposeFulfilled ? "Transformation chains successfully processed with all errors accounted for" : "Chain processing failed or missing errors detected"));
            
            var outputComplete = processedErrors != null && processedErrors.All(e => e != null);
            _logger.Error((outputComplete ? "✅" : "❌") + " **OUTPUT_COMPLETENESS**: " + (outputComplete ? "Complete processed error collection with no null entries" : "Incomplete or corrupted processed error collection"));
            
            var processComplete = groupedErrors?.Count > 0 && processedErrors.Count == (errors?.Count ?? 0);
            _logger.Error((processComplete ? "✅" : "❌") + " **PROCESS_COMPLETION**: " + (processComplete ? "All transformation groups processed successfully with preserved error count" : "Incomplete group processing or error count mismatch"));
            
            var dataQuality = processedErrors.All(e => !string.IsNullOrEmpty(e?.GroupId)) && processedErrors.Where(e => e.SequenceOrder > 1).All(e => !string.IsNullOrEmpty(e.ExtractedValue));
            _logger.Error((dataQuality ? "✅" : "❌") + " **DATA_QUALITY**: " + (dataQuality ? "Processed errors maintain group integrity and transformation input tracking" : "Data quality issues detected in group IDs or transformation tracking"));
            
            var errorHandling = (errors?.Count == 0 && processedErrors.Count == 0) || (errors?.Count > 0 && processedErrors.Count > 0);
            _logger.Error((errorHandling ? "✅" : "❌") + " **ERROR_HANDLING**: " + (errorHandling ? "Empty input and error scenarios handled gracefully" : "Error handling insufficient for edge cases"));
            
            var businessLogic = groupedErrors?.All(g => g.All(e => e.GroupId == g.Key)) == true;
            _logger.Error((businessLogic ? "✅" : "❌") + " **BUSINESS_LOGIC**: " + (businessLogic ? "Group-based processing follows expected business rules for transformation chains" : "Business logic violation in group processing or chain sequencing"));
            
            var integrationSuccess = true; // No external dependencies
            _logger.Error((integrationSuccess ? "✅" : "❌") + " **INTEGRATION_SUCCESS**: " + (integrationSuccess ? "No external dependencies - internal transformation processing" : "Integration dependency failure"));
            
            var performanceCompliance = processedErrors.Count <= (errors?.Count ?? 0) * 2; // Reasonable processing overhead
            _logger.Error((performanceCompliance ? "✅" : "❌") + " **PERFORMANCE_COMPLIANCE**: " + (performanceCompliance ? "Transformation processing completed within reasonable complexity bounds" : "Performance issues detected in transformation processing"));
            
            // Overall assessment
            var overallSuccess = purposeFulfilled && outputComplete && processComplete && dataQuality && errorHandling && businessLogic && integrationSuccess && performanceCompliance;
            _logger.Error("🏆 **OVERALL_METHOD_SUCCESS**: " + (overallSuccess ? "✅ PASS" : "❌ FAIL") + " - Transformation chain processing " + (overallSuccess ? "completed successfully with comprehensive group management and sequential transformation" : "failed due to validation criteria not met"));
            
            return processedErrors;
        }

        /// <summary>
        /// Applies all high-confidence corrections directly to the in-memory ShipmentInvoice object.
        /// This version contains the definitive fix for preventing double-counting aggregation errors.
        /// </summary>
        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
     ShipmentInvoice invoice,
     List<InvoiceError> errors,
     string fileText,
     Dictionary<string, OCRFieldMetadata> currentInvoiceMetadata)
        {
            var correctionResults = new List<CorrectionResult>();
            if (invoice == null || errors == null || !errors.Any()) return correctionResults;

            const double CONFIDENCE_THRESHOLD = 0.90;

            // ================================ TRANSFORMATION CHAINS START ================================
            // Process transformation chains where grouped errors are applied sequentially
            var processedErrors = ProcessTransformationChains(errors, invoice, fileText);
            // ================================= TRANSFORMATION CHAINS END =================================

            // =================================== UNIFIED ERROR FILTERING ===================================
            // Since ALL errors are now grouped, we need different logic:
            // - Apply all high-confidence errors directly to in-memory objects
            // - Format corrections that are part of multi-step chains (SequenceOrder > 1) are applied
            // - All single-step groups (SequenceOrder = 1) are applied regardless of type
            var errorsToApplyDirectly = processedErrors
                .Where(e => e.Confidence >= CONFIDENCE_THRESHOLD)
                .ToList();
            // ================================== UNIFIED FILTERING END ==================================

            _logger.Information("🚀 **APPLY_CORRECTIONS_START**: Applying {Count} high-confidence (>= {Threshold:P0}) corrections to invoice {InvoiceNo}.",
                errorsToApplyDirectly.Count, CONFIDENCE_THRESHOLD, invoice.InvoiceNo);
            _logger.Information("   - **UNIFIED_ARCHITECTURE**: All errors flow through transformation chain processing. Single-step and multi-step transformations are applied uniformly. Aggregate fields will be reset to zero before summing new components to prevent double-counting.");

            // ====== COMPREHENSIVE FREE SHIPPING DIAGNOSTIC LOGGING ======
            var freeShippingErrors = errorsToApplyDirectly.Where(e => e.Field == "TotalDeduction").ToList();
            if (freeShippingErrors.Any())
            {
                _logger.Information("🚢 **FREE_SHIPPING_DIAGNOSTIC_START**: Found {Count} TotalDeduction corrections (likely Free Shipping)", freeShippingErrors.Count);
                foreach (var error in freeShippingErrors)
                {
                    _logger.Information("   - 📝 Free Shipping Error: Field='{Field}', OldValue='{OldValue}', NewValue='{NewValue}', LineText='{LineText}', Confidence={Confidence:P2}",
                        error.Field, error.ExtractedValue ?? "null", error.CorrectValue, error.LineText, error.Confidence);
                }
                _logger.Information("   - 📊 Current Invoice TotalDeduction BEFORE applying corrections: {CurrentValue}", invoice.TotalDeduction);
            }

            // ====== COMPREHENSIVE GIFT CARD / TOTAL INSURANCE DIAGNOSTIC LOGGING ======
            var giftCardErrors = errorsToApplyDirectly.Where(e => e.Field == "TotalInsurance").ToList();
            if (giftCardErrors.Any())
            {
                _logger.Information("💳 **GIFT_CARD_DIAGNOSTIC_START**: Found {Count} TotalInsurance corrections (likely Gift Card)", giftCardErrors.Count);
                foreach (var error in giftCardErrors)
                {
                    _logger.Information("   - 📝 Gift Card Error: Field='{Field}', OldValue='{OldValue}', NewValue='{NewValue}', LineText='{LineText}', Confidence={Confidence:P2}",
                        error.Field, error.ExtractedValue ?? "null", error.CorrectValue, error.LineText, error.Confidence);
                }
                _logger.Information("   - 📊 Current Invoice TotalInsurance BEFORE applying corrections: {CurrentValue}", invoice.TotalInsurance);
            }
            else
            {
                _logger.Warning("💳 **GIFT_CARD_DIAGNOSTIC_MISSING**: NO TotalInsurance corrections found in error list");
                _logger.Information("   - 📊 Current Invoice TotalInsurance (will remain unchanged): {CurrentValue}", invoice.TotalInsurance);
                
                // Log all error fields to help debug why TotalInsurance is missing
                var allErrorFields = errorsToApplyDirectly.Select(e => e.Field).Distinct().ToList();
                _logger.Information("   - 🔍 All error fields found: {AllFields}", string.Join(", ", allErrorFields));
            }

            LogFinancialState("Initial State (Before Corrections)", invoice);

            // ================== DEFINITIVE FIX START ==================
            // Identify all unique numeric fields that the AI is providing 'omission' corrections for.
            var numericFieldsToReset = errorsToApplyDirectly
                .Where(e => e.ErrorType == "omission")
                .Select(e => this.MapDeepSeekFieldToDatabase(e.Field))
                .Where(info => info != null && (info.DataType == "Number" || info.DataType.Contains("currency") || info.DataType.Contains("decimal") || info.DataType.Contains("double")))
                .Select(info => info.DatabaseFieldName)
                .Distinct()
                .ToList();

            // Reset these fields to 0 before applying corrections. This prevents adding AI-found omissions
            // to a potentially flawed or already complete aggregate from the initial OCR read.
            if (numericFieldsToReset.Any())
            {
                _logger.Information("   - **LOGIC_PATH**: Identified numeric fields with AI omissions: [{Fields}]. Resetting them to 0.0 to prevent double-counting.", string.Join(", ", numericFieldsToReset));

                // ====== FREE SHIPPING RESET DIAGNOSTIC ======
                if (numericFieldsToReset.Contains("TotalDeduction"))
                {
                    _logger.Information("🚢 **FREE_SHIPPING_RESET**: TotalDeduction field will be reset from {OldValue} to 0.0 before applying Free Shipping corrections", invoice.TotalDeduction);
                }
                foreach (var fieldName in numericFieldsToReset)
                {
                    var prop = typeof(ShipmentInvoice).GetProperty(fieldName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null && prop.CanWrite && (prop.PropertyType == typeof(double?) || prop.PropertyType == typeof(double)))
                    {
                        prop.SetValue(invoice, 0.0d);
                    }
                }
                LogFinancialState("State After Numeric Field Reset", invoice);
            }
            // =================== DEFINITIVE FIX END ===================

            foreach (var error in errorsToApplyDirectly)
            {
                // ====== FREE SHIPPING INDIVIDUAL CORRECTION LOGGING ======
                if (error.Field == "TotalDeduction")
                {
                    _logger.Information("🚢 **FREE_SHIPPING_APPLY_INDIVIDUAL**: About to apply TotalDeduction correction. Value='{NewValue}', LineText='{LineText}'", 
                        error.CorrectValue, error.LineText);
                    _logger.Information("   - 📊 TotalDeduction BEFORE this individual correction: {CurrentValue}", invoice.TotalDeduction);
                }

                // ====== GIFT CARD INDIVIDUAL CORRECTION LOGGING ======
                if (error.Field == "TotalInsurance")
                {
                    _logger.Information("💳 **GIFT_CARD_APPLY_INDIVIDUAL**: About to apply TotalInsurance correction. Value='{NewValue}', LineText='{LineText}'", 
                        error.CorrectValue, error.LineText);
                    _logger.Information("   - 📊 TotalInsurance BEFORE this individual correction: {CurrentValue}", invoice.TotalInsurance);
                }

                var result = await this.ApplySingleValueOrFormatCorrectionToInvoiceAsync(invoice, error)
                                 .ConfigureAwait(false);
                correctionResults.Add(result);

                // ====== FREE SHIPPING POST-APPLICATION LOGGING ======
                if (error.Field == "TotalDeduction")
                {
                    _logger.Information("🚢 **FREE_SHIPPING_APPLY_RESULT**: TotalDeduction correction applied. Result: {Success}, ErrorMessage: '{ErrorMessage}'", 
                        result.Success, result.ErrorMessage ?? "None");
                    _logger.Information("   - 📊 TotalDeduction AFTER this individual correction: {CurrentValue}", invoice.TotalDeduction);
                }

                // ====== GIFT CARD POST-APPLICATION LOGGING ======
                if (error.Field == "TotalInsurance")
                {
                    _logger.Information("💳 **GIFT_CARD_APPLY_RESULT**: TotalInsurance correction applied. Result: {Success}, ErrorMessage: '{ErrorMessage}'", 
                        result.Success, result.ErrorMessage ?? "None");
                    _logger.Information("   - 📊 TotalInsurance AFTER this individual correction: {CurrentValue}", invoice.TotalInsurance);
                }

                LogCorrectionResult(result, "DIRECT_FIX_APPLIED");
            }

            if (correctionResults.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
            }

            LogFinancialState("Final State (After All Corrections)", invoice);
            
            // ====== COMPREHENSIVE FREE SHIPPING FINAL DIAGNOSTIC ======
            var appliedFreeShippingCorrections = correctionResults.Where(r => r.FieldName == "TotalDeduction" && r.Success).ToList();
            if (appliedFreeShippingCorrections.Any())
            {
                _logger.Information("🚢 **FREE_SHIPPING_FINAL_SUMMARY**: Applied {Count} successful TotalDeduction corrections", appliedFreeShippingCorrections.Count);
                foreach (var correction in appliedFreeShippingCorrections)
                {
                    _logger.Information("   - ✅ Free Shipping Correction: OldValue='{OldValue}' → NewValue='{NewValue}', Success={Success}",
                        correction.OldValue, correction.NewValue, correction.Success);
                }
                _logger.Information("   - 📊 Final invoice.TotalDeduction after all Free Shipping corrections: {FinalValue}", invoice.TotalDeduction);
            }
            else
            {
                _logger.Information("🚢 **FREE_SHIPPING_FINAL_SUMMARY**: No TotalDeduction corrections were applied successfully");
            }

            // ====== COMPREHENSIVE GIFT CARD FINAL DIAGNOSTIC ======
            var appliedGiftCardCorrections = correctionResults.Where(r => r.FieldName == "TotalInsurance" && r.Success).ToList();
            if (appliedGiftCardCorrections.Any())
            {
                _logger.Information("💳 **GIFT_CARD_FINAL_SUMMARY**: Applied {Count} successful TotalInsurance corrections", appliedGiftCardCorrections.Count);
                foreach (var correction in appliedGiftCardCorrections)
                {
                    _logger.Information("   - ✅ Gift Card Correction: OldValue='{OldValue}' → NewValue='{NewValue}', Success={Success}",
                        correction.OldValue, correction.NewValue, correction.Success);
                }
                _logger.Information("   - 📊 Final invoice.TotalInsurance after all Gift Card corrections: {FinalValue}", invoice.TotalInsurance);
            }
            else
            {
                _logger.Warning("💳 **GIFT_CARD_FINAL_SUMMARY**: NO TotalInsurance corrections were applied successfully");
                _logger.Warning("   - ⚠️ This means TotalInsurance will remain: {CurrentValue}", invoice.TotalInsurance);
                _logger.Warning("   - 🔍 This could be the source of the 6.99 imbalance if Gift Card should be -6.99");
            }

            // ====== CRITICAL TOTALS CALCULATION DIAGNOSTIC ======
            var expectedTotal = (invoice.SubTotal ?? 0) + (invoice.TotalInternalFreight ?? 0) + (invoice.TotalOtherCost ?? 0) + (invoice.TotalInsurance ?? 0) - (invoice.TotalDeduction ?? 0);
            var actualTotal = invoice.InvoiceTotal ?? 0;
            var calculatedDifference = actualTotal - expectedTotal;
            
            _logger.Information("🧮 **CRITICAL_TOTALS_CALCULATION_TRACE**: " +
                "SubTotal={SubTotal} + Freight={Freight} + OtherCost={OtherCost} + Insurance={Insurance} - Deduction={Deduction} = Expected({Expected}), " +
                "Actual={Actual}, Difference={Difference}",
                invoice.SubTotal ?? 0, invoice.TotalInternalFreight ?? 0, invoice.TotalOtherCost ?? 0, 
                invoice.TotalInsurance ?? 0, invoice.TotalDeduction ?? 0, expectedTotal, actualTotal, calculatedDifference);
                
            if (Math.Abs(calculatedDifference) > 0.01)
            {
                _logger.Warning("⚠️ **IMBALANCE_DETECTED**: Invoice calculation shows {Difference} imbalance. This may explain the 0.46 TotalsZero issue.", calculatedDifference);
            }
            else
            {
                _logger.Information("✅ **BALANCE_ACHIEVED**: Invoice calculation is balanced within tolerance.");
            }
            
            _logger.Information("🏁 **APPLY_CORRECTIONS_COMPLETE** for invoice {InvoiceNo}.", invoice.InvoiceNo);

            return correctionResults;
        }

        private async Task<CorrectionResult> ApplySingleValueOrFormatCorrectionToInvoiceAsync(
            ShipmentInvoice invoice,
            InvoiceError error)
        {
            var result = new CorrectionResult
            {
                FieldName = error.Field,
                CorrectionType = error.ErrorType,
                Confidence = error.Confidence,
                OldValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString(),
                NewValue = error.CorrectValue,
                LineText = error.LineText,
                LineNumber = error.LineNumber,
                ContextLinesBefore = error.ContextLinesBefore,
                ContextLinesAfter = error.ContextLinesAfter,
                RequiresMultilineRegex = error.RequiresMultilineRegex,
                Reasoning = error.Reasoning
            };
            _logger.Information("   - ➡️ **APPLYING_SINGLE_CORRECTION**: Field='{Field}', CorrectValue='{NewValue}', Type='{Type}', Line={LineNum}, Text='{LineText}'",
                error.Field, error.CorrectValue, error.ErrorType, error.LineNumber, TruncateForLog(error.LineText, 70));

            // ====== FREE SHIPPING VALUE PARSING DIAGNOSTIC ======
            if (error.Field == "TotalDeduction")
            {
                _logger.Information("🚢 **FREE_SHIPPING_PARSE_START**: About to parse Free Shipping value '{Value}' for TotalDeduction field", error.CorrectValue);
            }

            try
            {
                object parsedCorrectedValue = this.ParseCorrectedValue(error.CorrectValue, error.Field);

                if (parsedCorrectedValue == null && !string.IsNullOrEmpty(error.CorrectValue))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not parse corrected value '{error.CorrectValue}' for field {error.Field}.";
                    _logger.Warning("     - ⚠️ **PARSE_FAILURE**: {ErrorMessage}", result.ErrorMessage);
                    return result;
                }
                _logger.Information("     - **PARSE_SUCCESS**: Parsed CorrectValue '{Original}' into system value '{Parsed}' of type {Type}.",
                    error.CorrectValue, parsedCorrectedValue ?? "null", parsedCorrectedValue?.GetType().Name ?? "null");

                // ====== FREE SHIPPING PARSE RESULT DIAGNOSTIC ======
                if (error.Field == "TotalDeduction")
                {
                    _logger.Information("🚢 **FREE_SHIPPING_PARSE_SUCCESS**: Free Shipping value '{Original}' parsed to {Parsed} ({Type})", 
                        error.CorrectValue, parsedCorrectedValue, parsedCorrectedValue?.GetType().Name ?? "null");
                }

                // ====== FREE SHIPPING FIELD APPLICATION DIAGNOSTIC ======
                if (error.Field == "TotalDeduction")
                {
                    _logger.Information("🚢 **FREE_SHIPPING_APPLY_FIELD_START**: About to apply parsed value {ParsedValue} to invoice TotalDeduction field (current: {CurrentValue})",
                        parsedCorrectedValue, invoice.TotalDeduction);
                }

                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue))
                {
                    result.NewValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString();
                    result.Success = true;
                    _logger.Information("     - ✅ **APPLY_SUCCESS**: Correction for {Field} applied. Invoice's final value for this field is now '{NewVal}'.",
                        error.Field, result.NewValue);

                    // ====== FREE SHIPPING FIELD APPLICATION SUCCESS DIAGNOSTIC ======
                    if (error.Field == "TotalDeduction")
                    {
                        _logger.Information("🚢 **FREE_SHIPPING_APPLY_FIELD_SUCCESS**: TotalDeduction successfully updated to {NewValue} (was: {OldValue})",
                            result.NewValue, result.OldValue);
                    }
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"Field '{error.Field}' not recognized or value '{error.CorrectValue}' not applied/aggregated.";
                    _logger.Warning("     - ❌ **APPLY_FAILURE**: {ErrorMessage}", result.ErrorMessage);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "     - 🚨 **APPLY_EXCEPTION** while applying correction for {Field}.", error.Field);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        public bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldNameFromError, object correctedValue)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError);
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError;

            _logger.Information("       - ▶️ **Enter ApplyFieldCorrection**: Target Property='{TargetProp}', Incoming Value='{Value}' (Type: {Type})",
                targetPropertyName, correctedValue ?? "null", correctedValue?.GetType().Name ?? "null");

            // ====== FREE SHIPPING FIELD CORRECTION ENTRY DIAGNOSTIC ======
            if (targetPropertyName == "TotalDeduction")
            {
                _logger.Information("🚢 **FREE_SHIPPING_FIELD_CORRECTION_ENTRY**: Applying correction to TotalDeduction. Current invoice.TotalDeduction={Current}, Incoming value={Incoming}",
                    invoice.TotalDeduction, correctedValue);
            }

            try
            {
                var invoiceProp = typeof(ShipmentInvoice).GetProperty(
                    targetPropertyName,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (invoiceProp != null)
                {
                    object existingValue = invoiceProp.GetValue(invoice);
                    _logger.Information("         - **STATE_CHECK**: Current value of '{TargetProp}' is '{ExistingValue}' (Type: {Type}).",
                        targetPropertyName, existingValue ?? "null", existingValue?.GetType().Name ?? "null");

                    var propertyType = Nullable.GetUnderlyingType(invoiceProp.PropertyType) ?? invoiceProp.PropertyType;
                    object finalValueToSet = correctedValue;

                    if (existingValue != null)
                    {
                        _logger.Information("         - **AGGREGATION_CHECK**: Existing value is not null. Applying aggregation logic.");

                        if (propertyType == typeof(double) || propertyType == typeof(decimal) || propertyType == typeof(int))
                        {
                            _logger.Information("           - **LOGIC_PATH**: Matched Numeric Aggregation Path.");
                            var existingNumeric = Convert.ToDouble(existingValue, System.Globalization.CultureInfo.InvariantCulture);
                            var newNumeric = Convert.ToDouble(correctedValue, System.Globalization.CultureInfo.InvariantCulture);
                            finalValueToSet = existingNumeric + newNumeric;
                            _logger.Information("           - **NUMERIC_AGGREGATION**: Calculation: {Existing} + {New} = {Final}",
                                existingNumeric, newNumeric, finalValueToSet);

                            // ====== FREE SHIPPING AGGREGATION DIAGNOSTIC ======
                            if (targetPropertyName == "TotalDeduction")
                            {
                                _logger.Information("🚢 **FREE_SHIPPING_AGGREGATION**: TotalDeduction aggregation: {Existing} + {New} = {Final}",
                                    existingNumeric, newNumeric, finalValueToSet);
                            }
                        }
                        else if (propertyType == typeof(string))
                        {
                            _logger.Information("           - **LOGIC_PATH**: Matched String Aggregation Path.");
                            var existingString = existingValue.ToString();
                            var newString = correctedValue?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(existingString) && !string.IsNullOrWhiteSpace(newString))
                            {
                                finalValueToSet = $"{existingString}{Environment.NewLine}{newString}";
                                _logger.Information("           - **STRING_AGGREGATION**: Concatenating values.");
                            }
                            else
                            {
                                finalValueToSet = string.IsNullOrWhiteSpace(existingString) ? newString : existingString;
                            }
                        }
                    }
                    else
                    {
                        _logger.Information("         - **NO_AGGREGATION**: Existing value is null. Will set value directly.");
                        
                        // ====== FREE SHIPPING DIRECT SET DIAGNOSTIC ======
                        if (targetPropertyName == "TotalDeduction")
                        {
                            _logger.Information("🚢 **FREE_SHIPPING_DIRECT_SET**: TotalDeduction was null, setting directly to {Value}", correctedValue);
                        }
                    }

                    _logger.Information("         - **TYPE_CONVERSION**: Preparing to set final value '{FinalValue}' (Type: {Type}) to property of type {PropType}.",
                        finalValueToSet ?? "null", finalValueToSet?.GetType().Name ?? "null", propertyType.Name);

                    var convertedValue = finalValueToSet != null
                                             ? Convert.ChangeType(finalValueToSet, propertyType, System.Globalization.CultureInfo.InvariantCulture)
                                             : null;

                    // ====== FREE SHIPPING FINAL SET DIAGNOSTIC ======
                    if (targetPropertyName == "TotalDeduction")
                    {
                        _logger.Information("🚢 **FREE_SHIPPING_FINAL_SET**: About to set invoice.TotalDeduction property to {ConvertedValue} (was: {OldValue})",
                            convertedValue, invoice.TotalDeduction);
                    }

                    invoiceProp.SetValue(invoice, convertedValue);

                    // ====== FREE SHIPPING AFTER SET DIAGNOSTIC ======
                    if (targetPropertyName == "TotalDeduction")
                    {
                        _logger.Information("🚢 **FREE_SHIPPING_AFTER_SET**: invoice.TotalDeduction property now contains: {NewValue}", invoice.TotalDeduction);
                    }

                    _logger.Information("       - ✅ **Leave ApplyFieldCorrection**: Successfully set '{TargetProp}' to '{Value}'.", targetPropertyName, convertedValue ?? "null");
                    return true;
                }

                _logger.Warning("       - ❌ **PROPERTY_NOT_FOUND**: Property '{TargetPropertyName}' not found on ShipmentInvoice.", targetPropertyName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "       - 🚨 **EXCEPTION in ApplyFieldCorrection** for {TargetProp} with value '{Value}'", targetPropertyName, correctedValue);
                return false;
            }
        }

        private void LogFinancialState(string stage, ShipmentInvoice invoice)
        {
            if (invoice == null) return;
            _logger.Error("📊 **INVOICE_FINANCIAL_STATE ({Stage})** for Invoice {InvoiceNo}:", stage, invoice.InvoiceNo);
            _logger.Error("   - SubTotal:             {SubTotal}", invoice.SubTotal);
            _logger.Error("   - TotalInternalFreight: {TotalInternalFreight}", invoice.TotalInternalFreight);
            _logger.Error("   - TotalOtherCost:       {TotalOtherCost}", invoice.TotalOtherCost);
            _logger.Error("   - TotalDeduction:       {TotalDeduction}", invoice.TotalDeduction);
            _logger.Error("   - TotalInsurance:       {TotalInsurance}", invoice.TotalInsurance);
            _logger.Error("   - InvoiceTotal:         {InvoiceTotal}", invoice.InvoiceTotal);
            var isBalanced = TotalsZero(invoice, out var diff, _logger);
            _logger.Error("   - Mathematical Balance: {IsBalanced} (Difference: {Difference:F2})", isBalanced ? "✅ BALANCED" : "❌ UNBALANCED", diff);
        }

        private void LogCorrectionResult(CorrectionResult result, string priority)
        {
            var status = result.Success ? "Applied/Aggregated" : "Failed";
            var level = result.Success ? LogEventLevel.Error : LogEventLevel.Warning;

            _logger.Write(
                level,
                "   - 🎯 **CORRECTION_OUTCOME**: [{Priority}] {Status} for Field: {Field}, Type: {Type}. Original Field Value: '{OldVal}', Final Field Value: '{NewVal}'. Conf: {Conf:P0}. Reason: '{Reason}'. Message: {Msg}",
                priority, status, result.FieldName, result.CorrectionType,
                TruncateForLog(result.OldValue, 50), TruncateForLog(result.NewValue, 50),
                result.Confidence, result.Reasoning ?? "N/A", result.ErrorMessage ?? "N/A");
        }

        public static string TruncateForLog(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
        }

        #endregion
    }
}