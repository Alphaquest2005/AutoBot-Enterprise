using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using OCR.Business.Entities;
using TrackableEntities;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Test that creates preset OCR corrections in the database to verify the write/read cycle
    /// </summary>
    [TestFixture]
    [Category("Database")]
    [Category("PresetData")]
    public class OCRCorrectionService_PresetDatabaseTests
    {
        private static ILogger _logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRPresetDatabaseTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting OCR Preset Database Tests");
        }

        [Test]
        public async Task CreatePresetAmazonOCRCorrections()
        {
            _logger.Information("=== CREATING PRESET AMAZON OCR CORRECTIONS WITH REAL TEMPLATE CONTEXT ===");
            
            using (var ctx = new OCRContext())
            {
                // Clean up any existing test corrections first
                var existingCorrections = ctx.OCRCorrectionLearning
                    .Where(x => x.CreatedBy == "PresetTest" && x.DocumentType == "Amazon")
                    .ToList();
                
                if (existingCorrections.Any())
                {
                    _logger.Information("Removing {Count} existing preset test corrections", existingCorrections.Count);
                    ctx.OCRCorrectionLearning.RemoveRange(existingCorrections);
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }

                // CRITICAL: From template_context_amazon.json, we need to create a NEW line for TotalInsurance
                // because the existing Gift Card line (LineId: 1830, FieldId: 2579) maps to "TotalOtherCost", not "TotalInsurance"
                
                // Create preset Gift Card correction (TotalInsurance = -6.99)
                // NOTE: This needs to create a NEW line because existing line maps to wrong field
                var giftCardCorrection = new OCRCorrectionLearning
                {
                    FieldName = "TotalInsurance", // Caribbean customs requirement
                    OriginalError = "", // Omission
                    CorrectValue = "-6.99",
                    CorrectionType = "omission",
                    Success = true,
                    LineNumber = 35, // From Amazon invoice text
                    LineText = "Gift Card Amount: -$6.99",
                    DocumentType = "Amazon",
                    FilePath = "Amazon.com - Order 112-9126443-1163432.pdf",
                    CreatedDate = DateTime.Now,
                    CreatedBy = "PresetTest",
                    DeepSeekReasoning = "Gift card amount represents customer payment, maps to TotalInsurance (negative value) per Caribbean customs rules",
                    Confidence = 0.95,
                    RequiresMultilineRegex = false,
                    ContextLinesBefore = "Shipping & Handling: $6.99\nFree Shipping: -$0.46\nFree Shipping: -$6.53\nEstimated tax to be collected: $11.34",
                    ContextLinesAfter = "Grand Total: $166.30",
                    // CRITICAL: No LineId/PartId/RegexId specified because this should create NEW entities
                    LineId = null, // Will trigger creation of new line
                    PartId = 1028, // Use existing Amazon part from template context  
                    RegexId = null, // Will create new regex
                    TrackingState = TrackingState.Added
                };

                // Create preset Free Shipping correction (TotalDeduction = 6.99)  
                // NOTE: This can REUSE existing line because FieldId 2580 already maps to "TotalDeduction"
                var freeShippingCorrection = new OCRCorrectionLearning
                {
                    FieldName = "TotalDeduction", // Already correctly mapped in existing template
                    OriginalError = "", // Omission
                    CorrectValue = "6.99",
                    CorrectionType = "omission", 
                    Success = true,
                    LineNumber = 32, // From Amazon invoice text
                    LineText = "Free Shipping: -$0.46\nFree Shipping: -$6.53",
                    DocumentType = "Amazon",
                    FilePath = "Amazon.com - Order 112-9126443-1163432.pdf",
                    CreatedDate = DateTime.Now,
                    CreatedBy = "PresetTest",
                    DeepSeekReasoning = "Free shipping represents supplier cost reduction, maps to TotalDeduction (positive value) per existing template",
                    Confidence = 0.90,
                    RequiresMultilineRegex = true,
                    ContextLinesBefore = "Item(s) Subtotal: $161.95\nShipping & Handling: $6.99",
                    ContextLinesAfter = "Estimated tax to be collected: $11.34\nGift Card Amount: -$6.99",
                    // CRITICAL: Use existing template context data for Free Shipping
                    LineId = 1831, // Existing FreeShipping line from template
                    PartId = 1028, // Amazon part from template context
                    RegexId = 2031, // Existing FreeShipping regex from template
                    TrackingState = TrackingState.Added
                };

                // Add to context
                ctx.OCRCorrectionLearning.Add(giftCardCorrection);
                ctx.OCRCorrectionLearning.Add(freeShippingCorrection);

                _logger.Error("🔍 **PRESET_CORRECTIONS_ADDED**: Adding 2 preset corrections to database");

                // Save changes
                await ctx.SaveChangesAsync().ConfigureAwait(false);

                _logger.Error("🔍 **PRESET_CORRECTIONS_SAVED**: Gift Card ID={GiftCardId}, Free Shipping ID={FreeShippingId}", 
                    giftCardCorrection.Id, freeShippingCorrection.Id);

                // Verify they were saved correctly
                var savedCorrections = ctx.OCRCorrectionLearning
                    .Where(x => x.CreatedBy == "PresetTest" && x.DocumentType == "Amazon")
                    .ToList();

                Assert.That(savedCorrections.Count, Is.EqualTo(2), "Should have saved 2 preset corrections");

                var savedGiftCard = savedCorrections.FirstOrDefault(x => x.FieldName == "TotalInsurance");
                var savedFreeShipping = savedCorrections.FirstOrDefault(x => x.FieldName == "TotalDeduction");

                Assert.That(savedGiftCard, Is.Not.Null, "Gift card correction should be saved");
                Assert.That(savedGiftCard.CorrectValue, Is.EqualTo("-6.99"), "Gift card value should be -6.99");

                Assert.That(savedFreeShipping, Is.Not.Null, "Free shipping correction should be saved");
                Assert.That(savedFreeShipping.CorrectValue, Is.EqualTo("6.99"), "Free shipping value should be 6.99");

                _logger.Error("✅ **PRESET_TEST_PASSED**: Successfully created and verified 2 Amazon OCR corrections in database");
                _logger.Error("🔍 **PRESET_DETAILS**: Gift Card: TotalInsurance=-6.99, Free Shipping: TotalDeduction=6.99");
            }
        }

        [Test]
        public async Task VerifyPresetCorrectionsCanBeRetrieved()
        {
            _logger.Information("=== VERIFYING PRESET CORRECTIONS CAN BE RETRIEVED ===");
            
            using (var ctx = new OCRContext())
            {
                var corrections = ctx.OCRCorrectionLearning
                    .Where(x => x.CreatedBy == "PresetTest" && x.DocumentType == "Amazon")
                    .OrderBy(x => x.FieldName)
                    .ToList();

                _logger.Error("🔍 **PRESET_RETRIEVAL**: Found {Count} preset corrections", corrections.Count);

                foreach (var correction in corrections)
                {
                    _logger.Error("🔍 **PRESET_CORRECTION**: FieldName={FieldName} | CorrectValue={CorrectValue} | CorrectionType={CorrectionType} | Success={Success} | CreatedDate={CreatedDate}", 
                        correction.FieldName, correction.CorrectValue, correction.CorrectionType, correction.Success, correction.CreatedDate);
                }

                // Verify we can find specific corrections
                var giftCardCorrection = corrections.FirstOrDefault(x => x.FieldName == "TotalInsurance" && x.CorrectValue == "-6.99");
                var freeShippingCorrection = corrections.FirstOrDefault(x => x.FieldName == "TotalDeduction" && x.CorrectValue == "6.99");

                Assert.That(giftCardCorrection, Is.Not.Null, "Should be able to retrieve gift card correction");
                Assert.That(freeShippingCorrection, Is.Not.Null, "Should be able to retrieve free shipping correction");

                _logger.Error("✅ **PRESET_RETRIEVAL_PASSED**: Successfully retrieved preset corrections from database");
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            // Clean up test data after each test
            using (var ctx = new OCRContext())
            {
                var testCorrections = ctx.OCRCorrectionLearning
                    .Where(x => x.CreatedBy == "PresetTest")
                    .ToList();

                if (testCorrections.Any())
                {
                    _logger.Information("Cleaning up {Count} test corrections", testCorrections.Count);
                    ctx.OCRCorrectionLearning.RemoveRange(testCorrections);
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}