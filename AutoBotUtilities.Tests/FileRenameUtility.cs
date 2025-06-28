using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Serilog;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Utility to rename PDF files with problematic characters to clean, standardized names
    /// This ensures smooth processing in the diagnostic pipeline
    /// </summary>
    [TestFixture]
    public class FileRenameUtility
    {
        private static ILogger _logger;
        private const string TestDataPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data";

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.NUnitOutput()
                .CreateLogger();

            _logger = Log.ForContext<FileRenameUtility>();
            _logger.Information("🔧 **FILE_RENAME_UTILITY_INITIALIZED**: Preparing to clean PDF filenames");
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Rename all PDF files to clean, standardized names for smooth processing
        /// </summary>
        [Test]
        public void RenameAllPDFFilesToCleanNames()
        {
            _logger.Information("🧹 **FILENAME_CLEANUP_START**: Renaming PDF files with problematic characters");

            if (!Directory.Exists(TestDataPath))
            {
                _logger.Error("❌ **DIRECTORY_NOT_FOUND**: Test Data directory does not exist: {Path}", TestDataPath);
                Assert.Fail($"Test Data directory not found: {TestDataPath}");
            }

            // Get all PDF files
            var pdfFiles = Directory.GetFiles(TestDataPath, "*.pdf", SearchOption.TopDirectoryOnly);
            _logger.Information("📋 **PDF_INVENTORY**: Found {Count} PDF files to process", pdfFiles.Length);

            var renameCount = 0;
            var errorCount = 0;

            foreach (var pdfFile in pdfFiles)
            {
                try
                {
                    var originalName = Path.GetFileName(pdfFile);
                    var cleanName = GenerateCleanFilename(originalName);

                    if (originalName != cleanName)
                    {
                        var newPath = Path.Combine(TestDataPath, cleanName);
                        
                        // Check if target already exists
                        if (File.Exists(newPath))
                        {
                            _logger.Warning("⚠️ **TARGET_EXISTS**: Skipping {Original} -> {Clean} (target exists)", 
                                originalName, cleanName);
                            continue;
                        }

                        // Rename the PDF file
                        File.Move(pdfFile, newPath);
                        renameCount++;
                        
                        _logger.Information("✅ **PDF_RENAMED**: {Original} -> {Clean}", originalName, cleanName);

                        // Also rename corresponding .txt file if it exists
                        var originalTxtFile = pdfFile + ".txt";
                        if (File.Exists(originalTxtFile))
                        {
                            var newTxtPath = newPath + ".txt";
                            File.Move(originalTxtFile, newTxtPath);
                            _logger.Information("✅ **TXT_RENAMED**: {Original}.txt -> {Clean}.txt", originalName, cleanName);
                        }
                    }
                    else
                    {
                        _logger.Information("✓ **ALREADY_CLEAN**: {FileName}", originalName);
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.Error(ex, "❌ **RENAME_ERROR**: Failed to rename {FileName}", Path.GetFileName(pdfFile));
                }
            }

            _logger.Information("🎯 **CLEANUP_COMPLETE**: Renamed {RenameCount} files, {ErrorCount} errors", 
                renameCount, errorCount);

            if (errorCount > 0)
            {
                Assert.Fail($"Failed to rename {errorCount} files");
            }
        }

        /// <summary>
        /// Generate a clean, standardized filename from a problematic one
        /// </summary>
        private string GenerateCleanFilename(string originalFilename)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilename);
            var extension = Path.GetExtension(originalFilename);

            // Step 1: Handle known problematic patterns
            var cleanName = nameWithoutExtension;

            // Amazon file special case - extract key identifiers
            if (cleanName.Contains("03142025_7_24_24"))
            {
                cleanName = "Amazon_03142025_Order";
            }
            // Other date-based files
            else if (Regex.IsMatch(cleanName, @"^\d{8}"))
            {
                // Keep date prefix, clean the rest
                var dateMatch = Regex.Match(cleanName, @"^(\d{8})(.*)");
                if (dateMatch.Success)
                {
                    var datePart = dateMatch.Groups[1].Value;
                    var descPart = dateMatch.Groups[2].Value;
                    
                    // Clean the description part
                    descPart = CleanDescriptionPart(descPart);
                    cleanName = datePart + descPart;
                }
            }

            // Step 2: Remove or replace problematic characters
            cleanName = CleanGeneralCharacters(cleanName);

            // Step 3: Ensure no double underscores and clean up
            cleanName = Regex.Replace(cleanName, @"_{2,}", "_"); // Replace multiple underscores with single
            cleanName = cleanName.Trim('_'); // Remove leading/trailing underscores

            // Step 4: Ensure reasonable length
            if (cleanName.Length > 100)
            {
                cleanName = cleanName.Substring(0, 100).TrimEnd('_');
            }

            return cleanName + extension;
        }

        /// <summary>
        /// Clean the description part of a filename
        /// </summary>
        private string CleanDescriptionPart(string description)
        {
            if (string.IsNullOrEmpty(description))
                return "";

            // Remove common prefixes
            description = description.TrimStart('_', '-', ' ');

            // Extract meaningful parts
            if (description.Contains("USD") && description.Contains("XCD"))
            {
                return "_USD_XCD_Exchange";
            }
            else if (description.Contains("$") && Regex.IsMatch(description, @"\$[\d,]+\.?\d*"))
            {
                var amountMatch = Regex.Match(description, @"\$(\d+(?:,\d{3})*(?:\.\d{2})?)");
                if (amountMatch.Success)
                {
                    var amount = amountMatch.Groups[1].Value.Replace(",", "");
                    return $"_Amount_{amount}";
                }
            }
            else if (description.ToLower().Contains("invoice"))
            {
                return "_Invoice";
            }
            else if (description.ToLower().Contains("receipt"))
            {
                return "_Receipt";
            }
            else if (description.ToLower().Contains("order"))
            {
                return "_Order";
            }

            // Fallback: clean general characters
            return "_" + CleanGeneralCharacters(description).Trim('_');
        }

        /// <summary>
        /// Clean general problematic characters from text
        /// </summary>
        private string CleanGeneralCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            // Replace problematic characters with underscores or remove them
            text = Regex.Replace(text, @"[,\s'""`:;!@#\$%\^&\*\(\)\+\=\[\]\{\}\|\\/<>\?]", "_");
            
            // Remove special Unicode characters and control characters
            text = Regex.Replace(text, @"[^\w\-_\.]", "_");
            
            // Clean up multiple consecutive underscores
            text = Regex.Replace(text, @"_{2,}", "_");
            
            return text;
        }

        /// <summary>
        /// Preview what renames would happen without actually doing them
        /// </summary>
        [Test]
        public void PreviewFileRenames()
        {
            _logger.Information("👁️ **PREVIEW_MODE**: Showing what renames would happen");

            if (!Directory.Exists(TestDataPath))
            {
                _logger.Error("❌ **DIRECTORY_NOT_FOUND**: Test Data directory does not exist: {Path}", TestDataPath);
                return;
            }

            var pdfFiles = Directory.GetFiles(TestDataPath, "*.pdf", SearchOption.TopDirectoryOnly);
            _logger.Information("📋 **PREVIEW_INVENTORY**: Found {Count} PDF files", pdfFiles.Length);

            foreach (var pdfFile in pdfFiles)
            {
                var originalName = Path.GetFileName(pdfFile);
                var cleanName = GenerateCleanFilename(originalName);

                if (originalName != cleanName)
                {
                    _logger.Information("🔄 **WOULD_RENAME**: {Original} -> {Clean}", originalName, cleanName);
                }
                else
                {
                    _logger.Information("✓ **ALREADY_CLEAN**: {FileName}", originalName);
                }
            }
        }
    }
}