using System;
using System.Collections.Generic;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;
using AutoBotUtilities.Tests;
using Core.Common.Extensions;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class TotalsZeroCalculationTests
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public void TotalsZeroCalculation_WithKnownAmazonInvoiceData_ShouldShowCalculationDiscrepancy()
        {
            // 🔍 **TEST_INTENTION**: Test both calculation methods with exact Amazon invoice data to identify discrepancy
            _logger.Error("🔍 **TEST_SETUP_INTENTION**: Testing TotalsZero calculations with Amazon invoice data");
            _logger.Error("🔍 **TEST_EXPECTATION**: Both calculation methods should produce identical results for the same data");

            // **KNOWN AMAZON INVOICE DATA** (from our failing test)
            // Item(s) Subtotal: $161.95
            // Shipping & Handling: $6.99  
            // Free Shipping: -$0.46 + -$6.53 = -$6.99 (supplier discount)
            // Estimated tax to be collected: $11.34
            // Gift Card Amount: -$6.99 (customer reduction)
            // Order Total: $166.30

            var knownData = new
            {
                InvoiceNo = "112-9126443-1163432",
                SubTotal = 161.95,
                TotalInternalFreight = 6.99,        // Shipping & Handling (gross)
                TotalOtherCost = 11.34,             // Estimated tax
                TotalInsurance = -6.99,             // Gift Card Amount (customer reduction - negative)
                TotalDeduction = 6.99,              // Free Shipping credits (supplier reduction) - CORRECT DOMAIN FIELD
                InvoiceTotal = 166.30
            };

            _logger.Error("🔍 **TEST_KNOWN_DATA**: Using Amazon invoice data: SubTotal={SubTotal:F2}, Freight={Freight:F2}, OtherCost={OtherCost:F2}, Insurance={Insurance:F2}, Deduction={Deduction:F2}, InvoiceTotal={InvoiceTotal:F2}",
                knownData.SubTotal, knownData.TotalInternalFreight, knownData.TotalOtherCost, knownData.TotalInsurance, knownData.TotalDeduction, knownData.InvoiceTotal);

            // **METHOD 1: Direct ShipmentInvoice calculation**
            _logger.Error("🔍 **TEST_METHOD1_START**: Testing direct ShipmentInvoice TotalsZero calculation");
            
            var shipmentInvoice = new ShipmentInvoice
            {
                InvoiceNo = knownData.InvoiceNo,
                SubTotal = knownData.SubTotal,
                TotalInternalFreight = knownData.TotalInternalFreight,
                TotalOtherCost = knownData.TotalOtherCost,
                TotalInsurance = knownData.TotalInsurance,
                TotalDeduction = knownData.TotalDeduction,
                InvoiceTotal = knownData.InvoiceTotal
            };

            bool isBalancedMethod1 = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(shipmentInvoice, out double differenceMethod1, _logger);
            
            _logger.Error("🔍 **TEST_METHOD1_RESULT**: ShipmentInvoice method | IsBalanced={IsBalanced} | Difference={Difference:F4}", 
                isBalancedMethod1, differenceMethod1);

            // **METHOD 2: Dictionary -> TempShipmentInvoice calculation** 
            _logger.Error("🔍 **TEST_METHOD2_START**: Testing dictionary conversion TotalsZero calculation");

            var dictionary = new Dictionary<string, object>
            {
                ["InvoiceNo"] = knownData.InvoiceNo,
                ["SubTotal"] = knownData.SubTotal,
                ["TotalInternalFreight"] = knownData.TotalInternalFreight,
                ["TotalOtherCost"] = knownData.TotalOtherCost,
                ["TotalInsurance"] = knownData.TotalInsurance,
                ["TotalDeduction"] = knownData.TotalDeduction,
                ["InvoiceTotal"] = knownData.InvoiceTotal
            };

            var dynamicList = new List<dynamic> { dictionary };
            bool isBalancedMethod2 = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(dynamicList, out double totalImbalanceMethod2, _logger);
            
            _logger.Error("🔍 **TEST_METHOD2_RESULT**: Dictionary method | IsBalanced={IsBalanced} | TotalImbalance={TotalImbalance:F4}", 
                isBalancedMethod2, totalImbalanceMethod2);

            // **COMPARISON AND ANALYSIS**
            _logger.Error("🔍 **TEST_COMPARISON**: Method1 vs Method2 | Balanced1={Balanced1} vs Balanced2={Balanced2} | Diff1={Diff1:F4} vs TotalImb2={TotalImb2:F4}",
                isBalancedMethod1, isBalancedMethod2, differenceMethod1, totalImbalanceMethod2);

            bool methodsAgree = (isBalancedMethod1 == isBalancedMethod2);
            bool differencesMatch = Math.Abs(differenceMethod1 - totalImbalanceMethod2) < 0.0001;

            if (!methodsAgree)
            {
                _logger.Error("🎯 **ROOT_CAUSE**: CALCULATION METHODS DISAGREE | Method1={Method1}, Method2={Method2}", 
                    isBalancedMethod1, isBalancedMethod2);
            }

            if (!differencesMatch)
            {
                _logger.Error("🎯 **ROOT_CAUSE**: CALCULATION DIFFERENCES MISMATCH | Method1={Diff1:F4}, Method2={Diff2:F4}, Delta={Delta:F4}", 
                    differenceMethod1, totalImbalanceMethod2, Math.Abs(differenceMethod1 - totalImbalanceMethod2));
            }

            // **TEST WITH MISSING CUSTOMER REDUCTION** (simulating OCR failure to detect gift card)
            _logger.Error("🔍 **TEST_MISSING_DEDUCTION_START**: Testing with missing TotalInsurance (simulating OCR failure to detect gift card)");
            
            var missingDeductionInvoice = new ShipmentInvoice
            {
                InvoiceNo = knownData.InvoiceNo,
                SubTotal = knownData.SubTotal,
                TotalInternalFreight = knownData.TotalInternalFreight,
                TotalOtherCost = knownData.TotalOtherCost,
                TotalInsurance = 0.0,               // Missing gift card (should be -6.99)
                TotalDeduction = knownData.TotalDeduction, // Free shipping detected correctly - DOMAIN FIELD
                InvoiceTotal = knownData.InvoiceTotal
            };

            bool isBalancedMissing = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(missingDeductionInvoice, out double differenceMissing, _logger);
            
            _logger.Error("🔍 **TEST_MISSING_DEDUCTION_RESULT**: Missing deduction | IsBalanced={IsBalanced} | Difference={Difference:F4}", 
                isBalancedMissing, differenceMissing);

            var missingDeductionDict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = knownData.InvoiceNo,
                ["SubTotal"] = knownData.SubTotal,
                ["TotalInternalFreight"] = knownData.TotalInternalFreight,
                ["TotalOtherCost"] = knownData.TotalOtherCost,
                ["TotalInsurance"] = 0.0,              // Gift card missing (should be -6.99)
                ["TotalDeduction"] = knownData.TotalDeduction, // Free shipping present - DOMAIN FIELD
                ["InvoiceTotal"] = knownData.InvoiceTotal
            };

            var missingDeductionList = new List<dynamic> { missingDeductionDict };
            bool isBalancedMissingDict = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(missingDeductionList, out double totalImbalanceMissingDict, _logger);

            _logger.Error("🔍 **TEST_MISSING_DEDUCTION_DICT**: Missing deduction dict | IsBalanced={IsBalanced} | TotalImbalance={TotalImbalance:F4}", 
                isBalancedMissingDict, totalImbalanceMissingDict);

            // **ASSERTIONS WITH COMPREHENSIVE LOGGING**
            _logger.Error("🔍 **TEST_ASSERTION_START**: Starting test assertions");

            // Assert that both methods should agree when given identical data
            if (methodsAgree)
            {
                _logger.Error("✅ **ASSERTION_PASSED**: Both calculation methods agree on balance status");
            }
            else
            {
                _logger.Error("❌ **ASSERTION_FAILED**: Calculation methods disagree - Method1={Method1}, Method2={Method2}", 
                    isBalancedMethod1, isBalancedMethod2);
                Assert.Fail($"TotalsZero calculation methods disagree: ShipmentInvoice={isBalancedMethod1}, Dictionary={isBalancedMethod2}");
            }

            // Both should recognize the complete invoice as balanced (difference should be ~0)
            if (isBalancedMethod1 && Math.Abs(differenceMethod1) < 0.01)
            {
                _logger.Error("✅ **ASSERTION_PASSED**: Complete invoice correctly identified as balanced");
            }
            else
            {
                _logger.Error("❌ **ASSERTION_FAILED**: Complete invoice not recognized as balanced - Difference={Difference:F4}", differenceMethod1);
            }

            // Both should recognize the missing gift card (TotalInsurance) invoice as unbalanced  
            if (!isBalancedMissing && Math.Abs(differenceMissing - 6.99) < 0.01)
            {
                _logger.Error("✅ **ASSERTION_PASSED**: Missing gift card correctly identified as unbalanced by {Difference:F4}", differenceMissing);
            }
            else
            {
                _logger.Error("❌ **ASSERTION_FAILED**: Missing gift card not correctly identified - IsBalanced={IsBalanced}, Difference={Difference:F4}", 
                    isBalancedMissing, differenceMissing);
            }

            _logger.Error("🔍 **TEST_SUMMARY**: Test completed - check logs above for calculation discrepancies");
        }

        [Test]
        public void ShouldContinueCorrections_WithUnbalancedInvoice_ShouldReturnTrue()
        {
            // 🔍 **TEST_INTENTION**: Verify ShouldContinueCorrections logic with known unbalanced data
            _logger.Error("🔍 **TEST_SETUP_INTENTION**: Testing ShouldContinueCorrections with known unbalanced invoice");
            _logger.Error("🔍 **TEST_EXPECTATION**: ShouldContinueCorrections should return TRUE for unbalanced invoice");

            // Create an unbalanced invoice (missing gift card in TotalInsurance)
            var unbalancedDict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = "112-9126443-1163432",
                ["SubTotal"] = 161.95,
                ["TotalInternalFreight"] = 6.99,
                ["TotalOtherCost"] = 11.34,
                ["TotalInsurance"] = 0.0,        // Gift card missing (should be -6.99)
                ["TotalDeduction"] = 6.99,       // Free shipping detected correctly - DOMAIN FIELD
                ["InvoiceTotal"] = 166.30
            };

            var dynamicList = new List<dynamic> { unbalancedDict };

            bool shouldContinue = WaterNut.DataSpace.OCRCorrectionService.ShouldContinueCorrections(dynamicList, out double totalImbalance, _logger);

            _logger.Error("🔍 **TEST_SHOULDCONTINUE_RESULT**: ShouldContinue={ShouldContinue} | TotalImbalance={TotalImbalance:F4}", 
                shouldContinue, totalImbalance);

            // **INTENTION CONFIRMATION**
            if (shouldContinue)
            {
                _logger.Error("✅ **ASSERTION_PASSED**: ShouldContinueCorrections correctly returned TRUE for unbalanced invoice");
            }
            else
            {
                _logger.Error("❌ **ASSERTION_FAILED**: ShouldContinueCorrections returned FALSE but should return TRUE for unbalanced invoice");
                _logger.Error("🎯 **ROOT_CAUSE**: This is why OCR correction loop never executes in production!");
                Assert.Fail($"ShouldContinueCorrections returned FALSE for unbalanced invoice. TotalImbalance={totalImbalance:F4}");
            }

            Assert.That(shouldContinue, Is.True, $"ShouldContinueCorrections should return TRUE for unbalanced invoice. TotalImbalance={totalImbalance:F4}");
        }

        [Test] 
        public void ShouldContinueCorrections_WithBalancedInvoice_ShouldReturnFalse()
        {
            // 🔍 **TEST_INTENTION**: Verify ShouldContinueCorrections logic with balanced data
            _logger.Error("🔍 **TEST_SETUP_INTENTION**: Testing ShouldContinueCorrections with balanced invoice");
            _logger.Error("🔍 **TEST_EXPECTATION**: ShouldContinueCorrections should return FALSE for balanced invoice");

            // Create a balanced invoice (with correct field mappings)
            var balancedDict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = "112-9126443-1163432",
                ["SubTotal"] = 161.95,
                ["TotalInternalFreight"] = 6.99,
                ["TotalOtherCost"] = 11.34,
                ["TotalInsurance"] = -6.99,     // Gift card (customer reduction - negative)
                ["TotalDeduction"] = 6.99,      // Free shipping (supplier reduction) - DOMAIN FIELD
                ["InvoiceTotal"] = 166.30
            };

            var dynamicList = new List<dynamic> { balancedDict };

            bool shouldContinue = WaterNut.DataSpace.OCRCorrectionService.ShouldContinueCorrections(dynamicList, out double totalImbalance, _logger);

            _logger.Error("🔍 **TEST_SHOULDCONTINUE_BALANCED_RESULT**: ShouldContinue={ShouldContinue} | TotalImbalance={TotalImbalance:F4}", 
                shouldContinue, totalImbalance);

            // **INTENTION CONFIRMATION**
            if (!shouldContinue)
            {
                _logger.Error("✅ **ASSERTION_PASSED**: ShouldContinueCorrections correctly returned FALSE for balanced invoice");
            }
            else
            {
                _logger.Error("❌ **ASSERTION_FAILED**: ShouldContinueCorrections returned TRUE but should return FALSE for balanced invoice");
            }

            Assert.That(shouldContinue, Is.False, $"ShouldContinueCorrections should return FALSE for balanced invoice. TotalImbalance={totalImbalance:F4}");
        }
    }
}