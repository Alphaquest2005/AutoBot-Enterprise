using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using WaterNut.DataSpace;
using System.Data.Entity;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Comprehensive tests for OCR correction service database update functionality
    /// Tests the complete workflow from DeepSeek corrections to database persistence
    /// </summary>
    [TestFixture]
    [Category("Database")]
    [Category("Integration")]
    [Category("OCRCorrection")]
    public class OCRCorrectionService_DatabaseUpdateTests
    {
        #region Test Setup and Configuration

        private static ILogger _logger;
        private OCRCorrectionService _service;
        private List<int> _createdRegexIds;
        private List<int> _createdFieldFormatIds;
        private List<int> _createdLineIds;
        private string _testSessionId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testSessionId = $"OCRDBTest_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N[..8]}";

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRDatabaseUpdateTests_{_testSessionId}.log")
                .Enrich.WithProperty("TestSession", _testSessionId)
                .CreateLogger();

            _logger.Information("=== Starting OCR Correction Database Update Tests ===");
            _logger.Information("Test Session ID: {TestSessionId}", _testSessionId);

            // Initialize tracking lists for cleanup
            _createdRegexIds = new List<int>();
            _createdFieldFormatIds = new List<int>();
            _createdLineIds = new List<int>();

            // Verify database connectivity
            VerifyDatabaseConnectivity();
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Created new OCRCorrectionService instance for test: {TestName}",
                TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _service?.Dispose();
                _logger.Information("Disposed OCRCorrectionService instance for test: {TestName}",
                    TestContext.CurrentContext.Test.Name);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error disposing OCRCorrectionService");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            try
            {
                // Clean up test data from database
                CleanupTestData();
                _logger.Information("=== Completed OCR Correction Database Update Tests ===");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during test cleanup");
            }
        }

        #endregion

        #region Database Connectivity and Verification

        [Test]
        [Category("Infrastructure")]
        [Order(1)]
        public void VerifyDatabaseConnection_ShouldConnectSuccessfully()
        {
            // Arrange & Act
            bool connectionSuccessful = false;
            Exception connectionError = null;

            try
            {
                using var ctx = new OCRContext();
                var testQuery = ctx.OCR_RegularExpressions.Take(1).ToList();
                connectionSuccessful = true;
                _logger.Information("✓ Database connection verified successfully");
            }
            catch (Exception ex)
            {
                connectionError = ex;
                _logger.Error(ex, "✗ Database connection failed");
            }

            // Assert
            Assert.That(connectionSuccessful, Is.True,
                $"Database connection should be successful. Error: {connectionError?.Message}");
        }

        [Test]
        [Category("Infrastructure")]
        [Order(2)]
        public void VerifyRequiredTables_ShouldExist()
        {
            // Arrange
            var requiredTables = new[]
            {
                "OCR_RegularExpressions",
                "OCR_FieldFormatRegEx",
                "OCR_Lines",
                "Fields"
            };

            // Act & Assert
            using var ctx = new OCRContext();

            foreach (var tableName in requiredTables)
            {
                bool tableExists = false;
                try
                {
                    switch (tableName)
                    {
                        case "OCR_RegularExpressions":
                            ctx.OCR_RegularExpressions.Take(1).ToList();
                            tableExists = true;
                            break;
                        case "OCR_FieldFormatRegEx":
                            ctx.OCR_FieldFormatRegEx.Take(1).ToList();
                            tableExists = true;
                            break;
                        case "OCR_Lines":
                            ctx.OCR_Lines.Take(1).ToList();
                            tableExists = true;
                            break;
                        case "Fields":
                            ctx.Fields.Take(1).ToList();
                            tableExists = true;
                            break;
                    }
                    _logger.Information("✓ Table {TableName} exists and is accessible", tableName);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "✗ Table {TableName} is not accessible", tableName);
                }

                Assert.That(tableExists, Is.True, $"Required table {tableName} should exist and be accessible");
            }
        }

        #endregion

        #region Test Data Creation and Management

        /// <summary>
        /// Creates test ShipmentInvoice with known OCR errors for testing
        /// </summary>
        private ShipmentInvoice CreateTestInvoiceWithOCRErrors()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = $"TEST-{_testSessionId}-001",
                InvoiceTotal = 123.45m,  // Will simulate OCR reading as "123,45"
                SubTotal = 100.00m,      // Will simulate OCR reading as "1OO.OO"
                TotalInternalFreight = 15.50m,  // Will simulate OCR reading as "15.5O"
                TotalOtherCost = 5.95m,  // Will simulate OCR reading as "5,95"
                TotalInsurance = 2.00m,  // Will simulate OCR reading as "2.OO"
                TotalDeduction = 0.00m,  // Will simulate OCR reading as "O.OO"
                // Add metadata for tracking
                CreatedBy = _testSessionId,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates multiple test invoices with different error patterns
        /// </summary>
        private List<ShipmentInvoice> CreateTestInvoiceCollection()
        {
            return new List<ShipmentInvoice>
            {
                // Invoice 1: Decimal separator errors
                new ShipmentInvoice
                {
                    InvoiceNo = $"TEST-{_testSessionId}-001",
                    InvoiceTotal = 1234.56m,
                    SubTotal = 1000.00m,
                    TotalInternalFreight = 150.50m,
                    TotalOtherCost = 84.06m,
                    CreatedBy = _testSessionId,
                    CreatedDate = DateTime.Now
                },

                // Invoice 2: OCR character confusion (O vs 0)
                new ShipmentInvoice
                {
                    InvoiceNo = $"TEST-{_testSessionId}-002",
                    InvoiceTotal = 500.00m,
                    SubTotal = 450.00m,
                    TotalInternalFreight = 50.00m,
                    CreatedBy = _testSessionId,
                    CreatedDate = DateTime.Now
                },

                // Invoice 3: Negative number format errors
                new ShipmentInvoice
                {
                    InvoiceNo = $"TEST-{_testSessionId}-003",
                    InvoiceTotal = 750.25m,
                    SubTotal = 800.25m,
                    TotalDeduction = 50.00m, // Should be negative in OCR
                    CreatedBy = _testSessionId,
                    CreatedDate = DateTime.Now
                },

                // Invoice 4: Mixed error types
                new ShipmentInvoice
                {
                    InvoiceNo = $"TEST-{_testSessionId}-004",
                    InvoiceTotal = 2500.75m,
                    SubTotal = 2200.00m,
                    TotalInternalFreight = 200.50m,
                    TotalOtherCost = 100.25m,
                    TotalDeduction = 0.00m,
                    CreatedBy = _testSessionId,
                    CreatedDate = DateTime.Now
                }
            };
        }

        /// <summary>
        /// Creates test template with field mappings for metadata extraction
        /// </summary>
        private Invoice CreateTestTemplate()
        {
            var template = new Invoice
            {
                Id = -1, // Temporary ID for testing
                InvoiceNo = $"TEMPLATE-{_testSessionId}",
                CreatedBy = _testSessionId,
                CreatedDate = DateTime.Now
            };

            // Add template lines and fields for testing
            // This will be expanded in subsequent implementations
            return template;
        }

        /// <summary>
        /// Creates test field mappings for metadata extraction
        /// </summary>
        private Dictionary<string, (int LineId, int FieldId)> CreateTestFieldMappings()
        {
            return new Dictionary<string, (int LineId, int FieldId)>
            {
                ["InvoiceTotal"] = (1, 101),
                ["SubTotal"] = (2, 102),
                ["TotalInternalFreight"] = (3, 103),
                ["TotalOtherCost"] = (4, 104),
                ["TotalInsurance"] = (5, 105),
                ["TotalDeduction"] = (6, 106)
            };
        }

        /// <summary>
        /// Creates expected DeepSeek correction results for testing
        /// </summary>
        private List<CorrectionResult> CreateExpectedCorrections()
        {
            return new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "123,45",
                    NewValue = "123.45",
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "FieldFormat",
                    ErrorType = "decimal_separator"
                },
                new CorrectionResult
                {
                    FieldName = "SubTotal",
                    OldValue = "1OO.OO",
                    NewValue = "100.00",
                    Success = true,
                    Confidence = 0.92,
                    CorrectionType = "FieldFormat",
                    ErrorType = "ocr_character_confusion"
                },
                new CorrectionResult
                {
                    FieldName = "TotalInternalFreight",
                    OldValue = "15.5O",
                    NewValue = "15.50",
                    Success = true,
                    Confidence = 0.88,
                    CorrectionType = "FieldFormat",
                    ErrorType = "ocr_character_confusion"
                }
            };
        }

        /// <summary>
        /// Creates comprehensive correction result collection for various error types
        /// </summary>
        private List<CorrectionResult> CreateComprehensiveCorrectionResults()
        {
            return new List<CorrectionResult>
            {
                // Decimal separator corrections
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "1234,56",
                    NewValue = "1234.56",
                    Success = true,
                    Confidence = 0.98,
                    CorrectionType = "FieldFormat",
                    LineNumber = 5,
                    Reasoning = "European decimal separator detected, converted to US format"
                },

                // OCR character confusion (O vs 0)
                new CorrectionResult
                {
                    FieldName = "SubTotal",
                    OldValue = "45O.OO",
                    NewValue = "450.00",
                    Success = true,
                    Confidence = 0.94,
                    CorrectionType = "FieldFormat",
                    LineNumber = 8,
                    Reasoning = "OCR confused letter O with digit 0"
                },

                // Negative number format
                new CorrectionResult
                {
                    FieldName = "TotalDeduction",
                    OldValue = "50.00-",
                    NewValue = "-50.00",
                    Success = true,
                    Confidence = 0.91,
                    CorrectionType = "FieldFormat",
                    LineNumber = 12,
                    Reasoning = "Trailing negative sign moved to prefix"
                },

                // Currency symbol removal
                new CorrectionResult
                {
                    FieldName = "TotalOtherCost",
                    OldValue = "$84.06",
                    NewValue = "84.06",
                    Success = true,
                    Confidence = 0.99,
                    CorrectionType = "FieldFormat",
                    LineNumber = 10,
                    Reasoning = "Removed currency symbol from numeric field"
                },

                // Missing field detection
                new CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    OldValue = "",
                    NewValue = "25.00",
                    Success = true,
                    Confidence = 0.85,
                    CorrectionType = "MissingField",
                    LineNumber = 15,
                    Reasoning = "Insurance amount found in document but not extracted by OCR"
                }
            };
        }

        /// <summary>
        /// Creates mock OCR field metadata for testing
        /// </summary>
        private Dictionary<string, OCRFieldMetadata> CreateMockOCRMetadata()
        {
            return new Dictionary<string, OCRFieldMetadata>
            {
                ["InvoiceTotal"] = new OCRFieldMetadata
                {
                    FieldName = "InvoiceTotal",
                    LineId = 1,
                    FieldId = 101,
                    RegexId = 1001,
                    Value = "123,45",
                    RawValue = "123,45",
                    LineNumber = 5,
                    Section = "Header",
                    Instance = "1"
                },
                ["SubTotal"] = new OCRFieldMetadata
                {
                    FieldName = "SubTotal",
                    LineId = 2,
                    FieldId = 102,
                    RegexId = 1002,
                    Value = "1OO.OO",
                    RawValue = "1OO.OO",
                    LineNumber = 8,
                    Section = "Header",
                    Instance = "1"
                },
                ["TotalInternalFreight"] = new OCRFieldMetadata
                {
                    FieldName = "TotalInternalFreight",
                    LineId = 3,
                    FieldId = 103,
                    RegexId = 1003,
                    Value = "15.5O",
                    RawValue = "15.5O",
                    LineNumber = 10,
                    Section = "Header",
                    Instance = "1"
                },
                ["TotalOtherCost"] = new OCRFieldMetadata
                {
                    FieldName = "TotalOtherCost",
                    LineId = 4,
                    FieldId = 104,
                    RegexId = 1004,
                    Value = "5,95",
                    RawValue = "5,95",
                    LineNumber = 11,
                    Section = "Header",
                    Instance = "1"
                },
                ["TotalInsurance"] = new OCRFieldMetadata
                {
                    FieldName = "TotalInsurance",
                    LineId = 5,
                    FieldId = 105,
                    RegexId = 1005,
                    Value = "2.OO",
                    RawValue = "2.OO",
                    LineNumber = 12,
                    Section = "Header",
                    Instance = "1"
                },
                ["TotalDeduction"] = new OCRFieldMetadata
                {
                    FieldName = "TotalDeduction",
                    LineId = 6,
                    FieldId = 106,
                    RegexId = 1006,
                    Value = "O.OO",
                    RawValue = "O.OO",
                    LineNumber = 13,
                    Section = "Header",
                    Instance = "1"
                }
            };
        }

        #endregion

        #region Database State Verification Methods

        /// <summary>
        /// Verifies that a field format regex entry was created correctly
        /// </summary>
        private async Task<bool> VerifyFieldFormatRegexCreated(string fieldName, string expectedPattern, string expectedReplacement)
        {
            try
            {
                using var ctx = new OCRContext();
                var fieldFormatRegex = await ctx.OCR_FieldFormatRegEx
                    .Include(ffr => ffr.Fields)
                    .Include(ffr => ffr.RegEx)
                    .Include(ffr => ffr.ReplacementRegEx)
                    .Where(ffr => ffr.Fields.Field == fieldName)
                    .OrderByDescending(ffr => ffr.Id)
                    .FirstOrDefaultAsync();

                if (fieldFormatRegex == null)
                {
                    _logger.Warning("No field format regex found for field: {FieldName}", fieldName);
                    return false;
                }

                // Track for cleanup
                _createdFieldFormatIds.Add(fieldFormatRegex.Id);

                var patternMatches = fieldFormatRegex.RegEx?.RegEx?.Contains(expectedPattern) ?? false;
                var replacementMatches = fieldFormatRegex.ReplacementRegEx?.RegEx == expectedReplacement;

                _logger.Information("Field format regex verification for {FieldName}: Pattern={PatternMatch}, Replacement={ReplacementMatch}",
                    fieldName, patternMatches, replacementMatches);

                return patternMatches && replacementMatches;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error verifying field format regex for field: {FieldName}", fieldName);
                return false;
            }
        }

        /// <summary>
        /// Verifies that OCR metadata was extracted correctly
        /// </summary>
        private bool VerifyOCRMetadataExtraction(Dictionary<string, OCRFieldMetadata> metadata, string fieldName, int expectedLineId, int expectedFieldId)
        {
            try
            {
                if (!metadata.ContainsKey(fieldName))
                {
                    _logger.Warning("OCR metadata missing for field: {FieldName}", fieldName);
                    return false;
                }

                var fieldMetadata = metadata[fieldName];
                var lineIdMatches = fieldMetadata.LineId == expectedLineId;
                var fieldIdMatches = fieldMetadata.FieldId == expectedFieldId;

                _logger.Information("OCR metadata verification for {FieldName}: LineId={LineIdMatch} ({Expected}), FieldId={FieldIdMatch} ({ExpectedField})",
                    fieldName, lineIdMatches, expectedLineId, fieldIdMatches, expectedFieldId);

                return lineIdMatches && fieldIdMatches;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error verifying OCR metadata for field: {FieldName}", fieldName);
                return false;
            }
        }

        /// <summary>
        /// Verifies database connectivity during setup
        /// </summary>
        private void VerifyDatabaseConnectivity()
        {
            try
            {
                using var ctx = new OCRContext();
                var testQuery = ctx.RegularExpressions.Take(1).ToList();
                _logger.Information("✓ Database connectivity verified during setup");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "✗ Database connectivity failed during setup");
                throw new InvalidOperationException("Cannot proceed with tests - database connectivity failed", ex);
            }
        }

        /// <summary>
        /// Cleans up test data created during test execution
        /// </summary>
        private void CleanupTestData()
        {
            try
            {
                _logger.Information("Starting cleanup of test data for session: {TestSessionId}", _testSessionId);

                using var ctx = new OCRContext();

                // Clean up field format regex entries
                if (_createdFieldFormatIds.Any())
                {
                    var fieldFormatEntries = ctx.OCR_FieldFormatRegEx
                        .Where(ffr => _createdFieldFormatIds.Contains(ffr.Id))
                        .ToList();

                    ctx.OCR_FieldFormatRegEx.RemoveRange(fieldFormatEntries);
                    _logger.Information("Marked {Count} field format regex entries for deletion", fieldFormatEntries.Count);
                }

                // Clean up regex entries
                if (_createdRegexIds.Any())
                {
                    var regexEntries = ctx.RegularExpressions
                        .Where(re => _createdRegexIds.Contains(re.Id))
                        .ToList();

                    ctx.RegularExpressions.RemoveRange(regexEntries);
                    _logger.Information("Marked {Count} regex entries for deletion", regexEntries.Count);
                }

                // Clean up line entries
                if (_createdLineIds.Any())
                {
                    var lineEntries = ctx.Lines
                        .Where(ol => _createdLineIds.Contains(ol.Id))
                        .ToList();

                    ctx.Lines.RemoveRange(lineEntries);
                    _logger.Information("Marked {Count} line entries for deletion", lineEntries.Count);
                }

                // Note: ShipmentInvoices are not in OCRContext, they would be in EntryDataContext
                // For now, we'll skip cleaning up test invoices as they're not in this context

                // Save all changes
                var deletedCount = ctx.SaveChanges();
                _logger.Information("✓ Successfully cleaned up {Count} test data entries", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during test data cleanup");
                // Don't throw - cleanup errors shouldn't fail the test suite
            }
        }

        /// <summary>
        /// Creates a test correction result for database testing
        /// </summary>
        private CorrectionResult CreateTestCorrection(string fieldName, string oldValue, string newValue,
            string correctionType = "FieldFormat", string errorType = "format_error", double confidence = 0.90)
        {
            return new CorrectionResult
            {
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                Success = true,
                Confidence = confidence,
                CorrectionType = correctionType,
                Reasoning = $"Test correction: {errorType}"
            };
        }

        /// <summary>
        /// Verifies that database changes were persisted correctly
        /// </summary>
        private async Task<bool> VerifyDatabasePersistence(int entityId, string entityType)
        {
            try
            {
                using var ctx = new OCRContext();

                switch (entityType.ToLower())
                {
                    case "regex":
                        var regex = await ctx.RegularExpressions.FindAsync(entityId);
                        return regex != null;

                    case "fieldformat":
                        var fieldFormat = await ctx.OCR_FieldFormatRegEx.FindAsync(entityId);
                        return fieldFormat != null;

                    case "line":
                        var line = await ctx.Lines.FindAsync(entityId);
                        return line != null;

                    default:
                        _logger.Warning("Unknown entity type for persistence verification: {EntityType}", entityType);
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error verifying database persistence for {EntityType} ID {EntityId}", entityType, entityId);
                return false;
            }
        }

        #endregion

        #region Phase 2: Core Database Update Testing

        [Test]
        [Category("DatabaseUpdate")]
        [Category("FieldFormat")]
        [Order(10)]
        public async Task CreateFieldFormatRegexForCorrection_DecimalSeparatorError_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var testInvoice = CreateTestInvoiceWithOCRErrors();
            var fieldMappings = CreateTestFieldMappings();
            var correction = CreateTestCorrection("InvoiceTotal", "123,45", "123.45", "FieldFormat", "decimal_separator", 0.95);

            _logger.Information("Testing field format regex creation for decimal separator correction");

            // Act
            bool correctionApplied = false;
            try
            {
                using var ctx = new OCRContext();

                // Create the correction tuple as expected by the method
                var correctionTuple = (
                    FieldName: correction.FieldName,
                    OldValue: correction.OldValue,
                    NewValue: correction.NewValue,
                    Metadata: new OCRFieldMetadata
                    {
                        LineId = fieldMappings[correction.FieldName].LineId,
                        FieldId = fieldMappings[correction.FieldName].FieldId,
                        RegexId = null,
                        FieldName = correction.FieldName,
                        Value = correction.OldValue,
                        RawValue = correction.OldValue
                    }
                );

                // Call the method under test using reflection to access private method
                var method = typeof(OCRLegacySupport).GetMethod("CreateFieldFormatRegexForCorrection",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                if (method != null)
                {
                    await (Task)method.Invoke(null, new object[] { ctx, correctionTuple, _logger });
                    await ctx.SaveChangesAsync();
                    correctionApplied = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying field format correction");
            }

            // Assert
            Assert.That(correctionApplied, Is.True, "Field format correction should be applied successfully");

            // Verify database entry was created
            var regexCreated = await VerifyFieldFormatRegexCreated("InvoiceTotal", @"(\d+),(\d{2})", "$1.$2");
            Assert.That(regexCreated, Is.True, "Field format regex should be created in database");

            _logger.Information("✓ Decimal separator field format regex created successfully");
        }

        [Test]
        [Category("DatabaseUpdate")]
        [Category("FieldFormat")]
        [Order(11)]
        public async Task CreateFieldFormatRegexForCorrection_OCRCharacterConfusion_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var correction = CreateTestCorrection("SubTotal", "1OO.OO", "100.00", "FieldFormat", "ocr_character_confusion", 0.92);
            var fieldMappings = CreateTestFieldMappings();

            _logger.Information("Testing field format regex creation for OCR character confusion");

            // Act
            bool correctionApplied = false;
            try
            {
                using var ctx = new OCRContext();

                var correctionTuple = (
                    FieldName: correction.FieldName,
                    OldValue: correction.OldValue,
                    NewValue: correction.NewValue,
                    Metadata: new OCRFieldMetadata
                    {
                        LineId = fieldMappings[correction.FieldName].LineId,
                        FieldId = fieldMappings[correction.FieldName].FieldId,
                        RegexId = null,
                        FieldName = correction.FieldName,
                        Value = correction.OldValue,
                        RawValue = correction.OldValue
                    }
                );

                // Use reflection to call private method
                var method = typeof(OCRLegacySupport).GetMethod("CreateFieldFormatRegexForCorrection",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                if (method != null)
                {
                    await (Task)method.Invoke(null, new object[] { ctx, correctionTuple, _logger });
                    await ctx.SaveChangesAsync();
                    correctionApplied = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying OCR character confusion correction");
            }

            // Assert
            Assert.That(correctionApplied, Is.True, "OCR character confusion correction should be applied successfully");

            // Verify database entry was created with character substitution pattern
            var regexCreated = await VerifyFieldFormatRegexCreated("SubTotal", "O", "0");
            Assert.That(regexCreated, Is.True, "OCR character confusion regex should be created in database");

            _logger.Information("✓ OCR character confusion field format regex created successfully");
        }

        [Test]
        [Category("DatabaseUpdate")]
        [Category("FieldFormat")]
        [Order(12)]
        public async Task CreateFieldFormatRegexForCorrection_NegativeNumberFormat_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var correction = CreateTestCorrection("TotalDeduction", "50.00-", "-50.00", "FieldFormat", "negative_format", 0.88);
            var fieldMappings = CreateTestFieldMappings();

            _logger.Information("Testing field format regex creation for negative number format");

            // Act
            bool correctionApplied = false;
            try
            {
                using var ctx = new OCRContext();

                var correctionTuple = (
                    FieldName: correction.FieldName,
                    OldValue: correction.OldValue,
                    NewValue: correction.NewValue,
                    Metadata: new OCRFieldMetadata
                    {
                        LineId = fieldMappings[correction.FieldName].LineId,
                        FieldId = fieldMappings[correction.FieldName].FieldId,
                        RegexId = null,
                        FieldName = correction.FieldName,
                        Value = correction.OldValue,
                        RawValue = correction.OldValue
                    }
                );

                // Use reflection to call private method
                var method = typeof(OCRLegacySupport).GetMethod("CreateFieldFormatRegexForCorrection",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                if (method != null)
                {
                    await (Task)method.Invoke(null, new object[] { ctx, correctionTuple, _logger });
                    await ctx.SaveChangesAsync();
                    correctionApplied = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying negative number format correction");
            }

            // Assert
            Assert.That(correctionApplied, Is.True, "Negative number format correction should be applied successfully");

            // Verify database entry was created with negative number pattern
            var regexCreated = await VerifyFieldFormatRegexCreated("TotalDeduction", @"(\d+\.?\d*)-$", "-$1");
            Assert.That(regexCreated, Is.True, "Negative number format regex should be created in database");

            _logger.Information("✓ Negative number format field format regex created successfully");
        }

        #endregion
    }
}
