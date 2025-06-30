using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Utils;
using AutoBot;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Batch PDF Text Extraction for AI Prompt Testing
    /// Extracts text from all PDFs in Test Data folder for DeepSeek validation
    /// </summary>
    [TestFixture]
    public class BatchPDFTextExtractor
    {
        private static ILogger _logger;
        private const string TestDataPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data";
        private const string OutputPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Extracted Text";

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // Initialize logging for text extraction
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            _logger = Log.ForContext<BatchPDFTextExtractor>();
            
            // Ensure output directory exists
            Directory.CreateDirectory(OutputPath);
            
            _logger.Information("🧠 **BATCH_EXTRACTION_MANDATE**: Operating under Assertive Self-Documenting Logging Mandate v4.1");
            _logger.Information("📊 **EXTRACTION_SETUP**: Batch PDF text extraction initialized");
            _logger.Information("📂 **INPUT_PATH**: {TestDataPath}", TestDataPath);
            _logger.Information("📁 **OUTPUT_PATH**: {OutputPath}", OutputPath);
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Extract text from all PDFs in Test Data folder
        /// Creates .txt files for each PDF using InvoiceReader pipeline
        /// </summary>
        [Test]
        public async Task ExtractTextFromAllPDFs()
        {
            var extractionStartTime = DateTime.Now;
            
            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                _logger.Information("🔍 **BATCH_EXTRACTION_START**: Beginning text extraction from all PDFs");
                
                // Get all PDF files
                var pdfFiles = Directory.GetFiles(TestDataPath, "*.pdf", SearchOption.TopDirectoryOnly);
                _logger.Information("📋 **PDF_INVENTORY**: Found {PdfCount} PDF files for text extraction", pdfFiles.Length);
                
                var results = new List<(string PdfPath, string TextPath, bool Success, string Error)>();
                int successCount = 0;
                int failureCount = 0;
                
                foreach (var pdfPath in pdfFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(pdfPath);
                    var textPath = Path.Combine(OutputPath, $"{fileName}.txt");
                    
                    _logger.Information("🔧 **FILE_PROCESSING_START**: {FileName}", fileName);
                    
                    try
                    {
                        // Check if text file already exists
                        if (File.Exists(textPath))
                        {
                            _logger.Information("✅ **TEXT_EXISTS**: {FileName}.txt already exists, skipping extraction", fileName);
                            results.Add((pdfPath, textPath, true, "Already exists"));
                            successCount++;
                            continue;
                        }
                        
                        // Extract text using InvoiceReader pipeline
                        var extractedText = await this.ExtractTextFromPDF(pdfPath).ConfigureAwait(false);
                        
                        if (!string.IsNullOrEmpty(extractedText))
                        {
                            // Save text to file
                            File.WriteAllText(textPath, extractedText);
                            
                            _logger.Information("✅ **EXTRACTION_SUCCESS**: {FileName} - Text extracted ({TextLength} characters)", 
                                fileName, extractedText.Length);
                            
                            results.Add((pdfPath, textPath, true, null));
                            successCount++;
                        }
                        else
                        {
                            _logger.Error("❌ **EXTRACTION_FAILURE**: {FileName} - No text extracted", fileName);
                            results.Add((pdfPath, textPath, false, "No text extracted"));
                            failureCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "❌ **EXTRACTION_ERROR**: {FileName} - Exception during extraction", fileName);
                        results.Add((pdfPath, textPath, false, ex.Message));
                        failureCount++;
                    }
                }
                
                // Summary report
                var totalTime = DateTime.Now - extractionStartTime;
                _logger.Information("🏁 **BATCH_EXTRACTION_COMPLETE**: Processing completed in {TotalMinutes:F1} minutes", totalTime.TotalMinutes);
                _logger.Information("📊 **EXTRACTION_SUMMARY**: Success: {SuccessCount}, Failures: {FailureCount}, Total: {TotalCount}", 
                    successCount, failureCount, results.Count);
                
                // Detailed results
                foreach (var result in results)
                {
                    var status = result.Success ? "✅ SUCCESS" : "❌ FAILED";
                    var fileName = Path.GetFileName(result.PdfPath);
                    _logger.Information("📋 **RESULT**: {Status} - {FileName} - {Error}", 
                        status, fileName, result.Error ?? "OK");
                }
                
                // Generate inventory file
                await this.GenerateExtractionInventory(results).ConfigureAwait(false);
                
                Assert.That(successCount, Is.GreaterThan(0), $"At least one PDF should be successfully extracted. Got {successCount} successes out of {results.Count}");
            }
        }

        /// <summary>
        /// Extract text from a single PDF using PRODUCTION PDFUtils.ImportPDF pipeline
        /// This ensures we use the exact same process as production
        /// </summary>
        private async Task<string> ExtractTextFromPDF(string pdfPath)
        {
            try
            {
                _logger.Information("🔧 **PRODUCTION_EXTRACTION_START**: Using production PDFUtils.ImportPDF pipeline for {PdfPath}", pdfPath);
                
                // 🎯 **BUSINESS_REQUIREMENT**: Use production code path to ensure consistency
                _logger.Information("💼 **PRODUCTION_RATIONALE**: This matches exactly what happens in production processing");
                
                // Get importable file types (production step 1)
                var fileLst = await FileTypeManager.GetImportableFileType(
                                  FileTypeManager.EntryTypes.Unknown, 
                                  FileTypeManager.FileFormats.PDF, 
                                  pdfPath).ConfigureAwait(false);
                
                var fileTypes = fileLst.OfType<CoreEntities.Business.Entities.FileTypes>().ToList();
                
                if (!fileTypes.Any())
                {
                    _logger.Error("❌ **NO_FILE_TYPE**: No importable file type found for {PdfPath}", pdfPath);
                    return null;
                }
                
                var fileType = fileTypes.First();
                _logger.Information("📋 **FILE_TYPE_RESOLVED**: Using FileType: {FileTypeDescription} (ID: {FileTypeId})", 
                    fileType.Description, fileType.Id);
                
                // Use production ImportPDF method (production step 2)
                var importResult = await PDFUtils.ImportPDF(
                                       new FileInfo[] { new FileInfo(pdfPath) }, 
                                       fileType, 
                                       _logger).ConfigureAwait(false);
                
                _logger.Information("📊 **IMPORT_RESULT**: Production import completed, result count: {ResultCount}", 
                    importResult?.Count ?? 0);
                
                // Look for generated text file (production creates .txt files)
                var textFilePath = pdfPath + ".txt";
                if (File.Exists(textFilePath))
                {
                    var extractedText = File.ReadAllText(textFilePath);
                    _logger.Information("✅ **PRODUCTION_EXTRACTION_SUCCESS**: Found production-generated text file ({TextLength} characters)", 
                        extractedText.Length);
                    return extractedText;
                }
                else
                {
                    _logger.Error("❌ **NO_TEXT_FILE**: Production import did not generate expected text file: {TextFilePath}", textFilePath);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PRODUCTION_EXTRACTION_EXCEPTION**: Error using production pipeline for {PdfPath}", pdfPath);
                return null;
            }
        }

        /// <summary>
        /// Generate inventory file with extraction results
        /// </summary>
        private async Task GenerateExtractionInventory(List<(string PdfPath, string TextPath, bool Success, string Error)> results)
        {
            var inventoryPath = Path.Combine(OutputPath, "extraction_inventory.txt");
            
            var inventoryContent = new List<string>
            {
                "# PDF Text Extraction Inventory",
                $"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                $"# Total Files: {results.Count}",
                $"# Successful: {results.Count(r => r.Success)}",
                $"# Failed: {results.Count(r => !r.Success)}",
                "",
                "# Format: STATUS | PDF_FILE | TEXT_FILE | ERROR",
                ""
            };
            
            foreach (var result in results.OrderBy(r => Path.GetFileName(r.PdfPath)))
            {
                var status = result.Success ? "SUCCESS" : "FAILED";
                var pdfFile = Path.GetFileName(result.PdfPath);
                var textFile = result.Success ? Path.GetFileName(result.TextPath) : "N/A";
                var error = result.Error ?? "OK";
                
                inventoryContent.Add($"{status} | {pdfFile} | {textFile} | {error}");
            }
            
            File.WriteAllLines(inventoryPath, inventoryContent);
            _logger.Information("📋 **INVENTORY_CREATED**: Extraction inventory saved to {InventoryPath}", inventoryPath);
        }

        /// <summary>
        /// Validate extracted text files exist and have content
        /// </summary>
        [Test]
        public void ValidateExtractedTextFiles()
        {
            _logger.Information("🔍 **VALIDATION_START**: Checking extracted text files");
            
            var textFiles = Directory.GetFiles(OutputPath, "*.txt", SearchOption.TopDirectoryOnly)
                .Where(f => !Path.GetFileName(f).StartsWith("extraction_inventory"));
            
            var validFiles = 0;
            var invalidFiles = 0;
            
            foreach (var textFile in textFiles)
            {
                var fileName = Path.GetFileName(textFile);
                var fileInfo = new FileInfo(textFile);
                
                if (fileInfo.Length > 0)
                {
                    var content = File.ReadAllText(textFile);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        _logger.Information("✅ **VALID_TEXT**: {FileName} - {FileSize} bytes", fileName, fileInfo.Length);
                        validFiles++;
                    }
                    else
                    {
                        _logger.Warning("⚠️ **EMPTY_TEXT**: {FileName} - File exists but contains no text", fileName);
                        invalidFiles++;
                    }
                }
                else
                {
                    _logger.Error("❌ **INVALID_TEXT**: {FileName} - File is empty", fileName);
                    invalidFiles++;
                }
            }
            
            _logger.Information("📊 **VALIDATION_SUMMARY**: Valid: {ValidCount}, Invalid: {InvalidCount}, Total: {TotalCount}", 
                validFiles, invalidFiles, validFiles + invalidFiles);
            
            Assert.That(validFiles, Is.GreaterThan(0), "At least one text file should contain valid content");
        }
    }
}