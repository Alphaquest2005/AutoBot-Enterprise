using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using OCR.Business.Entities;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class QuickDatabaseCheck
    {
        private static ILogger _logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public async Task CheckRecentOCRCorrections()
        {
            _logger.Information("=== CHECKING RECENT OCR CORRECTIONS ===");
            
            using (var ctx = new OCRContext())
            {
                var cutoffTime = DateTime.Now.AddMinutes(-10); // Last 10 minutes
                
                var recentCorrections = ctx.OCRCorrectionLearning
                    .Where(x => x.CreatedDate >= cutoffTime)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(10)
                    .ToList();

                _logger.Error("🔍 **RECENT_CORRECTIONS**: Found {Count} corrections in last 10 minutes", recentCorrections.Count);

                foreach (var correction in recentCorrections)
                {
                    _logger.Error("🔍 **CORRECTION_DETAIL**: ID={Id} | FieldName={FieldName} | CorrectValue={CorrectValue} | CreatedDate={CreatedDate} | FilePath={FilePath}", 
                        correction.Id, correction.FieldName, correction.CorrectValue, correction.CreatedDate, correction.FilePath);
                }

                // Check for Amazon-specific corrections
                var amazonCorrections = recentCorrections
                    .Where(x => x.FilePath != null && x.FilePath.Contains("Amazon"))
                    .ToList();

                _logger.Error("🔍 **AMAZON_CORRECTIONS**: Found {Count} Amazon-specific corrections", amazonCorrections.Count);

                foreach (var amazonCorrection in amazonCorrections)
                {
                    _logger.Error("🔍 **AMAZON_DETAIL**: FieldName={FieldName} | CorrectValue={CorrectValue} | Success={Success} | CorrectionType={CorrectionType}", 
                        amazonCorrection.FieldName, amazonCorrection.CorrectValue, amazonCorrection.Success, amazonCorrection.CorrectionType);
                }

                // Pass if we found any corrections or provide detailed failure message
                if (recentCorrections.Any())
                {
                    _logger.Error("✅ **DATABASE_VERIFICATION_PASSED**: Found {Count} recent OCR corrections", recentCorrections.Count);
                }
                else
                {
                    _logger.Error("❌ **DATABASE_VERIFICATION_FAILED**: No recent OCR corrections found - may indicate pipeline issue");
                }
            }
        }
    }
}