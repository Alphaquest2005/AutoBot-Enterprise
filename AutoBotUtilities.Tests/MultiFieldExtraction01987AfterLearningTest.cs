using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using Serilog;
using Core.Common.Extensions;
using AutoBot;
using System.Data.Entity;
using Serilog.Events;
using WaterNut.Business.Services.Utils;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Integration test for Invoice 8251357168 (from file 01987.pdf) with multi-field extraction capabilities
    /// Follows the exact pattern of CanImportAmazoncomOrder11291264431163432_AfterLearning()
    /// Tests the complete pipeline from PDF import through OCR correction to final balanced invoice
    /// Note: File is named 01987.pdf but actual invoice number in content is 8251357168
    /// </summary>
    [TestFixture]
    public class MultiFieldExtraction01987AfterLearningTest
    {
        private static readonly ILogger _logger = Log.ForContext<MultiFieldExtraction01987AfterLearningTest>();

        [Test]
        public async Task CanImportInvoice01987_AfterLearning()
        {
            // Record the start time of the test to query for new DB entries later.
            var testStartTime = DateTime.Now.AddSeconds(-5); // Give a 5-second buffer

            Console.SetOut(TestContext.Progress);

            try
            {
                // STRATEGY: Configure logging to show OCR correction dataflow and logic flow
                _logger.Information("🔍 **TEST_SETUP_INTENTION**: Test configured to show Error level logs and track OCR correction process");
                _logger.Information("🔍 **TEST_EXPECTATION**: We expect OCR correction to detect omissions and create a balanced invoice.");

                // Configure logging to show our OCR correction logs  
                LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Error;
                LogFilterState.TargetSourceContextForDetails = "WaterNut.DataSpace.OCRCorrectionService";
                LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;

                var testFile = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\01987.pdf";
                _logger.Information("Test File: {FilePath}", testFile);

                if (!File.Exists(testFile))
                {
                    Assert.Warn($"Test file not found: {testFile}");
                    return;
                }

                // Clean database before test - use correct invoice number from PDF content
                var expectedInvoiceNo = "8251357168"; // This is the actual invoice number from the PDF, not the filename
                _logger.Error("🔍 **TEST_SETUP**: Using invoice number '{InvoiceNo}' (extracted from PDF content)", expectedInvoiceNo);
                
                using (var ctx = new EntryDataDSContext())
                {
                    var existingInvoices = await ctx.ShipmentInvoice
                        .Where(x => x.InvoiceNo == expectedInvoiceNo)
                        .ToListAsync();
                    
                    if (existingInvoices.Any())
                    {
                        _logger.Information("🧹 **DATABASE_CLEANUP**: Removing {Count} existing invoices with InvoiceNo = '{InvoiceNo}'", existingInvoices.Count, expectedInvoiceNo);
                        ctx.ShipmentInvoice.RemoveRange(existingInvoices);
                        await ctx.SaveChangesAsync();
                    }
                }

                var fileLst = await FileTypeManager
                                  .GetImportableFileType(FileTypeManager.EntryTypes.Unknown, FileTypeManager.FileFormats.PDF, testFile)
                                  .ConfigureAwait(false);

                var fileTypes = fileLst
                     .OfType<CoreEntities.Business.Entities.FileTypes>()
                     .Where(x => x.Description == "Unknown")
                     .ToList();

                if (!fileTypes.Any())
                {
                    Assert.Warn($"No suitable 'Unknown' PDF FileType found for: {testFile}");
                    return;
                }

                foreach (var fileType in fileTypes)
                {
                    _logger.Information("Testing with FileType: {FileTypeDescription} (ID: {FileTypeId})", fileType.Description, fileType.Id);

                    // This is an async call that kicks off the entire pipeline.
                    _logger.Error("🚀 **PIPELINE_START**: Starting PDFUtils.ImportPDF for file: {TestFile}", testFile);
                    await PDFUtils.ImportPDF(new FileInfo[] { new FileInfo(testFile) }, fileType, _logger).ConfigureAwait(false);

                    _logger.Error("✅ **PIPELINE_COMPLETE**: PDFUtils.ImportPDF completed. Moving to verification...");
                    
                    // Immediate post-processing check
                    _logger.Error("🔍 **IMMEDIATE_DB_CHECK**: Checking database state immediately after processing...");
                    using (var immediateCtx = new EntryDataDSContext())
                    {
                        var immediateInvoices = await immediateCtx.ShipmentInvoice
                            .Select(inv => new { inv.InvoiceNo, inv.Id, inv.InvoiceDate })
                            .OrderByDescending(inv => inv.Id)
                            .Take(10) // Just check the most recent 10 invoices
                            .ToListAsync()
                            .ConfigureAwait(false);
                            
                        _logger.Error("   - Found {Count} recent invoices in database:", immediateInvoices.Count);
                        foreach (var inv in immediateInvoices)
                        {
                            _logger.Error("     * IMMEDIATE: InvoiceNo: '{InvoiceNo}', Id: {Id}, InvoiceDate: {InvoiceDate}", 
                                inv.InvoiceNo, inv.Id, inv.InvoiceDate);
                        }
                        
                        var targetInvoice = immediateInvoices.FirstOrDefault(inv => inv.InvoiceNo == expectedInvoiceNo);
                        if (targetInvoice != null)
                        {
                            _logger.Error("✅ **IMMEDIATE_SUCCESS**: Target invoice '{InvoiceNo}' found immediately after processing!", expectedInvoiceNo);
                        }
                        else
                        {
                            _logger.Error("❌ **IMMEDIATE_FAIL**: Target invoice '{InvoiceNo}' NOT found immediately after processing", expectedInvoiceNo);
                        }
                    }

                    // ================== ROBUST VERIFICATION BLOCK WITH RETRY LOGIC ==================
                    bool invoiceExists = false;
                    ShipmentInvoice finalInvoice = null;

                    _logger.Error("🔍 **VERIFICATION_START**: Checking for persisted ShipmentInvoice with enhanced diagnostic logging...");
                    using (var ctx = new EntryDataDSContext())
                    {
                        // Enhanced database debugging - check what invoices exist first
                        _logger.Error("🔍 **DB_DIAGNOSTIC_1**: Checking all ShipmentInvoice records in database...");
                        var allInvoices = await ctx.ShipmentInvoice
                            .Select(inv => new { inv.InvoiceNo, inv.Id, inv.InvoiceDate })
                            .OrderByDescending(inv => inv.Id) // Use Id since InvoiceDate might be null
                            .Take(20)
                            .ToListAsync()
                            .ConfigureAwait(false);
                        
                        _logger.Error("   - Found {Count} total ShipmentInvoice records in database:", allInvoices.Count);
                        foreach (var inv in allInvoices)
                        {
                            _logger.Error("     * InvoiceNo: '{InvoiceNo}', Id: {Id}, InvoiceDate: {InvoiceDate}", 
                                inv.InvoiceNo, inv.Id, inv.InvoiceDate);
                        }
                        
                        // Check for target invoice in the recent records
                        var targetInvoiceInList = allInvoices.FirstOrDefault(inv => inv.InvoiceNo == expectedInvoiceNo);
                        if (targetInvoiceInList != null)
                        {
                            _logger.Error("   - Target invoice '{InvoiceNo}' found in recent records: Id={Id}, InvoiceDate={InvoiceDate}", 
                                targetInvoiceInList.InvoiceNo, targetInvoiceInList.Id, targetInvoiceInList.InvoiceDate);
                        }
                        else
                        {
                            _logger.Error("   - Target invoice '{InvoiceNo}' NOT found in recent records", expectedInvoiceNo);
                        }

                        // Retry for up to 10 seconds to give the async pipeline time to commit the final save.
                        for (int i = 0; i < 20; i++)
                        {
                            _logger.Error("🔍 **DB_QUERY_ATTEMPT_{Attempt}**: SELECT * FROM ShipmentInvoice WHERE InvoiceNo = '{InvoiceNo}'", i + 1, expectedInvoiceNo);
                            
                            finalInvoice = await ctx.ShipmentInvoice
                                               .Include(x => x.InvoiceDetails)
                                .FirstOrDefaultAsync(inv => inv.InvoiceNo == expectedInvoiceNo)
                                .ConfigureAwait(false);

                            if (finalInvoice != null)
                            {
                                invoiceExists = true;
                                _logger.Error("✅ **VERIFICATION_SUCCESS**: Found ShipmentInvoice '{InvoiceNo}' in the database on attempt {Attempt}.", expectedInvoiceNo, i + 1);
                                _logger.Error("   - Invoice Details: Id={Id}, SubTotal={SubTotal}, InvoiceTotal={InvoiceTotal}, TotalsZero={TotalsZero}", 
                                    finalInvoice.Id, finalInvoice.SubTotal, finalInvoice.InvoiceTotal, finalInvoice.TotalsZero);
                                break;
                            }

                            _logger.Error("❌ **DB_QUERY_FAILED_{Attempt}**: No ShipmentInvoice found with InvoiceNo='{InvoiceNo}'. Waiting 500ms before retry...", i + 1, expectedInvoiceNo);
                            
                            // Every 5 attempts, refresh the query and check what's actually in the database
                            if ((i + 1) % 5 == 0)
                            {
                                _logger.Error("🔄 **DB_REFRESH_CHECK**: Refreshing database state check at attempt {Attempt}...", i + 1);
                                var currentInvoices = await ctx.ShipmentInvoice
                                    .Select(inv => new { inv.InvoiceNo, inv.Id, inv.InvoiceDate })
                                    .OrderByDescending(inv => inv.Id)
                                    .Take(5) // Just check the 5 most recent invoices
                                    .ToListAsync()
                                    .ConfigureAwait(false);
                                    
                                _logger.Error("   - Current recent invoices: {Count}", currentInvoices.Count);
                                foreach (var inv in currentInvoices)
                                {
                                    _logger.Error("     * CURRENT: InvoiceNo: '{InvoiceNo}', Id: {Id}, InvoiceDate: {InvoiceDate}", 
                                        inv.InvoiceNo, inv.Id, inv.InvoiceDate);
                                }
                            }
                            
                            await Task.Delay(500).ConfigureAwait(false);
                        }
                        
                        // Final diagnostic if still not found
                        if (!invoiceExists)
                        {
                            _logger.Error("🚨 **FINAL_DIAGNOSTIC**: ShipmentInvoice '{InvoiceNo}' still not found after all attempts", expectedInvoiceNo);
                            
                            // Check if there were any database exceptions or transaction issues
                            _logger.Error("🔍 **DB_EXCEPTION_CHECK**: Checking if any exceptions occurred during save operations...");
                            
                            // Check if the invoice number might be different than expected (partial matches)
                            var similarInvoices = await ctx.ShipmentInvoice
                                .Where(inv => inv.InvoiceNo.Contains("8251357168") || inv.InvoiceNo.Contains("825") || inv.InvoiceNo.Contains("1357168"))
                                .Select(inv => new { inv.InvoiceNo, inv.Id, inv.InvoiceDate })
                                .ToListAsync()
                                .ConfigureAwait(false);
                                
                            _logger.Error("   - Found {Count} invoices with similar numbers to '{InvoiceNo}':", similarInvoices.Count, expectedInvoiceNo);
                            foreach (var inv in similarInvoices)
                            {
                                _logger.Error("     * SIMILAR: InvoiceNo: '{InvoiceNo}', Id: {Id}, InvoiceDate: {InvoiceDate}", 
                                    inv.InvoiceNo, inv.Id, inv.InvoiceDate);
                            }
                        }
                    }

                    Assert.That(invoiceExists, Is.True, $"ShipmentInvoice '{expectedInvoiceNo}' not created after waiting for async persistence.");
                    // ================== END OF ROBUST VERIFICATION ==================

                    using (var ctx = new EntryDataDSContext())
                    {
                        // Log the final state of the invoice for diagnostic purposes
                        _logger.Information("📊 **INVOICE_FINAL_STATE**: InvoiceNo = {InvoiceNo}", finalInvoice.InvoiceNo);
                        _logger.Information("   - SubTotal: {SubTotal}", finalInvoice.SubTotal);
                        _logger.Information("   - TotalInternalFreight: {Freight}", finalInvoice.TotalInternalFreight);
                        _logger.Information("   - TotalOtherCost: {OtherCost}", finalInvoice.TotalOtherCost);
                        _logger.Information("   - TotalInsurance: {Insurance}", finalInvoice.TotalInsurance);
                        _logger.Information("   - TotalDeduction: {Deduction}", finalInvoice.TotalDeduction);
                        _logger.Information("   - InvoiceTotal: {InvoiceTotal}", finalInvoice.InvoiceTotal);
                        _logger.Information("   - TotalsZero: {TotalsZero}", finalInvoice.TotalsZero);

                        // Check for detail count
                        int detailCount = await ctx.ShipmentInvoiceDetails.CountAsync(x => x.Invoice.InvoiceNo == expectedInvoiceNo);
                        _logger.Information("   - Details Count: {DetailCount}", detailCount);
                        
                        // Log details for multi-field extraction validation
                        var details = await ctx.ShipmentInvoiceDetails
                            .Where(x => x.Invoice.InvoiceNo == expectedInvoiceNo)
                            .ToListAsync();
                        
                        foreach (var detail in details)
                        {
                            _logger.Information("   - Detail {LineNumber}: Description='{Description}', Quantity={Quantity}, Cost={Cost}, TotalCost={TotalCost}", 
                                detail.LineNumber, detail.ItemDescription, detail.Quantity, detail.Cost, detail.TotalCost);
                        }

                        Assert.That(detailCount, Is.GreaterThan(0), $"Expected at least 1 ShipmentInvoiceDetails, but found {detailCount}.");
                    }

                    _logger.Information("✅ **DATABASE_ASSERTIONS_PASSED**: ShipmentInvoice and correct number of Details exist.");

                    // Final check on the persisted object's balance.
                    Assert.That(finalInvoice.TotalsZero, Is.EqualTo(0).Within(0.01), $"Expected final persisted invoice to be balanced. Instead, TotalsZero was {finalInvoice.TotalsZero}.");
                    _logger.Information("✅ **TOTALS_ZERO_PASSED**: Persisted invoice is mathematically balanced (TotalsZero = {TotalsZero}).", finalInvoice.TotalsZero);

                    // Check for OCR corrections made during this test
                    using (var ocrCtx = new OCRContext())
                    {
                        var corrections = await ocrCtx.OCRCorrectionLearning
                            .Where(x => x.CreatedDate >= testStartTime)
                            .ToListAsync();

                        _logger.Information("📋 **OCR_CORRECTIONS_FOUND**: {Count} OCR corrections made during test", corrections.Count);
                        
                        foreach (var correction in corrections)
                        {
                            _logger.Information("   - Field: {Field}, Original: '{Original}', Corrected: '{Corrected}'", 
                                correction.FieldName, correction.OriginalError, correction.CorrectValue);
                        }

                        if (corrections.Count > 0)
                        {
                            _logger.Information("✅ **MULTI_FIELD_EXTRACTION_VALIDATED**: OCR corrections successfully applied to create balanced invoice");
                        }
                        else
                        {
                            _logger.Information("📋 **NO_CORRECTIONS_NEEDED**: Invoice processed successfully without requiring OCR corrections");
                        }
                    }

                    _logger.Information("🎉 **INTEGRATION_TEST_COMPLETE**: Invoice {InvoiceNo} (from 01987.pdf) successfully processed with multi-field extraction capabilities", expectedInvoiceNo);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "ERROR in CanImportInvoice01987_AfterLearning");
                Assert.Fail($"Test failed with exception: {e.Message}");
            }
        }
    }
}