// File: OCRCorrectionService/OCRCaribbeanCustomsProcessor.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using Serilog; // For logging

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Caribbean Customs Business Rules Processor

        /// <summary>
        /// Applies Caribbean/CARICOM customs business rules after basic OCR corrections.
        /// SIMPLIFIED: Transforms basic field mappings to Caribbean customs requirements.
        /// RULE: Customer credits (gift cards) → TotalInsurance (negative values)
        /// RULE: Supplier discounts (free shipping) → TotalDeduction (positive values)
        /// </summary>
        /// <param name="invoice">The invoice after basic OCR corrections</param>
        /// <param name="standardCorrections">The list of corrections applied by DeepSeek</param>
        /// <returns>List of additional corrections made for Caribbean customs compliance</returns>
        public List<CorrectionResult> ApplyCaribbeanCustomsRules(ShipmentInvoice invoice, List<CorrectionResult> standardCorrections)
        {
            var customsCorrections = new List<CorrectionResult>();
            
            if (invoice == null)
            {
                _logger.Warning("ApplyCaribbeanCustomsRules: Invoice is null, skipping customs rules processing");
                return customsCorrections;
            }

            _logger.Information("🏴‍☠️ **CARIBBEAN_SIMPLIFIED_START**: Applying simplified Caribbean customs rules to invoice {InvoiceNo}", invoice.InvoiceNo ?? "NULL");
            _logger.Information("🏴‍☠️ **CARIBBEAN_INPUT_STATE**: TotalInsurance={TotalInsurance}, TotalDeduction={TotalDeduction}", 
                invoice?.TotalInsurance, invoice?.TotalDeduction);

            try
            {
                // SIMPLIFIED RULE 1: Transform customer-owned value applications (gift cards → TotalInsurance negative)
                var customerValueCorrections = ProcessCustomerValueReductions(invoice, standardCorrections);
                customsCorrections.AddRange(customerValueCorrections);

                // SIMPLIFIED RULE 2: Ensure supplier reductions are positive in TotalDeduction
                var supplierReductionCorrections = ProcessSupplierReductions(invoice, standardCorrections);
                customsCorrections.AddRange(supplierReductionCorrections);

                _logger.Information("🏴‍☠️ **CARIBBEAN_SIMPLIFIED_COMPLETE**: Applied {CustomsRuleCount} Caribbean customs corrections", customsCorrections.Count);
                
                foreach (var correction in customsCorrections)
                {
                    _logger.Information("🏴‍☠️ **CARIBBEAN_CORRECTION**: {FieldName}: '{OldValue}' → '{NewValue}' | Reason: {Reasoning}", 
                        correction.FieldName, correction.OldValue, correction.NewValue, correction.Reasoning);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🏴‍☠️ **CARIBBEAN_CUSTOMS_ERROR**: Error applying Caribbean customs rules to invoice {InvoiceNo}", invoice.InvoiceNo ?? "NULL");
            }

            return customsCorrections;
        }

        /// <summary>
        /// Processes customer-owned value applications (gift cards, store credits) according to Caribbean customs rules.
        /// RULE: Customer credits should be in TotalInsurance field as NEGATIVE values.
        /// </summary>
        private List<CorrectionResult> ProcessCustomerValueReductions(ShipmentInvoice invoice, List<CorrectionResult> appliedCorrections)
        {
            var corrections = new List<CorrectionResult>();

            _logger.Information("🏴‍☠️ **CUSTOMER_VALUE_ANALYSIS**: Checking for customer-owned value applications (gift cards, store credits)");

            // Check if any corrections involved customer credits
            var customerCreditCorrections = appliedCorrections.Where(c => 
                c.Success && 
                (c.LineText?.IndexOf("Gift Card", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.LineText?.IndexOf("Store Credit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.LineText?.IndexOf("Account Credit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.LineText?.IndexOf("Points", StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            foreach (var customerCredit in customerCreditCorrections)
            {
                // Ensure customer credits are in TotalInsurance as negative values
                if (customerCredit.FieldName == "TotalInsurance")
                {
                    var negativeValue = EnsureNegativeValue(customerCredit.NewValue);
                    if (negativeValue != customerCredit.NewValue)
                    {
                        var ensureNegative = CreateCaribbeanTransformation(
                            fieldName: "TotalInsurance",
                            oldValue: customerCredit.NewValue,
                            newValue: negativeValue,
                            reasoning: "Caribbean customs rule: Customer credits must be negative values in TotalInsurance",
                            lineText: customerCredit.LineText,
                            lineNumber: customerCredit.LineNumber
                        );

                        corrections.Add(ensureNegative);
                        _logger.Information("🏴‍☠️ **CUSTOMER_CREDIT_SIGN_FIX**: Ensured customer credit is negative: {Value}", negativeValue);
                    }
                }
                // If DeepSeek put customer credit in wrong field, move it to TotalInsurance
                else if (customerCredit.FieldName == "TotalDeduction" && IsCustomerOwnedValue(customerCredit.LineText))
                {
                    var moveToInsurance = CreateCaribbeanTransformation(
                        fieldName: "TotalInsurance",
                        oldValue: invoice.TotalInsurance?.ToString() ?? "null",
                        newValue: EnsureNegativeValue(customerCredit.NewValue),
                        reasoning: "Caribbean customs rule: Customer-owned value moved from TotalDeduction to TotalInsurance as negative value",
                        lineText: customerCredit.LineText,
                        lineNumber: customerCredit.LineNumber
                    );

                    corrections.Add(moveToInsurance);
                    _logger.Information("🏴‍☠️ **CUSTOMER_CREDIT_MOVE**: Moved customer credit {Amount} from TotalDeduction to TotalInsurance (negative)", 
                        customerCredit.NewValue);
                }
            }

            return corrections;
        }

        /// <summary>
        /// Processes supplier-caused reductions (free shipping, promotional discounts) according to Caribbean customs rules.
        /// RULE: Supplier reductions should remain in TotalDeduction field as POSITIVE values.
        /// </summary>
        private List<CorrectionResult> ProcessSupplierReductions(ShipmentInvoice invoice, List<CorrectionResult> appliedCorrections)
        {
            var corrections = new List<CorrectionResult>();

            _logger.Information("🏴‍☠️ **SUPPLIER_REDUCTION_ANALYSIS**: Checking for supplier-caused reductions (free shipping, discounts)");

            var supplierReductions = appliedCorrections.Where(c => 
                c.Success && 
                (c.LineText?.IndexOf("Free Shipping", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.LineText?.IndexOf("Discount", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.LineText?.IndexOf("Promo", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 c.LineText?.IndexOf("Coupon", StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            foreach (var supplierReduction in supplierReductions)
            {
                // Ensure supplier reductions are in correct field with correct sign
                if (supplierReduction.FieldName == "TotalDeduction")
                {
                    // Ensure positive value for supplier reductions
                    var positiveValue = EnsurePositiveValue(supplierReduction.NewValue);
                    if (positiveValue != supplierReduction.NewValue)
                    {
                        var ensurePositive = CreateCaribbeanTransformation(
                            fieldName: "TotalDeduction",
                            oldValue: supplierReduction.NewValue,
                            newValue: positiveValue,
                            reasoning: "Caribbean customs rule: Supplier reductions must be positive values in TotalDeduction",
                            lineText: supplierReduction.LineText,
                            lineNumber: supplierReduction.LineNumber
                        );

                        corrections.Add(ensurePositive);
                        _logger.Information("🏴‍☠️ **SUPPLIER_REDUCTION_SIGN_FIX**: Ensured supplier reduction is positive: {Value}", positiveValue);
                    }
                }
            }

            return corrections;
        }

        /// <summary>
        /// Applies the Caribbean customs corrections to the in-memory invoice object.
        /// </summary>
        public void ApplyCaribbeanCustomsCorrectionsToInvoice(ShipmentInvoice invoice, List<CorrectionResult> customsCorrections)
        {
            _logger.Information("🏴‍☠️ **CARIBBEAN_APPLICATION_ENTRY**: Applying {Count} Caribbean customs corrections to invoice {InvoiceNo}", 
                customsCorrections.Count, invoice?.InvoiceNo);

            foreach (var correction in customsCorrections)
            {
                try
                {
                    ApplySingleCaribbeanCorrection(invoice, correction);
                    _logger.Information("🏴‍☠️ **CARIBBEAN_CORRECTION_APPLIED**: {FieldName} = {NewValue}", correction.FieldName, correction.NewValue);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "🚨 **CARIBBEAN_APPLICATION_ERROR**: Failed to apply correction {FieldName} = {NewValue}", 
                        correction.FieldName, correction.NewValue);
                }
            }

            _logger.Information("🏴‍☠️ **CARIBBEAN_APPLICATION_RESULT**: Final state - TotalInsurance={TotalInsurance}, TotalDeduction={TotalDeduction}", 
                invoice?.TotalInsurance, invoice?.TotalDeduction);
        }

        #endregion

        #region Caribbean Rules Helper Methods

        /// <summary>
        /// Determines if a line of text represents customer-owned value (gift cards, store credits).
        /// </summary>
        private bool IsCustomerOwnedValue(string lineText)
        {
            if (string.IsNullOrEmpty(lineText)) return false;

            var customerIndicators = new[] { "gift card", "store credit", "account credit", "points", "loyalty", "refund applied" };
            return customerIndicators.Any(indicator => lineText.IndexOf(indicator, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Ensures a value is negative (for customer credits in TotalInsurance).
        /// </summary>
        private string EnsureNegativeValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            
            if (double.TryParse(value, out var numericValue))
            {
                return (-Math.Abs(numericValue)).ToString("F2");
            }
            
            return value;
        }

        /// <summary>
        /// Ensures a value is positive (for supplier reductions in TotalDeduction).
        /// </summary>
        private string EnsurePositiveValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            
            if (double.TryParse(value, out var numericValue))
            {
                return Math.Abs(numericValue).ToString("F2");
            }
            
            return value;
        }

        /// <summary>
        /// Creates a CorrectionResult for Caribbean customs transformations.
        /// </summary>
        private CorrectionResult CreateCaribbeanTransformation(string fieldName, string oldValue, string newValue, 
            string reasoning, string lineText = null, int lineNumber = 0)
        {
            return new CorrectionResult
            {
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                Reasoning = reasoning,
                LineText = lineText,
                LineNumber = lineNumber,
                Success = true,
                Confidence = 0.99, // High confidence for business rule transformations
                CorrectionType = "caribbean_customs_rule",
                RequiresMultilineRegex = false
            };
        }

        /// <summary>
        /// Applies a single Caribbean customs correction to the invoice.
        /// </summary>
        private void ApplySingleCaribbeanCorrection(ShipmentInvoice invoice, CorrectionResult correction)
        {
            switch (correction.FieldName)
            {
                case "TotalInsurance":
                    if (double.TryParse(correction.NewValue, out var insuranceValue))
                        invoice.TotalInsurance = insuranceValue;
                    break;
                    
                case "TotalDeduction":
                    if (double.TryParse(correction.NewValue, out var deductionValue))
                        invoice.TotalDeduction = deductionValue;
                    break;
                    
                default:
                    _logger.Warning("🏴‍☠️ **CARIBBEAN_UNKNOWN_FIELD**: Unknown field for Caribbean correction: {FieldName}", correction.FieldName);
                    break;
            }
        }

        #endregion
    }
}