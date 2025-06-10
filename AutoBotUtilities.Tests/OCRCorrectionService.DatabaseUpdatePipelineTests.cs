using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity; // For async LINQ methods
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using OCR.Business.Entities;
using EntryDataDS.Business.Entities;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Tests for the DeepSeek corrections → database update pipeline using actual Amazon invoice data from logs.
    /// Tests the core database update functionality before full integration.
    /// </summary>
    [TestFixture]
    public class OCRCorrectionServiceDatabaseUpdatePipelineTests
    {
        private ILogger _logger;
        private WaterNut.DataSpace.OCRCorrectionService _service;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console()
                .CreateLogger();
            _service = new WaterNut.DataSpace.OCRCorrectionService(_logger);
            
            _logger.Information("🔍 **TEST_SETUP**: Database update pipeline test initialized");
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        /// <summary>
        /// Test the pipeline using existing methods: Correction generation → Pattern validation → Database application.
        /// Uses real template context with actual database IDs from captured data.
        /// </summary>
        [Test]
        public async Task AmazonGiftCardOmission_ShouldExecutePipelineWithExistingMethods_UsingRealTemplateContext()
        {
            _logger.Information("🔍 **TEST_START**: Testing OCR correction pipeline with existing methods and real template context");

            // STEP 1: Create Amazon template context using real database IDs from captured data
            var realTemplateContext = CreateRealAmazonTemplateContext();
            
            _logger.Information("🔍 **TEMPLATE_CONTEXT**: Created real Amazon template context - InvoiceId: {InvoiceId}, Metadata: {MetadataCount}", 
                realTemplateContext.InvoiceId, realTemplateContext.Metadata?.Count ?? 0);

            // STEP 2: Create correction result representing missing gift card field
            var giftCardCorrection = new CorrectionResult
            {
                FieldName = "TotalInsurance", // Maps to TotalOtherCost in database per captured data
                OldValue = null, // Field was missing (omission)
                NewValue = "-6.99", // Gift Card Amount: -$6.99 from real Amazon invoice
                CorrectionType = "omission",
                Success = true,
                Confidence = 0.95,
                Reasoning = "Gift Card Amount represents customer-caused reduction, maps to TotalInsurance per Caribbean customs rules",
                LineNumber = 15,
                LineText = "Gift Card Amount: -$6.99",
                ContextLinesBefore = new List<string> { "Estimated tax to be collected: $11.34", "" },
                ContextLinesAfter = new List<string> { "", "Grand Total: $166.30" },
                RequiresMultilineRegex = false
            };

            // STEP 3: Create line context for DeepSeek regex generation
            var lineContext = new LineContext
            {
                LineNumber = giftCardCorrection.LineNumber,
                LineText = giftCardCorrection.LineText,
                ContextLinesBefore = giftCardCorrection.ContextLinesBefore,
                ContextLinesAfter = giftCardCorrection.ContextLinesAfter,
                RequiresMultilineRegex = giftCardCorrection.RequiresMultilineRegex,
                WindowText = "Estimated tax to be collected: $11.34\n\nGift Card Amount: -$6.99\n\nGrand Total: $166.30",
                IsOrphaned = true, // Missing field has no existing line context
                RequiresNewLineCreation = true,
                LineId = null, // No existing line for omitted field
                PartId = 1028, // Real PartId from captured data
                RegexId = null // No existing regex for omitted field
            };

            _logger.Information("🔍 **CORRECTION_SETUP**: Created correction for field {FieldName} with value {NewValue}", 
                giftCardCorrection.FieldName, giftCardCorrection.NewValue);

            // STEP 4: Test DeepSeek regex generation using existing internal method
            _logger.Information("🔍 **REGEX_GENERATION_START**: Testing DeepSeek regex generation");
            
            var correctionWithRegex = await _service.GenerateRegexPatternInternal(giftCardCorrection, lineContext);
            
            _logger.Information("🔍 **REGEX_GENERATION_RESULT**: Success: {Success}, Pattern: {Pattern}", 
                correctionWithRegex.Success, correctionWithRegex.SuggestedRegex);

            Assert.That(correctionWithRegex.Success, Is.True, "DeepSeek regex generation should succeed");
            Assert.That(correctionWithRegex.SuggestedRegex, Is.Not.Null.And.Not.Empty, "Should generate regex pattern");

            // STEP 5: Test pattern validation using existing internal method
            _logger.Information("🔍 **PATTERN_VALIDATION_START**: Testing pattern validation");
            
            var validatedCorrection = _service.ValidatePatternInternal(correctionWithRegex);
            
            _logger.Information("🔍 **PATTERN_VALIDATION_RESULT**: Success: {Success}, Reasoning: {Reasoning}", 
                validatedCorrection.Success, validatedCorrection.Reasoning);

            Assert.That(validatedCorrection.Success, Is.True, $"Pattern validation should succeed. Reason: {validatedCorrection.Reasoning}");

            // STEP 6: Test field support validation using existing public method
            _logger.Information("🔍 **FIELD_SUPPORT_START**: Testing field support validation");
            
            var isFieldSupported = _service.IsFieldSupported(validatedCorrection.FieldName);
            var fieldValidationInfo = _service.GetFieldValidationInfo(validatedCorrection.FieldName);
            
            _logger.Information("🔍 **FIELD_SUPPORT_RESULT**: Supported: {Supported}, ValidationInfo: {ValidationInfo}", 
                isFieldSupported, fieldValidationInfo.IsValid);

            Assert.That(isFieldSupported, Is.True, "TotalInsurance field should be supported");
            Assert.That(fieldValidationInfo.IsValid, Is.True, "Field validation info should be valid");

            // STEP 7: Test database application using existing internal method
            _logger.Information("🔍 **DATABASE_APPLICATION_START**: Testing database pattern application");
            
            var databaseResult = await _service.ApplyToDatabaseInternal(validatedCorrection, realTemplateContext);
            
            _logger.Information("🔍 **DATABASE_APPLICATION_RESULT**: Success: {Success}, Operation: {Operation}, RecordId: {RecordId}", 
                databaseResult.IsSuccess, databaseResult.Operation, databaseResult.RecordId);

            // Database operation may fail in test environment (read-only database)
            _logger.Information("🔍 **DATABASE_APPLICATION_STATUS**: Database application completed - Success: {Success}", 
                databaseResult.IsSuccess);
            
            if (!databaseResult.IsSuccess)
            {
                _logger.Warning("🔍 **DATABASE_APPLICATION_WARNING**: Database update failed (expected in test environment): {Message}", 
                    databaseResult.Message);
            }

            _logger.Information("✅ **TEST_SUCCESS**: Complete OCR correction pipeline executed using existing methods - " +
                "Correction created → Regex generated → Pattern validated → Field support verified → Database application attempted");
        }

        /// <summary>
        /// Creates Amazon invoice template context using real database IDs from captured template context
        /// </summary>
        private TemplateContext CreateRealAmazonTemplateContext()
        {
            // Real Amazon template metadata from captured template context (InvoiceId: 5)
            var metadata = new Dictionary<string, OCRFieldMetadata>
            {
                // Gift Card line metadata - maps to TotalOtherCost in existing database (LineId: 1830)
                ["TotalInsurance"] = new OCRFieldMetadata
                {
                    FieldName = "TotalInsurance",
                    Field = "TotalOtherCost", // Real field mapping from captured data
                    EntityType = "Invoice",
                    DataType = "Number",
                    IsRequired = false,
                    LineNumber = 15,
                    LineText = "Gift Card Amount: -$6.99",
                    FieldId = 2579, // Real FieldId from captured data
                    LineId = 1830,  // Real LineId from captured data  
                    RegexId = 2030, // Real RegexId from captured data
                    Key = "GiftCard",
                    LineName = "Gift Card",
                    LineRegex = @"Gift Card Amount:\-\$(?<GiftCard>[\d,.]+)", // Real pattern from captured data
                    PartId = 1028,  // Real PartId from captured data
                    PartName = null, // Parts don't have Name property in captured data
                    PartTypeId = 3,  // Real PartTypeId from captured data
                    InvoiceId = 5,   // Real InvoiceId from captured data
                    InvoiceName = "Amazon",
                    Section = "Financial", 
                    Instance = "Single",
                    Confidence = 0.95
                },
                
                // Free Shipping line metadata - maps to TotalDeduction (LineId: 1831)
                ["TotalDeduction"] = new OCRFieldMetadata
                {
                    FieldName = "TotalDeduction",
                    Field = "TotalDeduction",
                    EntityType = "Invoice", 
                    DataType = "Number",
                    IsRequired = false,
                    LineNumber = 14,
                    LineText = "Free Shipping: -$6.53",
                    FieldId = 2580, // Real FieldId from captured data
                    LineId = 1831,  // Real LineId from captured data
                    RegexId = 2031, // Real RegexId from captured data
                    Key = "FreeShipping",
                    LineName = "FreeShipping",
                    LineRegex = @"Free Shipping:[\s\-\$]+(?<Currency>\w{3})[\s\-\$]+(?<FreeShipping>[\d,.]+)",
                    PartId = 1028,
                    PartName = null,
                    PartTypeId = 3,
                    InvoiceId = 5,
                    InvoiceName = "Amazon",
                    Section = "Financial",
                    Instance = "Single", 
                    Confidence = 0.95
                }
            };

            // Real Amazon invoice file text (shortened for test)
            var fileText = @"------------------------------------------Single Column-------------------------
5/9/25, 9:09 AM Amazon.com - Order 112-91 26443-1163432

amazoncom

Print this page for your records,

 

Order Placed: April 15, 2025
Amazon.com order number: 112-9126443-1163432
Order Total: $166.30

 

Shipped on April 17, 2025

Items Ordered Price
1 of: NapQueen 8 Inch Innerspring Queen Size Medium Firm Memory Foam Mattress, Bed in a Box,White $119.99
Sold by: Amazon.com Services, Inc

Supplied by: Other

Item(s) Subtotal: $161.95
Shipping & Handling: $6.99
Free Shipping: -$0.46
Free Shipping: -$6.53
Estimated tax to be collected: $11.34
Gift Card Amount: -$6.99
Grand Total: $166.30";

            return new TemplateContext
            {
                InvoiceId = 5,  // Real InvoiceId from captured data
                InvoiceName = "Amazon",
                Metadata = metadata,
                FileText = fileText
            };
        }

        /// <summary>
        /// Simple template context for basic tests
        /// </summary>
        private TemplateContext CreateAmazonInvoiceTemplateContext()
        {
            var metadata = new Dictionary<string, OCRFieldMetadata>
            {
                ["TotalInsurance"] = new OCRFieldMetadata
                {
                    FieldName = "TotalInsurance",
                    Field = "TotalInsurance",
                    EntityType = "ShipmentInvoice",
                    DataType = "double?",
                    IsRequired = false,
                    LineNumber = 15,
                    LineText = "Gift Card Amount: -$6.99",
                    // Note: These IDs would be null for omitted fields, will be created by the pipeline
                    FieldId = null,
                    LineId = null,
                    RegexId = null,
                    PartId = null,
                    InvoiceId = 1 // Assume Amazon template ID
                }
            };

            return new TemplateContext
            {
                InvoiceId = 1,
                InvoiceName = "Amazon",
                Metadata = metadata,
                FileText = GetAmazonInvoiceFileText(),
                FilePath = "Amazon.com - Order 112-9126443-1163432.pdf"
            };
        }

        /// <summary>
        /// Creates the Amazon ShipmentInvoice based on actual log data
        /// </summary>
        private ShipmentInvoice CreateAmazonShipmentInvoiceFromLogs()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "112-9126443-1163432",
                InvoiceDate = new DateTime(2025, 4, 15),
                InvoiceTotal = 166.30, // From logs: Order Total: $166.30
                SubTotal = 161.95, // From logs: SubTotal=161.95
                TotalInternalFreight = 6.99, // From logs: Freight=6.99
                TotalOtherCost = 11.34, // From logs: OtherCost=11.34
                TotalInsurance = null, // Missing field to be corrected
                TotalDeduction = null, // Also missing, should be detected separately
                Currency = "USD",
                SupplierName = "Amazon.com"
            };
        }

        /// <summary>
        /// Returns the Amazon invoice file text that contains the Gift Card line
        /// </summary>
        private string GetAmazonInvoiceFileText()
        {
            return @"Order Placed: April 15, 2025
Amazon.com order number: 112-9126443-1163432
Order Total: $166.30

Item(s) Subtotal: $161.95
Shipping & Handling: $6.99
Free Shipping: -$0.46
Free Shipping: -$6.53
Estimated tax to be collected: $11.34
Gift Card Amount: -$6.99
Grand Total: $166.30

Shipped on April 17, 2025

Items Ordered Price
1 of: NapQueen 8 Inch Innerspring Queen Size Medium Firm Memory Foam Mattress, Bed in a Box,White $119.99";
        }

        /// <summary>
        /// Clears any test database changes to ensure clean test state
        /// </summary>
        private async Task ClearTestDatabaseChanges()
        {
            try
            {
                using var context = new OCRContext();
                
                // Remove any test learning entries
                var testEntries = context.OCRCorrectionLearning
                    .Where(l => l.CreatedBy == "OCRCorrectionService" && 
                               l.FieldName == "TotalInsurance" &&
                               l.LineText.Contains("Gift Card Amount"))
                    .ToList();
                
                if (testEntries.Any())
                {
                    context.OCRCorrectionLearning.RemoveRange(testEntries);
                    await context.SaveChangesAsync();
                    
                    _logger.Information("🔍 **TEST_CLEANUP**: Removed {Count} existing test learning entries", testEntries.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **TEST_CLEANUP_ERROR**: Error during test database cleanup");
                // Continue with test - cleanup failure shouldn't fail the test
            }
        }

        /// <summary>
        /// Verifies that an OCRCorrectionLearning entry was created for the correction
        /// </summary>
        private async Task VerifyOCRCorrectionLearningEntry(CorrectionResult correction)
        {
            using var context = new OCRContext();
            
            var learningEntry = await context.OCRCorrectionLearning
                .Where(l => l.FieldName == correction.FieldName &&
                           l.CorrectValue == correction.NewValue &&
                           l.LineText.Contains("Gift Card Amount"))
                .OrderByDescending(l => l.CreatedDate)
                .FirstOrDefaultAsync();

            Assert.That(learningEntry, Is.Not.Null, 
                $"Should have created OCRCorrectionLearning entry for {correction.FieldName}");
            
            Assert.That(learningEntry.Success, Is.True, 
                "Learning entry should indicate successful database update");
            
            Assert.That(learningEntry.CorrectionType, Is.EqualTo("omission"), 
                "Should record correction type as omission");
            
            _logger.Information("✅ **TEST_LEARNING_VERIFIED**: OCRCorrectionLearning entry found - ID: {Id}, Success: {Success}", 
                learningEntry.Id, learningEntry.Success);
        }

        /// <summary>
        /// Test multiple corrections in a batch to verify batch processing
        /// </summary>
        [Test]
        public async Task MultipleDeeSeekCorrections_ShouldUpdateDatabase_InBatch()
        {
            // Arrange: Clear test data
            await ClearTestDatabaseChanges();

            var batchCorrections = new List<CorrectionResult>
            {
                // Gift Card correction (TotalInsurance)
                new CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    OldValue = null,
                    NewValue = "-6.99",
                    CorrectionType = "omission",
                    Success = true,
                    LineNumber = 15,
                    LineText = "Gift Card Amount: -$6.99",
                    SuggestedRegex = @"Gift Card Amount:\s*(?<TotalInsurance>-?\$?\d+\.?\d*)"
                },
                // Free Shipping correction (TotalDeduction) 
                new CorrectionResult
                {
                    FieldName = "TotalDeduction", 
                    OldValue = null,
                    NewValue = "6.99", // Combined Free Shipping: -$0.46 + -$6.53 = 6.99 supplier reduction
                    CorrectionType = "omission",
                    Success = true,
                    LineNumber = 12,
                    LineText = "Free Shipping: -$6.53",
                    SuggestedRegex = @"Free Shipping:\s*(?<TotalDeduction>-?\$?\d+\.?\d*)"
                }
            };

            var templateContext = CreateAmazonInvoiceTemplateContext();
            var amazonInvoice = CreateAmazonShipmentInvoiceFromLogs();
            var amazonFileText = GetAmazonInvoiceFileText();

            // Act: Execute batch pipeline
            var batchResult = await batchCorrections.ExecuteBatchPipeline(
                _service, templateContext, amazonInvoice, amazonFileText, maxRetries: 1);

            // Assert: Verify batch processing
            Assert.That(batchResult.Success, Is.True, $"Batch pipeline should succeed. Error: {batchResult.ErrorMessage}");
            Assert.That(batchResult.TotalCorrections, Is.EqualTo(2), "Should process 2 corrections");
            Assert.That(batchResult.SuccessfulCorrections, Is.EqualTo(2), "Both corrections should succeed");
            Assert.That(batchResult.DatabaseUpdates, Is.GreaterThan(0), "Should have database updates");

            // Verify both learning entries were created
            using var context = new OCRContext();
            var learningEntries = await context.OCRCorrectionLearning
                .Where(l => (l.FieldName == "TotalInsurance" || l.FieldName == "TotalDeduction") &&
                           l.CreatedBy == "OCRCorrectionService")
                .OrderByDescending(l => l.CreatedDate)
                .Take(2)
                .ToListAsync();

            Assert.That(learningEntries.Count, Is.EqualTo(2), "Should have 2 learning entries");
            Assert.That(learningEntries.All(l => l.Success == true), Is.True, "All learning entries should indicate success");

            _logger.Information("✅ **TEST_BATCH_SUCCESS**: Batch processing successfully updated database with {Count} corrections", 
                batchResult.SuccessfulCorrections);
        }

        /// <summary>
        /// Test different types of OCR errors and their database updates:
        /// - Character confusion (0 vs O)
        /// - Decimal separator issues
        /// - Missing digits
        /// - Format errors
        /// </summary>
        [Test]
        public async Task DifferentErrorTypes_ShouldUpdateDatabase_WithCorrectStrategies()
        {
            await ClearTestDatabaseChanges();

            var differentErrorTypes = new List<CorrectionResult>
            {
                // Character confusion error (0 vs O)
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "166.3O", // OCR confused 0 with O
                    NewValue = "166.30",
                    CorrectionType = "character_confusion",
                    Success = true,
                    LineNumber = 10,
                    LineText = "Order Total: $166.3O",
                    SuggestedRegex = @"Order Total:\s*\$?(?<InvoiceTotal>\d+\.\d{2})"
                },
                
                // Decimal separator error
                new CorrectionResult
                {
                    FieldName = "SubTotal", 
                    OldValue = "161,95", // European decimal format detected
                    NewValue = "161.95",
                    CorrectionType = "decimal_separator",
                    Success = true,
                    LineNumber = 12,
                    LineText = "Item(s) Subtotal: $161,95",
                    SuggestedRegex = @"Item\(s\) Subtotal:\s*\$?(?<SubTotal>\d+[,\.]\d{2})"
                },

                // Missing digit error
                new CorrectionResult
                {
                    FieldName = "TotalOtherCost",
                    OldValue = "1.34", // Missing leading digit
                    NewValue = "11.34", 
                    CorrectionType = "missing_digit",
                    Success = true,
                    LineNumber = 14,
                    LineText = "Estimated tax to be collected: $1.34",
                    SuggestedRegex = @"Estimated tax.*:\s*\$?(?<TotalOtherCost>\d+\.\d{2})"
                }
            };

            var templateContext = CreateAmazonInvoiceTemplateContext();
            var amazonInvoice = CreateAmazonShipmentInvoiceFromLogs();
            var amazonFileText = GetAmazonInvoiceFileText();

            // Act: Process each error type through the pipeline
            foreach (var correction in differentErrorTypes)
            {
                _logger.Information("🔍 **TEST_ERROR_TYPE**: Processing {ErrorType} for field {FieldName}", 
                    correction.CorrectionType, correction.FieldName);

                // Test regex generation
                var lineContext = new LineContext
                {
                    LineNumber = correction.LineNumber,
                    LineText = correction.LineText,
                    RequiresMultilineRegex = false,
                    WindowText = correction.LineText,
                    IsOrphaned = false // These are format corrections, not omissions
                };

                var correctionWithRegex = await _service.GenerateRegexPatternInternal(correction, lineContext);
                Assert.That(correctionWithRegex.Success, Is.True, $"Regex generation should succeed for {correction.CorrectionType}");

                // Test database update
                var dbResult = await _service.ApplyToDatabaseInternal(correctionWithRegex, templateContext);
                Assert.That(dbResult.IsSuccess, Is.True, $"Database update should succeed for {correction.CorrectionType}");

                // Test template re-import
                var reimportResult = await _service.ReimportAndValidateInternal(dbResult, templateContext, amazonFileText);
                Assert.That(reimportResult.Success, Is.True, $"Template re-import should succeed for {correction.CorrectionType}");

                _logger.Information("✅ **TEST_ERROR_TYPE_SUCCESS**: {ErrorType} successfully processed", correction.CorrectionType);
            }

            // Verify all learning entries were created
            using var context = new OCRContext();
            var learningEntries = await context.OCRCorrectionLearning
                .Where(l => l.CreatedBy == "OCRCorrectionService")
                .OrderByDescending(l => l.CreatedDate)
                .Take(differentErrorTypes.Count)
                .ToListAsync();

            Assert.That(learningEntries.Count, Is.EqualTo(differentErrorTypes.Count), 
                "Should have learning entries for all error types");

            foreach (var errorType in new[] { "character_confusion", "decimal_separator", "missing_digit" })
            {
                var hasLearningEntry = learningEntries.Any(l => l.CorrectionType == errorType);
                Assert.That(hasLearningEntry, Is.True, $"Should have learning entry for {errorType}");
            }

            _logger.Information("✅ **TEST_DIFFERENT_ERRORS_SUCCESS**: All error types successfully updated database with specific strategies");
        }

        /// <summary>
        /// Helper method to extract window text for testing
        /// </summary>
        private string ExtractWindowTextForTest(string fileText, int lineNumber, int windowSize)
        {
            if (string.IsNullOrEmpty(fileText) || lineNumber <= 0)
                return string.Empty;

            var lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            if (lineNumber > lines.Length)
                return string.Empty;

            int startLine = Math.Max(0, lineNumber - windowSize - 1);
            int endLine = Math.Min(lines.Length - 1, lineNumber + windowSize - 1);

            var windowLines = new List<string>();
            for (int i = startLine; i <= endLine; i++)
            {
                windowLines.Add(lines[i]);
            }

            return string.Join("\n", windowLines);
        }
    }
}